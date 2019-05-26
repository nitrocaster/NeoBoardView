namespace NeoBoardView
{
    partial class AboutDialog
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
            this.btnOK = new System.Windows.Forms.Button();
            this.linkEmail = new System.Windows.Forms.LinkLabelEx();
            this.lProductName = new System.Windows.Forms.Label();
            this.lAuthor = new System.Windows.Forms.Label();
            this.linkHomePage = new System.Windows.Forms.LinkLabelEx();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(69, 130);
            this.btnOK.Margin = new System.Windows.Forms.Padding(60, 3, 60, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(126, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "Close";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // linkEmail
            // 
            this.linkEmail.AutoSize = true;
            this.linkEmail.LinkColor = System.Drawing.Color.Blue;
            this.linkEmail.Location = new System.Drawing.Point(12, 60);
            this.linkEmail.Name = "linkEmail";
            this.linkEmail.Size = new System.Drawing.Size(95, 13);
            this.linkEmail.TabIndex = 4;
            this.linkEmail.TabStop = true;
            this.linkEmail.Text = "su@nitrocaster.me";
            this.linkEmail.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkEmail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkEmail_LinkClicked);
            // 
            // lProductName
            // 
            this.lProductName.AutoSize = true;
            this.lProductName.Location = new System.Drawing.Point(12, 9);
            this.lProductName.Name = "lProductName";
            this.lProductName.Size = new System.Drawing.Size(78, 13);
            this.lProductName.TabIndex = 5;
            this.lProductName.Text = "NeoBoardView";
            // 
            // lAuthor
            // 
            this.lAuthor.AutoSize = true;
            this.lAuthor.Location = new System.Drawing.Point(12, 26);
            this.lAuthor.Name = "lAuthor";
            this.lAuthor.Size = new System.Drawing.Size(187, 13);
            this.lAuthor.TabIndex = 6;
            this.lAuthor.Text = "© 2016 Pavel Kovalenko / nitrocaster";
            // 
            // linkHomePage
            // 
            this.linkHomePage.AutoSize = true;
            this.linkHomePage.LinkColor = System.Drawing.Color.Blue;
            this.linkHomePage.Location = new System.Drawing.Point(12, 43);
            this.linkHomePage.Name = "linkHomePage";
            this.linkHomePage.Size = new System.Drawing.Size(149, 13);
            this.linkHomePage.TabIndex = 7;
            this.linkHomePage.TabStop = true;
            this.linkHomePage.Text = "nitrocaster.me/NeoBoardView";
            this.linkHomePage.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkHomePage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkHomePage_LinkClicked);
            // 
            // AboutDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 165);
            this.Controls.Add(this.linkHomePage);
            this.Controls.Add(this.lAuthor);
            this.Controls.Add(this.lProductName);
            this.Controls.Add(this.linkEmail);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.LinkLabelEx linkEmail;
        private System.Windows.Forms.Label lProductName;
        private System.Windows.Forms.Label lAuthor;
        private System.Windows.Forms.LinkLabelEx linkHomePage;
    }
}