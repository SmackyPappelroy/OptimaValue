using System;

namespace OptimaValue
{
    public static class DeleteConfirmationEvent
    {
        public static event EventHandler<DeleteEventArgs> DeleteEvent;

        public static void Confirmation(bool delete)
        {
            var args = new DeleteEventArgs()
            {
                Delete = delete
            };
            OnDeleted(args);
        }

        private static void OnDeleted(DeleteEventArgs args)
        {
            DeleteEvent?.Invoke(typeof(DeleteConfirmationEvent), args);
        }
    }

    public class DeleteEventArgs
    {
        public bool Delete { get; set; }
    }
}
