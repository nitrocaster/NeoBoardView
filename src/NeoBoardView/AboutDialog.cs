using System;
using System.Text;
using System.Windows.Forms;

namespace NeoBoardView
{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();
            Icon = Properties.Resources.AppIcon1;
            lProductName.Text += " "+Root.BuildString;
        }
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //if ((keyData & Keys.Modifiers)==Keys.None)
            switch (keyData)
            {
            case Keys.Escape:
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void linkEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:su@nitrocaster.me?Subject=NeoBoardView");
        }

        private void linkHomePage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://nitrocaster.me/NeoBoardView");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason==CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                return;
            }
            base.OnFormClosing(e);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
