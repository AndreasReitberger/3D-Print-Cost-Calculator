using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfFramework.Resources.Localization;

namespace WpfFramework.Validators
{
    public class GreaterThanZeroValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is null)
                return new ValidationResult(false, Strings.ValueCannotBeEmpty);
            switch(value)
            {
                case double d:
                    if(d > 0)
                        return ValidationResult.ValidResult;
                    else
                        return new ValidationResult(false, string.Format(Strings.EnterValueGreaterThanFormated, "0.00"));
                case int i:
                    if (i > 0)
                        return ValidationResult.ValidResult;
                    else
                        return new ValidationResult(false, string.Format(Strings.EnterValueGreaterThanFormated, "0"));
                case decimal dec:
                    if (dec > 0)
                        return ValidationResult.ValidResult;
                    else
                        return new ValidationResult(false, string.Format(Strings.EnterValueGreaterThanFormated, "0.00"));
                default:
                    return new ValidationResult(false, Strings.ValueTypeUnknown);
            }
        }
    }
}
