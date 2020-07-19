using System;

namespace OptimaValue
{
    public static class PlcStatusEvent
    {
        public static event EventHandler<PlcStatusEventArgs> NewMessage;
        public static void RaiseMessage(string message, string plcName, Status status)
        {

            PlcStatusEventArgs args = new PlcStatusEventArgs
            {
                Message = message,
                Status = status,
                PlcName = plcName
            };
            OnNewMessage(args);
        }

        private static void OnNewMessage(PlcStatusEventArgs e)
        {
            NewMessage?.Invoke(typeof(PlcStatusEvent), e);
        }
    }
}
