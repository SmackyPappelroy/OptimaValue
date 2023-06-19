using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using OptimaValue.Config;
using MQTTnet.AspNetCore;

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
            // Create and start the embedded Web API and SignalR host
            var host = CreateHostBuilder().Build();
            host.Start();

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

    public static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel(
                       o =>
                       {
                           // This will allow MQTT connections based on TCP port 1883.
                           o.ListenAnyIP(1883, l => l.UseMqtt());

                           // This will allow MQTT connections based on HTTP WebSockets with URI "localhost:5000/mqtt"
                           // See code below for URI configuration.
                           o.ListenAnyIP(5070); // Default HTTP pipeline
                       });
                webBuilder.UseStartup<Startup>();
                // Set the URL where the embedded Web API should listen
                webBuilder.UseUrls("http://localhost:5000"); // Replace with your desired URL
            });
}

