using System;

using System.Globalization;
using System.Windows.Data;

namespace WpfFramework.Converters
{
    public sealed class BooleanToValidInvalidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool valid && valid ? 
                WpfFramework.Resources.Localization.Strings.Valid : WpfFramework.Resources.Localization.Strings.Invalid;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
