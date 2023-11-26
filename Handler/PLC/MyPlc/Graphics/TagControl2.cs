using CsvHelper;
using CsvHelper.Configuration;
using OptimaValue.Handler.PLC.MyPlc.Graphics;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OptimaValue.Config;
using System.Threading.Tasks;
using System.Text.Json;
using System.Runtime.CompilerServices;
using S7.Net.Types;
using System.Reflection;
using System.Data.SqlTypes;
using FileLogger;
using ClosedXML.Excel;

namespace OptimaValue;

enum IntouchType { None, Int, Real, Bool }
public partial class TagControl2 : UserControl
{
    private readonly string PlcName = string.Empty;

    private List<TagDefinitions> tags;
    private DataTable myTable = new DataTable();
    private readonly TreeView myTreeView;
    private AddPlcFromFile addTagForm;
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
        this.flowLayoutPanel.DragLeave += FlowLayoutPanel_DragLeave;
    }

    private void FlowLayoutPanel_DragLeave(object sender, EventArgs e)
    {
        this.flowLayoutPanel.BackColor = Color.FromArgb(67, 62, 71);
    }

    private void TagControl2_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (FileList[0].Substring(FileList[0].Length - 3, 3).ToLower() == "csv")
            {
                e.Effect = DragDropEffects.Copy;
                this.flowLayoutPanel.BackColor = Color.FromArgb(92, 81, 100);
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        else
        {
            this.flowLayoutPanel.BackColor = Color.FromArgb(67, 62, 71);
            e.Effect = DragDropEffects.None;
        }
    }

    private void TagControl2_DragDrop(object sender, DragEventArgs e)
    {
        string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
        this.flowLayoutPanel.BackColor = Color.FromArgb(67, 62, 71);

        fileName = FileList[0];
        var fileEnding = fileName.Substring(fileName.Length - 4, 4).ToLower();

        if (fileEnding != ".csv".ToLower() && fileEnding != "json".ToLower())
        {
            Logger.LogError("Fel fil-format");
            return;
        }
        if (fileEnding == "json")
        {
            ReadJsonFile(fileName);
        }
        else
            ImportFile(fileName);
    }

    private void ReadJsonFile(string fileName)
    {
        //try
        //{
        //    var json = File.ReadAllText(fileName);
        //    var tiaPortalTags = System.Text.Json.JsonSerializer.Deserialize<List<TiaPortalTag>>(json);
        //    if (tiaPortalTags != null)
        //    {
        //        var tagDefs = new List<TagDefinitions>();
        //        foreach (var tag in tiaPortalTags)
        //        {
        //            var tagdef = new TagDefinitions()
        //            {
        //                Active = true,
        //                Name = tag.TagName,
        //                DataType = Enum.Parse<DataType>(tag.DataType, true),
        //                BlockNr = int.Parse(tag.DbNr),
        //                StartByte = int.Parse(tag.Offset),
        //                LogType= LogType.Cyclic,
        //                LogFreq = LogFrequency._1s,
        //            };
        //            tagDefs.Add(tagdef);
        //        }
        //        AddTag(tagDefs);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    MessageBox.Show(ex.Message);
        //}
    }

    private async void TagControl2_Load(object sender, EventArgs e)
    {
        await PopulateTagsAsync();
    }

    private async Task PopulateTagsAsync()
    {
        await Task.Run(() =>
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
                                flowLayoutPanel.Invoke((System.Windows.Forms.MethodInvoker)delegate
                                {
                                    flowLayoutPanel.Controls.Add(newTag);
                                });
                                this.Disposed += (s, e) => newTag.Dispose();
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
                            flowLayoutPanel.Invoke((System.Windows.Forms.MethodInvoker)delegate
                            {
                                flowLayoutPanel.Controls.Add(newTag);
                            });
                        }
                    }
                }
            });

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

            var _rawMin = (float)(tbl.AsEnumerable().ElementAt(rowIndex).Field<double>("rawMin"));
            var _rawMax = (float)(tbl.AsEnumerable().ElementAt(rowIndex).Field<double>("rawMax"));

            var _scaleMin = (float)(tbl.AsEnumerable().ElementAt(rowIndex).Field<double>("scaleMin"));
            var _scaleMax = (float)(tbl.AsEnumerable().ElementAt(rowIndex).Field<double>("scaleMax"));
            var _scaleOffset = (float)(tbl.AsEnumerable().ElementAt(rowIndex).Field<double>("scaleOffset"));
            var _calculation = (tbl.AsEnumerable().ElementAt(rowIndex).Field<string>("calculation"));

            var myTag = new TagDefinitions()
            {
                Active = _active,
                BitAddress = _bitAddress,
                BlockNr = _blockNr,
                DataType = _dataType,
                Deadband = _deadband,
                Id = _id,
                LogFreq = _logFreq,
                LastLogTime = System.DateTime.MinValue,
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
                rawMin = _rawMin,
                rawMax = _rawMax,
                scaleMin = _scaleMin,
                scaleMax = _scaleMax,
                scaleOffset = _scaleOffset,
                Calculation = _calculation
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
            addTagForm = new AddPlcFromFile(PlcName, myPlc);
            addTagForm.FormClosing += AddTagForm_FormClosing;
            addTagForm.Show();
        }
    }

    private void click_Export(object sender, EventArgs e)
    {
        SaveFileDialog save = new();
        save.Filter = "CSV File|*.CSV|Excel File|*.XLSX";
        save.Title = "Spara taggar till fil";
        DialogResult result = save.ShowDialog();



        if (save.FileName != "" && result == DialogResult.OK)
        {
            if (save.FileName.EndsWith(".CSV"))
            {
                saveToCsvFile(save.FileName);
            }
            else if (save.FileName.EndsWith(".XLSX"))
            {
                //saveToCsvFile(save.FileName);
                saveToExcel(save.FileName);
            }
        }
    }

    private void saveToCsvFile(string fileName)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";"
        };
        try
        {
            using (var sw = new StreamWriter(fileName))
            {

                var wr = new CsvWriter(sw, config);

                wr.WriteRecords(tags);
                Logger.LogSuccess($"Sparade {myPlc.PlcName}s taggar till {fileName}");
            }
        }
        catch (IOException)
        {
            Logger.LogError($"Lyckades ej spara {myPlc.PlcName}s taggar till {fileName}");
        }
    }

    private void saveToExcel(string fileName)
    {
        //populate myList with data

        DataTable dt = new DataTable();

        // get the properties of the class
        PropertyInfo[] props = typeof(TagDefinitions).GetProperties();

        // add the columns to the DataTable
        foreach (PropertyInfo prop in props)
        {
            dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }

        try
        {
            // add the data to the DataTable
            foreach (TagDefinitions obj in tags)
            {
                var row = dt.NewRow();
                foreach (PropertyInfo prop in props)
                {
                    // if the property is datetime
                    if (prop.GetValue(obj, null) == null)
                    {
                        row[prop.Name] = DBNull.Value;
                    }
                    else if (prop.PropertyType == typeof(System.DateTime))
                    {
                        // If value is DateTime.MinValue set it to Sql datetime.minvalue
                        if ((System.DateTime)prop.GetValue(obj, null) == System.DateTime.MinValue)
                        {
                            row[prop.Name] = SqlDateTime.MinValue.Value;
                        }
                        else
                        {
                            row[prop.Name] = prop.GetValue(obj, null);
                        }
                    }
                    else
                    {
                        row[prop.Name] = prop.GetValue(obj, null);
                    }
                }
                dt.Rows.Add(row);
            }
        }
        catch (Exception ex)
        {
            throw;
        }

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Sheet1");

            worksheet.Cell(1, 1).InsertTable(dt);

            workbook.SaveAs(fileName);

        }
    }

    private void saveToExcelFileFromCsvFile(string fileName)
    {
        // Open the CSV file and read its contents into a DataTable
        DataTable dt = new DataTable();
        using (StreamReader sr = new StreamReader(fileName))
        {
            string[] headers = sr.ReadLine().Split(';');
            foreach (string header in headers)
            {
                dt.Columns.Add(header);
            }
            while (!sr.EndOfStream)
            {
                string[] rows = sr.ReadLine().Split(';');
                DataRow dr = dt.NewRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    dr[i] = rows[i];
                }
                dt.Rows.Add(dr);
            }
        }

        // Create a new XLWorkbook and add the DataTable as a worksheet
        string xlsxFile = @"c:\temp\test.xlsx";
        using (XLWorkbook wb = new XLWorkbook())
        {
            wb.AddWorksheet(dt, "Sheet1");
            wb.SaveAs(xlsxFile);
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
                        if (records == null)
                        {
                            $"Hittade inga taggar i {myPlc.PlcName}".SendStatusMessage(Severity.Warning);
                            return;
                        }
                        if (records.Count == 0 || !records.Exists(x => x.PlcName == myPlc.PlcName))
                        {
                            $"Hittade inga taggar i {myPlc.PlcName}".SendStatusMessage(Severity.Warning);
                            return;
                        }

                        AddTag(records);
                        UpdateTag(records);
                        Logger.LogSuccess($"Importerade {myPlc.PlcName}s taggar från {fileName}");
                        return;
                    }
                    catch (HeaderValidationException)
                    {
                        // Kolla om det är Intouch taggar
                        intouchCsv = true;
                    }
                }


            }
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
            Logger.LogError($"Lyckades ej läsa {myPlc.PlcName}s taggar från {fileName}");
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
            var query = $"INSERT INTO {Settings.Databas}.dbo.tagConfig ";
            query += $"(active,name,description,logType,timeOfDay,deadband,plcName,varType,blockNr,dataType,startByte,nrOfElements,bitAddress,logFreq,";
            query += $"tagUnit,eventId,isBooleanTrigger,boolTrigger,analogTrigger,analogValue,scaleMin,scaleMax,scaleOffset,calculation) ";
            query += $"VALUES ('{tag.Active}','{tag.Name}','{tag.Description}','{tag.LogType}','{tag.TimeOfDay}',";
            query += $"{tag.Deadband},'{tag.PlcName}','{tag.VarType}',{tag.BlockNr}, ";
            query += $"'{tag.DataType}',{tag.StartByte},{tag.NrOfElements},";
            query += $"{tag.BitAddress},'{tag.LogFreq}','{tag.TagUnit}',{tag.EventId},'{tag.IsBooleanTrigger}','";
            query += $"{tag.BoolTrigger}','{tag.AnalogTrigger}',{tag.AnalogValue},{tag.scaleMin},{tag.scaleMax},{tag.scaleOffset},'{tag.Calculation}')";

            DatabaseSql.AddTag(query);

        }

    }

    private void UpdateTag(List<TagDefinitions> list)
    {
        foreach (var tag in list)
        {
            var query = $"UPDATE {Settings.Databas}.dbo.tagConfig ";
            query += $"SET active='{tag.Active}',name='{tag.Name}',logType='{tag.LogType}',timeOfDay='{tag.TimeOfDay}'";
            query += $",deadband={tag.Deadband},plcName='{tag.PlcName}',description='{tag.Description}',varType='{tag.VarType}',blockNr={tag.BlockNr}" +
                $",dataType='{tag.DataType}',startByte={tag.StartByte},nrOfElements={tag.NrOfElements}" +
                $",bitAddress={tag.BitAddress},logFreq='{tag.LogFreq}',";
            query += $"tagUnit='{tag.TagUnit}',eventId={tag.EventId},isBooleanTrigger='{tag.IsBooleanTrigger}'" +
                $",boolTrigger='{tag.BoolTrigger}',analogTrigger='{tag.AnalogTrigger}',analogValue={tag.AnalogValue}, " +
                $"scaleMin={tag.scaleMin},scaleMax={tag.scaleMax},scaleOffset={tag.scaleOffset},calculation='{tag.Calculation}'" +
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

    private void TagControl2_DragEnter_1(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop) && e.Data.GetDataPresent(DataFormats.Text))
        {
            e.Effect = DragDropEffects.Copy;
        }
    }

    private void TagControl2_DragDrop_1(object sender, DragEventArgs e)
    {
        // Import JSON file
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                if (file.EndsWith(".json"))
                {
                    var json = File.ReadAllText(file);
                    var tagList = JsonSerializer.Deserialize<List<TagDefinitions>>(json);
                    AddTag(tagList);
                    Redraw();
                }
            }
        }
    }
}

