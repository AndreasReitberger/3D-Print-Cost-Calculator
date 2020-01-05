using MahApps.Metro.IconPacks;
using System.Collections.Generic;

namespace WpfFramework
{
    public static class SettingsViewManager
    {
        // List of all applications
        public static List<SettingsViewInfo> List => new List<SettingsViewInfo>
        {
            // General
            new SettingsViewInfo(Name.General, new PackIconModern{ Kind = PackIconModernKind.Box }, Group.General),
            new SettingsViewInfo(Name.Window, new PackIconMaterial { Kind = PackIconMaterialKind.Application }, Group.General),
            new SettingsViewInfo(Name.Appearance, new PackIconMaterial { Kind = PackIconMaterialKind.AutoFix }, Group.General),
            new SettingsViewInfo(Name.Language, new PackIconMaterial { Kind = PackIconMaterialKind.Flag }, Group.General),
            new SettingsViewInfo(Name.Update, new PackIconMaterial { Kind = PackIconMaterialKind.Download }, Group.General),
            //new SettingsViewInfo(Name.ImportExport, new PackIconMaterial { Kind = PackIconMaterialKind.Import }, Group.General),
            new SettingsViewInfo(Name.Settings, new PackIconMaterialLight { Kind = PackIconMaterialLightKind.Cog }, Group.General),

            // Applications
            new SettingsViewInfo(Name.Slicer,new PackIconModern{ Kind = PackIconModernKind.Slice }, Group.Applications),
            new SettingsViewInfo(Name.Gcode,new PackIconModern{ Kind = PackIconModernKind.PageCode }, Group.Applications),

            // Debug
            new SettingsViewInfo(Name.EventLogger,new PackIconMaterial{ Kind = PackIconMaterialKind.BugCheckOutline }, Group.Debug),

        };

        public static string TranslateName(Name name)
        {
            switch (name)
            {
                case Name.General:
                    return Resources.Localization.Strings.SettingsNameGeneral;
                case Name.Window:
                    return Resources.Localization.Strings.SettingsNameWindow;
                case Name.Appearance:
                    return Resources.Localization.Strings.SettingsNameAppearance;
                case Name.Language:
                    return Resources.Localization.Strings.SettingsNameLanguage;
                case Name.Update:
                    return Resources.Localization.Strings.SettingsNameUpdate;
                case Name.Settings:
                    return Resources.Localization.Strings.SettingsNameAppSettings;
                case Name.ImportExport:
                    return Resources.Localization.Strings.SettingsNameImportExport;
                case Name.Slicer:
                    return Resources.Localization.Strings.SettingsNameSlicer;
                case Name.Gcode:
                    return Resources.Localization.Strings.SettingsNameGcode;
                case Name.EventLogger:
                    return Resources.Localization.Strings.SettingsEventLogger;
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
                case Group.Debug:
                    return Resources.Localization.Strings.SettingsGroupDebug;
                default:
                    return Resources.Localization.Strings.SettingsGroupNotFound;
            }
        }

        public enum Name
        {
            General,
            Window,
            Appearance,
            Language,
            Update,
            ImportExport,
            Settings,
            Slicer,
            Gcode,
            EventLogger,

        }

        public enum Group
        {
            General,
            Applications,
            Debug,
        }
    }
}
