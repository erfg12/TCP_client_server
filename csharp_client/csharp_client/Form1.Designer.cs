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
            this.msgs.Location = new System.Drawing.Point(12, 12);
            this.msgs.Name = "msgs";
            this.msgs.Size = new System.Drawing.Size(407, 397);
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 450);
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
    }
}

