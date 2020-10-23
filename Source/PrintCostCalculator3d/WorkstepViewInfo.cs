using AndreasReitberger.Enums;
using AndreasReitberger.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PrintCostCalculator3d
{
    public class WorkstepViewInfo
    {
        public String Name { get; set; }
        public string TranslatedName { get; set; }
        public Canvas Icon { get; set; }
        public WorkstepType Group { get; set; }
        public string TranslatedGroup { get; set; }
        public bool IsVisible { get; set; }
        public bool Selected
        { get; set; }
        public Workstep Workstep
        { get; set; }

        public WorkstepViewInfo()
        {

        }

        public WorkstepViewInfo(String name, Canvas icon, WorkstepType group)
        {
            Name = TranslatedName = name;
            Icon = icon;
            Group = group;
            TranslatedGroup = WorkstepViewManager.TranslateGroup(group);
        }

        public WorkstepViewInfo(String name, UIElement uiElement, WorkstepType group)
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
