﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csharp_client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        SslStream stream;
        TcpClient client;

        private static Hashtable certificateErrors = new Hashtable();

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public static bool ValidateServerCertificate( object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        //connect to server
        void Connect(String server)
        {
            try
            {
                Int32 port = 13000;
                client = new TcpClient(server, port);

                //stream = client.GetStream();
                stream = new SslStream( client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null );
                // The server name must match the name on the server certificate.
                string serverName = "server";
                try
                {
                    stream.AuthenticateAsClient(serverName);
                }
                catch (AuthenticationException e)
                {
                    Console.WriteLine("Exception: {0}", e.Message);
                    if (e.InnerException != null)
                        Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                    Console.WriteLine("Authentication failed - closing the connection.");
                    client.Close();
                    return;
                }

                Byte[] data = System.Text.Encoding.ASCII.GetBytes(nameBox.Text + " has connected to the server!" + '\0'); //send data
                stream.Write(data, 0, data.Length);
                ThreadPool.QueueUserWorkItem(ThreadProc);
                connectBtn.Enabled = false;
            }
            catch (ArgumentNullException e)
            {
                AppndText("ArgumentNullException:" + e, Color.Red);
            }
            catch (SocketException e)
            {
                AppndText("SocketException:" + e, Color.Red);
            }
            catch
            {
                AppndText("Connection failed", Color.Red);
            }
        }

        //send text with color and scroll textbox down
        void AppndText(string text, Color color)
        {
            if (text.Contains('\0'))
                text = text.Replace("\0", string.Empty);

            msgs.SelectionStart = msgs.TextLength;
            msgs.SelectionLength = 0;

            msgs.SelectionColor = color;
            msgs.AppendText(text + Environment.NewLine);
            msgs.SelectionColor = msgs.ForeColor;

            msgs.SelectionStart = msgs.Text.Length;
            msgs.ScrollToCaret();
        }

        //send text from sendTextbox to TcpListener
        Byte[] readBuffer = new Byte[8192];
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(nameBox.Text + ": " + sendTextbox.Text + '\0'); //send data
                stream.Write(data, 0, data.Length);
            }
            catch
            {
                AppndText("ERROR: not connected to server", Color.Red);
                connectBtn.Enabled = true;
            }
        }

        void ThreadProc(object obj)
        {
            //Console.WriteLine("[DEBUG] client connection passed to ThreadProc...");
            String data = null;
            int i;

            try
            {
                Byte[] bytes = new Byte[8192];
                List<byte> storage = new List<byte>();
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

                    Invoke(new MethodInvoker(delegate
                    {
                        AppndText(data, Color.Blue);
                    }));

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
                Invoke(new MethodInvoker(delegate
                {
                    AppndText("connection died:", Color.Red);
                }));
                connectBtn.Invoke(new MethodInvoker(delegate
                {
                    connectBtn.Enabled = true;
                }));
            }
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            Connect(ipAddrBox.Text);
        }
    }
}
