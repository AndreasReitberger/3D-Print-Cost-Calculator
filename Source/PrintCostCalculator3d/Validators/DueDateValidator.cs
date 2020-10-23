using System;
using System.Globalization;
using System.Windows.Controls;
using PrintCostCalculator3d.Resources.Localization;

namespace PrintCostCalculator3d.Validators
{
    public class DueDateValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            DateTime dueDate = (DateTime)value;
            if (dueDate == null)
                return new ValidationResult(false, Strings.FieldCannotBeEmpty);
            else if (dueDate.Date <= DateTime.Now.Date)
                return new ValidationResult(false, "Das Fälligkeitsdatum kann nicht in der Vergangenheit liegen!");
            else
                return ValidationResult.ValidResult;
        }
    }
}
