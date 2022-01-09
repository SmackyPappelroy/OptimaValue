using LiveCharts.Defaults;
using LiveCharts.Geared;
using System;
using System.Collections.Generic;
using System.Data;
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
        public GearedValues<DateTimePoint> Values { get; }

        public double Integral = 0;
        public TimeSpan TimeOverZero = TimeSpan.Zero;
        public double NumberOfTimesOverZero = 0;
        public double StandardDeviation = 0;


        public DataStatistics(StatisticFilter filter, MyLineSeries series)
        {
            Filter = filter;
            Values = series.LineSeries.Values as GearedValues<DateTimePoint>;
            CalculateStatistics();
        }

        private void CalculateStatistics()
        {
            if (Filter == StatisticFilter.Inget)
                return;

            // Integral
            var minDate = Values.Select(x => x.DateTime).Min();
            var maxDate = Values.Select(x => x.DateTime).Max();

            var antalTimmar = (maxDate - minDate).TotalSeconds / 3600;

            var resultat = Values.Select(x => x.Value).Average();

            Integral = resultat * antalTimmar;

            // Beräkna tid över 0
            var valuesOverZero = Values.Where(x => x.Value > 0).ToList().Count;
            TimeOverZero = TimeSpan.FromSeconds(valuesOverZero);

            // Beräkna antal gånger över 0
            bool lastValueGreaterThanZero = false;
            foreach (var item in Values)
            {
                if (item.Value > 0 && !lastValueGreaterThanZero)
                {
                    lastValueGreaterThanZero = true;
                    NumberOfTimesOverZero++;
                }

                if (item.Value <= 0 && lastValueGreaterThanZero)
                    lastValueGreaterThanZero = false;
            }

            var vals = Values.Select(x => x.Value).ToList();
            double avg = vals.Average();
            StandardDeviation = Math.Sqrt(vals.Average(v => Math.Pow(v - avg, 2)));
        }
    }
}
