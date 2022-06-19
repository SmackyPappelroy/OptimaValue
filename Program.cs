using System;
using System.Threading;
using System.Windows.Forms;
using OptimaValue.Config;

namespace OptimaValue;

static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        bool createdNew = true;

        using Mutex mutex = new(true, "OptimaValue", out createdNew);
        if (createdNew)
        {
            Apps.Logger = new FileLogger(@"C:\OptimaValue\", true);
            Settings.Load();
            Settings.OptimaValueFilePath = Application.ExecutablePath;
#if RELEASE
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif
            ApplicationConfiguration.Initialize();
            try
            {
                Application.Run(new MasterForm());
            }
            catch (Exception ex)
            {
                Apps.Logger.EnableFileLog = true;
                Apps.Logger.Log($"Applikationen krashade", Severity.Error, ex);
                Environment.Exit(0);
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

