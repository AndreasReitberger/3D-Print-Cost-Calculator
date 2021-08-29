using PrintCostCalculator3d.Resources.Localization;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PrintCostCalculator3d.Converters
{
    public sealed class CostElementLocalizedNameConverter : IValueConverter
    {
        /* Translate the name of the accent */
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value as string;
            if ((string.IsNullOrEmpty(str)))
                return "No valid string passed!";

            switch(str.ToLower().Trim())
            {
                case "margin":
                    return Strings.Margin;
                case "tax":
                    return Strings.Tax;
                case "handlingfee":
                    return Strings.HandlingFee;
                case "filtercosts":
                    return Strings.FilterCosts;
                case "curingcosts":
                    return Strings.CuringCosts;
                case "washingcosts":
                    return Strings.WashingCosts;
                case "glovecosts":
                    return Strings.GloveCosts;
                case "nozzlewearcosts":
                    return Strings.NozzleWearCosts;
                case "printsheetwearcosts":
                    return Strings.PrintSheetWearCosts;
                default:
                    return str;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
