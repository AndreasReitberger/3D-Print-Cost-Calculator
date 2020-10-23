using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintCostCalculator3d.Models.GCode.Helper;

namespace PrintCostCalculator3d.Models.GCode
{
    public class GCodeProcessResult
    {
        #region Properties
        public bool ValidX { get; set; } = false;
        public bool ValidY { get; set; } = false;
        public bool ValidZ { get; set; } = false;
        public float MaxX { get; set; } = float.MinValue;
        public float MinX { get; set; } = float.MaxValue;
        public float MaxY { get; set; } = float.MinValue;
        public float MinY { get; set; } = float.MaxValue;
        public float MaxZ { get; set; } = float.MinValue;
        public float MinZ { get; set; } = float.MaxValue;
        public float PrintTimeAddition { get; set; } = 0;
        public float TotalFilament { get; set; } = 0;
        public float LastSpeed { get; set; } = 0;
        public long Order { get; set; } = 0;

        public GCodeModel Model { get; set; } = new GCodeModel(); 
        #endregion
    }
}