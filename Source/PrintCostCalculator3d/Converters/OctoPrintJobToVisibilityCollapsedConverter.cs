using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using AndreasReitberger.Models;

namespace PrintCostCalculator3d.Converters
{
    public sealed class OctoPrintJobToVisibilityCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is OctoPrintJobInfoJob job && job != null && job.File.Name != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
