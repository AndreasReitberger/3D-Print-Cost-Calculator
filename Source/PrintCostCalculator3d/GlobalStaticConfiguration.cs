using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndreasReitberger.Enums;
using AndreasReitberger.Models.MaterialAdditions;
using PrintCostCalculator3d.Models._3dprinting;
using PrintCostCalculator3d.Models.Exporter;
using PrintCostCalculator3d.Resources.Localization;

//ADDITIONAL
using PrintCostCalculator3d.Utilities;

namespace PrintCostCalculator3d
{
    public class GlobalStaticConfiguration
    {
        // Type to search (verage type speed --> 187 chars/min)
        public static TimeSpan SearchDispatcherTimerTimeSpan => new TimeSpan(0, 0, 0, 0, 750);
        public static TimeSpan CredentialsUILockTime => new TimeSpan(0, 0, 120);

        // Filter
        public static string ApplicationFileExtensionFilter => "Application (*.exe)|*.exe";

        // Settings
        public static ApplicationName General_DefaultApplicationViewName => ApplicationName._3dPrintingCalcualtion;
        public static int General_BackgroundJobInterval => 15;
        public static int General_HistoryListEntries => 5;
        public static double Appearance_Opacity => 0.85;

        public static bool Window_AllowMultipleInstances = true;

        // Support
        public static string supportEmail = "kontakt@andreas-reitberger.de";
        public static string documentationUri = "https://andreas-reitberger.de/en/docs/3d-druckkosten-kalkulator/";
        public static string goProUri = "https://andreas-reitberger.de/en/produkt-kategorie/3d-druckkosten-kalkulator/";

        // Materials
        public static ObservableCollection<_3dPrinterMaterialTypes> defaultMaterials = new ObservableCollection<_3dPrinterMaterialTypes>()
        {
            // Filaments
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "PLA"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "ABS"},
            new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "ASA"},
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

        public static ObservableCollection<Material3dType> MaterialTypes = new ObservableCollection<Material3dType>()
        {
            // Filaments
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "PLA"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "ABS"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "ASA"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "PET"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "PETG"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "FLEX"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "HIPS"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "EDGE"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "NGEN"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "NYLON"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "PVA"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "PC"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "PP"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "PEI"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "PEEK"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "PEKK"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "POM"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "PSU"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "PVDF"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "SCAFF"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Filament, "MISC"),

            // Resin
            new Material3dType(Guid.NewGuid(), Material3dFamily.Resin, "TOUGH"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Resin, "CAST"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Resin, "MISC"),

            // Powder
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "Alumide", "PA12-MED(Al)"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "CarbonMide", "PA12-CF"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "EOS PEEK HP3", "PEEK"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "PA 1101", "PA11"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "PA 1102 Black", "PA11"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "PA 2105", "PA12"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "PA 2200 Balance 1.0", "PA12"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "PA 2200 Performance 1.0", "PA12"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "PA 2200 Speed 1.0", "PA12"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "PA 2200 Top Speed 1.0", "PA12"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "PA 2201", "PA12"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "PA 2202 Black", "PA12"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "PA 2210 FR", "PA12 FR"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "PA 2241 FR","PA12"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "PA 2100 GF","PA12-GB"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "PP 1101", "PA12-MED(Al)"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "PrimeCast 101",  "PP"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "PrimePart® PLUS PA 2221",  "PA12"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "PrimePart® ST PEBA 2301",  "TPA"),
            new Material3dType(Guid.NewGuid(), Material3dFamily.Powder, "MISC"),

            // Misc 
            new Material3dType(Guid.NewGuid(), Material3dFamily.Misc, "MISC"),
        };
        public static ObservableCollection<string> MaterialAttributes = new ObservableCollection<string>()
        {
            // Filaments
            Strings.TemperatureNozzle,
            Strings.TemperatureHeatedBed,
            Strings.TemperatureDrying,
            Strings.DurationDrying,

            // Resin
            Strings.DurationCuring,
        };

        // Templates 
        public static ObservableCollection<ExporterTemplate> defaultTemplates = new ObservableCollection<ExporterTemplate>()
        {
            new ExporterTemplate()
            {
                Id = Guid.NewGuid(),
                IsDefault = true,
                Name = Strings.DefaultExcelOfferExporterTemplate,
                TemplatePath = @"Resources\TemplatesExporter\export_template.xlsx",

                ExporterTarget = ExporterTarget.List,
                Settings = new ObservableCollection<ExporterSettings>()
                {
                    new ExporterSettings() {
                        Id = Guid.NewGuid(),
                        Coordinates = new ExcelCoordinates() {Column = "B", Row = 23},
                        Attribute = ExporterAttribute.Attributes.FirstOrDefault(attr => attr.Property == ExporterProperty.CalculationList && attr.Target == ExporterTarget.List),
                        WorkSheetName = "OfferTemplate"
                    }
                }
            },
            new ExporterTemplate()
            {
                Id = Guid.NewGuid(),
                IsDefault = true,
                Name = Strings.DefaultExcelSingleCalculationTemplate,
                TemplatePath = @"Resources\TemplatesExporter\export_single_template.xlsx",
                ExporterTarget = ExporterTarget.Single,
                Settings = new ObservableCollection<ExporterSettings>()
                {
                    new ExporterSettings() {
                        Id = Guid.NewGuid(),
                        Coordinates = new ExcelCoordinates() {Column = "B", Row = 3},
                        Attribute = ExporterAttribute.Attributes.FirstOrDefault(attr => attr.Property == ExporterProperty.CalculationPriceMaterial && attr.Target == ExporterTarget.Single),
                        WorkSheetName = "Data"
                    },
                    new ExporterSettings() {
                        Id = Guid.NewGuid(),
                        Coordinates = new ExcelCoordinates() {Column = "B", Row = 4},
                        Attribute = ExporterAttribute.Attributes.FirstOrDefault(attr => attr.Property == ExporterProperty.CalculationPriceEnergy && attr.Target == ExporterTarget.Single),
                        WorkSheetName = "Data"
                    },
                    new ExporterSettings() {
                        Id = Guid.NewGuid(),
                        Coordinates = new ExcelCoordinates() {Column = "B", Row = 5},
                        Attribute = ExporterAttribute.Attributes.FirstOrDefault(attr => attr.Property == ExporterProperty.CalculationPricePrinter && attr.Target == ExporterTarget.Single),
                        WorkSheetName = "Data"
                    },
                    new ExporterSettings() {
                        Id = Guid.NewGuid(),
                        Coordinates = new ExcelCoordinates() {Column = "B", Row = 6},
                        Attribute = ExporterAttribute.Attributes.FirstOrDefault(attr => attr.Property == ExporterProperty.CalculationPriceWorksteps && attr.Target == ExporterTarget.Single),
                        WorkSheetName = "Data"
                    },
                    new ExporterSettings() {
                        Id = Guid.NewGuid(),
                        Coordinates = new ExcelCoordinates() {Column = "B", Row = 7},
                        Attribute = ExporterAttribute.Attributes.FirstOrDefault(attr => attr.Property == ExporterProperty.CalculationPriceTax && attr.Target == ExporterTarget.Single),
                        WorkSheetName = "Data"
                    },
                    new ExporterSettings() {
                        Id = Guid.NewGuid(),
                        Coordinates = new ExcelCoordinates() {Column = "B", Row = 8},
                        Attribute = ExporterAttribute.Attributes.FirstOrDefault(attr => attr.Property == ExporterProperty.CalculationPriceHandling && attr.Target == ExporterTarget.Single),
                        WorkSheetName = "Data"
                    },
                    new ExporterSettings() {
                        Id = Guid.NewGuid(),
                        Coordinates = new ExcelCoordinates() {Column = "B", Row = 9},
                        Attribute = ExporterAttribute.Attributes.FirstOrDefault(attr => attr.Property == ExporterProperty.CalculationMargin && attr.Target == ExporterTarget.Single),
                        WorkSheetName = "Data"}
                }
            }
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
        // Gcode Multiple file parser
        public static double GcodeMultiParser_WidthCollapsed => 40;
        public static double GcodeMultiParser_DefaultWidthExpanded => 250;
        public static double GcodeMultiParser_MaxWidthExpanded => 450;

        public static string[] Gcode_ValidFileTypes = new string[] { ".gcode", ".gco", ".gc" };
        // Calculation Viewer
        public static double CalculationView_WidthCollapsed => 40;
        public static double CalculationView_DefaultWidthExpanded => 450;
        public static double CalculationView_MaxWidthExpanded => 550;
        public static bool CalculationView_UseVolumeForCalculation => true;
        // Repetier View
        public static double Repetier_WidthCollapsed => 40;
        public static double Repetier_DefaultWidthExpanded => 450;
        public static double Repetier_MaxWidthExpanded => 550;

        public static double RepetierServerPro_Panel_WidthCollapsed => 40;
        public static double RepetierServerPro_Panel_DefaultWidthExpanded => 250;
        public static double RepetierServerPro_Panel_MaxWidthExpanded => 550;

        public static bool RepetierServerPro_Default_ShowFunctions = true;

        // OctoPrint View
        public static double OctoPrint_WidthCollapsed => 40;
        public static double OctoPrint_DefaultWidthExpanded => 450;
        public static double OctoPrint_MaxWidthExpanded => 550;
        
        public static double OctoPrint_Panel_WidthCollapsed => 40;
        public static double OctoPrint_Panel_DefaultWidthExpanded => 250;
        public static double OctoPrint_Panel_MaxWidthExpanded => 550;

        public static bool OctoPrint_Default_ShowFunctions = true;

        public static int OctoPrint_DefaultUpdateInterval = 2;
        public static int OctoPrint_MinUpdateInterval = 1;
        public static int OctoPrint_MaxUpdateInterval = 5;

        // Additional versions
        public static string downloadWebCamVersionUri = "https://drive.google.com/open?id=1d1hiIq_qQ78dAl0CuxT7f0JsJF3UnkWL";
    }
}
