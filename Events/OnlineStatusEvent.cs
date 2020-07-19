using System;
using System.Drawing;

namespace OptimaValue
{
    public static class OnlineStatusEvent
    {
        public static event EventHandler<OnlineStatusEventArgs> NewMessage;

        public static void RaiseMessage(ConnectionStatus connectionStatus, string plcName, string elapsedTime = "")
        {
            OnlineStatusEventArgs args = new OnlineStatusEventArgs();
            if (connectionStatus == ConnectionStatus.Connected)
                args.Message = "Ansluten";
            else
                args.Message = "Ej Ansluten";
            args.connectionStatus = connectionStatus;

            if (args.connectionStatus == ConnectionStatus.Connected)
                args.Color = UIColors.Active;
            else
                args.Color = Color.FromArgb(67, 62, 71);

            args.ElapsedTime = elapsedTime;

            args.PlcName = plcName;

            OnNewMessage(args);
        }

        private static void OnNewMessage(OnlineStatusEventArgs e)
        {
            NewMessage?.Invoke(typeof(StatusEvent), e);
        }
    }
}
