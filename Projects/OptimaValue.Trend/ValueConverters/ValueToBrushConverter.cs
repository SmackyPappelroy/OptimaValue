using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace OptimaValue.Trend
{
    public class ValueToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object result = null;

            if (value == null) return "N/A";

            if (value.GetType() == typeof(DataGridTextColumn))
            {
                string path = ((Binding)((DataGridTextColumn)value).Binding).Path.Path;
                return path;
            }
            else if (value.GetType() == typeof(string))
            {
                result = ((Color)ColorConverter.ConvertFromString(value.ToString()));
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }
    }
}
