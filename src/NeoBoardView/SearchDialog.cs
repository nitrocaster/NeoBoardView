using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NeoBoardView
{
    public partial class SearchDialog : Form
    {
        private SearchCallback cb;

        public SearchDialog()
        {
            InitializeComponent();
            Icon = Properties.Resources.AppIcon1;
        }

        private string GetSubjDesc(QuerySubject qs)
        {
            switch (qs)
            {
            case QuerySubject.Net: return "Net";
            case QuerySubject.Part: return "Part";
            case QuerySubject.Pin: return "Part.Pin";
            case QuerySubject.Nail: return "Nail";
            default: return "";
            }
        }

        private AutoCompleteStringCollection parts = new AutoCompleteStringCollection();
        private AutoCompleteStringCollection pins = new AutoCompleteStringCollection();
        private AutoCompleteStringCollection nails = new AutoCompleteStringCollection();
        private AutoCompleteStringCollection nets = new AutoCompleteStringCollection();

        public void InitializeAutocomplete()
        {
            parts.Clear();
            pins.Clear();
            nails.Clear();
            nets.Clear();
            // parts
            foreach (SceneObjects.Part part in Root.Scene.Parts)
                parts.Add(part.Name);
            // pins
            foreach (SceneObjects.Pin pin in Root.Scene.Pins)
            {
                if (!pins.Contains(pin.Name))
                    pins.Add(pin.Name);
            }
            // nails
            foreach (SceneObjects.Nail nail in Root.Scene.Nails)
            {
                if (nails.Contains(nail.Name))
                    nails.Add(nail.Name);
            }
            // nets
            foreach (string pin in pins)
                nets.Add(pin);
            foreach (string nail in nails)
            {
                if (nets.Contains(nail))
                    nets.Add(nail);
            }
        }

        public void Setup(QuerySubject subj, int queryCount, SearchCallback cb)
        {
            this.cb = cb;
            Text = "Find "+GetSubjDesc(subj);
            lStatus.Text = string.Empty;
            switch (subj)
            {
            case QuerySubject.Net:
                tbQuery1.AutoCompleteCustomSource = nets;
                tbQuery2.AutoCompleteCustomSource = nets;
                tbQuery3.AutoCompleteCustomSource = nets;
                break;
            case QuerySubject.Part:
                tbQuery1.AutoCompleteCustomSource = parts;
                tbQuery2.AutoCompleteCustomSource = parts;
                tbQuery3.AutoCompleteCustomSource = parts;
                break;
            case QuerySubject.Pin:
                tbQuery1.AutoCompleteCustomSource = pins;
                tbQuery2.AutoCompleteCustomSource = pins;
                tbQuery3.AutoCompleteCustomSource = pins;
                break;
            case QuerySubject.Nail:
                tbQuery1.AutoCompleteCustomSource = nails;
                tbQuery2.AutoCompleteCustomSource = nails;
                tbQuery3.AutoCompleteCustomSource = nails;
                break;
            }
            tbQuery1.Clear();
            tbQuery2.Clear();
            tbQuery3.Clear();
            switch (queryCount)
            {
            case 1:
                tbQuery1.Visible = true;
                lQuery1.Visible = true;
                tbQuery2.Visible = false;
                lQuery2.Visible = false;
                tbQuery3.Visible = false;
                lQuery3.Visible = false;
                break;
            case 2:
                tbQuery1.Visible = true;
                lQuery1.Visible = true;
                tbQuery2.Visible = true;
                lQuery2.Visible = true;
                tbQuery3.Visible = false;
                lQuery3.Visible = false;
                break;
            case 3:
                tbQuery1.Visible = true;
                lQuery1.Visible = true;
                tbQuery2.Visible = true;
                lQuery2.Visible = true;
                tbQuery3.Visible = true;
                lQuery3.Visible = true;
                break;
            }
            tbQuery1.Focus();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            var q = new[] {tbQuery1.Text.Trim(), tbQuery2.Text.Trim(), tbQuery3.Text.Trim()};
            bool found;
            cb(q, out found);
            if (!found)
                lStatus.Text = "Nothing found";
            else
                Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //if ((keyData & Keys.Modifiers)==Keys.None)
            switch (keyData)
            {
            case Keys.Enter:
                btnFind_Click(this, null);
                return true;
            case Keys.Escape:
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    public delegate void SearchCallback(string[] query, out bool found);

    public enum QuerySubject
    {
        Net,
        Part,
        Pin,
        Nail
    }
}
