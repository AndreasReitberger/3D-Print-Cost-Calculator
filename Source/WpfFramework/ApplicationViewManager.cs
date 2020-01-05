using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;

namespace WpfFramework
{
    public static class ApplicationViewManager
    {
        // List of all applications
        public static List<ApplicationViewInfo> GetList()
        {
            var list = new List<ApplicationViewInfo>();

            foreach (Name name in Enum.GetValues(typeof(Name)))
            {
                if (name != Name.None)
                    list.Add(new ApplicationViewInfo(name));
            }

            return list;
        }

        public static string GetTranslatedNameByName(Name name)
        {
            switch (name)
            {
                case Name._3dPrintingMaterial:
                    return Resources.Localization.Strings.AppName3dPrinterMaterialOverview;
                case Name._3dPrintingPrinter:
                    return Resources.Localization.Strings.AppName3dPrinterOverview;
                case Name._3dPrintingCalcualtion:
                    return Resources.Localization.Strings.AppName3dPrintCostCalculator;
                case Name.EventLog:
                    return Resources.Localization.Strings.AppNameEventLog;
                default:
                    return Resources.Localization.Strings.AppNameNotFound;
            }
        }

        public static Canvas GetIconByName(Name name)
        {
            var canvas = new Canvas();

            switch (name)
            {
                case Name._3dPrintingMaterial:
                    canvas.Children.Add(new PackIconModern { Kind = PackIconModernKind.Box });
                    break;
                case Name._3dPrintingPrinter:
                    canvas.Children.Add(new PackIconMaterial { Kind = PackIconMaterialKind.Printer3d });
                    break;
                case Name._3dPrintingCalcualtion:
                    canvas.Children.Add(new PackIconModern { Kind = PackIconModernKind.Calculator });
                    break;
                case Name.EventLog:
                    canvas.Children.Add(new PackIconModern { Kind = PackIconModernKind.DebugStepInto });
                    break;
                default:
                    canvas.Children.Add(new PackIconModern { Kind = PackIconModernKind.SmileyFrown });
                    break;
            }

            return canvas;
        }

        //Application Names
        public enum Name
        {
            None,
            _3dPrintingCalcualtion,
            _3dPrintingPrinter,
            _3dPrintingMaterial,
            EventLog
        }

        public static void InvalidateByName(Name ApplicationName)
        {
            ApplicationViewInfo app = GetList().Find(x => x.Name == ApplicationName);
            if(app != null)
            {

            }
        }
        public static void Invalidate(ApplicationViewInfo AppName)
        {
            ApplicationViewInfo app = GetList().Find(x => x.Name == AppName.Name);
            if(app != null)
            {

            }
        }
    }
}
