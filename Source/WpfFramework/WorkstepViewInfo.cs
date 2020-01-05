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
    public class WorkstepViewInfo
    {
        public String Name { get; set; }
        public string TranslatedName { get; set; }
        public Canvas Icon { get; set; }
        public WorkstepViewManager.Group Group { get; set; }
        public string TranslatedGroup { get; set; }
        public bool IsVisible { get; set; }
        public bool Selected
        { get; set; }

        public WorkstepViewInfo()
        {

        }

        public WorkstepViewInfo(String name, Canvas icon, WorkstepViewManager.Group group)
        {
            Name = TranslatedName = name;
            Icon = icon;
            Group = group;
            TranslatedGroup = WorkstepViewManager.TranslateGroup(group);
        }

        public WorkstepViewInfo(String name, UIElement uiElement, WorkstepViewManager.Group group)
        {
            Name = TranslatedName = name;
            var canvas = new Canvas();
            canvas.Children.Add(uiElement);
            Icon = canvas;
            Group = group;
            TranslatedGroup = WorkstepViewManager.TranslateGroup(group);
        }
    }
}
