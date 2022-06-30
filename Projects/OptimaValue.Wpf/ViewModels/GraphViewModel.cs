using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Definitions.Series;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace OptimaValue.Wpf
{
    public class GraphViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly TimeSpan StatusMessageTimeout = TimeSpan.FromSeconds(5);
        private event Action<string> OnStatusMessage;

        private DispatcherTimer statusMessageTimer;
        private List<Tag> AvailableTags;
        private GraphWindow window;
        private bool isLoaded;
        public GraphViewModel()
        {
            // Reference to singleton instance of graphwindow
            window = Master.GetService<GraphWindow>();

            window.OnLoaded += (async (obj) =>
            {
                // Hook into new statusmessage raised
                OnStatusMessage += GraphViewModel_OnStatusMessage;
                // When the window is first loaded

                AvailableTags = new();
                AvailableTags = await SqlHelper.GetDistinctTags();
                StaticClass.AvailableTags = AvailableTags;
                // Map distinct tags to combobox
                Dispatcher.CurrentDispatcher?.Invoke(() =>
                {
                    MapTagsToComboBox();
                });
                // Clear status message after 5 seconds
                statusMessageTimer = new DispatcherTimer();
                statusMessageTimer.Interval = StatusMessageTimeout;
                statusMessageTimer.Tick += ((sender, e) =>
                {
                    statusMessageTimer.Stop();
                    Dispatcher.CurrentDispatcher?.Invoke(() =>
                    {
                        StatusText = string.Empty;
                    });
                });
                // If window gains focus
                window.Activated += (sender, e) =>
                {
                    if (window.myPopupStack.Visibility != Visibility.Visible)
                    {
                        window.myPopupStack.Visibility = Visibility.Visible;
                    }
                };
                // If window looses focus
                window.Deactivated += (sender, e) =>
                {
                    if (window.myPopupStack.Visibility != Visibility.Collapsed)
                    {
                        window.myPopupStack.Visibility = Visibility.Collapsed;
                    }
                };
                // Om fönstret windowstate ändras
                window.StateChanged += (sender, e) =>
                {
                    if (window.WindowState == WindowState.Minimized)
                    {
                        window.myPopupStack.Visibility = Visibility.Collapsed;
                    }
                    else if (window.IsActive)
                    {
                        window.myPopupStack.Visibility = Visibility.Visible;
                    }
                };

                Window w = Window.GetWindow(window.ControlBorder);
                // Move popup with window
                if (null != w)
                {
                    w.LocationChanged += delegate (object sender2, EventArgs args)
                    {
                        var offset = window.myPopup.HorizontalOffset;
                        // "bump" the offset to cause the popup to reposition itself
                        //   on its own
                        window.myPopup.HorizontalOffset = offset + 1;
                        window.myPopup.HorizontalOffset = offset;
                    };
                    // Also handle the window being resized (so the popup's position stays
                    //  relative to its target element if the target element moves upon 
                    //  window resize)
                    w.SizeChanged += delegate (object sender3, SizeChangedEventArgs e2)
                    {
                        var offset = window.myPopup.HorizontalOffset;
                        window.myPopup.HorizontalOffset = offset + 1;
                        window.myPopup.HorizontalOffset = offset;
                    };
                }
                // Chart has updated
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
                    }

                });
                window.MinSlider.ValueChanged += ((sender, e) =>
                {
                    if (e.NewValue > MaxValueGraph)
                    {
                        MinSliderValue = MaxValueGraph;
                    }
                    else if (e.NewValue < MinValueGraph)
                    {
                        MinSliderValue = MinValueGraph;
                    }
                    else
                    {
                        MinSliderValue = (double)e.NewValue;
                    }
                    if (e.NewValue > window.MaxSlider.Value)
                    {
                        MinSliderValue = window.MaxSlider.Value;
                    }
                    window.MyChart.AxisY[0].MinValue = MinSliderValue;
                });
                window.MaxSlider.ValueChanged += ((sender, e) =>
                {
                    if (e.NewValue < MinValueGraph)
                    {
                        MaxSliderValue = MinValueGraph;
                    }
                    else if (e.NewValue > MaxValueGraph)
                    {
                        MaxSliderValue = MaxValueGraph;
                    }
                    else
                    {
                        MaxSliderValue = (double)e.NewValue;
                    }
                    if (e.NewValue < window.MinSlider.Value)
                    {
                        MaxSliderValue = window.MinSlider.Value;
                    }
                    window.MyChart.AxisY[0].MaxValue = MaxSliderValue;

                });
                // Window is loaded
                isLoaded = true;
            });


        }



        private void MapTagsToComboBox()
        {
            // Run on UI-thread
            Dispatcher.CurrentDispatcher?.Invoke(() =>
            {
                window.comboTag.ItemsSource = AvailableTags;
                window.comboTag.DisplayMemberPath = nameof(Tag.Name);
                window.comboTag.SelectedValuePath = nameof(Tag.TagId);
            });

        }

        private void GraphViewModel_OnStatusMessage(string message)
        {
            StatusText = message;
            statusMessageTimer.Start();
        }
        public BindingList<Line> Lines { get; set; }
        public DateTime MinDateGraph { get; set; }
        public DateTime MaxDateGraph { get; set; }
        public DateTime MinDateSeries { get; set; }
        public DateTime MaxDateSeries { get; set; }

        public DateTime InputStartDate { get; set; } = DateTime.Now.Date;
        public DateTime InputEndDate { get; set; } = DateTime.Now.Date;
        public TimeOnly InputStartTime { get; set; } = new TimeOnly(0, 0, 1);
        public TimeOnly InputEndTime { get; set; } = new TimeOnly(23, 59, 59);
        public double MinValueGraph { get; set; }
        public double MaxValueGraph { get; set; }
        public double MinSliderValue { get; set; }
        public double MaxSliderValue { get; set; }
        public string StatusText { get; set; }
        public SeriesCollection Series { get; set; }
        public ZoomingOptions Zoom { get; set; } = ZoomingOptions.X;

        public DateTime StartDate => new DateTime(InputStartDate.Ticks + InputStartTime.Ticks);

        public DateTime EndDate => new DateTime(InputEndDate.Ticks + InputEndTime.Ticks);

        private ICommand addLineCommand;
        public ICommand AddLineCommand
        {
            get
            {
                if (addLineCommand == null)
                {
                    addLineCommand = new RelayCommand(parameter => AddLine((int)parameter));
                }
                return addLineCommand;
            }
        }

        private ICommand refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (refreshCommand == null)
                {
                    refreshCommand = new RelayCommand(_ => RefreshLines());
                }
                return refreshCommand;
            }
        }

        public async void RefreshLines()
        {
            if (Lines == null || Lines.Count == 0)
                return;

            foreach (var line in Lines)
            {
                try
                {
                    await line.GetSqlData(MinDateSeries.Subtract(line.ExtraTimeInSql), MaxDateSeries.Add(line.ExtraTimeInSql), 0);
                    await line.GetValuesFromTable(MinDateSeries, MaxDateSeries);
                    await line.GetMinMaxAvg();
                    await line.UpdateDataStatistics();

                    var serie = Series.Where(x => x.Title == line.Tag.Name).FirstOrDefault();
                    serie = line.GLineSeries;
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
            FormatSeries();
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

            if (Lines.Any(l => l.Tag.TagId == tagId))
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

            Line line = new(tagId, TimeSpan.FromHours(1));
            StaticClass.AddLine(line);

            try
            {
                await line.SetupLine(StartDate, EndDate);
                if (Series == null)
                {
                    Series = new();
                }
                if (line.GLineSeries != null)
                {
                    Series.Add(line.GLineSeries);
                    Lines.Add(line);
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



        private void FormatSeries()
        {
            if (Lines.Count == 0)
            {
                MinValueGraph = 0;
                MaxValueGraph = 0;
                MinDateGraph = MaxDateGraph = MinDateSeries = MaxDateSeries = DateTime.MinValue;
                return;
            }
            window.MyChart.AxisY.First().MaxValue = Lines.Select(x => x.MaxValue).Max();
            window.MyChart.AxisY.First().MinValue = Lines.Select(x => x.MinValue).Min();
            window.MyChart.AxisX.First().Separator = DefaultAxes.CleanSeparator;
            window.MyChart.AxisX.First().LabelFormatter = Lines.Select(x => x.FormatterX).First();
            window.MyChart.AxisY.First().LabelFormatter = Lines.Select(x => x.FormatterY).First();
            window.MyChart.AxisY.First().Separator = DefaultAxes.CleanSeparator;

            MinDateGraph = Lines.Select(x => x.MinDate).Min();
            MaxDateGraph = Lines.Select(x => x.MaxDate).Max();

            MinValueGraph = Lines.Select(x => x.MinValue).Min();
            MaxValueGraph = Lines.Select(x => x.MaxValue).Max();
            window.MinSlider.Value = MinValueGraph;
            window.MaxSlider.Value = MaxValueGraph;
            window.MaxSlider.TickFrequency = MaxValueGraph / 100;
            window.MinSlider.TickFrequency = MaxValueGraph / 100;

        }

        private ICommand removeLineCommand;
        public ICommand RemoveLineCommand
        {
            get
            {
                if (removeLineCommand == null)
                {
                    removeLineCommand = new RelayCommand(parameter => RemoveLine((int)parameter));
                }
                return removeLineCommand;
            }
        }

        /// <summary>
        /// Remove a lineseries from the list
        /// </summary>
        /// <param name="tagId"></param>
        public void RemoveLine(int tagId)
        {
            if (tagId < 0)
            {
                RaiseMessage("Välj en tag");
                return;
            }
            if (Lines == null || Lines.Count == 0)
                return;
            Line line = Lines.FirstOrDefault(l => l.Tag.TagId == tagId);

            var series = Series.Where(x => x.Title == line.Tag.Name).FirstOrDefault();
            Series.Remove(series);
            StaticClass.RemoveLine(line);
            if (line == null)
                return;
            Lines.Remove(line);
            FormatSeries();
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
    }
}
