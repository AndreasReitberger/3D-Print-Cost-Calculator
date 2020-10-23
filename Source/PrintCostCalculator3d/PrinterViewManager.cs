using AndreasReitberger.Enums;
using MahApps.Metro.IconPacks;
using System.Collections.Generic;


namespace PrintCostCalculator3d
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

        public static string TranslateGroup(Printer3dType group)
        {
            switch (group)
            {
                case Printer3dType.FDM:
                    return Resources.Localization.Strings.FDM;
                //return Resources.Localization.Strings.General;
                case Printer3dType.SLA:
                    return Resources.Localization.Strings.SLA;
                case Printer3dType.CDLP:
                    return Resources.Localization.Strings.CDLP;
                case Printer3dType.DLP:
                    return Resources.Localization.Strings.DLP;
                case Printer3dType.MJ:
                    return Resources.Localization.Strings.MJ;
                case Printer3dType.NPJ:
                    return Resources.Localization.Strings.NPJ;
                case Printer3dType.DOD:
                    return Resources.Localization.Strings.DOD;
                case Printer3dType.BJ:
                    return Resources.Localization.Strings.BJ;
                case Printer3dType.MJF:
                    return Resources.Localization.Strings.MJF;
                case Printer3dType.SLS:
                    return Resources.Localization.Strings.SLS;
                case Printer3dType.SLM:
                    return Resources.Localization.Strings.SLM;
                case Printer3dType.DMLS:
                    return Resources.Localization.Strings.DMLS;
                case Printer3dType.EBM:
                    return Resources.Localization.Strings.EBM;
                case Printer3dType.LENS:
                    return Resources.Localization.Strings.LENS;
                case Printer3dType.EBAM:
                    return Resources.Localization.Strings.EBAM;
                default:
                    return Resources.Localization.Strings.SettingsGroupNotFound;
            }
        }
    }
}
