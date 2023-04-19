using S7.Net;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OptimaValue.Config;
using OpcUa.UI;
using System.Threading.Tasks;
using OpcUa;
using System.Windows.Controls;
using OpcUaHm.Common;
using System.Collections.Generic;
using OpcUa.DA;
using OpcUa.UI.Controls;
using OptimaValue.Handler.PLC.MyPlc.Graphics.Parameters;
using System.Net;
using System.Windows.Documents;

namespace OptimaValue.Handler.PLC.MyPlc.Graphics
{
    public partial class AddPlcFromFile : Form
    {
        private ExtendedPlc MyPlc;
        private readonly string PlcName = string.Empty;
        private TagDefinitions tag;
        public event EventHandler TagChanged;
        private readonly ExtendedPlc opcPlc;
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
                paraRawMin.ParameterValue = 0.ToString();
                paraRawMax.ParameterValue = 0.ToString();
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
            SetVisibility();
        }

        bool opcFormOpen = false;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (MyPlc.isOpc)
            {
                btnOpc.Visible = true;
                btnOpc.Click += ((sender, args) =>
                {
                    if (!opcFormOpen)
                    {
                        PopulateOpcTags();
                        opcFormOpen = true;
                    }
                });

            }
        }

        private class ComboTag
        {
            public string Name { get; set; }
            public string FullName { get; set; }
        }
        private List<ComboTag> Tags = new();

        FormBrowseOpc opcForm;
        private async void PopulateOpcTags()
        {
            try
            {
                await MyPlc.Plc.ConnectAsync();
                if (MyPlc.IsConnected)
                {
                    if (MyPlc.CpuType == CpuType.OpcUa)
                    {
                        if (MyPlc.Plc is OpcPlc opc)
                        {
                            if (opc.Client is UaClient uaClient)
                            {
                                var opcControl = new BrowseOpcTreeControl(uaClient.MySession, uaClient);
                                opcForm = new FormBrowseOpc(opcControl);
                                opcForm.FormClosing += ((sender, args) =>
                                {
                                    opcFormOpen = false;
                                });
                                opcForm.Show();
                                opcForm.FormClosing += (s, e) =>
                                {
                                    MyPlc.Plc.Dispose();
                                };
                                opcForm.OnAddOpcTag += Form_OnAddOpcTag;
                                opcForm.OnDeleteOpcTag += Form_OnDeleteOpcTag;
                            }

                        }
                    }
                    else if (MyPlc.CpuType == CpuType.OpcDa)
                    {
                        if (MyPlc.Plc is OpcPlc opc)
                        {
                            if (opc.Client is DaClient daClient)
                            {
                                // TODO: Add DA client
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void Form_OnDeleteOpcTag(string obj)
        {

        }

        private void Form_OnAddOpcTag(string obj)
        {
            paraName.ParameterValue = obj;

            var opcPlc = MyPlc.Plc as OpcPlc;
            try
            {
                if (!opcPlc.IsConnected)
                    opcPlc.Connect();
                //var tag = Tags.Where(x => x.Name == obj).FirstOrDefault();
                var subNodes = opcPlc.ExploreOpc(obj, false, true);

                try
                {
                    var descriptionNode = subNodes.Where(x => x.Name.Contains("escription")).FirstOrDefault();
                    var description = opcPlc.Read(descriptionNode.Tag);
                    paraDescription.ParameterValue = description.Value.ToString();
                }
                catch (Exception) { }


                var unitsNode = subNodes.Where(x => x.Name.Contains("Units")).FirstOrDefault();
                var rawLowNode = subNodes.Where(x => x.Name.Contains("RawLow")).FirstOrDefault();
                var rawHiNode = subNodes.Where(x => x.Name.Contains("RawHigh")).FirstOrDefault();
                var scaleLowNode = subNodes.Where(x => x.Name.Contains("ScaledLow")).FirstOrDefault();
                var scaleHiNode = subNodes.Where(x => x.Name.Contains("ScaledHigh")).FirstOrDefault();

                var units = opcPlc.Read(unitsNode.Tag).Value.ToString();
                var rawLo = opcPlc.Read(rawLowNode.Tag);
                var rawHi = opcPlc.Read(rawHiNode.Tag);
                var scaledLo = opcPlc.Read(scaleLowNode.Tag);
                var scaleHi = opcPlc.Read(scaleHiNode.Tag);

                paraRawMin.ParameterValue = rawLo.Value.ToString();
                paraRawMax.ParameterValue = rawHi.Value.ToString();

                paraScaleMin.ParameterValue = scaledLo.Value.ToString();
                paraScaleMax.ParameterValue = scaleHi.Value.ToString();

                paraUnit.ParameterValue = units;
            }
            catch (Exception ex)
            {

            }
        }

        private void SetVisibility()
        {
            if (MyPlc.isOpc)
            {
                paraBitAddress.Visible = false;
                paraDataType.Visible = false;
                paraVarType.Visible = false;
                paraStartAddress.Visible = false;
                paraBlockNr.Visible = false;
            }
            if (tag is not null && tag.LogType == LogType.Calculated)
            {
                paraUnit.Visible = true;
                paraFreq.Visible = true;
                paraLogType.Visible = true;
                paraLogTime.Visible = false;
                paraVarType.Visible = false;
                paraRawMin.Visible = false;
                paraRawMax.Visible = false;
                paraScaleMax.Visible = false;
                paraScaleMin.Visible = false;
                paraScaleOffset.Visible = false;
                paraBitAddress.Visible = false;
                paraDeadband.Visible = false;
                paraBlockNr.Visible = false;
                paraStartAddress.Visible = false;
                paraNrOfValues.Visible = false;
                paraDataType.Visible = false;
                btnEkvation.Visible = true;
            }
            else if (tag is not null && tag.LogType == LogType.RateOfChange)
            {
                paraUnit.Visible = true;
                paraFreq.Visible = true;
                paraLogType.Visible = true;
                paraLogTime.Visible = false;
                paraVarType.Visible = true;
                paraRawMin.Visible = true;
                paraRawMax.Visible = true;
                paraScaleMax.Visible = true;
                paraScaleMin.Visible = true;
                paraScaleOffset.Visible = true;
                paraBitAddress.Visible = true;
                paraDeadband.Visible = true;
                paraBlockNr.Visible = true;
                paraStartAddress.Visible = true;
                paraNrOfValues.Visible = false;
                paraDataType.Visible = true;
                btnEkvation.Visible = false;
            }
            else if (tag is not null && tag.LogType == LogType.Adaptive)
            {
                paraUnit.Visible = true;
                paraFreq.Visible = true;
                paraLogType.Visible = true;
                paraVarType.Visible = true;
                paraRawMin.Visible = true;
                paraRawMax.Visible = true;
                paraScaleMax.Visible = true;
                paraScaleMin.Visible = true;
                paraScaleOffset.Visible = true;
                paraBitAddress.Visible = true;
                paraDeadband.Visible = true;
                paraBlockNr.Visible = true;
                paraStartAddress.Visible = true;
                paraDataType.Visible = true;
                paraLogTime.Visible = false;
                paraNrOfValues.Visible = false;
                btnEkvation.Visible = false;
            }
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

            paraRawMin.ParameterValue = tag.rawMin.ToString();
            paraRawMax.ParameterValue = tag.rawMax.ToString();

            paraScaleMin.ParameterValue = tag.scaleMin.ToString();
            paraScaleMax.ParameterValue = tag.scaleMax.ToString();
            paraScaleOffset.ParameterValue = tag.scaleOffset.ToString();
        }

        #region Validation

        private void ValidateControl(object sender, CancelEventArgs e)
        {
            if (sender is parameterTextControl paraTextBox)
            {
                string errorMessage = "";
                errorProvider.SetIconAlignment(paraTextBox, ErrorIconAlignment.MiddleLeft);

                switch (paraTextBox.Name)
                {
                    case "paraName":
                        errorProvider.SetIconAlignment(paraName, ErrorIconAlignment.TopRight);
                        errorMessage = ValidateName(paraTextBox);
                        break;
                    case "paraUnit":
                        errorProvider.SetIconAlignment(paraUnit, ErrorIconAlignment.BottomLeft);
                        errorMessage = ValidateUnit(paraTextBox);
                        break;
                    case "paraLogTime":
                        errorMessage = ValidateTimeOfDay(paraTextBox);
                        break;
                    case "paraDeadband":
                        errorProvider.SetIconAlignment(paraDeadband, ErrorIconAlignment.MiddleLeft);
                        errorMessage = ValidateFloatValue(paraTextBox);
                        break;
                    case "paraRawMin":
                        errorMessage = ValidateFloatValue(paraTextBox);
                        break;
                    case "paraRawMax":
                        errorMessage = ValidateFloatValue(paraTextBox);
                        break;
                    case "paraScaleMin":
                        errorMessage = ValidateFloatValue(paraTextBox);
                        break;
                    case "paraScaleMax":
                        errorMessage = ValidateFloatValue(paraTextBox);
                        break;
                    case "paraScaleOffset":
                        errorMessage = ValidateFloatValue(paraTextBox);
                        break;
                    case "paraBlockNr":
                        errorMessage = ValidateUIntValue(paraTextBox);
                        break;
                    case "paraStartAddress":
                        errorMessage = ValidateUIntValue(paraTextBox);
                        break;
                    case "paraNrOfValues":
                        errorMessage = ValidateUIntValue(paraTextBox);
                        break;
                    case "paraBitAddres":
                        errorMessage = ValidateBitAddress(paraTextBox);
                        break;
                }

                errorProvider.SetError(paraTextBox, errorMessage);
            }
        }

        private string ValidateName(parameterTextControl textBox)
        {
            if (string.IsNullOrEmpty(textBox.ParameterValue))
                return "Fyll i ett namn";
            else if (textBox.Text.Length > 50)
                return "Max 50 tecken";

            return "";
        }

        private string ValidateUnit(parameterTextControl textBox)
        {
            if (textBox.ParameterValue.Length > 30)
                return "Max 30 tecken";

            return "";
        }

        private string ValidateTimeOfDay(parameterTextControl textBox)
        {
            if (string.IsNullOrEmpty(textBox.ParameterValue))
                return "Ange en tid på dagen";
            else if (!TimeSpan.TryParse(textBox.ParameterValue, out TimeSpan _))
                return "Inte ett klockslag";

            return "";
        }

        private string ValidateFloatValue(parameterTextControl textBox)
        {
            if (string.IsNullOrEmpty(textBox.ParameterValue))
                return "Ange ett numeriskt värde";
            else if (!float.TryParse(textBox.ParameterValue, out float _))
                return "Inte ett numeriskt positivt värde";

            return "";
        }

        private string ValidateUIntValue(parameterTextControl textBox)
        {
            if (string.IsNullOrEmpty(textBox.ParameterValue))
                return "Ange ett numeriskt värde";
            else if (!uint.TryParse(textBox.ParameterValue, out uint _))
                return "Inte ett numeriskt positivt värde";

            return "";
        }

        private string ValidateBitAddress(parameterTextControl textBox)
        {
            if (string.IsNullOrEmpty(textBox.ParameterValue))
                return "Ange ett numeriskt värde";
            else if (!byte.TryParse(textBox.ParameterValue, out byte resultByte))
                return "Inte ett numeriskt positivt värde";
            else if (resultByte > 16)
                return "För stort värde";

            return "";
        }

        private bool ValidateAll()
        {
            var logType = paraLogType.comboBoxen.SelectedItem.ToString();

            bool okName = ValidateName(paraName) == "";
            bool okLogType = !string.IsNullOrEmpty(logType);
            bool okUnit = ValidateUnit(paraUnit) == "";
            bool okTimeOfDay = ValidateTimeOfDay(paraLogTime) == "";
            bool okDeadBand = ValidateFloatValue(paraDeadband) == "";
            bool okVarType = paraVarType.comboBoxen.SelectedItem != null && !string.IsNullOrEmpty(paraVarType.comboBoxen.SelectedItem.ToString());
            bool okDataType = paraDataType.comboBoxen.SelectedItem != null && !string.IsNullOrEmpty(paraDataType.comboBoxen.SelectedItem.ToString());
            bool okBlockNr = ValidateUIntValue(paraBlockNr) == "";
            bool okStartByte = ValidateUIntValue(paraStartAddress) == "";
            bool okNrOfElements = ValidateUIntValue(paraNrOfValues) == "";
            bool okBitAddress = ValidateBitAddress(paraBitAddress) == "";
            bool okLogFreq = !string.IsNullOrEmpty(paraFreq.comboBoxen.SelectedItem.ToString());

            bool okRawMin = ValidateFloatValue(paraRawMin) == "" || logType == "WriteWatchDogInt16";
            bool okRawMax = ValidateFloatValue(paraRawMax) == "" || logType == "WriteWatchDogInt16";
            bool okScaleMin = ValidateFloatValue(paraScaleMin) == "" || logType == "WriteWatchDogInt16";
            bool okScaleMax = ValidateFloatValue(paraScaleMax) == "" || logType == "WriteWatchDogInt16";
            bool okScaleOffset = ValidateFloatValue(paraScaleOffset) == "" || logType == "WriteWatchDogInt16";

            if (MyPlc.isOpc && okName) return true;

            return okName && okLogType && okUnit && okTimeOfDay && okDeadBand && okVarType && okDataType &&
                okBlockNr && okStartByte && okNrOfElements && okBitAddress && okLogFreq &&
                okRawMin && okRawMax && okScaleMin && okScaleMax && okScaleOffset;
        }

        #endregion

        private void AddTagToSql(DataTable dataTable)
        {
            if (CheckIfExists)
                UpdateTag();
        }

        private void AddNewTag()
        {
            string query = BuildQuery("INSERT");
            DatabaseSql.AddTag(query);

            var tbl = new DataTable();
            tbl = DatabaseSql.GetLastTag();

            tag = CreateTagFromDataTable(tbl);

            btnNew.BackColor = greenColor;
            btnNew.Image = Properties.Resources.add_new_64px_Gray;
            timeOut.Start();
        }

        private void UpdateTag()
        {
            string query = BuildQuery("UPDATE");
            bool success = DatabaseSql.UpdateTag(query);

            if (success)
            {
                btnSave.BackColor = blueColor;
                btnSave.Image = Properties.Resources.available_updates_64px_gray;
                timeOut.Start();
            }
        }

        private string BuildQuery(string action)
        {
            string query = string.Empty;

            if (action == "INSERT")
            {
                query = $"INSERT INTO {Settings.Databas}.dbo.tagConfig ";
                query += BuildQueryCommon();
            }
            else if (action == "UPDATE")
            {
                query = $"UPDATE {Settings.Databas}.dbo.tagConfig ";
                query += $"SET {BuildQueryCommon()} WHERE id = {tag.Id}";
            }

            return query;
        }

        private string BuildQueryCommon()
        {
            string deadband = paraDeadband.ParameterValue.Replace(",", ".");
            string varType = MyPlc.isOpc ? "Opc" : paraVarType.comboBoxen.SelectedItem.ToString();

            int blockNr = MyPlc.isOpc ? 0 : int.Parse(paraBlockNr.ParameterValue);
            int startAddress = MyPlc.isOpc ? 0 : int.Parse(paraStartAddress.ParameterValue);
            int nrOfValues = MyPlc.isOpc ? 0 : int.Parse(paraNrOfValues.ParameterValue);
            byte bitAddress = (byte)(MyPlc.isOpc ? 0 : byte.Parse(paraBitAddress.ParameterValue));

            string queryCommon = $"(active,name,description,logType,timeOfDay,deadband,plcName,varType,blockNr,dataType,startByte,nrOfElements,bitAddress,logFreq,";
            queryCommon += $"tagUnit,eventId,isBooleanTrigger,boolTrigger,analogTrigger,analogValue,scaleMin,scaleMax,scaleOffset,rawMin,rawMax,calculation) ";
            queryCommon += $"VALUES ('{checkActive.Checked}','{paraName.ParameterValue}','{paraDescription.ParameterValue}','{paraLogType.comboBoxen.SelectedItem}','{paraLogTime.ParameterValue}',";
            queryCommon += $"{deadband},'{PlcName}','{varType}',{blockNr}, ";
            queryCommon += $"'{paraDataType.comboBoxen.SelectedItem}',{startAddress},{nrOfValues},";
            queryCommon += $"{bitAddress},'{paraFreq.comboBoxen.SelectedItem}','{paraUnit.ParameterValue}',{tag.EventId},'{tag.IsBooleanTrigger}','";
            queryCommon += $"{tag.BoolTrigger}','{tag.AnalogTrigger}',{tag.AnalogValue},{paraScaleMin.ParameterValue},{paraScaleMax.ParameterValue},{paraScaleOffset.ParameterValue},{paraRawMin.ParameterValue},{paraRawMax.ParameterValue},'{tag.Calculation}')";

            return queryCommon;
        }

        private TagDefinitions CreateTagFromDataTable(DataTable tbl)
        {
            var tagDef = new TagDefinitions()
            {
                Active = tbl.AsEnumerable().ElementAt(0).Field<bool>("active"),
                BitAddress = tbl.AsEnumerable().ElementAt(0).Field<byte>("bitAddress"),
                BlockNr = tbl.AsEnumerable().ElementAt(0).Field<int>("blockNr"),
                DataType = (S7.Net.DataType)Enum.Parse(typeof(S7.Net.DataType), tbl.AsEnumerable().ElementAt(0).Field<string>("dataType")),
                Deadband = (float)tbl.AsEnumerable().ElementAt(0).Field<double>("deadband"),
                Id = tbl.AsEnumerable().ElementAt(0).Field<int>("id"),
                LogFreq = (LogFrequency)Enum.Parse(typeof(LogFrequency), tbl.AsEnumerable().ElementAt(0).Field<string>("logFreq")),
                LastLogTime = DateTime.MinValue,
                LogType = (LogType)Enum.Parse(typeof(LogType), tbl.AsEnumerable().ElementAt(0).Field<string>("logType")),
                NrOfElements = tbl.AsEnumerable().ElementAt(0).Field<int>("nrOfElements"),
                StartByte = tbl.AsEnumerable().ElementAt(0).Field<int>("startByte"),
                TimeOfDay = tbl.AsEnumerable().ElementAt(0).Field<TimeSpan>("timeOfDay"),
                VarType = (VarType)Enum.Parse(typeof(VarType), tbl.AsEnumerable().ElementAt(0).Field<string>("varType")),
                TagUnit = tbl.AsEnumerable().ElementAt(0).Field<string>("tagUnit"),
                EventId = tbl.AsEnumerable().ElementAt(0).Field<int>("eventId"),
                IsBooleanTrigger = tbl.AsEnumerable().ElementAt(0).Field<bool>("isBooleanTrigger"),
                BoolTrigger = (BooleanTrigger)Enum.Parse(typeof(BooleanTrigger), tbl.AsEnumerable().ElementAt(0).Field<string>("boolTrigger")),
                AnalogTrigger = (AnalogTrigger)Enum.Parse(typeof(AnalogTrigger), tbl.AsEnumerable().ElementAt(0).Field<string>("analogTrigger")),
                AnalogValue = (float)tbl.AsEnumerable().ElementAt(0).Field<double>("analogValue"),
                scaleMin = (float)tbl.AsEnumerable().ElementAt(0).Field<double>("scaleMin"),
                scaleMax = (float)tbl.AsEnumerable().ElementAt(0).Field<double>("scaleMax"),
                scaleOffset = (float)tbl.AsEnumerable().ElementAt(0).Field<double>("scaleOffset"),
                rawMin = (float)tbl.AsEnumerable().ElementAt(0).Field<double>("rawMin"),
                rawMax = (float)tbl.AsEnumerable().ElementAt(0).Field<double>("rawMax"),
                Calculation = tbl.AsEnumerable().ElementAt(0).Field<string>("calculation")
            };
            return tagDef;
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
            tbl.Columns.Add("rawMin", typeof(float));
            tbl.Columns.Add("rawMax", typeof(float));
            tbl.Columns.Add("scaleMin", typeof(float));
            tbl.Columns.Add("scaleMax", typeof(float));
            tbl.Columns.Add("scaleOffset", typeof(float));
            tbl.Columns.Add("calculation", typeof(string));

            if (!MyPlc.isOpc)
            {
                tbl.Rows.Add(
                    checkActive.Checked,
                    paraName.ParameterValue,
                    paraDescription.ParameterValue,
                    paraLogType.comboBoxen.SelectedItem.ToString(),
                    TimeSpan.Parse(paraLogTime.ParameterValue),
                    float.Parse(paraDeadband.ParameterValue),
                    PlcName,
                    paraVarType.comboBoxen.SelectedItem.ToString(),
                    int.Parse(paraBlockNr.ParameterValue),
                    paraDataType.comboBoxen.SelectedItem.ToString(),
                    int.Parse(paraStartAddress.ParameterValue),
                    int.Parse(paraNrOfValues.ParameterValue),
                    byte.Parse(paraBitAddress.ParameterValue),
                    paraFreq.comboBoxen.SelectedItem.ToString(),
                    paraUnit.ParameterValue,
                    tag.EventId,
                    tag.IsBooleanTrigger,
                    tag.BoolTrigger, tag.AnalogTrigger,
                    tag.AnalogValue,
                    float.Parse(paraRawMin.ParameterValue),
                    float.Parse(paraRawMax.ParameterValue),
                    float.Parse(paraScaleMin.ParameterValue),
                    float.Parse(paraScaleMax.ParameterValue),
                    float.Parse(paraScaleOffset.ParameterValue),
                    tag.Calculation ?? string.Empty);

                return tbl;
            }
            else
            {
                tbl.Rows.Add(checkActive.Checked, paraName.ParameterValue, paraDescription.ParameterValue, paraLogType.comboBoxen.SelectedItem.ToString(),
                TimeSpan.Parse(paraLogTime.ParameterValue), float.Parse(paraDeadband.ParameterValue),
                PlcName, "", 0, "", 0, 1, byte.Parse(paraBitAddress.ParameterValue),
                paraFreq.comboBoxen.SelectedItem.ToString(), tag.EventId, tag.IsBooleanTrigger, tag.BoolTrigger, tag.AnalogTrigger, tag.AnalogValue
                , float.Parse(paraRawMin.ParameterValue), float.Parse(paraRawMax.ParameterValue), float.Parse(paraScaleMin.ParameterValue),
                float.Parse(paraScaleMax.ParameterValue), float.Parse(paraScaleOffset.ParameterValue),
                tag.Calculation);

                return tbl;
            }
        }

        private void comboLogType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (paraLogType.comboBoxen.SelectedItem.ToString())
            {
                case "Cyclic":
                    paraLogTime.Visible = false;
                    paraDeadband.Visible = false;

                    paraScaleMax.Visible = true;
                    paraScaleMin.Visible = true;
                    paraScaleOffset.Visible = true;
                    paraUnit.Visible = true;
                    paraRawMin.Visible = true;
                    paraRawMax.Visible = true;
                    paraLogType.Visible = true;
                    paraBitAddress.Visible = true;
                    paraVarType.Visible = true;
                    btnEkvation.Visible = false;

                    break;
                case "Delta":
                    paraLogTime.Visible = false;
                    paraDeadband.Visible = true;

                    paraScaleMax.Visible = true;
                    paraScaleMin.Visible = true;
                    paraScaleOffset.Visible = true;
                    paraUnit.Visible = true;
                    paraRawMin.Visible = true;
                    paraRawMax.Visible = true;
                    paraLogType.Visible = true;
                    paraBitAddress.Visible = true;
                    paraVarType.Visible = true;
                    btnEkvation.Visible = false;

                    break;
                case "TimeOfDay":
                    paraLogTime.Visible = true;
                    paraDeadband.Visible = false;

                    paraScaleMax.Visible = true;
                    paraScaleMin.Visible = true;
                    paraScaleOffset.Visible = true;
                    paraUnit.Visible = true;
                    paraRawMin.Visible = true;
                    paraRawMax.Visible = true;
                    paraLogType.Visible = true;
                    paraBitAddress.Visible = true;
                    paraVarType.Visible = true;
                    btnEkvation.Visible = false;

                    break;
                case "WriteWatchDogInt16":
                    paraRawMin.Visible = false;
                    paraRawMax.Visible = false;
                    paraScaleMax.Visible = false;
                    paraScaleMin.Visible = false;
                    paraScaleOffset.Visible = false;
                    paraUnit.Visible = false;
                    paraBitAddress.Visible = false;
                    paraVarType.Visible = false;
                    btnEkvation.Visible = false;
                    break;
                case "Calculated":
                    // Set all fields to inivisible
                    paraUnit.Visible = true;
                    paraFreq.Visible = true;
                    paraLogType.Visible = true;
                    paraLogTime.Visible = false;
                    paraVarType.Visible = false;
                    paraRawMin.Visible = false;
                    paraRawMax.Visible = false;
                    paraScaleMax.Visible = false;
                    paraScaleMin.Visible = false;
                    paraScaleOffset.Visible = false;
                    paraBitAddress.Visible = false;
                    paraDeadband.Visible = false;
                    paraBlockNr.Visible = false;
                    paraStartAddress.Visible = false;
                    paraNrOfValues.Visible = false;
                    paraDataType.Visible = false;
                    btnEkvation.Visible = true;
                    break;
                case "RateOfChange":
                    paraUnit.Visible = true;
                    paraFreq.Visible = true;
                    paraLogType.Visible = true;
                    paraLogTime.Visible = false;
                    paraVarType.Visible = true;
                    paraRawMin.Visible = true;
                    paraRawMax.Visible = true;
                    paraScaleMax.Visible = true;
                    paraScaleMin.Visible = true;
                    paraScaleOffset.Visible = true;
                    paraBitAddress.Visible = true;
                    paraDeadband.Visible = true;
                    paraBlockNr.Visible = true;
                    paraStartAddress.Visible = true;
                    paraNrOfValues.Visible = false;
                    paraDataType.Visible = true;
                    btnEkvation.Visible = false;
                    break;
                case "Adaptive":
                    paraUnit.Visible = true;
                    paraFreq.Visible = true;
                    paraLogType.Visible = true;
                    paraVarType.Visible = true;
                    paraRawMin.Visible = true;
                    paraRawMax.Visible = true;
                    paraScaleMax.Visible = true;
                    paraScaleMin.Visible = true;
                    paraScaleOffset.Visible = true;
                    paraBitAddress.Visible = true;
                    paraDeadband.Visible = true;
                    paraBlockNr.Visible = true;
                    paraStartAddress.Visible = true;
                    paraDataType.Visible = true;
                    paraLogTime.Visible = false;
                    paraNrOfValues.Visible = false;
                    btnEkvation.Visible = false;
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
            if (opcForm != null)
                opcForm.Close();
        }

        private void txtDeadband_TextChanged(object sender, CancelEventArgs e)
        {

        }

        private void clickEkvation(object sender, EventArgs e)
        {
            if (tag is null)
            {
                paraVarType.comboBoxen.SelectedIndex = 0;
                paraBlockNr.ParameterValue = "0";
                paraStartAddress.ParameterValue = "0";

                tag = new TagDefinitions()
                {
                    Active = checkActive.Checked,
                    AnalogTrigger = AnalogTrigger.Equal,
                    Name = paraName.ParameterValue,
                    Description = paraDescription.ParameterValue,
                    BlockNr = 0,
                    StartByte = 0,
                    NrOfElements = 0,
                    BitAddress = 0,
                };
            }
            var calculationForm = new CalculationForm(MyPlc, tag);
            calculationForm.Show();
        }
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        public bool logInfoFormOpen = false;
        private void btnInfo_Click(object sender, EventArgs e)
        {
            if (logInfoFormOpen)
                return;

            var logInfoForm = new LogTypeInfoForm(this);
            logInfoForm.Show();
            logInfoFormOpen = true;
        }
    }
}
