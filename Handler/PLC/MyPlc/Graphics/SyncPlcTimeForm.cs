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
        private bool syncDbOk;
        private bool syncOffsetOk;
        private bool syncBitOk;


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

            if (!syncDbOk || !syncOffsetOk || !syncBitOk)
                return false;
            else
                return true;
        }
        #endregion

        public SyncPlcTimeForm(ExtendedPlc myPlc)
        {
            InitializeComponent();
            MyPlc = myPlc;
            txtSynkDb.Text = myPlc.SyncTimeDbNr.ToString();
            txtSynkByte.Text = myPlc.SyncTimeOffset.ToString();
            checkActive.Checked = myPlc.SyncActive;
            txtSyncBool.Text = myPlc.SyncBoolAddress.ToString();
            if (MyPlc.CPU == S7.Net.CpuType.S7300 || MyPlc.CPU == S7.Net.CpuType.S7400)
                pictureBox1.Image = Properties.Resources.S7300Sync;
            else
                pictureBox1.Image = Properties.Resources.S71500Sync__Custom_;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateAll())
            {
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
            string activeString;
            if (checkActive.Checked)
                activeString = "True";
            else
                activeString = "False";


            DatabaseSql.SavePlcSyncParameters(
                syncDb: txtSynkDb.Text
                , syncByte: txtSynkByte.Text
                , syncBool: txtSyncBool.Text
                , activeString: activeString
                , plcId: MyPlc.Id);

        }
    }
}
