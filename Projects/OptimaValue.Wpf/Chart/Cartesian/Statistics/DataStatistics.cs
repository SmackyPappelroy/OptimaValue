using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue.Wpf
{
    public enum StatisticFilter
    {
        Inget = 0,
        Max = 1,
    }

    public class DataStatistics
    {
        public StatisticFilter Filter { get; }
        public MyLineSeries Series { get; }

        public double Integral = 0;
        public TimeSpan TimeOverZero = TimeSpan.Zero;
        public double NumberOfTimesOverZero = 0;

        public DataStatistics(StatisticFilter filter, MyLineSeries series)
        {
            Filter = filter;
            Series = series;
            CalculateStatistics();
        }

        private void CalculateStatistics()
        {
            if (Filter == StatisticFilter.Inget)
                return;

            // Integral
            var minDate = Series.ChartValues.AsEnumerable().Select(x => x.DateTime).Min();
            var maxDate = Series.ChartValues.AsEnumerable().Select(x => x.DateTime).Max();

            var antalTimmar = (maxDate - minDate).TotalSeconds / 3600;

            var resultat = Series.ChartValues.AsEnumerable().Select(x => x.Value).Average();

            Integral = resultat * antalTimmar;

            // Beräkna tid över 0
            var valuesOverZero = Series.ChartValues.AsEnumerable().Where(x => x.Value > 0).ToList().Count;
            TimeOverZero = TimeSpan.FromSeconds(valuesOverZero);

            // Beräkna antal gånger över 0

            bool lastValueGreaterThanZero = false;
            foreach (var item in Series.ChartValues)
            {
                if (item.Value > 0 && !lastValueGreaterThanZero)
                {
                    lastValueGreaterThanZero = true;
                    NumberOfTimesOverZero++;
                }

                if (item.Value <= 0 && lastValueGreaterThanZero)
                    lastValueGreaterThanZero = false;
            }
        }
    }
}
