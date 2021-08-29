using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PrintCostCalculator3d.Converters
{
    public sealed class RepetierIntToDateTimeConverter : IValueConverter
    {
        /* Translate the name of the accent */
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                DateTime dt = DateTime.MinValue;
                TimeSpan ts = TimeSpan.FromMilliseconds(System.Convert.ToDouble(value));
                dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).Add(ts);

                return dt;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
            /*
            DateTime dt = (DateTime)value;
            if (dt == null)
                return 0;
            return dt.Second;
            */
        }
    }
}
