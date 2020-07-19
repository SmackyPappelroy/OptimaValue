namespace OptimaValue
{
    public static class EventExtensionMethods
    {
        public static void SendThisStatusMessage(this string message, Status status = Status.Ok)
        {
            StatusEvent.RaiseMessage(message, status);
        }
    }
}
