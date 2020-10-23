using MahApps.Metro.IconPacks;
using System.Collections.Generic;

namespace PrintCostCalculator3d
{
    public static class SettingsViewManager
    {
        // List of all applications
        public static List<SettingsViewInfo> List => new List<SettingsViewInfo>
        {
            // General
            new SettingsViewInfo(SettingsViewName.General, new PackIconMaterial{ Kind = PackIconMaterialKind.Layers }, Group.General),
            new SettingsViewInfo(SettingsViewName.Window, new PackIconPicolIcons { Kind = PackIconPicolIconsKind.BrowserWindow }, Group.General),
            new SettingsViewInfo(SettingsViewName.Appearance, new PackIconMaterial { Kind = PackIconMaterialKind.Palette }, Group.General),
            new SettingsViewInfo(SettingsViewName.Language, new PackIconMaterial { Kind = PackIconMaterialKind.Translate }, Group.General),
            new SettingsViewInfo(SettingsViewName.Update, new PackIconMaterial { Kind = PackIconMaterialKind.Download }, Group.General),
            //new SettingsViewInfo(Name.ImportExport, new PackIconMaterial { Kind = PackIconMaterialKind.Import }, Group.General),
            new SettingsViewInfo(SettingsViewName.Calculation, new PackIconMaterial { Kind = PackIconMaterialKind.Calculator }, Group.General),
            new SettingsViewInfo(SettingsViewName.Printer, new PackIconMaterial { Kind = PackIconMaterialKind.Printer3d }, Group.General),
            new SettingsViewInfo(SettingsViewName.Settings, new PackIconMaterialLight { Kind = PackIconMaterialLightKind.Cog }, Group.General),

            // Applications
            new SettingsViewInfo(SettingsViewName.Slicer,new PackIconModern{ Kind = PackIconModernKind.Slice }, Group.Applications),
            new SettingsViewInfo(SettingsViewName.Gcode,new PackIconModern{ Kind = PackIconModernKind.PageCode }, Group.Applications),

            // Exporter
            new SettingsViewInfo(SettingsViewName.ExcelExporter,new PackIconModern{ Kind = PackIconModernKind.OfficeExcel }, Group.Exporter),

            // License
            new SettingsViewInfo(SettingsViewName.EULA,new PackIconMaterial{ Kind = PackIconMaterialKind.AccountCheckOutline }, Group.License),

            // Debug
            new SettingsViewInfo(SettingsViewName.EventLogger,new PackIconMaterial{ Kind = PackIconMaterialKind.BugCheckOutline }, Group.Debug),

            // Privacy
            new SettingsViewInfo(SettingsViewName.PrivacyPolicy,new PackIconMaterial{ Kind = PackIconMaterialKind.AccountHeartOutline }, Group.Privacy),
        };

        public static string TranslateName(SettingsViewName name)
        {
            switch (name)
            {
                case SettingsViewName.General:
                    return Resources.Localization.Strings.SettingsNameGeneral;
                case SettingsViewName.Window:
                    return Resources.Localization.Strings.SettingsNameWindow;
                case SettingsViewName.Appearance:
                    return Resources.Localization.Strings.SettingsNameAppearance;
                case SettingsViewName.ExcelExporter:
                    return Resources.Localization.Strings.SettingsNameExcelExporter;
                case SettingsViewName.Language:
                    return Resources.Localization.Strings.SettingsNameLanguage;
                case SettingsViewName.Update:
                    return Resources.Localization.Strings.SettingsNameUpdate;
                case SettingsViewName.Calculation:
                    return Resources.Localization.Strings.SettingsNameCalculation;
                case SettingsViewName.Printer:
                    return Resources.Localization.Strings.SettingsNamePrinter;
                case SettingsViewName.Settings:
                    return Resources.Localization.Strings.SettingsNameAppSettings;
                case SettingsViewName.ImportExport:
                    return Resources.Localization.Strings.SettingsNameImportExport;
                case SettingsViewName.Slicer:
                    return Resources.Localization.Strings.SettingsNameSlicer;
                case SettingsViewName.Gcode:
                    return Resources.Localization.Strings.SettingsNameGcode;
                case SettingsViewName.EULA:
                    return Resources.Localization.Strings.SettingsNameEULA;
                case SettingsViewName.EventLogger:
                    return Resources.Localization.Strings.SettingsEventLogger;
                case SettingsViewName.PrivacyPolicy:
                    return Resources.Localization.Strings.SettingsPrivacyPolicy;
                default:
                    return Resources.Localization.Strings.SettingsNameNotFound;
            }
        }

        public static string TranslateGroup(Group group)
        {
            switch (group)
            {
                case Group.General:
                    return Resources.Localization.Strings.SettingsGroupGeneral;
                    //return Resources.Localization.Strings.General;
                case Group.Applications:
                    return Resources.Localization.Strings.SettingsGroupApps;
                case Group.Exporter:
                    return Resources.Localization.Strings.SettingsGroupApps;
                case Group.License:
                    return Resources.Localization.Strings.SettingsGroupLicense;
                case Group.Debug:
                    return Resources.Localization.Strings.SettingsGroupDebug;
                case Group.Privacy:
                    return Resources.Localization.Strings.SettingsGroupPrivacyPolicy;
                default:
                    return Resources.Localization.Strings.SettingsGroupNotFound;
            }
        }


        public enum Group
        {
            General,
            Applications,
            Exporter,
            License,
            Debug,
            Privacy,
        }
    }
}
