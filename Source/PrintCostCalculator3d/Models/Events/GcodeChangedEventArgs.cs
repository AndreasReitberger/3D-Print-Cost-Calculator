using AndreasReitberger.Models;
using System;

namespace PrintCostCalculator3d.Models.Events
{
    public class GcodeChangedEventArgs : EventArgs
    {
        public Gcode NewGcode { get; set; }
        public Gcode PreviousGcode { get; set; }
    }
}
