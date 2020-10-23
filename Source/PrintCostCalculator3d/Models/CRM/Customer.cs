using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintCostCalculator3d.Models.CRM
{
    class Customer
    {
        #region Properties
        public bool IsCompany { get; set; }
        public string CustomerId { get; set; }
        public List<Person> Persons { get; set; }
        public Address Address { get; set; }
        #endregion

        #region Constructor
        public Customer()
        {
            Persons = new List<Person>();
        }
        #endregion
    }
}
