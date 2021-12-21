using S7.Net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace OptimaValue
{
    public static class TagsToLog
    {
        #region Lists
        public static DataTable AllTagsTable;

        public static List<TagDefinitions> AllLogValues { get; set; } = new List<TagDefinitions>();
        #endregion


        /// <summary>
        /// Gets all tags from sql or tags for one or all <see cref="ExtendedPlc"/>
        /// </summary>
        /// <param name="plcName"></param>
        /// <returns></returns>
        public static DataTable GetAllTagsFromSql(string plcName = "")
        {
            if (AllTagsTable == null)
                AllTagsTable = new DataTable();

            AllTagsTable = DatabaseSql.GetTags(plcName);

            if (plcName == string.Empty)
            {
                AllLogValues = TagTableToList(AllTagsTable);
            }

            return AllTagsTable;
        }

        /// <summary>
        /// Converts Datatable to a list of <see cref="TagDefinitions"/>/>
        /// </summary>
        /// <param name="tbl"></param>
        /// <returns></returns>
        private static List<TagDefinitions> TagTableToList(DataTable tbl)
        {
            var logValues = new List<TagDefinitions>();
            logValues.Clear();
            logValues = (from DataRow dr in AllTagsTable.Rows
                         select new TagDefinitions()
                         {
                             Id = (int)dr["id"],
                             Active = (bool)dr["active"],
                             Name = dr["name"].ToString(),
                             LogType = (LogType)Enum.Parse(typeof(LogType), dr["logType"].ToString()),
                             TimeOfDay = (TimeSpan)dr["timeOfDay"],
                             Deadband = (float)((double)dr["deadband"]),
                             PlcName = dr["plcName"].ToString(),
                             VarType = (VarType)Enum.Parse(typeof(VarType), dr["varType"].ToString()),
                             BlockNr = (int)dr["blockNr"],
                             DataType = (DataType)Enum.Parse(typeof(DataType), dr["dataType"].ToString()),
                             StartByte = (int)dr["startByte"],
                             NrOfElements = (int)dr["nrOfElements"],
                             BitAddress = (byte)dr["bitAddress"],
                             LogFreq = (LogFrequency)Enum.Parse(typeof(LogFrequency), dr["logFreq"].ToString()),
                             LastLogTime = DateTime.MinValue,
                             TagUnit = dr["tagUnit"].ToString(),
                             EventId = (int)dr["eventId"],
                             IsBooleanTrigger = (bool)dr["isBooleanTrigger"],
                             BoolTrigger = (BooleanTrigger)Enum.Parse(typeof(BooleanTrigger), dr["boolTrigger"].ToString()),
                             AnalogTrigger = (AnalogTrigger)Enum.Parse(typeof(AnalogTrigger), dr["analogTrigger"].ToString()),
                             AnalogValue = (float)((double)dr["analogValue"]),
                             scaleMin = (int)dr["scaleMin"],
                             scaleMax = (int)dr["scaleMax"],
                             scaleOffset = (int)dr["scaleOffset"],
                         }).ToList();

            // Sorterar listan alfabetiskt
            logValues.Sort((x, y) => string.Compare(x.Name, y.Name));

            return logValues;
        }
    }
}
