using System;
using System.Drawing;

namespace OptimaValue
{
    public class OnlineStatusEventArgs : EventArgs
    {
        public string Message { get; }
        public ConnectionStatus ConnectionStatus { get; }
        public Color Color { get; }
        public string ElapsedTime { get; }
        public string PlcName { get; }
        public int TotalConnectionAttempts { get; }
        public int FailedConnectionAttempts { get; }
        public TimeSpan TotalReconnectTime { get; }

        public OnlineStatusEventArgs(ConnectionStatus connectionStatus, string message, Color color, string elapsedTime, string plcName, int totalConnectionAttempts, int failedConnectionAttempts, TimeSpan totalReconnectTime)
        {
            ConnectionStatus = connectionStatus;
            Message = message;
            Color = color;
            ElapsedTime = elapsedTime;
            PlcName = plcName;
            TotalConnectionAttempts = totalConnectionAttempts;
            FailedConnectionAttempts = failedConnectionAttempts;
            TotalReconnectTime = totalReconnectTime;
        }
    }
}
