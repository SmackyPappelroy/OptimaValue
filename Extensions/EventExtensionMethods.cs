using Logger;
using System;

namespace OptimaValue
{
    public static class EventExtensionMethods
    {
        public static void SendStatusMessage(this string message, Severity severity = Severity.Information, Exception ex = null)
        {
            FileLoggerInstance.Instance.Log(new LogTemplate(message, ex, severity));
        }
    }
}
