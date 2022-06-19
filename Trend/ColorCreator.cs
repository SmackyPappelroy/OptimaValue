using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OptimaValue
{
    internal class ColorCreator
    {
        public (SolidColorBrush Stroke, SolidColorBrush Fill) CreateRandomColor()
        {
            Random redColor = new Random();
            var red = Convert.ToByte(redColor.Next(0, 255));

            Random greenColor = new Random();
            var green = Convert.ToByte(greenColor.Next(0, 255));

            Random blueColor = new Random();
            var blue = Convert.ToByte(blueColor.Next(0, 255));

            return ChangeColors(red: red, green: green, blue: blue);
        }

        private (SolidColorBrush stroke, SolidColorBrush fill) ChangeColors(byte red, byte green, byte blue)
        {
            var brightnessFactor = 0.3f;
            byte fillColorAlpha = 40;
            //var fillColorAlpha = 30;


            Color strokeColor = ChangeColorBrightness(Color.FromArgb(255, red, green, blue), brightnessFactor);
            Color fillColor = ChangeColorBrightness(Color.FromArgb(fillColorAlpha, red, green, blue), brightnessFactor);
            return (new SolidColorBrush(strokeColor), new SolidColorBrush(fillColor));
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
    }
}
