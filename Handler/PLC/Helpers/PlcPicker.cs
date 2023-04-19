using S7.Net;
using System;
using System.Data;
using System.Linq;

namespace OptimaValue
{
    public static class PlcPicker
    {

        #region Public Properties
        public static bool Active = false;
        public static string name = string.Empty;
        public static string ipAddress = string.Empty;
        public static CpuType cpuType = CpuType.S7300;
        public static short rack = 0;
        public static short slot = 0;
        public static Int32 activePlcId = Int32.MaxValue;
        public static Int32 highestPlcId = Int32.MinValue;
        public static Int32 lowestPlcId = Int32.MaxValue;



        /// <summary>
        /// Nr of enbled PLCs in database config
        /// </summary>
        public static int NrActiveEnabledPlc
        {
            get
            {
                var nr = 0;
                if (PlcConfig.tblPlcConfig == null)
                    return nr;
                else
                {
                    for (int rowIndex = 0; rowIndex < PlcConfig.tblPlcConfig.Rows.Count; rowIndex++)
                    {

                        if ((bool)PlcConfig.tblPlcConfig.Rows[rowIndex][1])
                            nr++;
                    }
                    return nr;
                }
            }
        }
        #endregion

        #region Private Properties
        private static bool MoreThanOnePlc = false;
        #endregion

        /// <summary>
        /// Finds the next <see cref="Plc"/> from the Sql table
        /// </summary>
        /// <returns><code>True</code>If successfull</returns>
        public static ExtendedPlc SelectPlc()
        {
            var tbl = PlcConfig.tblPlcConfig;
            var activePlcRows = tbl.SelectRows("Active = 'True'", "id");
            highestPlcId = activePlcRows.FindHighestNumber<int>("id");
            lowestPlcId = activePlcRows.FindLowestNumber<int>("id");
            if (highestPlcId == default && lowestPlcId == default)
                return null;

            MoreThanOnePlc = (highestPlcId != lowestPlcId);

            if (activePlcId == int.MaxValue || !MoreThanOnePlc)
            {
                activePlcId = lowestPlcId;
                return MapValues();
            }

            if (activePlcId != int.MaxValue)
            {
                activePlcId = tbl.SelectRows("Active = 'True'", "id").FindNextNumber<int>(activePlcId, "id");
                return MapValues();
            }

            return null;
        }

        private static ExtendedPlc MapValues()
        {
            var tbl = PlcConfig.tblPlcConfig;

            var row = tbl.AsEnumerable().FirstOrDefault(r => r.Field<int>("id") == activePlcId);

            if (row != null)
            {
                Active = row.Field<bool>("Active");
                name = row.Field<string>("name");
                ipAddress = row.Field<string>("ipAddress");
                cpuType = Enum.Parse<CpuType>(row.Field<string>("cpuType"));
                rack = row.Field<short>("rack");
                slot = row.Field<short>("slot");
            }

            return PlcConfig.PlcList.FirstOrDefault(p => p.ActivePlcId == activePlcId);
        }


    }
}
