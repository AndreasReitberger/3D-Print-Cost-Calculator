using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PrintCostCalculator3d.Models._3dprinting
{
    public struct CostElement
    {
        #region Properties
        public string Name { get; set; }
        public decimal Price { get; set; }
        public Brush Color { get; set; }
        #endregion
    }
}
