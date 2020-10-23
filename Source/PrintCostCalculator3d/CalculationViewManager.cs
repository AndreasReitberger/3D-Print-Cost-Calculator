using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintCostCalculator3d
{
    public static class CalculationViewManager
    {
        // List of all applications
        public static List<WorkstepViewInfo> List => new List<WorkstepViewInfo>
        {
            /*
            // General
            new StationViewInfo(Name.General, new PackIconModern{ Kind = PackIconModernKind.Box }, Group.General),

            // Applications
            new StationViewInfo(Name.IPScanner, ApplicationViewManager.GetIconByName(ApplicationViewManager.Name.IPScanner), Group.Applications),
            */
        };

        public static string TranslateGroup(Group group)
        {
            switch (group)
            {
                case Group.MISC:
                    return Resources.Localization.Strings.Misc;
                default:
                    return Resources.Localization.Strings.SettingsGroupNotFound;
            }
        }


        public enum Group
        {
            MISC,
        }
    }
}
