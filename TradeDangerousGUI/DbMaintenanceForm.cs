using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace TDHelper
{
    public partial class DbMaintenanceForm : Form
    {
        public DbMaintenanceForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Process the reqest for a database update.
        /// </summary>
        private void AnalyseDatabase()
        {
            // Display warning message and ask for input.
            DialogResult dialog = TopMostMessageBox.Show(
                true,
                true,
                "This may take a very long time (~20 minutes). Are you sure you want to do this?",
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
                    MainForm.DBUpdateCommandString = "ANALYZE";

                    Close();

                    break;
            }
        }

        /// <summary>
        /// Process the reqest for a database vacuum.
        /// </summary>
        private void VacuumDatabase()
        {
            // Display warning message and ask for input.
            DialogResult dialog = TopMostMessageBox.Show(
                true,
                true,
                "This may take an excessively long time, possibly many hours. Are you sure you want to do this?",
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
                    MainForm.DBUpdateCommandString = "VACUUM";

                    Close();

                    break;
            }
        }

        /// <summary>
        /// The event handler for analye clicked.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void EventHandler_Analyse_Click(object sender, EventArgs e)
        {
            AnalyseDatabase();
        }

        /// <summary>
        /// The event handler for close clicked.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void EventHandler_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// The event handler for vacuum clicked.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void EventHandler_Vacuum_Click(object sender, EventArgs e)
        {
            VacuumDatabase();
        }
    }
}