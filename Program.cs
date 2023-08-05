using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using FileLogger;
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
        // Check if OptimaValue.Service is running
        if (Process.GetProcessesByName("OptimaValue.Service").Length > 0)
        {
            MessageBox.Show("OptimaValue.Service is already running");
            return;
        }

        using Mutex mutex = new(true, "OptimaValue", out createdNew);
        if (createdNew)
        {
            Settings.Load();
            Settings.OptimaValueFilePath = Application.ExecutablePath;
#if RELEASE
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
#endif
            ApplicationConfiguration.Initialize();
            try
            {
                Logger.Initialize(@"C:\OptimaValue\", true);
                Application.Run(new MasterForm());
                Logger.Instance.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Initialize(@"C:\OptimaValue\", true);
                Logger.LogError($"Applikationen krashade", ex);
                Logger.Instance.Dispose();
                Environment.Exit(0);
            }
        }
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Logger.Initialize(@"C:\OptimaValue\", true);
        Logger.LogError($"Applikationen krashade{Environment.NewLine + e.ExceptionObject}");
        Logger.Instance.Dispose(); // Ensure all logs are written before application crashes
        Environment.Exit(0);
    }

    private static void TaskScheduler_UnobservedTaskException(object sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
    {
        Logger.Initialize(@"C:\OptimaValue\", true);
        Logger.LogError($"Applikationen krashade{Environment.NewLine + e.Exception}");
        Logger.Instance.Dispose(); // Ensure all logs are written before application crashes
        Environment.Exit(0);
    }
}

