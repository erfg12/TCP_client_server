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
        SslStream stream;
        NetworkStream nStream;
        TcpClient client;
        IAsyncResult conResult;
        Byte[] readBuffer = new Byte[8192];
        private static Hashtable certificateErrors = new Hashtable();

        public chatForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ipAddrBox.Text = Properties.Settings.Default.ipAddr;
        }

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public static bool ValidateServerCertificate( object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        //send text from sendTextbox to TcpListener
        private void sndMsg_Click(object sender, EventArgs e)
        {
            try
            {
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
                connectBtn.Text = "connect";
                connectBtn.ForeColor = Color.Green;
            }
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            // connect to the server
            if (connectBtn.Text == "connect")
            {
                Thread newThread = new Thread(Connect);
                newThread.Start();
            }
            else
                EndConnect(conResult);
        }

        private void ipAddrBox_TextChanged(object sender, EventArgs e)
        {
            // save our host for next time use
            Properties.Settings.Default.ipAddr = ipAddrBox.Text;
            Properties.Settings.Default.Save();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // end our connection
            EndConnect(conResult);
        }

        private void membersBtn_Click(object sender, EventArgs e)
        {
            // send command to list connected members
            Byte[] data = System.Text.Encoding.ASCII.GetBytes("lm|" + '\0');

            if (useSSL.Checked)
                stream.Write(data, 0, data.Length);
            else
                nStream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Process incoming packets in a new thread
        /// </summary>
        /// <param name="obj"></param>
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
                                AppndText(msg[t], Color.Blue);
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
                try
                {
                    AppndText("Connection died.", Color.Red);
                    connectBtn.Invoke(new MethodInvoker(delegate
                    {
                        connectBtn.Text = "connect";
                        connectBtn.ForeColor = Color.Green;
                    }));
                }
                catch { }
            }
        }

        /// <summary>
        /// Connect to the server
        /// </summary>
        /// <param name="server">server hostname/ip</param>
        /// <param name="port">server port</param>
        void Connect()
        {
            string server = ipAddrBox.Text;
            string port = portBox.Text;
            try
            {
                AppndText("Attempting to connect to server...", Color.Green);
                // try to connect with timeout of 3 seconds
                client = new TcpClient();
                conResult = client.BeginConnect(server, Convert.ToInt32(port), null, null);

                var success = conResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3));

                if (!success)
                {
                    throw new Exception("Failed to connect.");
                }

                Byte[] data = System.Text.Encoding.ASCII.GetBytes("con|" + nameBox.Text + '\0'); //send data
                connectBtn.Invoke(new MethodInvoker(delegate
                {
                    connectBtn.Text = "disconnect";
                    connectBtn.ForeColor = Color.Red;
                }));

                if (useSSL.Checked)
                {
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
                AppndText("Connected to server!", Color.Green);
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
                connectBtn.Invoke(new MethodInvoker(delegate
                {
                    connectBtn.Text = "connect";
                    connectBtn.ForeColor = Color.Green;
                }));
            }
        }

        /// <summary>
        /// Append text to rich textbox and scroll down
        /// </summary>
        /// <param name="text">text to display in rich textbox</param>
        /// <param name="color">color to display text in</param>
        void AppndText(string text, Color color)
        {
            msgs.Invoke(new MethodInvoker(delegate
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
            }));
        }

        /// <summary>
        /// End our connection with the server properly
        /// </summary>
        /// <param name="ar"></param>
        void EndConnect(IAsyncResult ar)
        {
            try
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
                connectBtn.Text = "connect";
                connectBtn.ForeColor = Color.Green;
                AppndText("Disconnected from server.", Color.Red);
            } catch { }
        }
    }
}
