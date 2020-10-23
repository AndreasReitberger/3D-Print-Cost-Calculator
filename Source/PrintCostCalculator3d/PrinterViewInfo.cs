using System;
using System.Windows;
using System.Windows.Controls;
using AndreasReitberger.Enums;
using AndreasReitberger.Models;

namespace PrintCostCalculator3d
{
    public class PrinterViewInfo
    {
        public String Name { get; set; }
        public string TranslatedName { get; set; }
        public Canvas Icon { get; set; }
        public Printer3dType Group { get; set; }
        public string TranslatedGroup { get; set; }
        public bool IsVisible { get; set; }
        public bool Selected
        { get; set; }
        public Printer3d Printer
        { get; set; }

        public PrinterViewInfo()
        {

        }

        public PrinterViewInfo(String name, Canvas icon, Printer3dType group)
        {
            Name = TranslatedName = name;
            Icon = icon;
            Group = group;
            TranslatedGroup = PrinterViewManager.TranslateGroup(group);
        }

        public PrinterViewInfo(String name, UIElement uiElement, Printer3dType group)
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
