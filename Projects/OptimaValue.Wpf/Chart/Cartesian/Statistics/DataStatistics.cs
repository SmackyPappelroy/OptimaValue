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


        public DataStatistics(StatisticFilter filter, MyLineSeriesOld series)
        {
            Filter = filter;
            Values = series.LineSeries.Values as GearedValues<DateTimePoint>;
            CalculateStatistics();
        }

        public DataStatistics(Line line)
        {
            Filter = StatisticFilter.Max;
            Values = line.GLineSeries.Values as GearedValues<DateTimePoint>;
            CalculateStatistics();
        }

        public DataStatistics(StatisticFilter filter, GearedValues<DateTimePoint> values)
        {
            Filter = filter;
            Values = values;
            CalculateStatistics();
        }

        public void CalculateStatistics()
        {
            if (Filter == StatisticFilter.Inget)
                return;

            // Integral
            if (Values == null)
                return;
            if (Values.Count == 0)
                return;
            var filteredValues = Values.Where(x => !double.IsNaN(x.Value)).ToList();
            if (filteredValues.Count == 0)
                return;

            Integral = filteredValues.Sum(x => x.Value);

            var minDate = Values.Select(x => x.DateTime).Min();
            var maxDate = Values.Select(x => x.DateTime).Max();

            var antalTimmar = (maxDate - minDate).TotalSeconds / 3600;

            var resultat = filteredValues.Select(x => x.Value).Average();

            Integral = resultat * antalTimmar;

            // Beräkna tid över 0
            var valuesOverZero = filteredValues.Where(x => x.Value > 0).ToList().Count;
            TimeOverZero = TimeSpan.FromSeconds(valuesOverZero);

            // Beräkna antal gånger över 0
            bool lastValueGreaterThanZero = false;
            foreach (var item in filteredValues)
            {
                if (item.Value > 0 && !lastValueGreaterThanZero)
                {
                    lastValueGreaterThanZero = true;
                    NumberOfTimesOverZero++;
                }

                if (item.Value <= 0 && lastValueGreaterThanZero)
                    lastValueGreaterThanZero = false;
            }

            var vals = filteredValues.Select(x => x.Value).ToList();
            double avg = vals.Average();
            StandardDeviation = Math.Sqrt(vals.Average(v => Math.Pow(v - avg, 2)));
        }
    }
}
