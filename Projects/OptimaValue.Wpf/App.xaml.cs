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

        public App()
        {
            Master.Instance.Setup();
        }



        protected override async void OnStartup(StartupEventArgs e)
        {
            bool createdNew;

            using Mutex mutex = new Mutex(true, "OptimaValueWpf", out createdNew);
            if (!createdNew)
                Current.Shutdown();

            try
            {
                //throw new NotImplementedException();
                SqlMethods.CreateConnectionString();
                await Master.Instance.StartAsync();

                var graphWindow = Master.GetService<GraphWindow>();
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

            try
            {
                Log.CloseAndFlush();
                await Master.Instance.StopAsync();
            }
            finally
            {

                base.OnExit(e);
            }

        }


    }
}


