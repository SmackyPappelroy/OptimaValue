using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using FileLogger;
using Microsoft.Extensions.Hosting;
using OptimaValue.Config;

namespace OptimaValue
{
    public static class Program
    {
        internal static FileLog LoggerInstance;
        private static bool AppCrashed = false;

        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            if (Process.GetProcessesByName("OptimaValue.Service").Length > 0)
            {
                MessageBox.Show("OptimaValue.Service is already running");
                return;
            }

            using Mutex mutex = new(true, "Global\\OptimaValueUniqueMutexName", out bool createdNew);
            if (createdNew)
            {
#if RELEASE
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                System.Threading.Tasks.TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
#endif
                Settings.Load();
                if(DatabaseSql.TableExist())
                    LoggerInstance = CreateFileLogger();
                Settings.OptimaValueFilePath = Application.ExecutablePath;

                ApplicationConfiguration.Initialize();
                try
                {
                    Application.Run(new MasterForm());     // Startar WinForms-fönstret
                }
                catch (Exception ex)
                {
                    LogError($"Applikationen krashade", ex);
                }
            }
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            if (LoggerInstance != null)
            {
                if (!AppCrashed)
                {
                    Logger.LogInfo($"Applikationen avslutades av {Environment.UserName}");
                }
                LoggerInstance.Dispose();
                LoggerInstance = null;
            }
        }

        private static void LogError(string message, Exception ex = null)
        {
            AppCrashed = true;

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
        }

        public static FileLog CreateFileLogger()
        {
            return new Logger.LoggerBuilder()
                .WithDirectoryPath(@"C:\OptimaValue\")
                .WithLogDelay(5)
                .EnableFileLogging(true)
                .EnableSqlLogging(Settings.Server, Settings.Databas, Settings.User, Settings.Password)
                .Build();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogError($"Applikationen krashade", new Exception(e.ExceptionObject.ToString()));
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
        {
            LogError($"Applikationen krashade", e.Exception);
        }


    }
}
