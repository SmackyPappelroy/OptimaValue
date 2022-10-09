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
            var tbl = tblPlcConfig;
            if (tbl.Rows.Count == 0)
            {
                PlcList.Clear();
                return false;
            }
            if (PlcList == null)
                PlcList = new List<ExtendedPlc>();
            PlcList.Clear();
            for (int rowIndex = 0; rowIndex < tbl.Rows.Count; rowIndex++)
            {
                var _id = (tbl.AsEnumerable().ElementAt(rowIndex).Field<int>("id"));
                var _ip = (tbl.AsEnumerable().ElementAt(rowIndex).Field<string>("ipAddress"));
                var _cpu = (CpuType)Enum.Parse(typeof(CpuType), (tbl.AsEnumerable().ElementAt(rowIndex).Field<string>("cpuType")));
                var _rack = (tbl.AsEnumerable().ElementAt(rowIndex).Field<short>("rack"));
                var _slot = (tbl.AsEnumerable().ElementAt(rowIndex).Field<short>("slot"));
                var _name = (tbl.AsEnumerable().ElementAt(rowIndex).Field<string>("name"));

                var _syncTimeDbNr = (tbl.AsEnumerable().ElementAt(rowIndex).Field<int>("syncTimeDbNr"));
                var _syncTimeDbOffset = (tbl.AsEnumerable().ElementAt(rowIndex).Field<int>("syncTimeOffset"));
                var _syncActive = (tbl.AsEnumerable().ElementAt(rowIndex).Field<bool>("syncActive"));
                var _syncBoolAddress = (tbl.AsEnumerable().ElementAt(rowIndex).Field<string>("syncBoolAddress"));
                var _lastSyncTime = (tbl.AsEnumerable().ElementAt(rowIndex).Field<DateTime>("lastSyncTime"));
                var _activePlcId = (int)tbl.AsEnumerable().ElementAt(rowIndex).Field<Int32>("id");
                var _active = tbl.AsEnumerable().ElementAt(rowIndex).Field<bool>("Active");

                var plcConfig = new PlcConfiguration(_name, _cpu, _ip,
                    _rack, _slot, _activePlcId,
                    _active, _syncTimeDbNr, _syncTimeDbOffset,
                    _syncBoolAddress, _syncActive, _lastSyncTime);

                var myPlc = new ExtendedPlc(plcConfig);
                PlcList.Add(myPlc);
            }

            if (PlcList.Count > 0)
                return true;
            else
                return false;
        }

        public static ExtendedPlc FindPlc(int plcId) => PlcList.Find(p => p.Id == plcId);

        #endregion
    }
}