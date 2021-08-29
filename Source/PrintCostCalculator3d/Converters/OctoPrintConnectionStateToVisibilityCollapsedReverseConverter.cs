using AndreasReitberger.Enum;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace PrintCostCalculator3d.Converters
{
    public sealed class OctoPrintConnectionStateToVisibilityCollapsedReverseConverter : IValueConverter
    {
        #region Properties
        public OctoPrintConnectionStates[] VisibilityState { get; set; }
        #endregion
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (VisibilityState == null || VisibilityState.Length == 0)
                VisibilityState = new OctoPrintConnectionStates[] { OctoPrintConnectionStates.Operational, OctoPrintConnectionStates.Printing };

            return value is OctoPrintConnectionStates state && VisibilityState.Any(st => st == state) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
