using System;

namespace OptimaValue
{
    public static class RedrawTreeEvent
    {
        public static event EventHandler<RedrawTreeEventArgs> NewMessage;

        public static void RaiseMessage(bool redraw)
        {

            RedrawTreeEventArgs args = new RedrawTreeEventArgs
            {
                Redraw = redraw,
            };
            OnNewMessage(args);
        }

        private static void OnNewMessage(RedrawTreeEventArgs e)
        {
            NewMessage?.Invoke(typeof(StatusEvent), e);
        }
    }
}
