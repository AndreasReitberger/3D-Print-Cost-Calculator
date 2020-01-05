using System;
using System.Globalization;
using System.Windows.Data;
using WpfFramework.Models.Settings;

namespace WpfFramework.Converters
{
    public sealed class IsDefaultSettingsLocationToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as string == SettingsManager.GetDefaultSettingsLocation();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
