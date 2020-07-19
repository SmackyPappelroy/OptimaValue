using S7.Net;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;

namespace OptimaValue
{
    public static class SendValuesToSql
    {
        public static BlockingCollection<rawValueClass> rawValueBlock;

        private static Thread sqlThread;
        private static object addBlock = new object();

        private static List<LogValuesSql> SqlValues;
        private static List<rawValueClass> rawValueInternal;
        private static object sqlLock = new object();
        private static DateTime lastLogTime = DateTime.MinValue;

        public static void StartSql()
        {
            if (sqlThread == null)
                sqlThread = new Thread(SqlCycler);
            else if (sqlThread.ThreadState != ThreadState.Running)
            {
                sqlThread = null;
                sqlThread = new Thread(SqlCycler);
            }

            sqlThread.Start();
        }

        private static void SqlCycler()
        {
            if (rawValueInternal == null)
                rawValueInternal = new List<rawValueClass>();

            while (true)
            {
                if (rawValueBlock == null)
                    rawValueBlock = new BlockingCollection<rawValueClass>();

                if (rawValueBlock.Count > 0)
                {
                    if (rawValueBlock.TryTake(out rawValueClass newRaw))
                        rawValueInternal.Add(newRaw);
                }

                if (rawValueInternal.Count > 0 && lastLogTime.AddSeconds(10) < DateTime.Now
                    || (Master.AbortSqlLog && rawValueInternal.Count > 0))
                {
                    MapValue();
                    lastLogTime = DateTime.Now;
                }
                if (Master.AbortSqlLog && rawValueBlock.Count == 0)
                {
                    AbortSqlThread();
                }
            }
        }

        private static void AbortSqlThread()
        {
            Master.AbortSqlLog = false;
            sqlThread.Abort();
        }

        public static int MapValue()
        {
            if (SqlValues == null)
                SqlValues = new List<LogValuesSql>();

            var startRows = SqlValues.Count;
            SqlValues.Clear();

            foreach (rawValueClass raw in rawValueInternal)
            {
                var sql = new LogValuesSql()
                {
                    logTime = raw.logValue.LastLogTime,
                    tagName = raw.logValue.name,
                    plcName = raw.logValue.plcName
                };

                switch (raw.logValue.varType)
                {
                    case VarType.Bit: // klar
                        if (raw.unknownTag is BitArray)
                        {
                            for (int i = 0; i < ((BitArray)raw.unknownTag).Length; i++)
                            {
                                sql.value = ((BitArray)raw.unknownTag)[i] == true ? "True" : "False";
                                sql.numericValue = ((BitArray)raw.unknownTag)[i] == true ? 1 : 0;
                                sql.tagName = $"{raw.logValue.name}[{i}]";
                                SqlValues.Add(sql);
                            }
                        }
                        else
                        {
                            sql.value = (bool)raw.unknownTag == true ? "True" : "False";
                            sql.numericValue = (bool)raw.unknownTag == true ? 1 : 0;
                            SqlValues.Add(sql);
                        }

                        break;
                    case VarType.Byte: // klar
                        if (raw.unknownTag is byte[])
                        {
                            for (int i = 0; i < ((byte[])raw.unknownTag).Length; i++)
                            {
                                sql.value = ((byte[])raw.unknownTag)[i].ToString();
                                sql.numericValue = ((byte[])raw.unknownTag)[i];
                                sql.tagName = $"{raw.logValue.name}[{i}]";
                                SqlValues.Add(sql);
                            }
                        }
                        else
                        {
                            sql.value = ((byte)raw.unknownTag).ToString();
                            sql.numericValue = (byte)raw.unknownTag;
                            SqlValues.Add(sql);
                        }
                        break;
                    case VarType.Word: // klar
                        if (raw.unknownTag is ushort[])
                        {
                            for (int i = 0; i < ((ushort[])raw.unknownTag).Length; i++)
                            {
                                sql.numericValue = ((ushort[])raw.unknownTag)[i];
                                sql.value = sql.numericValue.ToString();
                                sql.tagName = $"{raw.logValue.name}[{i}]";
                                SqlValues.Add(sql);
                            }
                        }
                        else
                        {
                            sql.numericValue = (ushort)raw.unknownTag;
                            sql.value = sql.numericValue.ToString();
                            SqlValues.Add(sql);
                        }

                        break;
                    case VarType.DWord: // klar
                        if (raw.unknownTag is uint[])
                        {
                            for (int i = 0; i < ((uint[])raw.unknownTag).Length; i++)
                            {
                                sql.numericValue = ((uint[])(raw.unknownTag))[i];
                                sql.value = sql.numericValue.ToString();
                                sql.tagName = $"{raw.logValue.name}[{i}]";
                                SqlValues.Add(sql);
                            }
                        }
                        else
                        {
                            sql.numericValue = (uint)raw.unknownTag;
                            sql.value = sql.numericValue.ToString();
                            SqlValues.Add(sql);
                        }

                        break;
                    case VarType.Int: // klar
                        if (raw.unknownTag is short[])
                        {
                            for (int i = 0; i < ((short[])raw.unknownTag).Length; i++)
                            {
                                sql.numericValue = ((short[])raw.unknownTag)[i];
                                sql.value = sql.numericValue.ToString();
                                sql.tagName = $"{raw.logValue.name}[{i}]";
                                SqlValues.Add(sql);
                            }
                        }
                        else
                        {
                            sql.numericValue = (short)raw.unknownTag;
                            sql.value = sql.numericValue.ToString();
                            SqlValues.Add(sql);
                        }
                        break;
                    case VarType.DInt: // klar
                        if (raw.unknownTag is int[])
                        {
                            for (int i = 0; i < ((int[])raw.unknownTag).Length; i++)
                            {
                                sql.numericValue = ((int[])(raw.unknownTag))[i];
                                sql.value = sql.numericValue.ToString();
                                sql.tagName = $"{raw.logValue.name}[{i}]";
                                SqlValues.Add(sql);
                            }
                        }
                        else
                        {
                            sql.numericValue = (int)raw.unknownTag;
                            sql.value = sql.numericValue.ToString();
                            SqlValues.Add(sql);
                        }
                        break;
                    case VarType.Real: //Klar
                        if (raw.unknownTag is float[])
                        {
                            for (int i = 0; i < ((float[])raw.unknownTag).Length; i++)
                            {
                                sql.numericValue = ((float[])(raw.unknownTag))[i];
                                sql.value = sql.numericValue.ToString();
                                sql.tagName = $"{raw.logValue.name}[{i}]";
                                SqlValues.Add(sql);
                            }
                        }
                        else
                        {
                            sql.numericValue = (float)raw.unknownTag;
                            sql.value = sql.numericValue.ToString();
                            SqlValues.Add(sql);
                        }
                        break;
                    case VarType.String:
                        sql.numericValue = 0;
                        sql.value = raw.unknownTag.ToString();
                        SqlValues.Add(sql);
                        break;
                    case VarType.StringEx:
                        sql.numericValue = 0;
                        sql.value = raw.unknownTag.ToString();
                        SqlValues.Add(sql);
                        break;
                    case VarType.Timer:
                        break;
                    case VarType.Counter:
                        break;
                    default:
                        break;
                }
            }

            rawValueInternal.Clear();

            LogToSql();

            return SqlValues.Count - startRows;


        }

        private static void LogToSql()
        {
            var tbl = SqlValues.ConvertToDataTable<LogValuesSql>();

            lock (sqlLock)
            {
                if (tbl.Rows.Count == 0)
                    return;

                string connection = PlcConfig.ConnectionString();

                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlBulkCopy objBulk = new SqlBulkCopy(connection))
                    {
                        objBulk.DestinationTableName = "logValues";
                        objBulk.ColumnMappings.Add("tagName", "tagName");
                        objBulk.ColumnMappings.Add("plcName", "plcName");
                        objBulk.ColumnMappings.Add("logTime", "logTime");
                        objBulk.ColumnMappings.Add("value", "value");
                        objBulk.ColumnMappings.Add("numericValue", "numericValue");
                        try
                        {
                            con.Open();
                            objBulk.WriteToServer(tbl);
                        }
                        catch (SqlException ex)
                        {
                            $"Problem vid lagring till Sql \n\r{ex.Message}".SendThisStatusMessage(Status.Error);
                        }
                    }
                }
            }
        }

        public static void AddRawValue(object unknownObject, TagDefinitions taggen)
        {
            lock (addBlock)
            {
                var val = new rawValueClass()
                {
                    logValue = taggen,
                    unknownTag = unknownObject
                };
                rawValueBlock.Add(val);
            }
        }
    }
}
