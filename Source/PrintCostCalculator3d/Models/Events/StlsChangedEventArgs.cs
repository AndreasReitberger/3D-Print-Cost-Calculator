using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace PrintCostCalculator3d.Models.Events
{
    public class StlsChangedEventArgs : EventArgs
    {
        public List<Stl> NewItems { get; set; } = new List<Stl>();
        public List<Stl> OldItems { get; set; } = new List<Stl>();
        public NotifyCollectionChangedAction Action { get; set; } = NotifyCollectionChangedAction.Add;
        public ObservableCollection<Stl> Stls { get; set; } = new ObservableCollection<Stl>();     
    }
}
