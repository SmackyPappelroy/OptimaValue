using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Geared;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using LiveCharts.Wpf.Charts.Base;
using PropertyChanged;
using Serilog;

namespace OptimaValue.Wpf;

/// <summary>
/// Interaction logic for ChartControl.xaml
/// </summary>
public partial class ChartControl : UserControl, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    #region Classes
    public class TagGraphic
    {
        public string Name { get; set; }
        public SolidColorBrush Color { get; set; }

        public TagGraphic()
        {
            Name = string.Empty;
            Color = new SolidColorBrush(Colors.White);
        }
    }
    #endregion

    #region Properties

    private SeriesCollection series = new SeriesCollection();
    public SeriesCollection Series
    {
        get => series;
        set
        {
            series = value;
        }
    }

    private List<DateTime> dates;
    public List<DateTime> Dates
    {
        get { return dates; }
        set { dates = value; }
    }

    private string startTimeString;
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
        }
        catch (Exception ex)
        {
            startDateTime = DateTime.MinValue;
        }
    }

    private List<Tag> displayedTags;
    public List<Tag> DisplayedTags
    {
        get => displayedTags;
        set
        {
            displayedTags = value;
        }
    }


    public DateTime StartTime { get; set; }

    private DateTime startDate = DateTime.Now - TimeSpan.FromDays(1);
    public DateTime StartDate
    {
        get => startDate;
        set
        {
            startDate = value;
            if (!Validation.GetHasError(txtStartTime))
                SetStartTime();
            else
                startDateTime = DateTime.MinValue;
        }
    }

    private DateTime startDateTime = DateTime.MinValue;

    private string stopTimeString;
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
        }
        catch (Exception ex)
        {
            stopDateTime = DateTime.MinValue;
        }
    }

    public DateTime StopTime { get; set; }

    private DateTime stopDate = DateTime.Now;
    public DateTime StopDate
    {
        get => stopDate;
        set
        {
            stopDate = value;
            if (!Validation.GetHasError(txtStopTime))
                SetStopTime();
            else
                stopDateTime = DateTime.MinValue;
        }
    }

    private DateTime stopDateTime = DateTime.MinValue;

    private string tagName = "";
    public string TagName
    {
        get => tagName;
        set
        {
            if (!string.IsNullOrEmpty(value))
                tagName = value;
        }
    }

    private IAxisWindow selectedWindow;
    public IAxisWindow SelectedWindow
    {
        get => selectedWindow;
        set
        {
            selectedWindow = value;
        }
    }

    private Func<double, string> formatterY { get; set; }
    public Func<double, string> FormatterY
    {
        get => formatterY;
        set
        {
            formatterY = value;
        }
    }

    private Func<double, string> formatterX { get; set; }
    public Func<double, string> FormatterX
    {
        get => formatterX;
        set
        {
            formatterX = value;
        }
    }


    private DateTime initialDateTime = DateTime.Now;
    public DateTime InitialDateTime
    {
        get => initialDateTime;
        set
        {
            initialDateTime = value;
        }
    }

    private PeriodUnits period = PeriodUnits.Seconds;
    public PeriodUnits Period
    {
        get => period;
        set
        {
            period = value;
        }
    }


    private double minValueY = 0;
    private double maxValueY = 100;
    public double MinValueY
    {
        get => minValueY;
        set
        {
            minValueY = value;
        }
    }
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
    public double MinValueX
    {
        get => minValueX;
        set
        {
            minValueX = value;
        }
    }
    public double MaxValueX
    {
        get => maxValueX;
        set
        {
            maxValueX = value;
        }
    }

    private string dateSpan;
    public string DateSpan
    {
        get => dateSpan;
        set
        {
            dateSpan = value;
        }
    }

    public List<Tag> AvailableTags;

    private ItemsControl myItemControl;
    public ItemsControl MyItemControl
    {
        get => myItemControl;
        set { myItemControl = value; }
    }


    #endregion

    #region Constructor
    public ChartControl()
    {
        Dates = new();
        LineSeriesList = new();
        this.Loaded += ChartControl_Loaded;
        InitializeComponent();
        StartTimeString = "00:00";
        StopTimeString = "23:59";
        DataContext = this;
        FormatterY = val => val.ToString("0.000");
        Series = new SeriesCollection();
        FormatterX = x => new DateTime((long)x).ToString("yyyy-MM-dd HH:mm:ss.ff");
        EnableButtons();
    }



    #endregion


    private List<string> calcSource;
    public List<string> CalcSource
    {
        get => calcSource;
        set { calcSource = value; }
    }



    public List<MyLineSeries> LineSeriesList;

    public StatisticFilter StatFilter { get; set; } = new();


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
            };

            TextBlock textBlockMin = new TextBlock()
            {
                Margin = new Thickness(0),
                Text = ": " + item.MinValueY.ToString("0.000") + $"{tagUnitString}     ",
                Height = 20,
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 54, 93, 218)),
            };
            System.Windows.Controls.Image imageAvg = new()
            {
                Source = avgBitmap,
                Height = 20,
                Width = 20,
                Stretch = Stretch.UniformToFill,
            };
            TextBlock textBlockAvg = new TextBlock()
            {
                Margin = new Thickness(0),
                Text = ": " + item.AvgValueY.ToString("0.000") + $"{tagUnitString}     ",
                Height = 20,
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 104, 218, 54)),
            };
            System.Windows.Controls.Image imageMax = new()
            {
                Source = maxBitmap,
                Height = 20,
                Width = 20,
                Stretch = Stretch.UniformToFill,
            };
            TextBlock textBlockMax = new TextBlock()
            {
                Margin = new Thickness(0),
                Text = ": " + item.MaxValueY.ToString("0.000") + $"{tagUnitString}     ",
                Height = 20,
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 176, 80, 73)),
            };
            TextBlock textBlockIntegral = new TextBlock()
            {
                Margin = new Thickness(0),
                Text = $"∫ {stats.Integral.ToString("0.0")}",
                Height = 20,
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.AntiqueWhite),
            };
            TextBlock textBlockOverZero = new TextBlock()
            {
                Margin = new Thickness(0),
                Text = $"     >0: {stats.NumberOfTimesOverZero}ggr [{stats.TimeOverZero}]",
                Height = 20,
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.AntiqueWhite),
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
            }

            stackPanel.Children.Add(ellipse);
            stackPanel.Children.Add(textStackPanel);

            stackPanels.Add(stackPanel);
        }

        if (MyItemControl == null)
            MyItemControl = new ItemsControl();

        MyItemControl.ItemsSource = stackPanels;

    }

    private async void ChartControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (!DesignerProperties.GetIsInDesignMode(this))
            await GetAvailableTags();
    }

    private async Task GetAvailableTags()
    {
        var query = $"SELECT DISTINCT MCValueLog.dbo.tagConfig.name,MCValueLog.dbo.tagConfig.description,MCValueLog.dbo.tagConfig.tagUnit FROM MCValueLog.dbo.logValues INNER JOIN MCValueLog.dbo.tagConfig ON MCValueLog.dbo.logValues.tag_id = MCValueLog.dbo.tagConfig.id";
        var connectionString = Config.SqlMethods.ConnectionString;
#if DEBUG
        connectionString = (@"Server=DESKTOP-4OD098D\MINSERVER;Database=MCValueLog;User Id=sa;Password=sa; ");
#endif
        using SqlConnection con = new SqlConnection(Config.SqlMethods.ConnectionString);
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
            Log.Error($"Ingen kontakt med databas {Config.SqlMethods.Server}");
        }

    }

    private async void AddOnClick(object sender, RoutedEventArgs e)
    {
        if (startDateTime == DateTime.MinValue && stopDateTime == DateTime.MinValue)
            return;

        if (startDateTime >= stopDateTime)
            return;

        await ChartData.GetChartDataAsync(startDateTime, stopDateTime);

        if (!AddTag())
            return;
        ConfigureChart(false);
    }



    private async void UpdateOnClick(object sender, RoutedEventArgs e)
    {
        await UpdateChartAsync();
    }

    private async Task UpdateChartAsync(bool changeColor = true)
    {
        if (startDateTime == DateTime.MinValue && stopDateTime == DateTime.MinValue)
            return;

        if (startDateTime >= stopDateTime)
            return;

        foreach (var item in Series)
        {
            var glineSerie = item as GLineSeries;
            Series.Remove(item);
            Series.Add(item);
        }

        startDateTime = new DateTime((long)Series.Chart.AxisX.Min().View.MinValue);
        stopDateTime = new DateTime((long)Series.Chart.AxisX.Max().View.MaxValue);

        startDatePicker.Text = startDateTime.ToString("yyyy-MM-dd");
        txtStartTime.Text = startDateTime.ToString("HH:mm");
        stopDatePicker.Text = stopDateTime.ToString("yyyy-MM-dd");
        txtStopTime.Text = stopDateTime.ToString("HH:mm");

        await ChartData.GetChartDataAsync(startDateTime, stopDateTime);

        if (!AddTag("", true))
            return;

        ConfigureChart(changeColor);
    }



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

        DisplayedTags.RemoveAll(x => x.Name == tagName);
        ConfigureChart();
    }

    private async void StopOnClick(object sender, RoutedEventArgs e)
    {
        source.Cancel();
        EnableButtons();
        startDateTime = oldStartDateTime;
        stopDateTime = oldStopDateTime;
        source = new();
        foreach (var item in LineSeriesList)
        {
            item.LineSeries = null;
        }
        Series.Clear();
        await UpdateChartAsync();
    }

    private CancellationTokenSource source = new();

    DateTime oldStartDateTime = new();
    DateTime oldStopDateTime = new();

    private async void PlayOnClick(object sender, RoutedEventArgs e)
    {
        if (DisplayedTags == null || DisplayedTags.Count == 0)
        {
            MessageBox.Show("Lägg till tag att visa...");
            return;
        }

        oldStartDateTime = startDateTime;
        oldStopDateTime = stopDateTime;

        startDateTime = DateTime.Now - TimeSpan.FromMinutes(11);
        stopDateTime = DateTime.Now - TimeSpan.FromMinutes(1);

        await ChartData.GetChartDataAsync(startDateTime, stopDateTime);
        AddTag("", true);
        ConfigureChart(false);

        if (ChartData.ChartTableAllTags.Rows.Count == 0)
        {
            startDateTime = oldStartDateTime;
            stopDateTime = oldStopDateTime;
            await ChartData.GetChartDataAsync(startDateTime, stopDateTime);
            AddTag();
            ConfigureChart(false);
            MessageBox.Show("Inga nya rader de senaste 10 minuterna");
            return;
        }

        await StartPlayTask(source);
    }

    private async Task StartPlayTask(CancellationTokenSource source)
    {
        TimeSpan timeToAdd = TimeSpan.FromSeconds(1);
        TimeSpan timeSpanStart = TimeSpan.FromMinutes(11);
        TimeSpan timeSpanStop = TimeSpan.FromMinutes(11);

        switch (timeInterval)
        {
            case TimeInterval.HourOne:
                timeSpanStart = TimeSpan.FromHours(1) - TimeSpan.FromMinutes(1);
                timeSpanStop = TimeSpan.FromMinutes(1);
                break;
            case TimeInterval.Min30:
                timeSpanStart = TimeSpan.FromMinutes(30) - TimeSpan.FromMinutes(1);
                timeSpanStop = TimeSpan.FromMinutes(1);
                break;
            case TimeInterval.Min10:
                timeSpanStart = TimeSpan.FromMinutes(10) - TimeSpan.FromMinutes(1);
                timeSpanStop = TimeSpan.FromMinutes(1);
                break;
            case TimeInterval.Min1:
                timeSpanStart = TimeSpan.FromMinutes(1) - TimeSpan.FromSeconds(10);
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
                    foreach (var tag in DisplayedTags)
                    {
                        var line = new MyLineSeries(tag);

                        // Gets value for each tag
                        line.GetChartValues();

                        LineSeriesList.Add(line);

                        Series.Add(line.LineSeries);

                        ConfigureChart(false);

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
                                timeSpanStart = TimeSpan.FromHours(1) - TimeSpan.FromMinutes(1);
                                break;
                            case TimeInterval.Min30:
                                timeSpanStart = TimeSpan.FromMinutes(30) - TimeSpan.FromMinutes(1);
                                break;
                            case TimeInterval.Min10:
                                timeSpanStart = TimeSpan.FromMinutes(10) - TimeSpan.FromMinutes(1);
                                break;
                            case TimeInterval.Min1:
                                timeSpanStart = TimeSpan.FromMinutes(1) - TimeSpan.FromSeconds(10);
                                timeSpanStop = TimeSpan.FromSeconds(10);
                                stopDateTime = DateTime.Now - timeSpanStop;
                                break;
                            default:
                                break;
                        }
                        startDateTime = DateTime.Now - timeSpanStart;
                    }
                    oldTimeInterval = timeInterval;
                    startDateTime += timeToAdd;
                    stopDateTime += timeToAdd;
                    DataTable tbl = await ChartData.GetChartDataAsync(startDateTime, stopDateTime);
                    if (tbl.Rows.Count > 0)
                        UpdateAndAddChartAsync(tbl);
                    await Task.Delay(timeToAdd);

                }
            }
        }
        catch (OperationCanceledException ex)
        {

        }

    }

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

    private void UpdateAndAddChartAsync(DataTable tbl)
    {
        if (startDateTime == DateTime.MinValue && stopDateTime == DateTime.MinValue)
            return;

        if (startDateTime >= stopDateTime)
            return;


        foreach (var item in DisplayedTags)
        {
            var gearedValues = ChartData.AddSeriesValues(item.Name, tbl);
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

    private void ConfigureChart(bool changeColor = true)
    {

        if (LineSeriesList.Count == 0)
        {
            DisplayedTags = new();
            UpdateTagGraphic();
            return;
        }

        ConfigureXAxisRange(minValueX: minValueX, maxValueX: maxValueX);

        foreach (var line in LineSeriesList)
        {

            if (DisplayedTags == null)
            {
                Series.Add(line.LineSeries as GLineSeries);
                continue;
            }


            if (Series.Where(x => x.Title == line.Tag.Name).ToList().Count == 0)
            {
                Series.Add(line.LineSeries as GLineSeries);
            }
            else if (changeColor)
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

    private bool AddTag(string tagName = "", bool update = false)
    {
        if (DisplayedTags == null)
            DisplayedTags = new();

        if (!update)
        {
            if (string.IsNullOrEmpty(tagName))
                tagName = comboTag.SelectedValue.ToString();

            var newTag = AvailableTags.Where(x => x.Name == tagName).FirstOrDefault();

            var tempDisplayedTags = DisplayedTags.ToList();

            if (!tempDisplayedTags.Exists(x => x.Name == tagName))
            {
                DisplayedTags.Add(newTag);
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
        foreach (var item in DisplayedTags)
        {
            MyLineSeries myLine = new(item);
            if (!myLine.GetChartValues())
                return false;

            LineSeriesList.Add(myLine);
        }
        return true;
    }

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



        if (minimumValueY != 0 && diffY >= 0)
            MinValueY = Math.Round(minimumValueY - diffY * percentToAddOnY / 100, 0);
        else
            MinValueY = -5;

        if (MinValueY == MaxValueY)
        {
            MinValueY = MinValueY - 5;
            MaxValueY = MaxValueY + 5;
        }

        MinValueX = LineSeriesList.Min(x => x.MinValueX);
        MaxValueX = LineSeriesList.Max(x => x.MaxValueX);
    }

    private void Axis_OnRangeChanged(LiveCharts.Events.RangeChangedEventArgs eventArgs)
    {
        var currentRange = eventArgs.Range;

        ConfigureXAxisRange(double.NaN, double.NaN, currentRange);
    }

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

    private void OnHourChecked(object sender, RoutedEventArgs e)
    {
        timeInterval = TimeInterval.HourOne;
        check10Min.IsChecked = false;
        check10Min.IsChecked = false;
        check1Min.IsChecked = false;
    }

    private void On30MinChecked(object sender, RoutedEventArgs e)
    {
        timeInterval = TimeInterval.Min30;
        checkHour.IsChecked = false;
        check10Min.IsChecked = false;
        check1Min.IsChecked = false;
    }

    private void On10MinChecked(object sender, RoutedEventArgs e)
    {
        timeInterval = TimeInterval.Min10;
        checkHour.IsChecked = false;
        check30Min.IsChecked = false;
        check1Min.IsChecked = false;
    }

    private void On1MinChecked(object sender, RoutedEventArgs e)
    {
        timeInterval = TimeInterval.Min1;
        checkHour.IsChecked = false;
        check10Min.IsChecked = false;
        check30Min.IsChecked = false;
    }


}

