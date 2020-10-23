using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;

namespace PrintCostCalculator3d.Models.Settings
{
    public class LocalizationManager
    {

        private const string _defaultCultureCode = "en-US";

        //private const string _baseFlagImageUri = @"pack://application:,,,/WpfFramework;component/Resources/Localization/Flags/";
        private const string _baseFlagImageUri = @"/Resources/Localization/Flags/";


        private static LocalizationManager _instance = null;


        public static LocalizationManager GetInstance(string cultureCode = _defaultCultureCode)
        {
            if (_instance == null)
                _instance = new LocalizationManager(cultureCode);

            return _instance;
        }


        public static Uri GetImageUri(string cultureCode)
        {
            return new Uri(_baseFlagImageUri + cultureCode + ".png", UriKind.Relative);
        }


        public static List<LocalizationInfo> List => new List<LocalizationInfo>
        {
            //https://www.fincher.org/Utilities/CountryLanguageList.shtml
            new LocalizationInfo("English", "English", GetImageUri("en-US"), "Andreas", "en-US",100, true),
            new LocalizationInfo("German", "Deutsch",  GetImageUri("de-DE"), "Andreas", "de-DE",100, true),
            new LocalizationInfo("German - Switzerland", "Deutsch - Schweiz", GetImageUri("de-DE"), "Andreas", "de-CH",100, true),
            new LocalizationInfo("French", "Français", GetImageUri("fr-FR"), "Sébastien", "fr-FR", 100, false),

            new LocalizationInfo("Czech", "český", GetImageUri("cs-CZ"), "", "cs-CZ",0, false),
            new LocalizationInfo("Dutch", "Nederlands",GetImageUri("nl-NL"), "", "nl-NL",0, false),
            new LocalizationInfo("Russian", "Русский", GetImageUri("ru-RU"), "", "ru-RU", 0, false),
            new LocalizationInfo("Spanish", "Español", GetImageUri("es-ES"), "", "es-ES", 0, false),
            new LocalizationInfo("Swedish", "Svenska",GetImageUri("sv-SE"), "", "sv-SE", 0, false),
            new LocalizationInfo("Japanese", "日本人", GetImageUri("ja-JP"), "", "ja-JP", 0, false),
            new LocalizationInfo("Norwegian", "BokmÃ¥l", GetImageUri("nb-NO"), "Allram", "nb-NO", 90, false),
            new LocalizationInfo("Norwegian", "Nynorsk", GetImageUri("nb-NO"), "Allram", "nn-NO", 90, false),
            new LocalizationInfo("Italian", "Italiano", GetImageUri("it-IT"), "Filippo", "it-IT", 95, false),
            new LocalizationInfo("Afrikaans", "Afrikaans", GetImageUri("af-ZA"), "", "af-ZA", 0, false),
            new LocalizationInfo("Hungarian", "Magyar", GetImageUri("hu-HU"), "", "hu-HU", 0, false),
   
        };


        public LocalizationInfo Current { get; private set; } = new LocalizationInfo();

        public CultureInfo Culture { get; private set; }

        private LocalizationManager(string cultureCode = _defaultCultureCode)
        {
            if (string.IsNullOrEmpty(cultureCode))
                cultureCode = CultureInfo.CurrentCulture.Name;

            var info = GetLocalizationInfoBasedOnCode(cultureCode) ?? List.First();

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

        public static LocalizationInfo GetLocalizationInfoBasedOnCode(string cultureCode)
        {
            return List.FirstOrDefault(x => x.Code == cultureCode) ?? null;
        }


        public void Change(LocalizationInfo info)
        {
            Current = info;

            Culture = new CultureInfo(info.Code);
        }
    }
}
