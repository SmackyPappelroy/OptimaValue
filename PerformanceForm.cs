using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

namespace OptimaValue
{
    public partial class PerformanceForm : Form
    {
        private readonly Timer cycleTimer;
        private readonly PerformanceCounter myAppCpu;
        private readonly PerformanceCounter myAppRam;
        private SeriesCollection seriesCollection; // Serie-samling för diagrammet

        public PerformanceForm()
        {
            InitializeComponent();
            //var operatingSystem = new OperatingSystem()
            cycleTimer = new Timer()
            {
                Interval = 1000
            };
            if (OperatingSystem.IsWindows())
            {
                myAppCpu = new PerformanceCounter("Process", "% Processor Time", "OptimaValue", true);
                myAppRam = new PerformanceCounter("Process", "Working Set", "OptimaValue", true);
            }

            cycleTimer.Tick += CycleTimer_Tick;
            cycleTimer.Start();
            Text = "Loggningsstatistik";
            InitializeTrend();
        }

        private void InitializeTrend()
        {
            seriesCollection = new SeriesCollection
        {
            new LineSeries
            {
                Title = "Last Cycle Time",
                Values = new ChartValues<double>()
            }
        };
            // Anta att du har en CartesianChart som heter chart
            cartesianChart1.Series = seriesCollection;
        }

        private void CycleTimer_Tick(object sender, EventArgs e)
        {
            if (OperatingSystem.IsWindows())
            {
                double pct = myAppCpu.NextValue();
                double ram = myAppRam.NextValue();
                txtCpu.Text = (pct / 1000).ToString("P");
                txtRam.Text = $"{(ram / 1024 / 1024).ToString("F", CultureInfo.CurrentCulture)} MB";
                AddLoggingStats();
            }
            txtThread.Text = Process.GetCurrentProcess().Threads.Count.ToString();

            // Lägg till det senaste värdet i diagrammet
            double lastCycleTimeValue = LoggerHandler.LoggingStats.LastCycleTime.TotalMilliseconds; // Ersätt detta med faktiska värdet
            seriesCollection[0].Values.Add(lastCycleTimeValue);

            // Ta bort gamla värden så att diagrammet visar data för endast den senaste minuten
            if (seriesCollection[0].Values.Count > 60) // Anta att Tick-händelsen inträffar varje sekund
            {
                seriesCollection[0].Values.RemoveAt(0);
            }
        }

        private void AddLoggingStats()
        {
            try
            {
                lblLastCycle.Text = LoggerHandler.LoggingStats.LastCycleTime.FormatTime();
                lblAvgCycle.Text = LoggerHandler.LoggingStats.AverageCycleTime.FormatTime();
                lblMaxCycle.Text = LoggerHandler.LoggingStats.MaxCycleTime.FormatTime();
                lblMinCycle.Text = LoggerHandler.LoggingStats.MinCycleTime.FormatTime();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void txtCpu_Leave(object sender, EventArgs e)
        {
            cycleTimer.Tick -= CycleTimer_Tick;
            cycleTimer.Stop();
        }
    }
}
