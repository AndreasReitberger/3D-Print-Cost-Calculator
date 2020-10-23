using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AndreasReitberger.Models;
using PrintCostCalculator3d.Models._3dprinting;

namespace PrintCostCalculator3d
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
        public Calculation3d Calculation
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
