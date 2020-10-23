using PrintCostCalculator3d.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PrintCostCalculator3d
{
    public class StockItemViewInfo
    {
        public String Name { get; set; }
        public string TranslatedName { get; set; }
        public Canvas Icon { get; set; }
        public StockItemViewManager.Group Group { get; set; }
        public string TranslatedGroup { get; set; }
        public bool IsVisible { get; set; }
        public bool Selected
        { get; set; }
        public MaterialStockItem Item
        { get; set; }

        public StockItemViewInfo()
        {

        }

        public StockItemViewInfo(String name, Canvas icon, StockItemViewManager.Group group)
        {
            Name = TranslatedName = name;
            Icon = icon;
            Group = group;
            TranslatedGroup = StockItemViewManager.TranslateGroup(group);
        }

        public StockItemViewInfo(String name, UIElement uiElement, StockItemViewManager.Group group)
        {
            Name = TranslatedName = name;
            var canvas = new Canvas();
            canvas.Children.Add(uiElement);
            Icon = canvas;
            Group = group;
            TranslatedGroup = StockItemViewManager.TranslateGroup(group);
        }
    }
}
