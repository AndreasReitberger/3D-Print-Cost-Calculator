using System;
using System.Windows.Media.Imaging;


namespace PrintCostCalculator3d.Models.Settings
{
    public class LocalizationInfo
    {

        public string Name { get; set; }


        public string NativeName { get; set; }

        public Uri FlagUri { get; set; }


        public string Translator { get; set; }


        public string Code { get; set; }


        public double PercentTranslated { get; set; }

        public bool IsOfficial { get; set; }


        public LocalizationInfo()
        {

        }

        public LocalizationInfo(string code)
        {
            Code = code;
        }

        public LocalizationInfo(string name, string nativeName, Uri flagUri, string translator, string code, double percentTranslated, bool isOfficial)
        {
            Name = name;
            NativeName = nativeName;
            FlagUri = flagUri;
            Translator = translator;
            Code = code;
            PercentTranslated = percentTranslated;
            IsOfficial = isOfficial;
        }
    }
}
