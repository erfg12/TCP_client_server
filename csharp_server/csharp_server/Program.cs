using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace csharp_server
{
    class Program
    {
        static void Main(string[] args)
        {
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
        private static void ThreadProc(object obj)
        {
            //Console.WriteLine("[DEBUG] client connection passed to ThreadProc...");
            String data = null;
            var client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            int i;

            try
            {
                Byte[] bytes = new Byte[8192];
                List<byte> storage = new List<byte>();
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    //received
                    storage.AddRange(bytes); //store it.
                    if (Array.IndexOf(bytes, (byte)0) >= 0) //done?
                        data = System.Text.Encoding.ASCII.GetString(storage.ToArray(), 0, storage.Count());
                    else //maybe client has lag, wait for null char
                        continue;

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
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes("I received your message!"); //change this later
                    stream.Write(msg, 0, msg.Length);

                    storage.Clear(); //empty storage
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
