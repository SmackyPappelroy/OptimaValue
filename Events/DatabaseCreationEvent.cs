using System;

namespace OptimaValue
{
    public class DatabaseCreationEventArgs : EventArgs
    {
        public bool Created { get; set; }
    }
    public static class DatabaseCreationNotifier
    {
        public static event EventHandler<DatabaseCreationEventArgs> DatabaseCreated;

        public static void NotifyDatabaseCreated(bool created)
        {
            var args = new DatabaseCreationEventArgs { Created = created };
            RaiseDatabaseCreatedEvent(args);
        }

        private static void RaiseDatabaseCreatedEvent(DatabaseCreationEventArgs e)
        {
            DatabaseCreated?.Invoke(typeof(DatabaseCreationNotifier), e);
        }
    }
}