using System.Drawing;
using System.Windows.Forms;

namespace NeoBoardView
{
    partial class MainDialog
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
            this.scToolPanel = new System.Windows.Forms.SplitContainer();
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnFindPin = new System.Windows.Forms.Button();
            this.btnFindNail = new System.Windows.Forms.Button();
            this.btnFindNet = new System.Windows.Forms.Button();
            this.btnFindPart = new System.Windows.Forms.Button();
            this.btnSide = new System.Windows.Forms.Button();
            this.btnNames = new System.Windows.Forms.Button();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnRotateCW = new System.Windows.Forms.Button();
            this.btnRotateCCW = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.pbDrawingSurface = new System.Windows.Forms.PictureBox();
            this.scFooter = new System.Windows.Forms.SplitContainer();
            this.lDebug = new System.Windows.Forms.Label();
            this.lStatus = new System.Windows.Forms.Label();
            this.btnHeurShapes = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.scToolPanel)).BeginInit();
            this.scToolPanel.Panel1.SuspendLayout();
            this.scToolPanel.Panel2.SuspendLayout();
            this.scToolPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbDrawingSurface)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scFooter)).BeginInit();
            this.scFooter.Panel1.SuspendLayout();
            this.scFooter.Panel2.SuspendLayout();
            this.scFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // scToolPanel
            // 
            this.scToolPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scToolPanel.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scToolPanel.IsSplitterFixed = true;
            this.scToolPanel.Location = new System.Drawing.Point(0, 0);
            this.scToolPanel.Name = "scToolPanel";
            this.scToolPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scToolPanel.Panel1
            // 
            this.scToolPanel.Panel1.Controls.Add(this.btnHeurShapes);
            this.scToolPanel.Panel1.Controls.Add(this.btnAbout);
            this.scToolPanel.Panel1.Controls.Add(this.btnFindPin);
            this.scToolPanel.Panel1.Controls.Add(this.btnFindNail);
            this.scToolPanel.Panel1.Controls.Add(this.btnFindNet);
            this.scToolPanel.Panel1.Controls.Add(this.btnFindPart);
            this.scToolPanel.Panel1.Controls.Add(this.btnSide);
            this.scToolPanel.Panel1.Controls.Add(this.btnNames);
            this.scToolPanel.Panel1.Controls.Add(this.btnHome);
            this.scToolPanel.Panel1.Controls.Add(this.btnRotateCW);
            this.scToolPanel.Panel1.Controls.Add(this.btnRotateCCW);
            this.scToolPanel.Panel1.Controls.Add(this.btnOpen);
            this.scToolPanel.Panel1.Margin = new System.Windows.Forms.Padding(4);
            this.scToolPanel.Panel1.Padding = new System.Windows.Forms.Padding(2);
            this.scToolPanel.Panel1MinSize = 24;
            // 
            // scToolPanel.Panel2
            // 
            this.scToolPanel.Panel2.Controls.Add(this.pbDrawingSurface);
            this.scToolPanel.Panel2.Padding = new System.Windows.Forms.Padding(2);
            this.scToolPanel.Panel2MinSize = 240;
            this.scToolPanel.Size = new System.Drawing.Size(624, 419);
            this.scToolPanel.SplitterDistance = 25;
            this.scToolPanel.SplitterWidth = 1;
            this.scToolPanel.TabIndex = 1;
            // 
            // btnAbout
            // 
            this.btnAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(185)))), ((int)(((byte)(185)))));
            this.btnAbout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbout.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(204)));
            this.btnAbout.Location = new System.Drawing.Point(601, 4);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(21, 21);
            this.btnAbout.TabIndex = 11;
            this.btnAbout.Text = "A";
            this.btnAbout.UseVisualStyleBackColor = false;
            // 
            // btnFindPin
            // 
            this.btnFindPin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(185)))), ((int)(((byte)(185)))));
            this.btnFindPin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFindPin.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(204)));
            this.btnFindPin.Location = new System.Drawing.Point(194, 4);
            this.btnFindPin.Name = "btnFindPin";
            this.btnFindPin.Size = new System.Drawing.Size(21, 21);
            this.btnFindPin.TabIndex = 8;
            this.btnFindPin.Text = "FPP";
            this.btnFindPin.UseVisualStyleBackColor = false;
            // 
            // btnFindNail
            // 
            this.btnFindNail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(185)))), ((int)(((byte)(185)))));
            this.btnFindNail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFindNail.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(204)));
            this.btnFindNail.Location = new System.Drawing.Point(240, 4);
            this.btnFindNail.Name = "btnFindNail";
            this.btnFindNail.Size = new System.Drawing.Size(21, 21);
            this.btnFindNail.TabIndex = 10;
            this.btnFindNail.Text = "FNL";
            this.btnFindNail.UseVisualStyleBackColor = false;
            // 
            // btnFindNet
            // 
            this.btnFindNet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(185)))), ((int)(((byte)(185)))));
            this.btnFindNet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFindNet.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(204)));
            this.btnFindNet.Location = new System.Drawing.Point(217, 4);
            this.btnFindNet.Name = "btnFindNet";
            this.btnFindNet.Size = new System.Drawing.Size(21, 21);
            this.btnFindNet.TabIndex = 9;
            this.btnFindNet.Text = "FNT";
            this.btnFindNet.UseVisualStyleBackColor = false;
            // 
            // btnFindPart
            // 
            this.btnFindPart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(185)))), ((int)(((byte)(185)))));
            this.btnFindPart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFindPart.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(204)));
            this.btnFindPart.Location = new System.Drawing.Point(171, 4);
            this.btnFindPart.Name = "btnFindPart";
            this.btnFindPart.Size = new System.Drawing.Size(21, 21);
            this.btnFindPart.TabIndex = 7;
            this.btnFindPart.Text = "FPT";
            this.btnFindPart.UseVisualStyleBackColor = false;
            // 
            // btnSide
            // 
            this.btnSide.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(185)))), ((int)(((byte)(185)))));
            this.btnSide.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSide.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(204)));
            this.btnSide.Location = new System.Drawing.Point(75, 4);
            this.btnSide.Name = "btnSide";
            this.btnSide.Size = new System.Drawing.Size(21, 21);
            this.btnSide.TabIndex = 3;
            this.btnSide.Text = "S";
            this.btnSide.UseVisualStyleBackColor = false;
            // 
            // btnNames
            // 
            this.btnNames.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(185)))), ((int)(((byte)(185)))));
            this.btnNames.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNames.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(204)));
            this.btnNames.Location = new System.Drawing.Point(121, 4);
            this.btnNames.Name = "btnNames";
            this.btnNames.Size = new System.Drawing.Size(21, 21);
            this.btnNames.TabIndex = 5;
            this.btnNames.Text = "N";
            this.btnNames.UseVisualStyleBackColor = false;
            // 
            // btnHome
            // 
            this.btnHome.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(185)))), ((int)(((byte)(185)))));
            this.btnHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHome.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(204)));
            this.btnHome.Location = new System.Drawing.Point(98, 4);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(21, 21);
            this.btnHome.TabIndex = 4;
            this.btnHome.Text = "H";
            this.btnHome.UseVisualStyleBackColor = false;
            // 
            // btnRotateCW
            // 
            this.btnRotateCW.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(185)))), ((int)(((byte)(185)))));
            this.btnRotateCW.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRotateCW.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(204)));
            this.btnRotateCW.Location = new System.Drawing.Point(48, 4);
            this.btnRotateCW.Name = "btnRotateCW";
            this.btnRotateCW.Size = new System.Drawing.Size(21, 21);
            this.btnRotateCW.TabIndex = 2;
            this.btnRotateCW.Text = "R";
            this.btnRotateCW.UseVisualStyleBackColor = false;
            // 
            // btnRotateCCW
            // 
            this.btnRotateCCW.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(185)))), ((int)(((byte)(185)))));
            this.btnRotateCCW.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRotateCCW.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(204)));
            this.btnRotateCCW.Location = new System.Drawing.Point(25, 4);
            this.btnRotateCCW.Name = "btnRotateCCW";
            this.btnRotateCCW.Size = new System.Drawing.Size(21, 21);
            this.btnRotateCCW.TabIndex = 1;
            this.btnRotateCCW.Text = "L";
            this.btnRotateCCW.UseVisualStyleBackColor = false;
            // 
            // btnOpen
            // 
            this.btnOpen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(185)))), ((int)(((byte)(185)))));
            this.btnOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpen.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(204)));
            this.btnOpen.Location = new System.Drawing.Point(2, 4);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(21, 21);
            this.btnOpen.TabIndex = 0;
            this.btnOpen.Text = "O";
            this.btnOpen.UseVisualStyleBackColor = false;
            // 
            // pbDrawingSurface
            // 
            this.pbDrawingSurface.BackColor = System.Drawing.Color.White;
            this.pbDrawingSurface.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbDrawingSurface.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbDrawingSurface.Location = new System.Drawing.Point(2, 2);
            this.pbDrawingSurface.Name = "pbDrawingSurface";
            this.pbDrawingSurface.Size = new System.Drawing.Size(620, 389);
            this.pbDrawingSurface.TabIndex = 0;
            this.pbDrawingSurface.TabStop = false;
            this.pbDrawingSurface.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbDrawingSurface_MouseDown);
            this.pbDrawingSurface.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbDrawingSurface_MouseMove);
            this.pbDrawingSurface.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbDrawingSurface_MouseUp);
            // 
            // scFooter
            // 
            this.scFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scFooter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.scFooter.IsSplitterFixed = true;
            this.scFooter.Location = new System.Drawing.Point(0, 0);
            this.scFooter.Name = "scFooter";
            this.scFooter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scFooter.Panel1
            // 
            this.scFooter.Panel1.Controls.Add(this.scToolPanel);
            this.scFooter.Panel1MinSize = 160;
            // 
            // scFooter.Panel2
            // 
            this.scFooter.Panel2.Controls.Add(this.lDebug);
            this.scFooter.Panel2.Controls.Add(this.lStatus);
            this.scFooter.Panel2MinSize = 24;
            this.scFooter.Size = new System.Drawing.Size(624, 445);
            this.scFooter.SplitterDistance = 419;
            this.scFooter.SplitterWidth = 1;
            this.scFooter.TabIndex = 3;
            // 
            // lDebug
            // 
            this.lDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lDebug.AutoSize = true;
            this.lDebug.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lDebug.Location = new System.Drawing.Point(450, 6);
            this.lDebug.Name = "lDebug";
            this.lDebug.Size = new System.Drawing.Size(91, 14);
            this.lDebug.TabIndex = 99;
            this.lDebug.Text = "<debug info>";
            this.lDebug.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lStatus
            // 
            this.lStatus.AutoSize = true;
            this.lStatus.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lStatus.Location = new System.Drawing.Point(3, 6);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(63, 14);
            this.lStatus.TabIndex = 4;
            this.lStatus.Text = "<status>";
            // 
            // btnHeurShapes
            // 
            this.btnHeurShapes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(185)))), ((int)(((byte)(185)))));
            this.btnHeurShapes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHeurShapes.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(204)));
            this.btnHeurShapes.Location = new System.Drawing.Point(144, 4);
            this.btnHeurShapes.Name = "btnHeurShapes";
            this.btnHeurShapes.Size = new System.Drawing.Size(21, 21);
            this.btnHeurShapes.TabIndex = 6;
            this.btnHeurShapes.Text = "HS";
            this.btnHeurShapes.UseVisualStyleBackColor = false;
            // 
            // MainDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 445);
            this.Controls.Add(this.scFooter);
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "MainDialog";
            this.Text = "NeoBoardView";
            this.scToolPanel.Panel1.ResumeLayout(false);
            this.scToolPanel.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scToolPanel)).EndInit();
            this.scToolPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbDrawingSurface)).EndInit();
            this.scFooter.Panel1.ResumeLayout(false);
            this.scFooter.Panel2.ResumeLayout(false);
            this.scFooter.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scFooter)).EndInit();
            this.scFooter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer scToolPanel;
        private System.Windows.Forms.PictureBox pbDrawingSurface;
        private SplitContainer scFooter;
        private Label lStatus;
        private Button btnOpen;
        private Button btnRotateCCW;
        private Button btnRotateCW;
        private Button btnHome;
        private Button btnNames;
        private Button btnSide;
        private Button btnFindPart;
        private Button btnFindNet;
        private Button btnFindNail;
        private Button btnFindPin;
        private Button btnAbout;
        private Label lDebug;
        private Button btnHeurShapes;
    }
}
