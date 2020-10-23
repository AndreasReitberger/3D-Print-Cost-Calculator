using System.Windows;

namespace PrintCostCalculator3d.Utilities
{
    public static class CommonMethods
    {
        public static void SetClipboard(string text)
        {
            Clipboard.SetDataObject(text, true);
        }
    }
}
