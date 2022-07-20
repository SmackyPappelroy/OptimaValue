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
using System.Windows.Markup;
using System.Globalization;

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
            FrameworkElement.LanguageProperty.OverrideMetadata(
               typeof(FrameworkElement),
               new FrameworkPropertyMetadata(
                   XmlLanguage.GetLanguage(
                   CultureInfo.CurrentCulture.IetfLanguageTag)));

            bool createdNew;

            using Mutex mutex = new Mutex(true, "OptimaValueWpf", out createdNew);
            if (!createdNew)
                Current.Shutdown();

            try
            {
                //throw new NotImplementedException();
                await Master.Instance.StartAsync();

                var trendWindow = Master.GetService<TrendWindow>();
                trendWindow.Show();

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


