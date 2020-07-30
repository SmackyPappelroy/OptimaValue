using OptimaValue.Handler.PLC.MyPlc.Graphics;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace OptimaValue
{
    public partial class TagControl2 : UserControl
    {
        private readonly string PlcName = string.Empty;

        private List<TagDefinitions> tags;
        private readonly DataTable myTable = new DataTable();
        private readonly TreeView myTreeView;
        private AddTag addTagForm;
        private readonly ExtendedPlc myPlc;
        private bool statFormOpen = false;
        private AllTagsStatsForm statForm;

        private bool addMenuOpen = false;

        public TagControl2(ExtendedPlc activePlc, TreeView treeView)
        {
            InitializeComponent();
            PlcName = activePlc.PlcName;
            myTreeView = treeView;
            myPlc = activePlc;
            if (activePlc.logger.IsStarted)
                contextMenuStrip.Enabled = false;
        }


        private void TagControl2_Load(object sender, EventArgs e)
        {
            PopulateTags();
        }

        private void PopulateTags()
        {
            if (myPlc != null)
            {
                if (myPlc.logger.IsStarted)
                {
                    foreach (TagDefinitions tag in TagsToLog.AllLogValues)
                    {
                        if (tag.plcName == myPlc.PlcName)
                        {
                            var newTag = new SingleTagControl(tag, myPlc, myTreeView);
                            newTag.TagChanged -= NewTag_TagChanged;
                            newTag.TagChanged += NewTag_TagChanged;
                            flowLayoutPanel.Controls.Add(newTag);
                        }
                    }
                    return;
                }
            }

            FetchTagsFromSql();
            if (myTable != null)
            {
                if (myTable.Rows.Count > 0)
                {
                    foreach (TagDefinitions tag in tags)
                    {
                        var newTag = new SingleTagControl(tag, myPlc, myTreeView);
                        newTag.TagChanged -= NewTag_TagChanged;
                        newTag.TagChanged += NewTag_TagChanged;
                        flowLayoutPanel.Controls.Add(newTag);
                    }
                }
            }
        }

        private void Redraw()
        {
            myTable.Clear();
            FetchTagsFromSql();
            flowLayoutPanel.Controls.Clear();
            if (myTable != null)
            {
                if (myTable.Rows.Count > 0)
                {
                    foreach (TagDefinitions tag in tags)
                    {
                        var newTag = new SingleTagControl(tag, myPlc, myTreeView);
                        newTag.TagChanged += NewTag_TagChanged;
                        flowLayoutPanel.Controls.Add(newTag);
                    }
                }
            }
        }

        private void NewTag_TagChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        private void FetchTagsFromSql()
        {
            var query = $"SELECT * FROM {SqlSettings.Default.Databas}.dbo.tagConfig ";
            query += $"WHERE plcName = '{PlcName}'";
            var connectionString = PlcConfig.ConnectionString();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                        {
                            con.Open();
                            adp.Fill(myTable);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StatusEvent.RaiseMessage($"Hittade inga taggar", Status.Ok);
            }
            PopulateList(myTable);
        }

        private bool PopulateList(DataTable tbl)
        {
            if (tbl == null)
                return false;

            if (tbl.Rows.Count == 0)
                return false;

            if (tags == null)
                tags = new List<TagDefinitions>();

            tags.Clear();

            for (int rowIndex = 0; rowIndex < tbl.Rows.Count; rowIndex++)
            {

                var _active = (tbl.AsEnumerable().ElementAt(rowIndex).Field<bool>("active"));
                var _bitAddress = (tbl.AsEnumerable().ElementAt(rowIndex).Field<byte>("bitAddress"));
                var _blockNr = (tbl.AsEnumerable().ElementAt(rowIndex).Field<int>("blockNr"));
                var _dataType = (DataType)Enum.Parse(typeof(DataType), (tbl.AsEnumerable().ElementAt(rowIndex).Field<string>("dataType")));
                var _deadband = (float)(tbl.AsEnumerable().ElementAt(rowIndex).Field<double>("deadband"));
                var _id = (tbl.AsEnumerable().ElementAt(rowIndex).Field<int>("id"));
                var _logFreq = (LogFrequency)Enum.Parse(typeof(LogFrequency), (tbl.AsEnumerable().ElementAt(rowIndex).Field<string>("logFreq")));
                var _logType = (LogType)Enum.Parse(typeof(LogType), (tbl.AsEnumerable().ElementAt(rowIndex).Field<string>("logType")));
                var _name = (tbl.AsEnumerable().ElementAt(rowIndex).Field<string>("name"));
                var _nrOfElements = (tbl.AsEnumerable().ElementAt(rowIndex).Field<int>("nrOfElements"));
                var _plcName = (tbl.AsEnumerable().ElementAt(rowIndex).Field<string>("plcName"));
                var _startByte = (tbl.AsEnumerable().ElementAt(rowIndex).Field<int>("startByte"));
                var _timeOfDay = (tbl.AsEnumerable().ElementAt(rowIndex).Field<TimeSpan>("timeOfDay"));
                var _varType = (VarType)Enum.Parse(typeof(VarType), (tbl.AsEnumerable().ElementAt(rowIndex).Field<string>("varType")));
                var _tagUnit = (tbl.AsEnumerable().ElementAt(rowIndex).Field<string>("tagUnit"));
                var _eventId = (tbl.AsEnumerable().ElementAt(rowIndex).Field<int>("eventId"));
                var _isBooleanTrigger = (tbl.AsEnumerable().ElementAt(rowIndex).Field<bool>("isBooleanTrigger"));
                var _boolTrigger = (BooleanTrigger)Enum.Parse(typeof(BooleanTrigger), (tbl.AsEnumerable().ElementAt(rowIndex).Field<string>("boolTrigger")));
                var _analogTrigger = (AnalogTrigger)Enum.Parse(typeof(AnalogTrigger), (tbl.AsEnumerable().ElementAt(rowIndex).Field<string>("analogTrigger")));
                var _analogValue = (float)(tbl.AsEnumerable().ElementAt(rowIndex).Field<double>("analogValue"));


                var myTag = new TagDefinitions()
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
                tags.Add(myTag);
            }
            // Sorterar listan alfabetiskt
            tags.Sort((x, y) => string.Compare(x.name, y.name));

            if (tags.Count > 0)
                return true;
            else
                return false;
        }


        private void addMenu_Click_1(object sender, EventArgs e)
        {
            myTreeView.Enabled = false;
            if (this.HasChildren)
            {
                foreach (Control theControl in this.Controls)
                {
                    theControl.Enabled = false;
                }
            }
            if (!addMenuOpen)
            {
                addTagForm = new AddTag(PlcName, myPlc);
                addTagForm.FormClosing += AddTagForm_FormClosing;
                addTagForm.Show();
            }
        }

        private void AddTagForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            addMenuOpen = false;
            myTreeView.Enabled = true;
            if (this.HasChildren)
            {
                foreach (Control theControl in this.Controls)
                {
                    theControl.Enabled = true;
                }
            }
            addTagForm.FormClosing -= AddTagForm_FormClosing;
            Redraw();
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (!statFormOpen)
            {
                statForm = null;
                statForm = new AllTagsStatsForm(myPlc);
                statForm.FormClosing += StatForm_FormClosing;
                statForm.Show();
                statFormOpen = true;
            }
            else
            {
                statForm.Show();
            }
        }

        private void StatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            statFormOpen = false;
            statForm.FormClosing -= StatForm_FormClosing;
        }
    }
}
