using WpfFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfFramework
{
    public class MachineHourRateViewInfo
    {
        public String Name { get; set; }
        public string TranslatedName { get; set; }
        public Canvas Icon { get; set; }
        public MachineHourRateViewManager.Group Group { get; set; }
        public string TranslatedGroup { get; set; }
        public bool IsVisible { get; set; }
        public bool Selected
        { get; set; }
        public MachineHourRate MachineHourRate
        { get; set; }

        public MachineHourRateViewInfo()
        {

        }

        public MachineHourRateViewInfo(String name, Canvas icon, MachineHourRateViewManager.Group group)
        {
            Name = TranslatedName = name;
            Icon = icon;
            Group = group;
            TranslatedGroup = MachineHourRateViewManager.TranslateGroup(group);
        }

        public MachineHourRateViewInfo(String name, UIElement uiElement, MachineHourRateViewManager.Group group)
        {
            Name = TranslatedName = name;
            var canvas = new Canvas();
            canvas.Children.Add(uiElement);
            Icon = canvas;
            Group = group;
            TranslatedGroup = MachineHourRateViewManager.TranslateGroup(group);
        }
    }
}
