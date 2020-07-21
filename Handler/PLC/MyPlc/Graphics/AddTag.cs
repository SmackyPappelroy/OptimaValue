using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace OptimaValue.Handler.PLC.MyPlc.Graphics
{
    public partial class AddTag : Form
    {
        private string PlcName = string.Empty;
        private TagDefinitions tag;
        public event EventHandler TagChanged;
        private ExtendedPlc MyPlc;

        protected virtual void OnTagChanged(EventArgs e)
        {
            TagChanged?.Invoke(this, e);
        }
        public AddTag(string plcName, ExtendedPlc myPlc, TagDefinitions taggen = null)
        {
            InitializeComponent();
            PlcName = plcName;
            tag = taggen;
            if (taggen != null)
                PopulateInputs();
            MyPlc = myPlc;
            if (MyPlc.logger.IsStarted)
            {
                btnSave.Enabled = false;
                btnNew.Enabled = false;
            }

            if (taggen == null)
            {
                txtTimeOfDay.Text = "12:00";
                txtDeadband.Text = 0.ToString();
                txtBitAddress.Text = 0.ToString();
                comboLogFreq.SelectedItem = "_1s";
                txtNrOfElements.Text = 1.ToString();
                comboLogFreq.Text = "_1s";
                comboDataType.Text = "DataBlock";
                comboDataType.SelectedItem = "DataBlock";
                comboLogType.Text = "Cyclic";
                comboLogType.SelectedItem = "Cyclic";
            }
        }

        private void PopulateInputs()
        {
            checkActive.Checked = tag.active;
            txtName.Text = tag.name;
            comboLogType.Text = tag.logType.ToString();
            comboLogType.SelectedItem = tag.logType;

            txtTimeOfDay.Text = tag.timeOfDay.ToString();
            txtDeadband.Text = tag.deadband.ToString();
            comboVarType.Text = tag.varType.ToString();
            comboVarType.SelectedItem = tag.varType;

            txtBlockNr.Text = tag.blockNr.ToString();
            comboDataType.Text = tag.dataType.ToString();
            comboDataType.SelectedItem = tag.dataType;

            txtStartByte.Text = tag.startByte.ToString();
            txtNrOfElements.Text = tag.nrOfElements.ToString();
            txtBitAddress.Text = tag.bitAddress.ToString();
            comboLogFreq.Text = tag.logFreq.ToString();
            comboLogFreq.SelectedItem = tag.logFreq;

            txtUnit.Text = tag.tagUnit;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        #region Validation
        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
                errorProvider.SetError(txtName, "Fyll i ett namn");
            else
                errorProvider.SetError(txtName, "");
        }

        private void txtTimeOfDay_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtTimeOfDay.Text))
                errorProvider.SetError(txtTimeOfDay, "Ange en tid på dagen");
            else if (!TimeSpan.TryParse(txtTimeOfDay.Text, out TimeSpan _))
                errorProvider.SetError(txtTimeOfDay, "Inte ett klockslag");
            else
                errorProvider.SetError(txtTimeOfDay, "");
        }

        private void txtDeadband_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDeadband.Text))
                errorProvider.SetError(txtDeadband, "Ange ett numeriskt värde");
            else if (!uint.TryParse(txtDeadband.Text, out uint _))
                errorProvider.SetError(txtDeadband, "Inte ett numeriskt positivt värde");
            else
                errorProvider.SetError(txtDeadband, "");
        }

        private void txtBlockNr_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBlockNr.Text))
                errorProvider.SetError(txtBlockNr, "Ange ett numeriskt värde");
            else if (!uint.TryParse(txtBlockNr.Text, out uint _))
                errorProvider.SetError(txtBlockNr, "Inte ett numeriskt positivt värde");
            else
                errorProvider.SetError(txtBlockNr, "");
        }

        private void txtStartByte_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtStartByte.Text))
                errorProvider.SetError(txtStartByte, "Ange ett numeriskt värde");
            else if (!uint.TryParse(txtStartByte.Text, out uint _))
                errorProvider.SetError(txtStartByte, "Inte ett numeriskt positivt värde");
            else
                errorProvider.SetError(txtStartByte, "");
        }

        private void txtNrOfElements_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNrOfElements.Text))
                errorProvider.SetError(txtNrOfElements, "Ange ett numeriskt värde");
            else if (!uint.TryParse(txtNrOfElements.Text, out uint _))
                errorProvider.SetError(txtNrOfElements, "Inte ett numeriskt positivt värde");
            else
                errorProvider.SetError(txtNrOfElements, "");
        }

        private void txtBitAddress_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBitAddress.Text))
                errorProvider.SetError(txtBitAddress, "Ange ett numeriskt värde");
            else if (!byte.TryParse(txtBitAddress.Text, out byte resultByte))
                errorProvider.SetError(txtBitAddress, "Inte ett numeriskt positivt värde");
            else if (resultByte > 16)
                errorProvider.SetError(txtBitAddress, "För stort värde");
            else
                errorProvider.SetError(txtBitAddress, "");
        }

        private bool ValidateAll()
        {
            var okName = false;
            var okLogType = false;
            var okTimeOfDay = false;
            var okDeadBand = false;
            var okVarType = false;
            var okBlockNr = false;
            var okDataType = false;
            var okStartByte = false;
            var okNrOfElements = false;
            var okBitAddress = false;
            var okLogFreq = false;

            if (!string.IsNullOrEmpty(txtName.Text))
                okName = true;

            if (!string.IsNullOrEmpty(comboLogType.SelectedItem.ToString()))
                okLogType = true;

            if (TimeSpan.TryParse(txtTimeOfDay.Text, out TimeSpan _))
                okTimeOfDay = true;

            if (ushort.TryParse(txtDeadband.Text, out ushort _))
                okDeadBand = true;

            if (!string.IsNullOrEmpty(comboVarType.SelectedItem.ToString()))
                okVarType = true;

            if (ushort.TryParse(txtBlockNr.Text, out ushort _))
                okVarType = true;

            if (!string.IsNullOrEmpty(comboDataType.SelectedItem.ToString()))
                okDataType = true;

            if (byte.TryParse(txtStartByte.Text, out byte _))
                okStartByte = true;

            if (ushort.TryParse(txtNrOfElements.Text, out ushort _))
                okNrOfElements = true;

            if (ushort.TryParse(txtBlockNr.Text, out ushort _))
                okBlockNr = true;

            if (byte.TryParse(txtBitAddress.Text, out byte _))
                okBitAddress = true;

            if (!string.IsNullOrEmpty(comboLogFreq.SelectedItem.ToString()))
                okLogFreq = true;

            if (okName && okTimeOfDay && okDeadBand && okVarType && okBlockNr
                && okDataType && okStartByte && okNrOfElements && okBitAddress &&
                okLogFreq && okLogType)
            {
                return true;
            }
            return false;

        }

        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateAll())
            {
                DataTable tbl = CreateTable();
                AddTagToSql(tbl);
            }
        }

        private void AddTagToSql(DataTable dataTable)
        {
            if (CheckIfExists())
                DeleteTag();
            AddNewTag();
        }

        private void AddNewTag()
        {
            var connectionString = PlcConfig.ConnectionString();
            var query = $"INSERT INTO {SqlSettings.Default.Databas}.dbo.tagConfig ";
            query += $"(active,name,logType,timeOfDay,deadband,plcName,varType,blockNr,dataType,startByte,nrOfElements,bitAddress,logFreq,tagUnit) ";
            query += $"VALUES ('{checkActive.Checked}','{txtName.Text}','{comboLogType.SelectedItem}','{txtTimeOfDay.Text}',";
            query += $"{int.Parse(txtDeadband.Text)},'{PlcName}','{comboVarType.SelectedItem}',{int.Parse(txtBlockNr.Text)}, ";
            query += $"'{comboDataType.SelectedItem}',{int.Parse(txtStartByte.Text)},{int.Parse(txtNrOfElements.Text)},";
            query += $"{byte.Parse(txtBitAddress.Text)},'{comboLogFreq.SelectedItem}','{txtUnit.Text}')";

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
                StatusEvent.RaiseMessage(ex.Message, Status.Error);
            }
        }

        private void DeleteTag()
        {
            var tagNamn = string.Empty;
            if (tag == null)
                tagNamn = txtName.Text;
            else
                tagNamn = tag.name;
            var connectionString = PlcConfig.ConnectionString();
            var query = $"DELETE FROM {SqlSettings.Default.Databas}.dbo.tagConfig ";
            query += $"WHERE name = '{tagNamn}' AND plcName = '{MyPlc.PlcName}'";
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
                StatusEvent.RaiseMessage(ex.Message, Status.Error);
            }
        }

        private bool CheckIfExists()
        {
            var tagNamn = string.Empty;
            if (tag == null)
                tagNamn = txtName.Text;
            else
                tagNamn = tag.name;
            object result = new object();
            var connectionString = PlcConfig.ConnectionString();
            var query = $"SELECT TOP 1 name FROM {SqlSettings.Default.Databas}.dbo.tagConfig ";
            query += $"WHERE name = '{tagNamn}' AND plcName = '{MyPlc.PlcName}'";
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
                StatusEvent.RaiseMessage(ex.Message, Status.Error);
            }
            if (result != null)
                return true;
            return false;
        }

        private DataTable CreateTable()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("active", typeof(bool));
            tbl.Columns.Add("name", typeof(string));
            tbl.Columns.Add("logType", typeof(string));
            tbl.Columns.Add("timeOfDay", typeof(TimeSpan));
            tbl.Columns.Add("deadband", typeof(float));
            tbl.Columns.Add("plcName", typeof(string));
            tbl.Columns.Add("varType", typeof(string));
            tbl.Columns.Add("blockNr", typeof(int));
            tbl.Columns.Add("dataType", typeof(string));
            tbl.Columns.Add("startByte", typeof(int));
            tbl.Columns.Add("nrOfElements", typeof(int));
            tbl.Columns.Add("bitAddress", typeof(byte));
            tbl.Columns.Add("logFreq", typeof(string));

            tbl.Rows.Add(checkActive.Checked, txtName.Text, comboLogType.SelectedItem.ToString(),
                TimeSpan.Parse(txtTimeOfDay.Text), float.Parse(txtDeadband.Text),
                PlcName, comboVarType.SelectedItem.ToString(), int.Parse(txtBlockNr.Text),
                comboDataType.SelectedItem.ToString(), int.Parse(txtStartByte.Text),
                int.Parse(txtNrOfElements.Text), byte.Parse(txtBitAddress.Text),
                comboLogFreq.SelectedItem.ToString());
            return tbl;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddNewTag();
        }
    }
}
