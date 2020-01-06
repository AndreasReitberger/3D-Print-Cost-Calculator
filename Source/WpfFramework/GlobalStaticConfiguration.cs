using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfFramework.Models._3dprinting;

//ADDITIONAL
using WpfFramework.Utilities;

namespace WpfFramework
{
    public class GlobalStaticConfiguration
    {
        // Type to search (verage type speed --> 187 chars/min)
        public static TimeSpan SearchDispatcherTimerTimeSpan => new TimeSpan(0, 0, 0, 0, 750);
        public static TimeSpan CredentialsUILockTime => new TimeSpan(0, 0, 120);

        // Filter
        public static string ApplicationFileExtensionFilter => "Application (*.exe)|*.exe";

        // Settings
        public static ApplicationViewManager.Name General_DefaultApplicationViewName => ApplicationViewManager.Name._3dPrintingCalcualtion;
        public static int General_BackgroundJobInterval => 15;
        public static int General_HistoryListEntries => 5;
        public static double Appearance_Opacity => 0.85;

        public static bool Window_AllowMultipleInstances = true;

        // Support
        public static string supportEmail = "kontakt@andreas-reitberger.de";
        public static string documentationUri = "https://andreas-reitberger.de/kb/3d-druckkosten-kalkulator/";

        // Materials
        public static ObservableCollection<_3dPrinterMaterialTypes> defaultMaterials = new ObservableCollection<_3dPrinterMaterialTypes>()
        {
            // Filaments
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "PLA"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "ABS"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "PET"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "FLEX"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "HIPS"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "EDGE"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "NGEN"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "NYLON"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "PVA"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "PC"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "PP"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "PEI"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "PEEK"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "PEKK"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "POM"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "PSU"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "PVDF"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "SCAFF"},

            // Powder
            //https://eos.materialdatacenter.com/eo/
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "Misc"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "Alumide", Polymer = "PA12-MED(Al)"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "CarbonMide", Polymer = "PA12-CF"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "EOS PEEK HP3", Polymer = "PEEK"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 1101", Polymer = "PA11"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 1102 Black", Polymer = "PA11"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2105", Polymer = "PA12"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2200 Balance 1.0", Polymer = "PA12"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2200 Performance 1.0", Polymer = "PA12"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2200 Speed 1.0", Polymer = "PA12"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2200 Top Quality 1.0", Polymer = "PA12"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2200 Top Speed 1.0", Polymer = "PA12"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2201", Polymer = "PA12"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2202 Black", Polymer = "PA12"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2210 FR", Polymer = "PA12 FR"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2241 FR", Polymer = "PA12"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2100 GF", Polymer = "PA12-GB"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PP 1101", Polymer = "PA12-MED(Al)"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PrimeCast 101", Polymer = "PP"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PrimePart® PLUS PA 2221", Polymer = "PA12"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PrimePart® ST PEBA 2301", Polymer = "TPA"},

            // Resins
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Resin, Material = "Tough"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Resin, Material = "Cast"},

            // Misc
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Misc, Material = "Misc"},
        };

        // EULA
        public static string eulaOnlinePath = "https://andreas-reitberger.de/wp-content/uploads/2015/12/eula-3d-print-cost-calcualtor-english.pdf";
        public static string eulaLocalPath = @"EULA\eula-3d-print-cost-calcualtor-english.txt";

        // Fixes
        public static double FloatPointFix => 1.0;

        // Gcode Viewer
        public static double GcodeInfo_WidthCollapsed => 40;
        public static double GcodeInfo_DefaultWidthExpanded => 250;
        public static double GcodeInfo_MaxWidthExpanded => 450;

        // Additional versions
        public static string downloadWebCamVersionUri = "https://drive.google.com/open?id=1d1hiIq_qQ78dAl0CuxT7f0JsJF3UnkWL";
    }
}
