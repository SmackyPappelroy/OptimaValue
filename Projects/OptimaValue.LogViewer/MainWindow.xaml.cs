using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace OptimaValue.LogViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }

        private async void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var listBoxItem = sender as ListBoxItem;
            if (listBoxItem != null && listBoxItem.IsSelected)
            {
                var logEntry = listBoxItem.Content as LogEntry; // Replace LogEntry with your model type
                if (logEntry != null)
                {
                    Clipboard.SetText(logEntry.ToString()); // Make sure your model has a sensible ToString method
                    listBoxItem.Background = new SolidColorBrush(Colors.LightGray); // Change to the color you want
                }
            }
        }

        private async void ListBoxItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var listBoxItem = sender as ListBoxItem;
            if (listBoxItem != null)
            {
                await Task.Delay(500); // Adjust the delay as needed
                listBoxItem.Background = new SolidColorBrush(Colors.White); // Change back to the original color
            }
        }

    }
}
