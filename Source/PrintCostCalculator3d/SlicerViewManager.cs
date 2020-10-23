using System.Collections.Generic;
using PrintCostCalculator3d.Models;
using PrintCostCalculator3d.Resources.Localization;

namespace PrintCostCalculator3d
{
    public static class SlicerViewManager
    {
        // List of all applications
        public static List<SlicerViewInfo> List => new List<SlicerViewInfo>
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
                case Group.GUI:
                    return Strings.GUI;
                case Group.CLI:
                    return Strings.CLI;
                default:
                    return Strings.SettingsGroupNotFound;
            }
        }

        public enum Group
        {
            [LocalizedDescription("GUI", typeof(Strings))]
            GUI,
            [LocalizedDescription("CLI", typeof(Strings))]
            CLI,
        }
    }
}
