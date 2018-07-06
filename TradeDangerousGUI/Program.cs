using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Globalization;

namespace TDHelper
{
    static class Program
    {
        public static bool updateOverride = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                using (Mutex mutex = new Mutex(false, "Global\\" + MainForm.AssemblyGuid))
                {
                    if (!mutex.WaitOne(0, false))
                        return;

                    if (args.Length == 1 && args[0] == "/noupdate")
                    {
                        updateOverride = true; // flag us as override
                    }
                    else if (args.Length == 1 && args[0] == "/g")
                    {
                        TopMostMessageBox.Show(
                            true, 
                            true, 
                            "You must include a URL pointing to a Zip file surrounded by quotes as your second argument!\r\nExample:  TDHelper.exe /g \"http://localhost:90/File.zip\"",
                            "TD Helper - Argument Error", 
                            MessageBoxButtons.OK);

                        return;
                    }
                    else if (args.Length == 2 && args[0] == "/g")
                    {
                        if (UpdateClass.IsValidURLArchive(args[1]))
                        {
                            DialogResult d = TopMostMessageBox.Show(
                                true, 
                                true, 
                                "We will now generate a manifest file in the current directory.", 
                                "TD Helper - Confirm", 
                                MessageBoxButtons.OKCancel);

                            if (d == DialogResult.OK)
                            {
                                UpdateClass.GenerateManifest(MainForm.localDir, MainForm.localDir + "\\TDHelper.manifest", args[1]);
                                return;
                            }
                        }
                    }
                    else if (args.Length == 1 && args[0] == "/?")
                    {
                        TopMostMessageBox.Show(
                            true, 
                            true, 
                            "Proper commandline arguments are:\r\n\r\n\t/noupdate   Disables auto-update.\r\n\t/g [URL]       Specifies a URL to assign as the package in the manifest.\r\n\t/?\t   This help message box.",
                            "TD Helper - Argument Help", 
                            MessageBoxButtons.OK);

                        return;
                    }

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm());
                }
            }
            catch (Exception e) { throw e; }
        }
    }
}
