using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Threading.Tasks;
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

        private bool ValidateField(TextBox textBox, string errorMessage, bool requirePositiveInteger = false)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                errorProvider.SetError(textBox, "Fältet får ej vara tomt");
                return false;
            }
            if (requirePositiveInteger && !uint.TryParse(textBox.Text, out uint _))
            {
                errorProvider.SetError(textBox, "Inte ett positivt heltal");
                return false;
            }
            errorProvider.SetError(textBox, string.Empty);
            return true;
        }


        private void txtSynkDb_Validating(object sender, CancelEventArgs e)
        {
            syncDbOk = ValidateField(txtSynkDb, "Fältet får ej vara tomt", requirePositiveInteger: true);
        }
        private void txtSynkByte_Validating(object sender, CancelEventArgs e)
        {
            syncOffsetOk = ValidateField(txtSynkByte, "Fältet får ej vara tomt", requirePositiveInteger: true);
        }
        private void txtSyncBool_Validating(object sender, CancelEventArgs e)
        {
            syncBitOk = ValidateField(txtSyncBool, "Fältet får ej vara tomt");
        }

        private bool ValidateAll()
        {
            syncDbOk = ValidateField(txtSynkDb, "Fältet får ej vara tomt", requirePositiveInteger: true);
            syncOffsetOk = ValidateField(txtSynkByte, "Fältet får ej vara tomt", requirePositiveInteger: true);
            syncBitOk = ValidateField(txtSyncBool, "Fältet får ej vara tomt");

            return syncDbOk && syncOffsetOk && syncBitOk;
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
            if ((S7.Net.CpuType)MyPlc.CpuType == S7.Net.CpuType.S7300 || (S7.Net.CpuType)MyPlc.CpuType == S7.Net.CpuType.S7400)
            {
                pictureBox1.Image = Properties.Resources.S7300Sync;
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            }
            else
            {
                pictureBox1.Image = Properties.Resources.S71500Sync__Custom_;
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            }

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

        private async void SaveToSql()
        {
            string activeString = checkActive.Checked ? "True" : "False";

            try
            {
                await Task.Run(() =>
                {
                    DatabaseSql.SavePlcSyncParameters(
                        syncDb: txtSynkDb.Text,
                        syncByte: txtSynkByte.Text,
                        syncBool: txtSyncBool.Text,
                        activeString: activeString,
                        plcId: MyPlc.Id);
                });
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ett fel uppstod vid sparandet till databasen: {ex.Message}");
            }
        }


    }
}
