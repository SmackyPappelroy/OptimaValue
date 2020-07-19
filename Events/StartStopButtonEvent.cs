using System;

namespace OptimaValue
{
    public static class StartStopButtonEvent
    {
        public static event EventHandler<StartStopButtonEventArgs> NewMessage;

        public static void RaiseMessage(bool isStarted)
        {
            var args = new StartStopButtonEventArgs()
            {
                IsStarted = isStarted
            };
            OnNewMessage(args);
        }

        private static void OnNewMessage(StartStopButtonEventArgs args)
        {
            NewMessage?.Invoke(typeof(StartStopButtonEvent), args);
        }
    }
}
