using AndreasReitberger.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PrintCostCalculator3d.Converters
{
    public sealed class OctoPrintJobToVisibilityCollapsedReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is OctoPrintJobInfoJob job && job != null && job.File.Name != null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
