using System;
using System.Collections.Generic;
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
            var dataTable = new DataTable();
            dataTable.Columns.Add("DateTime", typeof(DateTime));
            dataTable.Columns.Add("Value", typeof(double));

            var filteredData = series.ChartValues.Where(x => x.DateTime >= startDate && x.DateTime <= stopDate).ToList();

            foreach (var point in filteredData)
            {
                dataTable.Rows.Add(point.DateTime, point.Value);
            }
            return dataTable;
        }
    }
}
