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
                using var logger = new Logger.LoggerBuilder()
                    .WithDirectoryPath(@"C:\OptimaValue\")
                    .EnableFileLog(true)
                    .EnableSqlLogging(Settings.Server, Settings.Databas, Settings.User, Settings.Password)
                    .Build();
                Application.Run(new MasterForm());
            }
            catch (Exception ex)
            {
                using var logger = new Logger.LoggerBuilder()
                    .WithDirectoryPath(@"C:\OptimaValue\")
                    .EnableFileLog(true)
                    .EnableSqlLogging(Settings.Server, Settings.Databas, Settings.User, Settings.Password)
                    .Build();
                Logger.LogError($"Applikationen krashade", ex);
                Environment.Exit(0);
            }
        }
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        using var logger = new Logger.LoggerBuilder()
                    .WithDirectoryPath(@"C:\OptimaValue\")
                    .EnableFileLog(true)
                    .EnableSqlLogging(Settings.Server, Settings.Databas, Settings.User, Settings.Password)
                    .Build();
        Logger.LogError($"Applikationen krashade{Environment.NewLine + e.ExceptionObject}");
        Environment.Exit(0);
    }

    private static void TaskScheduler_UnobservedTaskException(object sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
    {
        using var logger = new Logger.LoggerBuilder()
                    .WithDirectoryPath(@"C:\OptimaValue\")
                    .EnableFileLog(true)
                    .EnableSqlLogging(Settings.Server, Settings.Databas, Settings.User, Settings.Password)
                    .Build();
        Logger.LogError($"Applikationen krashade{Environment.NewLine + e.Exception}");
        Environment.Exit(0);
    }
}

