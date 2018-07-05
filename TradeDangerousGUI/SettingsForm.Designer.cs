﻿namespace TDHelper
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
            this.validateEdcePath = new System.Windows.Forms.Button();
            this.edcePathBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tvFontSelectorButton = new System.Windows.Forms.Button();
            this.currentTVFontBox = new System.Windows.Forms.TextBox();
            this.currentTVFontLabel = new System.Windows.Forms.Label();
            this.validateNetLogsPath = new System.Windows.Forms.Button();
            this.validateTDPath = new System.Windows.Forms.Button();
            this.validatePythonPath = new System.Windows.Forms.Button();
            this.netLogsPathBox = new System.Windows.Forms.TextBox();
            this.tdPathBox = new System.Windows.Forms.TextBox();
            this.pythonPathBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.resetButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.rebuyPercentage = new System.Windows.Forms.NumericUpDown();
            this.miscGroupBox = new System.Windows.Forms.GroupBox();
            this.lblRebuyPercentage = new System.Windows.Forms.Label();
            this.overrideGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rebuyPercentage)).BeginInit();
            this.miscGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // extraRunParameters
            // 
            this.extraRunParameters.Location = new System.Drawing.Point(96, 225);
            this.extraRunParameters.Name = "extraRunParameters";
            this.extraRunParameters.Size = new System.Drawing.Size(257, 20);
            this.extraRunParameters.TabIndex = 13;
            this.toolTip1.SetToolTip(this.extraRunParameters, "This text will be added on to the end of the Run command [caution!]");
            this.extraRunParameters.KeyDown += new System.Windows.Forms.KeyEventHandler(this.generic_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 228);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Run parameters:";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(300, 345);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 16;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.Location = new System.Drawing.Point(12, 345);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 14;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
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
            this.overrideGroupBox.Controls.Add(this.validateEdcePath);
            this.overrideGroupBox.Controls.Add(this.edcePathBox);
            this.overrideGroupBox.Controls.Add(this.label5);
            this.overrideGroupBox.Controls.Add(this.tvFontSelectorButton);
            this.overrideGroupBox.Controls.Add(this.currentTVFontBox);
            this.overrideGroupBox.Controls.Add(this.currentTVFontLabel);
            this.overrideGroupBox.Controls.Add(this.validateNetLogsPath);
            this.overrideGroupBox.Controls.Add(this.validateTDPath);
            this.overrideGroupBox.Controls.Add(this.validatePythonPath);
            this.overrideGroupBox.Controls.Add(this.netLogsPathBox);
            this.overrideGroupBox.Controls.Add(this.tdPathBox);
            this.overrideGroupBox.Controls.Add(this.pythonPathBox);
            this.overrideGroupBox.Controls.Add(this.label4);
            this.overrideGroupBox.Controls.Add(this.label3);
            this.overrideGroupBox.Controls.Add(this.label2);
            this.overrideGroupBox.Controls.Add(this.overrideCopySystemToClipboard);
            this.overrideGroupBox.Controls.Add(this.overrideDisableNetLogs);
            this.overrideGroupBox.Controls.Add(this.overrideDoNotUpdate);
            this.overrideGroupBox.Controls.Add(this.label1);
            this.overrideGroupBox.Controls.Add(this.extraRunParameters);
            this.overrideGroupBox.Location = new System.Drawing.Point(12, 12);
            this.overrideGroupBox.Name = "overrideGroupBox";
            this.overrideGroupBox.Size = new System.Drawing.Size(363, 263);
            this.overrideGroupBox.TabIndex = 7;
            this.overrideGroupBox.TabStop = false;
            this.overrideGroupBox.Text = "Overrides";
            // 
            // validateEdcePath
            // 
            this.validateEdcePath.Location = new System.Drawing.Point(329, 190);
            this.validateEdcePath.Name = "validateEdcePath";
            this.validateEdcePath.Size = new System.Drawing.Size(24, 20);
            this.validateEdcePath.TabIndex = 16;
            this.validateEdcePath.Text = "...";
            this.validateEdcePath.UseVisualStyleBackColor = true;
            this.validateEdcePath.Click += new System.EventHandler(this.validateEdcePath_Click);
            // 
            // edcePathBox
            // 
            this.edcePathBox.Location = new System.Drawing.Point(96, 190);
            this.edcePathBox.Name = "edcePathBox";
            this.edcePathBox.Size = new System.Drawing.Size(227, 20);
            this.edcePathBox.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(27, 193);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "EDCE Path:";
            // 
            // tvFontSelectorButton
            // 
            this.tvFontSelectorButton.Location = new System.Drawing.Point(330, 86);
            this.tvFontSelectorButton.Name = "tvFontSelectorButton";
            this.tvFontSelectorButton.Size = new System.Drawing.Size(24, 20);
            this.tvFontSelectorButton.TabIndex = 6;
            this.tvFontSelectorButton.Text = "...";
            this.toolTip1.SetToolTip(this.tvFontSelectorButton, "Ctrl+Click to reset the TreeView font to the default");
            this.tvFontSelectorButton.UseVisualStyleBackColor = true;
            this.tvFontSelectorButton.Click += new System.EventHandler(this.tvFontSelectorButton_Click);
            // 
            // currentTVFontBox
            // 
            this.currentTVFontBox.Location = new System.Drawing.Point(97, 86);
            this.currentTVFontBox.Name = "currentTVFontBox";
            this.currentTVFontBox.Size = new System.Drawing.Size(227, 20);
            this.currentTVFontBox.TabIndex = 5;
            // 
            // currentTVFontLabel
            // 
            this.currentTVFontLabel.AutoSize = true;
            this.currentTVFontLabel.Location = new System.Drawing.Point(12, 89);
            this.currentTVFontLabel.Name = "currentTVFontLabel";
            this.currentTVFontLabel.Size = new System.Drawing.Size(79, 13);
            this.currentTVFontLabel.TabIndex = 13;
            this.currentTVFontLabel.Text = "TreeView Font:";
            // 
            // validateNetLogsPath
            // 
            this.validateNetLogsPath.Location = new System.Drawing.Point(330, 164);
            this.validateNetLogsPath.Name = "validateNetLogsPath";
            this.validateNetLogsPath.Size = new System.Drawing.Size(24, 20);
            this.validateNetLogsPath.TabIndex = 12;
            this.validateNetLogsPath.Text = "...";
            this.validateNetLogsPath.UseVisualStyleBackColor = true;
            this.validateNetLogsPath.Click += new System.EventHandler(this.validateNetLogsPath_Click);
            // 
            // validateTDPath
            // 
            this.validateTDPath.Location = new System.Drawing.Point(330, 138);
            this.validateTDPath.Name = "validateTDPath";
            this.validateTDPath.Size = new System.Drawing.Size(24, 20);
            this.validateTDPath.TabIndex = 10;
            this.validateTDPath.Text = "...";
            this.validateTDPath.UseVisualStyleBackColor = true;
            this.validateTDPath.Click += new System.EventHandler(this.validateTDPath_Click);
            // 
            // validatePythonPath
            // 
            this.validatePythonPath.Location = new System.Drawing.Point(330, 112);
            this.validatePythonPath.Name = "validatePythonPath";
            this.validatePythonPath.Size = new System.Drawing.Size(24, 20);
            this.validatePythonPath.TabIndex = 8;
            this.validatePythonPath.Text = "...";
            this.validatePythonPath.UseVisualStyleBackColor = true;
            this.validatePythonPath.Click += new System.EventHandler(this.validatePythonPath_Click);
            // 
            // netLogsPathBox
            // 
            this.netLogsPathBox.Location = new System.Drawing.Point(97, 164);
            this.netLogsPathBox.Name = "netLogsPathBox";
            this.netLogsPathBox.Size = new System.Drawing.Size(227, 20);
            this.netLogsPathBox.TabIndex = 11;
            // 
            // tdPathBox
            // 
            this.tdPathBox.Location = new System.Drawing.Point(97, 138);
            this.tdPathBox.Name = "tdPathBox";
            this.tdPathBox.Size = new System.Drawing.Size(227, 20);
            this.tdPathBox.TabIndex = 9;
            // 
            // pythonPathBox
            // 
            this.pythonPathBox.Location = new System.Drawing.Point(97, 112);
            this.pythonPathBox.Name = "pythonPathBox";
            this.pythonPathBox.Size = new System.Drawing.Size(227, 20);
            this.pythonPathBox.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 168);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Net Logs Path:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Python Path:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 141);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "TD Path:";
            // 
            // resetButton
            // 
            this.resetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resetButton.Location = new System.Drawing.Point(151, 345);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(84, 23);
            this.resetButton.TabIndex = 15;
            this.resetButton.Text = "Reset Settings";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
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
            this.miscGroupBox.Controls.Add(this.rebuyPercentage);
            this.miscGroupBox.Controls.Add(this.lblRebuyPercentage);
            this.miscGroupBox.Location = new System.Drawing.Point(12, 281);
            this.miscGroupBox.Name = "miscGroupBox";
            this.miscGroupBox.Size = new System.Drawing.Size(363, 50);
            this.miscGroupBox.TabIndex = 17;
            this.miscGroupBox.TabStop = false;
            this.miscGroupBox.Text = "Misc.";
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
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 380);
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
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.generic_KeyDown);
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
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button validateNetLogsPath;
        private System.Windows.Forms.Button validateTDPath;
        private System.Windows.Forms.Button validatePythonPath;
        private System.Windows.Forms.TextBox netLogsPathBox;
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
    }
}