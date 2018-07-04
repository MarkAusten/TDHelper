namespace TDHelper
{
    partial class TdMiniForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TdMiniForm));
            this.treeView = new System.Windows.Forms.TreeView();
            this.pinButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView.ForeColor = System.Drawing.Color.DarkOrange;
            this.treeView.HideSelection = false;
            this.treeView.LabelEdit = true;
            this.treeView.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.treeView.Location = new System.Drawing.Point(2, 2);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(240, 342);
            this.treeView.TabIndex = 0;
            this.treeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView_KeyDown);
            // 
            // pinButton
            // 
            this.pinButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pinButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pinButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pinButton.FlatAppearance.BorderSize = 0;
            this.pinButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pinButton.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pinButton.ForeColor = System.Drawing.Color.DimGray;
            this.pinButton.Location = new System.Drawing.Point(2, 323);
            this.pinButton.Name = "pinButton";
            this.pinButton.Size = new System.Drawing.Size(21, 21);
            this.pinButton.TabIndex = 5;
            this.pinButton.TabStop = false;
            this.pinButton.Text = "&T";
            this.pinButton.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.toolTip1.SetToolTip(this.pinButton, "Enable on-top mode for this window");
            this.pinButton.UseVisualStyleBackColor = false;
            this.pinButton.Click += new System.EventHandler(this.pinButton_Click);
            this.pinButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pinButton_KeyDown);
            // 
            // TdMiniForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.ClientSize = new System.Drawing.Size(244, 346);
            this.Controls.Add(this.pinButton);
            this.Controls.Add(this.treeView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(175, 90);
            this.Name = "TdMiniForm";
            this.Text = "TDHelper (mini)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TdMiniForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TdMiniForm_FormClosed);
            this.Load += new System.EventHandler(this.TdMiniForm_Load);
            this.LocationChanged += new System.EventHandler(this.TdMiniForm_LocationChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Button pinButton;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}