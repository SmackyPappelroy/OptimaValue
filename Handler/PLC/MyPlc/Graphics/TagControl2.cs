using CsvHelper;
using CsvHelper.Configuration;
using OptimaValue.Handler.PLC.MyPlc.Graphics;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace OptimaValue
{
    enum IntouchType { None, Int, Real, Bool }
    public partial class TagControl2 : UserControl
    {
        private readonly string PlcName = string.Empty;

        private List<TagDefinitions> tags;
        private DataTable myTable = new DataTable();
        private readonly TreeView myTreeView;
        private AddTag addTagForm;
        private readonly ExtendedPlc myPlc;
        private bool statFormOpen = false;
        private AllTagsStatsForm statForm;

        private bool addMenuOpen = false;

        private string fileName;

        CsvConfiguration csvConfig;

        public TagControl2(ExtendedPlc activePlc, TreeView treeView)
        {
            InitializeComponent();
            PlcName = activePlc.PlcName;
            myTreeView = treeView;
            myPlc = activePlc;
            if (activePlc.LoggerIsStarted)
                contextMenuStrip.Enabled = false;

            csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                DetectDelimiter = true
            };
            this.flowLayoutPanel.AllowDrop = true;

            this.flowLayoutPanel.DragEnter += TagControl2_DragEnter;
            this.flowLayoutPanel.DragDrop += TagControl2_DragDrop;
        }

        private void TagControl2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void TagControl2_DragDrop(object sender, DragEventArgs e)
        {
            string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            fileName = FileList[0];

            if (fileName.Substring(fileName.Length - 3) != "csv".ToLower())
            {
                Apps.Logger.Log("Fel fil-format", Severity.Error);
                return;
            }

            ImportFile(fileName);
        }


        private void TagControl2_Load(object sender, EventArgs e)
        {
            PopulateTags();
        }

        private void PopulateTags()
        {
            if (myPlc != null)
            {
                if (myPlc.LoggerIsStarted)
                {
                    foreach (TagDefinitions tag in TagsToLog.AllLogValues)
                    {
                        if (tag.PlcName == myPlc.PlcName)
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
            myTable = DatabaseSql.GetTags(plcName: PlcName);

            PopulateList(myTable);
        }

        private bool PopulateList(DataTable tbl)
        {
            if (tbl == null)
                return false;

            if (tbl.Rows.Count == 0)
            {
                if (tags != null)
                    tags.Clear();
                return false;
            }

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
                var _description = (tbl.AsEnumerable().ElementAt(rowIndex).Field<string>("description"));
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
                var _scaleMin = (tbl.AsEnumerable().ElementAt(rowIndex).Field<int>("scaleMin"));
                var _scaleMax = (tbl.AsEnumerable().ElementAt(rowIndex).Field<int>("scaleMax"));
                var _scaleOffset = (tbl.AsEnumerable().ElementAt(rowIndex).Field<int>("scaleOffset"));

                var myTag = new TagDefinitions()
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
                    Name = _name,
                    Description = _description,
                    NrOfElements = _nrOfElements,
                    PlcName = _plcName,
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
                tags.Add(myTag);
            }
            // Sorterar listan alfabetiskt
            tags.Sort((x, y) => string.Compare(x.Name, y.Name));

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

        private void click_Export(object sender, EventArgs e)
        {
            SaveFileDialog save = new();
            save.Filter = "CSV File|*.csv";
            save.Title = "Spara .CSV fil";
            DialogResult result = save.ShowDialog();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";"
            };

            if (save.FileName != "" && result == DialogResult.OK)
            {
                try
                {
                    using (var sw = new StreamWriter(save.FileName))
                    {
                        var wr = new CsvWriter(sw, config);

                        wr.WriteRecords(tags);
                        Apps.Logger.Log($"Sparade {myPlc.PlcName}s taggar till {save.FileName}", Severity.Success);
                    }
                }
                catch (IOException)
                {
                    Apps.Logger.Log($"Lyckades ej spara {myPlc.PlcName}s taggar till {save.FileName}", Severity.Error);
                }
            }
        }

        private void click_Import(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string file = dialog.FileName;
                ImportFile(file);
            }
        }

        private void ImportFile(string fileName)
        {
            bool intouchCsv = false;
            try
            {
                using (var sr = new StreamReader(fileName))
                {
                    using (var csvReader = new CsvReader(sr, csvConfig))
                    {
                        try
                        {
                            var records = csvReader.GetRecords<TagDefinitions>().ToList();
                            AddTag(records);
                            UpdateTag(records);
                            Apps.Logger.Log($"Importerade {myPlc.PlcName}s taggar från {fileName}", Severity.Success);
                            return;
                        }
                        catch (HeaderValidationException)
                        {
                            // Kolla om det är Intouch taggar
                            intouchCsv = true;
                        }
                    }


                }
                // TODO: Fixa så man kan läsa intouch taggar
                using (var sr = new StreamReader(fileName))
                {
                    var intouchInt = new List<IntouchInt>();
                    var intouchReal = new List<IntouchReal>();
                    var intouchBool = new List<IntouchBool>();

                    if (intouchCsv)
                    {
                        var config = new CsvConfiguration(CultureInfo.InvariantCulture);

                        //config.Delimiter = ";";
                        config.DetectDelimiter = true;
                        config.MissingFieldFound = (arg) => { };
                        config.BadDataFound = (arg) => { };

                        var intouchType = IntouchType.None;

                        using var csvReader = new CsvReader(sr, config);


                        while (csvReader.Read())
                        {
                            var discriminator = csvReader.GetField<string>(0);
                            switch (discriminator)
                            {
                                case ":IOInt":
                                    intouchType = IntouchType.Int;
                                    break;
                                case ":IOReal":
                                    intouchType = IntouchType.Real;
                                    break;
                                case ":IODisc":
                                    intouchType = IntouchType.Bool;
                                    break;
                                case "":
                                    intouchType = IntouchType.None;
                                    break;
                                default:
                                    break;
                            }
                            if (intouchType == IntouchType.Int)
                            {
                                if (discriminator == ":IOInt")
                                {
                                    csvReader.ReadHeader();
                                    csvReader.Read();
                                }
                                if (discriminator.Substring(0, 1) != ":" || discriminator != "")
                                {
                                    intouchInt.Add(csvReader.GetRecord<IntouchInt>());
                                }
                            }
                            else if (intouchType == IntouchType.Real)
                            {
                                if (discriminator == ":IOReal")
                                {
                                    csvReader.ReadHeader();
                                    csvReader.Read();
                                }
                                if (discriminator.Substring(0, 1) != ":" || discriminator != "")
                                {
                                    intouchReal.Add(csvReader.GetRecord<IntouchReal>());
                                }
                            }
                            else if (intouchType == IntouchType.Bool)
                            {
                                if (discriminator == ":IODisc")
                                {
                                    csvReader.ReadHeader();
                                    csvReader.Read();
                                }
                                if (discriminator.Substring(0, 1) != ":" || discriminator != "")
                                {
                                    intouchBool.Add(csvReader.GetRecord<IntouchBool>());
                                }
                            }
                        }
                    }
                    List<TagDefinitions> allTags = new();
                    intouchBool = intouchBool.Where(x => x.AccessName.ToLower() == PlcName.ToLower()).ToList();
                    intouchInt = intouchInt.Where(x => x.AccessName.ToLower() == PlcName.ToLower()).ToList();
                    intouchReal = intouchReal.Where(x => x.AccessName.ToLower() == PlcName.ToLower()).ToList();

                    if (intouchBool.Count > 0)
                    {
                        allTags.AddRange(MapIntouchTagsToLogTags(intouchBool));
                    }
                    if (intouchInt.Count > 0)
                    {
                        allTags.AddRange(MapIntouchTagsToLogTags(intouchInt));
                    }
                    if (intouchReal.Count > 0)
                    {
                        allTags.AddRange(MapIntouchTagsToLogTags(intouchReal));
                    }

                    if (allTags.Count > 0)
                    {
                        AddTag(allTags);
                        Redraw();
                    }
                }

            }
            catch (IOException)
            {
                Apps.Logger.Log($"Lyckades ej läsa {myPlc.PlcName}s taggar från {fileName}", Severity.Error);
            }
        }

        private List<TagDefinitions> MapIntouchTagsToLogTags(List<IntouchInt> intouchInt)
        {
            List<TagDefinitions> intList = new();
            foreach (IntouchInt intouchTag in intouchInt)
            {
                if (intouchTag.Logged.ToLower() == "yes")
                {
                    var address = intouchTag.ItemName.PLCAddress();
                    TagDefinitions tag = new()
                    {
                        Active = true,
                        BitAddress = address.bitNumber,
                        BlockNr = address.dbNumber,
                        DataType = address.dataType,
                        StartByte = address.startByte,
                        VarType = address.varType,
                        Deadband = intouchTag.LogDeadband,
                        LogFreq = LogFrequency._1s,
                        LogType = intouchTag.LogDeadband == 0 ? LogType.Cyclic : LogType.Delta,
                        Name = intouchTag.Name,
                        Description = intouchTag.Comment,
                        PlcName = intouchTag.AccessName,
                        scaleMin = intouchTag.MinEu,
                        scaleMax = intouchTag.MaxEu,
                        TagUnit = intouchTag.EngUnits
                    };
                    intList.Add(tag);
                }

            }
            return intList;
        }

        private List<TagDefinitions> MapIntouchTagsToLogTags(List<IntouchReal> intouchReal)
        {
            List<TagDefinitions> intList = new();
            foreach (IntouchReal intouchTag in intouchReal)
            {
                if (intouchTag.Logged.ToLower() == "yes")
                {
                    var address = intouchTag.ItemName.PLCAddress();
                    TagDefinitions tag = new()
                    {
                        Active = true,
                        BitAddress = address.bitNumber,
                        BlockNr = address.dbNumber,
                        DataType = address.dataType,
                        StartByte = address.startByte,
                        VarType = address.varType,
                        Deadband = intouchTag.LogDeadband,
                        LogFreq = LogFrequency._1s,
                        LogType = intouchTag.LogDeadband == 0 ? LogType.Cyclic : LogType.Delta,
                        Name = intouchTag.Name,
                        Description = intouchTag.Comment,
                        PlcName = intouchTag.AccessName,
                        scaleMin = (int)intouchTag.MinEu,
                        scaleMax = (int)intouchTag.MaxEu,
                        TagUnit = intouchTag.EngUnits
                    };
                    intList.Add(tag);
                }

            }
            return intList;
        }

        private List<TagDefinitions> MapIntouchTagsToLogTags(List<IntouchBool> intouchBool)
        {
            List<TagDefinitions> boolList = new();
            foreach (IntouchBool intouchTag in intouchBool)
            {
                if (intouchTag.Logged.ToLower() == "yes")
                {
                    var address = intouchTag.ItemName.PLCAddress();
                    TagDefinitions tag = new()
                    {
                        Active = true,
                        BitAddress = address.bitNumber,
                        BlockNr = address.dbNumber,
                        DataType = address.dataType,
                        StartByte = address.startByte,
                        VarType = address.varType,
                        Deadband = 0,
                        LogFreq = LogFrequency._1s,
                        LogType = LogType.Cyclic,
                        Name = intouchTag.Name,
                        Description = intouchTag.Comment,
                        PlcName = intouchTag.AccessName,
                        scaleMin = 0,
                        scaleMax = 0,
                    };
                    boolList.Add(tag);
                }

            }
            return boolList;
        }

        private void AddTag(List<TagDefinitions> newList)
        {

            newList = newList.OrderBy(x => x.Name).ToList();
            List<TagDefinitions> newTags = new();

            if (tags != null)
            {
                newTags = newList.Except(tags).ToList();
                newTags.RemoveAll(a => tags.Contains(a));
            }
            else
                newTags = newList;


            if (newTags.Count == 0)
                return;

            foreach (var tag in newTags)
            {
                var query = $"INSERT INTO {SqlSettings.Default.Databas}.dbo.tagConfig ";
                query += $"(active,name,description,logType,timeOfDay,deadband,plcName,varType,blockNr,dataType,startByte,nrOfElements,bitAddress,logFreq,";
                query += $"tagUnit,eventId,isBooleanTrigger,boolTrigger,analogTrigger,analogValue,scaleMin,scaleMax,scaleOffset) ";
                query += $"VALUES ('{tag.Active}','{tag.Name}','{tag.Description}','{tag.LogType}','{tag.TimeOfDay}',";
                query += $"{tag.Deadband},'{tag.PlcName}','{tag.VarType}',{tag.BlockNr}, ";
                query += $"'{tag.DataType}',{tag.StartByte},{tag.NrOfElements},";
                query += $"{tag.BitAddress},'{tag.LogFreq}','{tag.TagUnit}',{tag.EventId},'{tag.IsBooleanTrigger}','";
                query += $"{tag.BoolTrigger}','{tag.AnalogTrigger}',{tag.AnalogValue},{tag.scaleMin},{tag.scaleMax},{tag.scaleOffset})";

                DatabaseSql.AddTag(query);

            }

        }

        private void UpdateTag(List<TagDefinitions> list)
        {
            foreach (var tag in list)
            {
                var query = $"UPDATE {SqlSettings.Default.Databas}.dbo.tagConfig ";
                query += $"SET active='{tag.Active}',name='{tag.Name}',logType='{tag.LogType}',timeOfDay='{tag.TimeOfDay}'";
                query += $",deadband={tag.Deadband},plcName='{tag.PlcName}',description='{tag.Description}',varType='{tag.VarType}',blockNr={tag.BlockNr}" +
                    $",dataType='{tag.DataType}',startByte={tag.StartByte},nrOfElements={tag.NrOfElements}" +
                    $",bitAddress={tag.BitAddress},logFreq='{tag.LogFreq}',";
                query += $"tagUnit='{tag.TagUnit}',eventId={tag.EventId},isBooleanTrigger='{tag.IsBooleanTrigger}'" +
                    $",boolTrigger='{tag.BoolTrigger}',analogTrigger='{tag.AnalogTrigger}',analogValue={tag.AnalogValue}, " +
                    $"scaleMin={tag.scaleMin},scaleMax={tag.scaleMax},scaleOffset={tag.scaleOffset}" +
                    $" WHERE id = {tag.Id}";

                DatabaseSql.UpdateTag(query);

            }
            Redraw();

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
