using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue.Wpf
{
    internal class BaseChartException : Exception
    {
        public BaseChartException(string message)
            : base(message) { }
    }

    internal class ChartOutOfRangeSqlException : BaseChartException
    {
        public ChartOutOfRangeSqlException(Line line)
            : base($"Ingen data i {line.Tag.Name} sql DataTable") { }

        public ChartOutOfRangeSqlException(Line line, string message)
            : base($"{line}: {message}")
        {

        }
    }

    internal class ChartSqlTableNotCreatedException : BaseChartException
    {
        public ChartSqlTableNotCreatedException(Line line)
            : base($"Ej skapat datatable i {line.Tag.Name} sql DataTable") { }
    }

    internal class ChartDateTimeException : BaseChartException
    {
        public ChartDateTimeException(Line line, string message)
          : base($"{line}: {message}")
        {

        }
    }

    internal class ChartNoDataInSqlException : BaseChartException
    {
        public ChartNoDataInSqlException(Line line, string message)
          : base($"{line}: Ingen data hämtad från SQL{Environment.NewLine}{message}")
        { }
        public ChartNoDataInSqlException(Line line, string message, DateTime startDate, DateTime endDate)
         : base($"{line}: Ingen data hämtad mellan {startDate} - {endDate}{Environment.NewLine}{message}")
        { }
    }
}
