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

        public PerformanceForm()
        {
            InitializeComponent();
            cycleTimer = new Timer()
            {
                Interval = 1000
            };
            myAppCpu = new PerformanceCounter("Process", "% Processor Time", "OptimaValue", true);
            myAppRam = new PerformanceCounter("Process", "Working Set", "OptimaValue", true);
            cycleTimer.Tick += CycleTimer_Tick;
            cycleTimer.Start();
            Text = "CPU-belastning";
        }

        private void CycleTimer_Tick(object sender, EventArgs e)
        {
            double pct = myAppCpu.NextValue();
            double ram = myAppRam.NextValue();
            txtCpu.Text = (pct / 1000).ToString("P");
            txtRam.Text = $"{(ram / 1024 / 1024).ToString("F", CultureInfo.CurrentCulture)} MB";
            txtThread.Text = Process.GetCurrentProcess().Threads.Count.ToString();
        }

        private void txtCpu_Leave(object sender, EventArgs e)
        {
            cycleTimer.Tick -= CycleTimer_Tick;
            cycleTimer.Stop();
        }
    }
}
