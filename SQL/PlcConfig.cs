using S7.Net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace OptimaValue
{
    public static class PlcConfig
    {
        #region Public properties
        public static DataTable tblPlcConfig = new DataTable();
        public static List<ExtendedPlc> PlcList = new List<ExtendedPlc>();

        public static bool plcSettingsFormOpen = false;
        public static bool sqlSettingsFormOpen = false;
        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the configured <see cref="Plc"/>s from the Sql-server
        /// </summary>
        /// <returns><see cref="DataTable"/></returns>
        public static DataTable PopulateDataTable()
        {
            tblPlcConfig.Clear();
            tblPlcConfig = DatabaseSql.GetPlcDataTable();
            if (tblPlcConfig != null)
            {
                PopulatePlcList();
            }
            return tblPlcConfig;

        }

        /// <summary>
        /// Creates a <see cref="List{T}"/> of <see cref="LogPlc"/>
        /// </summary>
        /// <returns><code>True</code>If successfull</returns>
        private static bool PopulatePlcList()
        {
            if (tblPlcConfig == null || tblPlcConfig.Rows.Count == 0)
            {
                PlcList?.Clear();
                return false;
            }

            PlcList = tblPlcConfig.AsEnumerable()
                .Select(row => new PlcConfiguration(
                    plcName: row.Field<string>("name"),
                    cpuType: Enum.Parse<CpuType>(row.Field<string>("cpuType")),
                    ip: row.Field<string>("ipAddress"),
                    rack: row.Field<short>("rack"),
                    slot: row.Field<short>("slot"),
                    activePlcId: row.Field<int>("id"),
                    active: row.Field<bool>("Active"),
                    syncTimeDbNr: row.Field<int>("syncTimeDbNr"),
                    syncTimeOffset: row.Field<int>("syncTimeOffset"),
                    syncBoolAddress: row.Field<string>("syncBoolAddress"),
                    syncActive: row.Field<bool>("syncActive"),
                    lastSyncTime: row.Field<DateTime>("lastSyncTime")
                ))
                .Select(plcConfig => new ExtendedPlc(plcConfig))
                .ToList();

            return PlcList.Count > 0;
        }



        public static ExtendedPlc FindPlc(int plcId) => PlcList.Find(p => p.Id == plcId);

        #endregion
    }
}