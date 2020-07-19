using System;

namespace OptimaValue
{
    public static class StatusEvent
    {
        public static event EventHandler<StatusEventArgs> NewMessage;


        public static void RaiseMessage(string message, Status status)
        {

            StatusEventArgs args = new StatusEventArgs
            {
                Message = message,
                Status = status
            };
            OnNewMessage(args);
        }

        private static void OnNewMessage(StatusEventArgs e)
        {
            NewMessage?.Invoke(typeof(StatusEvent), e);
        }
    }


}
