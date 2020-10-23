using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PrintCostCalculator3d.Converters
{
    public sealed class ColorToStringConverter : IValueConverter
    {
        /* Translate the name of the accent */
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Color color))
                return "No valid accent passed!";

            var name = color.ToString();
            return name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
