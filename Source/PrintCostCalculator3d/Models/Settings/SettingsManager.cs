using log4net;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PrintCostCalculator3d.Models._3dprinting;
using PrintCostCalculator3d.Models.Exporter;
using PrintCostCalculator3d.Resources.Localization;
using AndreasReitberger;
using Assimp.Configs;

namespace PrintCostCalculator3d.Models.Settings
{
    public static class SettingsManager
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string SettingsFolderName = "Settings";
        private const string SettingsFileName = "Settings";
        private const string SettingsFileExtension = "xml";
        private const string IsPortableFileName = "IsPortable";
        private const string IsPortableExtension = "settings";

        public static SettingsInfo Current { get; set; }

        public static bool ForceRestart { get; set; }
        public static bool HotKeysChanged { get; set; }

        private static string GetApplicationName()
        {
            try
            {
                return Assembly.GetEntryAssembly().GetName().Name;
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return string.Empty;
            }
        }

        private static string GetApplicationLocation()
        {
            try
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return string.Empty;
            }
        }

        public static string GetSettingsFileName()
        {
            try
            {
                return $"{SettingsFileName}.{SettingsFileExtension}";
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return string.Empty;
            }
        }

        public static string GetIsPortableFileName()
        {
            try
            {
                return $"{IsPortableFileName}.{IsPortableExtension}";
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return string.Empty;
            }
        }

        #region Settings locations (default, custom, portable)
        public static string GetDefaultSettingsLocation()
        {
            try
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), GetApplicationName(), SettingsFolderName);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return string.Empty;
            }
        }

        public static string GetCustomSettingsLocation()
        {
            return Properties.Settings.Default.Settings_CustomSettingsLocation;
        }

        public static string GetPortableSettingsLocation()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), SettingsFolderName);
        }
        #endregion

        #region File paths
        private static string GetIsPortableFilePath()
        {
            return Path.Combine(GetApplicationLocation(), GetIsPortableFileName());
        }

        public static string GetSettingsFilePath()
        {
            return Path.Combine(GetSettingsLocation(), GetSettingsFileName());
        }
        #endregion

        #region IsPortable, SettingsLocation, SettingsLocationNotPortable
        public static bool GetIsPortable()
        {
            return File.Exists(GetIsPortableFilePath());
        }

        public static string GetSettingsLocation()
        {
            return GetIsPortable() ? GetPortableSettingsLocation() : GetSettingsLocationNotPortable();
        }

        public static string GetSettingsLocationNotPortable()
        {
            var settingsLocation = GetCustomSettingsLocation();

            if (!string.IsNullOrEmpty(settingsLocation) && Directory.Exists(settingsLocation))
                return settingsLocation;

            return GetDefaultSettingsLocation();
        }
        #endregion

        public static void Load()
        {
            try
            {
                if (File.Exists(GetSettingsFilePath()))
                {
                    SettingsInfo settingsInfo;

                    var xmlSerializer = new XmlSerializer(typeof(SettingsInfo));
                    //var xmlSerializer = new XmlSerializer(typeof(ExporterTemplate));

                    using (var fileStream = new FileStream(GetSettingsFilePath(), FileMode.Open))
                    {
                        settingsInfo = (SettingsInfo)xmlSerializer.Deserialize(fileStream);
                    }

                    Current = settingsInfo;

                    // Set the setting changed to false after loading them from a file...
                    Current.SettingsChanged = false;
                }
                else
                {
                    Current = new SettingsInfo();
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                Current = new SettingsInfo();
            }
        }

        public static void Save()
        {
            try
            {
                // Create the directory if it does not exist
                Directory.CreateDirectory(GetSettingsLocation());
                var t = GetSettingsLocation();
                var xmlSerializer = new XmlSerializer(typeof(SettingsInfo));

                using (var fileStream = new FileStream(Path.Combine(GetSettingsFilePath()), FileMode.Create))
                {
                    xmlSerializer.Serialize(fileStream, Current);
                }

                // Set the setting changed to false after saving them as file...
                Current.SettingsChanged = false;
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public static Task MoveSettingsAsync(string sourceLocation, string targedLocation, bool overwrite, string[] filesTargedLocation)
        {
            return Task.Run(() => MoveSettings(sourceLocation, targedLocation, overwrite, filesTargedLocation));
        }

        private static void MoveSettings(string sourceLocation, string targedLocation, bool overwrite, string[] filesTargedLocation = null)
        {
            var sourceFiles = Directory.GetFiles(sourceLocation);

            // Create the dircetory and copy the files to the new location
            Directory.CreateDirectory(targedLocation);

            foreach (var file in sourceFiles)
            {
                // Skip if file exists and user don't want to overwrite it
                if (!overwrite && (filesTargedLocation ?? throw new ArgumentNullException(nameof(filesTargedLocation))).Any(x => Path.GetFileName(x) == Path.GetFileName(file)))
                    continue;

                File.Copy(file, Path.Combine(targedLocation, Path.GetFileName(file)), overwrite);
            }

            // Delete the old files
            foreach (var file in sourceFiles)
                File.Delete(file);

            // Delete the folder, if it is not the default settings locations and does not contain any files or directories
            if (sourceLocation != GetDefaultSettingsLocation() && Directory.GetFiles(sourceLocation).Length == 0 && Directory.GetDirectories(sourceLocation).Length == 0)
                Directory.Delete(sourceLocation);
        }

        public static Task MakePortableAsync(bool isPortable, bool overwrite)
        {
            return Task.Run(() => MakePortable(isPortable, overwrite));
        }

        public static void MakePortable(bool isPortable, bool overwrite)
        {
            if (isPortable)
            {
                MoveSettings(GetSettingsLocationNotPortable(), GetPortableSettingsLocation(), overwrite);

                // After moving the files, set the indicator that the settings are now portable
                File.Create(GetIsPortableFilePath());
            }
            else
            {
                MoveSettings(GetPortableSettingsLocation(), GetSettingsLocationNotPortable(), overwrite);

                // Remove the indicator after moving the files...
                File.Delete(GetIsPortableFilePath());
            }
        }

        public static void InitDefault()
        {
            // Init new Settings with default data
            Current = new SettingsInfo
            {
                SettingsChanged = true
            };
        }

        public static void Reset()
        {
            InitDefault();

            ForceRestart = true;
        }

        public static void Update(Version programmVersion, Version settingsVersion)
        {
            try
            {
                // Version is 0.0.0.0 on first run or settings reset --> skip updates 
                if (settingsVersion > new Version("0.0.0.0"))
                {
                    var reorderApplications = false;
                    if (Current._3dPrinterMaterialTypes.FirstOrDefault(m => m.Kind == _3dprinting._3dPrinterMaterialKind.Misc & m.Material == "Misc") == null)
                    {
                        Current._3dPrinterMaterialTypes.Add(new _3dprinting._3dPrinterMaterialTypes()
                        { Id = Guid.NewGuid(), Kind = _3dprinting._3dPrinterMaterialKind.Misc, Material = "Misc" });
                    }

                    // Features added in 1.0.0.4
                    if (settingsVersion < new Version("1.0.4.0"))
                    {
                        Current.General_ApplicationList.Add(new ApplicationViewInfo(ApplicationName.EventLog));
                        reorderApplications = true;

                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "Misc" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "Alumide", Polymer = "PA12-MED(Al)" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "CarbonMide", Polymer = "PA12-CF" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "EOS PEEK HP3", Polymer = "PEEK" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 1101", Polymer = "PA11" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 1102 Black", Polymer = "PA11" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2105", Polymer = "PA12" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2200 Balance 1.0", Polymer = "PA12" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2200 Performance 1.0", Polymer = "PA12" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2200 Speed 1.0", Polymer = "PA12" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2200 Top Quality 1.0", Polymer = "PA12" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2200 Top Speed 1.0", Polymer = "PA12" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2201", Polymer = "PA12" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2202 Black", Polymer = "PA12" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2210 FR", Polymer = "PA12 FR" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2241 FR", Polymer = "PA12" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PA 2100 GF", Polymer = "PA12-GB" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PP 1101", Polymer = "PA12-MED(Al)" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PrimeCast 101", Polymer = "PP" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PrimePart® PLUS PA 2221", Polymer = "PA12" });
                        Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Powder, Material = "PrimePart® ST PEBA 2301", Polymer = "TPA" });
                    }

                    // Features added in 1.0.0.9
                    if (settingsVersion < new Version("1.0.9.0"))
                    {
                        var t = Current._3dPrinterMaterialTypes.FirstOrDefault(type => type.Kind == _3dPrinterMaterialKind.Filament && type.Material == "ASA");
                        if (t  == null)
                            Current._3dPrinterMaterialTypes.Add(new _3dPrinterMaterialTypes() { Id = Guid.NewGuid(), Kind = _3dPrinterMaterialKind.Filament, Material = "ASA" });
                        reorderApplications = true;

                        Current.ExporterExcel_Templates.Add(
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
                            });
                        Current.ExporterExcel_Templates.Add(
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
                            });

                    }
                    // Features added in 1.1.4.0
                    if (settingsVersion < new Version("1.1.4.0"))
                    {
                        try
                        {
                            FileInfo fileInfo = new FileInfo(GetSettingsFilePath());
                            string newPath = Path.Combine(fileInfo.DirectoryName, "Backup");
                            Backup(newPath);
                            
                        }
                        catch (Exception exc)
                        {
                            logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.Message, exc.TargetSite);
                        }

                        // Migrate old data into new 
                        if(Current.MaterialTypes == null || Current.MaterialTypes.Count == 0)
                        {
                            // Load new material types
                            Current.MaterialTypes = GlobalStaticConfiguration.MaterialTypes;
                        }
                        if(Current.MaterialAttributes == null || Current.MaterialAttributes.Count == 0)
                        {
                            // Load new material types
                            Current.MaterialAttributes = GlobalStaticConfiguration.MaterialAttributes;
                        }

                        if(Current._3dPrinterMaterials.Count > 0)
                        {
                            foreach (var material in Current._3dPrinterMaterials)
                            {
                                try
                                {
                                    // Try to parse existing printers
                                    var mat = new AndreasReitberger.Models.Material3d()
                                    {
                                        Id = material.Id,
                                        Name = material.Name,
                                        SKU = material.SKU,
                                        Uri = material.LinkToReorder,
                                        Density = Convert.ToDouble(material.Density),
                                        UnitPrice = Convert.ToDouble(material.UnitPrice),
                                        
                                        ColorCode = material.ColorCode,
                                        MaterialFamily = (AndreasReitberger.Enums.Material3dFamily)Enum.Parse(typeof(AndreasReitberger.Enums.Material3dFamily), material.TypeOfMaterial.Kind.ToString()),
                                        TypeOfMaterial = Current.MaterialTypes.FirstOrDefault(type => type.Material == material.TypeOfMaterial.Material),
                                        PackageSize = Convert.ToDouble(material.PackageSize),
                                        Unit = (AndreasReitberger.Enums.Unit)Enum.Parse(typeof(AndreasReitberger.Enums.Unit), material.Unit.ToString()),
                                        Attributes = material.TypeOfMaterial.Kind == _3dPrinterMaterialKind.Filament ?
                                            new System.Collections.Generic.List<AndreasReitberger.Models.MaterialAdditions.Material3dAttribute>()
                                            {
                                            new AndreasReitberger.Models.MaterialAdditions.Material3dAttribute() { Attribute = Strings.TemperatureNozzle, Value = material.TemperatureNozzle},
                                            new AndreasReitberger.Models.MaterialAdditions.Material3dAttribute() { Attribute = Strings.TemperatureHeatedBed, Value = material.TemperatureHeatbed},
                                            } :
                                            new System.Collections.Generic.List<AndreasReitberger.Models.MaterialAdditions.Material3dAttribute>()
                                            ,
                                        Manufacturer = material.Manufacturer != null ?
                                            Current.Manufacturers.FirstOrDefault(Manufacturer => Manufacturer.Id == material.Manufacturer.Id) :
                                            new AndreasReitberger.Models.Manufacturer()
                                            ,

                                    };
                                    /*
                                    switch(material.Unit)
                                    {
                                        case UnitOld.g:
                                            mat.Unit = AndreasReitberger.Enums.Unit.g;
                                            break;
                                        case UnitOld.kg:
                                            mat.Unit = AndreasReitberger.Enums.Unit.kg;
                                            break;
                                        case UnitOld.ml:
                                            mat.Unit = AndreasReitberger.Enums.Unit.ml;
                                            break;
                                        case UnitOld.l:
                                            mat.Unit = AndreasReitberger.Enums.Unit.l;
                                            break;
                                    }
                                    */
                                    Current.PrinterMaterials.Add(mat);
                                }
                                catch (Exception exc)
                                {
                                    logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                                    continue;
                                }
                            }
                            // Clear old printer list
                            Current._3dPrinterMaterials.Clear();
                        }
                        if(Current._3dPrinters.Count > 0)
                        {
                            foreach (var printer in Current._3dPrinters)
                            {
                                try
                                {
                                    // Try to parse existing printers
                                    var newPrinter = new AndreasReitberger.Models.Printer3d()
                                    {
                                        Id = printer.Id,
                                        Model = printer.Model,
                                        Price = Convert.ToDouble(printer.Price),
                                        PowerConsumption = Convert.ToDouble(printer.PowerConsumption),
                                        UseFixedMachineHourRating = printer.UseFixedMachineHourRating,
                                        Uri = printer.ShopUri,
                                        Type = (AndreasReitberger.Enums.Printer3dType)Enum.Parse(typeof(AndreasReitberger.Enums.Printer3dType), printer.Type.ToString()),
                                        MaterialType = (AndreasReitberger.Enums.Material3dFamily)Enum.Parse(typeof(AndreasReitberger.Enums.Material3dFamily), printer.Kind.ToString()),
                                        Attributes = printer.Kind == _3dPrinterMaterialKind.Filament ?
                                            new System.Collections.Generic.List<AndreasReitberger.Models.PrinterAdditions.Printer3dAttribute>()
                                            {
                                            new AndreasReitberger.Models.PrinterAdditions.Printer3dAttribute() { Attribute = Strings.TemperatureNozzle, Value = printer.MaxNozzleTemperature},
                                            new AndreasReitberger.Models.PrinterAdditions.Printer3dAttribute() { Attribute = Strings.TemperatureHeatedBed, Value = printer.MaxHeatbedTemperature},
                                            } :
                                            new System.Collections.Generic.List<AndreasReitberger.Models.PrinterAdditions.Printer3dAttribute>()
                                        ,
                                        Manufacturer = printer.Manufacturer != null ?
                                            Current.Manufacturers.FirstOrDefault(Manufacturer => Manufacturer.Id == printer.Manufacturer.Id) :
                                            new AndreasReitberger.Models.Manufacturer()
                                            ,
                                        BuildVolume = printer.BuildVolume != null ?
                                            new AndreasReitberger.Models.PrinterAdditions.BuildVolume(
                                                Convert.ToDouble(printer.BuildVolume.X),
                                                Convert.ToDouble(printer.BuildVolume.Y),
                                                Convert.ToDouble(printer.BuildVolume.Z)) :
                                            new AndreasReitberger.Models.PrinterAdditions.BuildVolume(0, 0, 0)
                                            ,
                                        
                                    };
                                    Current.Printers.Add(newPrinter);
                                }
                                catch (Exception exc)
                                {
                                    logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                                    continue;
                                }
                            }
                            // Clear old printer list
                            Current._3dPrinters.Clear();
                        }
                        if(Current._3dWorksteps.Count > 0)
                        {
                            foreach(var workstep in Current._3dWorksteps)
                            {
                                try
                                {
                                    // Try to parse existing printers
                                    var ws = new AndreasReitberger.Models.Workstep()
                                    {
                                        Id = workstep.Id,
                                        Name = workstep.Name,
                                        Duration = workstep.Duration != null ? workstep.Duration.TotalHours : 0,
                                        Price = Convert.ToDouble(workstep.Price),
                                        Category = workstep.Category != null ? Current.WorkstepCategories.FirstOrDefault(ows => ows.Name == workstep.Category.Name) : null,
                                    };

                                    switch(workstep.CalculationType)
                                    {
                                        case CalculationType.per_hour:
                                            ws.CalculationType = AndreasReitberger.Enums.CalculationType.PerHour;
                                            break;
                                        case CalculationType.per_job:
                                            ws.CalculationType = AndreasReitberger.Enums.CalculationType.PerJob;
                                            break;
                                        case CalculationType.per_piece:
                                            ws.CalculationType = AndreasReitberger.Enums.CalculationType.PerPiece;
                                            break;
                                    };
                                    switch(workstep.Type)
                                    {
                                        case WorkstepType.MISC:
                                            ws.Type = AndreasReitberger.Enums.WorkstepType.Misc;
                                            break;
                                            
                                        case WorkstepType.PRE:
                                            ws.Type = AndreasReitberger.Enums.WorkstepType.Pre;
                                            break;
                                            
                                        case WorkstepType.POST:
                                            ws.Type = AndreasReitberger.Enums.WorkstepType.Post;
                                            break;
                                    }

                                    Current.Worksteps.Add(ws);
                                }
                                catch (Exception exc)
                                {
                                    logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                                    continue;
                                }
                            }
                            Current._3dWorksteps.Clear();
                        }
                    }

                    // Features added in 1.1.5.0
                    if (settingsVersion < new Version("1.1.5.0"))
                    { 
                        if(Current.GcodeParser_PrinterConfigs.Count == 0)
                        {
                            Current.GcodeParser_PrinterConfigs.Add(new Slicer.SlicerPrinterConfiguration()
                            {
                                PrinterName = Strings.Default,
                                AMax_xy = 1000,
                                AMax_z = 1000,
                                AMax_e = 5000,
                                AMax_eExtrude = 1250,
                                AMax_eRetract = 1250,
                                PrintDurationCorrection = 1,
                            });
                        }
                    }
                    // Reorder application view
                    if (reorderApplications)
                    Current.General_ApplicationList = new ObservableCollection<ApplicationViewInfo>(Current.General_ApplicationList.OrderBy(info => info.Name));
                }
                // Set material types if missing
                if (Current.MaterialTypes.Count == 0)
                    Current.MaterialTypes = GlobalStaticConfiguration.MaterialTypes;
                // Update settings version
                Current.SettingsVersion = programmVersion.ToString();
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public static bool Backup(string destinationFolder)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(GetSettingsFilePath());
                if (fileInfo.Exists)
                {
                    string newPath = destinationFolder;
                    if (!Directory.Exists(newPath))
                        Directory.CreateDirectory(newPath);
                    File.Copy(
                        fileInfo.FullName,
                        Path.Combine(newPath, fileInfo.Name),
                        true
                        );
                    logger.InfoFormat(Strings.EventSettingsBackupSucceededFormated, newPath);
                    return true;
                }
                else
                {
                    logger.InfoFormat(Strings.EventSettingsFileNotFoundAtFormated, fileInfo.FullName);
                    return false;
                }
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.Message, exc.TargetSite);
                return false;
            }
        }
    }
}
