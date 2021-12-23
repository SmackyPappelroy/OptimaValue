using Microsoft.SqlServer.Management.Smo;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            comboServer.Text = SqlSettings.Default.Server != string.Empty ? SqlSettings.Default.Server : string.Empty;
            if (comboServer.Text != string.Empty)
            {
                comboServer.Items.Clear();
                comboServer.Items.Add(comboServer.Text);
                comboServer.SelectedIndex = 0;
            }
            txtDatabas.Text = SqlSettings.Default.Databas;
            txtUser.Text = SqlSettings.Default.User;
            txtPassword.Text = SqlSettings.Default.Password;
            DatabaseCreationEvent.CreatedEvent += DatabaseCreationEvent_CreatedEvent;
        }

        private void SettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            PlcConfig.sqlSettingsFormOpen = false;
            DatabaseCreationEvent.CreatedEvent -= DatabaseCreationEvent_CreatedEvent;
        }
        #endregion

        #region Winform user control events
        private async void btnSave_Click(object sender, EventArgs e)
        {
            this.ControlBox = false;
            Application.UseWaitCursor = true;
            btnSave.Enabled = false;
            SqlSettings.Default.User = txtUser.Text;
            SqlSettings.Default.Password = txtPassword.Text;
            SqlSettings.Default.Server = comboServer.SelectedItem.ToString();
            SqlSettings.Default.Databas = txtDatabas.Text;
            SqlSettings.Default.ConnectionString = ($"Server={@SqlSettings.Default.Server};Database={SqlSettings.Default.Databas};User Id={SqlSettings.Default.User};Password={SqlSettings.Default.Password}; ");
            SqlSettings.Default.Save();

            string serverString = ($"Server={@SqlSettings.Default.Server};Database=master;User Id={SqlSettings.Default.User};Password={SqlSettings.Default.Password}; ");

            if (!await DatabaseSql.TestConnectionAsync(1000, serverString))
            {
                btnSave.Enabled = true;
                this.ControlBox = true;
                Application.UseWaitCursor = false;
                "Ingen anslutning till SQL-server".SendStatusMessage(Severity.Warning);
                return;
            }

            btnSave.Enabled = false;

            var result = await DatabaseSql.TestConnectionAsync();
            if (!result)
            {
                $"Misslyckades att ansluta med följande Connection-sträng: {DatabaseSql.ConnectionString}".SendStatusMessage(Severity.Error);
                btnSave.Enabled = true;
                //}
                //else
                //{
                try
                {
                    var created = await contextInstance.CreateDb();
                    var result1 = await DatabaseSql.TestConnectionAsync();
                    if (result1)
                    {
                        "Skapade databas".SendStatusMessage(Severity.Success);
                    }
                }
                catch (Exception)
                {
                    "Misslyckades att skapa databas".SendStatusMessage(Severity.Error);
                }
            }
            Application.UseWaitCursor = false;
            btnSave.Enabled = true;
            this.ControlBox = true;
        }

        private void DatabaseCreationEvent_CreatedEvent(object sender, DataBaseCreationEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { DatabaseCreationEvent_CreatedEvent(sender, e); });
                return;
            }

            btnSave.Enabled = true;
            DatabaseCreated = true;
            DatabaseCreationEvent.CreatedEvent -= DatabaseCreationEvent_CreatedEvent;
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
