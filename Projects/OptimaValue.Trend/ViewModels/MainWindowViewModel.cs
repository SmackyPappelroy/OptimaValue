using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using GongSolutions.Wpf.DragDrop;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Geared;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Color = System.Drawing.Color;

namespace OptimaValue.Trend
{
    public class MainWindowViewModel : INotifyPropertyChanged, IDropTarget
    {
        public double XPointer { get; set; }
        public double YPointer { get; set; }
        private DispatcherTimer timerStatusMessage;
        public string WindowTitle { get; set; } = "OptimaValueTrend";
        public int DarkTheme { get; set; }
        public bool HasData => Lines?.Count > 0;
        public LinearGradientBrush TrendBackGround { get; set; }
        public ZoomingOptions Zoom { get; set; } = ZoomingOptions.None;
        public AxesCollection AxisCollection { get; set; } = new();
        public SeriesCollection Series { get; set; }
        public BindingList<Line> Lines { get; set; }
        public DateTime StartDate => new DateTime(InputStartDate.Ticks + InputStartTime.Ticks);
        public DateTime EndDate => new DateTime(InputEndDate.Ticks + InputEndTime.Ticks);
        public DateTime InputStartDate { get; set; } = DateTime.Now.Date;
        public DateTime InputEndDate { get; set; } = DateTime.Now.Date;
        public TimeOnly InputStartTime { get; set; } = new TimeOnly(0, 0, 1);
        public TimeOnly InputEndTime { get; set; } = new TimeOnly(23, 59, 59);
        public double MinValueGraph { get; set; }
        public double MaxValueGraph { get; set; }
        public double MinSliderValue { get; set; }
        public double MaxSliderValue { get; set; }
        public DateTime MinDateGraph { get; set; }
        public DateTime MaxDateGraph { get; set; }
        private DateTime minDateSeries;
        public DateTime MinDateSeries
        {
            get => minDateSeries;
            set
            {
                minDateSeries = value;
                StaticClass.MinDateSeries = minDateSeries;
                UpdateTime();
            }
        }
        private DateTime maxDateSeries;
        public DateTime MaxDateSeries
        {
            get => maxDateSeries;
            set
            {
                maxDateSeries = value;
                StaticClass.MaxDateSeries = maxDateSeries;
                UpdateTime();
            }
        }

        private void UpdateTime()
        {
            if (Series == null || Series.Count == 0)
                return;

            window.MyChart.AxisX[0].MinValue = MinDateSeries.Ticks;
            window.MyChart.AxisX[0].MaxValue = MaxDateSeries.Ticks;
        }

        public TimeSpan DiffTimeSeries { get; set; }
        public string StatusMessage { get; set; }


        private event Action<string> OnStatusMessage;
        public MainWindowViewModel()
        {

        }

        public MainWindowViewModel(MainWindow _window)
        {
            TrendBackGround = new LinearGradientBrush()
            {
                StartPoint = new System.Windows.Point(0, 0),
                EndPoint = new System.Windows.Point(0, 1),
                GradientStops = new GradientStopCollection()
                {
                    new System.Windows.Media.GradientStop((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1d2c35"),0),
                    new System.Windows.Media.GradientStop((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF000000"),1),
                }
            };
            model = new MainWindowModel();
            window = _window;
            OnStatusMessage += MainWindowViewModel_OnStatusMessage;
            window.MyChart.UpdaterTick += ((sender) =>
            {
                var chart = sender as CartesianChart;
                if (chart.Series != null && chart.Series.Count > 0)
                {
                    var temp = chart.AxisX[0].MinValue;
                    List<DateTime> minPoints = new();
                    List<DateTime> maxPoints = new();
                    for (int i = 0; i < chart.Series.Count; i++)
                    {
                        var minPoint = chart.Series[i].ClosestPointTo(chart.AxisX[0].ActualMinValue, AxisOrientation.X);
                        var maxPoint = chart.Series[i].ClosestPointTo(chart.AxisX[0].ActualMaxValue, AxisOrientation.X);

                        if (minPoint == null || maxPoint == null)
                            return;
                        var minDateTimePoint = (DateTimePoint)minPoint.Instance;
                        var maxTimePoint = (DateTimePoint)maxPoint.Instance;
                        minPoints.Add(minDateTimePoint.DateTime);
                        maxPoints.Add(maxTimePoint.DateTime);
                    }

                    // Min value on chart AxisX where min value is not double.NaN
                    MinDateSeries = new DateTime((long)chart.AxisX.Min().ActualMinValue);
                    MaxDateSeries = new DateTime((long)chart.AxisX.Max().ActualMaxValue);
                    DiffTimeSeries = MaxDateSeries > MinDateSeries ? MaxDateSeries - MinDateSeries : TimeSpan.MinValue;
                }

            });

        }



        private void MainWindowViewModel_OnStatusMessage(string obj)
        {
            StatusMessage = obj;
            if (timerStatusMessage == null)
            {
                timerStatusMessage = new()
                {
                    Interval = TimeSpan.FromSeconds(5),
                };



            }
            timerStatusMessage.Tick += ((sender, e) =>
            {
                StatusMessage = String.Empty;
                timerStatusMessage.Stop();
            });
            if (StatusMessage != String.Empty)
            {
                timerStatusMessage.Start();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        internal MainWindow window;
        private MainWindowModel model;


        private List<GridItem> availableItems = new();
        public List<GridItem> AvailableItems
        {
            get
            {
                if (availableItems.Count == 0)
                {
                    availableItems = (model ??= new()).GetGridItems();
                }
                return availableItems;
            }
            set
            {
                availableItems = value;
            }
        }
        public ObservableCollection<GridItem> SelectedItems { get; set; } = new();
        public GridItem SelectedItem { get; set; }

        private ICommand saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                    saveCommand = new RelayCommand((obj) => SaveTrend());
                return saveCommand;
            }
        }
        private ICommand loadCommand;
        public ICommand LoadCommand
        {
            get
            {
                if (loadCommand == null)
                    loadCommand = new RelayCommand(async (obj) => await LoadTrend());
                return loadCommand;
            }
        }

        private async Task LoadTrend()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Trend files (*.trend)|*.trend";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    await SaveClass.LoadData(this, dialog.FileName);

                    OnStatusMessage?.Invoke($"Laddade trend: {dialog.FileName}");

                }
                catch (Exception ex)
                {
                    OnStatusMessage?.Invoke($"Error loading trend: {ex.Message}");
                }
            }
        }

        private void SaveTrend()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Trend files (*.trend)|*.trend";
            if (dialog.ShowDialog() == true)
            {
                var fileName = dialog.FileName;

                SaveClass.SaveData(this, fileName);
                OnStatusMessage?.Invoke($"Sparade {fileName}");
            }
        }

        private ICommand darkThemeCommand;
        public ICommand DarkThemeCommand
        {
            get
            {
                if (darkThemeCommand == null)
                    darkThemeCommand = new RelayCommand((obj) => ToggleDarkTheme());
                return darkThemeCommand;
            }
        }

        private void ToggleDarkTheme()
        {
            if (DarkTheme == 0)
            {
                // Light theme
                DarkTheme = 1;
                TrendBackGround = new LinearGradientBrush()
                {
                    StartPoint = new System.Windows.Point(0, 0),
                    EndPoint = new System.Windows.Point(0, 1),
                    GradientStops = new GradientStopCollection()
                {
                    new System.Windows.Media.GradientStop((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFE7E7E7"),0),
                    new System.Windows.Media.GradientStop((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFFFFFFF"),1),
                }
                };
            }
            else if (DarkTheme == 1)
            {
                // Dark theme
                DarkTheme = 0;
                TrendBackGround = new LinearGradientBrush()
                {
                    StartPoint = new System.Windows.Point(0, 0),
                    EndPoint = new System.Windows.Point(0, 1),
                    GradientStops = new GradientStopCollection()
                {
                    new System.Windows.Media.GradientStop((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1d2c35"),0),
                    new System.Windows.Media.GradientStop((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF000000"),1),
                }
                };
            }
        }

        private ICommand moveUpCommand;
        public ICommand MoveUpCommand
        {
            get
            {
                if (moveUpCommand == null)
                    moveUpCommand = new RelayCommand((obj) => MoveUp());
                return moveUpCommand;
            }
        }

        private void MoveUp()
        {
            GridItem grid = window.DataGridTags.SelectedItem as GridItem;
            if (grid == null)
                return;

            var minValue = grid.scaleMin;
            var maxValue = grid.scaleMax;
            var offset = grid.scaleOffset;

            var step = (maxValue - minValue) / 100 * 20;

            grid.scaleOffset -= step;
        }

        private ICommand moveDownCommand;
        public ICommand MoveDownCommand
        {
            get
            {
                if (moveDownCommand == null)
                    moveDownCommand = new RelayCommand((obj) => MoveDown());
                return moveDownCommand;
            }
        }

        private void MoveDown()
        {
            GridItem grid = window.DataGridTags.SelectedItem as GridItem;
            if (grid == null)
                return;

            var minValue = grid.scaleMin;
            var maxValue = grid.scaleMax;
            var offset = grid.scaleOffset;

            var step = (maxValue - minValue) / 100 * 20;

            grid.scaleOffset += step;
        }

        private ICommand scaleOriginalCommand;
        public ICommand ScaleOriginalCommand
        {
            get
            {
                if (scaleOriginalCommand == null)
                {
                    scaleOriginalCommand = new RelayCommand((obj) => ScaleOriginal());
                }
                return scaleOriginalCommand;
            }
        }

        private void ScaleOriginal()
        {
            var selectedItem = window.DataGridTags.SelectedItem as GridItem;
            if (selectedItem == null)
                return;

            var line = StaticClass.Lines.Where(x => x.Tag.id == selectedItem.id).FirstOrDefault();
            if (line == null)
                return;

            selectedItem.scaleMin = (int)line.Tag.scaleMin;
            selectedItem.scaleMax = (int)line.Tag.scaleMax;
            FormatSeries(selectedItem.id);
        }

        private ICommand autoScaleCommand;
        public ICommand AutoScaleCommand
        {
            get
            {
                if (autoScaleCommand == null)
                {
                    autoScaleCommand = new RelayCommand((obj) => AutoScale());
                }
                return autoScaleCommand;
            }
        }

        private void AutoScale()
        {

            var selectedItem = window.DataGridTags.SelectedItem as GridItem;
            if (selectedItem == null)
                return;

            var line = StaticClass.Lines.Where(x => x.Tag.id == selectedItem.id).FirstOrDefault();
            if (line == null)
                return;

            // Hitta högsta och lägsta värde i synlig graf
            var serie = window.MyChart.Series.Where(x => x.Title == selectedItem.name).FirstOrDefault();
            var axisY = AxisCollection.Where(x => (int)x.Tag == selectedItem.id).FirstOrDefault();
            var minPoint = serie.ClosestPointTo(axisY.ActualMinValue, AxisOrientation.Y);
            var maxPoint = serie.ClosestPointTo(axisY.ActualMaxValue, AxisOrientation.Y);
            if (minPoint == null || maxPoint == null)
                return;
            var minDateTimePoint = (DateTimePoint)minPoint.Instance;
            var maxTimePoint = (DateTimePoint)maxPoint.Instance;

            selectedItem.scaleMin = (int)minDateTimePoint.Value;
            selectedItem.scaleMax = (int)maxTimePoint.Value;
            FormatSeries(selectedItem.id);
        }

        private ICommand loadDataCommand;
        public ICommand LoadDataCommand
        {
            get
            {
                if (loadDataCommand == null)
                {
                    loadDataCommand = new RelayCommand(async (obj) => await LoadData());
                }
                return loadDataCommand;
            }
        }

        private async Task LoadData()
        {
            if (MaxDateSeries > MinDateSeries)
            {
                foreach (var line in Lines)
                {
                    await line.RefreshData(MinDateSeries, MaxDateSeries);
                }
                FormatSeries();
                InputStartDate = new DateTime((long)window.MyChart.AxisX[0].MinValue).StartOfDay();
                InputStartTime = TimeOnly.FromDateTime(new DateTime((long)window.MyChart.AxisX[0].MinValue));
                InputEndDate = new DateTime((long)window.MyChart.AxisX[0].MaxValue).StartOfDay();
                InputEndTime = TimeOnly.FromDateTime(new DateTime((long)window.MyChart.AxisX[0].MaxValue));
            }

        }


        /// <summary>
        /// Add a lineseries to the list
        /// </summary>
        /// <param name="tagId"></param>
        public async void AddLine(int tagId)
        {
            if (tagId < 0)
            {
                RaiseMessage("Välj en tag");
                return;
            }
            if (Lines == null)
                Lines = new();

            if (Lines.Any(l => l.Tag.id == tagId))
            {
                RaiseMessage("Tag finns redan");
                return;
            }

            try
            {
                ValidateStartAndEndDate();
            }
            catch (Exception ex)
            {
                RaiseMessage(ex.Message);
                return;
            }

            Line line = new(tagId, this);
            StaticClass.AddLine(line);

            try
            {
                int index = 0;
                if (Series != null)
                {
                    index = (int)Series.Count;
                }
                await line.SetupLine(StartDate, EndDate, index);
                if (Series == null)
                {
                    AxisCollection.Clear();
                    Series = new();
                }
                if (Series.Count == 0)
                {
                    AxisCollection.Clear();
                    Series = new();
                }
                if (line.GLineSeries != null)
                {
                    Series.Add(line.GLineSeries);
                    var myIndex = Series.IndexOf(line.GLineSeries);
                    line.Tag.SeriesIndex = myIndex;
                    Lines.Add(line);
                    AxisCollection.Add(line.AxisY);
                    FormatSeries();
                }
            }
            catch (ChartNoDataInSqlException ex)
            {
                RaiseMessage(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                RaiseMessage(ex.Message);
                return;
            }
        }
        public Func<double, string> FormatterY;


        public void FormatSeries(int id = int.MinValue, int offset = 0)
        {
            if (Lines.Count == 0)
            {
                MinValueGraph = 0;
                MaxValueGraph = 0;
                MinDateGraph = MaxDateGraph = MinDateSeries = MaxDateSeries = DateTime.MinValue;
                return;
            }
            foreach (var line in Lines)
            {
                int offSet = 0;
                if (id == line.Tag.id)
                {
                    offSet = offset;

                    var gridItem = SelectedItems.FirstOrDefault(i => i.id == line.Tag.id);

                    var yAxis = window.MyChart.AxisY.Where(x => (int)x.Tag == line.Tag.id).FirstOrDefault();
                    yAxis.MaxValue = line.Tag.scaleMax + offSet;
                    yAxis.MinValue = line.Tag.scaleMin + offSet;
                }
                if (line.Tag.scaleMax == line.Tag.scaleMin || line.Tag.scaleMax == 0 && line.Tag.scaleMin == 0)
                {

                    if ((int)line.MaxValue == 0 && (int)line.MinValue == 0)
                    {
                        line.Tag.scaleMax = 1 + offSet;
                        line.Tag.scaleMin = -1 + offSet;
                    }
                    else
                    {
                        line.Tag.scaleMax = (int)line.MaxValue;
                        line.Tag.scaleMin = (int)line.MinValue;
                    }
                    var gridItem = SelectedItems.FirstOrDefault(i => i.id == line.Tag.id);
                    gridItem.scaleMin = line.Tag.scaleMin;
                    gridItem.scaleMax = line.Tag.scaleMax;
                    line.AxisY.MinValue = line.Tag.scaleMin;
                    line.AxisY.MaxValue = line.Tag.scaleMax;
                    var yAxis = window.MyChart.AxisY.Where(x => (int)x.Tag == line.Tag.id).FirstOrDefault();
                    yAxis.MaxValue = line.Tag.scaleMax;
                    yAxis.MinValue = line.Tag.scaleMin;
                }
            }
            //var maxY = Lines.Select(x => x.MaxValue).Max();
            //window.MyChart.AxisY.First().MaxValue = maxY + maxY * 0.1;
            //window.MyChart.AxisY.First().MinValue = Lines.Select(x => x.MinValue).Min();
            //window.MyChart.AxisY.First().LabelFormatter = Lines.Select(x => x.FormatterY).First();
            //window.MyChart.AxisY.First().Separator = DefaultAxes.CleanSeparator;

            window.MyChart.AxisX.First().Separator = DefaultAxes.CleanSeparator;
            window.MyChart.AxisX.First().LabelFormatter = Lines.Select(x => x.FormatterX).First();

            MinDateGraph = Lines.Select(x => x.MinDate).Min();
            MaxDateGraph = Lines.Select(x => x.MaxDate).Max();

            MinValueGraph = Lines.Select(x => x.MinValue).Min();
            MaxValueGraph = Lines.Select(x => x.MaxValue).Max();
            //window.MinSlider.Value = MinValueGraph;
            //window.MaxSlider.Value = MaxValueGraph;
            //window.MaxSlider.TickFrequency = MaxValueGraph / 100;
            //window.MinSlider.TickFrequency = MaxValueGraph / 100;

        }

        /// <summary>
        /// Validate star and enddate
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private bool ValidateStartAndEndDate()
        {
            if (InputStartDate > InputEndDate)
                throw new Exception("Start date must be before end date");

            return true;
        }

        /// <summary>
        /// Raise a message that is displayed on the UI
        /// </summary>
        /// <param name="message"></param>
        private void RaiseMessage(string message)
        {
            OnStatusMessage?.Invoke(message);
        }

        public void DragOver(IDropInfo dropInfo)
        {
            GridItem sourceItem = dropInfo.Data as GridItem;
            ObservableCollection<GridItem> targetItems = dropInfo.TargetCollection as ObservableCollection<GridItem>;

            if (sourceItem != null && targetItems != null) //&& !targetItems.Any(x => x.id == sourceItem.id))
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            GridItem sourceItem = (GridItem)dropInfo.Data;

            ObservableCollection<GridItem> targetItems = (ObservableCollection<GridItem>)dropInfo.TargetCollection;
            var targetIndex = dropInfo.InsertIndex;
            if (targetItems.Any(x => x.id == sourceItem.id))
            {
                targetItems.Remove(sourceItem);
                if (targetIndex < targetItems.Count)
                {
                    sourceItem.active = true;
                    targetItems.Insert(targetIndex, sourceItem);
                }
                else
                {
                    sourceItem.active = true;
                    targetItems.Add(sourceItem);
                }
                return;
            }
            var targetList = targetItems.ToList();
            if (!targetItems.Any(x => x.id == sourceItem.id))
            {
                sourceItem.active = true;
                targetItems.Add(sourceItem);
                AddLine(sourceItem.id);

                sourceItem.OnSmoothingChanged += ((smoothing) =>
                {
                    var line = Series.Where(x => x.Title == sourceItem.name).FirstOrDefault() as GLineSeries;
                    if (line == null) return;

                    line.LineSmoothness = smoothing;
                });
                sourceItem.OnScaleMinChanged += ((scaleMin) =>
                {
                    var axisY = AxisCollection.Where(x => (int)x.Tag == sourceItem.id).FirstOrDefault();
                    if (axisY == null) return;
                    axisY.MinValue = scaleMin;


                });
                sourceItem.OnScaleMaxChanged += ((scaleMax) =>
                {
                    var axisY = AxisCollection.Where(x => (int)x.Tag == sourceItem.id).FirstOrDefault();
                    if (axisY == null) return;
                    axisY.MaxValue = scaleMax;

                });
                sourceItem.OnScaleOffsetChanged += ((id, scaleOffset) =>
                {
                    FormatSeries(id, scaleOffset);
                });
                sourceItem.OnFillColorChanged += ((fillColor) =>
                {
                    var line = Series.Where(x => x.Title == sourceItem.name).FirstOrDefault() as GLineSeries;
                    line.Fill = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(fillColor));
                });
                sourceItem.OnLineColorChanged += ((lineColor) =>
                {
                    var line = Series.Where(x => x.Title == sourceItem.name).FirstOrDefault() as GLineSeries;
                    if (line == null) return;
                    line.Stroke = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(lineColor));
                    var axisY = AxisCollection.Where(x => (int)x.Tag == sourceItem.id).FirstOrDefault();
                    axisY.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(lineColor));
                });
                sourceItem.OnActiveChanged += ((active) =>
                {
                    if (active)
                    {
                        var line = Series.Where(x => x.Title == sourceItem.name).FirstOrDefault() as GLineSeries;
                        if (line == null) return;

                        line.Visibility = Visibility.Visible;

                        var axisY = AxisCollection.Where(x => (int)x.Tag == sourceItem.id).FirstOrDefault();
                        axisY.ShowLabels = true;

                    }
                    else
                    {
                        var line = Series.Where(x => x.Title == sourceItem.name).FirstOrDefault() as GLineSeries;
                        if (line == null) return;
                        line.Visibility = Visibility.Hidden;

                        var axisY = AxisCollection.Where(x => (int)x.Tag == sourceItem.id).FirstOrDefault();
                        axisY.ShowLabels = false;
                    }
                });
            }
        }


    }
}
