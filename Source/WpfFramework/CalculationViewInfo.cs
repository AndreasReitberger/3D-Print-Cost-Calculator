using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfFramework.Models._3dprinting;

namespace WpfFramework
{
    public class CalculationViewInfo
    {
        public String Name { get; set; }
        public string TranslatedName { get; set; }
        public Canvas Icon { get; set; }
        public CalculationViewManager.Group Group { get; set; }
        public string TranslatedGroup { get; set; }
        public bool IsVisible { get; set; }
        public bool Selected
        { get; set; }
        public _3dPrinterCalculationModel Calculation
        { get; set; }

        public CalculationViewInfo()
        {

        }

        public CalculationViewInfo(String name, Canvas icon, CalculationViewManager.Group group)
        {
            Name = TranslatedName = name;
            Icon = icon;
            Group = group;
            TranslatedGroup = CalculationViewManager.TranslateGroup(group);
        }

        public CalculationViewInfo(String name, UIElement uiElement, CalculationViewManager.Group group)
        {
            Name = TranslatedName = name;
            var canvas = new Canvas();
            canvas.Children.Add(uiElement);
            Icon = canvas;
            Group = group;
            TranslatedGroup = CalculationViewManager.TranslateGroup(group);
        }
    }
}
