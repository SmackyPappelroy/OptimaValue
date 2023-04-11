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
            if (tblPlcConfig == null)
                return false;

            if (tblPlcConfig.Rows.Count == 0)
            {
                PlcList.Clear();
                return false;
            }

            if (PlcList == null)
                PlcList = new List<ExtendedPlc>();

            PlcList.Clear();

            foreach (DataRow row in tblPlcConfig.Rows)
            {
                var id = row.Field<int>("id");
                var ip = row.Field<string>("ipAddress");
                var cpu = (CpuType)Enum.Parse(typeof(CpuType), row.Field<string>("cpuType"));
                var rack = row.Field<short>("rack");
                var slot = row.Field<short>("slot");
                var name = row.Field<string>("name");

                var syncTimeDbNr = row.Field<int>("syncTimeDbNr");
                var syncTimeDbOffset = row.Field<int>("syncTimeOffset");
                var syncActive = row.Field<bool>("syncActive");
                var syncBoolAddress = row.Field<string>("syncBoolAddress");
                var lastSyncTime = row.Field<DateTime>("lastSyncTime");
                var activePlcId = row.Field<int>("id");
                var active = row.Field<bool>("Active");

                var plcConfig = new PlcConfiguration(name, cpu, ip, rack, slot, activePlcId, active,
                    syncTimeDbNr, syncTimeDbOffset, syncBoolAddress, syncActive, lastSyncTime);

                var myPlc = new ExtendedPlc(plcConfig);
                PlcList.Add(myPlc);
            }

            return PlcList.Count > 0;
        }


        public static ExtendedPlc FindPlc(int plcId) => PlcList.Find(p => p.Id == plcId);

        #endregion
    }
}