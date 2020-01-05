using System;
using System.Windows;
using System.Windows.Controls;
using WpfFramework.Models.Slicer;

namespace WpfFramework
{
    public class SlicerViewInfo
    {
        public String Name { get; set; }
        public string TranslatedName { get; set; }
        public Canvas Icon { get; set; }
        public SlicerViewManager.Group Group { get; set; }
        public string TranslatedGroup { get; set; }
        public bool IsVisible { get; set; }
        public bool Selected
        { get; set; }
        public Slicer Slicer
        { get; set; }

        public SlicerViewInfo()
        {

        }

        public SlicerViewInfo(String name, Canvas icon, SlicerViewManager.Group group)
        {
            Name = TranslatedName = name;
            Icon = icon;
            Group = group;
            TranslatedGroup = SlicerViewManager.TranslateGroup(group);
        }

        public SlicerViewInfo(String name, UIElement uiElement, SlicerViewManager.Group group)
        {
            Name = TranslatedName = name;
            var canvas = new Canvas();
            canvas.Children.Add(uiElement);
            Icon = canvas;
            Group = group;
            TranslatedGroup = SlicerViewManager.TranslateGroup(group);
        }
    }
}
