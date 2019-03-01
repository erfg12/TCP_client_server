namespace csharp_client
{
    partial class chatForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.sendBtn = new System.Windows.Forms.Button();
            this.msgs = new System.Windows.Forms.RichTextBox();
            this.sendTextbox = new System.Windows.Forms.TextBox();
            this.ipAddrBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.connectBtn = new System.Windows.Forms.Button();
            this.nameBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.useSSL = new System.Windows.Forms.CheckBox();
            this.portBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.membersBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // sendBtn
            // 
            this.sendBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sendBtn.Location = new System.Drawing.Point(337, 416);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(38, 23);
            this.sendBtn.TabIndex = 6;
            this.sendBtn.Text = "send";
            this.sendBtn.UseVisualStyleBackColor = true;
            this.sendBtn.Click += new System.EventHandler(this.sndMsg_Click);
            // 
            // msgs
            // 
            this.msgs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.msgs.Location = new System.Drawing.Point(12, 37);
            this.msgs.Name = "msgs";
            this.msgs.Size = new System.Drawing.Size(407, 372);
            this.msgs.TabIndex = 2;
            this.msgs.TabStop = false;
            this.msgs.Text = "";
            // 
            // sendTextbox
            // 
            this.sendTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sendTextbox.Location = new System.Drawing.Point(12, 417);
            this.sendTextbox.MaxLength = 2048;
            this.sendTextbox.Name = "sendTextbox";
            this.sendTextbox.Size = new System.Drawing.Size(325, 20);
            this.sendTextbox.TabIndex = 5;
            // 
            // ipAddrBox
            // 
            this.ipAddrBox.Location = new System.Drawing.Point(150, 10);
            this.ipAddrBox.Name = "ipAddrBox";
            this.ipAddrBox.Size = new System.Drawing.Size(98, 20);
            this.ipAddrBox.TabIndex = 1;
            this.ipAddrBox.Text = "127.0.0.1";
            this.ipAddrBox.TextChanged += new System.EventHandler(this.ipAddrBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(130, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "IP:";
            // 
            // connectBtn
            // 
            this.connectBtn.ForeColor = System.Drawing.Color.Green;
            this.connectBtn.Location = new System.Drawing.Point(308, 9);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(67, 23);
            this.connectBtn.TabIndex = 3;
            this.connectBtn.Text = "connect";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            // 
            // nameBox
            // 
            this.nameBox.Location = new System.Drawing.Point(45, 10);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(85, 20);
            this.nameBox.TabIndex = 0;
            this.nameBox.Text = "Beta Tester";
            this.nameBox.TextChanged += new System.EventHandler(this.nameBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "name:";
            // 
            // useSSL
            // 
            this.useSSL.AutoSize = true;
            this.useSSL.Location = new System.Drawing.Point(380, 13);
            this.useSSL.Name = "useSSL";
            this.useSSL.Size = new System.Drawing.Size(46, 17);
            this.useSSL.TabIndex = 4;
            this.useSSL.Text = "SSL";
            this.useSSL.UseVisualStyleBackColor = true;
            this.useSSL.CheckedChanged += new System.EventHandler(this.useSSL_CheckedChanged);
            // 
            // portBox
            // 
            this.portBox.Location = new System.Drawing.Point(258, 10);
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(48, 20);
            this.portBox.TabIndex = 2;
            this.portBox.Text = "13000";
            this.portBox.TextChanged += new System.EventHandler(this.portBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(248, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = ":";
            // 
            // membersBtn
            // 
            this.membersBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.membersBtn.Location = new System.Drawing.Point(376, 416);
            this.membersBtn.Name = "membersBtn";
            this.membersBtn.Size = new System.Drawing.Size(43, 23);
            this.membersBtn.TabIndex = 7;
            this.membersBtn.Text = "mems";
            this.membersBtn.UseVisualStyleBackColor = true;
            this.membersBtn.Click += new System.EventHandler(this.membersBtn_Click);
            // 
            // chatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 450);
            this.Controls.Add(this.membersBtn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.portBox);
            this.Controls.Add(this.useSSL);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nameBox);
            this.Controls.Add(this.connectBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ipAddrBox);
            this.Controls.Add(this.sendTextbox);
            this.Controls.Add(this.msgs);
            this.Controls.Add(this.sendBtn);
            this.MinimumSize = new System.Drawing.Size(450, 250);
            this.Name = "chatForm";
            this.ShowIcon = false;
            this.Text = "TCP Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.chatForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button sendBtn;
        private System.Windows.Forms.RichTextBox msgs;
        private System.Windows.Forms.TextBox sendTextbox;
        private System.Windows.Forms.TextBox ipAddrBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button connectBtn;
        private System.Windows.Forms.TextBox nameBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox useSSL;
        private System.Windows.Forms.TextBox portBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button membersBtn;
    }
}

