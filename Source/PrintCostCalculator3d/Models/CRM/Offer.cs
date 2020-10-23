using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintCostCalculator3d.Models._3dprinting;

namespace PrintCostCalculator3d.Models.CRM
{
    class Offer
    {
        #region Properties
        public string OfferId { get; set; }
        public DateTime CreationDate { get; set; }
        public int DaysValid { get; set; }
        public Customer Customer { get; set; }
        public List<_3dPrinterCalculationModel> Calculations { get; set; }
        public OfferStates OfferState { get; set; }
        #endregion

        #region Constructor
        public Offer()
        {
            Calculations = new List<_3dPrinterCalculationModel>();
        }
        public Offer(Customer customer, List<_3dPrinterCalculationModel> calculations, DateTime creation, int validDays)
        {
            Customer = customer;
            Calculations = calculations;
            CreationDate = creation;
            DaysValid = validDays;
        }
        #endregion
    }

    public enum OfferStates
    { 
        Draft,
        Sent,
        Accepted,
        Declined,
    }
}
