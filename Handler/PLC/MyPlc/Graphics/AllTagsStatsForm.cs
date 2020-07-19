using System;
using System.Windows.Forms;

namespace OptimaValue.Handler.PLC.MyPlc.Graphics
{
    public partial class AllTagsStatsForm : Form
    {
        private Timer ScanTimer = new Timer();
        private ExtendedPlc MyPlc;
        public AllTagsStatsForm(ExtendedPlc plc)
        {
            InitializeComponent();
            MyPlc = plc;
            ScanTimer.Interval = 200;
            ScanTimer.Tick += ScanTimer_Tick;
            ScanTimer.Start();
            this.Text = MyPlc.PlcName;
            this.HandleDestroyed += AllTagsStatsForm_HandleDestroyed;
        }

        private void AllTagsStatsForm_HandleDestroyed(object sender, EventArgs e)
        {
            ScanTimer.Stop();
            ScanTimer.Tick -= ScanTimer_Tick;
        }

        private void ScanTimer_Tick(object sender, EventArgs e)
        {
            var plcActive = false;
            foreach (ExtendedPlc plc in PlcConfig.PlcList)
            {
                if (plc.logger.IsStarted)
                {
                    plcActive = true;
                    break;
                }
            }
            if (plcActive)
            {
                var successReads = 0;
                var failReads = 0;
                var loggedValues = 0;
                var activeTags = 0;
                foreach (TagDefinitions tag in TagsToLog.AllLogValues)
                {
                    if (tag.plcName == MyPlc.PlcName && tag.active)
                    {
                        successReads = successReads + tag.NrSuccededReadAttempts;
                        failReads = failReads + tag.NrFailedReadAttempts;
                        loggedValues = loggedValues + tag.TimesLogged;
                        activeTags++;
                    }

                }

                txtOk.Text = successReads.ToString();
                txtFail.Text = failReads.ToString();
                if (successReads > 0 || failReads > 0)
                {
                    var percent = ((successReads) / (successReads + failReads)).ToString("P");
                    txtPercent.Text = percent;
                }
                else
                    txtPercent.Text = "-";
                txtLog.Text = loggedValues.ToString();
                txtTags.Text = activeTags.ToString();
            }
            else
            {
                txtOk.Text = "-";
                txtFail.Text = "-";
                txtPercent.Text = "-";
                txtLog.Text = "-";
                txtTags.Text = "-";
            }
        }
    }
}
