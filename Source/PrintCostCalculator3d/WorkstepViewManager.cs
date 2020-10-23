using AndreasReitberger.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintCostCalculator3d
{
    public static class WorkstepViewManager
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

        public static string TranslateGroup(WorkstepType group)
        {
            switch (group)
            {
                case WorkstepType.Pre:
                    return Resources.Localization.Strings.Pre;
                //return Resources.Localization.Strings.General;
                case WorkstepType.Post:
                    return Resources.Localization.Strings.Post;
                case WorkstepType.Misc:
                    return Resources.Localization.Strings.Misc;
                default:
                    return Resources.Localization.Strings.SettingsGroupNotFound;
            }
        }
    }
}
