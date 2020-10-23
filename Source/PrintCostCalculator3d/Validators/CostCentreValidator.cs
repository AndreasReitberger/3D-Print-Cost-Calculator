using System;
using System.Globalization;
using System.Windows.Controls;

namespace PrintCostCalculator3d.Validators
{
    public class CostCentreValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string costcentre = (string)value;
            if (string.IsNullOrEmpty(costcentre))
                return new ValidationResult(false, "Feld darf nicht leer sein!");
            if(costcentre.Length != 6)
                return new ValidationResult(false, "Die Kostenstelle muss 6 Zeichen lang sein!");
            int num;
            bool isNummeric = int.TryParse(costcentre, out num);
            return isNummeric ? ValidationResult.ValidResult : new ValidationResult(false, "Die Kostenstelle darf nur aus nummerischen Zeichen bestehen!");
        }
    }
}
