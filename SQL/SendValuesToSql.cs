using S7.Net;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace OptimaValue
{
    public static class SendValuesToSql
    {
        public static ConcurrentBag<rawValueClass> rawValueBlock;

        private static Task sqlTask;
        private static CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        private static readonly object addBlock = new object();

        private static List<LogValuesSql> SqlValues;
        private static List<rawValueClass> rawValueInternal;
        private static readonly object sqlLock = new object();
        private static DateTime lastLogTime = DateTime.MinValue;

        public static void StartSql()
        {
            sqlTask = Task.Run(async () =>
            {
                await SqlCycler(cancelTokenSource.Token).ConfigureAwait(false);
            }, cancelTokenSource.Token)
                .ContinueWith(t =>
                {
                    t.Exception?.Handle(e => true);
                    AbortSqlThread();
                    Console.WriteLine("You have canceled the task");
                    cancelTokenSource = new CancellationTokenSource();
                }, TaskContinuationOptions.OnlyOnCanceled);
        }

        private static async Task SqlCycler(CancellationToken cancellationToken)
        {
            if (rawValueInternal == null)
                rawValueInternal = new List<rawValueClass>();

            while (true)
            {
                var tiden = DateTime.UtcNow;
                if (rawValueBlock == null)
                    rawValueBlock = new ConcurrentBag<rawValueClass>();

                if (rawValueBlock.Count > 0)
                {

                    if (rawValueBlock.TryTake(out rawValueClass newRaw))
                        rawValueInternal.Add(newRaw);
                }

                if (rawValueInternal.Count > 0 && lastLogTime.AddSeconds(10) < tiden
                    || (Master.AbortSqlLog && rawValueInternal.Count > 0))
                {
                    MapValue();
                    lastLogTime = tiden;
                }
                if (Master.AbortSqlLog)
                    RequestDisconnect();

                await Task.Delay(50);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        public static void RequestDisconnect()
        {
            if (sqlTask != null && !cancelTokenSource.IsCancellationRequested)
                cancelTokenSource.Cancel();
        }

        private static void AbortSqlThread()
        {
            while (rawValueBlock.Count > 0)
            {
                if (rawValueBlock.TryTake(out rawValueClass newRaw))
                    rawValueInternal.Add(newRaw);
                MapValue();
                Thread.Sleep(10);
            }
            Master.AbortSqlLog = false;
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
                    tag_id = raw.logValue.id
                };

                switch (raw.logValue.varType)
                {
                    case VarType.Bit: // klar
                        if (raw.unknownTag is BitArray array)
                        {
                            for (int i = 0; i < array.Length; i++)
                            {
                                sql.value = array[i] == true ? "True" : "False";
                                sql.numericValue = array[i] == true ? 1 : 0;
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
                        if (raw.unknownTag is byte[] v)
                        {
                            for (int i = 0; i < v.Length; i++)
                            {
                                sql.value = v[i].ToString();
                                sql.numericValue = v[i];
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
                        if (raw.unknownTag is ushort[] v1)
                        {
                            for (int i = 0; i < v1.Length; i++)
                            {
                                sql.numericValue = v1[i];
                                sql.value = sql.numericValue.ToString();
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
                        if (raw.unknownTag is uint[] v2)
                        {
                            for (int i = 0; i < v2.Length; i++)
                            {
                                sql.numericValue = v2[i];
                                sql.value = sql.numericValue.ToString();
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
                        if (raw.unknownTag is short[] v3)
                        {
                            for (int i = 0; i < v3.Length; i++)
                            {
                                sql.numericValue = v3[i];
                                sql.value = sql.numericValue.ToString();
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
                        if (raw.unknownTag is int[] v4)
                        {
                            for (int i = 0; i < v4.Length; i++)
                            {
                                sql.numericValue = v4[i];
                                sql.value = sql.numericValue.ToString();
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
                        if (raw.unknownTag is float[] v5)
                        {
                            for (int i = 0; i < v5.Length; i++)
                            {
                                sql.numericValue = v5[i];
                                sql.value = sql.numericValue.ToString();
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
                    case VarType.S7String:
                        sql.numericValue = 0;
                        sql.value = raw.unknownTag.ToString();
                        SqlValues.Add(sql);
                        break;
                    case VarType.Timer:
                        if (raw.unknownTag is double v6)
                        {
                            sql.numericValue = (float)v6;
                            sql.value = sql.numericValue.ToString();
                            SqlValues.Add(sql);
                        }
                        break;
                    case VarType.Counter:
                        if (raw.unknownTag is ushort v7)
                        {
                            sql.numericValue = v7;
                            sql.value = sql.numericValue.ToString();
                            SqlValues.Add(sql);
                        }
                        break;
                    case VarType.DateTime:
                        if (raw.unknownTag is System.DateTime v8)
                        {
                            sql.numericValue = 0;
                            sql.value = v8.ToString();
                            SqlValues.Add(sql);
                        }
                        break;
                    case VarType.DateTimeLong:
                        if (raw.unknownTag is System.DateTime v9)
                        {
                            sql.numericValue = 0;
                            sql.value = v9.ToString();
                            SqlValues.Add(sql);
                        }
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

            if (tbl.Rows.Count == 0)
                return;

            lock (sqlLock)
            {
                string connection = PlcConfig.ConnectionString();

                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlBulkCopy objBulk = new SqlBulkCopy(connection))
                    {
                        objBulk.DestinationTableName = "logValues";
                        objBulk.ColumnMappings.Add("logTime", "logTime");
                        objBulk.ColumnMappings.Add("value", "value");
                        objBulk.ColumnMappings.Add("numericValue", "numericValue");
                        objBulk.ColumnMappings.Add("tag_id", "tag_id");
                        try
                        {
                            con.Open();
                            objBulk.WriteToServer(tbl);
                        }
                        catch (SqlException ex)
                        {
                            $"Problem vid lagring till Sql \n\r{ex.Message}".SendThisStatusMessage(Severity.Error);
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
