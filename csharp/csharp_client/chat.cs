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
        client_class c_lass = new client_class();
        public chatForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        //send text from sendTextbox to TcpListener
        private void sndMsg_Click(object sender, EventArgs e)
        {
            if (sendTextbox.Text.Count() < 1)
            {
                MessageBox.Show("You need to type in a message first.");
                return;
            }

            try
            {
                if (sendTextbox.Text.Contains("|"))
                    sendTextbox.Text = sendTextbox.Text.Replace('|', '?');
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(nameBox.Text + ": " + sendTextbox.Text + '\0'); //send data
                //Byte[] data = System.Text.Encoding.ASCII.GetBytes(nameBox.Text + ": TEST #1" + '\0' + nameBox.Text + ": TEST #2" + '\0' + nameBox.Text + ": TEST #3" + '\0' + nameBox.Text + ": TEST #4" + '\0' + nameBox.Text + ": TEST #5" + '\0'); //big string test
                if (useSSL.Checked)
                    c_lass.stream.Write(data, 0, data.Length);
                else
                    c_lass.nStream.Write(data, 0, data.Length);
            }
            catch
            {
                c_lass.AppndText(msgs, "ERROR: not connected to server", Color.Red);
                connectBtn.Text = "connect";
                connectBtn.ForeColor = Color.Green;
            }
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            // connect to the server
            if (connectBtn.Text == "connect")
            {
                c_lass.Connect(ipAddrBox.Text, portBox.Text, msgs, connectBtn, nameBox.Text, useSSL.Checked);
            }
            else
            {
                c_lass.EndConnect(c_lass.conResult);
                connectBtn.Text = "connect";
                connectBtn.ForeColor = Color.Green;
                c_lass.AppndText(msgs, "Disconnected from server.", Color.Red);
            }
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
            c_lass.EndConnect(c_lass.conResult);
            connectBtn.Text = "connect";
            connectBtn.ForeColor = Color.Green;
            c_lass.AppndText(msgs, "Disconnected from server.", Color.Red);
        }

        private void membersBtn_Click(object sender, EventArgs e)
        {
            // send command to list connected members
            if (connectBtn.Text != "disconnect")
                return;

            Byte[] data = System.Text.Encoding.ASCII.GetBytes("lm|" + '\0');

            if (useSSL.Checked)
                c_lass.stream.Write(data, 0, data.Length);
            else
                c_lass.nStream.Write(data, 0, data.Length);
        }

        private void nameBox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.nick = nameBox.Text;
            Properties.Settings.Default.Save();
        }

        private void portBox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.port = portBox.Text;
            Properties.Settings.Default.Save();
        }

        private void useSSL_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ssl = useSSL.Checked;
            Properties.Settings.Default.Save();
        }

        private void chatForm_Shown(object sender, EventArgs e)
        {
            ipAddrBox.Text = Properties.Settings.Default.ipAddr;
            nameBox.Text = Properties.Settings.Default.nick;
            portBox.Text = Properties.Settings.Default.port;
            useSSL.Checked = Properties.Settings.Default.ssl;
        }
    }
}
