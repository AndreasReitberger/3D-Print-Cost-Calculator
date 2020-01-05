using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;

namespace WpfFramework.Models.Settings
{
    public static class LocalizationManager
    {
        #region Variables
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion
        public static List<LocalizationInfo> List => new List<LocalizationInfo>
        {
            //http://docwiki.embarcadero.com/RADStudio/Rio/en/Language_Culture_Names,_Codes,_and_ISO_Values
            new LocalizationInfo("English", "English", new Uri("/Resources/Localization/Flags/en-US.png", UriKind.Relative), "Andreas", "en-US",100, true),
            new LocalizationInfo("German", "Deutsch", new Uri("/Resources/Localization/Flags/de-DE.png", UriKind.Relative), "Andreas", "de-DE",100, true),
            new LocalizationInfo("German - Switzerland", "Deutsch - Schweiz", new Uri("/Resources/Localization/Flags/de-CH.png", UriKind.Relative), "Andreas", "de-CH",100, true),
            new LocalizationInfo("French", "Français", new Uri("/Resources/Localization/Flags/fr-FR.png", UriKind.Relative), "Sébastien", "fr-FR", 90, false), 

            new LocalizationInfo("Czech", "český",new Uri("/Resources/Localization/Flags/cs-CZ.png", UriKind.Relative), "", "cs-CZ",0, false),
            new LocalizationInfo("Dutch", "Nederlands",new Uri("/Resources/Localization/Flags/nl-NL.png", UriKind.Relative), "", "nl-NL",0, false),
            new LocalizationInfo("Russian", "Русский", new Uri("/Resources/Localization/Flags/ru-RU.png", UriKind.Relative), "", "ru-RU", 0, false),
            new LocalizationInfo("Spanish", "Español", new Uri("/Resources/Localization/Flags/es-ES.png", UriKind.Relative), "", "es-ES", 0, false),
            new LocalizationInfo("Swedish", "Svenska", new Uri("/Resources/Localization/Flags/sv-SE.png", UriKind.Relative), "", "sv-SE", 0, false),
            new LocalizationInfo("Japanese", "日本人", new Uri("/Resources/Localization/Flags/ja-JP.png", UriKind.Relative), "", "ja-JP", 0, false),
            
        };

        public static LocalizationInfo Current { get; set; } = new LocalizationInfo();

        public static CultureInfo Culture { get; set; }

        public static void Load()
        {
            // Get the language from the user settings
            var cultureCode = SettingsManager.Current.Localization_CultureCode;

            // If it's empty... detect the windows language
            if (string.IsNullOrEmpty(cultureCode))
                cultureCode = CultureInfo.CurrentCulture.Name;

            // Get the language from the list
            var info = List.FirstOrDefault(x => x.Code == cultureCode) ?? List.First();

            // Change the language if it's different than en-US
            if (info.Code != List.First().Code)
            {
                Change(info);
            }
            else
            {
                Current = info;
                Culture = new CultureInfo(info.Code);
            }
        }

        public static void Change(LocalizationInfo info)
        {
            // Set the current localization
            Current = info;

            // Set the culture code
            Culture = new CultureInfo(info.Code);
        }

        public static string TranslateIPStatus(object value)
        {
            if (!(value is IPStatus ipStatus))
                return "-/-";

            var status = Resources.Localization.Strings.ResourceManager.GetString("IPStatus_" + ipStatus, Culture);

            return string.IsNullOrEmpty(status) ? ipStatus.ToString() : status;
        }

        public static string TranslateTcpState(object value)
        {
            if (!(value is TcpState tcpState))
                return "-/-";

            var status = Resources.Localization.Strings.ResourceManager.GetString("TcpState_" + tcpState, Culture);

            return string.IsNullOrEmpty(status) ? tcpState.ToString() : status;
        }
    }
}
