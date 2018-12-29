using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace TDHelper
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            ((Button)sender).Enabled = false;

            Close();
        }

        /// <summary>
        /// Copy the values from the setting boxes to the settings obejct.
        /// </summary>
        private void CopyValuesToSettings()
        {
            TDSettings settings = MainForm.settingsRef;

            settings.DisableNetLogs = overrideDisableNetLogs.Checked;
            settings.DoNotUpdate = overrideDoNotUpdate.Checked;
            settings.CopySystemToClipboard = !overrideCopySystemToClipboard.Checked;

            settings.PythonPath = pythonPathBox.Text;
            settings.TDPath = tdPathBox.Text;
            settings.NetLogPath = txtNetLogsPath.Text;

            settings.ExtraRunParams = extraRunParameters.Text;
            settings.Locale = txtLocale.Text;

            settings.RebuyPercentage = rebuyPercentage.Value;

            settings.Quiet = !chkQuiet.Checked;
            settings.Verbosity = verbosityComboBox.SelectedIndex;
            settings.TestSystems = testSystemsCheckBox.Checked;
            settings.ShowProgress = chkProgress.Checked;
            settings.Summary = chkSummary.Checked;
        }

        /// <summary>
        /// Extract the access token from the string if required.
        /// </summary>
        /// <param name="text">The string containing the access token to be extracted.</param>
        /// <returns>The raw access token.</returns>
        private string ExtractAccessToken(string text)
        {
            string token = string.Empty;

            if (text.IndexOf("access_token") != 0)
            {
                dynamic json = JValue.Parse(text);

                token = json.access_token;
            }

            return token;
        }

        /// <summary>
        /// Extract the access token expiry time from the string if required.
        /// </summary>
        /// <param name="text">The string containing the access token expiry time to be extracted.</param>
        /// <returns>The raw access token.</returns>
        private DateTime ExtractAccessTokenExpiry(string text)
        {
            DateTime token = DateTime.Today.AddDays(-1);

            if (text.IndexOf("expires") != 0)
            {
                dynamic json = JValue.Parse(text);

                double offset = json.expires;

                token = (new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
                    .AddSeconds(offset)
                    .ToLocalTime();
            }

            return token;
        }

        private void FormValidator()
        {
            CopyValuesToSettings();

            MainForm.SaveSettingsToIniFile();

            Close();
        }

        private void Generic_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                Close();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                FormValidator();
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            ((Button)sender).Enabled = false;

            FormValidator();
        }

        private void OverrideDisableNetLogs_CheckedChanged(object sender, EventArgs e)
        {
            SetNetlogPathState(((CheckBox)sender).Checked);
            MainForm.settingsRef.DisableNetLogs = ((CheckBox)sender).Checked;
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            ((Button)sender).Enabled = false;

            DialogResult d = TopMostMessageBox.Show(
                false,
                true,
                "This will wipe all configuration settings currently loaded, are you sure?",
                "TD Helper - Warning",
                MessageBoxButtons.YesNo);

            if (d == DialogResult.Yes)
            {
                MainForm.callForReset = true;
                Dispose();
            }

            ((Button)sender).Enabled = true;
        }

        /// <summary>
        ///  set the state of the net log path controls.
        /// </summary>
        /// <param name="state">The checked state of the checkbox.</param>
        private void SetNetlogPathState(bool state)
        {
            lblNetLogsPath.Enabled = !state;
            txtNetLogsPath.Enabled = !state;
            btnNetLogsPath.Enabled = !state;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            // load our current settings from the config
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
                txtNetLogsPath.Text = MainForm.settingsRef.NetLogPath;
            }

            if (!string.IsNullOrEmpty(MainForm.settingsRef.TreeViewFont))
            {
                // set our selected font and text box to what we've set in our config
                Font savedFont = MainForm.settingsRef.ConvertFromMemberFont();
                FontConverter fontToString = new FontConverter();
                currentTVFontBox.Text = string.Format("{0}", fontToString.ConvertToInvariantString(savedFont));
            }
            else
            {
                // set to a global default
                Font tvDefaultFont = new Font("Consolas", 8.25f);
                currentTVFontBox.Text = string.Format("{0}", MainForm.settingsRef.ConvertToFontString(tvDefaultFont));
                MainForm.settingsRef.TreeViewFont = MainForm.settingsRef.ConvertToFontString(tvDefaultFont);
            }

            rebuyPercentage.Value = MainForm.settingsRef.RebuyPercentage;
            chkQuiet.Checked = !MainForm.settingsRef.Quiet;
            testSystemsCheckBox.Checked = MainForm.settingsRef.TestSystems;
            verbosityComboBox.SelectedIndex = (int)MainForm.settingsRef.Verbosity;
            chkProgress.Checked = MainForm.settingsRef.ShowProgress;
            chkSummary.Checked = MainForm.settingsRef.Summary;
            txtLocale.Text = MainForm.settingsRef.Locale;
        }

        private void TvFontSelectorButton_Click(object sender, EventArgs e)
        {
            Font tvDefaultFont = new Font("Consolas", 8.25f);
            Font localFontObject = new Font("Consolas", 8.25f);

            FontDialog fontDialog = new FontDialog()
            {
                MinSize = 6,
                MaxSize = 32,
                Font = tvDefaultFont
            };

            if (Control.ModifierKeys == Keys.Control)
            {
                // reset our font to a preset default with a Ctrl+Click
                MainForm.settingsRef.TreeViewFont = MainForm.settingsRef.ConvertToFontString(tvDefaultFont);
            }

            if (fontDialog.ShowDialog(this) == DialogResult.OK)
            {
                localFontObject = fontDialog.Font;
                MainForm.settingsRef.TreeViewFont = MainForm.settingsRef.ConvertToFontString(localFontObject);
                currentTVFontBox.Text = string.Format("{0}", MainForm.settingsRef.TreeViewFont);
            }
            else
            {
                // set to our saved default, if that's null, set to the global default
                if (MainForm.settingsRef.TreeViewFont != null)
                    localFontObject = MainForm.settingsRef.ConvertFromMemberFont();
                else
                    localFontObject = tvDefaultFont;

                currentTVFontBox.Text = string.Format("{0}", MainForm.settingsRef.ConvertToFontString(localFontObject));
            }
        }

        private void ValidateNetLogsPath_Click(object sender, EventArgs e)
        {
            string origPath = MainForm.settingsRef.NetLogPath;
            DialogResult result = MainForm.ValidateNetLogPath(origPath, true);

            if (result == DialogResult.Cancel)
            {
                MainForm.settingsRef.NetLogPath = origPath;
            }

            txtNetLogsPath.Text = MainForm.settingsRef.NetLogPath;
        }

        private void ValidatePythonPath_Click(object sender, EventArgs e)
        {
            string origPath = MainForm.settingsRef.PythonPath;
            string origTDPath = MainForm.settingsRef.TDPath;

            MainForm.settingsRef.PythonPath = string.Empty;
            MainForm.ValidatePython(origPath);

            // adjust for Trade Dangerous Installer
            if (MainForm.settingsRef.PythonPath.EndsWith("trade.exe"))
            {
                MainForm.ValidateTDPath(origTDPath);
                tdPathBox.Text = MainForm.settingsRef.TDPath;
            }
            else if (MainForm.settingsRef.PythonPath.EndsWith("python.exe")
                && !MainForm.CheckIfFileOpens(Utilities.GetPathToTradePy()))
            {
                MainForm.settingsRef.TDPath = string.Empty;
                MainForm.ValidateTDPath(origTDPath);
                tdPathBox.Text = MainForm.settingsRef.TDPath;
            }

            pythonPathBox.Text = MainForm.settingsRef.PythonPath;
        }

        private void ValidateTDPath_Click(object sender, EventArgs e)
        {
            string origPath = MainForm.settingsRef.TDPath;
            string origPyPath = MainForm.settingsRef.PythonPath;

            // if we're using Trade Dangerous Installer, wipe our interpreter path first
            if (origPyPath.EndsWith("trade.exe"))
                MainForm.settingsRef.PythonPath = string.Empty;

            MainForm.settingsRef.TDPath = string.Empty;
            MainForm.ValidateTDPath(origPath);

            if (origPyPath.EndsWith("trade.exe"))
            {
                MainForm.ValidatePython(origPyPath);
                pythonPathBox.Text = MainForm.settingsRef.PythonPath;
            }

            tdPathBox.Text = MainForm.settingsRef.TDPath;
        }
    }
}