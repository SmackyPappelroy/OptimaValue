using FileLogger;
using System;

namespace OptimaValue
{
    public class DatabaseCreationEventArgs : EventArgs
    {
        public bool Created { get; }
        public DatabaseCreationEventArgs(bool created)
        {
            Created = created;
        }
    }

    public static class DatabaseCreationNotifier
    {
        public static event EventHandler<DatabaseCreationEventArgs> DatabaseCreated;

        public static void OnDatabaseCreated(bool created)
        {
            var args = new DatabaseCreationEventArgs(created);
            RaiseDatabaseCreatedEvent(args);
        }

        private static void RaiseDatabaseCreatedEvent(DatabaseCreationEventArgs e)
        {
            var handler = DatabaseCreated;
            if (handler != null)
            {
                try
                {
                    handler.Invoke(typeof(DatabaseCreationNotifier), e);
                }
                catch (Exception ex)
                {
                    // Logga eller hantera undantaget om det behövs
                    Logger.LogError($"Fel vid anrop av DatabaseCreated-event: {ex.Message}");
                }
            }
        }
    }
}
