using AndreasReitberger;
using AndreasReitberger.Enums;
using AndreasReitberger.Models;
using AndreasReitberger.Models.CalculationAdditions;
using AndreasReitberger.Models.MaterialAdditions;
using AndreasReitberger.Models.WorkstepAdditions;
using PrintCostCalculator3d.Enums;
using PrintCostCalculator3d.Models.Exporter;
using PrintCostCalculator3d.Models.Slicer;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

//ADDITIONAL

namespace PrintCostCalculator3d.Models.Settings
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class SettingsInfo : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Variables
        [XmlIgnore] public bool SettingsChanged { get; set; }

        string _settingsVersion = "0.0.0.0";
        public string SettingsVersion
        {
            get => _settingsVersion;
            set
            {
                if (value == _settingsVersion)
                    return;

                _settingsVersion = value;
                SettingsChanged = true;
            }
        }

        #region License
        bool _lic_IsFirstStart = true;
        public bool License_IsFirstStart
        {
            get => _lic_IsFirstStart;
            set
            {
                if (value == _lic_IsFirstStart)
                    return;

                _lic_IsFirstStart = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region EULA
        bool _showEULAOnStartup = true;
        public bool ShowEULAOnStartup
        {
            get => _showEULAOnStartup;
            set
            {
                if (value == _showEULAOnStartup)
                    return;

                _showEULAOnStartup = value;
                SettingsChanged = true;
            }
        }
        bool _agreedEULA = false;
        public bool AgreedEULA
        {
            get => _agreedEULA;
            set
            {
                if (value == _agreedEULA)
                    return;

                _agreedEULA = value;
                SettingsChanged = true;
            }
        }
        
        DateTime _agreedEULAOn;
        public DateTime AgreedEULAOn
        {
            get => _agreedEULAOn;
            set
            {
                if (value == _agreedEULAOn)
                    return;

                _agreedEULAOn = value;
                SettingsChanged = true;
            }
        }

        #endregion

        #region Dashboard

        #region Tabs
        ObservableCollection<DashboardTabContentType> _tabs = new ObservableCollection<DashboardTabContentType>();
        public ObservableCollection<DashboardTabContentType> Tabs
        {
            get => _tabs;
            set
            {
                if (value == _tabs)
                    return;

                _tabs = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #endregion

        #region ThirdParty

        #region HelixViewer
        bool _helix_showCameraInfo = GlobalStaticConfiguration.Helix_ShowCameraInfo;
        public bool Helix_ShowCameraInfo
        {
            get => _helix_showCameraInfo;
            set
            {
                if (_helix_showCameraInfo != value)
                {
                    _helix_showCameraInfo = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        bool _helix_RotateAroundMouseDownPoint = GlobalStaticConfiguration.Helix_RotateArountMouseDownPoint;
        public bool Helix_RotateAroundMouseDownPoint
        {
            get => _helix_RotateAroundMouseDownPoint;
            set
            {
                if (_helix_RotateAroundMouseDownPoint != value)
                {
                    _helix_RotateAroundMouseDownPoint = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }
        #endregion

        #region Gcode

        bool _GcodeParser_PreferValuesInCommentsFromKnownSlicers = true;
        public bool GcodeParser_PreferValuesInCommentsFromKnownSlicers
        {
            get => _GcodeParser_PreferValuesInCommentsFromKnownSlicers;
            set
            {
                if (value == _GcodeParser_PreferValuesInCommentsFromKnownSlicers)
                    return;

                _GcodeParser_PreferValuesInCommentsFromKnownSlicers = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        bool _GcodeViewer_ExpandProfileView = true;
        public bool GcodeViewer_ExpandProfileView
        {
            get => _GcodeViewer_ExpandProfileView;
            set
            {
                if (value == _GcodeViewer_ExpandProfileView)
                    return;

                _GcodeViewer_ExpandProfileView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        double _GcodeViewer_ProfileWidth = GlobalStaticConfiguration.GcodeInfo_DefaultWidthExpanded;
        public double GcodeViewer_ProfileWidth
        {
            get => _GcodeViewer_ProfileWidth;
            set
            {
                if (Math.Abs(value - _GcodeViewer_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _GcodeViewer_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        bool _GcodeInfo_ExpandView = true;
        public bool GcodeInfo_ExpandView
        {
            get => _GcodeInfo_ExpandView;
            set
            {
                if (value == _GcodeInfo_ExpandView)
                    return;

                _GcodeInfo_ExpandView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        double _GcodeInfo_ProfileWidth = GlobalStaticConfiguration.GcodeInfo_DefaultWidthExpanded;
        public double GcodeInfo_ProfileWidth
        {
            get => _GcodeInfo_ProfileWidth;
            set
            {
                if (Math.Abs(value - _GcodeInfo_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _GcodeInfo_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        bool _gcodeMultiParse_ExpandProfileView = true;
        public bool GcodeMultiParse_ExpandView
        {
            get => _gcodeMultiParse_ExpandProfileView;
            set
            {
                if (value == _gcodeMultiParse_ExpandProfileView)
                    return;

                _gcodeMultiParse_ExpandProfileView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        double _gcodeMultiParse_ProfileWidth = GlobalStaticConfiguration.GcodeMultiParser_DefaultWidthExpanded;
        public double GcodeMultiParse_ProfileWidth
        {
            get => _gcodeMultiParse_ProfileWidth;
            set
            {
                if (Math.Abs(value - _gcodeMultiParse_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _gcodeMultiParse_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        bool _AdvancedViewer_ExpandView = true;
        public bool AdvancedViewer_ExpandView
        {
            get => _AdvancedViewer_ExpandView;
            set
            {
                if (value == _AdvancedViewer_ExpandView)
                    return;

                _AdvancedViewer_ExpandView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        double _AdvancedViewer_ProfileWidth = GlobalStaticConfiguration.CalculationView_DefaultWidthExpanded;
        public double AdvancedViewer_ProfileWidth
        {
            get => _AdvancedViewer_ProfileWidth;
            set
            {
                if (Math.Abs(value - _AdvancedViewer_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _AdvancedViewer_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region Slicer
        ObservableCollection<Models.Slicer.Slicer> _slicers = new ObservableCollection<Models.Slicer.Slicer>();
        public ObservableCollection<Models.Slicer.Slicer> Slicers
        {
            get => _slicers;
            set
            {
                if (value == _slicers)
                    return;

                _slicers = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        
        Models.Slicer.Slicer _slicerLastUsed;
        public Models.Slicer.Slicer Slicer_LastUsed
        {
            get => _slicerLastUsed;
            set
            {
                if (value == _slicerLastUsed)
                    return;

                _slicerLastUsed = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        
        ObservableCollection<SlicerCommand> _slicerCommands = new ObservableCollection<SlicerCommand>();
        public ObservableCollection<SlicerCommand> SlicerCommands
        {
            get => _slicerCommands;
            set
            {
                if (value == _slicerCommands)
                    return;

                _slicerCommands = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        #endregion

        #endregion

        #region Printing3dLibrary

        #region Printers
        ObservableCollection<Printer3d> _printers = new ObservableCollection<Printer3d>();
        public ObservableCollection<Printer3d> Printers
        {
            get => _printers;
            set
            {
                if (value == _printers)
                    return;

                _printers = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region Materials

        ObservableCollection<Material3d> _materials = new ObservableCollection<Material3d>();
        public ObservableCollection<Material3d> PrinterMaterials
        {
            get => _materials;
            set
            {
                if (value == _materials)
                    return;

                _materials = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        ObservableCollection<Material3dType> _materialTypes = new ObservableCollection<Material3dType>();
        public ObservableCollection<Material3dType> MaterialTypes
        {
            get => _materialTypes;
            set
            {
                if (value == _materialTypes)
                    return;

                _materialTypes = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        #endregion
        
        #region HourlyMachineRates

        ObservableCollection<HourlyMachineRate> _hourlyMachineRates = new ObservableCollection<HourlyMachineRate>();
        public ObservableCollection<HourlyMachineRate> HourlyMachineRates
        {
            get => _hourlyMachineRates;
            set
            {
                if (value == _hourlyMachineRates)
                    return;

                _hourlyMachineRates = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        #endregion

        #region Worksteps
        ObservableCollection<Workstep> _worksteps = new ObservableCollection<Workstep>();
        public ObservableCollection<Workstep> Worksteps
        {
            get => _worksteps;
            set
            {
                if (value == _worksteps)
                    return;

                _worksteps = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        
        ObservableCollection<WorkstepCategory> _workstepCategories = new ObservableCollection<WorkstepCategory>();
        public ObservableCollection<WorkstepCategory> WorkstepCategories
        {
            get => _workstepCategories;
            set
            {
                if (value == _workstepCategories)
                    return;

                _workstepCategories = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        #endregion

        #region Suppliers and Manufacturers

        ObservableCollection<Manufacturer> _manufacturers = new ObservableCollection<Manufacturer>();
        public ObservableCollection<Manufacturer> Manufacturers
        {
            get => _manufacturers;
            set
            {
                if (value == _manufacturers)
                    return;

                _manufacturers = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        ObservableCollection<Supplier> _suppliers = new ObservableCollection<Supplier>();
        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set
            {
                if (value == _suppliers)
                    return;

                _suppliers = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        #endregion

        #region CustomAttributes
        ObservableCollection<string> _materialAttributes = new ObservableCollection<string>();
        public ObservableCollection<string> MaterialAttributes
        {
            get => _materialAttributes;
            set
            {
                if (_materialAttributes != value)
                {
                    _materialAttributes = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }
        #endregion

        #region Defaults
        ObservableCollection<Printer3d> _calculation_DefaultPrintersLib = new ObservableCollection<Printer3d>();
        public ObservableCollection<Printer3d> Calculation_DefaultPrintersLib
        {
            get => _calculation_DefaultPrintersLib;
            set
            {
                if (_calculation_DefaultPrintersLib != value)
                {
                    _calculation_DefaultPrintersLib = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        ObservableCollection<Material3d> _calculation_DefaultMaterialsLib = new ObservableCollection<Material3d>();
        public ObservableCollection<Material3d> Calculation_DefaultMaterialsLib
        {
            get => _calculation_DefaultMaterialsLib;
            set
            {
                if (_calculation_DefaultMaterialsLib != value)
                {
                    _calculation_DefaultMaterialsLib = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        ObservableCollection<Workstep> _calculation_DefaultWorkstepsLib = new ObservableCollection<Workstep>();
        public ObservableCollection<Workstep> Calculation_DefaultWorkstepsLib
        {
            get => _calculation_DefaultWorkstepsLib;
            set
            {
                if (_calculation_DefaultWorkstepsLib != value)
                {
                    _calculation_DefaultWorkstepsLib = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }
        
        #endregion

        #endregion

        #region 3dPrinting DELETE COLLECTIONS LATER

        ObservableCollection<MachineHourRate> _machineHourRateCalculations = new ObservableCollection<MachineHourRate>();
        public ObservableCollection<MachineHourRate> MachineHourRateCalculations
        {
            get => _machineHourRateCalculations;
            set
            {
                if (value == _machineHourRateCalculations)
                    return;

                _machineHourRateCalculations = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        
        double _energyCosts = 0.25f;
        public double EnergyCosts
        {
            get => _energyCosts;
            set
            {
                if(_energyCosts != value)
                {
                    _energyCosts = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        double _handlingFee = 5.00f;
        public double HandlingFee
        {
            get => _handlingFee;
            set
            {
                if(_handlingFee != value)
                {
                    _handlingFee = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        bool _applyTaxRate = true;
        public bool ApplyTaxRate
        {
            get => _applyTaxRate;
            set
            {
                if (_applyTaxRate != value)
                {
                    _applyTaxRate = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }
        
        bool _applyEnergyCosts = true;
        public bool ApplyEnergyCosts
        {
            get => _applyEnergyCosts;
            set
            {
                if (_applyEnergyCosts != value)
                {
                    _applyEnergyCosts = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        bool _applyProcedureSpecificAdditions = true;
        public bool ApplyProcedureSpecificAdditions
        {
            get => _applyProcedureSpecificAdditions;
            set
            {
                if (_applyProcedureSpecificAdditions != value)
                {
                    _applyProcedureSpecificAdditions = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        int _powerLevel = 50;
        public int PowerLevel
        {
            get => _powerLevel;
            set
            {
                if (_powerLevel != value)
                {
                    _powerLevel = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        double _taxRate = 19;
        public double TaxRate
        {
            get => _taxRate;
            set
            {
                if (_taxRate != value)
                {
                    _taxRate = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }
        #endregion

        #region Calculation
        bool _Calculation_ApplyCustomAdditions = false;
        public bool Calculation_ApplyCustomAdditions
        {
            get => _Calculation_ApplyCustomAdditions;
            set
            {
                if (_Calculation_ApplyCustomAdditions != value)
                {
                    _Calculation_ApplyCustomAdditions = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }
        
        bool _Calculation_ReloadCalculationsOnStartup = false;
        public bool Calculation_ReloadCalculationsOnStartup
        {
            get => _Calculation_ReloadCalculationsOnStartup;
            set
            {
                if (_Calculation_ReloadCalculationsOnStartup != value)
                {
                    _Calculation_ReloadCalculationsOnStartup = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        int _Calculation_SelectedInfoTab = 0;
        public int Calculation_SelectedInfoTab
        {
            get => _Calculation_SelectedInfoTab;
            set
            {
                if (_Calculation_SelectedInfoTab != value)
                {
                    _Calculation_SelectedInfoTab = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        double _Calculation_Margin = 30;
        public double Calculation_Margin
        {
            get => _Calculation_Margin;
            set
            {
                if (_Calculation_Margin != value)
                {
                    _Calculation_Margin = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        bool _ApplyEnhancedMarginSettings = false;
        public bool ApplyEnhancedMarginSettings
        {
            get => _ApplyEnhancedMarginSettings;
            set
            {
                if (_ApplyEnhancedMarginSettings != value)
                {
                    _ApplyEnhancedMarginSettings = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        bool _ExcludeWorkstepsFromMarginCalculation = false;
        public bool ExcludeWorkstepsFromMarginCalculation
        {
            get => _ExcludeWorkstepsFromMarginCalculation;
            set
            {
                if (_ExcludeWorkstepsFromMarginCalculation != value)
                {
                    _ExcludeWorkstepsFromMarginCalculation = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        bool _ExcludePrinterCostsFromMarginCalculation = false;
        public bool ExcludePrinterCostsFromMarginCalculation
        {
            get => _ExcludePrinterCostsFromMarginCalculation;
            set
            {
                if (_ExcludePrinterCostsFromMarginCalculation != value)
                {
                    _ExcludePrinterCostsFromMarginCalculation = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        bool _ExcludeMaterialCostsFromMarginCalculation = false;
        public bool ExcludeMaterialCostsFromMarginCalculation
        {
            get => _ExcludeMaterialCostsFromMarginCalculation;
            set
            {
                if (_ExcludeMaterialCostsFromMarginCalculation != value)
                {
                    _ExcludeMaterialCostsFromMarginCalculation = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }
           
        double _Calculation_FailRate = 5;
        public double Calculation_FailRate
        {
            get => _Calculation_FailRate;
            set
            {
                if (_Calculation_FailRate != value)
                {
                    _Calculation_FailRate = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }
            
        public bool Calculation_ShowGcodeInfos
        {
            get => _Calculation_ShowGcodeInfos;
            set
            {
                if (_Calculation_ShowGcodeInfos != value)
                {
                    _Calculation_ShowGcodeInfos = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }
        bool _Calculation_ShowGcodeInfos = true;

        public bool Calculation_ShowGcodeGrid
        {
            get => _Calculation_ShowGcodeGrid;
            set
            {
                if (_Calculation_ShowGcodeGrid != value)
                {
                    _Calculation_ShowGcodeGrid = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }
        bool _Calculation_ShowGcodeGrid = false;
        
        public bool Calculation_UseVolumeForCalculation
        {
            get => _Calculation_UseVolumeForCalculation;
            set
            {
                if (_Calculation_UseVolumeForCalculation != value)
                {
                    _Calculation_UseVolumeForCalculation = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }
        bool _Calculation_UseVolumeForCalculation = GlobalStaticConfiguration.CalculationView_UseVolumeForCalculation;

        ObservableCollection<CustomAddition> _Calculation_CustomAdditions = new ObservableCollection<CustomAddition>();
        public ObservableCollection<CustomAddition> Calculation_CustomAdditions
        {
            get => _Calculation_CustomAdditions;
            set
            {
                if (_Calculation_CustomAdditions != value)
                {
                    _Calculation_CustomAdditions = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        #endregion

        #region Procedure Additions

        Material3dFamily _calculation_LastProcedure = Material3dFamily.Filament;
        public Material3dFamily Calculation_LastProcedure
        {
            get => _calculation_LastProcedure;
            set
            {
                if (_calculation_LastProcedure != value)
                {
                    _calculation_LastProcedure = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        #region Filament
        bool _calculation_Filament_ApplyNozzleWearCosts = false;
        public bool Calculation_Filament_ApplyNozzleWearCosts
        {
            get => _calculation_Filament_ApplyNozzleWearCosts;
            set
            {
                if (_calculation_Filament_ApplyNozzleWearCosts != value)
                {
                    _calculation_Filament_ApplyNozzleWearCosts = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        double _calculation_Filament_NozzleReplacementCosts = 20;
        public double Calculation_Filament_NozzleReplacementCosts
        {
            get => _calculation_Filament_NozzleReplacementCosts;
            set
            {
                if (_calculation_Filament_NozzleReplacementCosts != value)
                {
                    _calculation_Filament_NozzleReplacementCosts = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        double _calculation_Filament_NozzleWearFactorPerPrintJob = 1;
        public double Calculation_Filament_NozzleWearFactorPerPrintJob
        {
            get => _calculation_Filament_NozzleWearFactorPerPrintJob;
            set
            {
                if (_calculation_Filament_NozzleWearFactorPerPrintJob != value)
                {
                    _calculation_Filament_NozzleWearFactorPerPrintJob = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }
       
        bool _calculation_Filament_ApplyPrintSheetWearCosts = false;
        public bool Calculation_Filament_ApplyPrintSheetWearCosts
        {
            get => _calculation_Filament_ApplyPrintSheetWearCosts;
            set
            {
                if (_calculation_Filament_ApplyPrintSheetWearCosts != value)
                {
                    _calculation_Filament_ApplyPrintSheetWearCosts = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        double _calculation_Filament_PrintSheetReplacementCosts = 40;
        public double Calculation_Filament_PrintSheetReplacementCosts
        {
            get => _calculation_Filament_PrintSheetReplacementCosts;
            set
            {
                if (_calculation_Filament_PrintSheetReplacementCosts != value)
                {
                    _calculation_Filament_PrintSheetReplacementCosts = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        double _calculation_Filament_PrintSheetWearFactorPerPrintJob = 1;
        public double Calculation_Filament_PrintSheetWearFactorPerPrintJob
        {
            get => _calculation_Filament_PrintSheetWearFactorPerPrintJob;
            set
            {
                if (_calculation_Filament_PrintSheetWearFactorPerPrintJob != value)
                {
                    _calculation_Filament_PrintSheetWearFactorPerPrintJob = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }
        #endregion

        #region Resin
        bool _calculation_Resin_ApplyGlovesCosts = false;
        public bool Calculation_Resin_ApplyGlovesCosts
        {
            get => _calculation_Resin_ApplyGlovesCosts;
            set
            {
                if (_calculation_Resin_ApplyGlovesCosts != value)
                {
                    _calculation_Resin_ApplyGlovesCosts = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        int _calculation_Resin_GlovesPerPrintJob = 2;
        public int Calculation_Resin_GlovesPerPrintJob
        {
            get => _calculation_Resin_GlovesPerPrintJob;
            set
            {
                if (_calculation_Resin_GlovesPerPrintJob != value)
                {
                    _calculation_Resin_GlovesPerPrintJob = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        int _calculation_Resin_GlovesInPackage = 100;
        public int Calculation_Resin_GlovesInPackage
        {
            get => _calculation_Resin_GlovesInPackage;
            set
            {
                if (_calculation_Resin_GlovesInPackage != value)
                {
                    _calculation_Resin_GlovesInPackage = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        double _calculation_Resin_GlovesPackagePrice = 30f;
        public double Calculation_Resin_GlovesPackagePrice
        {
            get => _calculation_Resin_GlovesPackagePrice;
            set
            {
                if (_calculation_Resin_GlovesPackagePrice != value)
                {
                    _calculation_Resin_GlovesPackagePrice = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        bool _calculation_Resin_ApplyFilterCosts = false;
        public bool Calculation_Resin_ApplyFilterCosts
        {
            get => _calculation_Resin_ApplyFilterCosts;
            set
            {
                if (_calculation_Resin_ApplyFilterCosts != value)
                {
                    _calculation_Resin_ApplyFilterCosts = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        double _calculation_Resin_FiltersPerPrintJob = 0.25f;
        public double Calculation_Resin_FiltersPerPrintJob
        {
            get => _calculation_Resin_FiltersPerPrintJob;
            set
            {
                if (_calculation_Resin_FiltersPerPrintJob != value)
                {
                    _calculation_Resin_FiltersPerPrintJob = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        int _calculation_ResinFiltersInPackage = 150;
        public int Calculation_Resin_FiltersInPackage
        {
            get => _calculation_ResinFiltersInPackage;
            set
            {
                if (_calculation_ResinFiltersInPackage != value)
                {
                    _calculation_ResinFiltersInPackage = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        double _calculation_Resin_FiltersPackagePrice = 30f;
        public double Calculation_Resin_FiltersPackagePrice
        {
            get => _calculation_Resin_FiltersPackagePrice;
            set
            {
                if (_calculation_Resin_FiltersPackagePrice != value)
                {
                    _calculation_Resin_FiltersPackagePrice = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        bool _calculation_Resin_ApplyWashingCosts = false;
        public bool Calculation_Resin_ApplyWashingCosts
        {
            get => _calculation_Resin_ApplyWashingCosts;
            set
            {
                if (_calculation_Resin_ApplyWashingCosts != value)
                {
                    _calculation_Resin_ApplyWashingCosts = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        double _calculation_Resin_IsopropanolContainerContent = 5f;
        public double Calculation_Resin_IsopropanolContainerContent
        {
            get => _calculation_Resin_IsopropanolContainerContent;
            set
            {
                if (_calculation_Resin_IsopropanolContainerContent != value)
                {
                    _calculation_Resin_IsopropanolContainerContent = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        double _calculation_Resin_IsopropanolContainerPrice = 30f;
        public double Calculation_Resin_IsopropanolContainerPrice
        {
            get => _calculation_Resin_IsopropanolContainerPrice;
            set
            {
                if (_calculation_Resin_IsopropanolContainerPrice != value)
                {
                    _calculation_Resin_IsopropanolContainerPrice = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        double _calculation_Resin_IsopropanolPerPrintJob = 0.25f;
        public double Calculation_Resin_IsopropanolPerPrintJob
        {
            get => _calculation_Resin_IsopropanolPerPrintJob;
            set
            {
                if (_calculation_Resin_IsopropanolPerPrintJob != value)
                {
                    _calculation_Resin_IsopropanolPerPrintJob = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        bool _calculation_Resin_ApplyCuringCosts = false;
        public bool Calculation_Resin_ApplyCuringCosts
        {
            get => _calculation_Resin_ApplyCuringCosts;
            set
            {
                if (_calculation_Resin_ApplyCuringCosts != value)
                {
                    _calculation_Resin_ApplyCuringCosts = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        double _calculation_Resin_CuringDurationInMinutes = 5f;
        public double Calculation_Resin_CuringDurationInMinutes
        {
            get => _calculation_Resin_CuringDurationInMinutes;
            set
            {
                if (_calculation_Resin_CuringDurationInMinutes != value)
                {
                    _calculation_Resin_CuringDurationInMinutes = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        double _calculation_Resin_CuringCostsPerHour = 1f;
        public double Calculation_Resin_CuringCostsPerHour
        {
            get => _calculation_Resin_CuringCostsPerHour;
            set
            {
                if (_calculation_Resin_CuringCostsPerHour != value)
                {
                    _calculation_Resin_CuringCostsPerHour = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }
        #endregion

        #region Powder
        bool _calculation_SLS_ApplyRefreshing = false;
        public bool Calculation_SLS_ApplyRefreshing
        {
            get => _calculation_SLS_ApplyRefreshing;
            set
            {
                if (_calculation_SLS_ApplyRefreshing != value)
                {
                    _calculation_SLS_ApplyRefreshing = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }
        #endregion

        #endregion

        #region EventLogger
        // Overall Logging
        bool _EventLogger_enableLogging = true;
        public bool EventLogger_EnableLogging
        {
            get => _EventLogger_enableLogging;
            set
            {
                if (value == _EventLogger_enableLogging)
                    return;

                _EventLogger_enableLogging = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        // Saved logs
        int _EventLogger_savedLogs = 150;
        public int EventLogger_AmountSavedLogs
        {
            get => _EventLogger_savedLogs;
            set
            {
                if (value == _EventLogger_savedLogs)
                    return;

                _EventLogger_savedLogs = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region General 

        ApplicationName _general_DefaultApplicationViewName = GlobalStaticConfiguration.General_DefaultApplicationViewName;
        public ApplicationName General_DefaultApplicationViewName
        {
            get => _general_DefaultApplicationViewName;
            set
            {
                if (value == _general_DefaultApplicationViewName)
                    return;

                _general_DefaultApplicationViewName = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        int _general_BackgroundJobInterval = GlobalStaticConfiguration.General_BackgroundJobInterval;
        public int General_BackgroundJobInterval
        {
            get => _general_BackgroundJobInterval;
            set
            {
                if (value == _general_BackgroundJobInterval)
                    return;

                _general_BackgroundJobInterval = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        int _general_HistoryListEntries = GlobalStaticConfiguration.General_HistoryListEntries;
        public int General_HistoryListEntries
        {
            get => _general_HistoryListEntries;
            set
            {
                if (value == _general_HistoryListEntries)
                    return;

                _general_HistoryListEntries = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        bool _general_OpenCalculationResultView = true;
        public bool General_OpenCalculationResultView
        {
            get => _general_OpenCalculationResultView;
            set
            {
                if (value == _general_OpenCalculationResultView)
                    return;

                _general_OpenCalculationResultView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        bool _general_SetLoadedCalculationAsSelected = true;
        public bool General_SetLoadedCalculationAsSelected
        {
            get => _general_SetLoadedCalculationAsSelected;
            set
            {
                if (value == _general_SetLoadedCalculationAsSelected)
                    return;

                _general_SetLoadedCalculationAsSelected = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        bool _general_NewCalculationWhenCalculate = false;
        public bool General_NewCalculationWhenCalculate
        {
            get => _general_NewCalculationWhenCalculate;
            set
            {
                if (value == _general_NewCalculationWhenCalculate)
                    return;

                _general_NewCalculationWhenCalculate = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        bool _general_overwriteCurrencySymbol = false;
        public bool General_OverwriteCurrencySymbol
        {
            get => _general_overwriteCurrencySymbol;
            set
            {
                if (value == _general_overwriteCurrencySymbol)
                    return;

                _general_overwriteCurrencySymbol = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        bool _general_overwriteNumberFormats = false;
        public bool General_OverwriteNumberFormats
        {
            get => _general_overwriteNumberFormats;
            set
            {
                if (value == _general_overwriteNumberFormats)
                    return;

                _general_overwriteNumberFormats = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        string _general_CurrencySymbol = string.Empty;
        public string General_CurrencySymbol
        {
            get => _general_CurrencySymbol;
            set
            {
                if (value == _general_CurrencySymbol)
                    return;

                _general_CurrencySymbol = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        string _general_OverwriteCultureCode = string.Empty;
        public string General_OverwriteCultureCode
        {
            get => _general_OverwriteCultureCode;
            set
            {
                if (value == _general_OverwriteCultureCode)
                    return;

                _general_OverwriteCultureCode = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        ObservableCollection<ApplicationViewInfo> _general_ApplicationList = new ObservableCollection<ApplicationViewInfo>();
        public ObservableCollection<ApplicationViewInfo> General_ApplicationList
        {
            get => _general_ApplicationList;
            set
            {
                if (value == _general_ApplicationList)
                    return;

                _general_ApplicationList = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        //SvnRepositiories
        ObservableCollection<object> _repos = new ObservableCollection<object>();
        public ObservableCollection<object> Repos
        {
            get => _repos;
            set
            {
                if (value == _repos)
                    return;

                _repos = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        // Window
        bool _window_ConfirmClose;
        public bool Window_ConfirmClose
        {
            get => _window_ConfirmClose;
            set
            {
                if (value == _window_ConfirmClose)
                    return;

                _window_ConfirmClose = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        bool _window_MinimizeInsteadOfTerminating;
        public bool Window_MinimizeInsteadOfTerminating
        {
            get => _window_MinimizeInsteadOfTerminating;
            set
            {
                if (value == _window_MinimizeInsteadOfTerminating)
                    return;

                _window_MinimizeInsteadOfTerminating = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        bool _window_MultipleInstances;
        public bool Window_MultipleInstances
        {
            get => _window_MultipleInstances;
            set
            {
                if (value == _window_MultipleInstances)
                    return;

                _window_MultipleInstances = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        bool _window_MinimizeToTrayInsteadOfTaskbar;
        public bool Window_MinimizeToTrayInsteadOfTaskbar
        {
            get => _window_MinimizeToTrayInsteadOfTaskbar;
            set
            {
                if (value == _window_MinimizeToTrayInsteadOfTaskbar)
                    return;

                _window_MinimizeToTrayInsteadOfTaskbar = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        // TrayIcon
        bool _trayIcon_AlwaysShowIcon;
        public bool TrayIcon_AlwaysShowIcon
        {
            get => _trayIcon_AlwaysShowIcon;
            set
            {
                if (value == _trayIcon_AlwaysShowIcon)
                    return;

                _trayIcon_AlwaysShowIcon = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        // Appearance
        #region Appearance
        string _appearance_AppTheme;
        public string Appearance_AppTheme
        {
            get => _appearance_AppTheme;
            set
            {
                if (value == _appearance_AppTheme)
                    return;

                _appearance_AppTheme = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        string _appearance_Accent;
        public string Appearance_Accent
        {
            get => _appearance_Accent;
            set
            {
                if (value == _appearance_Accent)
                    return;

                _appearance_Accent = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        bool _appearance_EnableTransparency;
        public bool Appearance_EnableTransparency
        {
            get => _appearance_EnableTransparency;
            set
            {
                if (value == _appearance_EnableTransparency)
                    return;

                _appearance_EnableTransparency = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        double _appearance_Opacity = GlobalStaticConfiguration.Appearance_Opacity;
        public double Appearance_Opacity
        {
            get => _appearance_Opacity;
            set
            {
                if (Math.Abs(value - _appearance_Opacity) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _appearance_Opacity = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        // Localization
        #region Localization
        string _localization_CultureCode;
        public string Localization_CultureCode
        {
            get => _localization_CultureCode;
            set
            {
                if (value == _localization_CultureCode)
                    return;

                _localization_CultureCode = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        // Autostart
        bool _autostart_StartMinimizedInTray;
        public bool Autostart_StartMinimizedInTray
        {
            get => _autostart_StartMinimizedInTray;
            set
            {
                if (value == _autostart_StartMinimizedInTray)
                    return;

                _autostart_StartMinimizedInTray = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }


        // Update
        bool _update_CheckForUpdatesAtStartup = true;
        public bool Update_CheckForUpdatesAtStartup
        {
            get => _update_CheckForUpdatesAtStartup;
            set
            {
                if (value == _update_CheckForUpdatesAtStartup)
                    return;

                _update_CheckForUpdatesAtStartup = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        bool _update_UseNewUpdater = GlobalStaticConfiguration.Update_UseNewUpdater;
        public bool Update_UseNewUpdater
        {
            get => _update_UseNewUpdater;
            set
            {
                if (value == _update_UseNewUpdater)
                    return;

                _update_UseNewUpdater = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        bool _update_IncludeBetaVersions = true;
        public bool Update_IncludeBetaVersions
        {
            get => _update_IncludeBetaVersions;
            set
            {
                if (value == _update_IncludeBetaVersions)
                    return;

                _update_IncludeBetaVersions = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        bool _update_IncludeAlphaVersions = true;
        public bool Update_IncludeAlphaVersions
        {
            get => _update_IncludeAlphaVersions;
            set
            {
                if (value == _update_IncludeAlphaVersions)
                    return;

                _update_IncludeAlphaVersions = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region Exporters
        ObservableCollection<ExporterTemplate> _exporterTemplates = new ObservableCollection<ExporterTemplate>();
        //ObservableCollection<ExporterTemplate> _exporterTemplates = GlobalStaticConfiguration.defaultTemplates;
        public ObservableCollection<ExporterTemplate> ExporterExcel_Templates
        {
            get => _exporterTemplates;
            set
            {
                if (value == _exporterTemplates)
                    return;

                _exporterTemplates = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        string _exporterExcel_TemplatePath = "export_template.xlsx";
        public string ExporterExcel_TemplatePath
        {
            get => _exporterExcel_TemplatePath;
            set
            {
                if (value == _exporterExcel_TemplatePath)
                    return;

                _exporterExcel_TemplatePath = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        
        string _exporterExcel_LastExportPath;
        public string ExporterExcel_LastExportPath
        {
            get => _exporterExcel_LastExportPath;
            set
            {
                if (value == _exporterExcel_LastExportPath)
                    return;

                _exporterExcel_LastExportPath = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        string _exporterExcel_StartColumn;
        public string ExporterExcel_StartColumn
        {
            get => _exporterExcel_StartColumn;
            set
            {
                if (value == _exporterExcel_StartColumn)
                    return;

                _exporterExcel_StartColumn = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        
        string _exporterExcel_StartRow;
        public string ExporterExcel_StartRow
        {
            get => _exporterExcel_StartRow;
            set
            {
                if (value == _exporterExcel_StartRow)
                    return;

                _exporterExcel_StartRow = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        bool _exporterExcel_WriteToTemplate;
        public bool ExporterExcel_WriteToTemplate
        {
            get => _exporterExcel_WriteToTemplate;
            set
            {
                if (value == _exporterExcel_WriteToTemplate)
                    return;

                _exporterExcel_WriteToTemplate = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        
        bool _exporterExcel_LastExportAsPdf = false;
        public bool ExporterExcel_LastExportAsPdf
        {
            get => _exporterExcel_LastExportAsPdf;
            set
            {
                if (value == _exporterExcel_LastExportAsPdf)
                    return;

                _exporterExcel_LastExportAsPdf = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region GcodeParser
        SlicerPrinterConfiguration _gcodeParser_PrinterConfig;
        public SlicerPrinterConfiguration GcodeParser_PrinterConfig
        {
            get => _gcodeParser_PrinterConfig;
            set
            {
                if (value == _gcodeParser_PrinterConfig)
                    return;
                _gcodeParser_PrinterConfig = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        ObservableCollection<SlicerPrinterConfiguration> _gcodeParser_PrinterConfigs = new ObservableCollection<SlicerPrinterConfiguration>();
        public ObservableCollection<SlicerPrinterConfiguration> GcodeParser_PrinterConfigs
        {
            get => _gcodeParser_PrinterConfigs;
            set
            {
                if (value == _gcodeParser_PrinterConfigs)
                    return;
                _gcodeParser_PrinterConfigs = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region Others
        // Application view       
        bool _expandApplicationView;
        public bool ExpandApplicationView
        {
            get => _expandApplicationView;
            set
            {
                if (value == _expandApplicationView)
                    return;

                _expandApplicationView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #endregion

        #region Constructor
        public SettingsInfo()
        {
            // General
            General_ApplicationList.CollectionChanged += CollectionChanged;
        }

        void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SettingsChanged = true;
        }
        #endregion
    }
}
