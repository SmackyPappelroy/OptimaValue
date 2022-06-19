using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace OptimaValue;

public class TrendModel
{

    private CartesianMapper<DateTimePoint> RowSeriesConfiguration { get; set; }
    private string RowSeriesLabel { get; set; }
    private DataTable SqlValues = new();
    public event Action<bool> OnValuesUpdated;
    /// <summary>
    /// Points to map to the Lineseries in the trend
    /// </summary>
    public ChartValues<DateTimePoint> ChartValuesDateTimePoints { get; set; }
    /// <summary>
    /// Values from Sql
    /// </summary>
    public List<DateTimePoint> DateTimePointsToTrend { get; private set; }
    public LineSeries LineSeries { get; set; }
    public TrendTag windowsForm { get; init; }
    public CancellationTokenSource source = new CancellationTokenSource();

    public Func<double, string> FormatterY { get; set; }
    public Func<double, string> FormatterX { get; set; }
    public int NumberOfValues => ChartValuesDateTimePoints.Count;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TimeSpan Duration { get; set; }
    private TimeSpan internalDuration { get; set; }

    public string TagName { get; private set; }
    public int TagId { get; init; }
    public double MinValueX { get; private set; }
    public double MaxValueX { get; private set; }
    public double MinValueY { get; private set; }
    public double MaxValueY { get; private set; }
    public SolidColorBrush Stroke { get; private set; }
    public SolidColorBrush Fill { get; private set; }


    public TrendModel(int id, TimeSpan duration)
    {
        TagId = id;
        // Get tagname from the ID-number from SQL
        TagName = GetTagNameFromSql(id);

        // Map x values to datetime and y values to double
        RowSeriesConfiguration = Mappers.Xy<DateTimePoint>()
            .X(dateTimePoint => dateTimePoint.DateTime.Ticks)
            .Y(dateTimePoint => dateTimePoint.Value);
        // The rowseries, maps to LineSeries.Values
        ChartValuesDateTimePoints = new ChartValues<DateTimePoint>();
        // Name of trend
        RowSeriesLabel = TagName;

        // Format Y-axis
        FormatterY = val => val.ToString("0.0");
        // Format X-axis
        FormatterX = x => new DateTime((long)x).ToString("yyyy-MM-dd HH:mm:ss");

        // Create color
        (Stroke, Fill) = new ColorCreator().CreateRandomColor();

        // Set color for lineseries
        LineSeries = new LineSeries()
        {
            Fill = Fill,
            Stroke = Stroke,
            Title = TagName
        };
        Duration = internalDuration = duration;
    }

    /// <summary>
    /// The cyclic <see cref="Task"/><para></para> Gets new data from Sql
    /// <para></para> Updates chart
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    public async Task<bool> StartUpdaterAsync(CancellationTokenSource src)
    {
        UpdateTime();
        while (!src.IsCancellationRequested)
        {
            try
            {
                if (src.IsCancellationRequested)
                    break;

                GetNewSqlData();

                if (SqlValues.Rows.Count > 1)
                    UpdateChart();

                if (internalDuration != Duration)
                {
                    Duration = internalDuration;
                    DateTimePointsToTrend = new();
                }
                await Task.Delay(1000);
            }
            catch (Exception)
            { }
        }
        return true;
    }

    private DateTime lastUpdateTime = DateTime.Now;
    private void UpdateChart()
    {
        while (DateTime.Now >= lastUpdateTime.Add(TimeSpan.FromSeconds(1)))
        {
            var tiden = DateTime.Now;
            try
            {
                if (DateTimePointsToTrend == null)
                    DateTimePointsToTrend = new();


                windowsForm.Invoke((MethodInvoker)delegate
                {
                    DateTimePointsToTrend = SqlValues.DataTableToDateTimePoints(Duration, TimeSpan.FromSeconds(30));
                    if (DateTimePointsToTrend != null)
                    {
                        ChartValuesDateTimePoints.Clear();
                        if (DateTimePointsToTrend.Count != 0)
                        {
                            ChartValuesDateTimePoints.AddRange(DateTimePointsToTrend);

                            MinValueX = ChartValuesDateTimePoints.Min(x => x.DateTime.Ticks);
                            MaxValueX = ChartValuesDateTimePoints.Max(x => x.DateTime.Ticks);
                            MinValueY = ChartValuesDateTimePoints.Where(x => !double.IsNaN(x.Value)).Min(x => x.Value);
                            MaxValueY = ChartValuesDateTimePoints.Where(x => !double.IsNaN(x.Value)).Max(x => x.Value);

                            OnValuesUpdated?.Invoke(true);
                        }
                    }


                });
                break;
            }
            finally
            {
                lastUpdateTime = DateTime.Now;
            }

        }


    }

    private void GetNewSqlData()
    {
        DataTable tempTable = new();
        var tiden = DateTime.Now;
        var startTime = tiden.Subtract(Duration * 5);
        var timeString = $" logTime BETWEEN '{startTime}' AND '{tiden}'";
        var query = $"SELECT * FROM {Config.Settings.Databas}.dbo.logValues WHERE tag_id = {TagId} AND{timeString}";
        using SqlConnection con = new SqlConnection(Config.Settings.ConnectionString);
        con.Open();
        using SqlCommand cmd = new SqlCommand(query, con);
        using SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        adapter.Fill(tempTable);
        if (tempTable.Rows.Count > 0)
        {
            DateTime oldMaxTime = DateTime.MinValue;
            tempTable.DefaultView.Sort = "logTime";
            SqlValues = tempTable.DefaultView.ToTable();


        }
    }

    public void UpdateValues(List<DateTimePoint> dateTimePoints)
    {
        foreach (var item in dateTimePoints)
        {
            if (item.Value < 0)
                item.Value *= -1;
        }

        if (ChartValuesDateTimePoints.Count > 0)
        {
            ChartValuesDateTimePoints.Remove(ChartValuesDateTimePoints.Last());
            foreach (var item in ChartValuesDateTimePoints)
            {
                if (item.DateTime < StartDate)
                    ChartValuesDateTimePoints.Remove(item);
            }

        }
        ChartValuesDateTimePoints.AddRange(dateTimePoints);




    }

    public void UpdateTime()
    {
        EndDate = DateTime.Now;
        StartDate = DateTime.Now - Duration;
    }

    private string GetTagNameFromSql(int tagId)
    {
        var query = $"SELECT name FROM {Config.Settings.Databas}.dbo.tagConfig WHERE id = {tagId}";
        using SqlConnection con = new SqlConnection(Config.Settings.ConnectionString);
        con.Open();
        using SqlCommand cmd = new SqlCommand(query, con);
        return cmd.ExecuteScalar().ToString();
    }

    internal async void WindowsForm_Load(object sender, EventArgs e)
    {
        windowsForm.txtTimeSpan.Text = Duration.ToString();

        await Task.Run(async () =>
        {
            await StartUpdaterAsync(source);
        }).ConfigureAwait(false);

    }

    internal void WindowsForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        source.Cancel();
    }

    internal void TxtTimeSpan_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
        var value = ((TextBox)sender).Text;
        if (string.IsNullOrEmpty(value))
        {
            return;
        }
        // Remove whitespace
        value = value.Replace(" ", "");
        if (!TimeSpan.TryParse(value, out TimeSpan result))
        {
            e.Cancel = true;
            return;
        }
        if (result > TimeSpan.Zero)
        {
            internalDuration = result;
            windowsForm.Focus();
        }

    }
}

public static class TrendExtensions
{
    public static TrendModel Create(this TrendModel trendModel, TrendTag windowsForm, int tagId, TimeSpan duration)
    {
        trendModel = new TrendModel(tagId, duration)
        {
            windowsForm = windowsForm,
        };

        windowsForm.Load += trendModel.WindowsForm_Load;
        windowsForm.FormClosing += trendModel.WindowsForm_FormClosing;
        windowsForm.txtTimeSpan.Validating += trendModel.TxtTimeSpan_Validating;
        return trendModel;
    }

    public static List<DateTimePoint> DataTableToDateTimePoints(this DataTable tbl, TimeSpan duration, TimeSpan timeOffset)
    {


        var dateTimePoints = new List<DateTimePoint>();
        tbl.DefaultView.Sort = "logTime";
        tbl = tbl.DefaultView.ToTable();

        foreach (DataRow row in tbl.Rows)
        {
            DateTimePoint dtPoint = new();
            bool conversionOk = true;
            if (!double.TryParse(row["numericValue"].ToString(), out double value))
                conversionOk = false;
            if (!DateTime.TryParse(row["logTime"].ToString(), out DateTime time))
                conversionOk = false;

            if (conversionOk)
            {
                dtPoint.DateTime = time;
                dtPoint.Value = value;
                dateTimePoints.Add(dtPoint);
            }
        }

        var filteredValues = dateTimePoints.Where(x => x.DateTime >= (DateTime.Now.Subtract(duration + timeOffset))
                             && x.DateTime <= DateTime.Now.Subtract(timeOffset)).ToList();
        if (filteredValues.Count == 0)
        {
            return default;
        }
        else
        {
            return filteredValues;
        }
    }
}