using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PrintCostCalculator3d.Converters
{
    public sealed class JobIdToVisibilityReverseCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is long id && id > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
