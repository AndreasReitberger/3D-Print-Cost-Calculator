using System;

namespace PrintCostCalculator3d.Models.Events
{
    public class StlChangedEventArgs : EventArgs
    {
        public Stl NewStl { get; set; }
        public Stl PreviousStl { get; set; }
    }
}
