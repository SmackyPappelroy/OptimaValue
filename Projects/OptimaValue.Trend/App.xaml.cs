using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace OptimaValue.Trend
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

                var mainWindow = Master.GetService<MainWindow>();
                mainWindow.Show();

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
