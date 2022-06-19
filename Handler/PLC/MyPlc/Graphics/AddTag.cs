using S7.Net;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OptimaValue.Config;

namespace OptimaValue.Handler.PLC.MyPlc.Graphics
{
    public partial class AddPlcFromFile : Form
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
        private Color redColor = Color.FromArgb(201, 74, 74);

        private bool CheckIfExists => DatabaseSql.TagExist(tag.Id);

        private bool CheckForDuplicateNames => DatabaseSql.CheckForDuplicateTagNames(tagName: paraName.ParameterValue, plcName: PlcName);

        protected virtual void OnTagChanged(EventArgs e)
        {
            TagChanged?.Invoke(this, e);
        }
        public AddPlcFromFile(string plcName, ExtendedPlc myPlc, TagDefinitions taggen = null)
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
                paraLogTime.ParameterValue = "12:00";
                paraDeadband.ParameterValue = 0.ToString();
                paraBitAddress.ParameterValue = 0.ToString();
                paraFreq.comboBoxen.SelectedItem = "_1s";
                paraNrOfValues.ParameterValue = 1.ToString();
                paraFreq.comboBoxen.Text = "_1s";
                paraDataType.comboBoxen.Text = "DataBlock";
                paraDataType.comboBoxen.SelectedItem = "DataBlock";
                paraLogType.comboBoxen.Text = "Cyclic";
                paraLogType.comboBoxen.SelectedItem = "Cyclic";
                paraScaleMin.ParameterValue = 0.ToString();
                paraScaleMax.ParameterValue = 0.ToString();
                paraScaleOffset.ParameterValue = 0.ToString();
            }
            timeOut.Interval = 300;
            timeOut.Tick += TimeOut_Tick;
            paraLogType.comboBoxen.SelectedIndexChanged += comboLogType_SelectedIndexChanged;
            paraVarType.comboBoxen.SelectedIndexChanged += comboVarType_SelectedIndexChanged;
            comboLogType_SelectedIndexChanged(this, EventArgs.Empty);
            comboVarType_SelectedIndexChanged(this, EventArgs.Empty);
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
            checkActive.Checked = tag.Active;
            paraName.ParameterValue = tag.Name;
            paraDescription.ParameterValue = tag.Description;
            paraLogType.comboBoxen.Text = tag.LogType.ToString();
            paraLogType.comboBoxen.SelectedItem = tag.LogType;

            paraLogTime.ParameterValue = tag.TimeOfDay.ToString();
            paraDeadband.ParameterValue = tag.Deadband.ToString();
            paraVarType.comboBoxen.Text = tag.VarType.ToString();
            paraVarType.comboBoxen.SelectedItem = tag.VarType;

            paraBlockNr.ParameterValue = tag.BlockNr.ToString();
            paraDataType.comboBoxen.Text = tag.DataType.ToString();
            paraDataType.comboBoxen.SelectedItem = tag.DataType;

            paraStartAddress.ParameterValue = tag.StartByte.ToString();
            paraNrOfValues.ParameterValue = tag.NrOfElements.ToString();
            paraBitAddress.ParameterValue = tag.BitAddress.ToString();
            paraFreq.comboBoxen.Text = tag.LogFreq.ToString();
            paraFreq.comboBoxen.SelectedItem = tag.LogFreq;

            paraUnit.ParameterValue = tag.TagUnit;

            paraScaleMin.ParameterValue = tag.scaleMin.ToString();
            paraScaleMax.ParameterValue = tag.scaleMax.ToString();
            paraScaleOffset.ParameterValue = tag.scaleOffset.ToString();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        #region Validation
        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            errorProvider.SetIconAlignment(paraName, ErrorIconAlignment.TopRight);
            if (string.IsNullOrEmpty(paraName.ParameterValue))
                errorProvider.SetError(paraName, "Fyll i ett namn");
            else if (paraName.Text.Length > 50)
                errorProvider.SetError(paraName, "Max 50 tecken");
            else
                errorProvider.SetError(paraName, "");
        }

        private void txtUnit_Validating(object sender, CancelEventArgs e)
        {
            errorProvider.SetIconAlignment(paraUnit, ErrorIconAlignment.BottomLeft);

            if (paraUnit.ParameterValue.Length > 30)
                errorProvider.SetError(paraUnit, "Max 30 tecken");
            else
                errorProvider.SetError(paraUnit, "");
        }

        private void txtTimeOfDay_Validating(object sender, CancelEventArgs e)
        {
            errorProvider.SetIconAlignment(paraLogTime, ErrorIconAlignment.MiddleLeft);

            if (string.IsNullOrEmpty(paraLogTime.ParameterValue))
                errorProvider.SetError(paraLogTime, "Ange en tid på dagen");
            else if (!TimeSpan.TryParse(paraLogTime.ParameterValue, out TimeSpan _))
                errorProvider.SetError(paraLogTime, "Inte ett klockslag");
            else
                errorProvider.SetError(paraLogTime, "");
        }

        private void txtDeadband_TextChanged(object sender, EventArgs e)
        {
            errorProvider.SetIconAlignment(paraDeadband, ErrorIconAlignment.MiddleLeft);

            if (string.IsNullOrEmpty(paraDeadband.ParameterValue))
                errorProvider.SetError(paraDeadband, "Ange ett numeriskt värde");
            else if (!float.TryParse(paraDeadband.ParameterValue, out float _))
                errorProvider.SetError(paraDeadband, "Inte ett numeriskt positivt värde");
            else
                errorProvider.SetError(paraDeadband, "");
        }

        private void txtBlockNr_Validating(object sender, CancelEventArgs e)
        {
            errorProvider.SetIconAlignment(paraBlockNr, ErrorIconAlignment.MiddleLeft);

            if (string.IsNullOrEmpty(paraBlockNr.ParameterValue))
                errorProvider.SetError(paraBlockNr, "Ange ett numeriskt värde");
            else if (!uint.TryParse(paraBlockNr.ParameterValue, out uint _))
                errorProvider.SetError(paraBlockNr, "Inte ett numeriskt positivt värde");
            else
                errorProvider.SetError(paraBlockNr, "");
        }
        private void txtScaleMin_Validating(object sender, CancelEventArgs e)
        {
            errorProvider.SetIconAlignment(paraScaleMin, ErrorIconAlignment.MiddleLeft);

            if (string.IsNullOrEmpty(paraScaleMin.ParameterValue))
                errorProvider.SetError(paraScaleMin, "Ange ett numeriskt värde");
            else if (!uint.TryParse(paraScaleMin.ParameterValue, out uint _))
                errorProvider.SetError(paraScaleMin, "Inte ett numeriskt positivt värde");
            else
                errorProvider.SetError(paraScaleMin, "");
        }
        private void txtScaleMax_Validating(object sender, CancelEventArgs e)
        {
            errorProvider.SetIconAlignment(paraScaleMax, ErrorIconAlignment.MiddleLeft);

            if (string.IsNullOrEmpty(paraScaleMax.ParameterValue))
                errorProvider.SetError(paraScaleMax, "Ange ett numeriskt värde");
            else if (!uint.TryParse(paraScaleMax.ParameterValue, out uint _))
                errorProvider.SetError(paraScaleMax, "Inte ett numeriskt positivt värde");
            else
                errorProvider.SetError(paraScaleMax, "");
        }
        private void txtScaleOffset_Validating(object sender, CancelEventArgs e)
        {
            errorProvider.SetIconAlignment(paraScaleOffset, ErrorIconAlignment.MiddleLeft);

            if (string.IsNullOrEmpty(paraScaleOffset.ParameterValue))
                errorProvider.SetError(paraScaleOffset, "Ange ett numeriskt värde");
            else if (!uint.TryParse(paraScaleOffset.ParameterValue, out uint _))
                errorProvider.SetError(paraScaleOffset, "Inte ett numeriskt positivt värde");
            else
                errorProvider.SetError(paraScaleOffset, "");
        }

        private void txtStartByte_Validating(object sender, CancelEventArgs e)
        {
            errorProvider.SetIconAlignment(paraStartAddress, ErrorIconAlignment.MiddleLeft);

            if (string.IsNullOrEmpty(paraStartAddress.ParameterValue))
                errorProvider.SetError(paraStartAddress, "Ange ett numeriskt värde");
            else if (!uint.TryParse(paraStartAddress.ParameterValue, out uint _))
                errorProvider.SetError(paraStartAddress, "Inte ett numeriskt positivt värde");
            else
                errorProvider.SetError(paraStartAddress, "");
        }

        private void txtNrOfElements_Validating(object sender, CancelEventArgs e)
        {
            errorProvider.SetIconAlignment(paraNrOfValues, ErrorIconAlignment.MiddleLeft);

            if (string.IsNullOrEmpty(paraNrOfValues.ParameterValue))
                errorProvider.SetError(paraNrOfValues, "Ange ett numeriskt värde");
            else if (!uint.TryParse(paraNrOfValues.ParameterValue, out uint _))
                errorProvider.SetError(paraNrOfValues, "Inte ett numeriskt positivt värde");
            else
                errorProvider.SetError(paraNrOfValues, "");
        }

        private void txtBitAddress_Validating(object sender, CancelEventArgs e)
        {
            errorProvider.SetIconAlignment(paraBitAddress, ErrorIconAlignment.MiddleLeft);

            if (string.IsNullOrEmpty(paraBitAddress.ParameterValue))
                errorProvider.SetError(paraBitAddress, "Ange ett numeriskt värde");
            else if (!byte.TryParse(paraBitAddress.ParameterValue, out byte resultByte))
                errorProvider.SetError(paraBitAddress, "Inte ett numeriskt positivt värde");
            else if (resultByte > 16)
                errorProvider.SetError(paraBitAddress, "För stort värde");
            else
                errorProvider.SetError(paraBitAddress, "");
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
            var okUnit = false;
            var tempString = string.Empty;
            var okScaleMin = false;
            var okScaleMax = false;
            var okScaleOffset = false;

            if (int.TryParse(paraScaleMin.ParameterValue, out int _))
                okScaleMin = true;

            if (int.TryParse(paraScaleMax.ParameterValue, out int _))
                okScaleMax = true;

            if (int.TryParse(paraScaleOffset.ParameterValue, out int _))
                okScaleOffset = true;

            if (paraUnit.ParameterValue.Length <= 30)
                okUnit = true;

            if (!string.IsNullOrEmpty(paraName.ParameterValue) && paraName.ParameterValue.Length <= 50)
                okName = true;

            if (!string.IsNullOrEmpty(paraLogType.comboBoxen.SelectedItem.ToString()))
                okLogType = true;

            if (TimeSpan.TryParse(paraLogTime.ParameterValue, out TimeSpan _))
                okTimeOfDay = true;

            if (float.TryParse(paraDeadband.ParameterValue, out float _))
                okDeadBand = true;

            if (paraVarType.comboBoxen.SelectedItem != null)
            {
                if (!string.IsNullOrEmpty(paraVarType.comboBoxen.SelectedItem.ToString()))
                    okVarType = true;
            }

            if (ushort.TryParse(paraBlockNr.ParameterValue, out ushort _))
                okBlockNr = true;

            if ((paraDataType.comboBoxen.SelectedItem != null))
            {
                if (!string.IsNullOrEmpty(paraDataType.comboBoxen.SelectedItem.ToString()))
                    okDataType = true;
            }

            if (ushort.TryParse(paraStartAddress.ParameterValue, out ushort _))
                okStartByte = true;

            if (ushort.TryParse(paraNrOfValues.ParameterValue, out ushort _))
                okNrOfElements = true;

            if (ushort.TryParse(paraBlockNr.ParameterValue, out ushort _))
                okBlockNr = true;

            if (byte.TryParse(paraBitAddress.ParameterValue, out byte _))
                okBitAddress = true;

            if (!string.IsNullOrEmpty(paraFreq.comboBoxen.SelectedItem.ToString()))
                okLogFreq = true;


            if (okName && okTimeOfDay && okDeadBand && okVarType && okBlockNr
                && okDataType && okStartByte && okNrOfElements && okBitAddress &&
                okLogFreq && okLogType && okUnit && okScaleMin && okScaleMax && okScaleOffset)
            {
                return true;
            }

            return false;

        }

        #endregion

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (tag == null)
            {
                btnSave.BackColor = redColor;
                timeOut.Start();
                return;
            }
            if (!paraName.ParameterValue.Equals(tag.Name))
                if (CheckForDuplicateNames)
                    return;

            if (ValidateAll())
            {
                if (tag.EventId == 0)
                {
                    tag.EventId = 0;
                    tag.IsBooleanTrigger = false;
                    tag.BoolTrigger = BooleanTrigger.OnTrue;
                    tag.AnalogTrigger = AnalogTrigger.Equal;
                    tag.AnalogValue = 0;
                }
                DataTable tbl = CreateTable();
                AddTagToSql(tbl);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateAll())
            {
                btnNew.BackColor = redColor;
                timeOut.Start();
                return;
            }
            if (!CheckForDuplicateNames)
                AddNewTag();
        }

        private void AddTagToSql(DataTable dataTable)
        {
            if (CheckIfExists)
                UpdateTag();
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
                tagEventId = tag.EventId;
                isBoolTrigger = tag.IsBooleanTrigger;
                analogTrigger = tag.AnalogTrigger.ToString();
                boolTrigger = tag.BoolTrigger.ToString();
                analogValue = tag.AnalogValue;

            }

            var deadband = paraDeadband.ParameterValue.Replace(",", ".");

            var query = $"INSERT INTO {Settings.Databas}.dbo.tagConfig ";
            query += $"(active,name,description,logType,timeOfDay,deadband,plcName,varType,blockNr,dataType,startByte,nrOfElements,bitAddress,logFreq,";
            query += $"tagUnit,eventId,isBooleanTrigger,boolTrigger,analogTrigger,analogValue,scaleMin,scaleMax,scaleOffset) ";
            query += $"VALUES ('{checkActive.Checked}','{paraName.ParameterValue}','{paraDescription.ParameterValue}','{paraLogType.comboBoxen.SelectedItem}','{paraLogTime.ParameterValue}',";
            query += $"{deadband},'{PlcName}','{paraVarType.comboBoxen.SelectedItem}',{int.Parse(paraBlockNr.ParameterValue)}, ";
            query += $"'{paraDataType.comboBoxen.SelectedItem}',{int.Parse(paraStartAddress.ParameterValue)},{int.Parse(paraNrOfValues.ParameterValue)},";
            query += $"{byte.Parse(paraBitAddress.ParameterValue)},'{paraFreq.comboBoxen.SelectedItem}','{paraUnit.ParameterValue}',{tagEventId},'{isBoolTrigger}','";
            query += $"{boolTrigger}','{analogTrigger}',{analogValue},{paraScaleMin.ParameterValue},{paraScaleMax.ParameterValue},{paraScaleOffset.ParameterValue})";

            DatabaseSql.AddTag(query);


            var tbl = new DataTable();
            tbl = DatabaseSql.GetLastTag();



            var _active = (tbl.AsEnumerable().ElementAt(0).Field<bool>("active"));
            var _bitAddress = (tbl.AsEnumerable().ElementAt(0).Field<byte>("bitAddress"));
            var _blockNr = (tbl.AsEnumerable().ElementAt(0).Field<int>("blockNr"));
            var _dataType = (S7.Net.DataType)Enum.Parse(typeof(S7.Net.DataType), (tbl.AsEnumerable().ElementAt(0).Field<string>("dataType")));
            var _deadband = (float)(tbl.AsEnumerable().ElementAt(0).Field<double>("deadband"));
            var _id = (tbl.AsEnumerable().ElementAt(0).Field<int>("id"));
            var _logFreq = (LogFrequency)Enum.Parse(typeof(LogFrequency), (tbl.AsEnumerable().ElementAt(0).Field<string>("logFreq")));
            var _logType = (LogType)Enum.Parse(typeof(LogType), (tbl.AsEnumerable().ElementAt(0).Field<string>("logType")));
            var _name = (tbl.AsEnumerable().ElementAt(0).Field<string>("name"));
            var _description = (tbl.AsEnumerable().ElementAt(0).Field<string>("description"));
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
            var _scaleMin = (tbl.AsEnumerable().ElementAt(0).Field<int>("scaleMin"));
            var _scaleMax = (tbl.AsEnumerable().ElementAt(0).Field<int>("scaleMax"));
            var _scaleOffset = (tbl.AsEnumerable().ElementAt(0).Field<int>("scaleOffset"));



            tag = new TagDefinitions()
            {
                Active = _active,
                BitAddress = _bitAddress,
                BlockNr = _blockNr,
                DataType = _dataType,
                Deadband = _deadband,
                Id = _id,
                LogFreq = _logFreq,
                LastLogTime = DateTime.MinValue,
                LogType = _logType,
                NrOfElements = _nrOfElements,
                StartByte = _startByte,
                TimeOfDay = _timeOfDay,
                VarType = _varType,
                TagUnit = _tagUnit,
                EventId = _eventId,
                IsBooleanTrigger = _isBooleanTrigger,
                BoolTrigger = _boolTrigger,
                AnalogTrigger = _analogTrigger,
                AnalogValue = _analogValue,
                scaleMin = _scaleMin,
                scaleMax = _scaleMax,
                scaleOffset = _scaleOffset,
            };
            btnNew.BackColor = greenColor;
            btnNew.Image = Properties.Resources.add_new_64px_Gray;
            timeOut.Start();
        }

        private void UpdateTag()
        {

            var temp = paraDeadband.ParameterValue.Replace(',', '.');

            var query = $"UPDATE {Settings.Databas}.dbo.tagConfig ";
            query += $"SET active='{checkActive.Checked}',name='{paraName.ParameterValue}',description='{paraDescription.ParameterValue}',logType='{paraLogType.comboBoxen.SelectedItem}',timeOfDay='{paraLogTime.ParameterValue}'";
            query += $",deadband={temp},plcName='{PlcName}',varType='{paraVarType.comboBoxen.SelectedItem}',blockNr={int.Parse(paraBlockNr.ParameterValue)}" +
                $",dataType='{paraDataType.comboBoxen.SelectedItem}',startByte={int.Parse(paraStartAddress.ParameterValue)},nrOfElements={int.Parse(paraNrOfValues.ParameterValue)}" +
                $",bitAddress={byte.Parse(paraBitAddress.ParameterValue)},logFreq='{paraFreq.comboBoxen.SelectedItem}',";
            query += $"tagUnit='{paraUnit.ParameterValue}',eventId={tag.EventId},isBooleanTrigger='{tag.IsBooleanTrigger}'" +
                $",boolTrigger='{tag.BoolTrigger}',analogTrigger='{tag.AnalogTrigger}',analogValue={tag.AnalogValue}, " +
                $"scaleMin={paraScaleMin.ParameterValue},scaleMax={paraScaleMax.ParameterValue},scaleOffset={paraScaleOffset.ParameterValue}" +
                $" WHERE id = {tag.Id}";

            bool success = DatabaseSql.UpdateTag(query);

            if (success)
            {
                btnSave.BackColor = blueColor;
                btnSave.Image = Properties.Resources.available_updates_64px_gray;
                timeOut.Start();
            }
        }

        private DataTable CreateTable()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("active", typeof(bool));
            tbl.Columns.Add("name", typeof(string));
            tbl.Columns.Add("description", typeof(string));
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
            tbl.Columns.Add("scaleMin", typeof(int));
            tbl.Columns.Add("scaleMax", typeof(int));
            tbl.Columns.Add("scaleOffset", typeof(int));


            tbl.Rows.Add(checkActive.Checked, paraName.ParameterValue, paraDescription.ParameterValue, paraLogType.comboBoxen.SelectedItem.ToString(),
                TimeSpan.Parse(paraLogTime.ParameterValue), float.Parse(paraDeadband.ParameterValue),
                PlcName, paraVarType.comboBoxen.SelectedItem.ToString(), int.Parse(paraBlockNr.ParameterValue),
                paraDataType.comboBoxen.SelectedItem.ToString(), int.Parse(paraStartAddress.ParameterValue),
                int.Parse(paraNrOfValues.ParameterValue), byte.Parse(paraBitAddress.ParameterValue),
                paraFreq.comboBoxen.SelectedItem.ToString(), tag.EventId, tag.IsBooleanTrigger, tag.BoolTrigger, tag.AnalogTrigger, tag.AnalogValue, int.Parse(paraScaleMin.ParameterValue),
                int.Parse(paraScaleMax.ParameterValue), int.Parse(paraScaleOffset.ParameterValue));
            return tbl;

        }

        private void comboLogType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (paraLogType.comboBoxen.SelectedItem.ToString())
            {
                case "Cyclic":
                    paraLogTime.Visible = false;

                    paraDeadband.Visible = false;
                    break;
                case "Delta":
                    paraLogTime.Visible = false;

                    paraDeadband.Visible = true;
                    break;
                case "TimeOfDay":
                    paraLogTime.Visible = true;

                    paraDeadband.Visible = false;
                    break;
                default:
                    break;
            }


            if (startup)
            {
                startup = false;
                return;
            }
            if (paraLogType.comboBoxen.SelectedItem.ToString() == "Event")
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
        private void comboVarType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (paraVarType.comboBoxen.SelectedItem == null)
                return;
            switch (paraVarType.comboBoxen.SelectedItem.ToString())
            {
                case "Bit":
                    paraScaleMax.Visible = false;

                    paraScaleMin.Visible = false;

                    paraScaleOffset.Visible = false;

                    paraBitAddress.Visible = true;
                    break;
                case "String":
                    paraScaleMax.Visible = false;

                    paraScaleMin.Visible = false;

                    paraScaleOffset.Visible = false;

                    paraBitAddress.Visible = false;
                    break;
                case "StringEx":
                    paraScaleMax.Visible = false;

                    paraScaleMin.Visible = false;

                    paraScaleOffset.Visible = false;

                    paraBitAddress.Visible = false;
                    break;
                default:
                    paraScaleMax.Visible = true;

                    paraScaleMin.Visible = true;

                    paraScaleOffset.Visible = true;

                    paraBitAddress.Visible = true;
                    break;
            }
        }


        private void EventForm_SaveEvent(object sender, SaveEventArgs e)
        {
            if (tag == null)
                tag = new TagDefinitions();
            tag.EventId = e.tag.EventId;
            tag.IsBooleanTrigger = e.tag.IsBooleanTrigger;
            tag.BoolTrigger = e.tag.BoolTrigger;
            tag.AnalogTrigger = e.tag.AnalogTrigger;
            tag.AnalogValue = e.tag.AnalogValue;
        }

        private void EventForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            eventFormOpen = false;
        }

        private void AddTag_FormClosing(object sender, FormClosingEventArgs e)
        {
            TagsToLog.GetAllTagsFromSql();
        }

        private void txtDeadband_TextChanged(object sender, CancelEventArgs e)
        {

        }
    }
}
