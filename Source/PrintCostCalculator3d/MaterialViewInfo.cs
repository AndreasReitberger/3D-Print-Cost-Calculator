using AndreasReitberger.Enums;
using AndreasReitberger.Models;
using PrintCostCalculator3d.Models._3dprinting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PrintCostCalculator3d
{
    public class MaterialViewInfo
    {
        public String Name { get; set; }
        public string TranslatedName { get; set; }
        public Canvas Icon { get; set; }
        public Material3dTypes Group { get; set; }
        public string TranslatedGroup { get; set; }
        public string ComponentName { get; set; }
        public bool Selected
        { get; set; }
        //public _3dPrinterMaterial Material
        public Material3d Material
        { get; set; }

        public MaterialViewInfo()
        {

        }

        public MaterialViewInfo(String name, Canvas icon, Material3dTypes group)
        {
            Name = TranslatedName = name;
            Icon = icon;
            Group = group;
            TranslatedGroup = MaterialViewManager.TranslateGroup(group);
        }

        public MaterialViewInfo(String name, UIElement uiElement, Material3dTypes group)
        {
            Name = TranslatedName = name;
            var canvas = new Canvas();
            canvas.Children.Add(uiElement);
            Icon = canvas;
            Group = group;
            TranslatedGroup = MaterialViewManager.TranslateGroup(group);
        }
    }
}
