using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Geared;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OptimaValue.Wpf
{
    public class MyLineSeriesOld
    {
        public Tag Tag;

        #region TagControl properties
        public TagControl TagControl;

        private DateTimePoint cursorValues = new();
        public DateTimePoint CursorValues
        {
            get => cursorValues;
            set
            {
                cursorValues = value;
                var doubleValue = Convert.ToDouble(value.Value);
                if (doubleValue % 1 == 0)
                    TagControl.ActualValue = value.Value.ToString("0");
                else
                    TagControl.ActualValue = value.Value.ToString("0.000");
            }
        }

        private string minValue;
        public string MinValue
        {
            get => minValue;
            set
            {
                minValue = value;
                TagControl.MinValue = value;
            }
        }

        private string maxValue;
        public string MaxValue
        {
            get => maxValue;
            set
            {
                maxValue = value;
                TagControl.MaxValue = value;
            }
        }

        private string averageValue;
        public string AverageValue
        {
            get => averageValue;
            set
            {
                averageValue = value;
                TagControl.AverageValue = value;
            }
        }

        private string integral;
        public string Integral
        {
            get => integral;
            set
            {
                integral = value;
                TagControl.Integral = value;
            }
        }

        private string integralPerTimme;
        public string IntegralPerTimme
        {
            get => integralPerTimme;
            set
            {
                integralPerTimme = value;
                TagControl.IntegralPerTimme = value;
            }
        }

        private string overZero;
        public string OverZero
        {
            get => overZero;
            set
            {
                overZero = value;
                TagControl.OverZero = value;
            }
        }

        private TimeSpan overZeroTime;
        public TimeSpan OverZeroTime
        {
            get => overZeroTime;
            set
            {
                overZeroTime = value;
                TagControl.OverZeroTime = value.ToString();
            }
        }

        private string deviation;
        public string Deviation
        {
            get => deviation;
            set
            {
                deviation = value;
                TagControl.Deviation = value;
            }
        }

        #endregion

        public GearedValues<DateTimePoint> ChartValues;

        public List<DateTimePoint> FilteredData = new();

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
        public MyLineSeriesOld(Tag tag)
        {
            Tag = tag;


            Color Stroke = tag.Stroke;
            Color Fill = tag.Fill;

            LineSeries = new()
            {
                AreaLimit = 0,
                PointGeometry = DefaultGeometries.Circle,
                PointGeometrySize = 10,
                StrokeThickness = 2,

                Foreground = new SolidColorBrush(Color.FromArgb(107, 48, 48, 48)),
                Stroke = new SolidColorBrush(Stroke),
                Fill = new SolidColorBrush(Fill),
            };
            TagControl = new();
            TagControl.TagName = Tag.Name;
            TagControl.TagColor = UpdateTagControlEllipseColor();
            TagControl.Description = Tag.Description;
            TagControl.TagUnit = Tag.Unit;

            ChartData.OnChartChanged += ChartData_OnChartChanged;
        }

        private LinearGradientBrush UpdateTagControlEllipseColor()
        {
            var brush = new LinearGradientBrush();
            brush.GradientStops.Add(new GradientStop(Tag.Stroke, 0));
            brush.GradientStops.Add(new GradientStop(Tag.Fill, 1));
            return brush;
        }

        #endregion

        public void UpdateLineColor()
        {
            Tag.UpdateColor();
            LineSeries.Stroke = new SolidColorBrush(Tag.Stroke);
            TagControl.TagColor = UpdateTagControlEllipseColor();
            LineSeries.Fill = new SolidColorBrush(Tag.Fill);
        }


        private void ChartData_OnChartChanged(bool obj)
        {
            GetChartValues();
        }

        public bool GetChartValues(bool fillEmptyValues = false)
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

            var result = ChartData.AddSeriesValues(Tag.Name, null, fillEmptyValues);

            if (result == null)
                return false;

            ChartValues.AddRange(result);
            LineSeries.Values = new GearedValues<DateTimePoint>(ChartValues.AsGearedValues().WithQuality(Quality.Highest));

            LineSeries.Name = Tag.Name.Replace(" ", "_");

            if (LineSeries.Values.Count == 0)
                return false;


            MinValueY = ChartValues.Where(x => !double.IsNaN(x.Value)).Min(x => x.Value);
            MaxValueY = ChartValues.Where(x => !double.IsNaN(x.Value)).Max(x => x.Value);
            AvgValueY = ChartValues.Where(x => !double.IsNaN(x.Value)).Average(x => x.Value);

            MinValueX = ChartValues.Min(x => x.DateTime.Ticks);
            MaxValueX = ChartValues.Max(x => x.DateTime.Ticks);
            return true;

        }


    }
}
