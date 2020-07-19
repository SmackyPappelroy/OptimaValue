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
            highestPlcId = tbl.SelectRows("Active = 'True'", "id").FindHighestNumber<int>("id");
            lowestPlcId = tbl.SelectRows("Active = 'True'", "id").FindLowestNumber<int>("id");
            if (highestPlcId == default && lowestPlcId == default)
                return null;

            if (highestPlcId != lowestPlcId)
                MoreThanOnePlc = true;
            else
                MoreThanOnePlc = false;

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

            for (int rowIndex = 0; rowIndex < tbl.Rows.Count; rowIndex++)
            {
                if (tbl.AsEnumerable().ElementAt(rowIndex).Field<Int32>("id") == activePlcId)
                {
                    Active = tbl.AsEnumerable().ElementAt(rowIndex).Field<bool>("Active");
                    name = (tbl.AsEnumerable().ElementAt(rowIndex).Field<string>("name"));
                    ipAddress = (tbl.AsEnumerable().ElementAt(rowIndex).Field<string>("ipAddress"));
                    cpuType = (CpuType)Enum.Parse(typeof(CpuType), (tbl.AsEnumerable().ElementAt(rowIndex).Field<string>("cpuType")));
                    rack = (tbl.AsEnumerable().ElementAt(rowIndex).Field<short>("rack"));
                    slot = (tbl.AsEnumerable().ElementAt(rowIndex).Field<short>("slot"));
                }
            }
            foreach (var plc in PlcConfig.PlcList)
            {
                if (plc.ActivePlcId == activePlcId)
                {
                    return plc;
                }
            }
            return null;
        }

    }
}
