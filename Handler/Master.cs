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

            foreach (ExtendedPlc plc in PlcConfig.PlcList)
            {
                if (plc.Active)
                {
                    plc.logger.Start();
                    ActivePlcs = true;
                }
            }
            if (ActivePlcs)
                SendValuesToSql.StartSql();
        }

        public static void StopLog(bool applicationShutdown)
        {
            foreach (ExtendedPlc plc in PlcConfig.PlcList)
            {
                plc.logger.Stop(applicationShutdown);
            }
            AbortSqlLog = true;
        }
    }
}
