using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue.Wpf
{
    public class Tag
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public Color Stroke { get; private set; }
        public Color Fill { get; private set; }



        public Tag()
        {
            CreateRandomColor();
        }

        public override bool Equals(object obj)
        {
            var tag = obj as Tag;

            if (tag == null)
                return false;

            if (tag.Name == Name)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + Name.GetHashCode();
                hash = hash * 23 + Description.GetHashCode();
                hash = hash * 23 + Unit.GetHashCode();
                hash = hash * 23 + Stroke.GetHashCode();
                hash = hash * 23 + Fill.GetHashCode();
                return hash;
            }
        }

        public void UpdateColor()
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

            (Stroke, Fill) = ChangeColors(red: red, green: green, blue: blue);
        }

        private (Color stroke, Color fill) ChangeColors(byte red, byte green, byte blue)
        {
            var brightnessFactor = 0.1f;
            Color strokeColor = ChangeColorBrightness(Color.FromArgb(255, red, green, blue), brightnessFactor);
            Color fillColor = ChangeColorBrightness(Color.FromArgb(30, red, green, blue), brightnessFactor);
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
    }
}
