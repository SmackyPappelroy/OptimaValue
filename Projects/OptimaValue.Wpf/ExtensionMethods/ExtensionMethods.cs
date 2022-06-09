using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue.Wpf
{
    public static class ExtensionMethods
    {
        public static DataTable ToDataTable(this MyLineSeries series, DateTime startDate, DateTime stopDate)
        {
            var dt = new DataTable();
            dt.Columns.Add("TagNamn", typeof(string));
            dt.Columns.Add("DateTime", typeof(DateTime));
            dt.Columns.Add("Value", typeof(double));

            var filteredData = series.ChartValues.Where(x => x.DateTime >= startDate && x.DateTime <= stopDate).ToList();

            foreach (var point in filteredData)
            {
                dt.Rows.Add(series.Tag.Name, point.DateTime, point.Value);
            }
            return dt;
        }


        public static XLWorkbook ToWorkSheet(this List<MyLineSeries> series, DateTime startDate, DateTime stopDate)
        {
            var dt = new DataTable();
            XLWorkbook book = new();

            dt.Columns.Add("DateTime", typeof(DateTime));

            series[0].FilteredData = series[0].ChartValues.Where(x => x.DateTime >= startDate && x.DateTime <= stopDate).ToList();
            var nrOfItemsFirst = series[0].FilteredData.Count;

            foreach (var item in series)
            {
                dt.Columns.Add(item.Tag.Name, typeof(double));
            }

            foreach (var item in series)
            {
                item.FilteredData = item.ChartValues.Where(x => x.DateTime >= startDate && x.DateTime <= stopDate).ToList();
                if (item.FilteredData.Count != nrOfItemsFirst)
                {
                    throw new Exception("All series must have the same number of items");
                }
            }


            for (int i = 0; i < nrOfItemsFirst - 1; i++)
            {
                List<object> list = new();
                list.Add(series[0].FilteredData[i].DateTime);
                for (int j = 0; j < series.Count; j++)
                {
                    list.Add(series[j].FilteredData[i].Value);
                }
                dt.Rows.Add(list.ToArray());
            }
            IXLWorksheet sheet = book.Worksheets.Add(dt, "TrendData");
            sheet.Column(1).Style.NumberFormat.Format = "yyyy-mm-dd hh:mm:ss";
            sheet.Columns().AdjustToContents();
            return book;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return new ObservableCollection<T>(source);
        }
    }
}
