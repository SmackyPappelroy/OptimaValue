using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using OptimaValue.Config;
using System.Threading;
using Serilog;
using Serilog.Events;

namespace OptimaValue.Wpf
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost host;

        public App()
        {
            SqlSettings.Load();
            host = Host.CreateDefaultBuilder()
           .ConfigureServices((context, services) =>
           {
               ConfigureServices(services);
           })
           .Build();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<GraphWindow>();

            Log.Logger = new LoggerConfiguration()
                        .WriteTo.File(@"C:\OptimaValue\OptimaValueGraf_.txt",
                            restrictedToMinimumLevel: LogEventLevel.Information,
                            rollingInterval: RollingInterval.Month,
                            fileSizeLimitBytes: 100000000)
                        .CreateLogger();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                //throw new NotImplementedException();
                SqlMethods.CreateConnectionString();
                await host.StartAsync();

                var graphWindow = host.Services.GetRequiredService<GraphWindow>();
                graphWindow.Show();

                base.OnStartup(e);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Applikationen kraschade");
                Log.CloseAndFlush();
                Application.Current.Shutdown();
            }

        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (host)
            {
                await host.StopAsync();
            }
            Log.CloseAndFlush();

            base.OnExit(e);
        }


    }
}


