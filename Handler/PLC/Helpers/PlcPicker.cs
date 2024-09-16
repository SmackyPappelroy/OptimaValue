using S7.Net;
using System;
using System.Data;
using System.Linq;

namespace OptimaValue
{
    public static class PlcPicker
    {

        #region Public Properties
        public static bool Active { get; private set; }
        public static string Name { get; private set; } = string.Empty;
        public static string IpAddress { get; private set; } = string.Empty;
        public static CpuType CpuType { get; private set; } = CpuType.S7300;
        public static short Rack { get; private set; } = 0;
        public static short Slot { get; private set; } = 0;
        public static int ActivePlcId { get; private set; } = int.MaxValue;
        public static int HighestPlcId { get; private set; } = int.MinValue;
        public static int LowestPlcId { get; private set; } = int.MaxValue;




        /// <summary>
        /// Nr of enbled PLCs in database config
        /// </summary>
        public static int NrActiveEnabledPlc
        {
            get
            {
                return PlcConfig.tblPlcConfig?.AsEnumerable()
                    .Count(row => row.Field<bool>("Active")) ?? 0;
            }
        }

        #endregion

        #region Private Properties
        private static bool MoreThanOnePlc = HighestPlcId != LowestPlcId;

        #endregion

        /// <summary>
        /// Väljer nästa aktiva PLC baserat på dess konfiguration i databasen.
        /// </summary>
        /// <returns>En instans av <see cref="ExtendedPlc"/> om en PLC hittas, annars <c>null</c>.</returns>
        public static ExtendedPlc SelectPlc()
        {
            var tbl = PlcConfig.tblPlcConfig;
            var activePlcRows = tbl.SelectRows("Active = 'True'", "id");
            HighestPlcId = activePlcRows.FindHighestNumber<int>("id");
            LowestPlcId = activePlcRows.FindLowestNumber<int>("id");
            if (HighestPlcId == default && LowestPlcId == default)
                return null;

            MoreThanOnePlc = (HighestPlcId != LowestPlcId);

            if (ActivePlcId == int.MaxValue || !MoreThanOnePlc)
            {
                ActivePlcId = LowestPlcId;
                return MapValues();
            }

            if (ActivePlcId != int.MaxValue)
            {
                ActivePlcId = tbl.SelectRows("Active = 'True'", "id")
                    .FindNextNumber<int>(ActivePlcId, "id");
                return MapValues();
            }

            return null;
        }

        private static ExtendedPlc MapValues()
        {
            var tbl = PlcConfig.tblPlcConfig;

            var row = tbl.AsEnumerable().FirstOrDefault(r => r.Field<int>("id") == ActivePlcId);

            if (row != null)
            {
                Active = row.Field<bool>("Active");
                Name = row.Field<string>("name");
                IpAddress = row.Field<string>("ipAddress");
                CpuType = Enum.Parse<CpuType>(row.Field<string>("cpuType"));
                Rack = row.Field<short?>("rack") ?? 0;
                Slot = row.Field<short?>("slot") ?? 0;
            }

            return PlcConfig.PlcList.FirstOrDefault(p => p.ActivePlcId == ActivePlcId);
        }


    }
}
