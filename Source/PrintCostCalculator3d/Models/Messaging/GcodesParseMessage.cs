using AndreasReitberger.Models;
using System.Collections.Generic;

namespace PrintCostCalculator3d.Models.Messaging
{
    public class GcodesParseMessage
    {
        #region Properties
        public List<List<string>> GcodeFiles { get; set; } = new List<List<string>>();
        public SlicerPrinterConfiguration SlicerConfig { get; set; }
        //public MessagingAction Action { get; set; } = MessagingAction.Add;
        #endregion
    }
}
