using AndreasReitberger.Models;
using PrintCostCalculator3d.Enums;

namespace PrintCostCalculator3d.Models.Messaging
{
    public class GcodesChangedMessage
    {
        #region Properties
        public Gcode Gcode { get; set; }
        public MessagingAction Action { get; set; } = MessagingAction.Add;
        #endregion
    }
}
