using System;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;

namespace PrintCostCalculator3d.Converters
{
    public sealed class StringToColorConverter : IValueConverter
    {
        /* Translate the name of the accent */
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string.IsNullOrEmpty((string)value)))
                return "No valid string passed!";

            Color c = (Color)ColorConverter.ConvertFromString((string)value);
            return c;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color c = (Color)value;
            if ((c != null))
            {
                return c.ToString();
            }
            else return string.Empty;
            //throw new NotImplementedException();
        }
    }
}
