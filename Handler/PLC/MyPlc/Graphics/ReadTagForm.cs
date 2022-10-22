using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OptimaValue
{
    public partial class ReadTagForm : Form
    {
        private ReadTagControl control;
        private IPlc plc;
        private PlcTag tag;

        public ReadTagForm(ReadTagControl control, IPlc plc, PlcTag tag)
        {
            this.plc = plc;
            this.tag = tag;
            InitializeComponent();
            this.control = control;
            this.Load += ReadTagForm_Load;
            this.control.Disposed += (s, e) => this.Close();
        }



        private void ReadTagForm_Load(object sender, EventArgs e)
        {
            this.Text = tag.Name;
            this.Controls.Add(control);
            control.Dock = DockStyle.Fill;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            control.Dispose();
            base.OnClosing(e);
        }
    }
}
