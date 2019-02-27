using System;
using System.Collections;
using System.Collections.Generic;
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
using static System.Windows.Forms.AxHost;

namespace csharp_client
{
    class client_class
    {
        public SslStream stream;
        public NetworkStream nStream;
        public TcpClient client;
        public IAsyncResult conResult;
        public Byte[] readBuffer = new Byte[8192];
        public static Hashtable certificateErrors = new Hashtable();

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        /// <summary>
        /// Process incoming packets in a new thread
        /// </summary>
        /// <param name="obj"></param>
        public void PacketProcessor(RichTextBox msgs, Button connectBtn, bool ssl = false)
        {
            int i = 0;
            String data = "";

            try
            {
                Byte[] bytes = new Byte[8192];

                while ((stream != null && (i = stream.Read(bytes, 0, bytes.Length)) != 0 && ssl) || (nStream != null && (i = nStream.Read(bytes, 0, bytes.Length)) != 0 && !ssl))
                {
                    data += System.Text.Encoding.ASCII.GetString(bytes.ToArray(), 0, i); // convert bytes to string and store
                    if (data.EndsWith("\0")) // check if end of msg (if last char is null)
                    {
                        string[] msg = data.Split('\0'); // split buffer at null terminator(s)
                        for (int t = 0, len = msg.Length; t < len; t++)
                        {
                            if (msg[t].Length > 0) // if not dead end
                            {
                                AppndText(msgs, msg[t], Color.Blue);
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
                    AppndText(msgs, "Connection died.", Color.Red);
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
        public void Connect(string server, string port, RichTextBox msgs, Button connectBtn, string name, bool ssl = false)
        {
            try
            {
                // try to connect with timeout of 3 seconds
                AppndText(msgs, "Attempting to connect to server...", Color.Green);

                client = new TcpClient();
                conResult = client.BeginConnect(server, Convert.ToInt32(port), null, null);
                Byte[] data = Encoding.ASCII.GetBytes("con|" + name + '\0'); //send data

                var success = conResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3));

                if (!success)
                {
                    throw new Exception("Failed to connect.");
                }

                connectBtn.Invoke(new MethodInvoker(delegate
                {
                    connectBtn.Text = "disconnect";
                    connectBtn.ForeColor = Color.Red;
                }));

                if (ssl)
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
                    }
                    var thread = new Thread(() => PacketProcessor(msgs, connectBtn, ssl));
                    thread.Start();
                    stream.Write(data, 0, data.Length);
                }
                else
                {
                    nStream = client.GetStream();
                    var thread = new Thread(() => PacketProcessor(msgs, connectBtn, ssl));
                    thread.Start();
                    nStream.Write(data, 0, data.Length);
                }
            }
            catch (ArgumentNullException e)
            {
                Debug.WriteLine("ArgumentNullException:" + e);
            }
            catch (SocketException e)
            {
                Debug.WriteLine("SocketException:" + e);
            }
            catch
            {
                AppndText(msgs, "Connection failed", Color.Red);
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
        public void AppndText(RichTextBox msgs, string text, Color color)
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
        public void EndConnect(IAsyncResult ar)
        {
            try
            {
                var state = (State)ar.AsyncState;

                if (!client.Connected)
                    return;

                try
                {
                    client.EndConnect(ar);
                }
                catch { }

                client.Close();
            }
            catch { }
        }
    }
}
