using FileLogger;
using System;

namespace OptimaValue
{
    public static class EventExtensionMethods
    {
        public static void SendStatusMessage(this string message, Severity severity = Severity.Information, Exception ex = null)
        {
            Logger.Instance.Log(new LogTemplate(message, ex, severity));
        }
    }
}
