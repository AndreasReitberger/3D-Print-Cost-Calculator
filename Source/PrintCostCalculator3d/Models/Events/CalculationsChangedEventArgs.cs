using AndreasReitberger.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace PrintCostCalculator3d.Models.Events
{
    public class CalculationsChangedEventArgs : EventArgs
    {
        public List<Calculation3d> NewItems { get; set; } = new List<Calculation3d>();
        public List<Calculation3d> OldItems { get; set; } = new List<Calculation3d>();
        public NotifyCollectionChangedAction Action { get; set; } = NotifyCollectionChangedAction.Add;
        public ObservableCollection<Calculation3d> Calculations { get; set; } = new ObservableCollection<Calculation3d>();     
    }
}
