using Opc.Ua;
using OpcUaHm.Common;
using OptimaValue;
using S7.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Sms;

namespace OptimaValue
{
    public partial class ReadTagControl : UserControl
    {
        private PlcTag tag;
        private IPlc plc;
        private System.Windows.Forms.Timer timer;

        public ReadTagControl(IPlc plc, PlcTag tag)
        {
            InitializeComponent();

            Disposed += Ondisposed;

            this.plc = plc;

            this.tag = tag;
            this.timer = new System.Windows.Forms.Timer()
            {
                Interval = 1000
            };
            timer.Start();
            timer.Tick += Timer_Tick;
        }

        private void Ondisposed(object sender, EventArgs e)
        {
            timer.Tick -= Timer_Tick;
            timer.Stop();
            timer.Dispose();
            plc.Disconnect();
            plc.Dispose();
        }

        private SemaphoreSlim lockObject = new(1);
        private async void Timer_Tick(object? sender, EventArgs e)
        {
            if (lockObject.CurrentCount == 0)
                return;

            await lockObject.WaitAsync();
            if (!plc.IsConnected)
            {
                await plc.ConnectAsync();
            }
            if (plc.IsConnected)
            {
                txtConnection.Text = $"Ansluten till {plc.PlcName}";
                if (plc.isNotPlc)
                {
                    var client = plc as OpcPlc;
                    try
                    {
                        var readEvent = await client.ReadAsync<object>(tag);
                        this.Invoke(new Action(() =>
                        {
                            if (readEvent.Type.IsArray)
                            {
                                txtValue.Text = string.Join(", ", (readEvent.Value as Array).Cast<object>());
                            }
                            else
                                txtValue.Text = readEvent.Value.ToString();

                            lblTagName.Text = tag.Name;
                            lblQuality.Text = readEvent.Quality == "Good" ? "Bra" : readEvent.Quality;
                            lblTime.Text = readEvent.SourceTimestamp.ToLocalTime().ToString();
                            lblDataType.Text = readEvent.Type.ToString();
                        }));
                    }
                    catch (Exception ex)
                    {
                        plc.SendPlcStatusMessage($"Lyckades ej ansluta till {plc.PlcName}", Status.Warning);
                    }
                }
                else
                {
                    try
                    {
                        var siemensPlc = plc as SiemensPlc;
                        var readEvent = await siemensPlc.ReadAsync(tag);
                        this.Invoke(new Action(() =>
                        {

                            txtValue.Text = readEvent.ToString();

                            lblTagName.Text = tag.Name;
                            lblQuality.Text = "Bra";
                            lblTime.Text = DateTime.Now.ToString();
                            lblDataType.Text = readEvent.GetType().ToString();
                        }));
                    }
                    catch { plc.SendPlcStatusMessage($"Lyckades ej ansluta till {plc.PlcName}", Status.Warning); }

                }
            }
            else
                txtConnection.Text = $"Inte ansluten till {plc.PlcName}";

            lockObject.Release();
        }

    }
}
