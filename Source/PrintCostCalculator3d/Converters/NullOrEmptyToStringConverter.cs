using System;
using System.Globalization;
using System.Windows.Data;

namespace PrintCostCalculator3d.Converters
{
    public sealed class NullOrEmptyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;

            return string.IsNullOrEmpty(str) ? "-/-" : str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
