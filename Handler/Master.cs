namespace OptimaValue
{
    public static class Master
    {
        public static bool AbortSqlLog = false;

        public static void StartLog()
        {
            //PlcConfig.PopulateDataTable();
            var ActivePlcs = false;

            if (TagsToLog.FetchValuesFromSql() == null)
                return;


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
