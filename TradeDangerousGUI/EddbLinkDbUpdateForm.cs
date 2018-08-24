using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace TDHelper
{
    public partial class EddbLinkDbUpdateForm : Form
    {
        public EddbLinkDbUpdateForm()
        {
            InitializeComponent();

            ResetToDefault();
        }

        /// <summary>
        /// Check the state of the params.
        /// </summary>
        private void CheckParameterState()
        {
            // Run 'listings' by default:
            // If no options, or if only 'force', 'skipvend', and/or 'fallback',
            // have been checked, enable 'listings'.
            IList<string> checkedControls = Controls.OfType<CheckBox>()
                .Where(x => x.Checked)
                .Select(x => x.Name)
                .ToList();

            if (checkedControls.Contains("chkFallback") ||
                checkedControls.Contains("chkForce") ||
                checkedControls.Contains("chkSkipvend"))
            {
                chkListings.Checked = true;
            }
        }

        /// <summary>
        /// The event handler for all checked changed.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void EventHandler_All_CheckedChanged(object sender, EventArgs e)
        {
            SetAll();
        }

        /// <summary>
        /// The event handler for Clean checked changed.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void EventHandler_Clean_CheckedChanged(object sender, EventArgs e)
        {
            SetClean();
        }

        /// <summary>
        /// The event handler for form closing.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void EventHandler_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        /// <summary>
        /// The event handler for item checked changed.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void EventHandler_Item_CheckedChanged(object sender, EventArgs e)
        {
            SetItem();
        }

        /// <summary>
        /// The event handler for listings checked changed.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void EventHandler_Listings_CheckedChanged(object sender, EventArgs e)
        {
            SetListings();
        }

        /// <summary>
        /// The event handler for reset clicked.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void EventHandler_Reset_Click(object sender, EventArgs e)
        {
            ResetToDefault();
        }

        /// <summary>
        /// The event handler for shipvend checked changed.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void EventHandler_Shipvend_CheckedChanged(object sender, EventArgs e)
        {
            SetShipvend();
        }

        /// <summary>
        /// The event handler for skipvend checked changed.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void EventHandler_Skipvend_CheckedChanged(object sender, EventArgs e)
        {
            SetSkipvend();
            SetAll();
        }

        /// <summary>
        /// The event handler for solo checked changed.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void EventHandler_Solo_CheckedChanged(object sender, EventArgs e)
        {
            SetSolo();
            SetAll();
        }

        /// <summary>
        /// The event handler for station checked changed.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void EventHandler_Station_CheckedChanged(object sender, EventArgs e)
        {
            SetStation();
        }

        /// <summary>
        /// The event handler for UPdate DB clicked.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void EventHandler_UpdateDb_Click(object sender, EventArgs e)
        {
            ProcessDdUpdateRequest();
        }

        /// <summary>
        /// The event handler for upvend checked changed.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void EventHandler_Upvend_CheckedChanged(object sender, EventArgs e)
        {
            SetUpvend();
        }

        /// <summary>
        /// Generate the DB update command string.
        /// </summary>
        /// <returns>The DB update command string.</returns>
        private string GetDbUpdateCommandString()
        {
            string result = string.Empty;

            // Get the options for all the checked controls.
            foreach (string option in Controls.OfType<CheckBox>()
                .Where(x => x.Checked)
                .Select(x => x.Tag.ToString().Trim()))
            {
                result += ",{0}".With(option);
            }

            // Remove the leading comma if required and remove the unnecessary options.
            if (!string.IsNullOrEmpty(result))
            {
                result += ",";

                if (result.Contains(",station,"))
                {
                    result = result.Replace(",system,", ",");
                }

                if (result.Contains(",upvend,"))
                {
                    result = result
                        .Replace(",station,", ",")
                        .Replace(",upgrade,", ",");
                }

                if (result.Contains(",shipvend,"))
                {
                    result = result
                        .Replace(",station,", ",")
                        .Replace(",ship,", ",");
                }

                if (result.Contains(",listings,"))
                {
                    result = result
                        .Replace(",item,", ",")
                        .Replace(",station,", ",");
                }

                if (result.Contains(",all,"))
                {
                    result = result
                        .Replace(",listings,", ",")
                        .Replace(",shipvend,", ",")
                        .Replace(",upvend,", ",");
                }

                if (result.Contains(",clean,"))
                {
                    result = result
                        .Replace(",all,", ",")
                        .Replace(",force", ",");
                }

                if (result.Contains(",skipvend,"))
                {
                    result = result
                        .Replace(",shipvend", ",")
                        .Replace(",upvend", ",");
                }

                if (result.Contains(",solo,"))
                {
                    result = result
                        .Replace(",listings,", ",")
                        .Replace(",skipvend", ",");
                }
            }

            // Remove the leading and trailing comma if required.
            if (!string.IsNullOrEmpty(result))
            {
                result = result.Trim(',');
            }

            return result;
        }

        /// <summary>
        /// Process the reqest for a database update.
        /// </summary>
        private void ProcessDdUpdateRequest()
        {
            // Check for the options needing warnings.
            if (chkClean.Checked)
            {
                // Display warning message and ask for input.
                DialogResult dialog = TopMostMessageBox.Show(
                    true,
                    true,
                    "The clean option will erase the entire database and rebuild from empty. Are you sure you want to do this?",
                    "TD Helper - Warning",
                    MessageBoxButtons.YesNoCancel);

                switch (dialog)
                {
                    case DialogResult.Cancel:
                        // Do nothing and leave the form open.
                        break;

                    case DialogResult.No:
                        // Reset the output parameters and close the form.
                        MainForm.DBUpdateCommandString = string.Empty;

                        Close();

                        break;

                    case DialogResult.Yes:
                        // Set the output parameters and close the form.
                        MainForm.DBUpdateCommandString = GetDbUpdateCommandString();

                        Close();

                        break;
                }
            }
            else
            {
                MainForm.DBUpdateCommandString = GetDbUpdateCommandString();

                Close();
            }
        }

        /// <summary>
        /// Reset the check box states to default.
        /// </summary>
        private void ResetToDefault()
        {
            UncheckAndEnableAll();

            // Skipvend is a persistent setting so deal with it.
            chkSkipvend.Checked = MainForm.settingsRef.SkipVend;

            // Solo is a persistant setting so check for that setting.
            if (MainForm.settingsRef.Solo)
            {
                chkSolo.Checked = true;
            }
            else
            {
                chkListings.Checked = true;
            }
        }

        /// <summary>
        /// Save any persistant settings to the settings object.
        /// </summary>
        private void SaveSettings()
        {
            if (chkSkipvend.Checked != MainForm.settingsRef.SkipVend)
            {
                MainForm.settingsRef.SkipVend = chkSkipvend.Checked;
            }

            if (chkSolo.Checked != MainForm.settingsRef.Solo)
            {
                MainForm.settingsRef.Solo = chkSolo.Checked;
            }
        }

        /// <summary>
        /// Set the controls for all.
        /// </summary>
        private void SetAll()
        {
            if (chkAll.Checked)
            {
                SetControlCheckedAndDisabled(chkItem);
                SetControlCheckedAndDisabled(chkShip);
                SetControlCheckedAndDisabled(chkStation);
                SetControlCheckedAndDisabled(chkSystem);
                SetControlCheckedAndDisabled(chkUpgrade);

                if (!chkSkipvend.Checked)
                {
                    SetControlCheckedAndDisabled(chkShipvend);
                    SetControlCheckedAndDisabled(chkUpvend);
                }

                if (!chkSolo.Checked)
                {
                    SetControlCheckedAndDisabled(chkListings);
                }
            }
            else
            {
                SetControlUncheckedAndEnabled(chkItem);
                SetControlUncheckedAndEnabled(chkShip);
                SetControlUncheckedAndEnabled(chkStation);
                SetControlUncheckedAndEnabled(chkSystem);
                SetControlUncheckedAndEnabled(chkUpgrade);

                if (!chkSkipvend.Checked)
                {
                    SetControlUncheckedAndEnabled(chkShipvend);
                    SetControlUncheckedAndEnabled(chkUpvend);
                }

                if (!chkSolo.Checked)
                {
                    SetControlUncheckedAndEnabled(chkListings);
                }
            }
        }

        /// <summary>
        /// Set the controls for clean.
        /// </summary>
        private void SetClean()
        {
            if (chkClean.Checked)
            {
                SetControlCheckedAndDisabled(chkAll);
                SetControlCheckedAndDisabled(chkForce);
            }
            else
            {
                SetControlUncheckedAndEnabled(chkAll);
                SetControlUncheckedAndEnabled(chkForce);
            }
        }

        /// <summary>
        /// Set the specified control disabled and checked.
        /// </summary>
        /// <param name="control"></param>
        private void SetControlCheckedAndDisabled(CheckBox control)
        {
            control.Checked = true;
            control.Enabled = false;
        }

        /// <summary>
        /// Set the specified control disabled and unchecked.
        /// </summary>
        /// <param name="control"></param>
        private void SetControlUncheckedAndDisabled(CheckBox control)
        {
            control.Checked = false;
            control.Enabled = false;
        }

        /// <summary>
        /// Set the specified control disabled and checked.
        /// </summary>
        /// <param name="control"></param>
        private void SetControlUncheckedAndEnabled(CheckBox control)
        {
            control.Checked = false;
            control.Enabled = true;
        }

        /// <summary>
        /// Set the controls for listings.
        /// </summary>
        private void SetItem()
        {
            if (chkItem.Checked)
            {
                SetControlCheckedAndDisabled(chkStation);
            }
            else
            {
                SetControlUncheckedAndEnabled(chkStation);
            }
        }

        /// <summary>
        /// Set the controls for listings.
        /// </summary>
        private void SetListings()
        {
            if (chkListings.Checked)
            {
                SetControlCheckedAndDisabled(chkItem);
            }
            else
            {
                SetControlUncheckedAndEnabled(chkItem);
            }
        }

        /// <summary>
        /// Set the controls for shipvend.
        /// </summary>
        private void SetShipvend()
        {
            if (chkShipvend.Checked)
            {
                SetControlCheckedAndDisabled(chkShip);
                SetControlCheckedAndDisabled(chkStation);
            }
            else
            {
                SetControlUncheckedAndEnabled(chkShip);
                SetControlUncheckedAndEnabled(chkStation);
            }
        }

        /// <summary>
        /// Set the controls for skipvend.
        /// </summary>
        private void SetSkipvend()
        {
            if (chkSkipvend.Checked)
            {
                SetControlUncheckedAndDisabled(chkShipvend);
                SetControlUncheckedAndDisabled(chkUpvend);
            }
            else
            {
                SetControlUncheckedAndEnabled(chkShipvend);
                SetControlUncheckedAndEnabled(chkUpvend);
            }
        }

        /// <summary>
        /// Set the controls for solo;
        /// </summary>
        private void SetSolo()
        {
            if (chkSolo.Checked)
            {
                SetControlUncheckedAndDisabled(chkListings);
                SetControlCheckedAndDisabled(chkSkipvend);
            }
            else
            {
                SetControlUncheckedAndEnabled(chkListings);
                SetControlUncheckedAndEnabled(chkSkipvend);
            }
        }

        /// <summary>
        /// Set the controls for station.
        /// </summary>
        private void SetStation()
        {
            if (chkStation.Checked)
            {
                SetControlCheckedAndDisabled(chkSystem);
            }
            else
            {
                SetControlUncheckedAndEnabled(chkSystem);
            }
        }

        /// <summary>
        /// Set the controls for upvend.
        /// </summary>
        private void SetUpvend()
        {
            if (chkUpvend.Checked)
            {
                SetControlCheckedAndDisabled(chkUpgrade);
                SetControlCheckedAndDisabled(chkStation);
            }
            else
            {
                SetControlUncheckedAndEnabled(chkUpgrade);
                SetControlUncheckedAndEnabled(chkStation);
            }
        }

        /// <summary>
        /// Uncheck and enable all checkboxes.
        /// </summary>
        private void UncheckAndEnableAll(CheckBox except = null)
        {
            foreach (CheckBox control in Controls.OfType<CheckBox>())
            {
                if (except == null || control.Name != except.Name)
                {
                    control.Checked = false;
                    control.Enabled = true;
                }
            }
        }
    }
}