namespace OptimaValue
{
    public static class EventExtensionMethods
    {
        public static void SendThisStatusMessage(this string message, Severity severity = Severity.Normal)
        {
            Apps.Logger.Log(message, severity);

        }
    }
}
