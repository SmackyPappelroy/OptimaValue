using DocumentFormat.OpenXml.Wordprocessing;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OptimaValue
{
    public partial class PerformanceForm : Form
    {
        private readonly Timer cycleTimer;
        private readonly PerformanceCounter myAppCpu;
        private readonly PerformanceCounter myAppRam;
        PerformanceCounter sqlReads;
        PerformanceCounter sqlWrites;



        string cpuText;

        private SeriesCollection seriesCollectionTime; // Serie-samling för diagrammet
        private SeriesCollection seriesCollectionCpu; // Serie-samling för diagrammet

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
                myAppCpu = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName, true);

                myAppRam = new PerformanceCounter("Process", "Working Set", Process.GetCurrentProcess().ProcessName, true);

                sqlReads = new PerformanceCounter("SQLServer:Buffer Manager",
                    "Page reads/sec");
                sqlWrites = new PerformanceCounter("SQLServer:Buffer Manager",
                    "Page writes/sec");
            }
            else
            {
                MessageBox.Show("Den här applikationen stöder bara Windows för performance monitoring.");
                cycleTimer.Stop();
                return;
            }


            cycleTimer.Tick += CycleTimer_Tick;
            cycleTimer.Start();
            Text = "Loggningsstatistik";
            InitializeTrend();
            FormClosing += PerformanceForm_FormClosing;
        }

        private void PerformanceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cycleTimer.Stop();
            myAppCpu.Dispose();
            myAppRam.Dispose();
            myAppCpu.Dispose();
            myAppRam.Dispose();
            sqlReads.Dispose();
            sqlWrites.Dispose();
        }

        private void InitializeTrend()
        {
            seriesCollectionTime = new SeriesCollection
        {
            new LineSeries
            {
                Title = "Last Cycle Time",
                Values = new ChartValues<double>(),
                PointGeometrySize = 6,
            }
        };
            // Anta att du har en CartesianChart som heter chart
            cartesianChart1.Series = seriesCollectionTime;

            seriesCollectionCpu = new SeriesCollection
                {
                new LineSeries
                    {
                        Title = "CPU Usage",
                        Values = new ChartValues<double>(),
                        PointGeometrySize = 6,
                        
                    }
                };
            // Anta att du har en CartesianChart som heter chart
            cpuCartesianChart.Series = seriesCollectionCpu;

        }

        private void CycleTimer_Tick(object sender, EventArgs e)
        {
            double cpuUsage = 0.0;
            if (OperatingSystem.IsWindows())
            {
                double ram = myAppRam.NextValue();
                try
                {
                    // CPU-beräkning direkt, utan Task.Delay
                    double pct = myAppCpu.NextValue();
                    int processorCount = Environment.ProcessorCount;
                    cpuUsage = pct / processorCount;  // Justera för flera kärnor
                    // Skriv ut CPU-användning med två decimaler
                    txtCpu.Text = cpuUsage.ToString("F", CultureInfo.CurrentCulture) + " %";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fel vid mätning av CPU-användning: {ex.Message}");
                }

                txtRam.Text = $"{(ram / 1024 / 1024).ToString("F", CultureInfo.CurrentCulture)} MB";

                txtSqlRead.Text = sqlReads.NextValue().ToString();
                txtSqlWrite.Text = sqlWrites.NextValue().ToString();
                AddLoggingStats();
            }

            txtThread.Text = Process.GetCurrentProcess().Threads.Count.ToString();

            // Lägg till det senaste värdet i diagrammet
            double lastCycleTimeValue = LoggerHandler.LoggingStats.LastCycleTime.TotalMilliseconds; // Ersätt detta med faktiska värdet
            seriesCollectionTime[0].Values.Add(lastCycleTimeValue);

            // Ta bort gamla värden så att diagrammet visar data för endast den senaste minuten
            if (seriesCollectionTime[0].Values.Count > 120) // Anta att Tick-händelsen inträffar varje sekund
            {
                seriesCollectionTime[0].Values.RemoveAt(0);
            }

            // Lägg till det senaste värdet i diagrammet
            seriesCollectionCpu[0].Values.Add(cpuUsage);

            // Ta bort gamla värden så att diagrammet visar data för endast den senaste minuten
            if (seriesCollectionCpu[0].Values.Count > 120) // Anta att Tick-händelsen inträffar varje sekund
            {
                seriesCollectionCpu[0].Values.RemoveAt(0);
            }

        }


        private void AddLoggingStats()
        {
            try
            {
                lblLastCycle.Text = LoggerHandler.LoggingStats.LastCycleTime.FormatTime();
                lblAvgCycle.Text = LoggerHandler.LoggingStats.AverageCycleTime.FormatTime();
                lblMaxCycle.Text = LoggerHandler.LoggingStats.MaxCycleTime.FormatTime();
                lblMinCycle.Text = LoggerHandler.LoggingStats.MinCycleTime == TimeSpan.MaxValue ? "0,00 ms"
                    : LoggerHandler.LoggingStats.MinCycleTime.FormatTime();
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
