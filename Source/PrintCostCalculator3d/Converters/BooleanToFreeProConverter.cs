using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PrintCostCalculator3d.Converters
{
    public sealed class BooleanToFreeProConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool valid && valid ?
                PrintCostCalculator3d.Resources.Localization.StaticStrings.ProductNamePro : PrintCostCalculator3d.Resources.Localization.StaticStrings.ProductNamePro;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
