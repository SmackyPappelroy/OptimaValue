using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace OptimaValue.Handler.PLC.MyPlc.Graphics.Parameters
{
    public partial class paramaterComboControl : UserControl
    {
        public paramaterComboControl()
        {
            InitializeComponent();
        }

        [Category("Combo")]
        [Browsable(true)]
        [DisplayName("Items")]
        public string[] ComboItems
        {
            get
            {
                return comboBox.Items.Cast<object>()
                        .Select(x => x.ToString()).ToArray();
            }
            set
            {
                comboBox.Items.Clear();
                comboBox.Items.AddRange(value);
            }
        }

        [Category("Combo")]
        [Browsable(true)]
        [DisplayName("HeaderText")]
        public string HeaderText
        {
            get => lblText.Text;
            set
            {
                lblText.Text = value;
            }
        }

        public ComboBox comboBoxen
        {
            get => comboBox;
            set { comboBox = value; }
        }
    }
}
