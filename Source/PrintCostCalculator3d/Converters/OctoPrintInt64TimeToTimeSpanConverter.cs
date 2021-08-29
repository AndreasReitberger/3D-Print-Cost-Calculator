using System;
using System.Globalization;
using System.Windows.Data;

namespace PrintCostCalculator3d.Converters
{
    public sealed class OctoPrintInt64TimeToTimeSpanConverter : IValueConverter
    {
        /* Translate the name of the accent */
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                TimeSpan ts = TimeSpan.FromSeconds(System.Convert.ToInt64(value));
                return ts;
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
                TimeSpan ts = (TimeSpan)value;
                if (ts == null)
                    return 0;
                return ts.TotalSeconds;
                //throw new NotImplementedException();
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
