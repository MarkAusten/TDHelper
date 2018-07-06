using System;
using System.IO;
using System.Windows.Forms;

namespace TDHelper
{
    public partial class ChangeLogForm : Form
    {
        public ChangeLogForm()
        {
            InitializeComponent();
        }

        private void ChangeLogForm_Load(object sender, EventArgs e)
        {
            string changelogPath = Path.Combine(MainForm.assemblyPath, "Changelog.txt");

            if (File.Exists(changelogPath))
            {
                changelogTextBox.Text = File.ReadAllText(changelogPath);
            }
        }

        private void ChangelogTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                this.Close();
            }
        }

        private void ChangelogTextBox_MouseEnter(object sender, EventArgs e)
        {
            changelogTextBox.Focus();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ExitButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                this.Close();
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OkButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                this.Close();
            }
        }
    }
}