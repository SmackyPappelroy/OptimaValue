using System;

namespace OptimaValue
{
    public class DataBaseCreationEventArgs : EventArgs
    {
        public bool Created { get; set; }
    }
    public static class DatabaseCreationEvent
    {
        public static event EventHandler<DataBaseCreationEventArgs> CreatedEvent;

        public static void RaiseMessage(bool created)
        {
            var args = new DataBaseCreationEventArgs()
            {
                Created = created
            };
            OnCreatedEvent(args);
        }

        public static void OnCreatedEvent(DataBaseCreationEventArgs e)
        {
            CreatedEvent?.Invoke(typeof(DatabaseCreationEvent), e);
        }
    }
}
