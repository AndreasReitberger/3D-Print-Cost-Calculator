using ControlzEx.Theming;
using MahApps.Metro;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace PrintCostCalculator3d.Converters
{
    public sealed class AppThemeToBrushConverter : IValueConverter
    {
        /* Convert an MahApps.Metro.Accent (from wpf/xaml-binding) to a Brush to fill rectangle with color in a ComboBox */
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ThemeManager.Current.Themes.First(x => 
            x.Name == ((Theme)System.Convert.ChangeType(value, typeof(Theme))).Name).Resources["MahApps.Brushes.AccentBase"] as Brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
