using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace OptimaValue.Handler.PLC.MyPlc.Graphics.Parameters
{
    public partial class parameterTextControl : UserControl
    {
        private object tag;
        private object mValue;
        private string PropertyNamn;
        public parameterTextControl(object taggen = null, string propertyNamn = default)
        {
            InitializeComponent();
            if (taggen != null)
            {
                tag = taggen;
                if (!propertyNamn.Equals(default))
                {
                    Header = propertyNamn;
                    ParameterValue = GetPropertyValue(tag, propertyNamn).ToString();
                    PropertyNamn = propertyNamn;
                }
            }
        }
        public parameterTextControl()
        {
            InitializeComponent();
        }

        private void SetProperty(object tag, string propertyName, object value)
        {
            Type type = tag.GetType();
            PropertyInfo prop = type.GetProperty(propertyName);
            prop.SetValue(tag, value, null);
        }

        private object GetPropertyValue(object tag, string propertyName)
        {
            var prop = tag.GetType().GetRuntimeProperties().FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));
            return prop?.GetValue(tag);
        }


        [Category("Parameter")]
        [Browsable(true)]
        [DisplayName("HeaderText")]
        public string Header
        {
            get => lblText.Text;
            set
            {
                lblText.Text = value;
            }
        }

        [Category("Parameter")]
        [Browsable(true)]
        [DisplayName("ParameterValue")]
        public string ParameterValue
        {
            get => txtParameter.Text;
            set
            {
                txtParameter.Text = value;
                if (tag != null)
                    mValue = value;
            }
        }




    }
}
