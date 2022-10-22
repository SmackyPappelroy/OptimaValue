using OpcUaHm.Common;
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
                    tag_id = raw.logValue.Id,
                };
                sql.opcQuality = raw.ReadValue.Quality;
                sql.value = raw.ReadValue.ValueAsString;
                sql.numericValue = raw.ReadValue.ValueAsFloat;
                SqlValues.Add(sql);

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
                DatabaseSql.SendLogValuesToSql(tbl);
            }
        }



        public static void AddRawValue(ReadValue _readValue, TagDefinitions taggen)
        {
            lock (addBlock)
            {
                var val = new rawValueClass()
                {
                    logValue = taggen,
                    ReadValue = _readValue
                };
                rawValueBlock.Add(val);
            }
        }
    }
}
