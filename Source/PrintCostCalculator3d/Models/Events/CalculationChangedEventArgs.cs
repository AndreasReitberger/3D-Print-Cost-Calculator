using AndreasReitberger.Models;
using System;


namespace PrintCostCalculator3d.Models.Events
{
    public class CalculationChangedEventArgs : EventArgs
    {
        public Calculation3d NewCalculation { get; set; }
        public Calculation3d PreviousCalculation { get; set; }
    }
}
