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
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {// load our current settings from the config
            if (!string.IsNullOrEmpty(MainForm.settingsRef.ExtraRunParams))
            {
                extraRunParameters.Text = MainForm.settingsRef.ExtraRunParams;
            }

            if (MainForm.settingsRef.DisableNetLogs)
            {
                overrideDisableNetLogs.Checked = true;
            }

            if (MainForm.settingsRef.DoNotUpdate)
            {
                overrideDoNotUpdate.Checked = true;
            }

            if (MainForm.settingsRef.CopySystemToClipboard)
            {
                overrideCopySystemToClipboard.Checked = true;
            }

            if (!string.IsNullOrEmpty(MainForm.settingsRef.PythonPath))
            {
                pythonPathBox.Text = MainForm.settingsRef.PythonPath;
            }

            if (!string.IsNullOrEmpty(MainForm.settingsRef.TDPath))
            {
                tdPathBox.Text = MainForm.settingsRef.TDPath;
            }

            if (!string.IsNullOrEmpty(MainForm.settingsRef.NetLogPath))
            {
                netLogsPathBox.Text = MainForm.settingsRef.NetLogPath;
            }

            if (!string.IsNullOrEmpty(MainForm.settingsRef.EdcePath))
            {
                edcePathBox.Text = MainForm.settingsRef.EdcePath;
            }

            if (!string.IsNullOrEmpty(MainForm.settingsRef.TreeViewFont))
            {// set our selected font and text box to what we've set in our config
                Font savedFont = MainForm.settingsRef.convertFromMemberFont();
                FontConverter fontToString = new FontConverter();
                currentTVFontBox.Text = string.Format("{0}", fontToString.ConvertToInvariantString(savedFont));
            }
            else
            {// set to a global default
                Font tvDefaultFont = new Font("Consolas", 8.25f);
                currentTVFontBox.Text = string.Format("{0}", MainForm.settingsRef.convertToFontString(tvDefaultFont));
                MainForm.settingsRef.TreeViewFont = MainForm.settingsRef.convertToFontString(tvDefaultFont);
            }

            this.rebuyPercentage.Value = MainForm.settingsRef.RebuyPercentage;
        }

        private void FormValidator()
        {
            this.CopyValuesToSettings();

            MainForm.SaveSettingsToIniFile();

            this.Close();
        }

        /// <summary>
        /// Copy the values from the setting boxes to the settings obejct.
        /// </summary>
        private void CopyValuesToSettings()
        {
            TDSettings settings = MainForm.settingsRef;

            settings.DisableNetLogs = this.overrideDisableNetLogs.Checked;
            settings.DoNotUpdate = this.overrideDoNotUpdate.Checked;
            settings.CopySystemToClipboard = !this.overrideCopySystemToClipboard.Checked;

            settings.PythonPath = this.pythonPathBox.Text;
            settings.TDPath = this.tdPathBox.Text;
            settings.EdcePath = this.edcePathBox.Text;
            settings.NetLogPath = this.netLogsPathBox.Text;

            settings.ExtraRunParams = this.extraRunParameters.Text;

            settings.RebuyPercentage = this.rebuyPercentage.Value;
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
            string origPath = MainForm.settingsRef.PythonPath;
            string origTDPath = MainForm.settingsRef.TDPath;

            MainForm.settingsRef.PythonPath = "";
            MainForm.ValidatePython(origPath);
            
            // adjust for Trade Dangerous Installer
            if (MainForm.settingsRef.PythonPath.EndsWith("trade.exe"))
            {
                MainForm.ValidateTDPath(origTDPath);
                tdPathBox.Text = MainForm.settingsRef.TDPath;
            }
            else if (MainForm.settingsRef.PythonPath.EndsWith("python.exe")
                && !MainForm.CheckIfFileOpens(MainForm.settingsRef.TDPath + "\\trade.py"))
            {
                MainForm.settingsRef.TDPath = "";
                MainForm.ValidateTDPath(origTDPath);
                tdPathBox.Text = MainForm.settingsRef.TDPath;
            }

            pythonPathBox.Text = MainForm.settingsRef.PythonPath;
        }

        private void validateTDPath_Click(object sender, EventArgs e)
        {
            string origPath = MainForm.settingsRef.TDPath;
            string origPyPath = MainForm.settingsRef.PythonPath;

            // if we're using Trade Dangerous Installer, wipe our interpreter path first
            if (origPyPath.EndsWith("trade.exe"))
                MainForm.settingsRef.PythonPath = "";

            MainForm.settingsRef.TDPath = "";
            MainForm.ValidateTDPath(origPath);

            if (origPyPath.EndsWith("trade.exe"))
            {
                MainForm.ValidatePython(origPyPath);
                pythonPathBox.Text = MainForm.settingsRef.PythonPath;
            }

            tdPathBox.Text = MainForm.settingsRef.TDPath;
        }

        private void validateNetLogsPath_Click(object sender, EventArgs e)
        {
            string origPath = MainForm.settingsRef.NetLogPath;
            MainForm.settingsRef.NetLogPath = "";
            MainForm.ValidateNetLogPath(origPath);
            netLogsPathBox.Text = MainForm.settingsRef.NetLogPath;
        }

        private void validateEdcePath_Click(object sender, EventArgs e)
        {
            string origPath = MainForm.settingsRef.EdcePath;
            MainForm.settingsRef.EdcePath = "";
            MainForm.ValidateEdcePath(origPath);
            edcePathBox.Text = MainForm.settingsRef.EdcePath;
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            DialogResult d = TopMostMessageBox.Show(false,true,"This will wipe all configuration settings currently loaded, are you sure?","Warning",MessageBoxButtons.YesNo);

            if (d == DialogResult.Yes)
            {
                MainForm.callForReset = true;
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
                MainForm.settingsRef.TreeViewFont = MainForm.settingsRef.convertToFontString(tvDefaultFont);
            }

            if (fontDialog.ShowDialog(this) == DialogResult.OK)
            {
                localFontObject = fontDialog.Font;
                MainForm.settingsRef.TreeViewFont = MainForm.settingsRef.convertToFontString(localFontObject);
                currentTVFontBox.Text = string.Format("{0}", MainForm.settingsRef.TreeViewFont);
            }
            else
            {// set to our saved default, if that's null, set to the global default
                if (MainForm.settingsRef.TreeViewFont != null)
                    localFontObject = MainForm.settingsRef.convertFromMemberFont();
                else
                    localFontObject = tvDefaultFont;

                currentTVFontBox.Text = string.Format("{0}", MainForm.settingsRef.convertToFontString(localFontObject));
            }
        }
    }
}
