using System.Threading.Tasks;

namespace OptimaValue
{
    public static class Master
    {
        public static bool AbortSqlLog = false;

        public static async Task<bool> StartLog()
        {
            bool ActivePlcs;

            if (!await PlcConfig.TestConnectionSqlAsync())
            {
                "Ingen kontakt med databas".SendThisStatusMessage(Severity.Warning);
                return false;
            }

            if (TagsToLog.FetchValuesFromSql() == null)
                return false;

            foreach (TagDefinitions item in TagsToLog.AllLogValues)
            {
                if ((int)item.LogFreq < Logger.FastestLogTime && item.Active)
                    Logger.FastestLogTime = (int)item.LogFreq;
            }

            Logger.Start();
            ActivePlcs = true;

            if (ActivePlcs)
                SendValuesToSql.StartSql();

            return true;
        }

        public static void StopLog(bool applicationShutdown)
        {
            Logger.Stop(applicationShutdown);
            AbortSqlLog = true;
        }
    }
}
