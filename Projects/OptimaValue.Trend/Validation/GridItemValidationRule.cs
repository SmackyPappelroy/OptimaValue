using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace OptimaValue.Trend;

public class GridItemValidationRule : ValidationRule
{
    private string headerName;
    public GridItemValidationRule()
    {
    }

    public GridItemValidationRule(string header)
    {
        headerName = header;
    }
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is BindingGroup)
        {
            GridItem item = (value as BindingGroup).Items[0] as GridItem;
            if (int.TryParse(item.scaleMin.ToString(), out int scaleMin) && int.TryParse(item.scaleMax.ToString(), out int scaleMax))
            {
                if (scaleMin > scaleMax)
                {
                    return new ValidationResult(false, "Min värde kan inte va större än maxvärde");
                }
            }
            else if (!int.TryParse(item.scaleMin.ToString(), out int scaleMins))
            {
                return new ValidationResult(false, "Maxvärde måste vara integer");
            }
            else if (!int.TryParse(item.scaleMax.ToString(), out int scaleMaxs))
            {
                return new ValidationResult(false, "Minvärde måste vara integer");
            }
            else if (!int.TryParse(item.scaleOffset.ToString(), out int scaleOffsets))
            {
                return new ValidationResult(false, "Offsetvärde måste vara integer");
            }

        }
        else
        {
            switch (headerName)
            {
                case "scaleMin":
                    if (!int.TryParse(value.ToString(), out int scaleMin))
                    {
                        return new ValidationResult(false, "Minvärde måste vara integer");
                    }
                    break;
                case "scaleMax":
                    if (!int.TryParse(value.ToString(), out int scaleMax))
                    {
                        return new ValidationResult(false, "Maxvärde måste vara integer");
                    }
                    break;
                case "scaleOffset":
                    if (!int.TryParse(value.ToString(), out int scaleOffset))
                    {
                        return new ValidationResult(false, "Offsetvärde måste vara integer");
                    }
                    break;
                default:
                    break;
            }
        }
        return ValidationResult.ValidResult;
    }
}
