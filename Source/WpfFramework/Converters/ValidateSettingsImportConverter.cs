﻿using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using WpfFramework.Utilities;

namespace WpfFramework.Converters
{
    public sealed class ValidateSettingsImportConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)values[0] && values.SubArray(1, values.Length - 1).Any(x => (bool)x);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
