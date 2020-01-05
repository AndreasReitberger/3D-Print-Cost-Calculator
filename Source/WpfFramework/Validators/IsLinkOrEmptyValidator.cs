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
    public class IsLinkOrEmptyValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if(string.IsNullOrEmpty((string)value))
                return ValidationResult.ValidResult;
            Uri uriResult;
            bool result = Uri.TryCreate((string)value, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return  result ? ValidationResult.ValidResult : new ValidationResult(false, Strings.EnterValidUri);
        }
    }
}
