using DocumentFormat.OpenXml.Wordprocessing;
using Logger;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace OptimaValue
{
    public static class Master
    {
        public static bool AbortSqlLog = false;
        public static bool IsStarted;
        public static bool Stopping;

        public static async Task<bool> StartLog()
        {
            bool ActivePlcs;

            if (!await DatabaseSql.TestConnectionAsync())
            {
                $"Ingen kontakt med databas {Config.Settings.ConnectionString}".SendStatusMessage(Severity.Warning);
                return false;
            }

            if (TagsToLog.GetAllTagsFromSql() == null)
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
            IsStarted = true;
            return true;
        }

        public static void StopLog(bool applicationShutdown)
        {
            Logger.Stop(applicationShutdown);
            AbortSqlLog = true;
            IsStarted = false;
        }
    }
}
