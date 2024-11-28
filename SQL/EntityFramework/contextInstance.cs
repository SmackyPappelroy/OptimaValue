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
                instance = new LoggingDBContext(); // Skapa en ny standardinstans
            }
        }

        public static async Task<bool> CreateDbAsync()
        {
            if (instance == null)
                Instantiate();

            // Kontrollera om databasen existerar och skapa den om inte
            var result = await instance.Database.EnsureCreatedAsync();
            DatabaseCreationNotifier.OnDatabaseCreated(result);
            return result;
        }
    }
}
