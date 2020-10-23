using System;

using System.Globalization;
using System.Windows.Data;

namespace PrintCostCalculator3d.Converters
{
    public sealed class BooleanToValidInvalidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool valid && valid ? 
                PrintCostCalculator3d.Resources.Localization.Strings.Valid : PrintCostCalculator3d.Resources.Localization.Strings.Invalid;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
