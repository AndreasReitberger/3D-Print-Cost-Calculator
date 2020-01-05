using WpfFramework.Models._3dprinting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfFramework
{
    public class MaterialViewInfo
    {
        public String Name { get; set; }
        public string TranslatedName { get; set; }
        public Canvas Icon { get; set; }
        public MaterialViewManager.Group Group { get; set; }
        public string TranslatedGroup { get; set; }
        public string ComponentName { get; set; }
        public bool Selected
        { get; set; }
        public _3dPrinterMaterial Material
        { get; set; }

        public MaterialViewInfo()
        {

        }

        public MaterialViewInfo(String name, Canvas icon, MaterialViewManager.Group group)
        {
            Name = TranslatedName = name;
            Icon = icon;
            Group = group;
            TranslatedGroup = group.ToString();
        }

        public MaterialViewInfo(String name, UIElement uiElement, MaterialViewManager.Group group)
        {
            Name = TranslatedName = name;
            var canvas = new Canvas();
            canvas.Children.Add(uiElement);
            Icon = canvas;
            Group = group;
            TranslatedGroup = group.ToString();
        }
    }
}
