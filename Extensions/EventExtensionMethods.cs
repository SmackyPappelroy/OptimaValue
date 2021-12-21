using System;

namespace OptimaValue
{
    public static class EventExtensionMethods
    {
        public static void SendStatusMessage(this string message, Severity severity = Severity.Normal, Exception ex = null)
        {
            Apps.Logger.Log(message, severity, ex);

        }
    }
}
