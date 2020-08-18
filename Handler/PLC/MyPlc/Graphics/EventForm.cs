using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OptimaValue.Handler.PLC.MyPlc.Graphics
{
    public partial class EventForm : Form
    {
        private TagDefinitions tag;
        private string PlcName;
        public event EventHandler<SaveEventArgs> SaveEvent;

        public EventForm(TagDefinitions _tag, string _plcName)
        {
            InitializeComponent();
            PlcName = _plcName;
            tag = _tag;
            if (tag == null)
                tag = new TagDefinitions();
            if (TagsToLog.AllLogValues.Count == 0)
                TagsToLog.FetchValuesFromSql();
            PopulateTagCombo(tag);
        }

        private void EventForm_Load(object sender, EventArgs e)
        {
            comboAnalog.SelectedIndex = 0;
            comboBoolTrigger.SelectedIndex = 0;
            if (comboEventId.Items.Count > 0)
                comboEventId.SelectedIndex = 0;
        }
        private void OnSaveEvent(TagDefinitions _tag)
        {
            var tagArgs = new SaveEventArgs()
            {
                tag = _tag
            };
            SaveEvent?.Invoke(typeof(EventForm), tagArgs);
        }

        private void PopulateTagCombo(TagDefinitions tag)
        {
            if (TagsToLog.AllLogValues == null)
                return;
            if (TagsToLog.AllLogValues.Count == 0)
                return;

            var listan = new List<string>();
            foreach (var taggsen in TagsToLog.AllLogValues)
            {
                if (tag.name != "")
                {
                    if (tag.name != taggsen.name && taggsen.plcName == PlcName && taggsen.logType != LogType.Event)
                        listan.Add(taggsen.name);
                }
            }
            comboEventId.DataSource = listan;
            comboBoolTrigger.SelectedItem = tag.boolTrigger;
            comboBoolTrigger.SelectedText = tag.boolTrigger.ToString();
            comboAnalog.SelectedItem = tag.analogTrigger;
            comboAnalog.SelectedText = tag.analogTrigger.ToString();
            txtValue.Text = tag.analogValue.ToString();
            radioBoolTrigger.Checked = tag.IsBooleanTrigger;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (TagsToLog.AllLogValues == null)
                return;
            if (TagsToLog.AllLogValues.Count == 0)
                return;

            if (radioBoolTrigger.Checked)
            {
                if (comboEventId.SelectedItem.ToString() != "" &&
                    comboBoolTrigger.SelectedItem.ToString() != "")
                {
                    SaveValues();
                }
            }
            else
            {
                if (comboEventId.SelectedItem.ToString() != "" &&
                    comboAnalog.SelectedItem.ToString() != "" &&
                    float.TryParse(txtValue.Text, out float _))
                {
                    SaveValues();
                }
            }
        }

        private void SaveValues()
        {
            tag.boolTrigger = (BooleanTrigger)Enum.Parse(typeof(BooleanTrigger), comboBoolTrigger.SelectedItem.ToString());
            tag.analogTrigger = (AnalogTrigger)Enum.Parse(typeof(AnalogTrigger), comboAnalog.SelectedItem.ToString());
            tag.IsBooleanTrigger = radioBoolTrigger.Checked;
            if (!radioBoolTrigger.Checked)
                tag.analogValue = float.Parse(txtValue.Text);
            foreach (var taggen in TagsToLog.AllLogValues)
            {
                if (taggen.plcName == PlcName)
                {
                    if (taggen.name == comboEventId.SelectedItem.ToString())
                        tag.eventId = taggen.id;
                }
            }
            if (tag.eventId != 0)
            {
                tag.IsBooleanTrigger = radioBoolTrigger.Checked;
                tag.boolTrigger = (BooleanTrigger)Enum.Parse(typeof(BooleanTrigger), comboBoolTrigger.SelectedItem.ToString());
                tag.analogTrigger = (AnalogTrigger)Enum.Parse(typeof(AnalogTrigger), comboAnalog.SelectedItem.ToString());
                if (!radioBoolTrigger.Checked)
                    tag.analogValue = float.Parse(txtValue.Text);
                OnSaveEvent(tag);
                Close();
            }
        }

        private void txtValue_TextChanged(object sender, EventArgs e)
        {
            if (!float.TryParse(txtValue.Text, out float _))
                errorProvider.SetError(txtValue, "Inget tal");
        }
    }
}
