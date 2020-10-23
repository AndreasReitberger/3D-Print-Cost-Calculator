using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintCostCalculator3d.Models.CRM
{
    class Address
    {
        #region Properties
        public string Street { get; set; }
        public string Number { get; set; }
        public string State { get; set; }
        public Countries Country { get; set; }
        #endregion
    }

    
}
