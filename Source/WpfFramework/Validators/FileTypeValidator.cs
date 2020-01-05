using System;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using WpfFramework.Resources.Localization;

namespace WpfFramework.Validators
{
    public class FileTypeValidator : ValidationRule
    {
        public string fileExtension
        { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string filepath = (string)value;
            if (string.IsNullOrEmpty(filepath))
                return new ValidationResult(false, Strings.FieldCannotBeEmpty);
            if (!filepath.EndsWith(fileExtension))
                return new ValidationResult(false, string.Format(Strings.FileExtensionFormatedInvalid, fileExtension));

            DirectoryInfo dir = new DirectoryInfo(filepath);
            if(!File.Exists(dir.FullName))
            {
                return new ValidationResult(false, string.Format(Strings.FileDoesNotExistsFormated, filepath));
            }
            return ValidationResult.ValidResult;
        }
    }
}
