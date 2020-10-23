using System.Globalization;
using System.IO;
using System.Windows.Controls;
using PrintCostCalculator3d.Models.Settings;

namespace PrintCostCalculator3d.Validators
{
    public class SettingsFolderExistsValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var path = value as string;

            if (Directory.Exists(path) || SettingsManager.GetDefaultSettingsLocation() == path)
                return ValidationResult.ValidResult;

            return new ValidationResult(false, Resources.Localization.Strings.FolderDoesNotExist);
        }
    }
}
