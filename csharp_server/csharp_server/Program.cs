using System;
using System.Collections.Generic;
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
        static List<SslStream> streams = new List<SslStream>();

        static X509Certificate serverCertificate = null;

        static void Main(string[] args)
        {
            serverCertificate = X509Certificate.CreateFromCertFile("cert.cer");
            TcpListener server = null;
            try
            {
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                if (args.Length > 0)
                    localAddr = IPAddress.Parse(args[0]);
                
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
            foreach (SslStream n in streams)
            {
                n.Write(msg, 0, msg.Length);
            }
        }
        private static void ThreadProc(object obj)
        {
            //Console.WriteLine("[DEBUG] client connection passed to ThreadProc...");
            String data = null;
            var client = (TcpClient)obj;

            SslStream stream = new SslStream(client.GetStream(), false);
            //NetworkStream stream = client.GetStream(); //moving on to SSL streams

            cl.Add(client);
            int i;

            try
            {
                Byte[] bytes = new Byte[8192];
                List<byte> storage = new List<byte>();

                stream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Tls, true);
            // Display the properties and settings for the authenticated stream.
            //DisplaySecurityLevel(sslStream);
            //DisplaySecurityServices(sslStream);
            //DisplayCertificateInformation(sslStream);
            //DisplayStreamProperties(sslStream);

            streams.Add(stream);

                // Set timeouts for the read and write to 5 seconds.
                stream.ReadTimeout = 5000;
                stream.WriteTimeout = 5000;

                //stream.Write(System.Text.Encoding.ASCII.GetBytes("test msg" + '\0')); //a test msg

                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    //received
                    int beforeNull = Array.IndexOf(bytes.Take(i).ToArray(), (byte)0);
                    if (beforeNull >= 0) //done
                    {
                        storage.AddRange(bytes.Take(beforeNull)); //store up to null
                        data = System.Text.Encoding.ASCII.GetString(storage.ToArray(), 0, storage.Count());
                    }
                    else
                    { //maybe client has lag, wait for null char
                        storage.AddRange(bytes.Take(i)); //store all of it.
                        continue;
                    }

                    //for commands in the future
                    string[] args = new string[data.Length];
                    string[] cmd = new string[data.Length];
                    if (data.Contains(Char.MaxValue))
                    {
                        //Console.WriteLine("[DEBUG] Received: {0}", data);
                        cmd = data.Split(Char.MaxValue);
                        if (cmd[1].Contains(","))
                            args = cmd[1].Split(',');
                        else
                            args[0] = cmd[1];
                        continue;
                    }
                    //end cmds

                    //send a response to client
                    Console.WriteLine("Received: {0}", data);

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data + '\0');
                    broadcast(msg);

                    storage.Clear(); //empty storage

                    // if the bytes are greater than beforenull, store the rest
                    if (bytes.Take(i).ToArray().Length - 1 > beforeNull)
                    {
                        Console.WriteLine("[DEBUG] leftover bytes in wire (bytes=" + (bytes.Take(i).ToArray().Length - 1) + " before=" + beforeNull);
                        storage.AddRange(bytes.Skip(i));
                    }
                }
            }
            catch
            {
                Console.WriteLine("client has left");
                client.Close();
            }
        }
    }
}
