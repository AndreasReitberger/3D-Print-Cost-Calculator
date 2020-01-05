﻿using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace WpfFramework.Converters
{
    public sealed class BooleansOrConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Any(value => (bool)value);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
