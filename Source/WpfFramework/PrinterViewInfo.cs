using System;
using System.Windows;
using System.Windows.Controls;

using WpfFramework.Models._3dprinting;

namespace WpfFramework
{
    public class PrinterViewInfo
    {
        public String Name { get; set; }
        public string TranslatedName { get; set; }
        public Canvas Icon { get; set; }
        public PrinterViewManager.Group Group { get; set; }
        public string TranslatedGroup { get; set; }
        public bool IsVisible { get; set; }
        public bool Selected
        { get; set; }
        public _3dPrinterModel Printer
        { get; set; }

        public PrinterViewInfo()
        {

        }

        public PrinterViewInfo(String name, Canvas icon, PrinterViewManager.Group group)
        {
            Name = TranslatedName = name;
            Icon = icon;
            Group = group;
            TranslatedGroup = PrinterViewManager.TranslateGroup(group);
        }

        public PrinterViewInfo(String name, UIElement uiElement, PrinterViewManager.Group group)
        {
            Name = TranslatedName = name;
            var canvas = new Canvas();
            canvas.Children.Add(uiElement);
            Icon = canvas;
            Group = group;
            TranslatedGroup = PrinterViewManager.TranslateGroup(group);
        }
    }
}
