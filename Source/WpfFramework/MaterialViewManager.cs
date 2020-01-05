using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfFramework
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


        public enum Group
        {
            Filament,
            Resin,
        }
    }
}
