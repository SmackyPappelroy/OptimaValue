using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OptimaValue
{
    public partial class sqlForm : Form
    {
        #region Form
        public sqlForm()
        {
            InitializeComponent();
            btnSave.Enabled = false;
        }
        private void SettingForm_Load(object sender, EventArgs e)
        {
            txtDatabas.Text = SqlSettings.Default.Databas;
            txtServer.Text = SqlSettings.Default.Server;
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
        private void btnSave_Click(object sender, EventArgs e)
        {
            Application.UseWaitCursor = true;
            btnSave.Enabled = false;
            SqlSettings.Default.User = txtUser.Text;
            SqlSettings.Default.Password = txtPassword.Text;
            SqlSettings.Default.Server = txtServer.Text;
            SqlSettings.Default.Databas = txtDatabas.Text;

            btnSave.Enabled = false;
            SqlSettings.Default.Save();

            var created = Task.Run(contextInstance.CreateDb);
        }

        private void DatabaseCreationEvent_CreatedEvent(object sender, DataBaseCreationEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { DatabaseCreationEvent_CreatedEvent(sender, e); });
                return;
            }

            btnSave.Enabled = true;
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
            if (!string.IsNullOrWhiteSpace(txtPassword.Text)
                && !string.IsNullOrWhiteSpace(txtUser.Text)
                && !string.IsNullOrWhiteSpace(txtServer.Text)
                && !string.IsNullOrWhiteSpace(txtDatabas.Text))
            {
                return true;
            }
            else
                return false;
        }
        #endregion
    }
}
