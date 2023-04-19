using OpcUaHm.Common;
using S7.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OptimaValue
{
    public static class SendValuesToSql
    {
        private static ConcurrentBag<RawValueClass> rawValueBlock;
        private static CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        private static Task sqlTask;
        private static List<RawValueClass> rawValueInternal;
        private static readonly object sqlLock = new object();
        private static DateTime lastLogTime = DateTime.MinValue;
        private static List<LogValuesSql> SqlValues;

        public static void StartSql()
        {
            sqlTask = Task.Run(async () => await SqlCycler(cancelTokenSource.Token), cancelTokenSource.Token)
                .ContinueWith(HandleTaskCancellation, TaskContinuationOptions.OnlyOnCanceled);
        }

        private static void HandleTaskCancellation(Task t)
        {
            t.Exception?.Handle(e => true);
            AbortSqlThread();
            Console.WriteLine("You have canceled the task");
            cancelTokenSource = new CancellationTokenSource();
        }

        private static async Task SqlCycler(CancellationToken cancellationToken)
        {
            InitializeCollectionsIfNeeded();

            while (true)
            {
                ProcessRawValueBlock();

                if (ShouldMapValues(DateTime.UtcNow))
                {
                    MapValue();
                    lastLogTime = DateTime.UtcNow;
                }

                if (Master.AbortSqlLog)
                    RequestDisconnect();

                await Task.Delay(50);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private static void InitializeCollectionsIfNeeded()
        {
            if (rawValueInternal == null)
                rawValueInternal = new List<RawValueClass>();

            if (rawValueBlock == null)
                rawValueBlock = new ConcurrentBag<RawValueClass>();
        }

        private static void ProcessRawValueBlock()
        {
            if (rawValueBlock.Count > 0 && rawValueBlock.TryTake(out RawValueClass newRaw))
                rawValueInternal.Add(newRaw);
        }

        private static bool ShouldMapValues(DateTime currentTime)
        {
            return rawValueInternal.Count > 0 && lastLogTime.AddSeconds(10) < currentTime
                || (Master.AbortSqlLog && rawValueInternal.Count > 0);
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
                if (rawValueBlock.TryTake(out RawValueClass newRaw))
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

            SqlValues.Clear();
            SqlValues.AddRange(ExtractLogValuesFromRawValues(rawValueInternal));
            rawValueInternal.Clear();

            LogToSql();

            return SqlValues.Count;
        }

        private static List<LogValuesSql> ExtractLogValuesFromRawValues(List<RawValueClass> rawValues)
        {
            var logValues = new List<LogValuesSql>();

            foreach (RawValueClass raw in rawValues)
            {
                logValues.Add(new LogValuesSql
                {
                    logTime = raw.logValue.LastLogTime,
                    tag_id = raw.logValue.Id,
                    opcQuality = raw.ReadValue.Quality,
                    value = raw.ReadValue.ValueAsString,
                    numericValue = raw.ReadValue.ValueAsFloat
                });
            }
            return logValues;
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

        public static void AddRawValue(ReadValue readValue, TagDefinitions tag)
        {
            var val = new RawValueClass
            {
                logValue = tag,
                ReadValue = readValue
            };
            rawValueBlock.Add(val);
        }
    }

    public class RawValueClass
    {
        public TagDefinitions logValue { get; set; }
        public ReadValue ReadValue { get; set; }
    }
}
