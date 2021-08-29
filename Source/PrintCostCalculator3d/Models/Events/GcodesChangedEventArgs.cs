using AndreasReitberger.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace PrintCostCalculator3d.Models.Events
{
    public class GcodesChangedEventArgs : EventArgs
    {
        public List<Gcode> NewItems { get; set; } = new List<Gcode>();
        public List<Gcode> OldItems { get; set; } = new List<Gcode>();
        public NotifyCollectionChangedAction Action { get; set; } = NotifyCollectionChangedAction.Add;
        public ObservableCollection<Gcode> Gcodes { get; set; } = new ObservableCollection<Gcode>();     
    }
}
