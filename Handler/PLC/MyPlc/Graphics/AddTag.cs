using S7.Net;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OptimaValue.Handler.PLC.MyPlc.Graphics
{
    public partial class AddTag : Form
    {
        private readonly string PlcName = string.Empty;
        private TagDefinitions tag;
        public event EventHandler TagChanged;
        private readonly ExtendedPlc MyPlc;
        private EventForm eventForm;
        private bool eventFormOpen = false;
        private bool startup = true;
        private System.Windows.Forms.Timer timeOut = new System.Windows.Forms.Timer();
        private Color greyColor = Color.FromArgb(67, 62, 71);
        private Color greenColor = Color.FromArgb(46, 148, 66);
        private Color blueColor = Color.FromArgb(46, 127, 148);

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
            if (MyPlc.LoggerIsStarted)
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
            timeOut.Interval = 300;
            timeOut.Tick += TimeOut_Tick;
        }

        private void TimeOut_Tick(object sender, EventArgs e)
        {
            btnNew.BackColor = greyColor;
            btnSave.BackColor = greyColor;
            btnNew.Image = Properties.Resources.add_new_64px;
            btnSave.Image = Properties.Resources.available_updates_64px;
            timeOut.Stop();
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
            else if (!float.TryParse(txtDeadband.Text, out float _))
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
            var tempString = string.Empty;

            if (!string.IsNullOrEmpty(txtName.Text))
                okName = true;

            if (!string.IsNullOrEmpty(comboLogType.SelectedItem.ToString()))
                okLogType = true;

            if (TimeSpan.TryParse(txtTimeOfDay.Text, out TimeSpan _))
                okTimeOfDay = true;

            if (float.TryParse(txtDeadband.Text, out float _))
                okDeadBand = true;

            if (!string.IsNullOrEmpty(comboVarType.SelectedItem.ToString()))
                okVarType = true;

            if (ushort.TryParse(txtBlockNr.Text, out ushort _))
                okBlockNr = true;

            if (!string.IsNullOrEmpty(comboDataType.SelectedItem.ToString()))
                okDataType = true;

            if (ushort.TryParse(txtStartByte.Text, out ushort _))
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!txtName.Text.Equals(tag.name))
                if (CheckForDuplicateNames())
                    return;

            if (ValidateAll())
            {
                if (tag.eventId == 0)
                {
                    tag.eventId = 0;
                    tag.IsBooleanTrigger = false;
                    tag.boolTrigger = BooleanTrigger.OnTrue;
                    tag.analogTrigger = AnalogTrigger.Equal;
                    tag.analogValue = 0;
                }
                DataTable tbl = CreateTable();
                AddTagToSql(tbl);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!CheckForDuplicateNames())
                AddNewTag();
        }

        private void AddTagToSql(DataTable dataTable)
        {
            if (CheckIfExists())
                UpdateTag();
        }

        private bool CheckIfExists()
        {
            string tagNamn;
            if (tag == null)
                tagNamn = txtName.Text;
            else
                tagNamn = tag.name;
            object result = new object();
            var connectionString = PlcConfig.ConnectionString();
            var query = $"SELECT TOP 1 name FROM {SqlSettings.Default.Databas}.dbo.tagConfig ";
            query += $"WHERE id = {tag.id}";
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
                Apps.Logger.Log(string.Empty, Severity.Error, ex);
            }
            if (result != null)
                return true;
            return false;
        }

        private bool CheckForDuplicateNames()
        {
            object result = new object();
            var connectionString = PlcConfig.ConnectionString();
            var query = $"SELECT TOP 1 name FROM {SqlSettings.Default.Databas}.dbo.tagConfig ";
            query += $"WHERE name = '{txtName.Text}' AND plcName = '{PlcName}'";
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
                Apps.Logger.Log(string.Empty, Severity.Error, ex);
            }
            if (result != null)
                return true;
            return false;
        }

        private void AddNewTag()
        {
            int tagEventId = 0;
            bool isBoolTrigger = false;
            float analogValue = 0;
            string boolTrigger = "OnTrue";
            string analogTrigger = "Equal";
            if (!(tag == null))
            {
                tagEventId = tag.eventId;
                isBoolTrigger = tag.IsBooleanTrigger;
                analogTrigger = tag.analogTrigger.ToString();
                boolTrigger = tag.boolTrigger.ToString();
                analogValue = tag.analogValue;

            }

            var connectionString = PlcConfig.ConnectionString();
            var query = $"INSERT INTO {SqlSettings.Default.Databas}.dbo.tagConfig ";
            query += $"(active,name,logType,timeOfDay,deadband,plcName,varType,blockNr,dataType,startByte,nrOfElements,bitAddress,logFreq,";
            query += $"tagUnit,eventId,isBooleanTrigger,boolTrigger,analogTrigger,analogValue) ";
            query += $"VALUES ('{checkActive.Checked}','{txtName.Text}','{comboLogType.SelectedItem}','{txtTimeOfDay.Text}',";
            query += $"{float.Parse(txtDeadband.Text)},'{PlcName}','{comboVarType.SelectedItem}',{int.Parse(txtBlockNr.Text)}, ";
            query += $"'{comboDataType.SelectedItem}',{int.Parse(txtStartByte.Text)},{int.Parse(txtNrOfElements.Text)},";
            query += $"{byte.Parse(txtBitAddress.Text)},'{comboLogFreq.SelectedItem}','{txtUnit.Text}',{tagEventId},'{isBoolTrigger}','";
            query += $"{boolTrigger}','{analogTrigger}',{analogValue})";

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
                Apps.Logger.Log(string.Empty, Severity.Error, ex);
            }

            query = $"SELECT TOP 1 * FROM {SqlSettings.Default.Databas}.dbo.tagConfig ORDER BY id DESC";
            var tbl = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        {
                            con.Open();
                            adp.Fill(tbl);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Apps.Logger.Log(string.Empty, Severity.Error, ex);
            }

            var _active = (tbl.AsEnumerable().ElementAt(0).Field<bool>("active"));
            var _bitAddress = (tbl.AsEnumerable().ElementAt(0).Field<byte>("bitAddress"));
            var _blockNr = (tbl.AsEnumerable().ElementAt(0).Field<int>("blockNr"));
            var _dataType = (S7.Net.DataType)Enum.Parse(typeof(S7.Net.DataType), (tbl.AsEnumerable().ElementAt(0).Field<string>("dataType")));
            var _deadband = (float)(tbl.AsEnumerable().ElementAt(0).Field<double>("deadband"));
            var _id = (tbl.AsEnumerable().ElementAt(0).Field<int>("id"));
            var _logFreq = (LogFrequency)Enum.Parse(typeof(LogFrequency), (tbl.AsEnumerable().ElementAt(0).Field<string>("logFreq")));
            var _logType = (LogType)Enum.Parse(typeof(LogType), (tbl.AsEnumerable().ElementAt(0).Field<string>("logType")));
            var _name = (tbl.AsEnumerable().ElementAt(0).Field<string>("name"));
            var _nrOfElements = (tbl.AsEnumerable().ElementAt(0).Field<int>("nrOfElements"));
            var _plcName = (tbl.AsEnumerable().ElementAt(0).Field<string>("plcName"));
            var _startByte = (tbl.AsEnumerable().ElementAt(0).Field<int>("startByte"));
            var _timeOfDay = (tbl.AsEnumerable().ElementAt(0).Field<TimeSpan>("timeOfDay"));
            var _varType = (VarType)Enum.Parse(typeof(VarType), (tbl.AsEnumerable().ElementAt(0).Field<string>("varType")));
            var _tagUnit = (tbl.AsEnumerable().ElementAt(0).Field<string>("tagUnit"));
            var _eventId = (tbl.AsEnumerable().ElementAt(0).Field<int>("eventId"));
            var _isBooleanTrigger = (tbl.AsEnumerable().ElementAt(0).Field<bool>("isBooleanTrigger"));
            var _boolTrigger = (BooleanTrigger)Enum.Parse(typeof(BooleanTrigger), (tbl.AsEnumerable().ElementAt(0).Field<string>("boolTrigger")));
            var _analogTrigger = (AnalogTrigger)Enum.Parse(typeof(AnalogTrigger), (tbl.AsEnumerable().ElementAt(0).Field<string>("analogTrigger")));
            var _analogValue = (float)(tbl.AsEnumerable().ElementAt(0).Field<double>("analogValue"));


            tag = new TagDefinitions()
            {
                active = _active,
                bitAddress = _bitAddress,
                blockNr = _blockNr,
                dataType = _dataType,
                deadband = _deadband,
                id = _id,
                logFreq = _logFreq,
                LastLogTime = DateTime.MinValue,
                logType = _logType,
                name = _name,
                nrOfElements = _nrOfElements,
                plcName = _plcName,
                startByte = _startByte,
                timeOfDay = _timeOfDay,
                varType = _varType,
                tagUnit = _tagUnit,
                eventId = _eventId,
                IsBooleanTrigger = _isBooleanTrigger,
                boolTrigger = _boolTrigger,
                analogTrigger = _analogTrigger,
                analogValue = _analogValue,
            };
            btnNew.BackColor = greenColor;
            btnNew.Image = Properties.Resources.add_new_64px_Gray;
            timeOut.Start();
        }

        private void UpdateTag()
        {

            var temp = txtDeadband.Text.Replace(',', '.');

            var connectionString = PlcConfig.ConnectionString();
            var query = $"UPDATE {SqlSettings.Default.Databas}.dbo.tagConfig ";
            query += $"SET active='{checkActive.Checked}',name='{txtName.Text}',logType='{comboLogType.SelectedItem}',timeOfDay='{txtTimeOfDay.Text}'";
            query += $",deadband={temp},plcName='{PlcName}',varType='{comboVarType.SelectedItem}',blockNr={int.Parse(txtBlockNr.Text)}" +
                $",dataType='{comboDataType.SelectedItem}',startByte={int.Parse(txtStartByte.Text)},nrOfElements={int.Parse(txtNrOfElements.Text)}" +
                $",bitAddress={byte.Parse(txtBitAddress.Text)},logFreq='{comboLogFreq.SelectedItem}',";
            query += $"tagUnit='{txtUnit.Text}',eventId={tag.eventId},isBooleanTrigger='{tag.IsBooleanTrigger}'" +
                $",boolTrigger='{tag.boolTrigger}',analogTrigger='{tag.analogTrigger}',analogValue={tag.analogValue} " +
                $" WHERE id = {tag.id}";

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
                btnSave.BackColor = blueColor;
                btnSave.Image = Properties.Resources.available_updates_64px_gray;
                timeOut.Start();
            }
            catch (SqlException ex)
            {
                Apps.Logger.Log(string.Empty, Severity.Error, ex);
            }
        }

        private void DeleteTag()
        {
            string tagNamn;
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
                Apps.Logger.Log(string.Empty, Severity.Error, ex);
            }
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
            tbl.Columns.Add("tagUnit", typeof(string));
            tbl.Columns.Add("eventId", typeof(int));
            tbl.Columns.Add("isBooleanTrigger", typeof(bool));
            tbl.Columns.Add("boolTrigger", typeof(string));
            tbl.Columns.Add("analogTrigger", typeof(string));
            tbl.Columns.Add("analogValue", typeof(float));


            tbl.Rows.Add(checkActive.Checked, txtName.Text, comboLogType.SelectedItem.ToString(),
                TimeSpan.Parse(txtTimeOfDay.Text), float.Parse(txtDeadband.Text),
                PlcName, comboVarType.SelectedItem.ToString(), int.Parse(txtBlockNr.Text),
                comboDataType.SelectedItem.ToString(), int.Parse(txtStartByte.Text),
                int.Parse(txtNrOfElements.Text), byte.Parse(txtBitAddress.Text),
                comboLogFreq.SelectedItem.ToString(), tag.eventId, tag.IsBooleanTrigger, tag.boolTrigger, tag.analogTrigger, tag.analogValue);
            return tbl;

        }

        private void comboLogType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
            {
                startup = false;
                return;
            }
            if (comboLogType.SelectedItem.ToString() == "Event")
            {
                if (!eventFormOpen)
                {
                    eventFormOpen = true;
                    eventForm = null;
                    eventForm = new EventForm(tag, PlcName);
                    eventForm.FormClosing += EventForm_FormClosing;
                    eventForm.SaveEvent += EventForm_SaveEvent;
                    eventForm.Show();
                }
                else
                {
                    eventForm.BringToFront();
                }
            }
        }

        private void EventForm_SaveEvent(object sender, SaveEventArgs e)
        {
            tag.eventId = e.tag.eventId;
            tag.IsBooleanTrigger = e.tag.IsBooleanTrigger;
            tag.boolTrigger = e.tag.boolTrigger;
            tag.analogTrigger = e.tag.analogTrigger;
            tag.analogValue = e.tag.analogValue;
        }

        private void EventForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            eventFormOpen = false;
        }

        private void AddTag_FormClosing(object sender, FormClosingEventArgs e)
        {
            TagsToLog.FetchValuesFromSql();
        }


    }
}
