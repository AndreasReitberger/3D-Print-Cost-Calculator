using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PrintCostCalculator3d.Converters
{

    public sealed class OctoPrintInt64DateToDateTimeConverter : IValueConverter
    {
        /* Translate the name of the accent */
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc); 
                dt = dt.AddSeconds(System.Convert.ToInt64(value)).ToLocalTime();

                return dt;
            }
            catch (Exception)
            {
                return TimeSpan.Zero;
            }


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                DateTime dt = (DateTime)value;
                if (dt == null)
                    return 0;
                return dt.Ticks;
                //throw new NotImplementedException();
            }
            catch (Exception)
            {
                return 0;
            }
}
    }
}
