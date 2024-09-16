using FileLogger;
using System;
using System.Drawing;

namespace OptimaValue;

public static class OnlineStatusEvent
{
    public static event EventHandler<OnlineStatusEventArgs> NewMessage;

    public static void RaiseNewMessage(ConnectionStatus connectionStatus, string plcName, 
        int totalConnectionAttempts, int failedConnectionAttempts, TimeSpan totalReconnectTime, string elapsedTime = "")
    {
        var args = CreateEventArgs(connectionStatus, plcName, totalConnectionAttempts, 
            failedConnectionAttempts, totalReconnectTime, elapsedTime);
        OnNewMessage(args);
    }

    private static OnlineStatusEventArgs CreateEventArgs(ConnectionStatus connectionStatus, string plcName, 
        int totalConnectionAttempts, int failedConnectionAttempts, TimeSpan totalReconnectTime, string elapsedTime)
    {
        return new OnlineStatusEventArgs(
            connectionStatus,
            GetStatusMessage(connectionStatus),
            GetStatusColor(connectionStatus),
            elapsedTime,
            plcName,
            totalConnectionAttempts,
            failedConnectionAttempts,
            totalReconnectTime
        );
    }

    private static string GetStatusMessage(ConnectionStatus connectionStatus) =>
        connectionStatus == ConnectionStatus.Connected ? "Ansluten" : "Ej Ansluten";

    private static Color GetStatusColor(ConnectionStatus connectionStatus) =>
        connectionStatus == ConnectionStatus.Connected ? UIColors.Active : UIColors.GreyColor;

    private static void OnNewMessage(OnlineStatusEventArgs e)
    {
        var handler = NewMessage;
        if (handler != null)
        {
            try
            {
                handler.Invoke(typeof(OnlineStatusEvent), e);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Fel vid anrop av NewMessage-event: {ex.Message}");
            }
        }
    }
}


