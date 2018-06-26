using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TDHelper
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }

        private void formValidator()
        {
            Form1.authCode = confirmCodeBox.Text;
        }

        private void confirmCodeBox_TextChanged(object sender, EventArgs e)
        {
            var ctrl = (TextBox)sender;
            var curPos = ctrl.SelectionStart;
            ctrl.Text = Regex.Replace(ctrl.Text, "[^0-9a-zA-Z ]", "");
            ctrl.SelectionStart = curPos;
        }

        private void confirmCodeBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                formValidator();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                Form1.authCode = ""; // make sure we return
                this.Dispose();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            formValidator();
        }
    }
}
