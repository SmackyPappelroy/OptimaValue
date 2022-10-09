using Opc.Ua.Export;
using OpcUa;
using OpcUaHm.Common;
using OptimaValue.Handler.PLC.MyPlc.Graphics;
using S7.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace OptimaValue.Handler.PLC.Graphics
{
    public partial class PlcSettingsControl : UserControl
    {
        public ConnectionStatus connectionStatus;
        public string PlcName = string.Empty;
        public string PlcIp = string.Empty;
        public string Slot = string.Empty;
        public string Rack = string.Empty;
        public bool Active = false;
        public int Id = 0;
        public CpuType CpuType;
        private ExtendedPlc MyPlc;
        private bool destroyed = false;
        private bool DeleteFormOpen = false;
        private DeletePlcConfirmationForm confirmForm;
        private SyncPlcTimeForm syncForm;
        private bool syncFormOpen;

        private Timer timeoutTimer;
        private bool tryConnect = false;
        private bool connected = false;

        private bool PlcExists => DatabaseSql.DoesPlcExist(PlcName);



        public PlcSettingsControl(ConnectionStatus conStatus, string plcName, string plcIp,
            string slot, string rack, bool active, CpuType cpuType, int id, ExtendedPlc myPlc)
        {
            InitializeComponent();


            timeoutTimer = new Timer()
            {
                Interval = 5000
            };
            timeoutTimer.Tick += TimeoutTimer_Tick;


            connectionStatus = conStatus;
            PlcName = plcName;
            PlcIp = plcIp;
            Slot = slot;
            Rack = rack;
            Active = active;
            Id = id;
            MyPlc = myPlc;
            if (MyPlc != null)
            {
                if (myPlc.isOpc)
                {
                    CpuType = myPlc.CpuType;
                }
            }
            else
                CpuType = CpuType.S71500;
            btnSyncTime.Visible = false;
            if (MyPlc != null)
            {
                MyPlc.StartedEvent += Logger_StartedEvent;
                if (MyPlc.LoggerIsStarted)
                {
                    this.Enabled = false;
                }
                else
                {
                    btnDelete.Enabled = true;
                    btnSave.Enabled = true;
                }
                btnSyncTime.Visible = true;
            }

            this.HandleDestroyed += PlcSettingsControl_HandleDestroyed;
            DeleteConfirmationEvent.DeleteEvent += DeleteConfirmationEvent_DeleteEvent;

            imageTest.Image = imageList.Images[0];
            btnConnect.Enabled = false;
        }

        private void TimeoutTimer_Tick(object sender, EventArgs e)
        {
            imageTest.Image = imageList.Images[0];
            tryConnect = false;
            timeoutTimer.Stop();
        }


        private void DeleteConfirmationEvent_DeleteEvent(object sender, DeleteEventArgs e)
        {
            if (e.Delete)
                DeletePlc();
        }

        private void PlcSettingsControl_HandleDestroyed(object sender, EventArgs e)
        {
            destroyed = true;
        }

        private void Logger_StartedEvent(object sender, EventArgs e)
        {
            if (destroyed)
                return;
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { Logger_StartedEvent(sender, e); });
                return;
            }

            if (MyPlc.LoggerIsStarted)
                this.Enabled = false;
            else
                this.Enabled = true;

            foreach (ExtendedPlc logPlc in PlcConfig.PlcList)
            {
                if (logPlc.PlcName == PlcName)
                    MyPlc = logPlc;
            }
        }

        private bool nameOk = false;
        private bool ipOk = false;
        private bool rackOk = false;
        private bool slotOk = false;
        private bool cpuOk = false;

        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            if (MyPlc != null)
            {
                if (string.IsNullOrEmpty(txtName.Text))
                {
                    errorProvider.SetError(txtName, "Fältet får ej va tomt");
                    nameOk = false;
                }
                else if (MyPlc.LoggerIsStarted)
                    errorProvider.SetError(txtName, "Plc aktiv");
                else
                {
                    nameOk = true;
                    errorProvider.SetError(txtName, "");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(txtName.Text))
                {
                    errorProvider.SetError(txtName, "Fältet får ej va tomt");
                    nameOk = false;
                }
                else
                {
                    nameOk = true;
                    errorProvider.SetError(txtName, "");
                }
            }
            if (!nameOk)
                btnConnect.Enabled = false;
            else if (ValidateAll())
                btnConnect.Enabled = true;
        }

        private void txtIp_Validating(object sender, CancelEventArgs e)
        {
            if (MyPlc != null)
            {
                if (string.IsNullOrEmpty(txtIp.Text))
                {
                    errorProvider.SetError(txtIp, "Fältet får ej va tomt");
                    ipOk = false;
                }
                else if (!InputValidation.CheckIPValid(txtIp.Text) && !txtIp.Text.ToLower().StartsWith("opc.tcp://"))
                {
                    errorProvider.SetError(txtIp, "Ingen giltig IP-adress / OPC UA adress");
                    ipOk = false;
                }
                else if (MyPlc.LoggerIsStarted)
                    errorProvider.SetError(txtIp, "Plc aktiv");
                else
                {
                    errorProvider.SetError(txtIp, "");
                    ipOk = true;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(txtIp.Text))
                {
                    errorProvider.SetError(txtIp, "Fältet får ej va tomt");
                    ipOk = false;
                }
                else if (!InputValidation.CheckIPValid(txtIp.Text) || (comboCpu.Text == "OPC" && string.IsNullOrWhiteSpace(txtIp.Text)))
                {
                    errorProvider.SetError(txtIp, "Ingen giltig IP-adress / OPC UA adress");
                    ipOk = false;
                }
                else
                {
                    errorProvider.SetError(txtIp, "");
                    ipOk = true;
                }
            }
            if (!ipOk)
                btnConnect.Enabled = false;
            else if (ValidateAll())
                btnConnect.Enabled = true;


        }

        private void txtRack_Validating(object sender, CancelEventArgs e)
        {
            if (MyPlc != null)
            {
                if (!byte.TryParse(txtRack.Text, out byte rackShort))
                {
                    errorProvider.SetError(txtRack, "Inget giltigt värde");
                    rackOk = false;
                }
                else if (rackShort > 9)
                {
                    errorProvider.SetError(txtRack, "För stort värde");
                    rackOk = false;
                }
                else if (MyPlc.LoggerIsStarted)
                    errorProvider.SetError(txtRack, "Plc aktiv");
                else
                {
                    rackOk = true;
                    errorProvider.SetError(txtIp, "");
                }
            }
            else
            {
                if (!byte.TryParse(txtRack.Text, out byte rackShort))
                {
                    errorProvider.SetError(txtRack, "Inget giltigt värde");
                    rackOk = false;
                }
                else if (rackShort > 9)
                {
                    errorProvider.SetError(txtRack, "För stort värde");
                    rackOk = false;
                }
                else
                {
                    rackOk = true;
                    errorProvider.SetError(txtIp, "");
                }
            }
            if (!rackOk)
                btnConnect.Enabled = false;
            else if (ValidateAll())
                btnConnect.Enabled = true;

        }

        private void txtSlot_Validating(object sender, CancelEventArgs e)
        {
            if (MyPlc != null)
            {
                if (!byte.TryParse(txtSlot.Text, out byte slotShort))
                {
                    errorProvider.SetError(txtSlot, "Inget giltigt värde");
                    slotOk = false;
                }
                else if (slotShort > 9)
                {
                    errorProvider.SetError(txtRack, "För stort värde");
                    slotOk = false;
                }
                else if (MyPlc.LoggerIsStarted)
                    errorProvider.SetError(txtRack, "Plc aktiv");
                else
                {
                    slotOk = true;
                    errorProvider.SetError(txtRack, "");
                }
            }
            else
            {
                if (!byte.TryParse(txtSlot.Text, out byte slotShort))
                {
                    errorProvider.SetError(txtSlot, "Inget giltigt värde");
                    slotOk = false;
                }
                else if (slotShort > 9)
                {
                    errorProvider.SetError(txtRack, "För stort värde");
                    slotOk = false;
                }
                else
                {
                    slotOk = true;
                    errorProvider.SetError(txtRack, "");
                }
            }
            if (!slotOk)
                btnConnect.Enabled = false;
            else if (ValidateAll())
                btnConnect.Enabled = true;


        }

        private void comboCpu_Validating(object sender, CancelEventArgs e)
        {
            if (MyPlc != null)
            {
                if (string.IsNullOrEmpty(comboCpu.SelectedItem.ToString()))
                {
                    errorProvider.SetError(comboCpu, "Fältet får ej va tomt");
                    cpuOk = false;
                }
                else if (MyPlc.LoggerIsStarted)
                    errorProvider.SetError(comboCpu, "Plc aktiv");
                else
                {
                    cpuOk = true;
                    errorProvider.SetError(comboCpu, "");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(comboCpu.SelectedItem.ToString()))
                {
                    errorProvider.SetError(comboCpu, "Fältet får ej va tomt");
                    cpuOk = false;
                }
                else
                {
                    cpuOk = true;
                    errorProvider.SetError(comboCpu, "");
                }
            }
            if (!cpuOk)
                btnConnect.Enabled = false;
            else if (ValidateAll())
                btnConnect.Enabled = true;


        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (MyPlc != null)
            {
                foreach (ExtendedPlc logPlc in PlcConfig.PlcList)
                {
                    if (logPlc.LoggerIsStarted)
                        return;
                }
                if (MyPlc.LoggerIsStarted)
                    return;
            }
            else
            {
                foreach (ExtendedPlc logPlc in PlcConfig.PlcList)
                {
                    if (logPlc.LoggerIsStarted)
                        return;
                }

            }

            ValidateAll();
            if (cpuOk && ipOk && slotOk && rackOk && nameOk)
                SavePlcConfig();
        }

        private bool ValidateAll()
        {
            if (comboCpu.SelectedItem != null)
                cpuOk = true;
            if (txtIp.Text.CheckValidIpAddress() || (!string.IsNullOrWhiteSpace(comboCpu.Text) && !string.IsNullOrWhiteSpace(txtIp.Text)))
                ipOk = true;
            if (short.TryParse(txtSlot.Text, out _))
                slotOk = true;
            if (short.TryParse(txtRack.Text, out _))
                rackOk = true;
            if (!string.IsNullOrEmpty(txtName.Text))
                nameOk = true;

            btnConnect.Enabled = (cpuOk && ipOk && slotOk && rackOk && nameOk);
            return btnConnect.Enabled;
        }

        private void SavePlcConfig()
        {
            string activeString;
            if (checkActive.Checked)
                activeString = "True";
            else
                activeString = "False";

            var ip = "";


            DatabaseSql.SavePlcConfig(activeString,
                name: txtName.Text,
                ip: txtIp.Text,
                cpu: comboCpu.SelectedItem.ToString(),
                rack: txtRack.Text,
                slot: txtSlot.Text,
                id: Id,
                plcName: PlcName);

            RedrawTreeEvent.RaiseMessage(true);
        }



        private void PlcSettingsControl_Load(object sender, EventArgs e)
        {

            txtName.Text = PlcName;
            if (MyPlc == null)
                txtIp.Text = PlcIp;
            else if (!MyPlc.isOpc)
                txtIp.Text = PlcIp;
            else
                txtIp.Text = MyPlc.Plc.ConnectionString;
            txtSlot.Text = Slot;
            txtRack.Text = Rack;
            checkActive.Checked = Active;
            comboCpu.SelectedItem = CpuType.ToString();
            btnConnect.Enabled = ValidateAll();
            if (MyPlc == null)
                btnConnect.Visible = false;
            else
                btnConnect.Visible = true;

            if (MyPlc != null && MyPlc.isOpc)
            {
                lblIpAddress.Text = "Connection string";
                lblSlot.Visible = false;
                txtSlot.Visible = false;
                lblRack.Visible = false;
                txtRack.Visible = false;
            }
            else
            {
                lblIpAddress.Text = "IP-adress";
                lblSlot.Visible = true;
                txtSlot.Visible = true;
                lblRack.Visible = true;
                txtRack.Visible = true;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!DeleteFormOpen)
            {
                confirmForm = new DeletePlcConfirmationForm();
                confirmForm.FormClosed += ConfirmForm_FormClosed;
                confirmForm.ShowDialog();
                DeleteFormOpen = true;
            }
            else
            {
                if (confirmForm != null)
                    confirmForm.BringToFront();
            }
        }

        private void btnSyncTime_Click(object sender, EventArgs e)
        {
            if (MyPlc != null)
            {
                foreach (ExtendedPlc logPlc in PlcConfig.PlcList)
                {
                    if (logPlc.LoggerIsStarted)
                        return;
                }
                if (MyPlc.LoggerIsStarted)
                    return;
            }
            else
                return;

            if (!syncFormOpen)
            {
                syncForm = new SyncPlcTimeForm(MyPlc);
                syncForm.Show();
                syncFormOpen = true;
                this.Enabled = false;
                syncForm.SavedEvent += (temp) =>
                {
                    btnSave_Click(this, EventArgs.Empty);
                };
                syncForm.FormClosed += SyncForm_FormClosed;

            }
        }

        private void SyncForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Enabled = true;
            syncFormOpen = false;
        }

        private void ConfirmForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            DeleteFormOpen = true;
            confirmForm.FormClosed -= ConfirmForm_FormClosed;
        }

        private void DeletePlc()
        {
            if (MyPlc != null)
            {
                foreach (ExtendedPlc logPlc in PlcConfig.PlcList)
                {
                    if (logPlc.LoggerIsStarted)
                        return;
                }
                if (MyPlc.LoggerIsStarted)
                    return;
            }
            else
            {
                foreach (ExtendedPlc logPlc in PlcConfig.PlcList)
                {
                    if (logPlc.LoggerIsStarted)
                        return;
                }
                RedrawTreeEvent.RaiseMessage(true);
                return;
            }

            if (!PlcExists)
                RedrawTreeEvent.RaiseMessage(true);
            else
            {
                DatabaseSql.DeletePlc(Id);
                RedrawTreeEvent.RaiseMessage(true);
            }
        }



        private void PlcSettingsControl_Leave(object sender, EventArgs e)
        {
            if (tryConnect)
            {
                MyPlc.Plc.Disconnect();
            }
        }


        bool loading = false;
        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (ValidateAll())
            {
                loading = true;
                tableLayoutPanel2.Enabled = false;
                try
                {
                    tryConnect = true;
                    btnConnect.Enabled = false;
                    Application.UseWaitCursor = true;
                    if (!MyPlc.isOpc)
                        await MyPlc.Plc.ConnectAsync();
                    else
                    {
                        await MyPlc.Plc.ConnectAsync();
                        PopulateOpcTopics();
                    }
                    imageTest.Image = imageList.Images[2];
                    SystemSounds.Beep.Play();
                    connected = true;
                    $"Ansluten till {MyPlc.PlcName}".SendStatusMessage(Severity.Success);
                }
                catch (Exception ex)
                {
                    imageTest.Image = imageList.Images[1];
                    connected = false;
                    SystemSounds.Hand.Play();
                    $"Ingen anslutning till {MyPlc.PlcName}".SendStatusMessage(Severity.Error);
                }
                finally
                {
                    if (!MyPlc.isOpc)
                        MyPlc.Plc.Disconnect();
                    else if (MyPlc.IsConnected)
                        MyPlc.Plc.Dispose();
                    btnConnect.Enabled = true;
                    timeoutTimer.Start();
                    Application.UseWaitCursor = false;
                    tableLayoutPanel2.Enabled = true;
                    loading = false;
                }
            }
        }

        private List<Node> PopulateOpcTopics()
        {
            var opcClient = MyPlc.Plc as OpcPlc;
            var nodes = opcClient.Client.ExploreOpc("", true);
            comboOpcTopic.Items.Clear();
            nodes = nodes.OrderBy(x => x.Tag).ToList();
            foreach (var item in nodes)
            {
                comboOpcTopic.Items.Add(item.Tag);
            }
            comboOpcTopic.SelectedIndex = 0;
            return nodes.ToList();
        }

        private void comboCpu_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboCpu.SelectedItem.ToString() == "OPC")
            {
                lblIpAddress.Text = "Connection string";
                lblSlot.Visible = false;
                txtSlot.Visible = false;
                lblRack.Visible = false;
                txtRack.Visible = false;
                comboOpcTopic.Visible = true;
            }
            else
            {
                comboOpcTopic.Visible = false;
                lblIpAddress.Text = "IP-adress";
                lblSlot.Visible = true;
                txtSlot.Visible = true;
                lblRack.Visible = true;
                txtRack.Visible = true;
            }
        }

        private void comboOpcTopic_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                txtName.Text = comboOpcTopic.Text;
                try
                {
                    var opcPlc = MyPlc.Plc as OpcPlc;
                    opcPlc.Connect();
                    var subNodes = opcPlc.Client.ExploreOpc(txtName.Text, false, true);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    var opcPlc = MyPlc.Plc as OpcPlc;
                    opcPlc.Disconnect();
                }
            }
        }
    }
}
