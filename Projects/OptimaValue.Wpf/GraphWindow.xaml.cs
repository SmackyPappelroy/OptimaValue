using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Geared;
using LiveCharts.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OptimaValue.Config;
using System.Windows.Threading;
using LiveCharts.Wpf;
using System.Windows.Controls.Primitives;
using ClosedXML.Excel;

namespace OptimaValue.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class GraphWindow : Window, INotifyPropertyChanged
{
    private string directoryPath = @$"C:\OptimaValue";
    private string filePath => directoryPath + @$"\Trend{DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss")}.xlsx";
    public event PropertyChangedEventHandler? PropertyChanged;

    private event Action<bool> OnTimeUpdated;

    #region Timer
    public DispatcherTimer timerClearStatus;

    #endregion

    #region Private fields
    private SqlStatus sqlStatus;
    #endregion

    #region Properties
    private ChartPoint selectedChartPoint;
    public ChartPoint SelectedChartPoint
    {
        get => selectedChartPoint;
        set
        {
            selectedChartPoint = value;
        }
    }

    private double cursorScreenPosition;
    public double CursorScreenPosition
    {
        get => cursorScreenPosition;
        set
        {
            cursorScreenPosition = value;
        }
    }

    private string chartStartDate;
    /// <summary>
    /// The start date of the chart
    /// </summary>
    public string ChartStartDate
    {
        get => chartStartDate;
        set
        {
            chartStartDate = value;
        }
    }

    /// <summary>
    /// The end date of the chart
    /// </summary>
    private string chartStopDate;
    public string ChartStopDate
    {
        get => chartStopDate;
        set
        {
            chartStopDate = value;
        }
    }

    /// <summary>
    /// How much statistics it should be shown in the top window
    /// </summary>
    public StatisticFilter StatFilter { get; set; } = new();

    public ZoomingOptions Zoom { get; set; } = ZoomingOptions.X;

    /// <summary>
    /// A line series to plot on the chart
    /// </summary>
    public List<MyLineSeries> LineSeriesList;

    private string statusText = string.Empty;
    /// <summary>
    /// The status text to display on the UI
    /// </summary>
    public string StatusText
    {
        get => statusText;
        set
        {
            statusText = value;
            if (IsLoaded && !string.IsNullOrEmpty(value))
                timerClearStatus.Start();
        }
    }
    private SeriesCollection series = new SeriesCollection();
    /// <summary>
    /// All the plotted series in the chart
    /// </summary>
    public SeriesCollection Series
    {
        get => series;
        set
        {
            series = value;
        }
    }

    private string startTimeString;
    /// <summary>
    /// The start time from the textbox
    /// </summary>
    public string StartTimeString
    {
        get => startTimeString;
        set
        {
            startTimeString = value;
            if (!Validation.GetHasError(txtStartTime))
                SetStartTime();
            else
                startDateTime = DateTime.MinValue;
        }
    }

    /// <summary>
    /// Parses the start time from the textbox and the date to a DateTime<para></para>
    /// Fires an event to update the chart
    /// </summary>
    private void SetStartTime()
    {
        try
        {
            var intPieces = StartTimeString.Split(':');
            var hourPart = int.Parse(intPieces[0]);
            var minutePart = int.Parse(intPieces[1]);
            int secondPart = 0;
            if (intPieces.Length > 2)
                secondPart = int.Parse(intPieces[2]);

            var hourSection = new TimeSpan(hourPart, minutePart, secondPart);
            var daySection = StartDate.Date;

            startDateTime = daySection.Add(hourSection);
            if (IsLoaded)
            {
                OnTimeUpdated?.Invoke(true);
            }
        }
        catch (Exception ex)
        {
            StatusText = "Lyckas inte konvertera start-tid till en DateTime";
            startDateTime = DateTime.MinValue;
        }
    }

    private int NrOfSeriesOnChart => MyChart.Series.Count;

    private List<Tag> tagsPlottedOnChart;
    /// <summary>
    /// The tags that are plotted on the chart 
    /// </summary>
    public List<Tag> TagsPlottedOnChart
    {
        get => tagsPlottedOnChart;
        set
        {
            tagsPlottedOnChart = value;
        }
    }

    private DateTime startDate = DateTime.Now - TimeSpan.FromDays(1);
    /// <summary>
    /// The start date bound to the <see cref="DatePicker"/>
    /// </summary>
    public DateTime StartDate
    {
        get => startDate;
        set
        {
            startDate = value;
            if (!IsLoaded)
                return;
            if (!Validation.GetHasError(txtStartTime))
                SetStartTime();
            else
                startDateTime = DateTime.MinValue;
        }
    }

    private DateTime startDateTime = DateTime.MinValue;

    private string stopTimeString;
    /// <summary>
    /// The time bound to the <see cref="TextBox"/> for stop time
    /// </summary>
    public string StopTimeString
    {
        get => stopTimeString;
        set
        {
            stopTimeString = value;
            if (!Validation.GetHasError(txtStopTime))
                SetStopTime();
            else
                stopDateTime = DateTime.MinValue;
        }
    }

    /// <summary>
    /// Parses the <see cref="TextBox"/> value for stop time and the <see cref="DatePicker"/> stop date
    /// </summary>
    private void SetStopTime()
    {
        try
        {
            var intPieces = StopTimeString.Split(':');
            var hourPart = int.Parse(intPieces[0]);
            var minutePart = int.Parse(intPieces[1]);
            int secondPart = 0;
            if (intPieces.Length > 2)
                secondPart = int.Parse(intPieces[2]);

            var hourSection = new TimeSpan(hourPart, minutePart, secondPart);
            var daySection = StopDate.Date;

            stopDateTime = daySection.Add(hourSection);
            if (IsLoaded)
            {
                OnTimeUpdated?.Invoke(true);
            }
        }
        catch (Exception ex)
        {
            StatusText = "Lyckas inte konvertera stopp-tid till en DateTime";
            stopDateTime = DateTime.MinValue;
        }
    }

    private DateTime stopDate = DateTime.Now;
    /// <summary>
    /// The stop date bound to the <see cref="DatePicker"/>
    /// </summary>
    public DateTime StopDate
    {
        get => stopDate;
        set
        {
            stopDate = value;
            if (!IsLoaded)
                return;
            if (!Validation.GetHasError(txtStopTime))
                SetStopTime();
            else
                stopDateTime = DateTime.MinValue;
        }
    }

    private DateTime stopDateTime = DateTime.MinValue;

    private Func<double, string> formatterY { get; set; }
    /// <summary>
    /// Formats the labels on the Y-axis
    /// </summary>
    public Func<double, string> FormatterY
    {
        get => formatterY;
        set
        {
            formatterY = value;
        }
    }

    private Func<double, string> formatterX { get; set; }
    /// <summary>
    /// Formats the labels on the X-axis for datetime
    /// </summary>
    public Func<double, string> FormatterX
    {
        get => formatterX;
        set
        {
            formatterX = value;
        }
    }

    private double minValueY = 0;
    private double maxValueY = 100;
    /// <summary>
    /// The minimum value displayed on the Y-axis
    /// </summary>
    public double MinValueY
    {
        get => minValueY;
        set
        {
            minValueY = value;
        }
    }
    /// <summary>
    /// The maximum value displayed on the Y-axis
    /// </summary>
    public double MaxValueY
    {
        get => maxValueY;
        set
        {
            maxValueY = value;
        }
    }

    private double minValueX = DateTime.Now.Ticks - TimeSpan.FromDays(1).Ticks;
    private double maxValueX = DateTime.Now.Ticks;
    /// <summary>
    /// The minimum value displayed on the X-axis
    /// </summary>
    public double MinValueX
    {
        get => minValueX;
        set
        {
            minValueX = value;
        }
    }
    /// <summary>
    /// The maximum value displayed on the X-axis
    /// </summary>
    public double MaxValueX
    {
        get => maxValueX;
        set
        {
            maxValueX = value;
        }
    }

    /// <summary>
    /// All the logged tags in Sql
    /// </summary>
    public List<Tag> AvailableTags;

    private ItemsControl myItemControl;
    /// <summary>
    /// The container for the tag statistics
    /// </summary>
    public ItemsControl MyItemControl
    {
        get => myItemControl;
        set { myItemControl = value; }
    }


    #endregion

    #region Constructor
    /// <summary>
    /// The default constructor
    /// </summary>
    public GraphWindow()
    {
        DataContext = this;
        LineSeriesList = new();
        OnTimeUpdated += GraphWindow_OnTimeUpdated;
        timerClearStatus = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(5) };
        timerClearStatus.Tick += TimerClearStatus_Tick;
        this.Loaded += ChartControl_Loaded;
        InitializeComponent();
        StartTimeString = "00:00";
        StopTimeString = "23:59";
        DataContext = this;
        FormatterY = val => val.ToString("0.000");
        Series = new SeriesCollection();
        FormatterX = x => new DateTime((long)x).ToString("yyyy-MM-dd HH:mm:ss.ff");
        EnableButtons();
        StatFilter = StatisticFilter.Max;
    }

    private void TimerClearStatus_Tick(object sender, EventArgs e)
    {
        Dispatcher?.Invoke(() =>
        {
            StatusText = "";
            timerClearStatus.Stop();
        });
    }

    /// <summary>
    /// If the user changes stop or start date / time the chart updates
    /// </summary>
    /// <param name="obj"></param>
    private async void GraphWindow_OnTimeUpdated(bool obj)
    {
        if (IsLoaded && ChartData.HasData)
            await UpdateChartAsync(ChartUpdateAction.UpdateTime);
    }

    #endregion

    #region Private methods
    /// <summary>
    /// Adds icon with name on top
    /// </summary>
    private void UpdateTagGraphic()
    {
        List<StackPanel> stackPanels = new List<StackPanel>();
        var minTid = new DateTime((long)MinValueX);
        var maxTid = new DateTime((long)MaxValueX);

        StackPanel timeStackPanel = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(5, 0, 0, 5),
        };

        TextBlock textBlockTime = new TextBlock()
        {
            Margin = new Thickness(0, 0, 0, 5),
            Text = minTid + "     -     " + maxTid + "     [" + (maxTid - minTid) + "]",
            FontSize = 20,
            FontWeight = FontWeights.Bold,
            //TextDecorations = TextDecorations.Underline,
            Foreground = new SolidColorBrush(Colors.LightGray),
        };
        timeStackPanel.Children.Add(textBlockTime);
        stackPanels.Add(timeStackPanel);

        foreach (var item in LineSeriesList)
        {
            DataStatistics stats = new(StatFilter, item);

            var tagName = item.LineSeries.Title;
            var brush = item.LineSeries.Stroke;
            StackPanel stackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(5, 0, 0, 5),
            };

            Ellipse ellipse = new Ellipse()
            {
                Width = 20,
                Height = 20,
                Fill = brush,
                Margin = new Thickness(5, 0, 5, 0),
            };
            var textStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };

            var tag = AvailableTags.Where(x => x.Name == tagName).FirstOrDefault();
            var tagUnitString = tag.Unit == "" ? "" : " " + tag.Unit;
            var descriptionString = tag.Description == "" ? "" : " " + tag.Description + " ";

            TextBlock textBlockTagName = new TextBlock()
            {
                Margin = new Thickness(0),
                Text = tag.Name + " ",
                FontSize = 14,
                //FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.LightGray),
            };
            TextBlock textBlockUnit = new TextBlock()
            {
                Margin = new Thickness(0),
                Text = tagUnitString,
                Height = 20,
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.Gray),
            };
            TextBlock textBlockDesciption = new TextBlock()
            {
                Margin = new Thickness(0),
                Text = descriptionString,
                Height = 20,
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.Gray),
            };
            TextBlock textBlockAfterDesciption = new TextBlock()
            {
                Margin = new Thickness(0),
                Text = "   ",
                Height = 20,
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.White),
            };

            var minBitmap = new BitmapImage();
            minBitmap.BeginInit();
            minBitmap.UriSource = new Uri(@"/Images/minimum_value_48px.png", UriKind.RelativeOrAbsolute);
            minBitmap.EndInit();

            var avgBitmap = new BitmapImage();
            avgBitmap.BeginInit();
            avgBitmap.UriSource = new Uri(@"/Images/average_48px.png", UriKind.RelativeOrAbsolute);
            avgBitmap.EndInit();

            var maxBitmap = new BitmapImage();
            maxBitmap.BeginInit();
            maxBitmap.UriSource = new Uri(@"/Images/maximum_48px.png", UriKind.RelativeOrAbsolute);
            maxBitmap.EndInit();

            System.Windows.Controls.Image imageMin = new()
            {
                Source = minBitmap,
                Height = 20,
                Width = 20,
                Stretch = Stretch.UniformToFill,
                ToolTip = "Minvärde"
            };

            TextBlock textBlockMin = new TextBlock()
            {
                Margin = new Thickness(0),
                Text = ": " + item.MinValueY.ToString("0.000") + $"{tagUnitString}     ",
                Height = 20,
                FontSize = 14,
                Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 54, 93, 218)),
                ToolTip = "Minvärde"
            };
            System.Windows.Controls.Image imageAvg = new()
            {
                Source = avgBitmap,
                Height = 20,
                Width = 20,
                Stretch = Stretch.UniformToFill,
                ToolTip = "Medelvärde"
            };
            TextBlock textBlockAvg = new TextBlock()
            {
                Margin = new Thickness(0),
                Text = ": " + item.AvgValueY.ToString("0.000") + $"{tagUnitString}     ",
                Height = 20,
                FontSize = 14,
                Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 104, 218, 54)),
                ToolTip = "Medelvärde"
            };
            System.Windows.Controls.Image imageMax = new()
            {
                Source = maxBitmap,
                Height = 20,
                Width = 20,
                Stretch = Stretch.UniformToFill,
                ToolTip = "Maxvärde"
            };
            TextBlock textBlockMax = new TextBlock()
            {
                Margin = new Thickness(0),
                Text = ": " + item.MaxValueY.ToString("0.000") + $"{tagUnitString}     ",
                Height = 20,
                FontSize = 14,
                Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 176, 80, 73)),
                ToolTip = "Maxvärde"
            };
            var integralPerTimme = stats.Integral / (maxTid - minTid).TotalHours;
            TextBlock textBlockIntegral = new TextBlock()
            {
                Margin = new Thickness(0),
                Text = $"∫ {stats.Integral.ToString("0.0")}   ∫/h {integralPerTimme.ToString("0.0")}",
                Height = 20,
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.AntiqueWhite),
                ToolTip = "Area under graf"
            };
            TextBlock textBlockOverZero = new TextBlock()
            {
                Margin = new Thickness(0),
                Text = $"     >0: {stats.NumberOfTimesOverZero}ggr [{stats.TimeOverZero}]",
                Height = 20,
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.AntiqueWhite),
                ToolTip = "Antal gånger över 0, tid över 0"
            };
            TextBlock textBlockStdDev = new TextBlock()
            {
                Margin = new Thickness(0),
                Text = $"    Std.-avvikelse: {stats.StandardDeviation.ToString("0.000")}",
                Height = 20,
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.AntiqueWhite),
                ToolTip = "Genomsnittlig avvikelse från medelvärdet"
            };



            textStackPanel.Children.Add(textBlockTagName);
            //textStackPanel.Children.Add(textBlockUnit);
            textStackPanel.Children.Add(textBlockDesciption);
            textStackPanel.Children.Add(textBlockAfterDesciption);
            textStackPanel.Children.Add(imageMin);
            textStackPanel.Children.Add(textBlockMin);
            textStackPanel.Children.Add(imageAvg);
            textStackPanel.Children.Add(textBlockAvg);
            textStackPanel.Children.Add(imageMax);
            textStackPanel.Children.Add(textBlockMax);
            if (StatFilter == StatisticFilter.Max)
            {
                textStackPanel.Children.Add(textBlockIntegral);
                textStackPanel.Children.Add(textBlockOverZero);
                textStackPanel.Children.Add(textBlockStdDev);
            }

            stackPanel.Children.Add(ellipse);
            stackPanel.Children.Add(textStackPanel);

            stackPanels.Add(stackPanel);
        }

        if (MyItemControl == null)
            MyItemControl = new ItemsControl();

        MyItemControl.ItemsSource = stackPanels;

    }

    /// <summary>
    /// When the <see cref="GraphWindow"/> is first loaded
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ChartControl_Loaded(object sender, RoutedEventArgs e)
    {
        sqlStatus = await SqlMethods.TestSqlConnectionAsync() ? SqlStatus.Connected : SqlStatus.Disconnected;

        if (!DesignerProperties.GetIsInDesignMode(this))
            await GetAvailableTags();
        await SetTimeRange();

        Window w = Window.GetWindow(ControlBorder);
        // w should not be Null now!
        if (null != w)
        {
            w.LocationChanged += delegate (object sender2, EventArgs args)
            {
                var offset = myPopup.HorizontalOffset;
                // "bump" the offset to cause the popup to reposition itself
                //   on its own
                myPopup.HorizontalOffset = offset + 1;
                myPopup.HorizontalOffset = offset;
            };
            // Also handle the window being resized (so the popup's position stays
            //  relative to its target element if the target element moves upon 
            //  window resize)
            w.SizeChanged += delegate (object sender3, SizeChangedEventArgs e2)
            {
                var offset = myPopup.HorizontalOffset;
                myPopup.HorizontalOffset = offset + 1;
                myPopup.HorizontalOffset = offset;
            };
        }
    }

    /// <summary>
    /// Finds max and minimum <see cref="DateTime"/> of the loged values in the database
    /// </summary>
    /// <returns></returns>
    private async Task SetTimeRange()
    {
        if (sqlStatus == SqlStatus.Connected)
        {
            var queryMinTid = $"SELECT MIN(logTime) FROM {SqlSettings.Databas}.dbo.logValues";
            var queryMaxTid = $"SELECT MAX(logTime) FROM {SqlSettings.Databas}.dbo.logValues";
            using SqlConnection con = new SqlConnection(SqlMethods.ConnectionString);
            try
            {
                await con.OpenAsync();
                using SqlCommand cmd = new SqlCommand(queryMinTid, con);
                StartDate = (DateTime)cmd.ExecuteScalar();
                StartTimeString = StartDate.ToString("HH:mm");
                using SqlCommand cmd2 = new SqlCommand(queryMaxTid, con);
                StopDate = (DateTime)cmd2.ExecuteScalar();
                StopTimeString = StopDate.ToString("HH:mm");
            }
            catch (Exception ex)
            {
                StatusText = ex.Message;
            }
        }
    }

    /// <summary>
    /// Finds all distinct logged tags in the database
    /// </summary>
    /// <returns></returns>
    private async Task GetAvailableTags()
    {
        var query = $"SELECT DISTINCT {SqlSettings.Databas}.dbo.tagConfig.name,{SqlSettings.Databas}.dbo.tagConfig.description,{SqlSettings.Databas}.dbo.tagConfig.tagUnit FROM {SqlSettings.Databas}.dbo.logValues INNER JOIN {SqlSettings.Databas}.dbo.tagConfig ON {SqlSettings.Databas}.dbo.logValues.tag_id = {SqlSettings.Databas}.dbo.tagConfig.id";
        var connectionString = Config.SqlMethods.ConnectionString;
        //#if DEBUG
        //        connectionString = (@"Server=DESKTOP-4OD098D\MINSERVER;Database=MCValueLogOrig;User Id=sa;Password=sa; ");
        //#endif
        using SqlConnection con = new SqlConnection(connectionString);
        using SqlCommand cmd = new SqlCommand(query, con);
        try
        {
            await con.OpenAsync();
            var reader = await cmd.ExecuteReaderAsync();

            AvailableTags = new();
            while (reader.Read())
            {
                AvailableTags.Add(new Tag()
                {
                    Name = reader["name"].ToString(),
                    Description = reader["description"].ToString(),
                    Unit = reader["tagUnit"].ToString()
                });
            }
            comboTag.ItemsSource = AvailableTags.AsEnumerable().Select(x => x.Name);
            comboTag.SelectedIndex = 0;
        }
        catch (SqlException ex)
        {
            StatusText = $"Ingen kontakt med databas {SqlMethods.Server}";
            Log.Error($"Ingen kontakt med databas {SqlMethods.Server}");
        }

    }

    /// <summary>
    /// Adds a new lineseries to the chart
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void AddOnClick(object sender, RoutedEventArgs e)
    {
        if (startDateTime == DateTime.MinValue && stopDateTime == DateTime.MinValue)
            return;

        if (startDateTime >= stopDateTime)
            return;

        await ChartData.GetChartDataAsync(startDateTime, stopDateTime);

        if (!AddTag())
            return;
        ConfigureChart();
    }


    private bool isUpdating = false;
    /// <summary>
    /// Updates the chart with with new colors
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void UpdateOnClick(object sender, RoutedEventArgs e)
    {
        //startDateTime = new DateTime((long)Series.Chart.AxisX.Min().View.MinValue);
        //stopDateTime = new DateTime((long)Series.Chart.AxisX.Max().View.MaxValue);

        if (!isUpdating)
        {
            isUpdating = true;
            await UpdateChartAsync(ChartUpdateAction.ChangeColor);
            isUpdating = false;
        }
    }

    /// <summary>
    /// Refreshes the chart
    /// </summary>
    /// <param name="chartUpdateAction"></param>
    /// <returns></returns>
    private async Task UpdateChartAsync(ChartUpdateAction chartUpdateAction)
    {
        if (startDateTime == DateTime.MinValue && stopDateTime == DateTime.MinValue)
            return;

        if (startDateTime >= stopDateTime)
            return;

        foreach (var item in Series)
        {
            Series.Remove(item);
            Series.Add(item);
        }

        if (chartUpdateAction == ChartUpdateAction.Stop)
        {
            startDateTime = oldStartDateTime;
            stopDateTime = oldStopDateTime;
        }

        startDatePicker.Text = startDateTime.ToString("yyyy-MM-dd");
        txtStartTime.Text = startDateTime.ToString("HH:mm");
        stopDatePicker.Text = stopDateTime.ToString("yyyy-MM-dd");
        txtStopTime.Text = stopDateTime.ToString("HH:mm");

        if (startDateTime == stopDateTime)
            return;
        if (await ChartData.GetChartDataAsync(startDateTime, stopDateTime) == null)
            return;

        //if (!AddTag("", true))
        //    return;
        AddTag("", true);

        ConfigureChart(chartUpdateAction);

        ChartStartDate = new DateTime((long)Series.Chart.AxisX.Min().View.MinValue).ToString("yyyy-MM-dd HH:mm:ss.ff");
        ChartStopDate = new DateTime((long)Series.Chart.AxisX.Max().View.MaxValue).ToString("yyyy-MM-dd HH:mm:ss.ff");
    }

    private bool isSaving = false;
    /// <summary>
    /// Saves the chart to a file
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Save(object sender, RoutedEventArgs e)
    {
        if (isSaving)
            return;

        isSaving = true;
        var bitmap = ControlToImage(this.MainGrid, 1200, 1200);

        Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
        dialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg";

        if (dialog.ShowDialog() == true)
        {
            bitmap.Save(dialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
        }
        isSaving = false;
    }

    /// <summary>
    /// Converts a control to an image
    /// </summary>
    /// <param name="target"></param>
    /// <param name="dpiX"></param>
    /// <param name="dpiY"></param>
    /// <returns></returns>
    public static System.Drawing.Bitmap ControlToImage(Visual target, double dpiX, double dpiY)
    {
        if (target == null)
        {
            return null;
        }
        // render control content
        Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
        RenderTargetBitmap rtb = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0),
           (int)(bounds.Height * dpiY / 96.0),
           dpiX,
           dpiY,
           PixelFormats.Pbgra32);
        DrawingVisual dv = new DrawingVisual();
        using (DrawingContext ctx = dv.RenderOpen())
        {
            VisualBrush vb = new VisualBrush(target);
            ctx.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
        }
        rtb.Render(dv);

        //convert image format
        MemoryStream stream = new MemoryStream();
        BitmapEncoder encoder = new BmpBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(rtb));
        encoder.Save(stream);

        return new System.Drawing.Bitmap(stream);
    }


    /// <summary>
    /// Removes a lineseries from the chart
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RemoveOnClick(object sender, RoutedEventArgs e)
    {
        var tagName = comboTag.SelectedValue.ToString();

        if (LineSeriesList == null)
            return;
        if (LineSeriesList.Count == 0)
            return;
        if (!LineSeriesList.Exists(x => x.LineSeries.Title == tagName))
            return;

        var foundList = LineSeriesList.Find(x => x.LineSeries.Title == tagName);
        var foundSeries = Series.First(x => x.Title == tagName);
        if (foundList != null)
        {
            LineSeriesList.Remove(foundList);
            Series.Clear();
        }

        TagsPlottedOnChart.RemoveAll(x => x.Name == tagName);
        ConfigureChart();
    }

    bool dontUpdateXCursor = false;
    /// <summary>
    /// Stops the live feed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void StopOnClick(object sender, RoutedEventArgs e)
    {
        source.Cancel();
        dontUpdateXCursor = false;
        EnableButtons();
        startDateTime = oldStartDateTime;
        stopDateTime = oldStopDateTime;
        source = new();
        foreach (var item in LineSeriesList)
        {
            item.LineSeries = null;
        }
        Series.Clear();
        await UpdateChartAsync(ChartUpdateAction.Stop);
    }

    private CancellationTokenSource source = new();

    DateTime oldStartDateTime = new();
    DateTime oldStopDateTime = new();

    /// <summary>
    /// Starts the live feed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void PlayOnClick(object sender, RoutedEventArgs e)
    {
        if (TagsPlottedOnChart == null || NrOfSeriesOnChart == 0)
        {
            StatusText = "Lägg till tag att visa...";
            return;
        }

        oldStartDateTime = new DateTime((long)Series.Chart.AxisX.Min().View.MinValue);
        oldStopDateTime = new DateTime((long)Series.Chart.AxisX.Max().View.MaxValue);

        startDateTime = DateTime.Now - TimeSpan.FromMinutes(11);
        stopDateTime = DateTime.Now - TimeSpan.FromMinutes(1);

        if (await ChartData.GetChartDataAsync(startDateTime, stopDateTime) == null)
        {
            StatusText = "Inga nya rader de senaste 10 minuterna";
            return;
        }
        if (ChartData.ChartTableAllTags.Rows.Count == 0)
        {
            startDateTime = oldStartDateTime;
            stopDateTime = oldStopDateTime;
            await ChartData.GetChartDataAsync(startDateTime, stopDateTime);
            AddTag();
            ConfigureChart();
            StatusText = "Inga nya rader de senaste 10 minuterna";
            return;
        }
        DisableButtons();
        AddTag("", true);
        ConfigureChart();
        check10Min.IsChecked = true;


        await StartPlayTask(source);
    }

    /// <summary>
    /// The task when displaying live feed
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    private async Task StartPlayTask(CancellationTokenSource source)
    {
        dontUpdateXCursor = true;
        TimeSpan timeToAdd = TimeSpan.FromSeconds(1);
        TimeSpan timeSpanStart = TimeSpan.FromMinutes(11);
        TimeSpan timeSpanStop = TimeSpan.FromMinutes(11);

        switch (timeInterval)
        {
            case TimeInterval.HourOne:
                timeSpanStart = TimeSpan.FromHours(1) + TimeSpan.FromMinutes(1);
                timeSpanStop = TimeSpan.FromMinutes(1);
                break;
            case TimeInterval.Min30:
                timeSpanStart = TimeSpan.FromMinutes(30) + TimeSpan.FromMinutes(1);
                timeSpanStop = TimeSpan.FromMinutes(1);
                break;
            case TimeInterval.Min10:
                timeSpanStart = TimeSpan.FromMinutes(10) + TimeSpan.FromMinutes(1);
                timeSpanStop = TimeSpan.FromMinutes(1);
                break;
            case TimeInterval.Min1:
                timeSpanStart = TimeSpan.FromMinutes(1) + TimeSpan.FromSeconds(10);
                timeSpanStop = TimeSpan.FromSeconds(10);
                break;
            default:
                break;
        }

        startDateTime = DateTime.Now - timeSpanStart;
        stopDateTime = DateTime.Now - timeSpanStop;

        DisableButtons();
        var token = source.Token;

        Series.Clear();
        LineSeriesList.Clear();


        TimeInterval oldTimeInterval = timeInterval;
        try
        {
            while (!token.IsCancellationRequested)
            {
                if (token.IsCancellationRequested)
                    break;

                // Om de inte finns några rader hämtade
                if (Series.Count == 0)
                {
                    // Gets new data
                    await ChartData.GetChartDataAsync(startDateTime, stopDateTime);
                    foreach (var tag in TagsPlottedOnChart)
                    {
                        var line = new MyLineSeries(tag);

                        // Gets value for each tag
                        line.GetChartValues();

                        LineSeriesList.Add(line);

                        Series.Add(line.LineSeries);

                        ConfigureChart();

                        if (MinValueY == MaxValueY)
                        {
                            MinValueY = MinValueY - 5;
                            MaxValueY = MaxValueY - 5;
                        }
                        else
                        {
                            MinValueY = 0;
                        }
                    }
                    await Task.Delay(timeToAdd);
                    startDateTime += timeToAdd;
                    stopDateTime += timeToAdd;
                    continue;
                }
                else
                {
                    if (timeInterval != oldTimeInterval)
                    {
                        switch (timeInterval)
                        {
                            case TimeInterval.HourOne:
                                timeSpanStart = TimeSpan.FromHours(1) + TimeSpan.FromMinutes(1);
                                timeSpanStop = TimeSpan.FromMinutes(1);
                                break;
                            case TimeInterval.Min30:
                                timeSpanStart = TimeSpan.FromMinutes(30) + TimeSpan.FromMinutes(1);
                                timeSpanStop = TimeSpan.FromMinutes(1);
                                break;
                            case TimeInterval.Min10:
                                timeSpanStart = TimeSpan.FromMinutes(10) + TimeSpan.FromMinutes(1);
                                timeSpanStop = TimeSpan.FromMinutes(1);
                                break;
                            case TimeInterval.Min1:
                                timeSpanStart = TimeSpan.FromMinutes(1) + TimeSpan.FromSeconds(10);
                                timeSpanStop = TimeSpan.FromSeconds(10);
                                stopDateTime = DateTime.Now - timeSpanStop;
                                break;
                            default:
                                break;
                        }
                        startDateTime = DateTime.Now - timeSpanStart;
                        stopDateTime = DateTime.Now - timeSpanStop;
                    }
                    oldTimeInterval = timeInterval;
                    startDateTime += timeToAdd;
                    stopDateTime += timeToAdd;
                    DataTable tbl = await ChartData.GetChartDataAsync(startDateTime, stopDateTime);
                    if (tbl == null)
                    {
                        StopOnClick(this, new RoutedEventArgs());
                        break;
                    }
                    if (tbl.Rows.Count > 0)
                        UpdateAndAddChartAsync(tbl);
                    await Task.Delay(timeToAdd);

                }
            }
        }
        catch (OperationCanceledException ex)
        {
            StatusText = ex.Message;
        }

    }

    /// <summary>
    /// Disables all buttons
    /// </summary>
    private void DisableButtons()
    {
        startDatePicker.IsEnabled = false;
        stopDatePicker.IsEnabled = false;
        txtStartTime.IsEnabled = false;
        txtStopTime.IsEnabled = false;
        btnAdd.IsEnabled = false;
        btnRemove.IsEnabled = false;
        btnUpdate.IsEnabled = false;
        comboTag.IsEnabled = false;
        btnRefresh.IsEnabled = false;
        btnPlay.Visibility = Visibility.Hidden;
        btnStop.Visibility = Visibility.Visible;
        btnAdd.Visibility = Visibility.Hidden;
        btnRemove.Visibility = Visibility.Hidden;
        btnUpdate.Visibility = Visibility.Hidden;

        checkHour.Visibility = Visibility.Visible;
        check30Min.Visibility = Visibility.Visible;
        check10Min.Visibility = Visibility.Visible;
        check1Min.Visibility = Visibility.Visible;
    }

    /// <summary>
    /// Enables all buttons
    /// </summary>
    private void EnableButtons()
    {
        startDatePicker.IsEnabled = true;
        stopDatePicker.IsEnabled = true;
        txtStartTime.IsEnabled = true;
        txtStopTime.IsEnabled = true;
        btnAdd.IsEnabled = true;
        btnRemove.IsEnabled = true;
        btnUpdate.IsEnabled = true;
        comboTag.IsEnabled = true;
        btnRefresh.IsEnabled = true;
        btnPlay.Visibility = Visibility.Visible;
        btnStop.Visibility = Visibility.Hidden;
        btnAdd.Visibility = Visibility.Visible;
        btnRemove.Visibility = Visibility.Visible;
        btnUpdate.Visibility = Visibility.Visible;

        checkHour.Visibility = Visibility.Hidden;
        check30Min.Visibility = Visibility.Hidden;
        check10Min.Visibility = Visibility.Hidden;
        check1Min.Visibility = Visibility.Hidden;

        check10Min.IsChecked = true;
    }

    /// <summary>
    /// Updates the chart asynchrously in live feed
    /// </summary>
    /// <param name="tbl"></param>
    private void UpdateAndAddChartAsync(DataTable tbl)
    {
        if (startDateTime == DateTime.MinValue && stopDateTime == DateTime.MinValue)
            return;

        if (startDateTime >= stopDateTime)
            return;


        foreach (var item in TagsPlottedOnChart)
        {
            var gearedValues = ChartData.AddSeriesValues(item.Name, tbl);
            if (gearedValues == null)
                return;
            var serie = Series.Where(x => x.Title == item.Name).FirstOrDefault() as GLineSeries;
            var lineSerie = LineSeriesList.Where(x => x.Tag.Name == item.Name).FirstOrDefault();
            serie.Values.Clear();
            serie.Values.AddRange(gearedValues);
            lineSerie.LineSeries.Values.Clear();
            lineSerie.LineSeries.Values.AddRange(gearedValues);
            var values = lineSerie.LineSeries.Values as GearedValues<DateTimePoint>;

            lineSerie.MinValueY = values.Min(x => x.Value);
            lineSerie.MaxValueY = values.Max(x => x.Value);
            lineSerie.AvgValueY = values.Average(x => x.Value);

            lineSerie.MinValueX = values.Min(x => x.DateTime.Ticks);
            lineSerie.MaxValueX = values.Max(x => x.DateTime.Ticks);
        }


        CalculateMinMaxXY();

        if (MinValueY == MaxValueY)
        {
            MinValueY = MinValueY - 5;
            MaxValueY = MaxValueY - 5;
        }
        else
        {
            MinValueY = 0;
        }

        UpdateTagGraphic();


    }

    /// <summary>
    /// Updates the chart with values
    /// </summary>
    /// <param name="updateAction"></param>
    private void ConfigureChart(ChartUpdateAction updateAction = ChartUpdateAction.Nothing)
    {

        if (LineSeriesList.Count == 0)
        {
            TagsPlottedOnChart = new();
            UpdateTagGraphic();
            return;
        }

        ConfigureXAxisRange(minValueX: minValueX, maxValueX: maxValueX);

        foreach (var line in LineSeriesList)
        {

            if (TagsPlottedOnChart == null)
            {
                Series.Add(line.LineSeries as GLineSeries);
                continue;
            }


            if (Series.Where(x => x.Title == line.Tag.Name).ToList().Count == 0)
            {
                Series.Add(line.LineSeries as GLineSeries);
            }
            else if (updateAction == ChartUpdateAction.ChangeColor)
            {
                // Ändra färg varje gång man uppdaterar
                line.UpdateLineColor();
                var series = Series.Where(x => x.Title == line.Tag.Name).FirstOrDefault() as GLineSeries;
                series.Stroke = line.LineSeries.Stroke;
                series.Fill = line.LineSeries.Fill;
            }
        }

        CalculateMinMaxXY();

        //Tags = LineSeriesList.Select(x => x.LineSeries.Title).ToArray().ToList();
        UpdateTagGraphic();
    }

    /// <summary>
    /// Creates a linechart for the sql values
    /// </summary>
    /// <param name="tagName"></param>
    /// <param name="update"></param>
    /// <returns></returns>
    private bool AddTag(string tagName = "", bool update = false)
    {
        if (TagsPlottedOnChart == null)
            TagsPlottedOnChart = new();

        if (!update)
        {
            if (string.IsNullOrEmpty(tagName))
                tagName = comboTag.SelectedValue.ToString();

            var newTag = AvailableTags.Where(x => x.Name == tagName).FirstOrDefault();

            var tempDisplayedTags = TagsPlottedOnChart.ToList();

            if (!tempDisplayedTags.Exists(x => x.Name == tagName))
            {
                TagsPlottedOnChart.Add(newTag);
            }

            MyLineSeries myLine = new(newTag);
            if (!myLine.GetChartValues())
                return false;

            if (LineSeriesList != null)
            {
                if (LineSeriesList.Count > 0)
                {
                    if (LineSeriesList.Exists(x => x.LineSeries.Title == tagName))
                        return false;
                }
            }

            LineSeriesList.Add(myLine);
            return true;
        }

        LineSeriesList = new();
        foreach (var item in TagsPlottedOnChart)
        {
            MyLineSeries myLine = new(item);
            if (!myLine.GetChartValues())
                return false;

            LineSeriesList.Add(myLine);
        }
        return true;
    }

    /// <summary>
    /// Calculates the limits for a lineseries
    /// </summary>
    private void CalculateMinMaxXY()
    {
        var percentToAddOnY = 7;
        var minimumValueY = LineSeriesList.Min(x => x.MinValueY);
        var maximumValueY = LineSeriesList.Max(x => x.MaxValueY);
        var diffY = maximumValueY - minimumValueY;

        if (diffY > 0)
            MaxValueY = Math.Round(maximumValueY + diffY * percentToAddOnY / 100, 0);
        else if (diffY == 0)
            MaxValueY = maximumValueY + diffY * percentToAddOnY / 100 + 5;
        else
            MaxValueY = maximumValueY + diffY * percentToAddOnY / 100;

        if (MaxValueY == maximumValueY)
            MaxValueY *= 2;



        if (minimumValueY >= 0 && diffY >= 0)
            MinValueY = 0;
        else if (diffY > 0)
            MinValueY = minimumValueY;
        else
            MinValueY = maximumValueY - 5;

        if (MinValueY == MaxValueY)
        {
            MinValueY = MinValueY - 5;
            MaxValueY = MaxValueY + 5;
        }

        MinValueX = LineSeriesList.Min(x => x.MinValueX);
        MaxValueX = LineSeriesList.Max(x => x.MaxValueX);
    }

    /// <summary>
    /// When axis changes reconfigure
    /// </summary>
    /// <param name="eventArgs"></param>
    private void Axis_OnRangeChanged(LiveCharts.Events.RangeChangedEventArgs eventArgs)
    {
        var currentRange = eventArgs.Range;

        ConfigureXAxisRange(double.NaN, double.NaN, currentRange);
    }

    /// <summary>
    /// Helper method for updating X-axis
    /// </summary>
    /// <param name="minValueX"></param>
    /// <param name="maxValueX"></param>
    /// <param name="range"></param>
    private void ConfigureXAxisRange(double minValueX, double maxValueX, double range = double.NaN)
    {
        double currentRange = 0;
        if (range == double.NaN)
            currentRange = maxValueX - minValueX;
        else
            currentRange = range;

        if (currentRange < TimeSpan.TicksPerDay * 2)
        {
            FormatterX = x => new DateTime((long)x).ToString("yyyy-MM-dd HH:mm:ss.ff");
            return;
        }

        if (currentRange < TimeSpan.TicksPerDay * 60)
        {
            FormatterX = x => new DateTime((long)x).ToString("yyyy-MM-dd HH:mm:ss");
            return;
        }

        if (currentRange < TimeSpan.TicksPerDay * 540)
        {
            FormatterX = x => new DateTime((long)x).ToString("yyyy-MM-dd HH:mm:ss");
            return;
        }

        FormatterX = x => new DateTime((long)x).ToString("yyyy-MM-dd HH:mm:ss");
    }

    private TimeInterval timeInterval = TimeInterval.Min10;

    /// <summary>
    /// Show hourly live feed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnHourChecked(object sender, RoutedEventArgs e)
    {
        timeInterval = TimeInterval.HourOne;
        check10Min.IsChecked = false;
        check10Min.IsChecked = false;
        check1Min.IsChecked = false;
    }

    /// <summary>
    /// Show 30-mins feed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void On30MinChecked(object sender, RoutedEventArgs e)
    {
        timeInterval = TimeInterval.Min30;
        checkHour.IsChecked = false;
        check10Min.IsChecked = false;
        check1Min.IsChecked = false;
    }

    /// <summary>
    /// Show 10 mins feed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void On10MinChecked(object sender, RoutedEventArgs e)
    {
        timeInterval = TimeInterval.Min10;
        checkHour.IsChecked = false;
        check30Min.IsChecked = false;
        check1Min.IsChecked = false;
    }

    /// <summary>
    /// Show 1 min feed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void On1MinChecked(object sender, RoutedEventArgs e)
    {
        timeInterval = TimeInterval.Min1;
        checkHour.IsChecked = false;
        check10Min.IsChecked = false;
        check30Min.IsChecked = false;
    }


    #endregion

    /// <summary>
    /// Updates the chart if the statistics filter <see cref="ComboBox"/> changes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void comboStats_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IsLoaded && ChartData.HasData)
        {
            await UpdateChartAsync(ChartUpdateAction.Nothing);
        }
    }

    private async void comboZoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IsLoaded && ChartData.HasData)
        {
            await UpdateChartAsync(ChartUpdateAction.Nothing);
        }
    }

    /// <summary>
    /// Updates the stopdate to the last logged value logdate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void GotoEnd_Click(object sender, RoutedEventArgs e)
    {
        if (sqlStatus == SqlStatus.Connected)
        {
            var queryMaxTid = $"SELECT MAX(logTime) FROM {SqlSettings.Databas}.dbo.logValues";
            using SqlConnection con = new SqlConnection(SqlMethods.ConnectionString);
            try
            {
                await con.OpenAsync();
                using SqlCommand cmd2 = new SqlCommand(queryMaxTid, con);
                StopDate = (DateTime)cmd2.ExecuteScalar();
                StopTimeString = StopDate.ToString("HH:mm");
                await UpdateChartAsync(ChartUpdateAction.Nothing);
            }
            catch (Exception ex)
            {
                StatusText = ex.Message;
            }
        }
    }

    private void MyChart_LayoutUpdated(object sender, EventArgs e)
    {
        if (TagsPlottedOnChart == null)
            return;
        if (IsLoaded && NrOfSeriesOnChart > 0)
        {
            ChartStartDate = new DateTime((long)Series.Chart.AxisX.Min().View.MinValue).ToString("yyyy-MM-dd HH:mm:ss.ff");
            ChartStopDate = new DateTime((long)Series.Chart.AxisX.Max().View.MaxValue).ToString("yyyy-MM-dd HH:mm:ss.ff");
        }
    }

    private async void RefreshOnClick(object sender, RoutedEventArgs e)
    {
        if (TagsPlottedOnChart == null)
            return;
        if (IsLoaded && NrOfSeriesOnChart > 0)
        {
            startDateTime = new DateTime((long)Series.Chart.AxisX.Min().View.MinValue);
            stopDateTime = new DateTime((long)Series.Chart.AxisX.Max().View.MaxValue);
            await UpdateChartAsync(ChartUpdateAction.UpdateTime);
        }
    }

    private void MoveChartCursorAndToolTip_OnMouseMove(object sender, MouseEventArgs e)
    {
        try
        {
            if (TagsPlottedOnChart == null)
                return;
            if (!IsLoaded || NrOfSeriesOnChart == 0)
                return;
            if (dontUpdateXCursor)
                return;

            var chart = sender as CartesianChart;

            if (!TryFindVisualChildElement(chart, out Canvas outerCanvas) ||
                !TryFindVisualChildElement(outerCanvas, out Canvas graphPlottingArea))
            {
                return;
            }

            Point chartMousePosition = e.GetPosition(chart);

            // Remove visual hover feedback for previous point
            SelectedChartPoint?.View.OnHoverLeave(SelectedChartPoint);

            // Find current selected chart point for the first x-axis
            Point chartPoint = chart.ConvertToChartValues(chartMousePosition);
            SelectedChartPoint = chart.Series[0].ClosestPointTo(chartPoint.X, AxisOrientation.X);

            // Show visual hover feedback for previous point
            if (SelectedChartPoint != null && SelectedChartPoint.View != null)
                SelectedChartPoint.View.OnHover(SelectedChartPoint);
            else
                return;


            // Add the cursor for the x-axis.
            // Since Chart internally reverses the screen coordinates
            // to match chart's coordinate system
            // and this coordinate system orientation applies also to Chart.VisualElements,
            // the UIElements like Popup and Line are added directly to the plotting canvas.
            if (chart.TryFindResource("CursorX") is Line cursorX
              && !graphPlottingArea.Children.Contains(cursorX))
            {
                graphPlottingArea.Children.Add(cursorX);
            }

            if (!(chart.TryFindResource("CursorXToolTip") is FrameworkElement cursorXToolTip))
            {
                return;
            }
        }
        catch (Exception ex)
        {
            StatusText = ex.Message;
        }

        // Add the cursor for the x-axis.
        // Since Chart internally reverses the screen coordinates
        // to match chart's coordinate system
        // and this coordinate system orientation applies also to Chart.VisualElements,
        // the UIElements like Popup and Line are added directly to the plotting canvas.
        //if (!graphPlottingArea.Children.Contains(cursorXToolTip))
        //{
        //    graphPlottingArea.Children.Add(cursorXToolTip);
        //}

        // Position the ToolTip
        //Point canvasMousePosition = e.GetPosition(graphPlottingArea);
        //Canvas.SetLeft(cursorXToolTip, canvasMousePosition.X - cursorXToolTip.ActualWidth);
        //Canvas.SetTop(cursorXToolTip, canvasMousePosition.Y);
    }

    // Helper method to traverse the visual tree of an element
    private bool TryFindVisualChildElement<TChild>(DependencyObject parent, out TChild resultElement)
      where TChild : DependencyObject
    {
        resultElement = null;
        for (var childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(parent); childIndex++)
        {
            DependencyObject childElement = VisualTreeHelper.GetChild(parent, childIndex);

            if (childElement is Popup popup)
            {
                childElement = popup.Child;
            }

            if (childElement is TChild)
            {
                resultElement = childElement as TChild;
                return true;
            }

            if (TryFindVisualChildElement(childElement, out resultElement))
            {
                return true;
            }
        }

        return false;
    }

    private async Task<bool> ExportChartDataToExcel(List<MyLineSeries> MyLineSeries)
    {
        using var wb = new XLWorkbook();
        Directory.CreateDirectory(directoryPath);

        int index = 0;
        foreach (var lineSeries in MyLineSeries)
        {
            index++;
            var sheetName = "Tag_" + index;
            var ws = wb.Worksheets.Add(lineSeries.ToDataTable(new DateTime((long)Series.Chart.AxisX.Min().View.MinValue)
                , new DateTime((long)Series.Chart.AxisX.Max().View.MaxValue)), sheetName);

            ws.Column(2).Style.NumberFormat.Format = "yyyy-mm-dd hh:mm:ss";
            ws.Columns().AdjustToContents();
        }
        if (MyLineSeries.Count == 0)
        {
            Dispatcher?.Invoke(() => StatusText = "Hittade ingen data att spara");
            return false;
        }

        wb.SaveAs(filePath);
        await Task.Delay(1);
        return true;
    }



    private async void Button_Excel(object sender, RoutedEventArgs e)
    {
        if (TagsPlottedOnChart == null)
            return;
        if (IsLoaded && NrOfSeriesOnChart > 0)
        {
            startDateTime = new DateTime((long)Series.Chart.AxisX.Min().View.MinValue);
            stopDateTime = new DateTime((long)Series.Chart.AxisX.Max().View.MaxValue);
            await UpdateChartAsync(ChartUpdateAction.UpdateTime);
            if (await ExportChartDataToExcel(LineSeriesList))
            {
                Dispatcher?.Invoke(() =>
                {
                    StatusText = $"{filePath} sparades";
                });
            }
        }
    }
}





