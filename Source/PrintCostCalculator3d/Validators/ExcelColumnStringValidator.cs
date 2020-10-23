using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using PrintCostCalculator3d.Utilities;

namespace PrintCostCalculator3d.Validators
{
    public class ExcelColumnStringValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value != null && (Regex.IsMatch((string)value, @"^[A-Za-z]+$")))
                return ValidationResult.ValidResult;

            return new ValidationResult(false, Resources.Localization.Strings.PleaseAddValidExcelColumnString);
        }
    }
}
