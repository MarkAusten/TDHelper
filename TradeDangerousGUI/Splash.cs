using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TDHelper
{
    public partial class Splash : Form
    {
        public const int HT_CAPTION = 0x2;
        public const int WM_NCLBUTTONDOWN = 0xA1;

        public Splash()
        {
            InitializeComponent();
        }

        public string Caption
        {
            get
            {
                return this.label1.Text;
            }

            set
            {
                this.label1.Text = value;
            }
        }

        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private void MainForm_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}