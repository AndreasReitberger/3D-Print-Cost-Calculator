using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfFramework.Converters
{
    public sealed class BooleanToFreeProConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool valid && valid ?
                WpfFramework.Resources.Localization.StaticStrings.ProductNamePro : WpfFramework.Resources.Localization.StaticStrings.ProductNamePro;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
