using ClosedXML.Excel;
using LiveCharts.Geared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue.Wpf
{
    public static class ExtensionMethods
    {
        //public static DataTable ToDataTable(this MyLineSeries series, DateTime startDate, DateTime stopDate)
        //{
        //    var dt = new DataTable();
        //    dt.Columns.Add("TagNamn", typeof(string));
        //    dt.Columns.Add("DateTime", typeof(DateTime));
        //    dt.Columns.Add("Value", typeof(double));

        //    var filteredData = series.ChartValues.Where(x => x.DateTime >= startDate && x.DateTime <= stopDate).ToList();

        //    foreach (var point in filteredData)
        //    {
        //        dt.Rows.Add(series.Tag.Name, point.DateTime, point.Value);
        //    }
        //    return dt;
        //}


        //public static XLWorkbook ToWorkSheet(this List<MyLineSeries> series, DateTime startDate, DateTime stopDate)
        //{
        //    var dt = new DataTable();
        //    XLWorkbook book = new();

        //    dt.Columns.Add("DateTime", typeof(DateTime));

        //    series[0].FilteredData = series[0].ChartValues.Where(x => x.DateTime >= startDate && x.DateTime <= stopDate).ToList();
        //    var nrOfItemsFirst = series[0].FilteredData.Count;

        //    foreach (var item in series)
        //    {
        //        dt.Columns.Add(item.Tag.Name, typeof(double));
        //    }

        //    foreach (var item in series)
        //    {
        //        item.FilteredData = item.ChartValues.Where(x => x.DateTime >= startDate && x.DateTime <= stopDate).ToList();
        //        if (item.FilteredData.Count != nrOfItemsFirst)
        //        {
        //            throw new Exception("All series must have the same number of items");
        //        }
        //    }


        //    for (int i = 0; i < nrOfItemsFirst - 1; i++)
        //    {
        //        List<object> list = new();
        //        list.Add(series[0].FilteredData[i].DateTime);
        //        for (int j = 0; j < series.Count; j++)
        //        {
        //            list.Add(series[j].FilteredData[i].Value);
        //        }
        //        dt.Rows.Add(list.ToArray());
        //    }

        //    IXLWorksheet sheet = book.Worksheets.Add(dt, "TrendData");
        //    sheet.Row(1).InsertRowsAbove(1);


        //    var alphabets = new Dictionary<char, int>()
        //                    {
        //                        {'A', 0},{'B', 1},{'C', 2},{'D', 3},
        //                        {'E', 4},{'F', 5}, {'G', 6},{'H', 7},
        //                        {'I', 8},{'J', 9},{'K', 10}, {'L', 11},
        //                        {'M', 12},{'N', 13},{'P', 14},{'Q', 15},
        //                        {'R', 16},{'S', 17},{'T', 18},{'U', 19},
        //                        {'V', 20},{'X', 21},{'Y', 22},{'Z', 23}
        //                    };

        //    int index = 1;

        //    var lastRowUsed = sheet.LastRowUsed().RowNumber();
        //    sheet.SheetView.FreezeRows(2);

        //    foreach (var item in series)
        //    {
        //        if (series.Count > alphabets.Count)
        //            break;
        //        var letter = alphabets.Where(x => x.Value == index).FirstOrDefault().Key;
        //        sheet.Range($"{letter}1:{letter}{lastRowUsed}").RangeUsed().AddConditionalFormat().DataBar(XLColor.Red)
        //           .LowestValue()
        //           .HighestValue();

        //        sheet.SparklineGroups.Add($"{letter}1", $"{letter}1:{letter}{lastRowUsed}")
        //        .SetStyle(XLSparklineTheme.Colorful1)
        //        .SetLineWeight(1)
        //        .HorizontalAxis
        //        .SetVisible(true)
        //        .SetColor(XLColor.Red);

        //        index++;

        //    }



        //    sheet.Column(1).Style.NumberFormat.Format = "yyyy-mm-dd hh:mm:ss";
        //    sheet.Columns().AdjustToContents();
        //    return book;
        //}

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return new ObservableCollection<T>(source);
        }

        public static IEnumerable<T> FastReverse<T>(this IList<T> items)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                yield return items[i];
            }
        }

        public static DataTable ReverseOrder(this DataTable dTable)
        {
            return dTable.AsEnumerable().Reverse().CopyToDataTable();
        }


    }


}
