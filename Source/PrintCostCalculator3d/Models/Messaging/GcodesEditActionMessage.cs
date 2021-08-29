using AndreasReitberger.Models;
using System.Collections.Generic;

namespace PrintCostCalculator3d.Models.Messaging
{
    public class GcodesEditActionMessage
    {
        #region Properties
        public List<Gcode> GcodeFiles { get; set; } = new List<Gcode>();

        #endregion
    }
}
