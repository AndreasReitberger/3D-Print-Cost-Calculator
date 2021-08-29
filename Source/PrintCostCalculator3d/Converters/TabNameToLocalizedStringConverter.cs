using PrintCostCalculator3d.Enums;
using PrintCostCalculator3d.Resources.Localization;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PrintCostCalculator3d.Converters
{
    public sealed class TabNameToLocalizedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not DashboardTabContentType tab)
            {
                return "-/-";
            }

            string tabName = tab switch
            {
                DashboardTabContentType.Calculator => Strings.Calculator,
                DashboardTabContentType.StlViewer => Strings.STLViewer,
                DashboardTabContentType.GcodeViewer => Strings.GcodeViewer,
                DashboardTabContentType.GcodeEditor => Strings.GcodeCodeLineEditor,
                _ => tab.ToString(),
            };
            return tabName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
