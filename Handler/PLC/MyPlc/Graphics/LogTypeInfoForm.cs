using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OptimaValue.Handler.PLC.MyPlc.Graphics
{
    public partial class LogTypeInfoForm : Form
    {
        private AddTag addTag;

        public LogTypeInfoForm(AddTag addTag)
        {
            InitializeComponent();
            this.addTag = addTag;
        }

        private void LogTypeInfoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            addTag.logInfoFormOpen = false;
        }
    }
}
