using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Geared;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace OptimaValue;

public class TrendModel
{

    private CartesianMapper<DateTimePoint> RowSeriesConfiguration { get; set; }
    private string RowSeriesLabel { get; set; }

    private TagInfo TagInfo { get; set; }

    public System.Windows.Forms.Timer timerStatusText;
    private string statusText;
    public string StatusText
    {
        get => statusText;
        set
        {
            statusText = value;
            if (!string.IsNullOrEmpty(value))
            {
                windowsForm.txtStatus.Invoke((MethodInvoker)delegate
                {
                    windowsForm.txtStatus.Visible = true;
                    windowsForm.txtStatus.Text = value;
                    timerStatusText.Start();
                });
            }
        }
    }
    private DataTable SqlValues = new();
    public event Action<bool> OnValuesUpdated;
    /// <summary>
    /// Points to map to the Lineseries in the trend
    /// </summary>
    public GearedValues<DateTimePoint> ChartValuesDateTimePoints { get; set; }
    public GearedValues<DateTimePoint> ChartSetpointDateTimePoints { get; set; } = new() { };
    /// <summary>
    /// Values from Sql
    /// </summary>
    public GearedValues<DateTimePoint> DateTimePointsToTrend { get; private set; }
    public LineSeries LineSeries { get; set; }
    public LineSeries LineSeriesSetpoint { get; set; }
    public TrendTag windowsForm { get; init; }
    public CancellationTokenSource source = new CancellationTokenSource();


    public Func<double, string> FormatterY { get; set; }
    public Func<double, string> FormatterX { get; set; }
    public int NumberOfValues => ChartValuesDateTimePoints.Count;
    public DateTime FirstLoggedDate { get; private set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime InputStartDate { get; set; } = DateTime.MinValue;

    public bool InputDatesOk => InputStartDate > DateTime.MinValue;
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
    public bool PlayActive { get; set; } = true;
    public double Setpoint { get; private set; } = double.MinValue;
    public double Minimum { get; private set; } = double.MinValue;
    public double Maximum { get; private set; } = double.MaxValue;


    public TrendModel(int id, TimeSpan duration)
    {
        TagId = id;
        // Get tagname from the ID-number from SQL
        TagInfo = GetTagInfoFromSql(id);
        TagName = TagInfo.TagName;
        // Get first occurence in the database
        FirstLoggedDate = GetStartDateFromSql(id);




        // The rowseries, maps to LineSeries.Values
        ChartValuesDateTimePoints = new GearedValues<DateTimePoint>();
        // Name of trend
        RowSeriesLabel = TagName;

        // Format Y-axis
        FormatterY = val => val.ToString("0.0");
        // Format X-axis
        if ((int)TagInfo.LogFrequency < 1000)
        {
            // Map x values to datetime and y values to double
            RowSeriesConfiguration = Mappers.Xy<DateTimePoint>()
                .X(dateTimePoint => dateTimePoint.DateTime.Ticks)
                .Y(dateTimePoint => dateTimePoint.Value);
            FormatterX = x => new DateTime((long)x).ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
        else
        {
            // Map x values to datetime and y values to double
            RowSeriesConfiguration = Mappers.Xy<DateTimePoint>()
                .X(dateTimePoint => dateTimePoint.DateTime.Ticks)
                .Y(dateTimePoint => dateTimePoint.Value);
            FormatterX = x => new DateTime((long)x).ToString("yyyy-MM-dd HH:mm:ss");
        }

        var temp = DateTime.Now.Ticks.ToString("yyyy-MM-dd HH:mm:ss ff");

        // Create color
        (Stroke, Fill) = new ColorCreator().CreateRandomColor();

        // Set color for lineseries
        LineSeries = new LineSeries()
        {
            Fill = Fill,
            Stroke = Stroke,
            Title = TagName,
            AreaLimit = 0,
            PointGeometry = DefaultGeometries.Circle,
            PointGeometrySize = 10,
            StrokeThickness = 2,
        };
        LineSeriesSetpoint = new LineSeries()
        {
            Values = ChartSetpointDateTimePoints,
            Stroke = Brushes.Red,
            StrokeThickness = 2,
            PointGeometry = null,
            LineSmoothness = 0,
            StrokeDashArray = new DoubleCollection() { 2, 2 },
            Title = "Setpoint"
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
                {
                    if (PlayActive)
                        UpdateChart();
                    if (InputStartDate < DateTime.MaxValue)
                        UpdateChart();
                }

                if (internalDuration != Duration)
                {
                    Duration = internalDuration;
                    DateTimePointsToTrend = new();
                }
                await Task.Delay((int)TagInfo.LogFrequency);
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
                    DateTimePointsToTrend = SqlValues.DataTableToDateTimePoints(Duration, TimeSpan.FromSeconds(15), InputDatesOk, InputStartDate, TagInfo);
                    if (DateTimePointsToTrend != null)
                    {
                        ChartValuesDateTimePoints.Clear();
                        if (DateTimePointsToTrend.Count != 0)
                        {
                            ChartValuesDateTimePoints.AddRange(DateTimePointsToTrend);

                            // No decimals
                            if (DateTimePointsToTrend.Any(x => (x.Value % 1 == 0)))
                                FormatterY = val => val.ToString("0");
                            else
                                FormatterY = val => val.ToString("0.00");
                            if (DateTimePointsToTrend.All(x => x.Value < 10))
                                FormatterY = val => val.ToString("0.000");

                            if (Setpoint != double.MinValue)
                            {
                                ChartSetpointDateTimePoints.Clear();
                                foreach (var item in DateTimePointsToTrend)
                                {
                                    DateTime dateTime = item.DateTime;

                                    ChartSetpointDateTimePoints.Add(new DateTimePoint(dateTime, Setpoint));
                                }
                            }
                            else if (ChartSetpointDateTimePoints.Count > 0)
                            {
                                ChartSetpointDateTimePoints.Clear();
                            }

                            MinValueX = ChartValuesDateTimePoints.Min(x => x.DateTime.Ticks);
                            MaxValueX = ChartValuesDateTimePoints.Max(x => x.DateTime.Ticks);
                            MinValueY = ChartValuesDateTimePoints.Where(x => !double.IsNaN(x.Value)).Min(x => x.Value);
                            MaxValueY = ChartValuesDateTimePoints.Where(x => !double.IsNaN(x.Value)).Max(x => x.Value);

                            OnValuesUpdated?.Invoke(true);
                        }
                    }
                    else if (InputStartDate > DateTime.MinValue)
                    {
                        StatusText = "Ingen data";
                    }

                });
                break;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
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
        var timeString = "";
        if (!InputDatesOk)
            timeString = $" logTime BETWEEN '{startTime}' AND '{tiden}'";
        else
            timeString = $" logTime BETWEEN '{InputStartDate}' AND '{InputStartDate.Add(Duration * 5)}'";
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

    private TagInfo GetTagInfoFromSql(int tagId)
    {
        var query = $"SELECT * FROM {Config.Settings.Databas}.dbo.tagConfig WHERE id = {tagId}";
        using SqlConnection con = new SqlConnection(Config.Settings.ConnectionString);
        con.Open();
        using SqlCommand cmd = new SqlCommand(query, con);
        using SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        DataTable tempTable = new();
        var tagInfo = new TagInfo();
        adapter.Fill(tempTable);
        if (tempTable.Rows.Count > 0)
        {
            tagInfo.TagId = (int)tempTable.Rows[0]["id"];
            tagInfo.TagName = (string)tempTable.Rows[0]["name"];
            Enum.TryParse((string)tempTable.Rows[0]["logFreq"], out LogFrequency logFrequency);
            tagInfo.LogFrequency = logFrequency;
            Enum.TryParse((string)tempTable.Rows[0]["logType"], out LogType logType);
            tagInfo.LogType = logType;
        }


        return tagInfo;
    }

    private DateTime GetStartDateFromSql(int id)
    {
        var query = $"SELECT MIN(logTime) FROM {Config.Settings.Databas}.dbo.logValues WHERE tag_id = {id}";
        using SqlConnection con = new SqlConnection(Config.Settings.ConnectionString);
        con.Open();
        using SqlCommand cmd = new SqlCommand(query, con);
        return Convert.ToDateTime(cmd.ExecuteScalar());
    }

    internal async void WindowsForm_Load(object sender, EventArgs e)
    {
        windowsForm.txtTimeSpan.Text = Duration.ToString();
        windowsForm.lblStartTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        windowsForm.lblStopTime.Text = DateTime.Now.Add(TimeSpan.FromSeconds(30)).ToString("yyyy-MM-dd HH:mm:ss");
        windowsForm.txtStatus.Text = string.Empty;
        windowsForm.chart.Series.Configuration = RowSeriesConfiguration;
        SetupStartAndStopDate(FirstLoggedDate, DateTime.Now);

        await Task.Run(async () =>
        {
            await StartUpdaterAsync(source);
        }).ConfigureAwait(false);

    }

    private void SetupStartAndStopDate(DateTime firstLoggedDate, DateTime now)
    {

        windowsForm.txtStartDate.Invoke((MethodInvoker)delegate
        {
            windowsForm.txtStartDate.Text = firstLoggedDate.ToString("yyyy-MM-dd HH:mm:ss");
        });

    }

    internal void WindowsForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        timerStatusText.Tick -= OnStatus_TextChanged;
        source.Cancel();
    }

    #region Validation

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
            windowsForm.errorProvider1.SetError(((TextBox)sender), "Invalid timespan");
            return;
        }
        if (result > TimeSpan.FromSeconds(10))
        {
            internalDuration = result;
            windowsForm.errorProvider1.Clear();
            windowsForm.Focus();
        }
        else
            windowsForm.errorProvider1.SetError(((TextBox)sender), "För lågt spann (större än 10 sekunder)");

    }

    internal void TxtSetpoint_Validating(object sender, CancelEventArgs e)
    {
        var value = ((TextBox)sender).Text;
        if (string.IsNullOrEmpty(value))
        {
            Setpoint = double.MinValue;
            return;
        }
        // Remove whitespace
        value = value.Replace(" ", "");
        if (!double.TryParse(value, out double result))
        {
            e.Cancel = true;
            windowsForm.errorProvider1.SetError(((TextBox)sender), "Fel format");
            return;
        }
        Setpoint = result;
        windowsForm.errorProvider1.Clear();
        windowsForm.Focus();
    }

    internal void TxtMax_Validating(object sender, CancelEventArgs e)
    {
        var value = ((TextBox)sender).Text;
        if (string.IsNullOrEmpty(value))
        {
            Maximum = double.MaxValue;
            return;
        }
        // Remove whitespace
        value = value.Replace(" ", "");
        if (!double.TryParse(value, out double result))
        {
            e.Cancel = true;
            windowsForm.errorProvider1.SetError(((TextBox)sender), "Fel format");
            return;
        }
        else if (result < Minimum)
        {
            e.Cancel = true;
            windowsForm.errorProvider1.SetError(((TextBox)sender), "Måste vara större än minimum");
            return;
        }
        Maximum = result;
        windowsForm.errorProvider1.Clear();
        windowsForm.Focus();
    }

    internal void TxtMin_Validating(object sender, CancelEventArgs e)
    {
        var value = ((TextBox)sender).Text;
        if (string.IsNullOrEmpty(value))
        {
            Minimum = double.MinValue;
            return;
        }
        // Remove whitespace
        value = value.Replace(" ", "");
        if (!double.TryParse(value, out double result))
        {
            e.Cancel = true;
            windowsForm.errorProvider1.SetError(((TextBox)sender), "Fel format");
            return;
        }
        else if (result > Maximum)
        {
            e.Cancel = true;
            windowsForm.errorProvider1.SetError(((TextBox)sender), "Måste vara mindre än maximum");
            return;
        }
        Minimum = result;
        windowsForm.errorProvider1.Clear();
        windowsForm.Focus();
    }

    internal void TxtStartDate_Validating(object sender, CancelEventArgs e)
    {
        var value = ((TextBox)sender).Text;
        if (string.IsNullOrEmpty(value))
        {
            InputStartDate = DateTime.MinValue;
            return;
        }
        if (value.Length >= 16)
        {
            if (!DateTime.TryParse(value, out DateTime result))
            {
                e.Cancel = true;
                windowsForm.errorProvider1.SetError(((TextBox)sender), "Fel datumformat");
                return;
            }
            else if (result > DateTime.Now.Subtract(Duration))
            {
                e.Cancel = true;
                windowsForm.errorProvider1.SetError(((TextBox)sender), "Sätt ett mindre datum");
                return;
            }
            else if (result < FirstLoggedDate)
            {
                e.Cancel = true;
                windowsForm.errorProvider1.SetError(((TextBox)sender), "Kan ej vara tidigare än första loggningen");
                return;
            }

            InputStartDate = result;
            windowsForm.errorProvider1.Clear();
            windowsForm.Focus();
        }

    }
    #endregion

    internal void Button1_Click(object sender, EventArgs e)
    {
        PlayActive = !PlayActive;
        if (PlayActive)
        {
            windowsForm.btnPlay.ImageIndex = 0;
            windowsForm.txtStartDate.Visible = false;
            windowsForm.btnLeft.Visible = false;
            windowsForm.btnRight.Visible = false;
            windowsForm.btnEnd.Visible = false;
            windowsForm.panelButton.Visible = false;
            InputStartDate = DateTime.MinValue;
            windowsForm.txtStartDate.Invoke((MethodInvoker)delegate
            {
                windowsForm.txtStartDate.Text = string.Empty;
            });
        }
        else
        {
            windowsForm.btnPlay.ImageIndex = 1;
            windowsForm.txtStartDate.Visible = true;
            windowsForm.btnLeft.Visible = true;
            windowsForm.btnRight.Visible = true;
            windowsForm.btnEnd.Visible = true;
            windowsForm.panelButton.Visible = true;
            windowsForm.txtStartDate.Invoke((MethodInvoker)delegate
            {
                var tid = DateTime.Now - Duration;
                windowsForm.txtStartDate.Text = tid.ToString("yyyy-MM-dd HH:mm:ss");
                InputStartDate = tid;
            });
        }
    }

    internal void TrackBar_ValueChanged(object sender, EventArgs e)
    {
        windowsForm.Invoke((MethodInvoker)delegate
        {
            windowsForm.Opacity = windowsForm.trackBar.Value / 100.0;
        });
    }

    internal void OnStatus_TextChanged(object sender, EventArgs e)
    {
        windowsForm.txtStatus.Invoke((MethodInvoker)delegate
        {
            windowsForm.txtStatus.Text = string.Empty;
            timerStatusText.Stop();
            windowsForm.txtStatus.Visible = false;

        });
    }

    internal void BtnLeft_Click(object sender, EventArgs e)
    {
        InputStartDate = InputStartDate.Subtract(Duration);
        windowsForm.txtStartDate.Invoke((MethodInvoker)delegate
        {
            windowsForm.txtStartDate.Text = InputStartDate.ToString("yyyy-MM-dd HH:mm:ss");
        });
    }

    internal void BtnRight_Click(object sender, EventArgs e)
    {
        if (!(InputStartDate.Add(Duration) > DateTime.Now))
            InputStartDate = InputStartDate.Add(Duration);
        windowsForm.txtStartDate.Invoke((MethodInvoker)delegate
        {
            windowsForm.txtStartDate.Text = InputStartDate.ToString("yyyy-MM-dd HH:mm:ss");
        });
    }

    internal void BtnEnd_Click(object sender, EventArgs e)
    {
        InputStartDate = DateTime.Now.Subtract(Duration);
        windowsForm.txtStartDate.Invoke((MethodInvoker)delegate
        {
            windowsForm.txtStartDate.Text = InputStartDate.ToString("yyyy-MM-dd HH:mm:ss");
        });
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

        trendModel.timerStatusText = new() { Interval = 3000 };
        windowsForm.Load += trendModel.WindowsForm_Load;
        windowsForm.FormClosing += trendModel.WindowsForm_FormClosing;
        windowsForm.txtTimeSpan.Validating += trendModel.TxtTimeSpan_Validating;
        windowsForm.txtSetpoint.Validating += trendModel.TxtSetpoint_Validating;
        windowsForm.txtMin.Validating += trendModel.TxtMin_Validating;
        windowsForm.txtMax.Validating += trendModel.TxtMax_Validating;
        windowsForm.txtStartDate.Validating += trendModel.TxtStartDate_Validating;
        windowsForm.btnPlay.Click += trendModel.Button1_Click;
        windowsForm.trackBar.ValueChanged += trendModel.TrackBar_ValueChanged;
        trendModel.timerStatusText.Tick += trendModel.OnStatus_TextChanged;
        windowsForm.btnLeft.Click += trendModel.BtnLeft_Click;
        windowsForm.btnRight.Click += trendModel.BtnRight_Click;
        windowsForm.btnEnd.Click += trendModel.BtnEnd_Click;
        return trendModel;
    }

    public static GearedValues<DateTimePoint> DataTableToDateTimePoints(this DataTable tbl, TimeSpan duration, TimeSpan timeOffset, bool inputDatesOk, DateTime inputDate, TagInfo tagInfo)
    {
        var dateTimePoints = new GearedValues<DateTimePoint>();
        tbl.DefaultView.Sort = "logTime";
        tbl = tbl.DefaultView.ToTable();
        CultureInfo cultureInfo = CultureInfo.InvariantCulture;

        foreach (DataRow row in tbl.Rows)
        {
            DateTimePoint dtPoint = new();
            bool conversionOk = true;
            if (!double.TryParse(row["numericValue"].ToString(), out double value))
                conversionOk = false;
            if ((int)tagInfo.LogFrequency < 1000)
            {
                // Tryparse with milliseconds
                var dateTime = row["logTime"];

                DateTime time = (DateTime)dateTime;

                dtPoint.DateTime = time;
                dtPoint.Value = value;
                dateTimePoints.Add(dtPoint);
            }
            else
            {
                // Tryparse with seconds
                if (!DateTime.TryParse(row["logTime"].ToString(), out DateTime time))
                    conversionOk = false;

                if (conversionOk)
                {
                    dtPoint.DateTime = time;
                    dtPoint.Value = value;
                    dateTimePoints.Add(dtPoint);
                }
            }
        }

        List<DateTimePoint> filteredValues = new();


        if (!inputDatesOk)
            filteredValues = dateTimePoints.Where(x => x.DateTime >= (DateTime.Now.Subtract(duration + timeOffset))
                                 && x.DateTime <= DateTime.Now.Subtract(timeOffset)).ToList();
        else
            filteredValues = dateTimePoints.Where(x => x.DateTime >= (inputDate)
                                     && x.DateTime <= inputDate.Add(duration)).ToList();

        var returnValues = filteredValues.AsChartValues().AsGearedValues();
        if (filteredValues.Count == 0)
        {
            return default;
        }
        else
        {
            return returnValues;
        }
    }

    public static string ToReadableString(this TimeSpan span)
    {
        string formatted = string.Format("{0} {1} {2} {3} {4}",
            span.Duration().Days > 0 ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? string.Empty : "s") : string.Empty,
            span.Duration().Hours > 0 ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? string.Empty : "s") : string.Empty,
            span.Duration().Minutes > 0 ? string.Format("{0:0} min{1}, ", span.Minutes, span.Minutes == 1 ? string.Empty : "s") : string.Empty,
            span.Duration().Seconds > 0 ? string.Format("{0:0}{1}", span.Seconds, span.Seconds == 1 ? string.Empty : "s") : string.Empty,
            span.Duration().Milliseconds > 0 ? string.Format("{0:0} m{1}", span.Milliseconds, span.Milliseconds == 1 ? string.Empty : "s") : string.Empty);

        if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

        if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

        return formatted;
    }
}