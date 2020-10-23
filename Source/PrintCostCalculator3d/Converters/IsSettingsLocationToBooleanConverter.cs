using System;
using System.Globalization;
using System.Windows.Data;
using PrintCostCalculator3d.Models.Settings;

namespace PrintCostCalculator3d.Converters
{
    public sealed class IsSettingsLocationToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as string == SettingsManager.GetSettingsLocation();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
