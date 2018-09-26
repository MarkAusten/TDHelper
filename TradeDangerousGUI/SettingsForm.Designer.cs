namespace TDHelper
{
    partial class SettingsForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.extraRunParameters = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.overrideDisableNetLogs = new System.Windows.Forms.CheckBox();
            this.overrideDoNotUpdate = new System.Windows.Forms.CheckBox();
            this.overrideCopySystemToClipboard = new System.Windows.Forms.CheckBox();
            this.overrideGroupBox = new System.Windows.Forms.GroupBox();
            this.chkSummary = new System.Windows.Forms.CheckBox();
            this.chkProgress = new System.Windows.Forms.CheckBox();
            this.testSystemsCheckBox = new System.Windows.Forms.CheckBox();
            this.verboseLabel = new System.Windows.Forms.Label();
            this.verbosityComboBox = new System.Windows.Forms.ComboBox();
            this.validateEdcePath = new System.Windows.Forms.Button();
            this.edcePathBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tvFontSelectorButton = new System.Windows.Forms.Button();
            this.currentTVFontBox = new System.Windows.Forms.TextBox();
            this.currentTVFontLabel = new System.Windows.Forms.Label();
            this.btnNetLogsPath = new System.Windows.Forms.Button();
            this.validateTDPath = new System.Windows.Forms.Button();
            this.validatePythonPath = new System.Windows.Forms.Button();
            this.txtNetLogsPath = new System.Windows.Forms.TextBox();
            this.tdPathBox = new System.Windows.Forms.TextBox();
            this.pythonPathBox = new System.Windows.Forms.TextBox();
            this.lblNetLogsPath = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.resetButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.rebuyPercentage = new System.Windows.Forms.NumericUpDown();
            this.miscGroupBox = new System.Windows.Forms.GroupBox();
            this.chkQuiet = new System.Windows.Forms.CheckBox();
            this.lblRebuyPercentage = new System.Windows.Forms.Label();
            this.lblLocale = new System.Windows.Forms.Label();
            this.txtLocale = new System.Windows.Forms.TextBox();
            this.overrideGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rebuyPercentage)).BeginInit();
            this.miscGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // extraRunParameters
            // 
            this.extraRunParameters.Location = new System.Drawing.Point(97, 254);
            this.extraRunParameters.Name = "extraRunParameters";
            this.extraRunParameters.Size = new System.Drawing.Size(257, 20);
            this.extraRunParameters.TabIndex = 13;
            this.toolTip1.SetToolTip(this.extraRunParameters, "This text will be added on to the end of the Run command [caution!]");
            this.extraRunParameters.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Generic_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 257);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Run parameters:";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(300, 424);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 28);
            this.okButton.TabIndex = 16;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.Location = new System.Drawing.Point(12, 424);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 28);
            this.cancelButton.TabIndex = 14;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // overrideDisableNetLogs
            // 
            this.overrideDisableNetLogs.AutoSize = true;
            this.overrideDisableNetLogs.Location = new System.Drawing.Point(6, 14);
            this.overrideDisableNetLogs.Name = "overrideDisableNetLogs";
            this.overrideDisableNetLogs.Size = new System.Drawing.Size(107, 17);
            this.overrideDisableNetLogs.TabIndex = 2;
            this.overrideDisableNetLogs.Text = "Disable Net Logs";
            this.overrideDisableNetLogs.UseVisualStyleBackColor = true;
            this.overrideDisableNetLogs.CheckedChanged += new System.EventHandler(this.OverrideDisableNetLogs_CheckedChanged);
            // 
            // overrideDoNotUpdate
            // 
            this.overrideDoNotUpdate.AutoSize = true;
            this.overrideDoNotUpdate.Location = new System.Drawing.Point(6, 37);
            this.overrideDoNotUpdate.Name = "overrideDoNotUpdate";
            this.overrideDoNotUpdate.Size = new System.Drawing.Size(130, 17);
            this.overrideDoNotUpdate.TabIndex = 3;
            this.overrideDoNotUpdate.Text = "Disable Auto-updating";
            this.overrideDoNotUpdate.UseVisualStyleBackColor = true;
            // 
            // overrideCopySystemToClipboard
            // 
            this.overrideCopySystemToClipboard.AutoSize = true;
            this.overrideCopySystemToClipboard.Location = new System.Drawing.Point(6, 60);
            this.overrideCopySystemToClipboard.Name = "overrideCopySystemToClipboard";
            this.overrideCopySystemToClipboard.Size = new System.Drawing.Size(244, 17);
            this.overrideCopySystemToClipboard.TabIndex = 4;
            this.overrideCopySystemToClipboard.Text = "Copy unrecognized system names to clipboard";
            this.overrideCopySystemToClipboard.UseVisualStyleBackColor = true;
            // 
            // overrideGroupBox
            // 
            this.overrideGroupBox.Controls.Add(this.chkSummary);
            this.overrideGroupBox.Controls.Add(this.chkProgress);
            this.overrideGroupBox.Controls.Add(this.testSystemsCheckBox);
            this.overrideGroupBox.Controls.Add(this.verboseLabel);
            this.overrideGroupBox.Controls.Add(this.verbosityComboBox);
            this.overrideGroupBox.Controls.Add(this.validateEdcePath);
            this.overrideGroupBox.Controls.Add(this.edcePathBox);
            this.overrideGroupBox.Controls.Add(this.label5);
            this.overrideGroupBox.Controls.Add(this.tvFontSelectorButton);
            this.overrideGroupBox.Controls.Add(this.currentTVFontBox);
            this.overrideGroupBox.Controls.Add(this.currentTVFontLabel);
            this.overrideGroupBox.Controls.Add(this.btnNetLogsPath);
            this.overrideGroupBox.Controls.Add(this.validateTDPath);
            this.overrideGroupBox.Controls.Add(this.validatePythonPath);
            this.overrideGroupBox.Controls.Add(this.txtNetLogsPath);
            this.overrideGroupBox.Controls.Add(this.tdPathBox);
            this.overrideGroupBox.Controls.Add(this.pythonPathBox);
            this.overrideGroupBox.Controls.Add(this.lblNetLogsPath);
            this.overrideGroupBox.Controls.Add(this.label3);
            this.overrideGroupBox.Controls.Add(this.label2);
            this.overrideGroupBox.Controls.Add(this.overrideCopySystemToClipboard);
            this.overrideGroupBox.Controls.Add(this.overrideDisableNetLogs);
            this.overrideGroupBox.Controls.Add(this.overrideDoNotUpdate);
            this.overrideGroupBox.Controls.Add(this.label1);
            this.overrideGroupBox.Controls.Add(this.extraRunParameters);
            this.overrideGroupBox.Location = new System.Drawing.Point(12, 12);
            this.overrideGroupBox.Name = "overrideGroupBox";
            this.overrideGroupBox.Size = new System.Drawing.Size(363, 318);
            this.overrideGroupBox.TabIndex = 7;
            this.overrideGroupBox.TabStop = false;
            this.overrideGroupBox.Text = "Overrides";
            // 
            // chkSummary
            // 
            this.chkSummary.AutoSize = true;
            this.chkSummary.Location = new System.Drawing.Point(259, 290);
            this.chkSummary.Name = "chkSummary";
            this.chkSummary.Size = new System.Drawing.Size(69, 17);
            this.chkSummary.TabIndex = 69;
            this.chkSummary.TabStop = false;
            this.chkSummary.Text = "Summary";
            this.toolTip1.SetToolTip(this.chkSummary, "Show run output in summary form.");
            this.chkSummary.UseVisualStyleBackColor = true;
            // 
            // chkProgress
            // 
            this.chkProgress.AutoSize = true;
            this.chkProgress.Location = new System.Drawing.Point(156, 290);
            this.chkProgress.Name = "chkProgress";
            this.chkProgress.Size = new System.Drawing.Size(97, 17);
            this.chkProgress.TabIndex = 68;
            this.chkProgress.TabStop = false;
            this.chkProgress.Text = "Show Progress";
            this.toolTip1.SetToolTip(this.chkProgress, "Show the progress of the calculations.");
            this.chkProgress.UseVisualStyleBackColor = true;
            // 
            // testSystemsCheckBox
            // 
            this.testSystemsCheckBox.AutoSize = true;
            this.testSystemsCheckBox.Location = new System.Drawing.Point(6, 83);
            this.testSystemsCheckBox.Name = "testSystemsCheckBox";
            this.testSystemsCheckBox.Size = new System.Drawing.Size(210, 17);
            this.testSystemsCheckBox.TabIndex = 61;
            this.testSystemsCheckBox.TabStop = false;
            this.testSystemsCheckBox.Text = "Notify when entering unknown systems";
            this.testSystemsCheckBox.UseVisualStyleBackColor = true;
            // 
            // verboseLabel
            // 
            this.verboseLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.verboseLabel.AutoSize = true;
            this.verboseLabel.Location = new System.Drawing.Point(38, 291);
            this.verboseLabel.Name = "verboseLabel";
            this.verboseLabel.Size = new System.Drawing.Size(53, 13);
            this.verboseLabel.TabIndex = 60;
            this.verboseLabel.Text = "Verbosity:";
            this.toolTip1.SetToolTip(this.verboseLabel, "Verbosity of output results");
            // 
            // verbosityComboBox
            // 
            this.verbosityComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.verbosityComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.verbosityComboBox.Items.AddRange(new object[] {
            "",
            "-v",
            "-vv",
            "-vvv"});
            this.verbosityComboBox.Location = new System.Drawing.Point(97, 288);
            this.verbosityComboBox.Name = "verbosityComboBox";
            this.verbosityComboBox.Size = new System.Drawing.Size(46, 21);
            this.verbosityComboBox.TabIndex = 59;
            this.verbosityComboBox.TabStop = false;
            // 
            // validateEdcePath
            // 
            this.validateEdcePath.Location = new System.Drawing.Point(330, 219);
            this.validateEdcePath.Name = "validateEdcePath";
            this.validateEdcePath.Size = new System.Drawing.Size(24, 20);
            this.validateEdcePath.TabIndex = 16;
            this.validateEdcePath.Text = "...";
            this.validateEdcePath.UseVisualStyleBackColor = true;
            this.validateEdcePath.Click += new System.EventHandler(this.ValidateEdcePath_Click);
            // 
            // edcePathBox
            // 
            this.edcePathBox.Location = new System.Drawing.Point(97, 219);
            this.edcePathBox.Name = "edcePathBox";
            this.edcePathBox.Size = new System.Drawing.Size(227, 20);
            this.edcePathBox.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 222);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "EDCE Path:";
            // 
            // tvFontSelectorButton
            // 
            this.tvFontSelectorButton.Location = new System.Drawing.Point(331, 115);
            this.tvFontSelectorButton.Name = "tvFontSelectorButton";
            this.tvFontSelectorButton.Size = new System.Drawing.Size(24, 20);
            this.tvFontSelectorButton.TabIndex = 6;
            this.tvFontSelectorButton.Text = "...";
            this.toolTip1.SetToolTip(this.tvFontSelectorButton, "Ctrl+Click to reset the TreeView font to the default");
            this.tvFontSelectorButton.UseVisualStyleBackColor = true;
            this.tvFontSelectorButton.Click += new System.EventHandler(this.TvFontSelectorButton_Click);
            // 
            // currentTVFontBox
            // 
            this.currentTVFontBox.Location = new System.Drawing.Point(98, 115);
            this.currentTVFontBox.Name = "currentTVFontBox";
            this.currentTVFontBox.Size = new System.Drawing.Size(227, 20);
            this.currentTVFontBox.TabIndex = 5;
            // 
            // currentTVFontLabel
            // 
            this.currentTVFontLabel.AutoSize = true;
            this.currentTVFontLabel.Location = new System.Drawing.Point(13, 118);
            this.currentTVFontLabel.Name = "currentTVFontLabel";
            this.currentTVFontLabel.Size = new System.Drawing.Size(79, 13);
            this.currentTVFontLabel.TabIndex = 13;
            this.currentTVFontLabel.Text = "TreeView Font:";
            // 
            // btnNetLogsPath
            // 
            this.btnNetLogsPath.Location = new System.Drawing.Point(331, 193);
            this.btnNetLogsPath.Name = "btnNetLogsPath";
            this.btnNetLogsPath.Size = new System.Drawing.Size(24, 20);
            this.btnNetLogsPath.TabIndex = 12;
            this.btnNetLogsPath.Text = "...";
            this.btnNetLogsPath.UseVisualStyleBackColor = true;
            this.btnNetLogsPath.Click += new System.EventHandler(this.ValidateNetLogsPath_Click);
            // 
            // validateTDPath
            // 
            this.validateTDPath.Location = new System.Drawing.Point(331, 167);
            this.validateTDPath.Name = "validateTDPath";
            this.validateTDPath.Size = new System.Drawing.Size(24, 20);
            this.validateTDPath.TabIndex = 10;
            this.validateTDPath.Text = "...";
            this.validateTDPath.UseVisualStyleBackColor = true;
            this.validateTDPath.Click += new System.EventHandler(this.ValidateTDPath_Click);
            // 
            // validatePythonPath
            // 
            this.validatePythonPath.Location = new System.Drawing.Point(331, 141);
            this.validatePythonPath.Name = "validatePythonPath";
            this.validatePythonPath.Size = new System.Drawing.Size(24, 20);
            this.validatePythonPath.TabIndex = 8;
            this.validatePythonPath.Text = "...";
            this.validatePythonPath.UseVisualStyleBackColor = true;
            this.validatePythonPath.Click += new System.EventHandler(this.ValidatePythonPath_Click);
            // 
            // txtNetLogsPath
            // 
            this.txtNetLogsPath.Location = new System.Drawing.Point(98, 193);
            this.txtNetLogsPath.Name = "txtNetLogsPath";
            this.txtNetLogsPath.Size = new System.Drawing.Size(227, 20);
            this.txtNetLogsPath.TabIndex = 11;
            // 
            // tdPathBox
            // 
            this.tdPathBox.Location = new System.Drawing.Point(98, 167);
            this.tdPathBox.Name = "tdPathBox";
            this.tdPathBox.Size = new System.Drawing.Size(227, 20);
            this.tdPathBox.TabIndex = 9;
            // 
            // pythonPathBox
            // 
            this.pythonPathBox.Location = new System.Drawing.Point(98, 141);
            this.pythonPathBox.Name = "pythonPathBox";
            this.pythonPathBox.Size = new System.Drawing.Size(227, 20);
            this.pythonPathBox.TabIndex = 7;
            // 
            // lblNetLogsPath
            // 
            this.lblNetLogsPath.AutoSize = true;
            this.lblNetLogsPath.Location = new System.Drawing.Point(14, 197);
            this.lblNetLogsPath.Name = "lblNetLogsPath";
            this.lblNetLogsPath.Size = new System.Drawing.Size(78, 13);
            this.lblNetLogsPath.TabIndex = 6;
            this.lblNetLogsPath.Text = "Net Logs Path:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Python Path:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 170);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "TD Path:";
            // 
            // resetButton
            // 
            this.resetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resetButton.Location = new System.Drawing.Point(151, 424);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(84, 28);
            this.resetButton.TabIndex = 15;
            this.resetButton.Text = "Reset Settings";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // rebuyPercentage
            // 
            this.rebuyPercentage.DecimalPlaces = 2;
            this.rebuyPercentage.Location = new System.Drawing.Point(111, 19);
            this.rebuyPercentage.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            131072});
            this.rebuyPercentage.Name = "rebuyPercentage";
            this.rebuyPercentage.Size = new System.Drawing.Size(60, 20);
            this.rebuyPercentage.TabIndex = 5;
            this.rebuyPercentage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.rebuyPercentage, "Percentage of ship cost for rebuy.");
            // 
            // miscGroupBox
            // 
            this.miscGroupBox.Controls.Add(this.lblLocale);
            this.miscGroupBox.Controls.Add(this.txtLocale);
            this.miscGroupBox.Controls.Add(this.chkQuiet);
            this.miscGroupBox.Controls.Add(this.rebuyPercentage);
            this.miscGroupBox.Controls.Add(this.lblRebuyPercentage);
            this.miscGroupBox.Location = new System.Drawing.Point(12, 336);
            this.miscGroupBox.Name = "miscGroupBox";
            this.miscGroupBox.Size = new System.Drawing.Size(363, 82);
            this.miscGroupBox.TabIndex = 17;
            this.miscGroupBox.TabStop = false;
            this.miscGroupBox.Text = "Misc.";
            // 
            // chkQuiet
            // 
            this.chkQuiet.AutoSize = true;
            this.chkQuiet.Location = new System.Drawing.Point(216, 22);
            this.chkQuiet.Name = "chkQuiet";
            this.chkQuiet.Size = new System.Drawing.Size(75, 17);
            this.chkQuiet.TabIndex = 6;
            this.chkQuiet.Text = "Play Alerts";
            this.chkQuiet.UseVisualStyleBackColor = true;
            // 
            // lblRebuyPercentage
            // 
            this.lblRebuyPercentage.AutoSize = true;
            this.lblRebuyPercentage.Location = new System.Drawing.Point(6, 22);
            this.lblRebuyPercentage.Name = "lblRebuyPercentage";
            this.lblRebuyPercentage.Size = new System.Drawing.Size(99, 13);
            this.lblRebuyPercentage.TabIndex = 2;
            this.lblRebuyPercentage.Text = "Rebuy Percentage:";
            // 
            // lblLocale
            // 
            this.lblLocale.AutoSize = true;
            this.lblLocale.Location = new System.Drawing.Point(7, 49);
            this.lblLocale.Name = "lblLocale";
            this.lblLocale.Size = new System.Drawing.Size(42, 13);
            this.lblLocale.TabIndex = 14;
            this.lblLocale.Text = "Locale:";
            // 
            // txtLocale
            // 
            this.txtLocale.Location = new System.Drawing.Point(97, 46);
            this.txtLocale.Name = "txtLocale";
            this.txtLocale.Size = new System.Drawing.Size(74, 20);
            this.txtLocale.TabIndex = 15;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 459);
            this.Controls.Add(this.miscGroupBox);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.overrideGroupBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.Text = "Misc. Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Generic_KeyDown);
            this.overrideGroupBox.ResumeLayout(false);
            this.overrideGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rebuyPercentage)).EndInit();
            this.miscGroupBox.ResumeLayout(false);
            this.miscGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox extraRunParameters;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox overrideDisableNetLogs;
        private System.Windows.Forms.CheckBox overrideDoNotUpdate;
        private System.Windows.Forms.CheckBox overrideCopySystemToClipboard;
        private System.Windows.Forms.GroupBox overrideGroupBox;
        private System.Windows.Forms.Label lblNetLogsPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnNetLogsPath;
        private System.Windows.Forms.Button validateTDPath;
        private System.Windows.Forms.Button validatePythonPath;
        private System.Windows.Forms.TextBox txtNetLogsPath;
        private System.Windows.Forms.TextBox tdPathBox;
        private System.Windows.Forms.TextBox pythonPathBox;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Button tvFontSelectorButton;
        private System.Windows.Forms.TextBox currentTVFontBox;
        private System.Windows.Forms.Label currentTVFontLabel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox miscGroupBox;
        private System.Windows.Forms.Label lblRebuyPercentage;
        private System.Windows.Forms.NumericUpDown rebuyPercentage;
        private System.Windows.Forms.Button validateEdcePath;
        private System.Windows.Forms.TextBox edcePathBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkQuiet;
        private System.Windows.Forms.CheckBox testSystemsCheckBox;
        private System.Windows.Forms.Label verboseLabel;
        private System.Windows.Forms.ComboBox verbosityComboBox;
        private System.Windows.Forms.CheckBox chkProgress;
        private System.Windows.Forms.CheckBox chkSummary;
        private System.Windows.Forms.Label lblLocale;
        private System.Windows.Forms.TextBox txtLocale;
    }
}