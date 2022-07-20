using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
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

namespace OptimaValue.Wpf.UserControls
{
    /// <summary>
    /// Interaction logic for TagListControl.xaml
    /// </summary>
    public partial class TagListControl : UserControl, IDropTarget
    {
        private TagListControlViewModel viewModel;

        public TagListControlViewModel Model { get; set; } = new();
        public TagListControl()
        {
            InitializeComponent();
            viewModel = Master.GetService<TagListControlViewModel>();
            DataContext = viewModel;
            DataGrid.ItemsSource = viewModel.GridItems;
            Loaded += TagListControl_Loaded;
            DataGrid.AutoGeneratingColumn += dgPrimaryGrid_AutoGeneratingColumn;
        }

        private void TagListControl_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid.Columns[0].Visibility = Visibility.Collapsed;
            DataGrid.IsReadOnly = true;
            foreach (var item in DataGrid.Columns)
            {
                item.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }

        }

        private void dgPrimaryGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var desc = e.PropertyDescriptor as PropertyDescriptor;
            var att = desc.Attributes[typeof(ColumnNameAttribute)] as ColumnNameAttribute;
            if (att != null)
            {
                e.Column.Header = att.Name;
            }
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }
    }
}
