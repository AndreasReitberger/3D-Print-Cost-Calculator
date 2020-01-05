using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WpfFramework.Resources.Localization;

namespace WpfFramework.Converters
{
    public sealed class PercentageProgressValueToStringConverter : IValueConverter
    {
        public string VariableName { get; set; }
        /* Translate the name of the accent */
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                long progress = System.Convert.ToInt64(value);
                return string.Format(Strings.PercentageProgressOf, progress, 100);
            }
            catch (Exception)
            {
                return string.Format(Strings.PercentageProgressOf, 0, 100);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
