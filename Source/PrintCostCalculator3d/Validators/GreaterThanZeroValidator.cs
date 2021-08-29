using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using PrintCostCalculator3d.Resources.Localization;

namespace PrintCostCalculator3d.Validators
{
    public class GreaterThanZeroValidator : ValidationRule
    {
        bool _allowZero = false;
        public bool AllowZero
        {   
            get => _allowZero; 
            set
            {
                if (_allowZero == value)
                    return;
                _allowZero = value;
            }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is null)
                return new ValidationResult(false, Strings.ValueCannotBeEmpty);
            switch(value)
            {
                case double d:
                    if(d > 0 || (d >= 0 && AllowZero))
                        return ValidationResult.ValidResult;
                    else
                        return new ValidationResult(false, string.Format(Strings.EnterValueGreaterThanFormated, "0.00"));
                case int i:
                    if (i > 0 || (i >= 0 && AllowZero))
                        return ValidationResult.ValidResult;
                    else
                        return new ValidationResult(false, string.Format(Strings.EnterValueGreaterThanFormated, "0"));
                case decimal dec:
                    if (dec > 0 || (dec >= 0m && AllowZero))
                        return ValidationResult.ValidResult;
                    else
                        return new ValidationResult(false, string.Format(Strings.EnterValueGreaterThanFormated, "0.00"));
                default:
                    return new ValidationResult(false, Strings.ValueTypeUnknown);
            }
        }
    }
}
