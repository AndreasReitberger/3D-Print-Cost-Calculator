using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace PrintCostCalculator3d.Converters
{
    public sealed class MoreThanXSelectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Collections.IList items = (System.Collections.IList)value;
            //var collection = items.Cast<ListBoxItem>();
            int limit = System.Convert.ToInt32(parameter);
           
            return (items.Count >= limit);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
