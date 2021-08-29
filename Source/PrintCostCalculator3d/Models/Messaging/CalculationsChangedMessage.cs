using AndreasReitberger.Models;
using PrintCostCalculator3d.Enums;

namespace PrintCostCalculator3d.Models.Messaging
{
    public class CalculationsChangedMessage
    {
        #region Properties
        public Calculation3d Calculation { get; set; }
        public MessagingAction Action { get; set; } = MessagingAction.Add;
        #endregion
    }
}
