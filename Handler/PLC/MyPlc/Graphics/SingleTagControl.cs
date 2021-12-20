using OptimaValue.Properties;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace OptimaValue.Handler.PLC.MyPlc.Graphics
{
    public partial class SingleTagControl : UserControl
    {
        public event EventHandler TagChanged;
        private readonly TagDefinitions SingleTag;
        private bool addTagOpen = false;
        private readonly TreeView myTreeView;
        private AddTag changeTagForm;
        private readonly ExtendedPlc MyPlc;
        private bool statFormOpen = false;
        private readonly Image errorImage;


        private bool destroyed = false;

        protected virtual void OnTagChanged(EventArgs e)
        {
            TagChanged?.Invoke(this, e);
        }

        public SingleTagControl(TagDefinitions tag, ExtendedPlc myPlc, TreeView treeView)
        {
            InitializeComponent();

            SingleTag = tag;
            txtName.Text = tag.Name;
            myTreeView = treeView;
            MyPlc = myPlc;
            errorImage = Resources.tag_error;
            statusPicture.Image = errorImage;

            if (tag.NrFailedReadAttempts == 0)
                statusPicture.Hide();
            else
                statusPicture.Show();

            tag.ReadErrorEvent += Tag_ReadErrorEvent;

            MyPlc.StartedEvent += Logger_StartedEvent;
            this.HandleDestroyed += SingleTagControl_HandleDestroyed;
            if (MyPlc.LoggerIsStarted)
            {
                contextMenuStrip.Enabled = false;
                statsImage.Enabled = true;
            }

        }

        private void Tag_ReadErrorEvent(object sender, ReadErrorEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { Tag_ReadErrorEvent(sender, e); });
                return;
            }
            if (e.Error)
                statusPicture.Show();
            else
                statusPicture.Hide();
        }

        private void SingleTagControl_HandleDestroyed(object sender, EventArgs e)
        {
            destroyed = true;
            MyPlc.StartedEvent -= Logger_StartedEvent;
        }

        private void Logger_StartedEvent(object sender, EventArgs e)
        {
            try
            {
                if (sender == null)
                    return;
                if (InvokeRequired && !destroyed)
                {
                    Invoke((MethodInvoker)delegate { Logger_StartedEvent(sender, e); });
                    return;
                }
                if (MyPlc.LoggerIsStarted)
                {
                    contextMenuStrip.Enabled = false;
                    statsImage.Enabled = true;
                }
                else
                {
                    contextMenuStrip.Enabled = true;
                    statsImage.Enabled = false;
                }
            }
            catch (System.ComponentModel.InvalidAsynchronousStateException) { }
            catch (InvalidOperationException) { }

        }

        private void changeTagMenu_Click(object sender, System.EventArgs e)
        {
            //if (MyPlc == null)
            //    PlcConfig.PopulateDataTable();

            if (!addTagOpen && !MyPlc.LoggerIsStarted)
            {
                changeTagForm = new AddTag(SingleTag.PlcName, MyPlc, SingleTag);
                changeTagForm.FormClosing += ChangeTagForm_FormClosing;
                addTagOpen = true;
                myTreeView.Enabled = false;
                changeTagForm.Show();
            }

        }

        private void ChangeTagForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            addTagOpen = false;
            myTreeView.Enabled = true;
            changeTagForm.FormClosing -= ChangeTagForm_FormClosing;
            OnTagChanged(EventArgs.Empty);
        }

        private void removeTagMenu_Click(object sender, System.EventArgs e)
        {
            if (MyPlc.LoggerIsStarted)
                return;
            DeleteTag();
            OnTagChanged(EventArgs.Empty);
        }
        private void DeleteTag()
        {
            var connectionString = PlcConfig.ConnectionString();
            var query = $"DELETE FROM {SqlSettings.Default.Databas}.dbo.logValues ";
            query += $"WHERE tag_id = {SingleTag.Id}";
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                Apps.Logger.Log(string.Empty, Severity.Error, ex);
            }


            query = $"DELETE FROM {SqlSettings.Default.Databas}.dbo.tagConfig ";
            query += $"WHERE id = {SingleTag.Id}";
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex2)
            {
                Apps.Logger.Log(string.Empty, Severity.Error, ex2);
            }
        }


        private void OpenStatsForm()
        {
            TagStatisticsForm statForm;
            TagDefinitions tag = null;
            foreach (TagDefinitions logValue in TagsToLog.AllLogValues)
            {
                if (SingleTag.Id == logValue.Id)
                    tag = logValue;
            }
            if (tag != null && !statFormOpen)
            {
                statFormOpen = true;
                statForm = new TagStatisticsForm(tag)
                {
                    Text = tag.Name
                };
                statForm.FormClosing += StatForm_FormClosing;
                statForm.Show();
            }
        }

        private void StatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            statFormOpen = false;
        }

        private void pictureBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpenStatsForm();
        }

        private void rensaFelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SingleTag.ClearScanTime();
        }
    }
}
