using System;
using System.Drawing;

namespace OptimaValue
{
    public static class OnlineStatusEvent
    {
        public static event EventHandler<OnlineStatusEventArgs> NewMessage;
        public static void RaiseMessage(ConnectionStatus connectionStatus, string plcName, string elapsedTime = "")
        {
            OnlineStatusEventArgs args = CreateEventArgs(connectionStatus, plcName, elapsedTime);
            OnNewMessage(args);
        }

        private static OnlineStatusEventArgs CreateEventArgs(ConnectionStatus connectionStatus, string plcName, string elapsedTime)
        {
            OnlineStatusEventArgs args = new OnlineStatusEventArgs
            {
                Message = connectionStatus == ConnectionStatus.Connected ? "Ansluten" : "Ej Ansluten",
                connectionStatus = connectionStatus,
                Color = connectionStatus == ConnectionStatus.Connected ? UIColors.Active : UIColors.GreyColor,
                ElapsedTime = elapsedTime,
                PlcName = plcName
            };

            return args;
        }

        private static void OnNewMessage(OnlineStatusEventArgs e)
        {
            NewMessage?.Invoke(typeof(StatusEvent), e);
        }
    }
}
