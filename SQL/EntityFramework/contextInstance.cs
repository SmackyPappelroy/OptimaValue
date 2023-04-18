using System.Threading.Tasks;

namespace OptimaValue
{
    public static class contextInstance
    {
        private static LoggingDBContext instance;

        public static void Instantiate()
        {
            if (instance == null)
            {
                instance = new LoggingDBContext();
            }
        }

        public static async Task<bool> CreateDb()
        {
            if (instance == null)
                Instantiate();

            var result = await Task.FromResult(instance.Database.CreateIfNotExists());
            DatabaseCreationNotifier.NotifyDatabaseCreated(result);
            return result;
        }

    }
}
