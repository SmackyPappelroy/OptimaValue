using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace OptimaValue.Handler.PLC.MyPlc.Graphics
{
    public partial class SyncPlcTimeForm : Form
    {
        private ExtendedPlc MyPlc;
        public event Action<bool> SavedEvent = (details) => { };

        #region Validation
        private bool aktuellDbOk;
        private bool aktuellOffsetOk;
        private bool syncDbOk;
        private bool syncOffsetOk;
        private bool syncBitOk;

        private void txtAktuellTidDb_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAktuellTidDb.Text))
            {
                errorProvider.SetError(txtAktuellTidDb, "Fältet får ej va tomt");
                aktuellDbOk = false;
            }
            else if (!uint.TryParse(txtAktuellTidDb.Text, out uint _))
            {
                errorProvider.SetError(txtAktuellTidDb, "Inte ett positivt heltal");
                aktuellDbOk = false;
            }
            else
                aktuellDbOk = true;
        }
        private void txtAktuellTidByte_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAktuellTidByte.Text))
            {
                errorProvider.SetError(txtAktuellTidByte, "Fältet får ej va tomt");
                aktuellOffsetOk = false;
            }
            else if (!uint.TryParse(txtAktuellTidByte.Text, out uint _))
            {
                errorProvider.SetError(txtAktuellTidByte, "Inte ett positivt heltal");
                aktuellOffsetOk = false;
            }
            else
                aktuellOffsetOk = true;
        }
        private void txtSynkDb_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSynkDb.Text))
            {
                errorProvider.SetError(txtSynkDb, "Fältet får ej va tomt");
                syncDbOk = false;
            }
            else if (!uint.TryParse(txtSynkDb.Text, out uint _))
            {
                errorProvider.SetError(txtSynkDb, "Inte ett positivt heltal");
                syncDbOk = false;
            }
            else
                syncDbOk = true;
        }
        private void txtSynkByte_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSynkByte.Text))
            {
                errorProvider.SetError(txtSynkByte, "Fältet får ej va tomt");
                syncOffsetOk = false;
            }
            else if (!uint.TryParse(txtSynkByte.Text, out uint _))
            {
                errorProvider.SetError(txtSynkByte, "Inte ett positivt heltal");
                syncOffsetOk = false;
            }
            else
                syncOffsetOk = true;
        }
        private void txtSyncBool_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSyncBool.Text))
            {
                errorProvider.SetError(txtSyncBool, "Fältet får ej va tomt");
                syncBitOk = false;
            }
            else
                syncBitOk = true;
        }

        private bool ValidateAll()
        {
            if (string.IsNullOrEmpty(txtAktuellTidDb.Text))
            {
                errorProvider.SetError(txtAktuellTidDb, "Fältet får ej va tomt");
                aktuellDbOk = false;
            }
            else if (!uint.TryParse(txtAktuellTidDb.Text, out uint _))
            {
                errorProvider.SetError(txtAktuellTidDb, "Inte ett positivt heltal");
                aktuellDbOk = false;
            }
            else
                aktuellDbOk = true;

            if (string.IsNullOrEmpty(txtAktuellTidByte.Text))
            {
                errorProvider.SetError(txtAktuellTidByte, "Fältet får ej va tomt");
                aktuellOffsetOk = false;
            }
            else if (!uint.TryParse(txtAktuellTidByte.Text, out uint _))
            {
                errorProvider.SetError(txtAktuellTidByte, "Inte ett positivt heltal");
                aktuellOffsetOk = false;
            }
            else
                aktuellOffsetOk = true;

            if (string.IsNullOrEmpty(txtSynkDb.Text))
            {
                errorProvider.SetError(txtSynkDb, "Fältet får ej va tomt");
                syncDbOk = false;
            }
            else if (!uint.TryParse(txtSynkDb.Text, out uint _))
            {
                errorProvider.SetError(txtSynkDb, "Inte ett positivt heltal");
                syncDbOk = false;
            }
            else
                syncDbOk = true;

            if (string.IsNullOrEmpty(txtSynkByte.Text))
            {
                errorProvider.SetError(txtSynkByte, "Fältet får ej va tomt");
                syncOffsetOk = false;
            }
            else if (!uint.TryParse(txtSynkByte.Text, out uint _))
            {
                errorProvider.SetError(txtSynkByte, "Inte ett positivt heltal");
                syncOffsetOk = false;
            }
            else
                syncOffsetOk = true;

            if (string.IsNullOrEmpty(txtSyncBool.Text))
            {
                errorProvider.SetError(txtSyncBool, "Fältet får ej va tomt");
                syncBitOk = false;
            }
            else
                syncBitOk = true;

            if (!aktuellDbOk || !aktuellOffsetOk || !syncDbOk || !syncOffsetOk || !syncBitOk)
                return false;
            else
                return true;
        }
        #endregion

        public SyncPlcTimeForm(ExtendedPlc myPlc)
        {
            InitializeComponent();
            MyPlc = myPlc;
            txtAktuellTidDb.Text = myPlc.ActualTimeDbNr.ToString();
            txtAktuellTidByte.Text = myPlc.ActualTimeOffset.ToString();
            txtSynkDb.Text = myPlc.SyncTimeDbNr.ToString();
            txtSynkByte.Text = myPlc.SyncTimeOffset.ToString();
            checkActive.Checked = myPlc.SyncActive;
            txtSyncBool.Text = myPlc.SyncBoolAddress.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateAll())
            {
                MyPlc.ActualTimeDbNr = int.Parse(txtAktuellTidDb.Text);
                MyPlc.ActualTimeOffset = int.Parse(txtAktuellTidByte.Text);
                MyPlc.SyncTimeDbNr = int.Parse(txtSynkDb.Text);
                MyPlc.SyncTimeOffset = int.Parse(txtSynkByte.Text);
                MyPlc.SyncBoolAddress = txtSyncBool.Text;
                MyPlc.SyncActive = checkActive.Checked;
                SaveToSql();
                Close();
            }
        }

        private void SaveToSql()
        {
            string connectionString = PlcConfig.ConnectionString();
            string activeString;
            if (checkActive.Checked)
                activeString = "True";
            else
                activeString = "False";

            string query;
            query = $"UPDATE {SqlSettings.Default.Databas}.dbo.plcConfig SET actualTimeDb={txtAktuellTidDb.Text},actualTimeOffset={txtAktuellTidByte.Text}";
            query += $",syncTimeDbNr={txtSynkDb.Text},syncTimeOffset={txtSynkByte.Text},syncBoolAddress='{txtSyncBool.Text}',syncActive='{activeString}'";
            query += $" WHERE id = {MyPlc.Id}";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
