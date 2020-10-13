namespace OptimaValue
{
    public static class Master
    {
        public static bool AbortSqlLog = false;

        public static void StartLog()
        {
            bool ActivePlcs;

            if (TagsToLog.FetchValuesFromSql() == null)
                return;

            foreach (TagDefinitions item in TagsToLog.AllLogValues)
            {
                if ((int)item.logFreq < Logger.FastestLogTime && item.active)
                    Logger.FastestLogTime = (int)item.logFreq;
            }

            Logger.Start();
            ActivePlcs = true;

            if (ActivePlcs)
                SendValuesToSql.StartSql();

        }

        public static void StopLog(bool applicationShutdown)
        {
            foreach (ExtendedPlc myPlc in PlcConfig.PlcList)
                myPlc.ReconnectRetries = 0;

            Logger.Stop(applicationShutdown);
            AbortSqlLog = true;
        }
    }
}
