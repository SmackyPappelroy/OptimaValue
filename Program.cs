using System;
using System.Data.Entity.Core.Mapping;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using FileLogger;
using OptimaValue.Config;

namespace OptimaValue
{
    static class Program
    {
        private static FileLog LoggerInstance;

        [STAThread]
        static void Main()
        {
            if (Process.GetProcessesByName("OptimaValue.Service").Length > 0)
            {
                MessageBox.Show("OptimaValue.Service is already running");
                return;
            }

            using Mutex mutex = new(true, "Global\\OptimaValueUniqueMutexName", out bool createdNew);
            if (createdNew)
            {
                Settings.Load();
                LoggerInstance = CreateFileLogger();
                Settings.OptimaValueFilePath = Application.ExecutablePath;

#if RELEASE
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                System.Threading.Tasks.TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
#endif
                ApplicationConfiguration.Initialize();
                try
                {
                    Application.Run(new MasterForm());
                    Logger.LogInfo($"Applikationen avslutades av {Environment.UserName}");
                    Logger.Dispose();
                }
                catch (Exception ex)
                {
                    LogErrorAndExit($"Applikationen krashade", ex);
                }

            }
        }

        private static void LogErrorAndExit(string message, Exception ex = null)
        {
            if (LoggerInstance != null)
            {
                if (ex != null)
                {
                    Logger.LogError(message, ex);
                }
                else
                {
                    Logger.LogError(message);
                }
            }
            Logger.Dispose();
            Environment.Exit(0);
        }

        public static FileLog CreateFileLogger()
        {
            return new Logger.LoggerBuilder()
                .WithDirectoryPath(@"C:\OptimaValue\")
                .WithLogDelay(5)
                .EnableFileLog(true)
                .EnableSqlLogging(Settings.Server, Settings.Databas, Settings.User, Settings.Password)
                .Build();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogErrorAndExit($"Applikationen krashade", new Exception(e.ExceptionObject.ToString()));
            Logger.Dispose();
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
        {
            LogErrorAndExit($"Applikationen krashade", e.Exception);
            Logger.Dispose();
        }
    }
}
