using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TDHelper
{
    public partial class TdMiniForm : Form
    {
        private const int SnapDist = 10;

        private string snipSystem = "";

        public TdMiniForm(MainForm instance)
        {
            InitializeComponent();
        }

        public string FormTitle
        {
            get { return this.Text; }
            set
            {
                this.Text 
                    = MainForm.t_CrTonTally > 0
                    ? value
                    : "TDHelper (mini-mode)";
            }
        }

        public TreeView TreeViewBox
        {
            get { return treeView; }
            set { treeView = value; }
        }

        private void CompensateNodeLength(int[] winSize)
        {
            // check for the longest treeview text string
            int maxNodeLength = 0, sumNodesHeight = 0, formWidth = this.Size.Width;
            List<int> procWidths = new List<int>(), procHeights = new List<int>();

            foreach (var n in TraverseNodes(treeView.Nodes))
            {
                // make a list of all node bounds

                // width + scrollbar
                procWidths.Add(Math.Max(n.Bounds.Right + 38, this.treeView.ClientSize.Width));
                sumNodesHeight += n.Bounds.Size.Height;
            }

            maxNodeLength = procWidths.Max(); // pick the largest in the list

            winSize[0] = maxNodeLength; // width
            winSize[1] = sumNodesHeight + 65; // height + titlebar

            if (this.MinimumSize.Width < winSize[0] && this.MinimumSize.Height < winSize[1])
            {
                // make sure we don't set the form smaller than the minimums
                this.Size = new Size(winSize[0], winSize[1]);
                this.MaximumSize = new Size(winSize[0], Screen.FromControl(this).WorkingArea.Bottom);
            }
        }

        private void PinButton_Click(object sender, EventArgs e)
        {
            // toggle on-top
            if (!this.TopMost)
            {
                // bold the button text as an indicator
                if (!MainForm.settingsRef.MiniModeOnTop)
                    MainForm.settingsRef.MiniModeOnTop = true;

                pinButton.BackColor = Color.FromArgb(50, 50, 50);
                pinButton.FlatAppearance.BorderColor = Color.FromArgb(50, 50, 50);
                pinButton.ForeColor = Color.DarkOrange;
                pinButton.Font = new Font(pinButton.Font, FontStyle.Bold);
                toolTip1.SetToolTip(pinButton, "Disable on-top mode for this window");
                this.TopMost = true;
            }
            else
            {
                // return to normal
                pinButton.BackColor = Color.FromArgb(30, 30, 30);
                pinButton.FlatAppearance.BorderColor = Color.FromArgb(30, 30, 30);
                pinButton.ForeColor = Color.DimGray;
                pinButton.Font = new Font(pinButton.Font, FontStyle.Regular);
                toolTip1.SetToolTip(pinButton, "Enable on-top mode for this window");
                MainForm.settingsRef.MiniModeOnTop = false;
                this.TopMost = false;
            }
        }

        private void PinButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                this.Close();
            }
        }

        private void TdMiniForm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void TdMiniForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm.settingsRef.LocationChild = MainForm.SaveWinLoc(this);
            MainForm.settingsRef.SizeChild = MainForm.SaveWinSize(this);
        }

        private void TdMiniForm_Load(object sender, EventArgs e)
        {
            Screen screen = Screen.FromControl(this);
            Rectangle workingArea = screen.WorkingArea;
            int[] winLoc = MainForm.LoadWinLoc(MainForm.settingsRef.LocationChild);
            int[] winSize = MainForm.LoadWinSize(MainForm.settingsRef.SizeChild);

            // if we've saved a fontname and fontsize, use them, otherwise use default
            if (MainForm.settingsRef.TreeViewFont != null)
            {
                this.treeView.Font = MainForm.settingsRef.ConvertFromMemberFont();
            }

            // only resize on our first parsing
            if (!MainForm.hasParsed)
            {
                // try to remember and restore the window size
                if (winSize.Length != 0 && winSize != null)
                {
                    CompensateNodeLength(winSize); // check our width and compensate
                }
                else
                {
                    // load our default size
                    MainForm.settingsRef.SizeChild = MainForm.SaveWinSize(this);
                    int[] t_winSize = MainForm.LoadWinSize(MainForm.settingsRef.SizeChild);

                    // check width
                    CompensateNodeLength(t_winSize); // check our width and compensate
                }

                MainForm.hasParsed = true;
            }
            else if (MainForm.hasParsed && winSize.Length != 0 && winSize != null)
            {
                // just restore our size without compensating
                this.Size = new Size(winSize[0], winSize[1]);
            }

            // try to remember and restore the window location
            if (winLoc.Length != 0 && winLoc != null)
            {
                this.Location = new Point(winLoc[0], winLoc[1]);

                // if we're restoring the location, let's force it to be visible on screen
                MainForm.ForceFormOnScreen(this);
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
            this.TopMost = !(MainForm.settingsRef.MiniModeOnTop);

            PinButton_Click(this, null);
        }

        private void TdMiniForm_LocationChanged(object sender, EventArgs e)
        {
            Rectangle workingArea = Screen.FromControl(this).WorkingArea;

            if (Math.Abs(workingArea.Left - this.Left) < SnapDist)
            {
                this.Left = workingArea.Left;
            }
            else if (Math.Abs(this.Left + this.Width - workingArea.Left - workingArea.Width) < SnapDist)
            {
                this.Left = workingArea.Left + workingArea.Width - this.Width;
            }

            if (Math.Abs(workingArea.Top - this.Top) < SnapDist)
            {
                this.Top = workingArea.Top;
            }
            else if (Math.Abs(this.Top + this.Height - workingArea.Top - workingArea.Height) < SnapDist)
            {
                this.Top = workingArea.Top + workingArea.Height - this.Height;
            }
        }

        private IEnumerable<TreeNode> TraverseNodes(TreeNodeCollection nodeTree)
        {
            // traverse all the nodes in a TreeView as a list
            foreach (TreeNode n in nodeTree)
            {
                yield return n;

                foreach (var result in TraverseNodes(n.Nodes))
                {
                    yield return result;
                }
            }
        }

        private void TreeView_KeyDown(object sender, KeyEventArgs e)
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

                if (!string.IsNullOrEmpty(resultLabel))
                {
                    Clipboard.SetText(resultLabel);
                }

                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {
                //(?:(?:Load|Unload)\s@\s|\d+x\s|^)(.+?(?=\/)|.+).*
                snipSystem = treeView.SelectedNode.Text;
                string resultLabel = Regex.Replace(snipSystem, @"(?:Unload|Load|^)\s\@\s(.+?(?=\/)).*", "$1").ToString();

                if (!string.IsNullOrEmpty(resultLabel))
                {
                    Clipboard.SetText(resultLabel);
                }

                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }
    }
}