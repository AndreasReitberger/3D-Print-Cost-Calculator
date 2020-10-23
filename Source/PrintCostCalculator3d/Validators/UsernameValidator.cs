using System.Globalization;
using System.Windows.Controls;

namespace PrintCostCalculator3d.Validators
{
    public class UsernameValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string username = (string)value;
            if (string.IsNullOrEmpty(username))
                return new ValidationResult(false, "Feld darf nicht leer sein!");
            else if ((username[0].ToString()).ToUpper() != "Q")
                return new ValidationResult(false, string.Format("Der Benutzername muss mit einem {0} beginnen!", 'Q'));
            else if (username.Length != 5 & username.Length != 6)
                return new ValidationResult(false, string.Format("Der Benutzername darf nur {0} oder {1} Zeichen lang sein!", 5, 6));
            
            else
                return ValidationResult.ValidResult;
        }
    }
}
