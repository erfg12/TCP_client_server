using System;
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
            ipAddrBox.Text = Properties.Settings.Default.ipAddr;
        }
        
        SslStream stream;
        NetworkStream nStream;
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
        void Connect(String server, String port)
        {
            try
            {
                client = new TcpClient(server, Convert.ToInt32(port));

                Byte[] data = System.Text.Encoding.ASCII.GetBytes(nameBox.Text + " has connected to the server!" + '\0'); //send data
                Byte[] listMembers = System.Text.Encoding.ASCII.GetBytes("lm|" + '\0'); //send data
                connectBtn.Enabled = false;

                if (useSSL.Checked) {
                    stream = new SslStream(client.GetStream(), true, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
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
                    ThreadPool.QueueUserWorkItem(ThreadProc); //start listening
                    stream.Write(data, 0, data.Length);
                    stream.Write(listMembers, 0, listMembers.Length);
                }
                else
                {
                    nStream = client.GetStream();
                    ThreadPool.QueueUserWorkItem(ThreadProc); //start listening
                    nStream.Write(data, 0, data.Length);
                    nStream.Write(listMembers, 0, listMembers.Length);
                }
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
        private void sndMsg_Click(object sender, EventArgs e)
        {
            try
            {
                 //if (!backgroundWorker1.IsBusy) //flood test
                 //   backgroundWorker1.RunWorkerAsync();

                    if (sendTextbox.Text.Contains("|"))
                        sendTextbox.Text = sendTextbox.Text.Replace('|', '?');
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes(nameBox.Text + ": " + sendTextbox.Text + '\0'); //send data
                    if (useSSL.Checked)
                        stream.Write(data, 0, data.Length);
                    else
                        nStream.Write(data, 0, data.Length);
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
                while ((stream != null && (i = stream.Read(bytes, 0, bytes.Length)) != 0 && useSSL.Checked) || (nStream != null && (i = nStream.Read(bytes, 0, bytes.Length)) != 0 && !useSSL.Checked))
                {
                    //received
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
                        
                        Invoke(new MethodInvoker(delegate
                        {
                            AppndText(data, Color.Blue);
                        }));
                        storage.Clear(); //empty storage

                        if (beforeNull < 0 || bal == 0) //no more nulls in stream
                        {
                            if (bal > 0)
                                storage.AddRange(bytes.Skip(prevNull).Take(bal)); //store remaining bytes
                            if (useSSL.Checked)
                                stream.Flush();
                            else
                                nStream.Flush();
                            Array.Clear(bytes, 0, bytes.Length);
                            break;
                        }
                    }
                    /*int beforeNull = Array.IndexOf(bytes.Take(i).ToArray(), (byte)0);
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
                    }*/
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
            Connect(ipAddrBox.Text, portBox.Text);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                try
                {
                    if (sendTextbox.Text.Contains("|"))
                        sendTextbox.Text = sendTextbox.Text.Replace('|', '?');
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes(nameBox.Text + ": " + sendTextbox.Text + '\0'); //send data
                    if (useSSL.Checked)
                        stream.Write(data, 0, data.Length);
                    else
                        nStream.Write(data, 0, data.Length);
                }
                catch { }
            }
        }

        private void ipAddrBox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ipAddr = ipAddrBox.Text;
            Properties.Settings.Default.Save();
        }
    }
}
