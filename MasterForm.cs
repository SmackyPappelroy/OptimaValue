using OptimaValue.Handler.PLC.Graphics;
using OptimaValue.Handler.PLC.MyPlc.Graphics;
using OptimaValue.Properties;
using S7.Net;
using System;
using System.Drawing;
using System.Linq;
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
        private System.Windows.Forms.Timer statusTimer;
        private System.Windows.Forms.Timer startStopButtonVisibilityTimer;
        private sqlForm SqlForm;
        private bool IsOpenSqlForm = false;
        private MyLogger myLogger = new MyLogger();

        private bool isSubscribed = false;
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
            startStopButtonVisibilityTimer.Start();
            Subscribe(true);

            SqlForm = new sqlForm();
            SqlForm.FormClosing += SqlForm_FormClosing;
            debugMenu.Checked = Settings.Default.Debug;
            notifyIcon.Visible = true;
            notifyIcon.Icon = Resources.icons8_gas_idle;

            menuStrip.BackColor = UIColors.ForeGroundLayer1;
            menuQuestion.ForeColor = UIColors.HeaderText;
            menuSettings.ForeColor = UIColors.HeaderText;
            hideMenu.ForeColor = UIColors.HeaderText;
            menuSettings.KeepOpenOnDropdownCheck();

            menuSettings.ChangeForeColorMenuItem(Color.Black, UIColors.HeaderText);
            menuQuestion.MouseHoverMenuItem(Color.Black, UIColors.HeaderText);
            hideMenu.MouseHoverMenuItem(Color.Black, UIColors.HeaderText);
        }
        #endregion

        #region Form
        private async void MasterForm_Load(object sender, EventArgs e)
        {
            var result = await PlcConfig.TestConnectionSqlAsync();
            if (!result)
            {
                txtStatus.Text = "Misslyckades att ansluta till Sql";
            }
            PopulateTree();
        }

        private void MasterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Subscribe(false);
            Master.StopLog(true);
            startStopButtonVisibilityTimer.Stop();
            System.Threading.Thread.Sleep(100);
        }

        private void debugMeny_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Debug = debugMenu.Checked;
            Settings.Default.Save();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var assemblyVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
            notifyIcon.ShowBalloonTip(5000, $"OptimaValue \nCopyright © 2020 v{assemblyVersion}", "By Hans-Martin Nilsson", ToolTipIcon.None);
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

        private void btnStart_Click(object sender, EventArgs e)
        {
            Master.StartLog();
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

        #region Events
        public void Subscribe(bool subscribe)
        {
            if (!isSubscribed && subscribe)
            {
                RedrawTreeEvent.NewMessage += RedrawTreeEvent_NewMessage;
                OnlineStatusEvent.NewMessage += OnlineStatusEvent_NewMessage;
                StatusEvent.NewMessage += StatusEvent_NewMessage;
                statusTimer.Tick += StatusTimer_Tick;
                startStopButtonVisibilityTimer.Tick += StartStopButtonVisibilityTimer_Tick;
                DatabaseCreationEvent.CreatedEvent += DatabaseCreationEvent_CreatedEvent;

                isSubscribed = true;
            }
            else if (isSubscribed && !subscribe)
            {
                RedrawTreeEvent.NewMessage -= RedrawTreeEvent_NewMessage;
                OnlineStatusEvent.NewMessage -= OnlineStatusEvent_NewMessage;
                StatusEvent.NewMessage -= StatusEvent_NewMessage;
                statusTimer.Tick -= StatusTimer_Tick;
                startStopButtonVisibilityTimer.Tick -= StartStopButtonVisibilityTimer_Tick;
                DatabaseCreationEvent.CreatedEvent -= DatabaseCreationEvent_CreatedEvent;

                isSubscribed = false;
            }
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
        }

        private void StartStopButtonVisibilityTimer_Tick(object sender, EventArgs e)
        {
            var anyActivePlcs = false;
            if (PlcConfig.PlcList == null)
            {
                btnStart.Visible = true;
                btnStop.Visible = false;
                return;
            }
            foreach (ExtendedPlc plc in PlcConfig.PlcList)
            {
                if (plc.ConnectionStatus == ConnectionStatus.Connected
                    || plc.logger.IsStarted)
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
        }

        private void StatusEvent_NewMessage(object sender, StatusEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { StatusEvent_NewMessage(sender, e); });
                return;
            }
            if (e.Message != "")
            {
                if (txtStatus.Text != string.Empty)
                    txtStatus.Text += $"\n\r\n\r" + e.Message;
                else
                    txtStatus.Text = e.Message;
                txtStatus.Visible = true;
                statusTimer.Start();
            }

            if (e.Status == Status.Error)
            {
                notifyIcon.ShowBalloonTip(3000, $"OptimaValue {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}", e.Message, ToolTipIcon.Error);
                errorImage.Visible = true;
            }
            else if (e.Status == Status.Warning)
            {
                notifyIcon.ShowBalloonTip(3000, $"OptimaValue {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}", e.Message, ToolTipIcon.Warning);
                errorImage.Visible = true;
            }

            if (!Settings.Default.Debug)
                return;
            var log = new LogRow
            {
                status = e.Status,
                Message = e.Message
            };
            myLogger.LogMessage(log);
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



        private void PopulateTree()
        {
            foreach (TreeNode node in treeView.Nodes)
            {
                node.Nodes.Clear();
            }
            var tbl = PlcConfig.PopulateDataTable();
            if (tbl == null)
                return;
            if (tbl.Rows.Count == 0)
            {
                btnStart.Enabled = false;
            }
            else
                btnStart.Enabled = true;
            foreach (ExtendedPlc plc in PlcConfig.PlcList)
            {
                AddPlcNode(plc.PlcName);
            }
        }

        private void addPlc_Click(object sender, EventArgs e)
        {
            AddPlcNode("PLC");
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

        private void AddPlcNode(string plcLabel)
        {

            var configurationNode = new TreeNode("Konfiguration");
            configurationNode.Name = "Konfiguration";
            configurationNode.ImageIndex = 4;
            configurationNode.SelectedImageIndex = 4;

            var statusNode = new TreeNode("Status");
            statusNode.Name = "Status";
            statusNode.ImageIndex = 5;
            statusNode.SelectedImageIndex = 5;

            var tagNode = new TreeNode("Taggar");
            tagNode.Name = "Taggar";
            tagNode.ImageIndex = 6;
            tagNode.SelectedImageIndex = 6;

            var plcNode = new TreeNode(plcLabel, new TreeNode[] { configurationNode, statusNode, tagNode });
            plcNode.Name = "PLC";
            plcNode.ImageIndex = 1;
            plcNode.SelectedImageIndex = 1;

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
                // Plcn finn i databas
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

                statusControl = new StatusControl(activePlc.PlcName);
                statusControl.Parent = contentPanel;
                statusControl.Dock = DockStyle.Fill;
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

                tagControl = new TagControl2(activePlc, treeView);
                tagControl.Parent = contentPanel;
                tagControl.Dock = DockStyle.Fill;
                tagControl.Show();


            }
            if (treeView.SelectedNode.Name != "Taggar")
            {
                if (tagControl != null)
                    tagControl.Hide();
                tagControl = null;

            }
        }







        private void SqlForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsOpenSqlForm = false;
            PopulateTree();
        }



        private void PerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            perFormOpen = false;
            perForm.Dispose();
        }

        private void hideMenu_Click(object sender, EventArgs e)
        {
            Hide();
            notifyIcon.ShowBalloonTip(5000, "OptimaValue körs i bakgrunden", "Se notify-ikon...", ToolTipIcon.None);
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            if (!Visible)
                Show();
            else
                BringToFront();
        }
    }
}
