using LiveCharts.Defaults;
using LiveCharts.Geared;
using LiveCharts.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OptimaValue.Wpf;

public class Line
{
    public DataTable DataTableSql;
    public List<DateTimePoint> DateTimePointsSql;
    public GLineSeries GLineSeries { get; set; }
    public Tag Tag { get; private set; }
    public DateTime MaxDate { get; internal set; }
    public DateTime MinDate { get; internal set; }
    public double MinValue { get; internal set; }
    public double MaxValue { get; internal set; }
    public double AvgValue { get; internal set; }
    public bool UpdateRequiredSql { get; internal set; }
    public DateTime GlineSeriesMaxDateTime { get; internal set; } = DateTime.MinValue;
    public DataStatistics DataStatistics { get; internal set; }
    public TagControl TagControl { get; internal set; }

    public Line(int id)
    {
        Tag = StaticClass.AvailableTags.FirstOrDefault(t => t.TagId == id);
    }


    public LinearGradientBrush UpdateTagControlEllipseColor()
    {
        var brush = new LinearGradientBrush();
        brush.GradientStops.Add(new GradientStop(Tag.Stroke, 0));
        brush.GradientStops.Add(new GradientStop(Tag.Fill, 1));
        return brush;
    }

    public override bool Equals(object myLine)
    {
        var line = myLine as Line;
        if (line == null)
            return false;

        return Tag.TagId == line.Tag.TagId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Tag.TagId);
    }


}

public static class StaticClass
{
    /// <summary>
    /// All the logged tags in Sql
    /// </summary>
    public static List<Tag> AvailableTags { get; set; }

    public static BindingList<Line> Lines { get; internal set; }

    public static void AddLine(Line line)
    {
        if (Lines == null)
            Lines = new();

        if (!Lines.Contains(line))
            Lines.Add(line);
        else
        {
            Lines.Remove(line);
            Lines.Add(line);
        }
    }

    public static void RemoveLine(Line line)
    {
        if (Lines == null)
            return;
        if (Lines.Contains(line))
            Lines.Remove(line);
    }
}

public static class LinesExtensions
{
    public static Line CreateLine(this Line line, int tagId)
    {
        if (line == null)
            line = new Line(tagId);

        line.TagControl = new();
        line.TagControl.TagName = line.Tag.Name;
        line.TagControl.TagColor = line.UpdateTagControlEllipseColor();
        line.TagControl.Description = line.Tag.Description;
        line.TagControl.TagUnit = line.Tag.Unit;

        return line;
    }

    public static async Task<Line> GetSqlData(this Line line, DateTime startDate, DateTime stopDate, int maxRows = 0)
    {
        var tbl = new DataTable();
        tbl.Clear();
        string query = string.Empty;
        if (maxRows > 0)
        {
            query = $"SELECT TOP {maxRows} FROM {Config.Settings.Databas}.[logValues] WHERE TagId = {line.Tag.TagId} AND DateTime >= '{startDate}' AND DateTime <= '{stopDate}' ORDER by logTime";
        }
        else
            query = $"SELECT * FROM {Config.Settings.Databas}.[logValues] WHERE TagId = {line.Tag.TagId} AND DateTime >= '{startDate}' AND DateTime <= '{stopDate}'";

        using var con = new SqlConnection(Config.Settings.ConnectionString);
        using var cmd = new SqlCommand(query, con);
        con.Open();
        var reader = await cmd.ExecuteReaderAsync();
        tbl.Load(reader);

        tbl.DefaultView.Sort = "logTime";
        tbl = tbl.DefaultView.ToTable();

        line.DataTableSql = tbl;

        line.MaxDate = (DateTime)tbl.AsEnumerable().Select(x => x["logTime"]).Max();
        line.MinDate = (DateTime)tbl.AsEnumerable().Select(x => x["logTime"]).Min();

        if (tbl != null)
        {
            if (tbl.Rows.Count > 0)
                line.UpdateRequiredSql = false;
        }
        await Task.Run(() =>
        {
            line.DateTimePointsSql = tbl.AsEnumerable().Select(x => new DateTimePoint((DateTime)x["logTime"], (double)x["numericValue"])).ToList();
        });

        return line;
    }

    public static Line CreateGlineSeries(this Line line)
    {
        if (line.DataTableSql == null)
            throw new NotImplementedException("Ej skapat DataTable");

        line.GLineSeries.Title = line.Tag.Name;

        line.GLineSeries.Values = new GearedValues<DateTimePoint>().WithQuality(Quality.Highest);

        return line;
    }

    public static async Task<Line> GetValuesFromTable(this Line line, DateTime startDate, DateTime stopDate)
    {
        if (line.DataTableSql == null)
            throw new ChartSqlTableNotCreatedException(line);

        if (startDate > stopDate)
            throw new ChartDateTimeException(line, "Startdatum är större än slutdatum");

        if (line.MaxDate < startDate || line.MinDate > stopDate)
            throw new ChartDateTimeException(line, "Datumet är utanför tidsintervalet");

        // Create async Task to get the data from the datatable
        await Task.Run(() =>
        {
            var tbl = line.DataTableSql.AsEnumerable().Where(x => (DateTime)x["logTime"] >= startDate && (DateTime)x["logTime"] <= stopDate).CopyToDataTable();
            line.GLineSeries.Values = tbl.AsEnumerable().Select(x => new DateTimePoint((DateTime)x["logTime"], (double)x["logValue"])).ToList().AsGearedValues().WithQuality(Quality.Highest);
        });

        return line;
    }

    public static async Task<Line> UpdateValuesFromTable(this Line line, TimeSpan duration)
    {
        if (line.MaxDate < line.GlineSeriesMaxDateTime + duration)
        {
            line.UpdateRequiredSql = true;
            throw new ChartOutOfRangeSqlException(line);
        }

        var nrOfValues = 0;
        // Add new values
        await Task.Run(() =>
        {
            try
            {
                var firstHigherValue = line.DateTimePointsSql.Where(x => (x.DateTime > line.GlineSeriesMaxDateTime) && (x.DateTime <= (line.GlineSeriesMaxDateTime + duration))).ToList();
                nrOfValues = firstHigherValue.Count;
                if (firstHigherValue.Count > 0)
                {
                    nrOfValues = firstHigherValue.Count;
                    line.GLineSeries.Values.AddRange(firstHigherValue);
                    while (nrOfValues > 0)
                    {
                        nrOfValues--;
                        line.GLineSeries.Values.RemoveAt(0);
                    }
                }
            }
            catch (ArgumentNullException)
            {
                line.UpdateRequiredSql = true;
                throw new ChartOutOfRangeSqlException(line);

            }
        });
        return line;
    }

    public static Line AddOneValue(this Line line, TimeSpan timeSpan)
    {
        line.GlineSeriesMaxDateTime = new DateTime((long)line.GLineSeries.ChartPoints.AsEnumerable().Select(x => x.X).Max());
        if (line.GlineSeriesMaxDateTime > line.MaxDate)
        {
            line.UpdateRequiredSql = true;
            throw new ChartOutOfRangeSqlException(line);
        }
        var nextValueFromSql = line.DataTableSql.AsEnumerable().Where(x => (DateTime)x["logTime"] > line.GlineSeriesMaxDateTime).FirstOrDefault();
        if (nextValueFromSql != null)
        {
            var nextValue = new DateTimePoint((DateTime)nextValueFromSql["logTime"], (double)nextValueFromSql["logValue"]);
            line.GLineSeries.Values.Add(nextValue);
        }
        else
        {
            line.UpdateRequiredSql = true;
            throw new ChartOutOfRangeSqlException(line);
        }
        return line;
    }

    public static async Task<Line> GetMinMaxAvg(this Line line)
    {
        if (line.GLineSeries.Values.Count == 0)
            return line;

        await Task.Run(() =>
        {
            line.MinValue = line.GLineSeries.ChartPoints.AsEnumerable().Select(x => x.Y).Min();
            line.MaxValue = line.GLineSeries.ChartPoints.AsEnumerable().Select(x => x.Y).Max();
            line.AvgValue = line.GLineSeries.ChartPoints.AsEnumerable().Select(x => x.Y).Average();
            line.MinDate = new DateTime((long)line.GLineSeries.ChartPoints.AsEnumerable().Select(x => x.X).Min());
            line.MaxDate = new DateTime((long)line.GLineSeries.ChartPoints.AsEnumerable().Select(x => x.X).Max());
            line.TagControl.MinValue = line.MinValue.ToString("0.00");
            line.TagControl.MaxValue = line.MaxValue.ToString("0.00");
            line.TagControl.AverageValue = line.AvgValue.ToString("0.00");
        });

        return line;
    }

    public static Line UpdateLineColor(this Line line)
    {
        line.Tag.UpdateColor();
        line.TagControl.TagColor = line.UpdateTagControlEllipseColor();
        return line;
    }

    public static async Task<Line> UpdateDataStatistics(this Line line)
    {
        await Task.Run(() =>
        {
            if (line.DataStatistics == null)
                line.DataStatistics = new DataStatistics(line);
            else
                line.DataStatistics.CalculateStatistics();
        });


        // Update TagControl
        line.TagControl.Integral = line.DataStatistics.Integral.ToString("0.00");
        var integralPerTimme = line.DataStatistics.Integral / (line.MaxDate - line.MinDate).TotalHours;
        line.TagControl.IntegralPerTimme = integralPerTimme.ToString("0.00");
        line.TagControl.OverZero = line.DataStatistics.NumberOfTimesOverZero.ToString("0.00");
        line.TagControl.OverZeroTime = line.DataStatistics.TimeOverZero.ToString("0.00");
        line.TagControl.Deviation = line.DataStatistics.StandardDeviation.ToString("0.00");
        return line;
    }
}
