using OptimaValue.Handler.PLC.MyPlc.Graphics;
using S7.Net;
using System;
using System.ComponentModel;
using System.Data.SqlClient;
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


        public PlcSettingsControl(ConnectionStatus conStatus, string plcName, string plcIp,
            string slot, string rack, bool active, CpuType cpuType, int id, ExtendedPlc myPlc)
        {
            InitializeComponent();
            connectionStatus = conStatus;
            PlcName = plcName;
            PlcIp = plcIp;
            Slot = slot;
            Rack = rack;
            Active = active;
            CpuType = cpuType;
            Id = id;
            MyPlc = myPlc;
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
                else if (!InputValidation.CheckIPValid(txtIp.Text))
                {
                    errorProvider.SetError(txtIp, "Ingen giltig IP-adress");
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
                else if (!InputValidation.CheckIPValid(txtIp.Text))
                {
                    errorProvider.SetError(txtIp, "Ingen giltig IP-adress");
                    ipOk = false;
                }
                else
                {
                    errorProvider.SetError(txtIp, "");
                    ipOk = true;
                }
            }

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

        private void ValidateAll()
        {
            if (comboCpu.SelectedItem != null)
                cpuOk = true;
            if (txtIp.Text.CheckValidIpAddress())
                ipOk = true;
            if (short.TryParse(txtSlot.Text, out _))
                slotOk = true;
            if (short.TryParse(txtRack.Text, out _))
                rackOk = true;
            if (!string.IsNullOrEmpty(txtName.Text))
                nameOk = true;
        }

        private void SavePlcConfig()
        {
            string connectionString = PlcConfig.ConnectionString();
            string activeString;
            if (checkActive.Checked)
                activeString = "True";
            else
                activeString = "False";

            string query;
            if (CheckIfExists())
            {
                query = $"UPDATE {SqlSettings.Default.Databas}.dbo.plcConfig SET active='{activeString}',name='{txtName.Text}'";
                query += $",ipAddress='{txtIp.Text}',cpuType='{comboCpu.SelectedItem}',rack={txtRack.Text},slot={txtSlot.Text}";
                query += $" WHERE id = {Id}";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                query = $"UPDATE {SqlSettings.Default.Databas}.dbo.tagConfig SET plcName='{txtName.Text}' ";
                query += $"WHERE plcName = '{PlcName}'";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                query = $"INSERT INTO {SqlSettings.Default.Databas}.dbo.plcConfig (active,name,ipAddress,cpuType,rack,slot)";
                query += $"VALUES ('{activeString}','{txtName.Text}','{txtIp.Text}','{comboCpu.SelectedItem}',{txtRack.Text},{txtSlot.Text})";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }



            RedrawTreeEvent.RaiseMessage(true);
        }

        private bool CheckIfExists()
        {
            object result = new object();
            var query = $"Select top 1 name FROM {SqlSettings.Default.Databas}.dbo.plcConfig WHERE name ='{PlcName}'";
            string connectionString = PlcConfig.ConnectionString();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        result = cmd.ExecuteScalar();
                    }
                }

            }
            catch (SqlException ex)
            {
                $"Misslyckades att läsa från SQL\r\n{ex.Message}".SendThisStatusMessage(Severity.Error);
            }

            if (result != null)
                return true;
            else
                return false;

        }

        private bool CheckIfExistsDelete()
        {
            object result = new object();
            var query = $"Select top 1 id FROM {SqlSettings.Default.Databas}.dbo.plcConfig WHERE id ='{Id}'";
            string connectionString = PlcConfig.ConnectionString();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        result = cmd.ExecuteScalar();
                    }
                }

            }
            catch (SqlException ex)
            {
                $"Misslyckades att läsa från SQL\r\n{ex.Message}".SendThisStatusMessage(Severity.Error);
            }

            if (result != null)
                return true;
            else
                return false;

        }

        private void PlcSettingsControl_Load(object sender, EventArgs e)
        {
            txtName.Text = PlcName;
            txtIp.Text = PlcIp;
            txtSlot.Text = Slot;
            txtRack.Text = Rack;
            checkActive.Checked = Active;
            comboCpu.SelectedItem = CpuType.ToString();
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

            if (!CheckIfExistsDelete())
                RedrawTreeEvent.RaiseMessage(true);
            else
            {
                var query = $"DELETE FROM {SqlSettings.Default.Databas}.dbo.plcConfig ";
                query += $"WHERE id ='{Id}'";
                var connectionString = PlcConfig.ConnectionString();
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    $"Misslyckades ta bort PLC från SQL\r\n{ex.Message}".SendThisStatusMessage(Severity.Error);

                }
                RedrawTreeEvent.RaiseMessage(true);
            }
        }


    }
}
