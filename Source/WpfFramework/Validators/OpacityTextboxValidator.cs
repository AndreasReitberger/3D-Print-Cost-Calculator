using System.Globalization;
using System.Windows.Controls;
using WpfFramework.Resources.Localization;

namespace WpfFramework.Validators
{
    public class OpacityTextboxValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!int.TryParse(value as string, out var result))
                return new ValidationResult(false, Strings.OnlyNumbersAreAllowed);

            if (result < 10 || result > 100)
                return new ValidationResult(false, Strings.ValidNumberFrom10Till100);

            return ValidationResult.ValidResult;

        }
    }
}
