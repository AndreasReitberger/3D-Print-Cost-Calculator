using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WpfFramework.Models;
using WpfFramework.Resources.Localization;

namespace WpfFramework.Converters
{
    public sealed class VariableProgressValueToStringConverter : IMultiValueConverter
    {
        public string VariableName { get; set; }
        /* Translate the name of the accent */
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                long layer = System.Convert.ToInt64(value[0]);
                long layers = System.Convert.ToInt64(value[1]);
                return string.Format(Strings.VariableProgressOf, layer, layers, this.VariableName);
            }
            catch(Exception )
            {
                return string.Format(Strings.VariableProgressOf, 0, 0, this.VariableName);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
