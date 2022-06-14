using LiveCharts.Defaults;
using LiveCharts.Geared;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OptimaValue.Config;
using System.Threading;
using System.Diagnostics;

namespace OptimaValue.Wpf
{
    public static class ChartData
    {
        static ChartData()
        {
            BuildStoredProcedureString();
        }
        private static string queryAllValuesString;
        private static string queryNewValuesString;

        private static string createStoredProcedureString;

        private static GraphWindow graphWindow = Master.GetService<GraphWindow>();

        public static DataTable ChartTableAllTags;
        public static DataTable ChartTableAllTagsNoFill;

        public static DateTime MinDate => ChartTableAllTags.AsEnumerable().Min(x => x.Field<DateTime>("logTime"));
        public static DateTime MaxDate => ChartTableAllTags.AsEnumerable().Max(x => x.Field<DateTime>("logTime"));

        public static bool HasData => ChartTableAllTags != null && ChartTableAllTags.Rows.Count > 0;
        public static bool HasNewData { private set; get; }

        public static List<string> DisplayedTags;

        public static event Action<bool> OnChartChanged;

        private static void BuildQueryAllValuesString(DateTime startDate, DateTime stopDate)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"DECLARE @columns NVARCHAR(MAX), @sql NVARCHAR(MAX);");
            sb.AppendLine(@"SET @columns = N'';");
            sb.AppendLine(@"SELECT @columns+=N', p.'+QUOTENAME([Name])");
            sb.AppendLine(@"FROM");
            sb.AppendLine(@"(");
            sb.AppendLine(@"    SELECT name AS [Name]");
            sb.AppendLine(@$"    FROM  (SELECT {SqlSettings.Databas}.dbo.tagConfig.name, {SqlSettings.Databas}.dbo.logValues.numericValue, {SqlSettings.Databas}.dbo.logValues.logTime");
            sb.AppendLine($"	FROM {SqlSettings.Databas}.dbo.logValues INNER JOIN {SqlSettings.Databas}.dbo.tagConfig ON {SqlSettings.Databas}.dbo.logValues.tag_id = {SqlSettings.Databas}.dbo.tagConfig.id WHERE {SqlSettings.Databas}.dbo.logValues.logTime >= '{startDate}' AND {SqlSettings.Databas}.dbo.logValues.logTime <= '{stopDate}') AS p");
            sb.AppendLine(@"    GROUP BY [Name]");
            sb.AppendLine(@") AS x;");
            sb.AppendLine(@"SET @sql = N'");
            sb.AppendLine(@"SELECT DATEADD(ms, -DATEPART(ms, logTime), logTime) as logTime, '+STUFF(@columns, 1, 2, '')+' FROM (");
            sb.AppendLine(@"SELECT DATEADD(ms, -DATEPART(ms, logTime), logTime) as [logTime], [numericValue] AS [numericValue], [name] as [Name] ");
            sb.AppendLine($"    FROM {SqlSettings.Databas}.dbo.logValues INNER JOIN {SqlSettings.Databas}.dbo.tagConfig ON {SqlSettings.Databas}.dbo.logValues.tag_id = {SqlSettings.Databas}.dbo.tagConfig.id WHERE {SqlSettings.Databas}.dbo.logValues.logTime >= ''{startDate}'' AND {SqlSettings.Databas}.dbo.logValues.logTime <= ''{stopDate}'') AS j PIVOT (SUM(numericValue) FOR [Name] in ");
            sb.AppendLine(@"	   ('+STUFF(REPLACE(@columns, ', p.[', ',['), 1, 1, '')+')) AS p;';");
            sb.AppendLine(@"EXEC sp_executesql @sql");

            queryAllValuesString = sb.ToString();
        }

        public static void BuildQueryNewValuesString()
        {
            DateTime lastTime = new();
            lastTime = ChartTableAllTags.AsEnumerable().Max(x => x.Field<DateTime>("logTime"));

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"DECLARE @columns NVARCHAR(MAX), @sql NVARCHAR(MAX);");
            sb.AppendLine(@"SET @columns = N'';");
            sb.AppendLine(@"SELECT @columns+=N', p.'+QUOTENAME([Name])");
            sb.AppendLine(@"FROM");
            sb.AppendLine(@"(");
            sb.AppendLine(@"    SELECT name AS [Name]");
            sb.AppendLine(@$"    FROM  (SELECT {SqlSettings.Databas}.dbo.tagConfig.name, {SqlSettings.Databas}.dbo.logValues.numericValue, {SqlSettings.Databas}.dbo.logValues.logTime");
            sb.AppendLine($"	FROM {SqlSettings.Databas}.dbo.logValues INNER JOIN {SqlSettings.Databas}.dbo.tagConfig ON {SqlSettings.Databas}.dbo.logValues.tag_id = {SqlSettings.Databas}.dbo.tagConfig.id WHERE {SqlSettings.Databas}.dbo.logValues.logTime > '{lastTime}') AS p");
            sb.AppendLine(@"    GROUP BY [Name]");
            sb.AppendLine(@") AS x;");
            sb.AppendLine(@"SET @sql = N'");
            sb.AppendLine(@"SELECT DATEADD(ms, -DATEPART(ms, logTime), logTime) as logTime, '+STUFF(@columns, 1, 2, '')+' FROM (");
            sb.AppendLine(@"SELECT DATEADD(ms, -DATEPART(ms, logTime), logTime) as [logTime], [numericValue] AS [numericValue], [name] as [Name] ");
            sb.AppendLine($"    FROM {SqlSettings.Databas}.dbo.logValues INNER JOIN {SqlSettings.Databas}.dbo.tagConfig ON {SqlSettings.Databas}.dbo.logValues.tag_id = {SqlSettings.Databas}.dbo.tagConfig.id WHERE {SqlSettings.Databas}.dbo.logValues.logTime > ''{lastTime}'') AS j PIVOT (SUM(numericValue) FOR [Name] in ");
            sb.AppendLine(@"	   ('+STUFF(REPLACE(@columns, ', p.[', ',['), 1, 1, '')+')) AS p;';");
            sb.AppendLine(@"EXEC sp_executesql @sql");

            queryNewValuesString = sb.ToString();
        }

        private static void BuildStoredProcedureString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"CREATE PROCEDURE spChartData");
            sb.AppendLine(@"AS");
            sb.AppendLine(@"BEGIN");
            sb.AppendLine(@"DECLARE @columns NVARCHAR(MAX), @sql NVARCHAR(MAX);");
            sb.AppendLine(@"SET @columns = N'';");
            sb.AppendLine(@"SELECT @columns+=N', p.'+QUOTENAME([Name])");
            sb.AppendLine(@"FROM");
            sb.AppendLine(@"(");
            sb.AppendLine(@"    SELECT name AS [Name]");
            sb.AppendLine(@$"    FROM  (SELECT {SqlSettings.Databas}.dbo.tagConfig.name, {SqlSettings.Databas}.dbo.logValues.numericValue, {SqlSettings.Databas}.dbo.logValues.logTime");
            sb.AppendLine(@$"	FROM {SqlSettings.Databas}.dbo.logValues INNER JOIN {SqlSettings.Databas}.dbo.tagConfig ON {SqlSettings.Databas}.dbo.logValues.tag_id = {SqlSettings.Databas}.dbo.tagConfig.id) AS p");
            sb.AppendLine(@"    GROUP BY [Name]");
            sb.AppendLine(@") AS x;");
            sb.AppendLine(@"SET @sql = N'");
            sb.AppendLine(@"SELECT DATEADD(ms, -DATEPART(ms, logTime), logTime) as logTime, '+STUFF(@columns, 1, 2, '')+' FROM (");
            sb.AppendLine(@"SELECT DATEADD(ms, -DATEPART(ms, logTime), logTime) as [logTime], [numericValue] AS [numericValue], [name] as [Name] ");
            sb.AppendLine(@$"    FROM [{SqlSettings.Databas}].[dbo].[vLogValues]) AS j PIVOT (SUM(numericValue) FOR [Name] in ");
            sb.AppendLine(@"	   ('+STUFF(REPLACE(@columns, ', p.[', ',['), 1, 1, '')+')) AS p;';");
            sb.AppendLine(@"EXEC sp_executesql @sql");
            sb.AppendLine(@"END");

            createStoredProcedureString = sb.ToString();
        }

        public static void CreateStoredProcedure()
        {
            try
            {
                using SqlConnection con = new SqlConnection(Config.SqlMethods.ConnectionString);
                using SqlCommand cmd = new SqlCommand(createStoredProcedureString, con);

                con.Open();

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

            }
        }
        private static DataTable oldChartValues = new();
        private static List<string> oldDisplayed = new();
        private static DateTime lastUpdateTime = DateTime.Now;
        private static DateTime lastStartTime = DateTime.MinValue;
        private static DateTime lastStopTime = DateTime.MinValue;
        public static async Task<DataTable> GetChartDataAsync(DateTime startTime, DateTime stopTime, bool fillEmptyValues = false)
        {
            if (lastStartTime == DateTime.MinValue)
            {
                lastStartTime = startTime;
                lastStopTime = stopTime;
            }
            else
            {
                // TODO: Hämta fler värden när man zoomar in
                // Timespan is less then previous
                if (startTime > lastStartTime && stopTime < lastStopTime)
                {
                    if (fillEmptyValues)
                        return ChartTableAllTags;
                    else
                        return ChartTableAllTagsNoFill;
                }
            }
            while (DateTime.Now.Subtract(lastUpdateTime) < TimeSpan.FromMilliseconds(50))
            {
                await Task.Delay(10);
            }
            var connectionString = Config.SqlMethods.ConnectionString;
            //#if DEBUG
            //            connectionString = (@"Server=DESKTOP-4OD098D\MINSERVER;Database=MCValueLog;User Id=sa;Password=sa; ");
            //#endif
            try
            {
                if (!await AnyDataBetweenDatesAsync(startTime, stopTime))
                {
                    lastUpdateTime = DateTime.Now;
                    lastStartTime = startTime;
                    lastStopTime = stopTime;
                    return null;
                }
            }
            catch (Exception) { lastUpdateTime = DateTime.Now; }


            BuildQueryAllValuesString(startTime, stopTime);
            if (ChartTableAllTags != null)
            {
                if (ChartTableAllTags.Rows.Count > 0)
                {
                    oldChartValues = ChartTableAllTags;
                }
            }
            if (DisplayedTags != null)
            {
                if (DisplayedTags.Count > 0)
                {
                    oldDisplayed = DisplayedTags;
                }
            }

            ChartTableAllTags = new();
            ChartTableAllTagsNoFill = new();
            DisplayedTags = new();

            DataTable tbl = new DataTable();
            try
            {
                using SqlConnection con = new SqlConnection(Config.SqlMethods.ConnectionString);
                using SqlCommand cmd = new SqlCommand(queryAllValuesString, con);

                con.Open();
                var reader = await cmd.ExecuteReaderAsync();
                tbl.Load(reader);

                tbl.DefaultView.Sort = "logTime";
                tbl = tbl.DefaultView.ToTable();

                ChartTableAllTags = tbl;

                var numberOfRows = Convert.ToInt32((stopTime - startTime).TotalSeconds);

                ChartTableAllTagsNoFill = ChartTableAllTags.Copy();

                ChartTableAllTags = ChartTableAllTags.FillGapsForwards(numberOfRows, stopTime - startTime, startTime, stopTime, graphWindow.MaxPointsPerSeries);

                //CreateStoredProcedure();


                Log.Logger.Information($"Tabell med all taggar: {ChartTableAllTags.Rows.Count} stycken");
                Debug.WriteLine($"Tabell med all taggar: {ChartTableAllTags.Rows.Count} stycken");

                if (fillEmptyValues)
                    return ChartTableAllTags;
                else
                    return ChartTableAllTagsNoFill;
            }
            catch (Exception ex)
            {
                Log.Error($"Lyckas ej hämta logvärden från databas i Chartdata.{nameof(GetChartDataAsync)}: {ex.Message}");
                if (oldChartValues != null)
                {
                    ChartTableAllTags = oldChartValues;
                }
                if (oldDisplayed != null)
                {
                    DisplayedTags = oldDisplayed;
                }
                return null;
            }
            finally
            {
                lastUpdateTime = DateTime.Now;
                lastStartTime = startTime;
                lastStopTime = stopTime;
            }
        }

        private static Task<bool> AnyDataBetweenDatesAsync(DateTime startTime, DateTime stopTime)
        {
            try
            {
                using SqlConnection con = new SqlConnection(Config.SqlMethods.ConnectionString);
                var startTimeParameter = new SqlParameter("@startTime", SqlDbType.DateTime) { Value = startTime };
                var stopTimeParameter = new SqlParameter("@stopTime", SqlDbType.DateTime) { Value = stopTime };
                using SqlCommand cmd = new SqlCommand(@$"SELECT COUNT(*) FROM [{SqlSettings.Databas}].[dbo].[logValues] WHERE logTime BETWEEN @startTime AND @stopTime", con);
                cmd.Parameters.Add(startTimeParameter);
                cmd.Parameters.Add(stopTimeParameter);
                con.Open();
                var result = (int)cmd.ExecuteScalar();
                if (result == 0)
                    StatusMessageEvent.RaiseMessage("Inga rader mellan de datumen");
                return Task.FromResult(result > 0);
            }
            catch (Exception)
            {
                StatusMessageEvent.RaiseMessage("Inga rader mellan de datumen");

            }
            return Task.FromResult(false);
        }


        private static DataTable FillGapsForwards(this DataTable tbl, int numberOfRowsIntable, TimeSpan timeLength, DateTime startTime, DateTime stopTime, int allowedNumberOfRows = 9500)
        {
            var sw = Stopwatch.StartNew();
            DataTable newTable = tbl.Clone();
            DataRow lastRow = tbl.Rows[0];
            DateTime lastTime = DateTime.MinValue;
            var numberOfColumns = tbl.Columns.Count;

            // Calculate timespan for allowedNumberOfRows
            var timeSpan = TimeSpan.FromSeconds(timeLength.TotalSeconds / allowedNumberOfRows);
            if (timeSpan < TimeSpan.FromSeconds(1))
                timeSpan = TimeSpan.FromSeconds(1);

            foreach (DataRow row in tbl.Rows)
            {

                if (lastTime == DateTime.MinValue)
                {
                    newTable.ImportRow(row);
                    lastRow = row;
                    lastTime = row.Field<DateTime>("logTime");
                    continue;
                }
                for (int i = 1; i < numberOfColumns; i++)
                {
                    if (row[i] == DBNull.Value)
                    {
                        if (lastRow[i] != DBNull.Value)
                        {
                            row[i] = lastRow[i];
                        }
                        else
                            row[i] = 0;
                    }
                }

                var rowTime = row.Field<DateTime>("logTime");

                if (rowTime - lastTime > timeSpan)
                {
                    while (rowTime >= lastTime + timeSpan)
                    {
                        row["logTime"] = lastTime + timeSpan;
                        newTable.ImportRow(row);
                        lastTime = lastTime + timeSpan;
                    }
                }
                else
                {
                    newTable.ImportRow(row);
                    lastTime = row.Field<DateTime>("logTime");
                }
                lastRow = row;

                if (tbl.Rows[tbl.Rows.Count - 1] == row && row.Field<DateTime>("logTime") < stopTime)
                {
                    while (row.Field<DateTime>("logTime") < stopTime)
                    {
                        row["logTime"] = row.Field<DateTime>("logTime") + timeSpan;
                        newTable.ImportRow(row);
                    }
                }

            }
            //newTable.FillGapsBackwards(numberOfRowsIntable, timeLength, allowedNumberOfRows);

            return newTable;
        }


        private static DataTable FillGapsBackwards(this DataTable tbl, int numberOfRowsIntable, TimeSpan timeLength, int allowedNumberOfRows = 9500)
        {
            var sw = Stopwatch.StartNew();

            var reverseTable = tbl.ReverseOrder();

            DataTable newTable = reverseTable.Clone();
            DataRow lastRow = reverseTable.Rows[0];
            DateTime lastTime = DateTime.MinValue;
            var numberOfColumns = tbl.Columns.Count;

            // Calculate timespan for allowedNumberOfRows
            var timeSpan = TimeSpan.FromSeconds(timeLength.TotalSeconds / allowedNumberOfRows);
            if (timeSpan < TimeSpan.FromSeconds(1))
                timeSpan = TimeSpan.FromSeconds(1);

            foreach (DataRow row in reverseTable.Rows)
            {

                if (lastTime == DateTime.MinValue)
                {
                    newTable.ImportRow(row);
                    lastRow = row;
                    lastTime = row.Field<DateTime>("logTime");
                    continue;
                }
                for (int i = 1; i < numberOfColumns; i++)
                {
                    if (row[i] == DBNull.Value)
                    {
                        if (lastRow[i] != DBNull.Value)
                        {
                            row[i] = lastRow[i];
                        }
                        else
                            row[i] = 0;
                    }
                }

                var rowTime = row.Field<DateTime>("logTime");

                if (rowTime - lastTime > timeSpan)
                {
                    while (rowTime >= lastTime + timeSpan)
                    {
                        row["logTime"] = lastTime + timeSpan;
                        newTable.ImportRow(row);
                        lastTime = lastTime + timeSpan;
                    }
                }
                else
                {
                    newTable.ImportRow(row);
                    lastTime = row.Field<DateTime>("logTime");
                }
                lastRow = row;
            }

            return newTable.ReverseOrder();
        }

        private static object convertLock = new();
        public static List<double> ConvertToListDouble<T>(this DataTable SourceData, string tagName, bool? fillMissingValues)
        {
            lock (convertLock)
            {
                var lastValue = double.NaN;

                List<double> list = new List<double>();
                if (SourceData == null || SourceData.Rows.Count < 1)
                    return list;

                try
                {
                    IEnumerable<double> enumerable = SourceData.AsEnumerable().Select(dataRow =>
                    {
                        var returnValue = double.NaN;
                        if (dataRow == null)
                        {
                            returnValue = default(double);
                            lastValue = (double)dataRow[tagName];
                        }
                        else if (dataRow[tagName] == DBNull.Value)
                        {
                            if ((bool)fillMissingValues)
                            {
                                if (!lastValue.Equals(default))
                                {
                                    returnValue = lastValue;
                                    lastValue = default;
                                }
                                else
                                {
                                    returnValue = default;
                                    lastValue = default;
                                }
                            }
                            else
                                returnValue = double.NaN;
                        }
                        else
                        {
                            lastValue = (double)dataRow[tagName];
                            returnValue = lastValue;
                        }
                        return returnValue;
                    });
                    if (enumerable == null)
                        return list;


                    return new List<double>(enumerable);
                }
                catch (ArgumentException)
                {
                    return list;
                }
            }
        }
        public static List<DateTime> ConvertToListDateTime<DateTime>(this DataTable SourceData, string tagName) where DateTime : struct
        {
            lock (convertLock)
            {
                var lastValue = new DateTime();

                lastValue = default;

                List<DateTime> list = new List<DateTime>();
                if (SourceData == null || SourceData.Rows.Count < 1)
                    return list;
                //IEnumerable<T> enumerable = SourceData.AsEnumerable().Select(RowConverter);
                try
                {
                    IEnumerable<DateTime> enumerable = SourceData.AsEnumerable().Select(dataRow =>
                    {
                        var returnValue = new DateTime();
                        if (dataRow == null)
                        {
                            returnValue = default(DateTime);
                            lastValue = (DateTime)dataRow[tagName];
                        }
                        else
                        {
                            lastValue = (DateTime)dataRow[tagName];
                            returnValue = lastValue;
                        }
                        return returnValue;
                    });
                    if (enumerable == null)
                        return list;


                    return new List<DateTime>(enumerable);
                }
                catch (ArgumentException)
                {
                    return list;
                }
            }
        }

        private static object addLock = new object();
        public static GearedValues<DateTimePoint> AddSeriesValues(string tagName, DataTable tbl = null, bool fillEmptyValues = false)
        {
            DataTable myTbl = new();
            if (tbl == null)
            {
                if (fillEmptyValues)
                    myTbl = ChartTableAllTags;
                else
                    myTbl = ChartTableAllTagsNoFill;
            }
            else
                myTbl = tbl;

            lock (addLock)
            {
                GearedValues<DateTimePoint> chartValues = new();

                var values = myTbl.ConvertToListDouble<double>(tagName, fillEmptyValues);
                var logTimes = myTbl.ConvertToListDateTime<DateTime>("logTime");

                var valuesCount = values.Count;
                var logTimesCount = logTimes.Count;

                if (values.Count == 0)
                    return null;

                for (int i = 0; i < logTimes.Count; i++)
                {
                    chartValues.Add(
                        new DateTimePoint()
                        {
                            Value = values[i],
                            DateTime = logTimes[i]
                        }
                        );
                }

                if (!DisplayedTags.Exists(x => x == tagName))
                    DisplayedTags.Add(tagName);

                return chartValues;
            }
        }

        internal static Task<bool> DoesTagExistBetweenDates(string tagName, DateTime startTime, DateTime stopTime)
        {
            try
            {
                using SqlConnection con = new SqlConnection(Config.SqlMethods.ConnectionString);
                con.Open();
                var query = $"SELECT id FROM {SqlSettings.Databas}.dbo.tagConfig WHERE name = '{tagName}'";
                using SqlCommand cmd1 = new SqlCommand(query, con);
                var tagId = (cmd1.ExecuteScalar()).ToString();

                var startTimeParameter = new SqlParameter("@startTime", SqlDbType.DateTime) { Value = startTime };
                var stopTimeParameter = new SqlParameter("@stopTime", SqlDbType.DateTime) { Value = stopTime };
                using SqlCommand cmd = new SqlCommand(@$"SELECT COUNT(*) FROM [{SqlSettings.Databas}].[dbo].[logValues] WHERE tag_id = {tagId} and logTime BETWEEN @startTime AND @stopTime", con);
                cmd.Parameters.Add(startTimeParameter);
                cmd.Parameters.Add(stopTimeParameter);
                var result = (int)cmd.ExecuteScalar();
                if (result == 0)
                    StatusMessageEvent.RaiseMessage($"Tag ej loggad mellan {startTime} - {stopTime}");
                return Task.FromResult(result > 0);
            }
            catch (Exception)
            {
                StatusMessageEvent.RaiseMessage($"Tag ej loggad mellan {startTime} - {stopTime}");

            }
            return Task.FromResult(false);
        }
    }
}
