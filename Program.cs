using System;
using System.Threading;
using System.Windows.Forms;

namespace OptimaValue
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createdNew = true;

            using (Mutex mutex = new Mutex(true, "OptimaValue", out createdNew))
            {
                if (createdNew)
                {
                    Apps.Logger = new FileLogger(@"C:\OptimaValue\", true);
                    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MasterForm());
                }
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Apps.Logger.EnableFileLog = true;
            Apps.Logger.Log($"Applikationen krashade{Environment.NewLine + e.ExceptionObject}", Severity.Error);
            Environment.Exit(0);
        }
    }
}
