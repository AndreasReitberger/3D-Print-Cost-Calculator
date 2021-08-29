using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PrintCostCalculator3d.Converters
{
    public class OctoPrintModelTypeToVisibilityCollapsedConverter : IValueConverter
    {
        #region Properties
        public string Type  { get; set; }
        #endregion

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string type && type == Type ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
