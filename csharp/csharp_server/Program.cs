using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace csharp_server
{
    class Program
    {
        static Dictionary<TcpClient, string> cl = new Dictionary<TcpClient, string>(); // connection, client name
        static Boolean ssl = false;
        static Dictionary<SslStream, string> streams = new Dictionary<SslStream, string>(); // connection, client name
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
                }

                if (ssl)
                {
                    byte[] pfxData = File.ReadAllBytes("keyStore.p12");
                    if (File.Exists("cert.cer"))
                        serverCertificate = new X509Certificate2(pfxData);

                    //serverCertificate = X509Certificate.CreateFromCertFile("keyStore.p12");
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
                    ThreadPool.QueueUserWorkItem(PacketProcessor, client);
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
        static void broadcast(string rlMsg)
        {
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(rlMsg + '\0');
            if (ssl)
            {
                foreach (SslStream n in streams.Keys)
                {
                    n.Write(msg);
                }
            }
            else
            {
                foreach (TcpClient n in cl.Keys)
                {
                    NetworkStream tes = n.GetStream();
                    tes.Write(msg, 0, msg.Length);
                }
            }
        }

        private static void ProcessMsg(NetworkStream nStream, SslStream stream, TcpClient tcp, string data)
        {
            if (data.Contains("|")) //a command
            {
                string[] args = new string[data.Length];
                string[] cmd = new string[data.Length];
                cmd = data.Split('|');
                
                byte[] sndMsg = { 0 };

                if (cmd[0].Equals("lm"))
                { //list members
                    if (ssl)
                    {
                        sndMsg = System.Text.Encoding.ASCII.GetBytes(String.Join(",", streams.Values) + '\0');
                        stream.Write(sndMsg);
                    }
                    else
                    {
                        sndMsg = System.Text.Encoding.ASCII.GetBytes(String.Join(",", cl.Values) + '\0');
                        nStream.Write(sndMsg, 0, sndMsg.Length);
                    }
                }

                if (cmd[0].Equals("con"))
                { //connected
                    if (ssl)
                        streams[stream] = cmd[1]; // set client name
                    else
                        cl[tcp] = cmd[1]; // set client name
                    broadcast("Welcome " + cmd[1] + "!");
                }
            }
            else //just a text msg
                broadcast(data);
        }

        private static void PacketProcessor(object obj)
        {
            //Console.WriteLine("[DEBUG] client connection passed to ThreadProc...");
            String data = "";
            var client = (TcpClient)obj;
            SslStream stream = null;
            NetworkStream nStream = null;

            if (ssl)
                stream = new SslStream(client.GetStream(), false);
            else
                nStream = client.GetStream(); //moving on to SSL streams

            cl.Add(client, "");
            int i;

            try
            {
                if (ssl)
                {
                    stream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls, true);
                    streams.Add(stream, "");
                    stream.ReadTimeout = 600000;
                    stream.WriteTimeout = 600000;
                }

                Byte[] bytes = new Byte[8192];

                while ((stream != null && (i = stream.Read(bytes, 0, bytes.Length)) != 0 && ssl) || (nStream != null && (i = nStream.Read(bytes, 0, bytes.Length)) != 0 && !ssl))
                {
                    data += System.Text.Encoding.ASCII.GetString(bytes.ToArray(), 0, i); // convert bytes to string and store
                    if (data.EndsWith("\0")) // check if end of msg (if last char is null)
                    {
                        string[] msg = data.Split('\0'); // split buffer at null terminator(s)
                        for (int t = 0, len = msg.Length; t < len; t++)
                        {
                            //Console.WriteLine("[DEBUG] msg " + t + " length:" + msg[t].Length);
                            if (msg[t].Length > 0) // if not dead end
                            {
                                Console.WriteLine("Received: {0}", msg[t]);
                                ProcessMsg(nStream, stream, client, msg[t]);
                            }
                            else
                                break; // data holder can be full of null chars
                        }
                        data = ""; // clear our data holder
                    }
                }
        }
            catch
            {
                Console.WriteLine("A client has left");
                client.Close();
                if (ssl)
                    streams.Remove(stream);
                else
                    cl.Remove(client);
                if (stream != null)
                    stream.Close();
                if (nStream != null)
                    nStream.Close();
            }
}
    }
}
