using Microsoft.SqlServer.Management.Smo;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using OptimaValue.Config;
using FileLogger;
using Settings = OptimaValue.Config.Settings;

namespace OptimaValue
{
    public partial class sqlForm : Form
    {
        public bool DatabaseCreated = false;
        #region Form
        public sqlForm()
        {
            InitializeComponent();
            btnSave.Enabled = false;
        }
        private void SettingForm_Load(object sender, EventArgs e)
        {
            comboServer.Text = Config.Settings.Server != string.Empty ? Config.Settings.Server : string.Empty;
            if (comboServer.Text != string.Empty)
            {
                comboServer.Items.Clear();
                comboServer.Items.Add(comboServer.Text);
                comboServer.SelectedIndex = 0;
            }
            txtDatabas.Text = Config.Settings.Databas;
            txtUser.Text = Config.Settings.User;
            txtPassword.Text = Config.Settings.Password;
            DatabaseCreationNotifier.DatabaseCreated += DatabaseCreationEvent_CreatedEvent;
        }

        private void SettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            PlcConfig.sqlSettingsFormOpen = false;
            DatabaseCreationNotifier.DatabaseCreated -= DatabaseCreationEvent_CreatedEvent;
        }
        #endregion

        #region Winform user control events
        private async void btnSave_Click(object sender, EventArgs e)
        {
            this.ControlBox = false;
            Application.UseWaitCursor = true;
            btnSave.Enabled = false;
            Config.Settings.User = txtUser.Text;
            Config.Settings.Password = txtPassword.Text;
            Config.Settings.Server = comboServer.SelectedItem.ToString();
            Config.Settings.Databas = txtDatabas.Text;

            string serverString = ($"Server={Config.Settings.Server};Database=master;User Id={Config.Settings.User};Password={Config.Settings.Password}; ");

            if (!await DatabaseSql.TestConnectionAsync(1000, serverString))
            {
                btnSave.Enabled = true;
                this.ControlBox = true;
                Application.UseWaitCursor = false;
                "Ingen anslutning till SQL-server".SendStatusMessage(Severity.Warning);
                return;
            }

            btnSave.Enabled = false;

            var result = await DatabaseSql.TestConnectionAsync() && DatabaseSql.TableExist();
            if (!result)
            {
                $"Misslyckades att ansluta med följande Connection-sträng: {Settings.ConnectionString}".SendStatusMessage(Severity.Error);
                btnSave.Enabled = true;
                //}
                //else
                //{
                try
                {
                    var created = await contextInstance.CreateDbAsync();
                    var result1 = await DatabaseSql.TestConnectionAsync();
                    if (result1)
                    {
                        "Skapade databas".SendStatusMessage(Severity.Success);
                        Application.UseWaitCursor = false;
                        this.Close();
                    }
                }
                catch (Exception)
                {
                    "Misslyckades att skapa databas".SendStatusMessage(Severity.Error);
                }
            }
            else
            {
                Application.UseWaitCursor = false;
                "Ansluten till databas".SendStatusMessage(Severity.Success);
                this.Close();
            }
            Application.UseWaitCursor = false;
            btnSave.Enabled = true;
            this.ControlBox = true;
        }

        private void DatabaseCreationEvent_CreatedEvent(object sender, DatabaseCreationEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { DatabaseCreationEvent_CreatedEvent(sender, e); });
                return;
            }

            btnSave.Enabled = true;
            DatabaseCreated = true;
            DatabaseCreationNotifier.DatabaseCreated -= DatabaseCreationEvent_CreatedEvent;
            Program.LoggerInstance =  Program.CreateFileLogger();
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = validateInputs();
        }

        private void txtUser_TextChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = validateInputs();
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = validateInputs();
        }
        #endregion

        #region Methods
        private bool validateInputs()
        {
            if (comboServer.Items.Count == 0)
            {
                comboServer.Items.Add(string.Empty);
                comboServer.SelectedItem = comboServer.Items[0];
            }

            if (!string.IsNullOrWhiteSpace(txtPassword.Text)
                && !string.IsNullOrWhiteSpace(txtUser.Text)
                  && !string.IsNullOrWhiteSpace(comboServer.SelectedItem.ToString())
                && !string.IsNullOrWhiteSpace(txtDatabas.Text))
            {
                return true;
            }
            else
                return false;
        }
        #endregion

        private void btnSearchServer_Click(object sender, EventArgs e)
        {
            if (OperatingSystem.IsWindows())
            {
                List<string> list = new();
                string ServerName = Environment.MachineName;
                RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
                using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
                {
                    RegistryKey instanceKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);
                    if (instanceKey != null)
                    {
                        foreach (var instanceName in instanceKey.GetValueNames())
                        {
                            list.Add(ServerName + "\\" + instanceName);
                        }
                    }
                }
                if (list.Count > 0)
                {
                    list.Sort();
                    comboServer.Items.Clear();
                    foreach (var item in list)
                    {
                        comboServer.Items.Add(item);
                    }
                    comboServer.SelectedItem = comboServer.Items[0];
                    btnSave.Enabled = true;
                }
            }
        }

        private void comboServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboServer.Items.Count == 0 || comboServer.SelectedItem == null)
                return;

            if (string.IsNullOrEmpty(comboServer.SelectedItem.ToString()))
                return;

            validateInputs();
        }

        private void comboServer_Validated(object sender, EventArgs e)
        {
            if (comboServer.Items.Count == 0)
                return;

            if (string.IsNullOrEmpty(comboServer.Text))
                return;

            comboServer.Items.Clear();
            comboServer.Items.Add(comboServer.Text);
            comboServer.SelectedItem = comboServer.Items[0];

            bool isValidated = validateInputs();
            if (!isValidated)
                comboServer.Items.Clear();
            else
                btnSave.Enabled = true;
        }


    }
}
