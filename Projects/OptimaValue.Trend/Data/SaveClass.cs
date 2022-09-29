using LiveCharts.Geared;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace OptimaValue.Trend
{
    public class SaveClass
    {
        public DateTime StartDateFetchedData { get; set; }
        public DateTime EndDateFetchedData { get; set; }
        public DateTime StartDateDisplayedData { get; set; }
        public DateTime EndDateDisplayedData { get; set; }
        public ObservableCollection<GridItem> SelectedItems { get; set; }

        [JsonIgnore]
        public string SerializedJsonText { get; private set; }

        public bool Serialize(BindingList<Line> lines)
        {
            SerializedJsonText = JsonSerializer.Serialize(this);
            return true;
        }

        public static void SaveData(MainWindowViewModel viewModel, string fileName)
        {
            SaveClass save = new SaveClass()
            {
                StartDateFetchedData = viewModel.StartDate,
                EndDateFetchedData = viewModel.EndDate,
                StartDateDisplayedData = viewModel.MinDateSeries,
                EndDateDisplayedData = viewModel.MaxDateSeries,
                SelectedItems = viewModel.SelectedItems
                //Series = Series,
            };
            save.Serialize(viewModel.Lines);
            StaticClass.LastSaveFile = fileName;
            UpdateWindowTitle(fileName, viewModel);
            // Save to file
            File.WriteAllText(fileName, save.SerializedJsonText);
        }

        public static async Task LoadData(MainWindowViewModel viewModel, string fileName)
        {
            if (fileName == "")
            {
                UpdateWindowTitle(fileName, viewModel);
                return;
            }
            try
            {
                var text = File.ReadAllText(fileName);

                var trend = JsonSerializer.Deserialize<SaveClass>(text);

                //Series = trend.Series;
                viewModel.InputStartDate = trend.StartDateFetchedData;
                viewModel.InputStartTime = new TimeOnly(trend.StartDateFetchedData.TimeOfDay.Ticks);
                viewModel.InputEndDate = trend.EndDateFetchedData;
                viewModel.InputEndTime = new TimeOnly(trend.EndDateFetchedData.TimeOfDay.Ticks);
                viewModel.MinDateSeries = trend.StartDateDisplayedData;
                viewModel.MaxDateSeries = trend.EndDateDisplayedData;

                viewModel.Lines = new();
                viewModel.Lines.Clear();
                viewModel.AxisCollection.Clear();
                viewModel.Series = new();
                viewModel.SelectedItems = trend.SelectedItems;
                int index = 0;
                foreach (var item in trend.SelectedItems)
                {
                    var line = new Line(item.id, viewModel, item.LineColor, item.FillColor);
                    await line.SetupLine(viewModel.StartDate, viewModel.EndDate, index);
                    viewModel.Series.Add(line.GLineSeries);
                    viewModel.Lines.Add(line);
                    if (index == 0)
                    {
                        viewModel.AxisCollection.Clear();
                        if (StaticClass.Lines != null)
                        {
                            StaticClass.Lines.Clear();
                            StaticClass.Lines = new();
                        }
                        else
                            StaticClass.Lines = new();
                    }
                    StaticClass.Lines.Add(line);
                    viewModel.AxisCollection.Add(line.AxisY);



                    viewModel.FormatSeries();

                    HookIntoEvents(item, viewModel);
                    index++;
                }
                UpdateWindowTitle(fileName, viewModel);
                StaticClass.LastSaveFile = fileName;
            }

            catch (Exception ex)
            {
                viewModel.RaiseMessage(ex.Message);
            }
        }

        private static void HookIntoEvents(GridItem item, MainWindowViewModel vm)
        {
            item.OnSmoothingChanged += ((smoothing) =>
            {
                var line = vm.Series.Where(x => x.Title == item.name).FirstOrDefault() as GLineSeries;
                if (line == null) return;

                line.LineSmoothness = smoothing;
            });
            item.OnScaleMinChanged += ((scaleMin) =>
            {
                var axisY = vm.AxisCollection.Where(x => (int)x.Tag == item.id).FirstOrDefault();
                if (axisY == null) return;
                axisY.MinValue = scaleMin;


            });
            item.OnScaleMaxChanged += ((scaleMax) =>
            {
                var axisY = vm.AxisCollection.Where(x => (int)x.Tag == item.id).FirstOrDefault();
                if (axisY == null) return;
                axisY.MaxValue = scaleMax;

            });
            item.OnScaleOffsetChanged += ((id, scaleOffset) =>
            {
                vm.FormatSeries(id, scaleOffset);
            });
            item.OnFillColorChanged += ((fillColor) =>
            {
                var line = vm.Series.Where(x => x.Title == item.name).FirstOrDefault() as GLineSeries;
                line.Fill = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(fillColor));
            });
            item.OnLineColorChanged += ((lineColor) =>
            {
                var line = vm.Series.Where(x => x.Title == item.name).FirstOrDefault() as GLineSeries;
                if (line == null) return;
                line.Stroke = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(lineColor));
                var axisY = vm.AxisCollection.Where(x => (int)x.Tag == item.id).FirstOrDefault();
                axisY.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(lineColor));
            });
            item.OnActiveChanged += ((active) =>
            {
                if (active)
                {
                    var line = vm.Series.Where(x => x.Title == item.name).FirstOrDefault() as GLineSeries;
                    if (line == null) return;

                    line.Visibility = Visibility.Visible;

                    var axisY = vm.AxisCollection.Where(x => (int)x.Tag == item.id).FirstOrDefault();
                    axisY.ShowLabels = true;

                }
                else
                {
                    var line = vm.Series.Where(x => x.Title == item.name).FirstOrDefault() as GLineSeries;
                    if (line == null) return;
                    line.Visibility = Visibility.Hidden;

                    var axisY = vm.AxisCollection.Where(x => (int)x.Tag == item.id).FirstOrDefault();
                    axisY.ShowLabels = false;
                }
            });
        }

        private static void UpdateWindowTitle(string fileName, MainWindowViewModel viewModel)
        {
            if (fileName == "")
            {
                viewModel.WindowTitle = "OptimaValueTrend";
            }
            else
            {
                //viewModel.WindowTitle = "OptimaValueTrend - [" + (fileName.Substring(fileName.LastIndexOf("\\") + 1)).Split('.').FirstOrDefault() + "]";
                viewModel.WindowTitle = "OptimaValueTrend - [" + fileName + "]";
            }
        }
    }
}
