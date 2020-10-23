using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintCostCalculator3d.Models.CRM
{
    class Person
    {
        #region Properties
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<ContactDetails> Contacts { get; set; }
        #endregion

        #region Constructor
        public Person()
        {
            Contacts = new List<ContactDetails>();
        }
        #endregion
    }
}
