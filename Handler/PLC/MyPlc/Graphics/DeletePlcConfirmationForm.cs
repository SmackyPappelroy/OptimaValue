using System;
using System.Windows.Forms;

namespace OptimaValue.Handler.PLC.MyPlc.Graphics
{
    public partial class DeletePlcConfirmationForm : Form
    {
        public DeletePlcConfirmationForm()
        {
            InitializeComponent();
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            DeleteConfirmationEvent.Confirmation(true);
            Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            DeleteConfirmationEvent.Confirmation(false);
            Close();
        }
    }
}
