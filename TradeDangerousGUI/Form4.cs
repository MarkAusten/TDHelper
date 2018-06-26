using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TDHelper
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {// load our current settings from the config
            if (!String.IsNullOrEmpty(Form1.settingsRef.EDAPIUser))
                edapiUserBox.Text = Form1.Decrypt(Form1.settingsRef.EDAPIUser);
            if (!String.IsNullOrEmpty(Form1.settingsRef.EDAPIPass))
                edapiPassBox.Text = Form1.Decrypt(Form1.settingsRef.EDAPIPass);

            if (!String.IsNullOrEmpty(Form1.settingsRef.ExtraRunParams))
                textBox1.Text = Form1.settingsRef.ExtraRunParams;
            if (Form1.settingsRef.DisableNetLogs)
                overrideDisableNetLogs.Checked = true;
            if (Form1.settingsRef.DoNotUpdate)
                overrideDoNotUpdate.Checked = true;
            if (Form1.settingsRef.CopySystemToClipboard)
                overrideCopySystemToClipboard.Checked = true;

            if (!String.IsNullOrEmpty(Form1.settingsRef.PythonPath))
                pythonPathBox.Text = Form1.settingsRef.PythonPath;
            if (!String.IsNullOrEmpty(Form1.settingsRef.TDPath))
                tdPathBox.Text = Form1.settingsRef.TDPath;
            if (!String.IsNullOrEmpty(Form1.settingsRef.NetLogPath))
                netLogsPathBox.Text = Form1.settingsRef.NetLogPath;

            if (!String.IsNullOrEmpty(Form1.settingsRef.TreeViewFont))
            {// set our selected font and text box to what we've set in our config
                Font savedFont = Form1.settingsRef.convertFromMemberFont();
                FontConverter fontToString = new FontConverter();
                currentTVFontBox.Text = String.Format("{0}", fontToString.ConvertToInvariantString(savedFont));
            }
            else
            {// set to a global default
                Font tvDefaultFont = new Font("Consolas", 8.25f);
                currentTVFontBox.Text = String.Format("{0}", Form1.settingsRef.convertToFontString(tvDefaultFont));
                Form1.settingsRef.TreeViewFont = Form1.settingsRef.convertToFontString(tvDefaultFont);
            }
        }

        private void FormValidator()
        {
            // push our current form settings to the AppSettings()
            Form1.settingsRef.ExtraRunParams = textBox1.Text;
            Form1.settingsRef.DisableNetLogs = overrideDisableNetLogs.Checked;
            Form1.settingsRef.DoNotUpdate = overrideDoNotUpdate.Checked;
            Form1.settingsRef.CopySystemToClipboard = overrideCopySystemToClipboard.Checked;

            // encrypt our edapi login details
            Form1.settingsRef.EDAPIUser = !String.IsNullOrEmpty(edapiUserBox.Text)
                ? Form1.Encrypt(edapiUserBox.Text)
                : String.Empty;

            Form1.settingsRef.EDAPIPass = !String.IsNullOrEmpty(edapiPassBox.Text)
                ? Form1.Encrypt(edapiPassBox.Text)
                : String.Empty;

            // save our misc settings to the config file
            Form1.Serialize(Form1.configFile, Form1.settingsRef.DoNotUpdate, "DoNotUpdate");
            Form1.Serialize(Form1.configFile, Form1.settingsRef.DisableNetLogs, "DisableNetLogs");
            Form1.Serialize(Form1.configFile, Form1.settingsRef.CopySystemToClipboard, "CopySystemToClipboard");
            Form1.Serialize(Form1.configFile, Form1.settingsRef.ExtraRunParams, "ExtraRunParams");
            // serialize our encrypted login details
            Form1.Serialize(Form1.configFile, Form1.settingsRef.EDAPIUser, "EDAPIUser");
            Form1.Serialize(Form1.configFile, Form1.settingsRef.EDAPIPass, "EDAPIPass");
            // save our paths as well
            Form1.Serialize(Form1.configFile, Form1.settingsRef.PythonPath, "PythonPath");
            Form1.Serialize(Form1.configFile, Form1.settingsRef.TDPath, "TDPath");
            Form1.Serialize(Form1.configFile, Form1.settingsRef.NetLogPath, "NetLogPath");
            // save our font name/size for the treeview
            if (!String.IsNullOrEmpty(Form1.settingsRef.TreeViewFont))
                Form1.Serialize(Form1.configFile, Form1.settingsRef.TreeViewFont, "TreeViewFont");

            this.Close();
        }

        private void generic_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                FormValidator();
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            FormValidator();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void validatePythonPath_Click(object sender, EventArgs e)
        {
            string origPath = Form1.settingsRef.PythonPath;
            string origTDPath = Form1.settingsRef.TDPath;

            Form1.settingsRef.PythonPath = "";
            Form1.validatePython(origPath);
            
            // adjust for Trade Dangerous Installer
            if (Form1.settingsRef.PythonPath.EndsWith("trade.exe"))
            {
                Form1.validateTDPath(origTDPath);
                tdPathBox.Text = Form1.settingsRef.TDPath;
            }
            else if (Form1.settingsRef.PythonPath.EndsWith("python.exe")
                && !Form1.checkIfFileOpens(Form1.settingsRef.TDPath + "\\trade.py"))
            {
                Form1.settingsRef.TDPath = "";
                Form1.validateTDPath(origTDPath);
                tdPathBox.Text = Form1.settingsRef.TDPath;
            }

            pythonPathBox.Text = Form1.settingsRef.PythonPath;
        }

        private void validateTDPath_Click(object sender, EventArgs e)
        {
            string origPath = Form1.settingsRef.TDPath;
            string origPyPath = Form1.settingsRef.PythonPath;

            // if we're using Trade Dangerous Installer, wipe our interpreter path first
            if (origPyPath.EndsWith("trade.exe"))
                Form1.settingsRef.PythonPath = "";

            Form1.settingsRef.TDPath = "";
            Form1.validateTDPath(origPath);

            if (origPyPath.EndsWith("trade.exe"))
            {
                Form1.validatePython(origPyPath);
                pythonPathBox.Text = Form1.settingsRef.PythonPath;
            }

            tdPathBox.Text = Form1.settingsRef.TDPath;
        }

        private void validateNetLogsPath_Click(object sender, EventArgs e)
        {
            string origPath = Form1.settingsRef.NetLogPath;
            Form1.settingsRef.NetLogPath = "";
            Form1.validateNetLogPath(origPath);
            netLogsPathBox.Text = Form1.settingsRef.NetLogPath;
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            DialogResult d = TopMostMessageBox.Show(false,true,"This will wipe all configuration settings currently loaded, are you sure?","Warning",MessageBoxButtons.YesNo);

            if (d == DialogResult.Yes)
            {
                Form1.callForReset = true;
                this.Dispose();
            }
        }

        private void tvFontSelectorButton_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            fontDialog.MinSize = 6;
            fontDialog.MaxSize = 32;
            Font tvDefaultFont = new Font("Consolas", 8.25f);
            Font localFontObject = new Font("Consolas", 8.25f);
            fontDialog.Font = tvDefaultFont;

            if (Control.ModifierKeys == Keys.Control)
            {// reset our font to a preset default with a Ctrl+Click
                Form1.settingsRef.TreeViewFont = Form1.settingsRef.convertToFontString(tvDefaultFont);
            }

            if (fontDialog.ShowDialog(this) == DialogResult.OK)
            {
                localFontObject = fontDialog.Font;
                Form1.settingsRef.TreeViewFont = Form1.settingsRef.convertToFontString(localFontObject);
                currentTVFontBox.Text = String.Format("{0}", Form1.settingsRef.TreeViewFont);
            }
            else
            {// set to our saved default, if that's null, set to the global default
                if (Form1.settingsRef.TreeViewFont != null)
                    localFontObject = Form1.settingsRef.convertFromMemberFont();
                else
                    localFontObject = tvDefaultFont;

                currentTVFontBox.Text = String.Format("{0}", Form1.settingsRef.convertToFontString(localFontObject));
            }
        }
    }
}
