namespace TDHelper
{
    partial class EddbLinkDbUpdateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EddbLinkDbUpdateForm));
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.chkClean = new System.Windows.Forms.CheckBox();
            this.chkFallback = new System.Windows.Forms.CheckBox();
            this.chkForce = new System.Windows.Forms.CheckBox();
            this.chkItem = new System.Windows.Forms.CheckBox();
            this.chkListings = new System.Windows.Forms.CheckBox();
            this.chkShip = new System.Windows.Forms.CheckBox();
            this.chkShipvend = new System.Windows.Forms.CheckBox();
            this.chkSkipvend = new System.Windows.Forms.CheckBox();
            this.chkSolo = new System.Windows.Forms.CheckBox();
            this.chkStation = new System.Windows.Forms.CheckBox();
            this.chkSystem = new System.Windows.Forms.CheckBox();
            this.chkUpgrade = new System.Windows.Forms.CheckBox();
            this.chkUpvend = new System.Windows.Forms.CheckBox();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.btnReset = new System.Windows.Forms.Button();
            this.btnUpdateDb = new System.Windows.Forms.Button();
            this.btnAnalyse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.Location = new System.Drawing.Point(12, 12);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(37, 17);
            this.chkAll.TabIndex = 0;
            this.chkAll.Tag = "all";
            this.chkAll.Text = "All";
            this.toolTips.SetToolTip(this.chkAll, "Update everything with latest dumpfiles. (Regenerates all tables)");
            this.chkAll.UseVisualStyleBackColor = true;
            this.chkAll.CheckedChanged += new System.EventHandler(this.EventHandler_All_CheckedChanged);
            // 
            // chkClean
            // 
            this.chkClean.AutoSize = true;
            this.chkClean.Location = new System.Drawing.Point(12, 35);
            this.chkClean.Name = "chkClean";
            this.chkClean.Size = new System.Drawing.Size(53, 17);
            this.chkClean.TabIndex = 1;
            this.chkClean.Tag = "clean";
            this.chkClean.Text = "Clean";
            this.toolTips.SetToolTip(this.chkClean, "Erase entire database and rebuild from empty. (Regenerates all tables.)");
            this.chkClean.UseVisualStyleBackColor = true;
            this.chkClean.CheckedChanged += new System.EventHandler(this.EventHandler_Clean_CheckedChanged);
            // 
            // chkFallback
            // 
            this.chkFallback.AutoSize = true;
            this.chkFallback.Location = new System.Drawing.Point(12, 58);
            this.chkFallback.Name = "chkFallback";
            this.chkFallback.Size = new System.Drawing.Size(66, 17);
            this.chkFallback.TabIndex = 2;
            this.chkFallback.Tag = "fallback";
            this.chkFallback.Text = "Fallback";
            this.toolTips.SetToolTip(this.chkFallback, "Fallback to using EDDB.io if Tromador\'s mirror isn\'t working.");
            this.chkFallback.UseVisualStyleBackColor = true;
            // 
            // chkForce
            // 
            this.chkForce.AutoSize = true;
            this.chkForce.Location = new System.Drawing.Point(12, 81);
            this.chkForce.Name = "chkForce";
            this.chkForce.Size = new System.Drawing.Size(53, 17);
            this.chkForce.TabIndex = 3;
            this.chkForce.Tag = "force";
            this.chkForce.Text = "Force";
            this.toolTips.SetToolTip(this.chkForce, "Force regeneration of selected items even if source file not updated since previo" +
        "us run. (Useful for updating Vendor tables if they were skipped during a \'-O cle" +
        "an\' run.)");
            this.chkForce.UseVisualStyleBackColor = true;
            // 
            // chkItem
            // 
            this.chkItem.AutoSize = true;
            this.chkItem.Location = new System.Drawing.Point(12, 104);
            this.chkItem.Name = "chkItem";
            this.chkItem.Size = new System.Drawing.Size(46, 17);
            this.chkItem.TabIndex = 4;
            this.chkItem.Tag = "item";
            this.chkItem.Text = "Item";
            this.toolTips.SetToolTip(this.chkItem, "Regenerate Categories and Items using latest commodities.json dump.");
            this.chkItem.UseVisualStyleBackColor = true;
            // 
            // chkListings
            // 
            this.chkListings.AutoSize = true;
            this.chkListings.Location = new System.Drawing.Point(12, 127);
            this.chkListings.Name = "chkListings";
            this.chkListings.Size = new System.Drawing.Size(61, 17);
            this.chkListings.TabIndex = 5;
            this.chkListings.Tag = "listings";
            this.chkListings.Text = "Listings";
            this.toolTips.SetToolTip(this.chkListings, "Update market data using latest listings.csv dump. (implies \'-O item,system,stati" +
        "on\')");
            this.chkListings.UseVisualStyleBackColor = true;
            this.chkListings.CheckedChanged += new System.EventHandler(this.EventHandler_Listings_CheckedChanged);
            // 
            // chkShip
            // 
            this.chkShip.AutoSize = true;
            this.chkShip.Location = new System.Drawing.Point(12, 150);
            this.chkShip.Name = "chkShip";
            this.chkShip.Size = new System.Drawing.Size(47, 17);
            this.chkShip.TabIndex = 6;
            this.chkShip.Tag = "ship";
            this.chkShip.Text = "Ship";
            this.toolTips.SetToolTip(this.chkShip, "Regenerate Ships using latest coriolis.io json dump.");
            this.chkShip.UseVisualStyleBackColor = true;
            // 
            // chkShipvend
            // 
            this.chkShipvend.AutoSize = true;
            this.chkShipvend.Location = new System.Drawing.Point(111, 12);
            this.chkShipvend.Name = "chkShipvend";
            this.chkShipvend.Size = new System.Drawing.Size(71, 17);
            this.chkShipvend.TabIndex = 7;
            this.chkShipvend.Tag = "shipvend";
            this.chkShipvend.Text = "Shipvend";
            this.toolTips.SetToolTip(this.chkShipvend, "Regenerate ShipVendors using latest stations.jsonl dump. (implies \'-O system,stat" +
        "ion,ship\')");
            this.chkShipvend.UseVisualStyleBackColor = true;
            this.chkShipvend.CheckedChanged += new System.EventHandler(this.EventHandler_Shipvend_CheckedChanged);
            // 
            // chkSkipvend
            // 
            this.chkSkipvend.AutoSize = true;
            this.chkSkipvend.Location = new System.Drawing.Point(111, 35);
            this.chkSkipvend.Name = "chkSkipvend";
            this.chkSkipvend.Size = new System.Drawing.Size(71, 17);
            this.chkSkipvend.TabIndex = 8;
            this.chkSkipvend.Tag = "skipvend";
            this.chkSkipvend.Text = "Skipvend";
            this.toolTips.SetToolTip(this.chkSkipvend, "Don\'t regenerate ShipVendors or UpgradeVendors. Supercedes \'-O all\', \'-O clean\'.");
            this.chkSkipvend.UseVisualStyleBackColor = true;
            this.chkSkipvend.CheckedChanged += new System.EventHandler(this.EventHandler_Skipvend_CheckedChanged);
            // 
            // chkSolo
            // 
            this.chkSolo.AutoSize = true;
            this.chkSolo.Location = new System.Drawing.Point(111, 58);
            this.chkSolo.Name = "chkSolo";
            this.chkSolo.Size = new System.Drawing.Size(47, 17);
            this.chkSolo.TabIndex = 9;
            this.chkSolo.Tag = "solo";
            this.chkSolo.Text = "Solo";
            this.toolTips.SetToolTip(this.chkSolo, "Don\'t download crowd-sourced market data. Supercedes \'-O all\', \'-O clean\', \'-O li" +
        "stings\'.");
            this.chkSolo.UseVisualStyleBackColor = true;
            this.chkSolo.CheckedChanged += new System.EventHandler(this.EventHandler_Solo_CheckedChanged);
            // 
            // chkStation
            // 
            this.chkStation.AutoSize = true;
            this.chkStation.Location = new System.Drawing.Point(111, 81);
            this.chkStation.Name = "chkStation";
            this.chkStation.Size = new System.Drawing.Size(59, 17);
            this.chkStation.TabIndex = 10;
            this.chkStation.Tag = "station";
            this.chkStation.Text = "Station";
            this.toolTips.SetToolTip(this.chkStation, "Regenerate Stations using latest stations.jsonl dump. (implies \'-O system\')");
            this.chkStation.UseVisualStyleBackColor = true;
            this.chkStation.CheckedChanged += new System.EventHandler(this.EventHandler_Station_CheckedChanged);
            // 
            // chkSystem
            // 
            this.chkSystem.AutoSize = true;
            this.chkSystem.Location = new System.Drawing.Point(111, 104);
            this.chkSystem.Name = "chkSystem";
            this.chkSystem.Size = new System.Drawing.Size(60, 17);
            this.chkSystem.TabIndex = 11;
            this.chkSystem.Tag = "system";
            this.chkSystem.Text = "System";
            this.toolTips.SetToolTip(this.chkSystem, "Regenerate Stations using latest stations.jsonl dump. (implies \'-O system\')");
            this.chkSystem.UseVisualStyleBackColor = true;
            // 
            // chkUpgrade
            // 
            this.chkUpgrade.AutoSize = true;
            this.chkUpgrade.Location = new System.Drawing.Point(111, 127);
            this.chkUpgrade.Name = "chkUpgrade";
            this.chkUpgrade.Size = new System.Drawing.Size(67, 17);
            this.chkUpgrade.TabIndex = 12;
            this.chkUpgrade.Tag = "upgrade";
            this.chkUpgrade.Text = "Upgrade";
            this.toolTips.SetToolTip(this.chkUpgrade, "Regenerate Upgrades using latest modules.json dump.");
            this.chkUpgrade.UseVisualStyleBackColor = true;
            // 
            // chkUpvend
            // 
            this.chkUpvend.AutoSize = true;
            this.chkUpvend.Location = new System.Drawing.Point(111, 150);
            this.chkUpvend.Name = "chkUpvend";
            this.chkUpvend.Size = new System.Drawing.Size(64, 17);
            this.chkUpvend.TabIndex = 13;
            this.chkUpvend.Tag = "upvend";
            this.chkUpvend.Text = "Upvend";
            this.toolTips.SetToolTip(this.chkUpvend, "Regenerate UpgradeVendors using latest stations.jsonl dump. (implies \'-O system,s" +
        "tation,upgrade\')");
            this.chkUpvend.UseVisualStyleBackColor = true;
            this.chkUpvend.CheckedChanged += new System.EventHandler(this.EventHandler_Upvend_CheckedChanged);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(259, 12);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 14;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.EventHandler_Reset_Click);
            // 
            // btnUpdateDb
            // 
            this.btnUpdateDb.Location = new System.Drawing.Point(259, 151);
            this.btnUpdateDb.Name = "btnUpdateDb";
            this.btnUpdateDb.Size = new System.Drawing.Size(75, 23);
            this.btnUpdateDb.TabIndex = 15;
            this.btnUpdateDb.Text = "Update DB";
            this.btnUpdateDb.UseVisualStyleBackColor = true;
            this.btnUpdateDb.Click += new System.EventHandler(this.EventHandler_UpdateDb_Click);
            // 
            // btnAnalyse
            // 
            this.btnAnalyse.Location = new System.Drawing.Point(259, 81);
            this.btnAnalyse.Name = "btnAnalyse";
            this.btnAnalyse.Size = new System.Drawing.Size(75, 23);
            this.btnAnalyse.TabIndex = 16;
            this.btnAnalyse.Text = "Analyse";
            this.btnAnalyse.UseVisualStyleBackColor = true;
            this.btnAnalyse.Click += new System.EventHandler(this.EventHandler_Analyse_Click);
            // 
            // EddbLinkDbUpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 187);
            this.Controls.Add(this.btnAnalyse);
            this.Controls.Add(this.btnUpdateDb);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.chkUpvend);
            this.Controls.Add(this.chkUpgrade);
            this.Controls.Add(this.chkSystem);
            this.Controls.Add(this.chkStation);
            this.Controls.Add(this.chkSolo);
            this.Controls.Add(this.chkSkipvend);
            this.Controls.Add(this.chkShipvend);
            this.Controls.Add(this.chkShip);
            this.Controls.Add(this.chkListings);
            this.Controls.Add(this.chkItem);
            this.Controls.Add(this.chkForce);
            this.Controls.Add(this.chkFallback);
            this.Controls.Add(this.chkClean);
            this.Controls.Add(this.chkAll);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EddbLinkDbUpdateForm";
            this.Text = "EDDBlink DB Update";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EventHandler_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkAll;
        private System.Windows.Forms.CheckBox chkClean;
        private System.Windows.Forms.CheckBox chkFallback;
        private System.Windows.Forms.CheckBox chkForce;
        private System.Windows.Forms.CheckBox chkItem;
        private System.Windows.Forms.CheckBox chkListings;
        private System.Windows.Forms.CheckBox chkShip;
        private System.Windows.Forms.CheckBox chkShipvend;
        private System.Windows.Forms.CheckBox chkSkipvend;
        private System.Windows.Forms.CheckBox chkSolo;
        private System.Windows.Forms.CheckBox chkStation;
        private System.Windows.Forms.CheckBox chkSystem;
        private System.Windows.Forms.CheckBox chkUpgrade;
        private System.Windows.Forms.CheckBox chkUpvend;
        private System.Windows.Forms.ToolTip toolTips;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnUpdateDb;
        private System.Windows.Forms.Button btnAnalyse;
    }
}