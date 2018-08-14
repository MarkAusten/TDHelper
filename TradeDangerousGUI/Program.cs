using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace TDHelper
{
    internal static class Program
    {
        public static bool updateOverride = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                using (Mutex mutex = new Mutex(false, "Global\\" + MainForm.AssemblyGuid))
                {
                    if (!mutex.WaitOne(0, false))
                    {
                        return;
                    }

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
                                UpdateClass.GenerateManifest(
                                    MainForm.assemblyPath, 
                                    Path.Combine(MainForm.assemblyPath, "TDHelper.manifest"), 
                                    args[1]);
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

                    string version = AssemblyName
                        .GetAssemblyName("TDHelper.exe")
                        .Version
                        .ToString();

                    SplashScreen.ShowSplashScreen();
                    SplashScreen.SetVersion(version);

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm());
                }
            }
            catch (Exception ex)
            {
                // Get the path to the error log.
                string errorLogPath = Path.Combine(
                    Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), 
                    "error.log");

                // Get the exception message plus any inner exception messages.
                string message = DateTime.Now.ToString("yyyy//MM/dd hh:mm:ss") + " : " + ex.Message;
                Exception pointer = ex.InnerException;

                while (pointer != null && string.IsNullOrEmpty(pointer.Message))
                {
                    message += Environment.NewLine + pointer.Message;
                }

                // Append the message to the error log
                File.AppendAllText(
                    errorLogPath, 
                    message + Environment.NewLine + Environment.NewLine, 
                    System.Text.Encoding.Default);

                // Show an error dialog to the user.
                MessageBox.Show(
                    "An error occured when running TDHelper. Please close this mesage box and try again. If this continues to happen please contact the administrator. Thanks.", 
                    "TD Helper - Error", 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
            }
        }
    }
}