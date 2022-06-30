using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Geared;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
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
using System.Windows.Threading;

namespace OptimaValue.Wpf;

public class Line : INotifyPropertyChanged
{
    private GraphWindow window;

    public event PropertyChangedEventHandler PropertyChanged;

    public DataTable DataTableSql;
    public List<DateTimePoint> DateTimePointsSql;
    public readonly TimeSpan ExtraTimeInSql = TimeSpan.FromHours(1);

    public GLineSeries GLineSeries { get; set; }
    public GearedValues<DateTimePoint> ChartValues;
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
    public Func<double, string> FormatterY { get; internal set; }
    public Func<double, string> FormatterX { get; internal set; }

    public Line(int id, TimeSpan extraTimeInSql)
    {
        ExtraTimeInSql = extraTimeInSql;
        Tag = StaticClass.AvailableTags.FirstOrDefault(t => t.TagId == id);
        window = Master.GetService<GraphWindow>();
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

    public override string ToString()
    {
        return Tag.Name;
    }

    public void CreateLine()
    {
        TagControl = new();
        TagControl.TagName = Tag.Name;
        TagControl.TagColor = UpdateTagControlEllipseColor();
        TagControl.Description = Tag.Description;
        TagControl.TagUnit = Tag.Unit;

        GLineSeries = new()
        {
            AreaLimit = 0,
            PointGeometry = DefaultGeometries.Circle,
            PointGeometrySize = 10,
            StrokeThickness = 2,

            Foreground = new SolidColorBrush(Color.FromArgb(107, 48, 48, 48)),
            Stroke = new SolidColorBrush(Tag.Stroke),
            Fill = new SolidColorBrush(Tag.Fill),
            Title = Tag.Name,
            Values = new GearedValues<DateTimePoint>().WithQuality(Quality.Highest)
        };
        // Format XY-axis
        FormatterY = val => val.ToString("0.0");
        FormatterX = x => new DateTime((long)x).ToString("yyyy-MM-dd HH:mm:ss.fff");
        ChartValues = new GearedValues<DateTimePoint>().WithQuality(Quality.Highest);
    }


    public async Task SetupLine(DateTime startDate, DateTime stopDate, int maxRows = 0)
    {
        CreateLine();
        await GetSqlData(startDate.Subtract(ExtraTimeInSql), stopDate.Add(ExtraTimeInSql), maxRows);
        await GetValuesFromTable(startDate, stopDate);
        await GetMinMaxAvg();
        await UpdateDataStatistics();
    }

    /// <summary>
    /// Gets data from sql between <paramref name="startDate"/> and <paramref name="stopDate"/>
    /// </summary>
    /// <param name="line"></param>
    /// <param name="startDate"></param>
    /// <param name="stopDate"></param>
    /// <param name="maxRows"></param>
    /// <returns></returns>
    /// <exception cref="ChartNoDataInSqlException"></exception>
    public async Task GetSqlData(DateTime startDate, DateTime stopDate, int maxRows = 0)
    {
        var tbl = new DataTable();
        tbl.Clear();
        string query = string.Empty;
        if (maxRows > 0)
        {
            query = $"SELECT TOP {maxRows} FROM {Config.Settings.Databas}.dbo.[logValues] WHERE tag_id = {Tag.TagId} AND logTime >= '{startDate}' AND logTime <= '{stopDate}' ORDER by logTime";
        }
        else
            query = $"SELECT * FROM {Config.Settings.Databas}.dbo.[logValues] WHERE tag_id = {Tag.TagId} AND logTime >= '{startDate}' AND logTime <= '{stopDate}'";

        using var con = new SqlConnection(Config.Settings.ConnectionString);
        using var cmd = new SqlCommand(query, con);
        con.Open();
        var reader = await cmd.ExecuteReaderAsync();
        tbl.Load(reader);

        if (tbl == null || tbl.Rows.Count == 0)
            throw new ChartNoDataInSqlException(this, string.Empty, startDate, stopDate);

        tbl.DefaultView.Sort = "logTime";
        tbl = tbl.DefaultView.ToTable();


        DataTableSql = tbl;

        MaxDate = (DateTime)tbl.AsEnumerable().Select(x => x["logTime"]).Max();
        MinDate = (DateTime)tbl.AsEnumerable().Select(x => x["logTime"]).Min();

        if (tbl != null)
        {
            if (tbl.Rows.Count > 0)
                UpdateRequiredSql = false;
        }
        await Task.Run(() =>
        {
            DateTimePointsSql = tbl.AsEnumerable().Select(x => new DateTimePoint((DateTime)x["logTime"], (double)x["numericValue"]))?.ToList();
        });

        return;
    }

    /// <summary>
    /// <para></para>
    /// Gets values from the <see cref="Line.DataTableSql"/> and adds them to a <see cref="GLineSeries"/>
    /// <para></para>
    /// Creates the <see cref="GLineSeries"/> if it is null
    /// </summary>
    /// <param name="line"></param>
    /// <param name="startDate"></param>
    /// <param name="stopDate"></param>
    /// <returns></returns>
    /// <exception cref="ChartSqlTableNotCreatedException"></exception>
    /// <exception cref="ChartDateTimeException"></exception>
    public async Task GetValuesFromTable(DateTime startDate, DateTime stopDate)
    {
        if (DataTableSql == null)
            throw new ChartSqlTableNotCreatedException(this);

        if (startDate > stopDate)
            throw new ChartDateTimeException(this, "Startdatum är större än slutdatum");

        if (MaxDate < startDate || MinDate > stopDate)
            throw new ChartDateTimeException(this, "Datumet är utanför tidsintervalet");


        GearedValues<DateTimePoint> values = new GearedValues<DateTimePoint>();
        // Create async Task to get the data from the datatable
        await Task.Run(() =>
        {
            var tbl = DataTableSql.AsEnumerable().Where(x => (DateTime)x["logTime"] >= startDate && (DateTime)x["logTime"] <= stopDate).CopyToDataTable();
            try
            {

                ChartValues = tbl.AsEnumerable().Select(x => new DateTimePoint((DateTime)x["logTime"], (double)x["numericValue"])).ToList().AsGearedValues().WithQuality(Quality.Highest);
            }
            catch (Exception ex)
            {
                var mes = ex;
            }


        });
        GLineSeries.Values = ChartValues;
        return;
    }

    /// <summary>
    /// Updates the data with a new time span <paramref name="duration"/>
    /// </summary>
    /// <param name="line"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    /// <exception cref="ChartOutOfRangeSqlException"></exception>
    public async Task UpdateValues(TimeSpan duration)
    {
        if (MaxDate < GlineSeriesMaxDateTime + duration)
        {
            UpdateRequiredSql = true;
            throw new ChartOutOfRangeSqlException(this);
        }

        var nrOfValues = 0;
        // Add new values
        await Task.Run(() =>
        {
            try
            {
                var firstHigherValue = DateTimePointsSql.Where(x => (x.DateTime > GlineSeriesMaxDateTime) && (x.DateTime <= (GlineSeriesMaxDateTime + duration))).ToList();
                nrOfValues = firstHigherValue.Count;
                if (firstHigherValue.Count > 0)
                {
                    nrOfValues = firstHigherValue.Count;
                    ChartValues.AddRange(firstHigherValue);
                    while (nrOfValues > 0)
                    {
                        nrOfValues--;
                        ChartValues.RemoveAt(0);
                    }
                }
            }
            catch (ArgumentNullException)
            {
                UpdateRequiredSql = true;
                throw new ChartOutOfRangeSqlException(this);

            }
        });
        GLineSeries.Values = ChartValues;
        return;
    }

    /// <summary>
    /// Adds one value to the line
    /// </summary>
    /// <param name="line"></param>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    /// <exception cref="ChartOutOfRangeSqlException"></exception>
    public void AddOneValueRemoveOldest()
    {
        GlineSeriesMaxDateTime = new DateTime((long)GLineSeries.ChartPoints.AsEnumerable().Select(x => x.X).Max());
        if (GlineSeriesMaxDateTime > MaxDate)
        {
            UpdateRequiredSql = true;
            throw new ChartOutOfRangeSqlException(this);
        }
        var nextValueFromSql = DataTableSql.AsEnumerable().Where(x => (DateTime)x["logTime"] > GlineSeriesMaxDateTime).FirstOrDefault();
        if (nextValueFromSql != null)
        {
            var nextValue = new DateTimePoint((DateTime)nextValueFromSql["logTime"], (double)nextValueFromSql["logValue"]);
            ChartValues.Add(nextValue);
            ChartValues.RemoveAt(0);
        }
        else
        {
            UpdateRequiredSql = true;
            throw new ChartOutOfRangeSqlException(this);
        }
        GLineSeries.Values = ChartValues;
        return;
    }

    /// <summary>
    /// Gets the min, max and average value for the line
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    public async Task GetMinMaxAvg()
    {
        if (GLineSeries.Values.Count == 0)
            return;

        await Task.Run(() =>
        {
            window.Dispatcher?.Invoke(() =>
            {
                MinValue = ChartValues.AsEnumerable().Select(x => x.Value).Min();
                MaxValue = ChartValues.AsEnumerable().Select(x => x.Value).Max();
                AvgValue = ChartValues.AsEnumerable().Select(x => x.Value).Average();
                MinDate = ChartValues.AsEnumerable().Select(x => x.DateTime).Min();
                MaxDate = ChartValues.AsEnumerable().Select(x => x.DateTime).Max();

                TagControl.MinValue = MinValue.ToString("0.00");
                TagControl.MaxValue = MaxValue.ToString("0.00");
                TagControl.AverageValue = AvgValue.ToString("0.00");
            });
        });

    }

    /// <summary>
    /// Updates the line color
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    public void UpdateLineColor()
    {
        Tag.UpdateColor();
        TagControl.TagColor = UpdateTagControlEllipseColor();
    }

    /// <summary>
    /// Updates the statistics for the series view
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    public async Task UpdateDataStatistics()
    {
        await Task.Run(() =>
        {
            if (DataStatistics == null)
                DataStatistics = new DataStatistics(this);
            else
                DataStatistics.CalculateStatistics();
        });


        // Update TagControl
        TagControl.Integral = DataStatistics.Integral.ToString("0.00");
        var integralPerTimme = DataStatistics.Integral / (MaxDate - MinDate).TotalHours;
        TagControl.IntegralPerTimme = integralPerTimme.ToString("0.00");
        TagControl.OverZero = DataStatistics.NumberOfTimesOverZero.ToString("0.00");
        TagControl.OverZeroTime = DataStatistics.TimeOverZero.ToString();
        TagControl.Deviation = DataStatistics.StandardDeviation.ToString("0.00");
        return;
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
        {
            Lines = new();
        }

        if (!Lines.Contains(line))
        {
            Lines.Add(line);
        }
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

}
