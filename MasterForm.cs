using IWshRuntimeLibrary;
using OptimaValue.Handler.PLC.Graphics;
using OptimaValue.Handler.PLC.MyPlc.Graphics;
using OptimaValue.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using FileLogger;
using System.Security.Principal;

namespace OptimaValue;

public partial class MasterForm : Form
{
    private class TagId
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
    #region Fields
    private AutoCompleteStringCollection stringCollection = new AutoCompleteStringCollection();
    private ExtendedPlc activePlc;
    private PlcSettingsControl settingsControl;
    private StatusControl statusControl;
    private TagControl2 tagControl;
    private PerformanceForm perForm;
    private bool perFormOpen = false;
    private readonly System.Windows.Forms.Timer statusTimer;
    private readonly System.Windows.Forms.Timer databaseTimer;
    private readonly System.Windows.Forms.Timer startStopButtonVisibilityTimer;
    private Queue<LogTemplate> messageQueue = new Queue<LogTemplate>();
    private System.Windows.Forms.Timer messageTimer = new System.Windows.Forms.Timer
    {
        Interval = 5000  // Set your interval here
    };
    private sqlForm SqlForm;
    private bool IsOpenSqlForm = false;
    private bool isSubscribed = false;
    private Image noDatabase;
    private Image okDatabase;
    private object lockObject = new();
    #endregion

    #region Properties
    private List<TagDefinitions> AvailableTagsTrend => GetAvailableTagsTrend();

    private List<TagDefinitions> GetAvailableTagsTrend()
    {
        lock (lockObject)
        {
            var activePlcs = PlcConfig.PlcList.Where(p => p.ConnectionStatus == ConnectionStatus.Connected);
            var tags = TagsToLog.AllLogValues.Where(x => x.PlcName == activePlcs.First().PlcName).ToList();
            tags = tags.Where(x => x.Active).ToList();
            tags = tags.OrderBy(x => x.PlcName).ThenBy(x => x.Name).ToList();
            return tags;
        }
    }

    private List<TagId> TagIds => GetTagIds();

    private List<TagId> GetTagIds()
    {
        List<TagId> ids = new();
        if (AvailableTagsTrend == null)
            return ids;
        foreach (var item in AvailableTagsTrend)
        {
            var id = new TagId()
            {
                Id = item.Id,
                Name = item.Name
            };
            ids.Add(id);
        }
        return ids;
    }




    private string databaseTooltip => DatabaseSql.isConnected ? "Ansluten till SQL-server" : "Ingen anslutning till SQL";
    #endregion

    #region Constructor
    public MasterForm()
    {
        InitializeComponent();
        txtStatus.Text = string.Empty;
        statusTimer = new System.Windows.Forms.Timer
        {
            Interval = 7000
        };
        startStopButtonVisibilityTimer = new System.Windows.Forms.Timer
        {
            Interval = 500
        };
        databaseTimer = new System.Windows.Forms.Timer
        {
            Interval = 500
        };
        Subscribe(true);
        databaseTimer.Start();

        SqlForm = new sqlForm();
        SqlForm.FormClosing += SqlForm_FormClosing;
        notifyIcon.Visible = true;
        notifyIcon.Icon = Resources.icons8_gas_idle;

        menuStrip.BackColor = UIColors.ForeGroundLayer1;
        menuQuestion.ForeColor = UIColors.HeaderText;
        menuSettings.ForeColor = UIColors.HeaderText;
        btnStartTrend.ForeColor = UIColors.HeaderText;

        menuSettings.ChangeForeColorMenuItem(Color.Black, UIColors.HeaderText);
        menuQuestion.MouseHoverMenuItem(Color.Black, UIColors.HeaderText);
        btnStartTrend.MouseHoverMenuItem(Color.DarkGray, UIColors.HeaderText);
        comboTrend.Visible = false;
        btnStartTrend.Visible = false;

    }

    private void AddTrendTags()
    {
        stringCollection.AddRange(AvailableTagsTrend.Select(x => x.Name).ToArray());
        comboTrend.ComboBox.DataSource = TagIds;
        comboTrend.ComboBox.DisplayMember = "Name";
        comboTrend.ComboBox.ValueMember = "Id";
        comboTrend.AutoCompleteCustomSource = stringCollection;
        comboTrend.AutoCompleteMode = AutoCompleteMode.Suggest;
        comboTrend.AutoCompleteSource = AutoCompleteSource.CustomSource;
    }
    #endregion

    #region Form
    private async void MasterForm_Load(object sender, EventArgs e)
    {
        serviceImage.Visible = ServiceInstallerAppForm.ServiceExists();
        noDatabase = databaseImageList.Images[0];
        okDatabase = databaseImageList.Images[1];

        btnStop.Visible = false;
        btnStart.Visible = false;
        startStopButtonVisibilityTimer.Start();
        notifyMenu.Checked = Properties.Settings.Default.notify;
        autoStartTool.Checked = Properties.Settings.Default.AutoStart;


        var connectionOK = await DatabaseSql.TestConnectionAsync();
        bool tableExist = false;
        if (connectionOK)
        {
            tableExist = DatabaseSql.TableExist();
        }
        if (!connectionOK)
        {
            txtStatus.Text = "Misslyckades att ansluta till Sql";
            databaseImage.Image = noDatabase;
        }
        else if (!tableExist)
        {
            txtStatus.Text = "Databasen finns inte";
            databaseImage.Image = noDatabase;
        }
        else
        {
            PopulateTree();
            databaseImage.Image = okDatabase;
        }
        toolTip.InitialDelay = 100;
        toolTip.AutoPopDelay = 5000;

        if (autoStartTool.Checked)
            await AutoStart();


    }

    private async Task AutoStart()
    {
        if (DatabaseStatus.isConnected)
        {
            await StartLogging();
        }
    }

    private void MasterForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        startStopButtonVisibilityTimer.Stop();
        Subscribe(false);
        Master.StopLog(true);
        startStopButtonVisibilityTimer.Stop();
        System.Threading.Thread.Sleep(100);
    }

    private void notifyIcon_Click(object sender, EventArgs e)
    {
        if (!this.Visible)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }
        else
        {
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
        }
    }

    private void toolStripMenuItem1_Click(object sender, EventArgs e)
    {
        var assemblyVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
        notifyIcon.ShowBalloonTip(5000, $".NET-version {Environment.Version} \nCopyright © 2020 - v{assemblyVersion} ", "By Hans-Martin Nilsson", ToolTipIcon.None);
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
        if (!perFormOpen)
        {
            perForm = null;
            perForm = new PerformanceForm();
            perForm.FormClosed -= PerForm_FormClosed;
            perForm.FormClosed += PerForm_FormClosed;
            perForm.Show();
            perFormOpen = true;
        }
        else
        {
            perForm.BringToFront();
        }
    }

    private async void btnStart_Click(object sender, EventArgs e)
    {
        await StartLogging();

    }

    private async Task StartLogging()
    {
        btnStart.Visible = false;

        // Close all forms except for MasterForm
        foreach (var form in Application.OpenForms.Cast<Form>().Where(f => f.Name != "MasterForm").ToArray())
        {
            form.Close();
        }

        if (!await Master.StartLog())
        {
            btnStart.Visible = true;
            return;
        }

        settingsControl?.Hide();
        statusControl?.Hide();
        tagControl?.Hide();

        addPlc.Enabled = false;

    }


    private void btnStop_Click(object sender, EventArgs e)
    {
        Master.StopLog(false);
        addPlc.Enabled = true;
    }

    private void databasToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (IsOpenSqlForm)
        {
            SqlForm.BringToFront();
        }
        else
        {
            SqlForm = null;
            SqlForm = new sqlForm();
            SqlForm.FormClosing += SqlForm_FormClosing;
            SqlForm.Show();
            IsOpenSqlForm = true;
        }
        menuSettings.HideDropDown();
    }
    #endregion

    #region TreeView

    private void addPlc_Click(object sender, EventArgs e)
    {
        AddPlcNode("PLC");
    }

    private void PopulateTree()
    {
        foreach (TreeNode node in treeView.Nodes)
        {
            node.Nodes.Clear();
        }
        var temp = Task.Run(() => PlcConfig.PopulateDataTable());
        var tbl = temp.Result;
        if (tbl == null)
            return;
        if (tbl.Rows.Count == 0)
        {
            btnStart.Enabled = false;
        }
        else
            btnStart.Enabled = true;

        foreach (ExtendedPlc myPlc in PlcConfig.PlcList)
        {
            AddPlcNode(myPlc.PlcName);
        }
    }

    private void renameMenu_MouseDown(object sender, MouseEventArgs e)
    {
        if (treeView.SelectedNode.Name == "PLC")
        {
            treeView.LabelEdit = true;
            if (!treeView.SelectedNode.IsEditing)
                treeView.SelectedNode.BeginEdit();
        }
    }

    private void CloseUserControls()
    {
        if (settingsControl != null)
        {
            settingsControl.Hide();
            settingsControl = null;
        }
        if (statusControl != null)
        {
            statusControl.Hide();
            statusControl = null;
        }
        if (tagControl != null)
        {
            tagControl.Hide();
            tagControl = null;
        }
    }

    private void AddPlcNode(string plcLabel)
    {

        var configurationNode = new TreeNode("Konfiguration")
        {
            Name = "Konfiguration",
            ImageIndex = 4,
            SelectedImageIndex = 4
        };

        var statusNode = new TreeNode("Status")
        {
            Name = "Status",
            ImageIndex = 5,
            SelectedImageIndex = 5
        };

        var tagNode = new TreeNode("Taggar")
        {
            Name = "Taggar",
            ImageIndex = 6,
            SelectedImageIndex = 6
        };

        var plcNode = new TreeNode(plcLabel, new TreeNode[] { configurationNode, statusNode, tagNode })
        {
            Name = "PLC",
            ImageIndex = 1,
            SelectedImageIndex = 1
        };

        treeView.TopNode.Nodes.Add(plcNode);
        treeView.TopNode.Expand();
    }

    private void treeView_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        switch (treeView.SelectedNode.Name)
        {
            case "Konfiguration":
                var plcName = treeView.SelectedNode.Parent?.Text;
                activePlc = PlcConfig.PlcList.Find(x => x.PlcName == plcName);

                settingsControl?.Hide();
                settingsControl = null;

                if (activePlc == null)
                {
                    settingsControl = new PlcSettingsControl(ConnectionStatus.Disconnected, string.Empty, string.Empty, "1", "0", false, CpuType.S71500, 0, activePlc)
                    {
                        Parent = contentPanel,
                        Dock = DockStyle.Fill
                    };
                }
                else
                {
                    settingsControl = new PlcSettingsControl(activePlc.ConnectionStatus, activePlc.PlcName, activePlc.PlcConfiguration.Ip, activePlc.PlcConfiguration.Slot.ToString(), activePlc.PlcConfiguration.Rack.ToString(), activePlc.Active, (CpuType)activePlc.PlcConfiguration.CpuType, activePlc.ActivePlcId, activePlc)
                    {
                        Parent = contentPanel,
                        Dock = DockStyle.Fill
                    };
                }
                statusControl?.Hide();
                statusControl = null;
                tagControl?.Hide();
                tagControl = null;
                settingsControl?.Show();
                break;

            case "Status":
                var plcNameStatus = treeView.SelectedNode.Parent?.Text;
                activePlc = PlcConfig.PlcList.Find(x => x.PlcName == plcNameStatus);

                if (activePlc == null)
                {
                    return;
                }

                statusControl?.Hide();
                statusControl = new StatusControl(activePlc.PlcName)
                {
                    Parent = contentPanel,
                    Dock = DockStyle.Fill
                };
                settingsControl?.Hide();
                settingsControl = null;
                tagControl?.Hide();
                tagControl = null;
                statusControl?.Show();
                break;

            case "Taggar":
                var plcNameTags = treeView.SelectedNode.Parent?.Text;
                activePlc = PlcConfig.PlcList.Find(x => x.PlcName == plcNameTags);

                if (activePlc == null)
                {
                    return;
                }

                tagControl?.Hide();
                tagControl = new TagControl2(activePlc, treeView)
                {
                    Parent = contentPanel,
                    Dock = DockStyle.Fill
                };
                settingsControl?.Hide();
                settingsControl = null;
                statusControl?.Hide();
                statusControl = null;
                tagControl?.Show();
                break;

            default:
                settingsControl?.Hide();
                settingsControl = null;
                statusControl?.Hide();
                statusControl = null;
                tagControl?.Hide();
                tagControl = null;
                break;
        }
    }

    #endregion

    #region Events
    public void Subscribe(bool subscribe)
    {
        if (subscribe)
        {
            RedrawTreeEvent.NewMessage += RedrawTreeEvent_NewMessage;
            OnlineStatusEvent.NewMessage += OnlineStatusEvent_NewMessage;
            Logger.NewLog += Instance_NewLog;
            messageTimer.Tick += MessageTimer_Tick;
            statusTimer.Tick += StatusTimer_Tick;
            startStopButtonVisibilityTimer.Tick += StartStopButtonVisibilityTimer_Tick;
            DatabaseCreationNotifier.DatabaseCreated += DatabaseCreationEvent_CreatedEvent;
            databaseTimer.Tick += DatabaseTimer_Tick;
            LoggerHandler.RestartEvent += Logger_RestartEvent;
            this.Resize += MasterForm_Resize;
        }
        else
        {
            RedrawTreeEvent.NewMessage -= RedrawTreeEvent_NewMessage;
            OnlineStatusEvent.NewMessage -= OnlineStatusEvent_NewMessage;
            Logger.NewLog -= Instance_NewLog;
            statusTimer.Tick -= StatusTimer_Tick;
            messageTimer.Tick -= MessageTimer_Tick;
            startStopButtonVisibilityTimer.Tick -= StartStopButtonVisibilityTimer_Tick;
            DatabaseCreationNotifier.DatabaseCreated -= DatabaseCreationEvent_CreatedEvent;
            LoggerHandler.RestartEvent -= Logger_RestartEvent;
            this.Resize -= MasterForm_Resize;
        }

        isSubscribed = subscribe;
    }

    private void Instance_NewLog(LogTemplate obj)
    {
        if (InvokeRequired)
        {
            Invoke((MethodInvoker)delegate { Instance_NewLog(obj); });
            return;
        }

        messageQueue.Enqueue(obj);
        ShowNextMessage();
    }
    private void ShowNextMessage()
    {
        if (!messageTimer.Enabled && messageQueue.Count > 0)
        {
            var obj = messageQueue.Dequeue();

            statusPanel.AutoScroll = obj.ToHmiString().Length > 100;
            txtStatus.Text = obj.ToHmiString(true);
            //txtStatus.Text = obj.Exception != null ? obj.Exception.ToString() : obj.ToHmiString();
            switch (obj.Severity)
            {
                case Severity.Warning:
                    lblStatus.ForeColor = UIColors.WarningColor;
                    lblStatus.Text = "Warning";
                    break;
                case Severity.Error:
                    lblStatus.ForeColor = UIColors.ErrorColor;
                    lblStatus.Text = "Error";
                    break;
                case Severity.Information:
                    lblStatus.ForeColor = UIColors.GreyVeryLightColor;
                    lblStatus.Text = "Information";
                    break;
                case Severity.Success:
                    lblStatus.ForeColor = UIColors.SuccessColor;
                    lblStatus.Text = "Success";
                    break;
                default:
                    lblStatus.ForeColor = UIColors.GreyVeryLightColor;
                    break;
            }

            var labelsInControl = statusPanel.Controls;
            messageTimer.Start();

            if (obj.Severity == Severity.Error || obj.Severity == Severity.Warning)
            {
                var tiden = DateTime.UtcNow + LoggerHandler.UtcOffset;
                var icon = obj.Severity == Severity.Error ? ToolTipIcon.Error : ToolTipIcon.Warning;
                if (Properties.Settings.Default.notify)
                {
                    notifyIcon.ShowBalloonTip(3000, $"OptimaValue {tiden.ToShortDateString()} {tiden.ToShortTimeString()}", obj.ToHmiString(), icon);
                }
                errorImage.Visible = true;
            }
        }
        if (txtStatus.Text.Length == 0)
        {
            lblStatus.ForeColor = UIColors.GreyVeryLightColor;
            lblStatus.Text = "Status";
        }
    }

    private void MessageTimer_Tick(object sender, EventArgs e)
    {
        messageTimer.Stop();
        txtStatus.Text = string.Empty;
        errorImage.Visible = false;
        statusPanel.AutoScroll = false;

        // If there are more messages in the queue, show the next one
        ShowNextMessage();
    }

    private void DatabaseTimer_Tick(object sender, EventArgs e)
    {
        if (!DatabaseStatus.isConnected)
        {
            databaseImage.Visible = !databaseImage.Visible;
            databaseImage.Image = noDatabase;
        }
        else
        {
            databaseImage.Visible = true;
            databaseImage.Image = okDatabase;

        }

        toolTip.SetToolTip(databaseImage, databaseTooltip);
    }

    private void MasterForm_Resize(object sender, EventArgs e)
    {
        if (this.WindowState == FormWindowState.Minimized)
        {
            notifyIcon.ShowBalloonTip(5000, "OptimaValue körs i bakgrunden", "Se notify-ikon...", ToolTipIcon.None);
            this.Hide();
        }
    }

    private void Logger_RestartEvent(object sender, EventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke((MethodInvoker)delegate { Logger_RestartEvent(sender, e); });
            return;
        }
        btnStart_Click(this, EventArgs.Empty);
    }

    private void DatabaseCreationEvent_CreatedEvent(object sender, DatabaseCreationEventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke((MethodInvoker)delegate { DatabaseCreationEvent_CreatedEvent(sender, e); });
            return;
        }
        if (e.Created)
            $"Skapade ny databas {Config.Settings.Databas}".SendStatusMessage();
        else
            $"Sparade inställningarna till SQL".SendStatusMessage();

        PlcConfig.PopulateDataTable();
        Application.UseWaitCursor = false;
        databaseImage.Image = okDatabase;
    }

    private void StartStopButtonVisibilityTimer_Tick(object sender, EventArgs e)
    {
        bool anyActivePlcs = PlcConfig.PlcList?.Any(x => x.LoggerIsStarted) ?? false;

        if (anyActivePlcs)
        {
            if (btnStart.Visible)
            {
                btnStart.Visible = false;
            }
            if (menuSettings.Enabled)
            {
                menuSettings.Enabled = false;
            }
            if (!btnStop.Visible)
            {
                btnStop.Visible = true;
            }
            if (treeView.TopNode.ImageIndex != 7)
            {
                treeView.TopNode.ImageIndex = 7;
                treeView.TopNode.SelectedImageIndex = 7;
            }
            if (notifyIcon.Icon != Resources.icons8_gas_running)
            {
                notifyIcon.Icon = Resources.icons8_gas_running;
            }
            if (notifyIcon.Text != "Ansluten")
            {
                notifyIcon.Text = "Ansluten";
            }
        }
        else
        {
            if (!btnStart.Visible)
            {
                btnStart.Visible = true;
            }
            if (!menuSettings.Enabled)
            {
                menuSettings.Enabled = true;
            }
            if (btnStop.Visible)
            {
                btnStop.Visible = false;
            }
            if (treeView.TopNode.ImageIndex != 0)
            {
                treeView.TopNode.ImageIndex = 0;
                treeView.TopNode.SelectedImageIndex = 0;
            }
            if (notifyIcon.Icon != Resources.icons8_gas_idle)
            {
                notifyIcon.Icon = Resources.icons8_gas_idle;
            }
            if (notifyIcon.Text != "Ej ansluten")
            {
                notifyIcon.Text = "Ej ansluten";
            }
        }

    }

    private void StatusTimer_Tick(object sender, EventArgs e)
    {
        statusTimer.Stop();
        txtStatus.Text = string.Empty;
        errorImage.Visible = false;
        statusPanel.AutoScroll = false;
    }

    private bool isConnected = false;
    private void OnlineStatusEvent_NewMessage(object sender, OnlineStatusEventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke((MethodInvoker)delegate { OnlineStatusEvent_NewMessage(sender, e); });
            return;
        }
        var treeNodeCollection = treeView.Nodes.Find("PLC", true);
        var treeNode = treeNodeCollection.First(n => n.Text == e.PlcName);
        switch (e.ConnectionStatus)
        {
            case ConnectionStatus.Disconnected:
                if (treeNode.ImageIndex != 1)
                {
                    treeNode.ImageIndex = 1;
                    treeNode.SelectedImageIndex = 1;
                }
                isConnected = false;
                comboTrend.Visible = false;
                //ClearTrendTags();
                btnStartTrend.Visible = false;
                break;
            case ConnectionStatus.Connecting:
                break;
            case ConnectionStatus.Connected:
                if (treeNode.ImageIndex != 2)
                {
                    treeNode.ImageIndex = 2;
                    treeNode.SelectedImageIndex = 2;
                }
                comboTrend.Visible = true;
                if (comboTrend.Items.Count == 0)
                    AddTrendTags();
                isConnected = true;
                btnStartTrend.Visible = true;
                break;
            case ConnectionStatus.Disconnecting:
                break;
            default:
                break;
        }
    }

    private void RedrawTreeEvent_NewMessage(object sender, RedrawTreeEventArgs e)
    {
        PopulateTree();
        CloseUserControls();
    }
    #endregion


    private void SqlForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        IsOpenSqlForm = false;
        if (SqlForm.DatabaseCreated == true || DatabaseStatus.isConnected)
        {
            //databaseImage.Image = okDatabase;
            PopulateTree();
        }

        //else
        //    databaseImage.Image = noDatabase;
    }

    private void PerForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        perFormOpen = false;
        perForm.Dispose();
    }

    private void notifyMenu_CheckedChanged(object sender, EventArgs e)
    {
        Properties.Settings.Default.notify = notifyMenu.Checked;
        Properties.Settings.Default.Save();
    }

    private void autoStartTool_CheckedChanged(object sender, EventArgs e)
    {
        if (autoStartTool.Checked != Properties.Settings.Default.AutoStart)
        {
            Properties.Settings.Default.AutoStart = autoStartTool.Checked;
            Properties.Settings.Default.Save();
            if (autoStartTool.Checked)
            {
                WshShell wshShell = new WshShell();
                IWshRuntimeLibrary.IWshShortcut shortcut;
                string startUpFolderPath =
                  Environment.GetFolderPath(Environment.SpecialFolder.Startup);

                // Create the shortcut
                shortcut =
                  (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(
                    startUpFolderPath + "\\" +
                    Application.ProductName + ".lnk");

                shortcut.TargetPath = Application.ExecutablePath;
                shortcut.WorkingDirectory = Application.StartupPath;
                shortcut.Description = "Launch My Application";
                // shortcut.IconLocation = Application.StartupPath + @"\App.ico";
                shortcut.Save();
            }
            else
            {
                string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                string fileName = startupFolderPath + "\\" +
                    Application.ProductName + ".lnk";

                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                }
            }
        }
    }

    private void startaTrendToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Config.Settings.Load();
        if (!Config.Settings.IsTrendRunning)
            Process.Start(OptimaValue.Config.Settings.OptimaValueWpfFilePath);
    }

    private void comboTrend_TextChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(comboTrend.Text))
            return;

        if (stringCollection.Contains(comboTrend.Text))
        {
            btnStartTrend.Visible = true;
        }
        else
        {
            btnStartTrend.Visible = false;
        }
    }

    private void btnStartTrend_Click(object sender, EventArgs e)
    {
        //var tagId = AvailableTagsTrend.Where(x => x.Name == comboTrend.Text).Select(x => x.Id).First();
        var tagId = TagIds.Where(x => x.Id == (int)comboTrend.ComboBox.SelectedValue
            ).First();
        var trendForm = new TrendTag(tagId.Id);
        try
        {
            trendForm.Show();

        }
        catch (Exception ex)
        {
            Logger.LogError("", ex);
        }
    }

    private bool serviceFormOpen = false;
    private void serviceToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (!IsUserAdministrator())
        {
            MessageBox.Show("Du måste köra programmet som administratör för att kunna installera tjänsten","Fel",MessageBoxButtons.OK,MessageBoxIcon.Error);
            return;
        }
        if (!serviceFormOpen)
        {
            var serviceForm = new ServiceInstallerAppForm();
            serviceForm.FormClosed += (s, e) =>
            {
                serviceFormOpen = false;
                serviceImage.Visible = ServiceInstallerAppForm.ServiceExists();
            };
            serviceForm.Show();
        }
    }

    private bool IsUserAdministrator()
    {
        // Kolla om användaren är administratör
        WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
  


}
