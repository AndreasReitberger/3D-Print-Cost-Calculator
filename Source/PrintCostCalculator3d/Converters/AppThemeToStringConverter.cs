using ControlzEx.Theming;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PrintCostCalculator3d.Converters
{
    public sealed class AppThemeToStringConverter : IValueConverter
    {
        /* Translate the name of the app theme */
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Theme theme))
                return "No valid theme passed!";

            var name = theme.Name;

            if (string.IsNullOrEmpty(name))
                name = theme.Name;

            return name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
