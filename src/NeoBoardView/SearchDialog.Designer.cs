namespace NeoBoardView
{
    partial class SearchDialog
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
            this.btnFind = new System.Windows.Forms.Button();
            this.tbQuery1 = new System.Windows.Forms.TextBox();
            this.lQuery1 = new System.Windows.Forms.Label();
            this.lQuery2 = new System.Windows.Forms.Label();
            this.lQuery3 = new System.Windows.Forms.Label();
            this.tbQuery2 = new System.Windows.Forms.TextBox();
            this.tbQuery3 = new System.Windows.Forms.TextBox();
            this.lStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(218, 90);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(75, 23);
            this.btnFind.TabIndex = 3;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // tbQuery1
            // 
            this.tbQuery1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbQuery1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tbQuery1.Location = new System.Drawing.Point(38, 12);
            this.tbQuery1.MaxLength = 128;
            this.tbQuery1.Name = "tbQuery1";
            this.tbQuery1.Size = new System.Drawing.Size(254, 20);
            this.tbQuery1.TabIndex = 0;
            // 
            // lQuery1
            // 
            this.lQuery1.AutoSize = true;
            this.lQuery1.Location = new System.Drawing.Point(12, 15);
            this.lQuery1.Name = "lQuery1";
            this.lQuery1.Size = new System.Drawing.Size(20, 13);
            this.lQuery1.TabIndex = 2;
            this.lQuery1.Text = "#1";
            // 
            // lQuery2
            // 
            this.lQuery2.AutoSize = true;
            this.lQuery2.Location = new System.Drawing.Point(12, 41);
            this.lQuery2.Name = "lQuery2";
            this.lQuery2.Size = new System.Drawing.Size(20, 13);
            this.lQuery2.TabIndex = 3;
            this.lQuery2.Text = "#2";
            // 
            // lQuery3
            // 
            this.lQuery3.AutoSize = true;
            this.lQuery3.Location = new System.Drawing.Point(12, 67);
            this.lQuery3.Name = "lQuery3";
            this.lQuery3.Size = new System.Drawing.Size(20, 13);
            this.lQuery3.TabIndex = 4;
            this.lQuery3.Text = "#3";
            // 
            // tbQuery2
            // 
            this.tbQuery2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbQuery2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tbQuery2.Location = new System.Drawing.Point(38, 38);
            this.tbQuery2.MaxLength = 128;
            this.tbQuery2.Name = "tbQuery2";
            this.tbQuery2.Size = new System.Drawing.Size(254, 20);
            this.tbQuery2.TabIndex = 1;
            // 
            // tbQuery3
            // 
            this.tbQuery3.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbQuery3.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tbQuery3.Location = new System.Drawing.Point(38, 64);
            this.tbQuery3.MaxLength = 128;
            this.tbQuery3.Name = "tbQuery3";
            this.tbQuery3.Size = new System.Drawing.Size(254, 20);
            this.tbQuery3.TabIndex = 2;
            // 
            // lStatus
            // 
            this.lStatus.AutoSize = true;
            this.lStatus.Location = new System.Drawing.Point(12, 95);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(47, 13);
            this.lStatus.TabIndex = 5;
            this.lStatus.Text = "<status>";
            // 
            // SearchDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 125);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.tbQuery3);
            this.Controls.Add(this.tbQuery2);
            this.Controls.Add(this.lQuery3);
            this.Controls.Add(this.lQuery2);
            this.Controls.Add(this.lQuery1);
            this.Controls.Add(this.tbQuery1);
            this.Controls.Add(this.btnFind);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "SearchDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Find";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.TextBox tbQuery1;
        private System.Windows.Forms.Label lQuery1;
        private System.Windows.Forms.Label lQuery2;
        private System.Windows.Forms.Label lQuery3;
        private System.Windows.Forms.TextBox tbQuery2;
        private System.Windows.Forms.TextBox tbQuery3;
        private System.Windows.Forms.Label lStatus;
    }
}