﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace WpfFramework.Converters
{
    public sealed class BooleanToVisibilityHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool visible && visible ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
