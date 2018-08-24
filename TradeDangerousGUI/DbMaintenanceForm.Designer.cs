namespace TDHelper
{
    partial class DbMaintenanceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DbMaintenanceForm));
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.btnVacuum = new System.Windows.Forms.Button();
            this.btnAnalyse = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblVacuum = new System.Windows.Forms.Label();
            this.lblAnalyse = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnVacuum
            // 
            this.btnVacuum.Location = new System.Drawing.Point(12, 83);
            this.btnVacuum.Name = "btnVacuum";
            this.btnVacuum.Size = new System.Drawing.Size(75, 23);
            this.btnVacuum.TabIndex = 15;
            this.btnVacuum.Text = "Vacuum";
            this.btnVacuum.UseVisualStyleBackColor = true;
            this.btnVacuum.Click += new System.EventHandler(this.EventHandler_Vacuum_Click);
            // 
            // btnAnalyse
            // 
            this.btnAnalyse.Location = new System.Drawing.Point(12, 12);
            this.btnAnalyse.Name = "btnAnalyse";
            this.btnAnalyse.Size = new System.Drawing.Size(75, 23);
            this.btnAnalyse.TabIndex = 16;
            this.btnAnalyse.Text = "Analyse";
            this.btnAnalyse.UseVisualStyleBackColor = true;
            this.btnAnalyse.Click += new System.EventHandler(this.EventHandler_Analyse_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(467, 282);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 17;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.EventHandler_Close_Click);
            // 
            // lblVacuum
            // 
            this.lblVacuum.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVacuum.Location = new System.Drawing.Point(93, 83);
            this.lblVacuum.Name = "lblVacuum";
            this.lblVacuum.Size = new System.Drawing.Size(449, 170);
            this.lblVacuum.TabIndex = 20;
            this.lblVacuum.Text = resources.GetString("lblVacuum.Text");
            // 
            // lblAnalyse
            // 
            this.lblAnalyse.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAnalyse.Location = new System.Drawing.Point(98, 12);
            this.lblAnalyse.Name = "lblAnalyse";
            this.lblAnalyse.Size = new System.Drawing.Size(444, 48);
            this.lblAnalyse.TabIndex = 21;
            this.lblAnalyse.Text = resources.GetString("lblAnalyse.Text");
            // 
            // DbMaintenanceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 317);
            this.Controls.Add(this.lblAnalyse);
            this.Controls.Add(this.lblVacuum);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnAnalyse);
            this.Controls.Add(this.btnVacuum);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DbMaintenanceForm";
            this.Text = "Database Maintenance";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTips;
        private System.Windows.Forms.Button btnVacuum;
        private System.Windows.Forms.Button btnAnalyse;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblVacuum;
        private System.Windows.Forms.Label lblAnalyse;
    }
}