using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfFramework.Validators
{
    public class EmptySelectionValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            return value == null
                ? new ValidationResult(false, Resources.Localization.Strings.ValidatorSelectionIsMandatory)
                : new ValidationResult(true, null);
        }
    }
}
