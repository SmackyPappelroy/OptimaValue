using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OptimaValue.Config;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue.Wpf
{
    /// <summary>
    /// Dependency injection class
    /// </summary>
    public class Master : IDisposable
    {
        private readonly string fileLoggerPath = @$"C:\OptimaValue\OptimaValueGraf_{DateTime.Now.ToString("G")}.txt";
        public IHost host;
        private bool IsConfigured = false;

        public static Master Instance = new();

        public Master()
        {
            //throw new NotImplementedException();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (IsConfigured)
                return;

            services.AddSingleton<GraphWindow>();

            Log.Logger = new LoggerConfiguration()
                        .WriteTo.File(fileLoggerPath,
                            restrictedToMinimumLevel: LogEventLevel.Information,
                            rollingInterval: RollingInterval.Month,
                            fileSizeLimitBytes: 100000000)
                        .CreateLogger();
            IsConfigured = true;
        }

        public static T GetService<T>() => Instance.host.Services.GetRequiredService<T>();

        public void Dispose()
        {
            host.Dispose();
        }
    }

    public static class MasterExtensions
    {
        public static Master Setup(this Master master)
        {
            // Find project file path
            var filePath = Process.GetCurrentProcess().MainModule.FileName;
            Settings.Load();
            Settings.OptimaValueWpfFilePath = filePath;
            Settings.IsTrendRunning = true;
            master.host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    master.ConfigureServices(services);
                })
           .Build();
            return master;
        }

        public static async Task StartAsync(this Master master)
        {
            await master.host.StartAsync();
        }

        public static async Task StopAsync(this Master master)
        {
            try
            {
                Settings.IsTrendRunning = false;
                Log.CloseAndFlush();
                await master.host.StopAsync();
            }
            catch (OperationCanceledException) { }

        }


    }
}


