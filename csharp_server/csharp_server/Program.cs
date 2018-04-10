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
                IPAddress localAddr = IPAddress.Parse("10.1.10.200");
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
            Byte[] bytes = new Byte[2048];

            try
            {
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    //received
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("Received: {0}", data);

                    //send a response to client
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes("I received your message!");
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
