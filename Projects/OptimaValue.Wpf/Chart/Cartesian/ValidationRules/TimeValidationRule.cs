using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OptimaValue.Wpf
{
    public class TimeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var input = (string)value;

                if (input.Count() < 5)
                    return new ValidationResult(false, @"Skriv in en sträng i formatet ##:##.##");

                var hourDigits = new string(input.Substring(0, 2).Where(char.IsDigit).ToArray());
                var thirdChar = new string(input.Substring(2, 1));
                var minuteDigits = new string(input.Substring(3, 2).Where(char.IsDigit).ToArray());
                if (input.Count() > 5)
                {
                    var fifthChar = new string(input.Substring(5, 1));
                    var secondDigits = new string(input.Substring(6, 2).Where(char.IsDigit).ToArray());

                    if (thirdChar != ":" || fifthChar != ":")
                        return new ValidationResult(false, "Ej giltigt format");
                    var secondConversion = int.TryParse(secondDigits, out var tempSeconds);


                    if (secondDigits.Count() != 2 || !secondConversion)
                        return new ValidationResult(false, "Sekund-värdet ej giltigt");

                    if (tempSeconds > 59)
                        return new ValidationResult(false, "Sekund-värdet ej giltigt");
                }


                var hourConversion = int.TryParse(hourDigits, out var tempHour);
                var minuteConversion = int.TryParse(minuteDigits, out var tempMinute);


                if (hourDigits.Count() != 2 || !hourConversion)
                    return new ValidationResult(false, "Tim-värdet ej giltigt");

                if (minuteDigits.Count() != 2 || !minuteConversion)
                    return new ValidationResult(false, "Minut-värdet ej giltigt");



                if (tempHour > 23)
                    return new ValidationResult(false, "Tim-värdet ej giltigt");
                if (tempMinute > 59)
                    return new ValidationResult(false, "Minut-värdet ej giltigt");




            }
            catch (Exception ex)
            {
                return new ValidationResult(false, "ej korrekt");
            }
            return ValidationResult.ValidResult;
        }
    }
}
