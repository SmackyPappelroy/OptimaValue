using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace OptimaValue.Trend
{
    public class GridItem : INotifyPropertyChanged
    {
        public event Action<double> OnSmoothingChanged;
        public event Action<int> OnScaleMinChanged;
        public event Action<int> OnScaleMaxChanged;
        public event Action<int, int> OnScaleOffsetChanged;
        public event Action<string> OnLineColorChanged;
        public event Action<string> OnFillColorChanged;
        public event Action<bool> OnActiveChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool mActive;
        public bool active
        {
            get => mActive;
            set
            {
                if (value != mActive)
                {
                    OnActiveChanged?.Invoke(value);
                    mActive = value;
                }
            }
        }
        public int id { get; set; }
        [ColumnName("Namn")]
        public string name { get; set; }
        [ColumnName("Beskrivning")]
        public string? description { get; set; }

        private int mScaleMin;
        [ColumnName("Minimum")]
        public int scaleMin
        {
            get => mScaleMin;
            set
            {
                if (value != mScaleMin)
                {

                    OnScaleMinChanged?.Invoke(value);
                    mScaleMin = value;
                }
            }
        }

        private int mScaleMax;
        [ColumnName("Maximum")]
        public int scaleMax
        {
            get => mScaleMax;
            set
            {
                if (value != mScaleMax)
                {
                    OnScaleMaxChanged?.Invoke(value);
                    mScaleMax = value;
                }
            }
        }

        private int mScaleOffset;
        [ColumnName("TrendOffset")]
        public int scaleOffset
        {
            get => mScaleOffset;
            set
            {
                if (value != mScaleOffset)
                {
                    OnScaleOffsetChanged?.Invoke(id, value);
                    mScaleOffset = value;
                }
            }
        }
        private double mSmoothing;
        public double smoothing
        {
            get => mSmoothing;
            set
            {
                if (value != mSmoothing)
                {
                    OnSmoothingChanged?.Invoke(value);
                    mSmoothing = value;
                }
            }
        }

        private string lineColor;
        [ColumnName("LinjeFärg")]
        public string LineColor
        {
            get => lineColor;
            set
            {
                if (value != lineColor)
                {
                    OnLineColorChanged?.Invoke(value);
                    lineColor = value;
                }
            }
        }
        private string fillColor;
        [ColumnName("FyllFärg")]
        public string FillColor
        {
            get => fillColor;
            set
            {
                if (value != fillColor)
                {
                    OnFillColorChanged?.Invoke(value);
                    fillColor = value;
                }
            }
        }

        public int SeriesIndex { get; set; }

        public GridItem()
        {
            CreateRandomColor();
        }

        private void CreateRandomColor()
        {
            Random redColor = new Random();
            var red = Convert.ToByte(redColor.Next(0, 255));

            Random greenColor = new Random();
            var green = Convert.ToByte(greenColor.Next(0, 255));

            Random blueColor = new Random();
            var blue = Convert.ToByte(blueColor.Next(0, 255));

            (var LineColortemp, var FillColortemp) = ChangeColors(red: red, green: green, blue: blue);

            LineColor = LineColortemp.ToString();
            FillColor = FillColortemp.ToString();
        }

        private (Color stroke, Color fill) ChangeColors(byte red, byte green, byte blue)
        {
            var brightnessFactor = 0.3f;
            byte fillColorAlpha = 40;
            //var fillColorAlpha = 30;


            Color strokeColor = ChangeColorBrightness(Color.FromArgb(255, red, green, blue), brightnessFactor);
            Color fillColor = ChangeColorBrightness(Color.FromArgb(fillColorAlpha, red, green, blue), brightnessFactor);
            return (strokeColor, fillColor);
        }

        /// <summary>
        /// Creates color with corrected brightness.
        /// </summary>
        /// <param name="color">Color to correct.</param>
        /// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1. 
        /// Negative values produce darker colors.</param>
        /// <returns>
        /// Corrected <see cref="Color"/> structure.
        /// </returns>
        private Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }

        public SolidColorBrush StringToSolidColorBrush(string colorString) => new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorString));
    }

    public class ColumnNameAttribute : Attribute
    {
        public ColumnNameAttribute(string Name) { this.Name = Name; }
        public string Name { get; set; }
    }


}
