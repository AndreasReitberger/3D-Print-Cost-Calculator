using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintCostCalculator3d.Models.Slicer
{
    public class SlicerPrinterConfiguration
    {
        public string PrinterName { get; set; }
        public float AMax_xy { get; set; }
        public float AMax_z { get; set; }
        public float AMax_e { get; set; }
        public float AMax_eExtrude { get; set; }
        public float AMax_eRetract { get; set; }
        public float PrintDurationCorrection { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as SlicerPrinterConfiguration;
            if (item == null)
                return false;
            return this.PrinterName.Equals(item.PrinterName);
        }
        public override int GetHashCode()
        {
            return this.PrinterName.GetHashCode();
        }
    }
}
