
using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace PrintCostCalculator3d.Validators
{
    public class FolderExistsValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var path = value as string;

            if (Directory.Exists(path))
                return ValidationResult.ValidResult;

            return new ValidationResult(false, Resources.Localization.Strings.FolderDoesNotExist);
        }
    }
}
