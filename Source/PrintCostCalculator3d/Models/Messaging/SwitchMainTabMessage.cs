using PrintCostCalculator3d.Enums;

namespace PrintCostCalculator3d.Models.Messaging
{
    public class SwitchMainTabMessage
    {
        #region Properties
        public int TargetTabIndex { get; set; } = 0;
        //public MessagingAction Action { get; set; } = MessagingAction.Add;
        #endregion
    }
}
