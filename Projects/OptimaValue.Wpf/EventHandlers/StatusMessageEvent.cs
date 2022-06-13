using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue.Wpf
{
    public class StatusEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public static class StatusMessageEvent
    {
        public static event EventHandler<StatusEventArgs> OnStatusTextChanged;

        public static void RaiseMessage(string message)
        {
            StatusEventArgs e = new StatusEventArgs() { Message = message };

            OnStatusTextChanged?.Invoke(null, e);
        }


    }
}
