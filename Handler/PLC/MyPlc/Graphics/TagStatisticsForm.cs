using System;
using System.Windows.Forms;

namespace OptimaValue.Handler.PLC.MyPlc.Graphics
{
    public partial class TagStatisticsForm : Form
    {
        private readonly TagDefinitions taggen;
        private readonly Timer ScanTimer = new Timer();
        public TagStatisticsForm(TagDefinitions tag)
        {
            InitializeComponent();
            taggen = tag;
            ScanTimer.Interval = 200;
            txtOk.Text = tag.NrSuccededReadAttempts.ToString();
            txtFail.Text = tag.NrFailedReadAttempts.ToString();
            txtPercent.Text = tag.PercentOk.ToString();
            txtScan.Text = tag.ScanTime;
            txtAverageScan.Text = tag.AverageScanTime;
        }

        private void TagStatisticsForm_Load(object sender, EventArgs e)
        {
            ScanTimer.Start();
            ScanTimer.Tick += TimeOutTimer_Tick;
        }

        private void TimeOutTimer_Tick(object sender, EventArgs e)
        {
            txtOk.Text = taggen.NrSuccededReadAttempts.ToString();
            txtFail.Text = taggen.NrFailedReadAttempts.ToString();
            txtPercent.Text = taggen.PercentOk.ToString();
            txtScan.Text = taggen.ScanTime;
            txtAverageScan.Text = taggen.AverageScanTime;
            txtLogs.Text = taggen.TimesLogged.ToString();
        }

        private void TagStatisticsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ScanTimer.Tick -= TimeOutTimer_Tick;
        }
    }
}
