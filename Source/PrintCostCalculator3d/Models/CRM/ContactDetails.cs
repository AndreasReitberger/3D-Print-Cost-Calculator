using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintCostCalculator3d.Models.CRM
{
    struct ContactDetails
    {
        #region Properties
        public ContactType Type { get; set; }
        public string ContactInformation { get; set; }
        #endregion
    }

    public enum ContactType
    {
        Mail,
        eMail,
        Phone,
        Fax
    }
}
