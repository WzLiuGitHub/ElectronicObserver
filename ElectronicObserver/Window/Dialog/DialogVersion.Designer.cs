﻿namespace ElectronicObserver.Window.Dialog
{
	partial class DialogVersion
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
            this.TextVersion = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TextAuthor = new System.Windows.Forms.LinkLabel();
            this.ButtonClose = new System.Windows.Forms.Button();
            this.TextInformation = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TextVersion
            // 
            this.TextVersion.AutoSize = true;
            this.TextVersion.Location = new System.Drawing.Point(12, 9);
            this.TextVersion.Margin = new System.Windows.Forms.Padding(3);
            this.TextVersion.Name = "TextVersion";
            this.TextVersion.Size = new System.Drawing.Size(208, 15);
            this.TextVersion.TabIndex = 1;
            this.TextVersion.Text = "試製七四式電子観測儀零型 (ver. 0.0)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 40);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "開発：";
            // 
            // TextAuthor
            // 
            this.TextAuthor.AutoSize = true;
            this.TextAuthor.Location = new System.Drawing.Point(12, 58);
            this.TextAuthor.Name = "TextAuthor";
            this.TextAuthor.Size = new System.Drawing.Size(55, 15);
            this.TextAuthor.TabIndex = 3;
            this.TextAuthor.TabStop = true;
            this.TextAuthor.Text = "Andante";
            this.TextAuthor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.TextAuthor_LinkClicked);
            // 
            // ButtonClose
            // 
            this.ButtonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonClose.Location = new System.Drawing.Point(377, 168);
            this.ButtonClose.Name = "ButtonClose";
            this.ButtonClose.Size = new System.Drawing.Size(75, 23);
            this.ButtonClose.TabIndex = 0;
            this.ButtonClose.Text = "閉じる";
            this.ButtonClose.UseVisualStyleBackColor = true;
            this.ButtonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // TextInformation
            // 
            this.TextInformation.AutoSize = true;
            this.TextInformation.Location = new System.Drawing.Point(12, 94);
            this.TextInformation.Name = "TextInformation";
            this.TextInformation.Size = new System.Drawing.Size(235, 15);
            this.TextInformation.TabIndex = 5;
            this.TextInformation.TabStop = true;
            this.TextInformation.Text = "http://electronicobserver.blog.fc2.com/";
            this.TextInformation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.TextInformation_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 76);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(200, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "配布元(不具合報告・連絡はこちらへ)：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 112);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Modified by:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 133);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(168, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "gre4bee, MyAngelKamikaze";
            // 
            // DialogVersion
            // 
            this.AcceptButton = this.ButtonClose;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(464, 203);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TextInformation);
            this.Controls.Add(this.ButtonClose);
            this.Controls.Add(this.TextAuthor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextVersion);
            this.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogVersion";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "バージョン";
            this.Load += new System.EventHandler(this.DialogVersion_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label TextVersion;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel TextAuthor;
		private System.Windows.Forms.Button ButtonClose;
		private System.Windows.Forms.LinkLabel TextInformation;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
	}
}