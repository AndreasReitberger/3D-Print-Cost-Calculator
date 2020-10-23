using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PrintCostCalculator3d.Validators
{
    public class FilePathValidator : ValidationRule
    {
        public string FileExtension { get; set; }
        public bool CheckIfFileExists { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var path = value as string;
            var ext = Path.GetExtension(path);

            if (CheckIfFileExists && File.Exists(path))
            {
                if (!string.IsNullOrEmpty(FileExtension))
                    return ext == FileExtension ? ValidationResult.ValidResult : new ValidationResult(false, Resources.Localization.Strings.FIlePathIsInvalid);
                else
                    return ValidationResult.ValidResult;
            }
            else if(!CheckIfFileExists)
            {
                if (!string.IsNullOrEmpty(FileExtension))
                    return ext == FileExtension ? ValidationResult.ValidResult : new ValidationResult(false, Resources.Localization.Strings.FIlePathIsInvalid);
                else
                    return ValidationResult.ValidResult;
            }
            else
            {
                return new ValidationResult(false, Resources.Localization.Strings.FIlePathIsInvalid);
            }

            
        }
    }
}
