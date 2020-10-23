using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintCostCalculator3d.Models.Syncfusion
{
    public class ChartItem
    {
        #region Properties
        public string Name { get; set; }
        public double Value { get; set; }
        #endregion

        #region Constructor
        public ChartItem()
        {

        }
        public ChartItem(string name, double value)
        {
            Name = name;
            Value = value;
        }
        #endregion
    }
}
