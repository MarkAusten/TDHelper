using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace TDHelper
{
    public partial class Form2 : Form
    {
        #region FormProps
        public TreeView treeViewBox
        {
            get { return treeView; }
            set { treeView = value; }
        }

        public string formTitle
        {
            get { return this.Text; }
            set 
            {
                if (Form1.t_CrTonTally > 0)
                    this.Text = value;
                else
                    this.Text = "TDHelper (mini-mode)";
            }
        }

        string snipSystem = "";
        #endregion


        #region Snap-To-Edge
        private const int SnapDist = 10;
        private void Form2_LocationChanged(object sender, EventArgs e)
        {
            Rectangle workingArea = Screen.FromControl(this).WorkingArea;

            if (Math.Abs(workingArea.Left - this.Left) < SnapDist)
                this.Left = workingArea.Left;
            else if (Math.Abs(this.Left + this.Width - workingArea.Left - workingArea.Width) < SnapDist)
                this.Left = workingArea.Left + workingArea.Width - this.Width;

            if (Math.Abs(workingArea.Top - this.Top) < SnapDist)
                this.Top = workingArea.Top;
            else if (Math.Abs(this.Top + this.Height - workingArea.Top - workingArea.Height) < SnapDist)
                this.Top = workingArea.Top + workingArea.Height - this.Height;
        }
        #endregion

        #region FormStuff
        public Form2(Form1 instance)
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Screen screen = Screen.FromControl(this);
            Rectangle workingArea = screen.WorkingArea;
            int[] winLoc = Form1.loadWinLoc(Form1.settingsRef.LocationChild);
            int[] winSize = Form1.loadWinSize(Form1.settingsRef.SizeChild);

            // if we've saved a fontname and fontsize, use them, otherwise use default
            if (Form1.settingsRef.TreeViewFont != null)
                this.treeView.Font = Form1.settingsRef.convertFromMemberFont();

            // only resize on our first parsing
            if (!Form1.hasParsed)
            {// try to remember and restore the window size
                if (winSize.Length != 0 && winSize != null)
                    compensateNodeLength(winSize); // check our width and compensate
                else
                {// load our default size
                    Form1.settingsRef.SizeChild = Form1.saveWinSize(this);
                    int[] t_winSize = Form1.loadWinSize(Form1.settingsRef.SizeChild);

                    // check width
                    compensateNodeLength(t_winSize); // check our width and compensate
                }

                Form1.hasParsed = true;
            }
            else if (Form1.hasParsed && winSize.Length != 0 && winSize != null)
            {// just restore our size without compensating
                this.Size = new Size(winSize[0], winSize[1]);
            }

            // try to remember and restore the window location
            if (winLoc.Length != 0 && winLoc != null)
            {
                this.Location = new Point(winLoc[0], winLoc[1]);

                // if we're restoring the location, let's force it to be visible on screen
                Form1.forceFormOnScreen(this);
            }
            else
            {
                this.Location = new Point()
                {
                    X = Math.Max(workingArea.X, workingArea.X + (workingArea.Width - this.Width) / 2),
                    Y = Math.Max(workingArea.Y, workingArea.Y + (workingArea.Height - this.Height) / 2)
                };
            }

            // remember our on-top state before passing to pinButton
            if (Form1.settingsRef.MiniModeOnTop)
                this.TopMost = false;
            else
                this.TopMost = true;

            pinButton_Click(this, null);
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1.settingsRef.LocationChild = Form1.saveWinLoc(this);
            Form1.settingsRef.SizeChild = Form1.saveWinSize(this);
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
        }
        #endregion

        #region FormMembers
        private void pinButton_Click(object sender, EventArgs e)
        {// toggle on-top
            if (!this.TopMost)
            {// bold the button text as an indicator
                if (!Form1.settingsRef.MiniModeOnTop)
                    Form1.settingsRef.MiniModeOnTop = true;

                pinButton.BackColor = Color.FromArgb(50, 50, 50);
                pinButton.FlatAppearance.BorderColor = Color.FromArgb(50, 50, 50);
                pinButton.ForeColor = Color.DarkOrange;
                pinButton.Font = new Font(pinButton.Font, FontStyle.Bold);
                toolTip1.SetToolTip(pinButton, "Disable on-top mode for this window");
                this.TopMost = true;
            }
            else
            {// return to normal
                pinButton.BackColor = Color.FromArgb(30, 30, 30);
                pinButton.FlatAppearance.BorderColor = Color.FromArgb(30, 30, 30);
                pinButton.ForeColor = Color.DimGray;
                pinButton.Font = new Font(pinButton.Font, FontStyle.Regular);
                toolTip1.SetToolTip(pinButton, "Enable on-top mode for this window");
                Form1.settingsRef.MiniModeOnTop = false;
                this.TopMost = false;
            }
        }

        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                this.Close();
            }
            else if (e.KeyCode == Keys.C && e.Modifiers == (Keys.Control | Keys.Shift))
            {
                snipSystem = treeView.SelectedNode.Text;
                string resultLabel = Regex.Replace(snipSystem, @"(?:Unload|Load|^)\s\@\s(.+?(?=\n|\s\[\d|$)).*", "$1").ToString();
                if (!String.IsNullOrEmpty(resultLabel))
                    Clipboard.SetText(resultLabel);

                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {
                //(?:(?:Load|Unload)\s@\s|\d+x\s|^)(.+?(?=\/)|.+).*
                snipSystem = treeView.SelectedNode.Text;
                string resultLabel = Regex.Replace(snipSystem, @"(?:Unload|Load|^)\s\@\s(.+?(?=\/)).*", "$1").ToString();
                if (!String.IsNullOrEmpty(resultLabel))
                    Clipboard.SetText(resultLabel);

                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void pinButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                this.Close();
            }
        }
        #endregion

        #region Helpers
        private void compensateNodeLength(int[] winSize)
        {// check for the longest treeview text string
            int maxNodeLength = 0, sumNodesHeight = 0, formWidth = this.Size.Width;
            List<int> procWidths = new List<int>(), procHeights = new List<int>();

            foreach (var n in TraverseNodes(treeView.Nodes))
            {// make a list of all node bounds

                // width + scrollbar
                procWidths.Add(Math.Max(n.Bounds.Right + 38, this.treeView.ClientSize.Width));
                sumNodesHeight += n.Bounds.Size.Height;
            }

            maxNodeLength = procWidths.Max(); // pick the largest in the list

            winSize[0] = maxNodeLength; // width
            winSize[1] = sumNodesHeight + 65; // height + titlebar

            if (this.MinimumSize.Width < winSize[0] && this.MinimumSize.Height < winSize[1])
            {// make sure we don't set the form smaller than the minimums
                this.Size = new Size(winSize[0], winSize[1]);
                this.MaximumSize = new Size(winSize[0], Screen.FromControl(this).WorkingArea.Bottom);
            }
        }

        IEnumerable<TreeNode> TraverseNodes(TreeNodeCollection nodeTree)
        {// traverse all the nodes in a TreeView as a list
            foreach(TreeNode n in nodeTree)
            {
                yield return n;

                foreach (var result in TraverseNodes(n.Nodes))
                {
                    yield return result;
                }
            }
        }
        #endregion
    }
}
