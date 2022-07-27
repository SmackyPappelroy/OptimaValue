using DocumentFormat.OpenXml.ExtendedProperties;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OptimaValue.Trend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindowViewModel viewModel { get; set; }
        public event Action<bool> OnWindowIsBound;
        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainWindowViewModel(this);
            DataContext = viewModel;
            //viewModel.window = this;
            Loaded += ((sender, e) =>
            {
                viewModel.window = this;
                DataGridTags.PreviewKeyDown += ((sender, e) =>
                {
                    if (e.Key == Key.Delete)
                    {
                        if (DataGridTags.SelectedItem is GridItem)
                        {
                            GridItem grid = DataGridTags.SelectedItem as GridItem;
                            foreach (var item in viewModel.Lines)
                            {
                                if (item.GLineSeries.ScalesYAt > 0)
                                    item.GLineSeries.ScalesYAt--;
                            }
                            foreach (var item in viewModel.Series)
                            {
                                if (item.ScalesYAt > 0)
                                    item.ScalesYAt--;
                            }
                            if (viewModel.Lines == null || viewModel.Lines.Count == 0)
                                return;
                            Line line = viewModel.Lines.FirstOrDefault(l => l.Tag.id == grid.id);
                            var axisY = viewModel.AxisCollection.Where(x => (int)x.Tag == grid.id).FirstOrDefault();
                            viewModel.AxisCollection.Remove(axisY);
                            if (viewModel.AxisCollection.Count == 0)
                                viewModel.AxisCollection.Clear();

                            var series = viewModel.Series.Where(x => x.Title == line.Tag.name).FirstOrDefault();
                            viewModel.Series.Remove(series);
                            StaticClass.RemoveLine(line);
                            if (line == null)
                                return;
                            viewModel.Lines.Remove(line);
                            viewModel.FormatSeries();
                        }
                    }
                    OnWindowIsBound?.Invoke(true);
                });

            });

        }



        protected override void OnSourceInitialized(EventArgs e)
        {
#if DEBUG
            var hwndSource = PresentationSource.FromVisual(this) as HwndSource;

            if (hwndSource != null)
                hwndSource.CompositionTarget.RenderMode = RenderMode.SoftwareOnly;
#endif
            base.OnSourceInitialized(e);
        }


    }
}
