﻿namespace csharp_client
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
            this.sendBtn.Click += new System.EventHandler(this.button1_Click);
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
            this.ipAddrBox.Location = new System.Drawing.Point(77, 12);
            this.ipAddrBox.Name = "ipAddrBox";
            this.ipAddrBox.Size = new System.Drawing.Size(119, 20);
            this.ipAddrBox.TabIndex = 4;
            this.ipAddrBox.Text = "127.0.0.1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "IP Address:";
            // 
            // connectBtn
            // 
            this.connectBtn.Location = new System.Drawing.Point(203, 10);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(75, 23);
            this.connectBtn.TabIndex = 6;
            this.connectBtn.Text = "connect";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 450);
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
    }
}

