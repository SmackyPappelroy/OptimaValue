using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Geared;
using LiveCharts.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OptimaValue.Wpf
{
    public class MyLineSeries
    {
        public Tag Tag;

        public GearedValues<DateTimePoint> ChartValues;

        public List<DateTimePoint> NewValuesToAdd { get; set; }

        public DateTime NewMaxLogTime => NewValuesToAdd.Count > 0 ? NewValuesToAdd.Max(x => x.DateTime) : DateTime.MinValue;

        private GLineSeries lineSeries;
        public GLineSeries LineSeries
        {
            get => lineSeries;
            set { lineSeries = value; }
        }

        public PeriodUnits Period;
        public double MinValueY;
        public double MaxValueY;
        public double AvgValueY;

        public double MinValueX;
        public double MaxValueX;


        #region Constructor
        public MyLineSeries(Tag tag)
        {
            Tag = tag;

            Color Stroke = tag.Stroke;
            Color Fill = tag.Fill;

            LineSeries = new()
            {
                AreaLimit = 0,
                PointGeometry = null,
                StrokeThickness = 1,

                Foreground = new SolidColorBrush(Color.FromArgb(107, 48, 48, 48)),
                Stroke = new SolidColorBrush(Stroke),
                Fill = new SolidColorBrush(Fill),
            };

            ChartData.OnChartChanged += ChartData_OnChartChanged;
        }

        #endregion

        public void UpdateLineColor()
        {
            Tag.UpdateColor();
            LineSeries.Stroke = new SolidColorBrush(Tag.Stroke);
            LineSeries.Fill = new SolidColorBrush(Tag.Fill);
        }

        private void ChartData_OnChartChanged(bool obj)
        {
            GetChartValues();
        }

        public bool GetChartValues()
        {
            LineSeries.Title = Tag.Name;

            if (LineSeries.Values == null)
            {
                LineSeries.Values = new GearedValues<DateTimePoint>().WithQuality(Quality.Highest);
            }
            if (ChartValues == null)
                ChartValues = new();

            LineSeries.Values.Clear();
            ChartValues.Clear();

            ChartValues.AddRange(ChartData.AddSeriesValues(Tag.Name));
            LineSeries.Values = new GearedValues<DateTimePoint>(ChartValues.AsGearedValues().WithQuality(Quality.Highest));

            if (LineSeries.Values.Count == 0)
                return false;

            MinValueY = ChartValues.Min(x => x.Value);
            MaxValueY = ChartValues.Max(x => x.Value);
            AvgValueY = ChartValues.Average(x => x.Value);

            MinValueX = ChartValues.Min(x => x.DateTime.Ticks);
            MaxValueX = ChartValues.Max(x => x.DateTime.Ticks);
            return true;

        }


    }
}
