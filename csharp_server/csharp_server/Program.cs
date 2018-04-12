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
                server = new TcpListener(localAddr, port);
                server.Start();
               
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
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
            Console.WriteLine("client connection passed to ThreadProc...");
            String data = null;
            var client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            int i;

            try
            {
                Byte[] bytes = new Byte[6]; //start here. Big enough for size string (s|####)
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    //received
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                    string[] args = new string[data.Length];
                    string[] cmd = new string[data.Length];

                    if (data.Contains("|"))
                    {
                        //Console.WriteLine("[DEBUG] Received: {0}", data);
                        cmd = data.Split('|');
                        if (cmd[1].Contains(","))
                            args = cmd[1].Split(',');
                        else
                            args[0] = cmd[1];

                        if (cmd[0].Contains("s"))
                        {
                            if (Convert.ToInt32(args[0]) < 6) continue; //minimum for size string
                            //Console.WriteLine("[DEBUG] changing buffer size to " + args[0]);
                            bytes = new byte[Convert.ToInt32(args[0])];
                        }
                        continue;
                    }

                    //send a response to client
                    Console.WriteLine("Received: {0}", data);
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes("I received your message!"); //change this later
                    stream.Write(Encoding.ASCII.GetBytes(msg.Length.ToString().PadLeft(4, '0')), 0, 4); //send length, always 4 bytes
                    stream.Write(msg, 0, msg.Length);
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
