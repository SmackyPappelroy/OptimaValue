using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OptimaValue
{
    public static class MyExtensionMethods
    {
        public static void KeepOpenOnDropdownCheck(this ToolStripMenuItem ctl)
        {
            foreach (var item in ctl.DropDownItems.OfType<ToolStripMenuItem>())
            {
                item.MouseEnter += (o, e) => ctl.DropDown.AutoClose = false;
                item.MouseLeave += (o, e) => ctl.DropDown.AutoClose = true;
            }

        }

        public static void ChangeForeColorMenuItem(this ToolStripMenuItem ctl, Color newColor, Color previousColor)
        {
            ctl.DropDownOpened += (o, e) => ctl.ForeColor = newColor;
            ctl.DropDownClosed += (o, e) => ctl.ForeColor = previousColor;
        }

        public static void MouseHoverMenuItem(this ToolStripMenuItem ctl, Color newColor, Color previousColor)
        {
            ctl.MouseEnter += (o, e) => ctl.ForeColor = newColor;
            ctl.MouseLeave += (o, e) => ctl.ForeColor = previousColor;
        }


    }
}
