using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace csharp_client
{
    public partial class chatForm : Form
    {
        public chatForm()
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
        IAsyncResult conResult;

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
                // try to connect with timeout of 1 second
                client = new TcpClient();
                conResult = client.BeginConnect(server, Convert.ToInt32(port), null, null);

                var success = conResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));

                if (!success)
                {
                    throw new Exception("Failed to connect.");
                }

                Byte[] data = System.Text.Encoding.ASCII.GetBytes("con|" + nameBox.Text + '\0'); //send data
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
                    ThreadPool.QueueUserWorkItem(PacketProcessor); //start listening
                    stream.Write(data, 0, data.Length);
                }
                else
                {
                    nStream = client.GetStream();
                    ThreadPool.QueueUserWorkItem(PacketProcessor); //start listening
                    nStream.Write(data, 0, data.Length);
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
                //Byte[] data = System.Text.Encoding.ASCII.GetBytes(nameBox.Text + ": TEST #1" + '\0' + nameBox.Text + ": TEST #2" + '\0' + nameBox.Text + ": TEST #3" + '\0' + nameBox.Text + ": TEST #4" + '\0' + nameBox.Text + ": TEST #5" + '\0'); //big string test
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

        void PacketProcessor(object obj)
        {
            int i = 0;
            String data = "";

            try
            {
                Byte[] bytes = new Byte[8192];

                while ((stream != null && (i = stream.Read(bytes, 0, bytes.Length)) != 0 && useSSL.Checked) || (nStream != null && (i = nStream.Read(bytes, 0, bytes.Length)) != 0 && !useSSL.Checked))
                {
                    data += System.Text.Encoding.ASCII.GetString(bytes.ToArray(), 0, i); // convert bytes to string and store
                    if (data.EndsWith("\0")) // check if end of msg (if last char is null)
                    {
                        string[] msg = data.Split('\0'); // split buffer at null terminator(s)
                        for (int t = 0, len = msg.Length; t < len; t++)
                        {
                            if (msg[t].Length > 0) // if not dead end
                            {
                                Invoke(new MethodInvoker(delegate
                                {
                                    AppndText(msg[t], Color.Blue);
                                }));
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            EndConnect(conResult);
        }

        private void membersBtn_Click(object sender, EventArgs e)
        {
            Byte[] data = System.Text.Encoding.ASCII.GetBytes("lm|" + '\0');

            if (useSSL.Checked)
                stream.Write(data, 0, data.Length);
            else
                nStream.Write(data, 0, data.Length);
        }

        void EndConnect(IAsyncResult ar)
        {
            var state = (State)ar.AsyncState;

            if (!client.Connected /*&& state.Success*/)
                return;

            try
            {
                client.EndConnect(ar);
            }
            catch { }

            client.Close();
        }
    }
}
