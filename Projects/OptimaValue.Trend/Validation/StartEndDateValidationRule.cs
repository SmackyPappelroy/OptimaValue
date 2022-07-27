using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OptimaValue.Trend
{
    public class StartEndDateValidationRule : ValidationRule
    {
        public bool IsStartDate { get; set; }
        public StartEndDateValidationRule()
        {

        }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!DateTime.TryParse(value.ToString(), out var result))
            {
                return new ValidationResult(false, "Ingen giltig tid");
            }
            if (IsStartDate)
            {
                var startDate = DateTime.Parse(value.ToString());

                if (startDate >= StaticClass.MaxDateSeries)
                {
                    return new ValidationResult(false, "Startdatum kan inte vara större än slutdatum");
                }
            }
            else
            {
                var endDate = DateTime.Parse(value.ToString());
                if (endDate <= StaticClass.MinDateSeries)
                {
                    return new ValidationResult(false, "Slutdatum kan inte vara för tidigt");
                }
            }
            return ValidationResult.ValidResult;
        }
    }
}
