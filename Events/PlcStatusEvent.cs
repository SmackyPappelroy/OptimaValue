using FileLogger;
using System;

namespace OptimaValue
{

    public static class PlcStatusEvent
    {
        public static event EventHandler<PlcStatusEventArgs> NewMessage;

        public static void OnNewMessageRaised(string message, string plcName, Status status)
        {
            PlcStatusEventArgs args = new PlcStatusEventArgs(message, plcName, status);
            RaiseNewMessage(args);
        }

        private static void RaiseNewMessage(PlcStatusEventArgs e)
        {
            var handler = NewMessage;
            if (handler != null)
            {
                try
                {
                    handler.Invoke(typeof(PlcStatusEvent), e);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Fel vid anrop av NewMessage-event: {ex.Message}");
                }
            }
        }
    }
}
