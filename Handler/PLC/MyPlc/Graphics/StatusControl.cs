using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OptimaValue.Handler.PLC.MyPlc.Graphics
{
    public partial class StatusControl : UserControl
    {
        private bool isSubscribed = false;
        private readonly string PlcName;
        private readonly System.Windows.Forms.Timer statusTimer = new System.Windows.Forms.Timer();
        private bool destroyed = false;

        public StatusControl(string plcName)
        {
            InitializeComponent();
            PlcName = plcName;
            txtPlc.Text = PlcName;
            lblPlcStatus.Text = $"{PlcName} Status";
            txtStatus.Text = string.Empty;
            panelStatus.BackColor = UIColors.GreyColor;
            Subscribe(true);
            statusTimer.Interval = 5000;
            this.HandleDestroyed += StatusControl_HandleDestroyed;
        }

        private void StatusControl_HandleDestroyed(object sender, System.EventArgs e)
        {
            destroyed = true;
            Subscribe(false);
        }

        private void Subscribe(bool _subscribe)
        {
            if (!isSubscribed && _subscribe)
            {
                PlcStatusEvent.NewMessage += PlcStatusEvent_NewMessage;
                OnlineStatusEvent.NewMessage += OnlineStatusEvent_NewMessage;
                statusTimer.Tick += StatusTimer_Tick;
                isSubscribed = true;
            }
            else if (isSubscribed && !_subscribe)
            {
                PlcStatusEvent.NewMessage -= PlcStatusEvent_NewMessage;
                statusTimer.Tick -= StatusTimer_Tick;
                OnlineStatusEvent.NewMessage -= OnlineStatusEvent_NewMessage;
                isSubscribed = false;
            }
        }

        private void StatusTimer_Tick(object sender, System.EventArgs e)
        {
            txtStatus.Text = string.Empty;
            statusTimer.Stop();
            errorImage.Visible = false;
        }

        private void OnlineStatusEvent_NewMessage(object sender, OnlineStatusEventArgs e)
        {
            if (sender == null)
                return;
            if (InvokeRequired && !destroyed)
            {
                Invoke((MethodInvoker)delegate { OnlineStatusEvent_NewMessage(sender, e); });
                return;
            }
            if (e.PlcName == PlcName)
            {
                // Hantera anslutningsstatus och driftstid
                txtUpTime.Visible = e.ConnectionStatus == ConnectionStatus.Connected;
                txtMisc.Visible = e.ConnectionStatus == ConnectionStatus.Connected;
                txtUpTime.Text = $"Kontinuerlig drift: {e.ElapsedTime}";
                txtPlc.Text = e.PlcName;
                panelStatus.BackColor = e.Color;

                // Kombinera statistiken i en enda textsträng för txtMisc
                var miscInfo = new StringBuilder();
                miscInfo.AppendLine($"Anslutningsförsök: {e.TotalConnectionAttempts}");
                miscInfo.AppendLine($"Misslyckade anslutningsförsök: {e.FailedConnectionAttempts}");

                // Hantera anslutningstid i millisekunder
                try
                {
                    miscInfo.AppendLine($"Anslutningstid snitt: {Math.Floor(e.TotalReconnectTime.TotalMilliseconds / e.TotalConnectionAttempts)} ms");
                }
                catch (FormatException ex)
                {
                    miscInfo.AppendLine("Anslutningstid snitt: Formatfel");
                    Console.WriteLine($"FormatException: {ex.Message}");
                }

                // Tilldela den kombinerade texten till txtMisc
                txtMisc.Text = miscInfo.ToString();
            }
        }



        private void PlcStatusEvent_NewMessage(object sender, PlcStatusEventArgs e)
        {
            try
            {
                if (sender == null)
                    return;
                if (InvokeRequired && !destroyed)
                {
                    Invoke((MethodInvoker)delegate { PlcStatusEvent_NewMessage(sender, e); });
                    return;
                }
                if (e.PlcName == PlcName)
                {
                    txtStatus.Text = e.Message;
                    if (e.Message != "")
                        statusTimer.Start();
                }
                if (e.Status == Status.Error || e.Status == Status.Warning)
                    errorImage.Visible = true;
            }
            catch (System.ComponentModel.InvalidAsynchronousStateException) { }

        }
    }
}
