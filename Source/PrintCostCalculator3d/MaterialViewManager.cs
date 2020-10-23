using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndreasReitberger.Enums;
using AndreasReitberger.Models.MaterialAdditions;
using PrintCostCalculator3d.Models;
using PrintCostCalculator3d.Resources.Localization;

namespace PrintCostCalculator3d
{
    public static class MaterialViewManager
    {
        // List of all applications
        public static List<MaterialViewInfo> List => new List<MaterialViewInfo>
        {
            /*
            // General
            new StationViewInfo(Name.General, new PackIconModern{ Kind = PackIconModernKind.Box }, Group.General),

            // Applications
            new StationViewInfo(Name.IPScanner, ApplicationViewManager.GetIconByName(ApplicationViewManager.Name.IPScanner), Group.Applications),
            */
        };

        public static string TranslateGroup(Material3dTypes group)
        {
            switch (group)
            {
                case Material3dTypes.Filament:
                    return Strings.Filament;
                case Material3dTypes.Resin:
                    return Strings.Resin;
                case Material3dTypes.Powder:
                    return Strings.Powder;
                case Material3dTypes.Misc:
                    return Strings.Misc;
                default:
                    return Strings.SettingsGroupNotFound;
            }
        }
    }
}
