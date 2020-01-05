using MahApps.Metro.IconPacks;
using System.Collections.Generic;


namespace WpfFramework
{
    public static class PrinterViewManager
    {
        // List of all applications
        public static List<PrinterViewInfo> List => new List<PrinterViewInfo>
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
                case Group.FDM:
                    return Resources.Localization.Strings.FDM;
                //return Resources.Localization.Strings.General;
                case Group.SLA:
                    return Resources.Localization.Strings.SLA;
                case Group.CLDP:
                    return Resources.Localization.Strings.DLP;
                case Group.DLP:
                    return Resources.Localization.Strings.DLP;
                case Group.MJ:
                    return Resources.Localization.Strings.MJ;
                case Group.NPJ:
                    return Resources.Localization.Strings.NPJ;
                case Group.DOD:
                    return Resources.Localization.Strings.DOD;
                case Group.BJ:
                    return Resources.Localization.Strings.BJ;
                case Group.MJF:
                    return Resources.Localization.Strings.MJF;
                case Group.SLS:
                    return Resources.Localization.Strings.SLS;
                case Group.SLM:
                    return Resources.Localization.Strings.SLM;
                case Group.DMLS:
                    return Resources.Localization.Strings.DMLS;
                case Group.EBM:
                    return Resources.Localization.Strings.EBM;
                case Group.LENS:
                    return Resources.Localization.Strings.LENS;
                case Group.EBAM:
                    return Resources.Localization.Strings.EBAM;
                default:
                    return Resources.Localization.Strings.SettingsGroupNotFound;
            }
        }

        public enum Group
        {
            FDM,
            SLA,
            CLDP,
            DLP,
            MJ,
            NPJ,
            DOD,
            BJ,
            MJF,
            SLS,
            SLM,
            DMLS,
            EBM,
            LENS,
            EBAM,
        }
    }
}
