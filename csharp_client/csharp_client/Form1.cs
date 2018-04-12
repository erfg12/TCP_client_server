using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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

        NetworkStream stream;

        //connect to server
        void Connect(String server)
        {
            try
            {
                Int32 port = 13000;
                TcpClient client = new TcpClient(server, port);
                stream = client.GetStream();
                AppndText("SERVER: You are now connected, welcome!", Color.Gray);
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
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(sendTextbox.Text); //send data
                stream.Write(data, 0, data.Length);
                AppndText("CLIENT:" + sendTextbox.Text, Color.Green);

                //receive
                String responseData = String.Empty;
                Int32 bytes = stream.Read(readBuffer, 0, readBuffer.Length);
                responseData = System.Text.Encoding.ASCII.GetString(readBuffer, 0, bytes);
                AppndText("SERVER:" + responseData, Color.Blue);
            }
            catch
            {
                AppndText("ERROR: not connected to server", Color.Red);
            }
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            Connect(ipAddrBox.Text);
        }
    }
}
