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

        public static DataTable FetchValuesFromSql(string plcName = "")
        {
            if (AllTagsTable == null)
                AllTagsTable = new DataTable();

            var query = $"SELECT * FROM {SqlSettings.Default.Databas}.dbo.tagConfig";
            string connection = PlcConfig.ConnectionString();
            try
            {
                using (SqlConnection con = new SqlConnection(connection))
                {
                    if (plcName == "")
                    {
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {

                            con.Open();

                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                if (AllTagsTable != null)
                                    AllTagsTable.Clear();
                                da.Fill(AllTagsTable);
                            }
                            AllLogValues.Clear();
                            AllLogValues = (from DataRow dr in AllTagsTable.Rows
                                            select new TagDefinitions()
                                            {
                                                id = (int)dr["id"],
                                                active = (bool)dr["active"],
                                                name = dr["name"].ToString(),
                                                logType = (LogType)Enum.Parse(typeof(LogType), dr["logType"].ToString()),
                                                timeOfDay = (TimeSpan)dr["timeOfDay"],
                                                deadband = (float)((double)dr["deadband"]),
                                                plcName = dr["plcName"].ToString(),
                                                varType = (VarType)Enum.Parse(typeof(VarType), dr["varType"].ToString()),
                                                blockNr = (int)dr["blockNr"],
                                                dataType = (DataType)Enum.Parse(typeof(DataType), dr["dataType"].ToString()),
                                                startByte = (int)dr["startByte"],
                                                nrOfElements = (int)dr["nrOfElements"],
                                                bitAddress = (byte)dr["bitAddress"],
                                                logFreq = (LogFrequency)Enum.Parse(typeof(LogFrequency), dr["logFreq"].ToString()),
                                                LastLogTime = DateTime.MinValue,
                                                tagUnit = dr["tagUnit"].ToString(),
                                                eventId = (int)dr["eventId"],
                                                IsBooleanTrigger = (bool)dr["isBooleanTrigger"],
                                                boolTrigger = (BooleanTrigger)Enum.Parse(typeof(BooleanTrigger), dr["boolTrigger"].ToString()),
                                                analogTrigger = (AnalogTrigger)Enum.Parse(typeof(AnalogTrigger), dr["analogTrigger"].ToString()),
                                                analogValue = (float)((double)dr["analogValue"]),
                                            }).ToList();

                            // Sorterar listan alfabetiskt
                            AllLogValues.Sort((x, y) => string.Compare(x.name, y.name));

                            return AllTagsTable;
                        }
                    }
                    else
                    {
                        var tagTable = new DataTable();
                        query = $"SELECT * FROM {SqlSettings.Default.Databas}.dbo.tagConfig WHERE plcName = '{plcName}'";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            con.Open();
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(tagTable);
                            }
                            return tagTable;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                StatusEvent.RaiseMessage(ex.Message, Status.Error);
                return null;
            }
            catch (Exception ex)
            {
                StatusEvent.RaiseMessage(ex.Message, Status.Error);
                return null;
            }


        }

        /// <summary>
        /// Deletes all rows from dbo.plcConfig table
        /// </summary>
        /// <returns>True if successfull</returns>
        public static bool DeleteTagSql()
        {
            string query = @"Delete FROM " + SqlSettings.Default.Databas + ".dbo.tagConfig";
            using (SqlConnection con = new SqlConnection(PlcConfig.ConnectionString()))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                catch (SqlException ex)
                {
                    StatusEvent.RaiseMessage(ex.Message, Status.Error);
                    return false;
                }
            }
        }
    }
}
