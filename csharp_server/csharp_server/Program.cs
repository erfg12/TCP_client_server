using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace csharp_server
{
    class Program
    {
        static List<TcpClient> cl = new List<TcpClient>();
        static Boolean ssl = false;
        static List<SslStream> streams = new List<SslStream>();
        //static List<TcpClient> nStreams = new List<TcpClient>();
        static X509Certificate serverCertificate = null;

        static void Main(string[] args)
        {
            TcpListener server = null;
            try
            {
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                if (args.Length > 0)
                    localAddr = IPAddress.Parse(args[0]);
                
                if (args.ElementAtOrDefault(1) != null)
                    port = Convert.ToInt32(args[1]);

                if (args.ElementAtOrDefault(2) != null)
                {
                    if (args[2].ToLower().Equals("ssl") || args[2].ToLower().Equals("true") || args[2].Equals("1"))
                        ssl = true;
                    if (File.Exists("cert.cer"))
                        serverCertificate = X509Certificate.CreateFromCertFile("cert.cer");
                    else
                    {
                        Console.WriteLine("ERROR: cert.cer file missing! Place cert.cer next to csharp_server.exe file.");
                        return;
                    }
                }

                Console.WriteLine("server is listening on IP:" + localAddr.ToString() + " Port:" + port + " SSL:" + ssl);
                
                server = new TcpListener(localAddr, port);
                server.Start();
                
                while (true)
                {
                    //Console.WriteLine("[DEBUG] Waiting for a connection...");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("A client has connected!");
                    ThreadPool.QueueUserWorkItem(ThreadProc, client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }
        }
        static void broadcast(byte[] msg)
        {
            if (ssl)
            {
                foreach (SslStream n in streams)
                {
                    n.Write(msg);
                }
            }
            else
            {
                foreach (TcpClient n in cl)
                {
                    NetworkStream tes = n.GetStream();
                    tes.Write(msg, 0, msg.Length);
                    //tes.ReadTimeout = 600000;
                    //tes.WriteTimeout = 600000;
                }
            }
        }

        private static void ProcessMsg(NetworkStream nStream, SslStream stream, string data)
        {
            if (data.Contains("|")) //a command
            {
                string[] args = new string[data.Length];
                string[] cmd = new string[data.Length];
                cmd = data.Split('|');
                if (cmd[1].Contains(","))
                    args = cmd[1].Split(',');
                else
                    args[0] = cmd[1];

                Console.WriteLine("Received Cmd: {0}", cmd[0]);
                byte[] sndMsg = { 0 };

                if (cmd[0].Equals("lm"))
                { //list members
                    if (ssl)
                        sndMsg = System.Text.Encoding.ASCII.GetBytes(streams.Count().ToString() + " client(s) currently connected." + '\0');
                    else
                    {
                        sndMsg = System.Text.Encoding.ASCII.GetBytes(cl.Count().ToString() + " client(s) currently connected." + '\0');
                    }
                }

                Console.WriteLine("Sending: {0}", System.Text.Encoding.Default.GetString(sndMsg));

                if (ssl)
                    stream.Write(sndMsg);
                else
                {
                    //NetworkStream tes = cl.GetStream();
                    nStream.Write(sndMsg, 0, sndMsg.Length);
                }
            }
            else //just a text msg
            {
                Console.WriteLine("Received Txt: {0}", data);

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                broadcast(msg);
            }
        }

        private static void ThreadProc(object obj)
        {
            //Console.WriteLine("[DEBUG] client connection passed to ThreadProc...");
            String data = null;
            var client = (TcpClient)obj;
            SslStream stream = null;
            NetworkStream nStream = null;

            if (ssl)
                stream = new SslStream(client.GetStream(), false);
            else
                nStream = client.GetStream(); //moving on to SSL streams

            cl.Add(client);
            int i;

            try
            {
                Byte[] bytes = new Byte[8192];
                List<byte> storage = new List<byte>();

                if (ssl)
                {
                    stream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Tls, true);
                    // Display the properties and settings for the authenticated stream.
                    //DisplaySecurityLevel(sslStream);
                    //DisplaySecurityServices(sslStream);
                    //DisplayCertificateInformation(sslStream);
                    //DisplayStreamProperties(sslStream);
                    streams.Add(stream);
                    stream.ReadTimeout = 600000;
                    stream.WriteTimeout = 600000;
                }
                /*else
                {
                    nStreams.Add(client);
                }*/

                while ((stream != null && (i = stream.Read(bytes, 0, bytes.Length)) != 0 && ssl) || (nStream != null && (i = nStream.Read(bytes, 0, bytes.Length)) != 0 && !ssl))
                {
                    int beforeNull = Array.IndexOf(bytes.Take(i).ToArray(), (byte)0) + 1;
                    int bal = i; //byte array length
                    int prevNull = 0;
                    // if the bytes are greater than beforenull, process what's left
                    while (bal >= beforeNull)
                    {
                        //Console.WriteLine("[DEBUG] bytes=" + bal + " beforeNull=" + beforeNull);                        
                        storage.AddRange(bytes.Skip(prevNull).Take(beforeNull)); //store up to null
                        data = System.Text.Encoding.ASCII.GetString(storage.ToArray(), 0, storage.Count()); //converted
                        int tmpStore = beforeNull;
                        beforeNull = Array.IndexOf(bytes.Skip(beforeNull).Take(beforeNull + 1).ToArray(), (byte)0); //another null char in stream?
                        bal = bytes.Skip(tmpStore).Take(beforeNull).ToArray().Length; // whatever is left to process of the streawm
                        prevNull = tmpStore;

                        ProcessMsg(nStream, stream, data);
                        storage.Clear(); //empty storage

                        if (beforeNull < 0 || bal == 0) //no more nulls in stream
                        {
                            if (bal > 0)
                                storage.AddRange(bytes.Skip(prevNull).Take(bal)); //store remaining bytes
                            if (ssl)
                                stream.Flush();
                            else
                                nStream.Flush();
                            Array.Clear(bytes, 0, bytes.Length);
                            break;
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("A client has left");
                client.Close();
                cl.Remove(client);
                if (stream != null)
                    stream.Close();
                if (nStream != null)
                    nStream.Close();
            }
        }
    }
}
