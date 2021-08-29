using PrintCostCalculator3d.Enums;
using System;

namespace PrintCostCalculator3d.Models.Messaging
{
    public class CalculationActionMessage
    {
        #region Properties
        public Guid CalculationId { get; set; } = Guid.Empty;
        public CalculationMessagingAction Action { get; set; } = CalculationMessagingAction.Add;
        #endregion
    }
}
