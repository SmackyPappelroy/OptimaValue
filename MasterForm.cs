using IWshRuntimeLibrary;
using OptimaValue.Handler.PLC.Graphics;
using OptimaValue.Handler.PLC.MyPlc.Graphics;
using OptimaValue.Properties;
using S7.Net;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OptimaValue
{
    public partial class MasterForm : Form
    {
        #region Properties
        private ExtendedPlc activePlc;
        private PlcSettingsControl settingsControl;
        private StatusControl statusControl;
        private TagControl2 tagControl;
        private PerformanceForm perForm;
        private bool perFormOpen = false;
        private readonly System.Windows.Forms.Timer statusTimer;
        private readonly System.Windows.Forms.Timer databaseTimer;
        private readonly System.Windows.Forms.Timer startStopButtonVisibilityTimer;
        private sqlForm SqlForm;
        private bool IsOpenSqlForm = false;

        private bool isSubscribed = false;

        private Image noDatabase;
        private Image okDatabase;

        private string databaseTooltip => databaseImage.Image == okDatabase ? "Ansluten till SQL-server" : "Ingen anslutning till SQL";
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
            debugMenu.Checked = Settings.Default.Debug;
            notifyIcon.Visible = true;
            notifyIcon.Icon = Resources.icons8_gas_idle;

            menuStrip.BackColor = UIColors.ForeGroundLayer1;
            menuQuestion.ForeColor = UIColors.HeaderText;
            menuSettings.ForeColor = UIColors.HeaderText;
            menuSettings.KeepOpenOnDropdownCheck();

            menuSettings.ChangeForeColorMenuItem(Color.Black, UIColors.HeaderText);
            menuQuestion.MouseHoverMenuItem(Color.Black, UIColors.HeaderText);
        }
        #endregion

        #region Form
        private async void MasterForm_Load(object sender, EventArgs e)
        {
            noDatabase = databaseImageList.Images[0];
            okDatabase = databaseImageList.Images[1];

            btnStop.Visible = false;
            btnStart.Visible = false;
            startStopButtonVisibilityTimer.Start();
            notifyMenu.Checked = Settings.Default.notify;
            autoStartTool.Checked = Settings.Default.AutoStart;


            var result = await DatabaseSql.TestConnectionAsync();
            if (!result)
            {
                txtStatus.Text = "Misslyckades att ansluta till Sql";
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

        private void debugMeny_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Debug = debugMenu.Checked;
            Settings.Default.Save();
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
            // Close all but Masterform
            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {
                if (Application.OpenForms[i].Name != "MasterForm")
                    Application.OpenForms[i].Close();
            }

            if (!await Master.StartLog())
                btnStart.Visible = true;

            if (settingsControl != null)
            {
                if (settingsControl.Visible == true)
                {
                    settingsControl.Hide();
                    settingsControl = null;
                }
            }

            if (statusControl != null)
            {
                if (statusControl.Visible == true)
                {
                    statusControl.Hide();
                    statusControl = null;
                }
            }
            if (tagControl != null)
            {
                if (tagControl.Visible == true)
                {
                    tagControl.Hide();
                    tagControl = null;
                }
            }

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
                SqlForm.BringToFront();
            else
            {
                SqlForm = null;
                SqlForm = new sqlForm();
                SqlForm.FormClosing += SqlForm_FormClosing;
                SqlForm.Show();
                IsOpenSqlForm = true;
            }
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
            if (treeView.SelectedNode.Name == "Konfiguration")
            {
                activePlc = PlcConfig.PlcList.Find(x => x.PlcName == treeView.SelectedNode.Parent.Text);

                if (settingsControl != null)
                {
                    settingsControl.Hide();
                }
                settingsControl = null;

                // Plcn finns ej i databas
                if (activePlc == null)
                {
                    settingsControl = new PlcSettingsControl(ConnectionStatus.Disconnected, string.Empty
                                      , string.Empty, 1.ToString(), 0.ToString()
                                      , false, CpuType.S71500, 0, activePlc)
                    {
                        Parent = contentPanel
                    };
                    settingsControl.Dock = DockStyle.Fill;
                }
                // Plcn finns i databas
                else
                {
                    settingsControl = new PlcSettingsControl(activePlc.ConnectionStatus, activePlc.PlcName
                  , activePlc.IP, activePlc.Slot.ToString(), activePlc.Rack.ToString()
                  , activePlc.Active, activePlc.CPU, activePlc.ActivePlcId, activePlc)
                    {
                        Parent = contentPanel
                    };
                    settingsControl.Dock = DockStyle.Fill;
                }
                if (activePlc != null)
                {
                    settingsControl.Show();
                }
                else if (activePlc == null)
                    settingsControl.Show();

            }

            if (treeView.SelectedNode.Name != "Konfiguration")
            {
                if (settingsControl != null)
                    settingsControl.Hide();
                settingsControl = null;
            }

            if (treeView.SelectedNode.Name == "Status")
            {
                activePlc = PlcConfig.PlcList.Find(x => x.PlcName == treeView.SelectedNode.Parent.Text);

                if (activePlc == null)
                    return;

                if (statusControl != null)
                    statusControl.Hide();
                statusControl = null;

                statusControl = new StatusControl(activePlc.PlcName)
                {
                    Parent = contentPanel,
                    Dock = DockStyle.Fill
                };
                statusControl.Show();
            }

            if (treeView.SelectedNode.Name != "Status")
            {
                if (statusControl != null)
                    statusControl.Hide();
                statusControl = null;
            }

            if (treeView.SelectedNode.Name == "Taggar")
            {
                activePlc = PlcConfig.PlcList.Find(x => x.PlcName == treeView.SelectedNode.Parent.Text);

                if (activePlc == null)
                    return;

                if (tagControl != null)
                    tagControl.Hide();
                tagControl = null;

                tagControl = new TagControl2(activePlc, treeView)
                {
                    Parent = contentPanel,
                    Dock = DockStyle.Fill
                };
                tagControl.Show();


            }
            if (treeView.SelectedNode.Name != "Taggar")
            {
                if (tagControl != null)
                    tagControl.Hide();
                tagControl = null;
            }

        }
        #endregion

        #region Events
        public void Subscribe(bool subscribe)
        {
            if (!isSubscribed && subscribe)
            {
                RedrawTreeEvent.NewMessage += RedrawTreeEvent_NewMessage;
                OnlineStatusEvent.NewMessage += OnlineStatusEvent_NewMessage;
                Apps.Logger.NewLog += Logger_NewLog;
                statusTimer.Tick += StatusTimer_Tick;
                startStopButtonVisibilityTimer.Tick += StartStopButtonVisibilityTimer_Tick;
                DatabaseCreationEvent.CreatedEvent += DatabaseCreationEvent_CreatedEvent;
                databaseTimer.Tick += DatabaseTimer_Tick;
                Logger.RestartEvent += Logger_RestartEvent;
                this.Resize += MasterForm_Resize;
                isSubscribed = true;
            }
            else if (isSubscribed && !subscribe)
            {
                RedrawTreeEvent.NewMessage -= RedrawTreeEvent_NewMessage;
                OnlineStatusEvent.NewMessage -= OnlineStatusEvent_NewMessage;
                Apps.Logger.NewLog -= Logger_NewLog;
                statusTimer.Tick -= StatusTimer_Tick;
                startStopButtonVisibilityTimer.Tick -= StartStopButtonVisibilityTimer_Tick;
                DatabaseCreationEvent.CreatedEvent -= DatabaseCreationEvent_CreatedEvent;
                Logger.RestartEvent -= Logger_RestartEvent;


                isSubscribed = false;
            }
        }

        private void DatabaseTimer_Tick(object sender, EventArgs e)
        {
            if (!DatabaseStatus.isConnected)
                databaseImage.Visible = !databaseImage.Visible;
            else
                databaseImage.Visible = true;

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

        private void DatabaseCreationEvent_CreatedEvent(object sender, DataBaseCreationEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { DatabaseCreationEvent_CreatedEvent(sender, e); });
                return;
            }
            if (e.Created)
                $"Skapade ny databas {SqlSettings.Default.Databas}".SendThisStatusMessage();
            else
                $"Sparade inställningarna till SQL".SendThisStatusMessage();

            PlcConfig.PopulateDataTable();
            Application.UseWaitCursor = false;
            databaseImage.Image = okDatabase;
        }

        private void StartStopButtonVisibilityTimer_Tick(object sender, EventArgs e)
        {
            var anyActivePlcs = false;
            if (PlcConfig.PlcList == null || PlcConfig.PlcList.Count == 0)
            {
                btnStart.Visible = false;
                btnStop.Visible = false;
                return;
            }
            foreach (ExtendedPlc myPlc in PlcConfig.PlcList)
            {
                if (myPlc.ConnectionStatus == ConnectionStatus.Connected
                    || myPlc.LoggerIsStarted)
                    anyActivePlcs = true;
            }
            if (anyActivePlcs)
            {
                if (btnStart.Visible != false)
                    btnStart.Visible = false;
                if (menuSettings.Enabled != false)
                    menuSettings.Enabled = false;
                if (btnStop.Visible != true)
                    btnStop.Visible = true;
                if (treeView.TopNode.ImageIndex != 7)
                {
                    treeView.TopNode.ImageIndex = 7;
                    treeView.TopNode.SelectedImageIndex = 7;
                }
                if (notifyIcon.Icon != Resources.icons8_gas_running)
                    notifyIcon.Icon = Resources.icons8_gas_running;
                if (notifyIcon.Text != "Ansluten")
                    notifyIcon.Text = "Ansluten";
            }
            else
            {
                if (btnStop.Visible != false)
                    btnStop.Visible = false;
                if (menuSettings.Enabled != true)
                    menuSettings.Enabled = true;
                if (btnStart.Visible != true)
                    btnStart.Visible = true;
                if (treeView.TopNode.ImageIndex != 0)
                {
                    treeView.TopNode.ImageIndex = 0;
                    treeView.TopNode.SelectedImageIndex = 0;
                }
                if (notifyIcon.Icon != Resources.icons8_gas_idle)
                    notifyIcon.Icon = Resources.icons8_gas_idle;
                if (notifyIcon.Text != "Ej ansluten")
                    notifyIcon.Text = "Ej ansluten";
            }
        }

        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            statusTimer.Stop();
            txtStatus.Text = string.Empty;
            errorImage.Visible = false;
            statusPanel.AutoScroll = false;
        }

        private void Logger_NewLog((string message, string hmiString, Severity LogSeverity, DateTime Tid, bool LogSuccess, Exception exception) obj)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { Logger_NewLog(obj); });
                return;
            }
            if (obj.exception != null)
            {
                statusPanel.AutoScroll = true;
                txtStatus.Text = obj.exception.ToString();
            }
            else
            {
                txtStatus.Text = obj.hmiString;
            }

            txtStatus.Visible = true;
            statusTimer.Start();

            if (obj.LogSeverity == Severity.Error)
            {
                var tiden = DateTime.UtcNow + Logger.UtcOffset;
                if (Settings.Default.notify)
                    notifyIcon.ShowBalloonTip(3000, $"OptimaValue {tiden.ToShortDateString()} {tiden.ToShortTimeString()}", obj.hmiString, ToolTipIcon.Error);
                errorImage.Visible = true;
            }
            else if (obj.LogSeverity == Severity.Warning)
            {
                var tiden = DateTime.UtcNow + Logger.UtcOffset;
                if (Settings.Default.notify)
                    notifyIcon.ShowBalloonTip(3000, $"OptimaValue {tiden.ToShortDateString()} {tiden.ToShortTimeString()}", obj.hmiString, ToolTipIcon.Warning);
                errorImage.Visible = true;
            }
        }


        private void OnlineStatusEvent_NewMessage(object sender, OnlineStatusEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { OnlineStatusEvent_NewMessage(sender, e); });
                return;
            }
            var treeNodeCollection = treeView.Nodes.Find("PLC", true);
            var treeNode = treeNodeCollection.First(n => n.Text == e.PlcName);
            switch (e.connectionStatus)
            {
                case ConnectionStatus.Disconnected:
                    if (treeNode.ImageIndex != 1)
                    {
                        treeNode.ImageIndex = 1;
                        treeNode.SelectedImageIndex = 1;
                    }
                    break;
                case ConnectionStatus.Connecting:
                    break;
                case ConnectionStatus.Connected:
                    if (treeNode.ImageIndex != 2)
                    {
                        treeNode.ImageIndex = 2;
                        treeNode.SelectedImageIndex = 2;
                    }
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
            if (SqlForm.DatabaseCreated == true)
            {
                databaseImage.Image = okDatabase;
                PopulateTree();
            }
            else
                databaseImage.Image = noDatabase;
        }

        private void PerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            perFormOpen = false;
            perForm.Dispose();
        }

        private void notifyMenu_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.notify = notifyMenu.Checked;
            Settings.Default.Save();
        }

        private void autoStartTool_CheckedChanged(object sender, EventArgs e)
        {
            if (autoStartTool.Checked != Settings.Default.AutoStart)
            {
                Settings.Default.AutoStart = autoStartTool.Checked;
                Settings.Default.Save();
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
    }
}
