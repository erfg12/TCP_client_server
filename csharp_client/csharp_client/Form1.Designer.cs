namespace csharp_client
{
    partial class Form1
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
            this.SuspendLayout();
            // 
            // sendBtn
            // 
            this.sendBtn.Location = new System.Drawing.Point(370, 415);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(49, 23);
            this.sendBtn.TabIndex = 0;
            this.sendBtn.Text = "send";
            this.sendBtn.UseVisualStyleBackColor = true;
            this.sendBtn.Click += new System.EventHandler(this.sndMsg_Click);
            // 
            // msgs
            // 
            this.msgs.Location = new System.Drawing.Point(12, 37);
            this.msgs.Name = "msgs";
            this.msgs.Size = new System.Drawing.Size(407, 372);
            this.msgs.TabIndex = 2;
            this.msgs.Text = "";
            // 
            // sendTextbox
            // 
            this.sendTextbox.Location = new System.Drawing.Point(12, 417);
            this.sendTextbox.MaxLength = 2048;
            this.sendTextbox.Name = "sendTextbox";
            this.sendTextbox.Size = new System.Drawing.Size(352, 20);
            this.sendTextbox.TabIndex = 3;
            this.sendTextbox.Text = "test message";
            // 
            // ipAddrBox
            // 
            this.ipAddrBox.Location = new System.Drawing.Point(174, 10);
            this.ipAddrBox.Name = "ipAddrBox";
            this.ipAddrBox.Size = new System.Drawing.Size(74, 20);
            this.ipAddrBox.TabIndex = 4;
            this.ipAddrBox.Text = "127.0.0.1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(154, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "IP:";
            // 
            // connectBtn
            // 
            this.connectBtn.Location = new System.Drawing.Point(308, 9);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(56, 23);
            this.connectBtn.TabIndex = 6;
            this.connectBtn.Text = "connect";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            // 
            // nameBox
            // 
            this.nameBox.Location = new System.Drawing.Point(49, 10);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(100, 20);
            this.nameBox.TabIndex = 7;
            this.nameBox.Text = "Beta Tester";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "name:";
            // 
            // useSSL
            // 
            this.useSSL.AutoSize = true;
            this.useSSL.Location = new System.Drawing.Point(378, 13);
            this.useSSL.Name = "useSSL";
            this.useSSL.Size = new System.Drawing.Size(46, 17);
            this.useSSL.TabIndex = 9;
            this.useSSL.Text = "SSL";
            this.useSSL.UseVisualStyleBackColor = true;
            // 
            // portBox
            // 
            this.portBox.Location = new System.Drawing.Point(258, 10);
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(48, 20);
            this.portBox.TabIndex = 10;
            this.portBox.Text = "13000";
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 450);
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
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
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
    }
}

