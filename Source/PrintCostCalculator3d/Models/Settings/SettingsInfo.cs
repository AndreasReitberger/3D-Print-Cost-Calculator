using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AndreasReitberger;
using AndreasReitberger.Models;
using AndreasReitberger.Models.CalculationAdditions;
using AndreasReitberger.Models.MaterialAdditions;
using AndreasReitberger.Models.WorkstepAdditions;
using PrintCostCalculator3d.Models._3dprinting;
using PrintCostCalculator3d.Models.Exporter;
using PrintCostCalculator3d.Models.Slicer;

//ADDITIONAL
using PrintCostCalculator3d.Utilities;

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

        [XmlIgnore] public bool isLicensed
        {
            get
            {
                return false;
            }
        }

        private string _settingsVersion = "0.0.0.0";
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
        private bool _lic_IsFirstStart = true;
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
        private bool _showEULAOnStartup = true;
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
        private bool _agreedEULA = false;
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
        
        private DateTime _agreedEULAOn;
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

        #region ThirdParty

        #region OctoPrint
        private bool _octoPrint_FirstStart = true;
        public bool OctoPrint_FirstStart
        {
            get => _octoPrint_FirstStart;
            set
            {
                if (value == _octoPrint_FirstStart)
                    return;

                _octoPrint_FirstStart = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _octoPrint_ShowNote = true;
        public bool OctoPrint_ShowNote
        {
            get => _octoPrint_ShowNote;
            set
            {
                if (value == _octoPrint_ShowNote)
                    return;

                _octoPrint_ShowNote = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _octoPrint_Address = string.Empty;
        public string OctoPrint_Address
        {
            get => _octoPrint_Address;
            set
            {
                if (value == _octoPrint_Address)
                    return;

                _octoPrint_Address = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        
        private string _octoPrint_API = string.Empty;
        public string OctoPrint_API
        {
            get => _octoPrint_API;
            set
            {
                if (value == _octoPrint_API)
                    return;

                _octoPrint_API = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _octoPrint_LastUsedPrinter = string.Empty;
        public string OctoPrint_LastUsedPrinter
        {
            get => _octoPrint_LastUsedPrinter;
            set
            {
                if (value == _octoPrint_LastUsedPrinter)
                    return;

                _octoPrint_LastUsedPrinter = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _octoPrint_enableLogging = false;
        public bool OctoPrint_EnableLogging
        {
            get => _octoPrint_enableLogging;
            set
            {
                if (value == _octoPrint_enableLogging)
                    return;

                _octoPrint_enableLogging = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _octoPrint_EnableSecureConnection = false;
        public bool OctoPrint_EnableSecureConnection
        {
            get => _octoPrint_EnableSecureConnection;
            set
            {
                if (value == _octoPrint_EnableSecureConnection)
                    return;

                _octoPrint_EnableSecureConnection = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _octoPrint_ExpandView = true;
        public bool OctoPrint_ExpandView
        {
            get => _octoPrint_ExpandView;
            set
            {
                if (value == _octoPrint_ExpandView) return;

                _octoPrint_ExpandView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private double _octoPrint_ProfileWidth = GlobalStaticConfiguration.OctoPrint_DefaultWidthExpanded;
        public double OctoPrint_ProfileWidth
        {
            get => _octoPrint_ProfileWidth;
            set
            {

                if (Math.Abs(value - _octoPrint_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _octoPrint_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        
        private bool _octoPrint_Panel_ExpandView = true;
        public bool OctoPrint_Panel_ExpandView
        {
            get => _octoPrint_Panel_ExpandView;
            set
            {
                if (value == _octoPrint_Panel_ExpandView) return;

                _octoPrint_Panel_ExpandView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        
        private bool _octoPrint_ShowFunctions = GlobalStaticConfiguration.OctoPrint_Default_ShowFunctions;
        public bool OctoPrint_ShowFunctions
        {
            get => _octoPrint_ShowFunctions;
            set
            {
                if (value == _octoPrint_ShowFunctions) return;

                _octoPrint_ShowFunctions = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private double _octoPrint_Panel_ProfileWidth = GlobalStaticConfiguration.OctoPrint_Panel_DefaultWidthExpanded;
        public double OctoPrint_Panel_ProfileWidth
        {
            get => _octoPrint_Panel_ProfileWidth;
            set
            {

                if (Math.Abs(value - _octoPrint_Panel_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _octoPrint_Panel_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _octoPrint_saveSettingsOnServer = true;
        public bool OctoPrint_SaveSettingsOnServer
        {
            get => _octoPrint_saveSettingsOnServer;
            set
            {
                if (value == _octoPrint_saveSettingsOnServer)
                    return;

                _octoPrint_saveSettingsOnServer = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _octoPrint_AutoConnectOnStartup = true;
        public bool OctoPrint_AutoConnectOnStartup
        {
            get => _octoPrint_AutoConnectOnStartup;
            set
            {
                if (value == _octoPrint_AutoConnectOnStartup)
                    return;

                _octoPrint_AutoConnectOnStartup = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _octoPrint_UpdateInterval = GlobalStaticConfiguration.OctoPrint_DefaultUpdateInterval;
        public int OctoPrint_UpdateInterval
        {
            get => _octoPrint_UpdateInterval;
            set
            {
                if (value == _octoPrint_UpdateInterval)
                    return;

                _octoPrint_UpdateInterval = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        #endregion

        #region RepetierServerPro
        private bool _repetierServerPro_FirstStart = true;
        public bool RepetierServerPro_FirstStart
        {
            get => _repetierServerPro_FirstStart;
            set
            {
                if (value == _repetierServerPro_FirstStart)
                    return;

                _repetierServerPro_FirstStart = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _repetierServerPro_ShowNote = true;
        public bool RepetierServerPro_ShowNote
        {
            get => _repetierServerPro_ShowNote;
            set
            {
                if (value == _repetierServerPro_ShowNote)
                    return;

                _repetierServerPro_ShowNote = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _repetierServerPro_ip = string.Empty;
        public string RepetierServerPro_Ip
        {
            get => _repetierServerPro_ip;
            set
            {
                if (value == _repetierServerPro_ip)
                    return;

                _repetierServerPro_ip = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _repetierServerPro_port = 3344;
        public int RepetierServerPro_Port
        {
            get => _repetierServerPro_port;
            set
            {
                if (value == _repetierServerPro_port)
                    return;

                _repetierServerPro_port = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private int _repetierServerPro_Interval = 2;
        public int RepetierServerPro_Interval
        {
            get => _repetierServerPro_Interval;
            set
            {
                if (value == _repetierServerPro_Interval)
                    return;

                _repetierServerPro_Interval = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private string _repetierServerPro_api = string.Empty;
        public string RepetierServerPro_API
        {
            get => _repetierServerPro_api;
            set
            {
                if (value == _repetierServerPro_api)
                    return;

                _repetierServerPro_api = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        
        private string _repetierServerPro_lastPrinterSlug = string.Empty;
        public string RepetierServerPro_LastPrinterSlug
        {
            get => _repetierServerPro_lastPrinterSlug;
            set
            {
                if (value == _repetierServerPro_lastPrinterSlug)
                    return;

                _repetierServerPro_lastPrinterSlug = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _repetierServerPro_enableLogging = false;
        public bool RepetierServerPro_EnableLogging
        {
            get => _repetierServerPro_enableLogging;
            set
            {
                if (value == _repetierServerPro_enableLogging)
                    return;

                _repetierServerPro_enableLogging = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        
        private bool _repetierServerPro_EnableSecureConnection = false;
        public bool RepetierServerPro_EnableSecureConnection
        {
            get => _repetierServerPro_EnableSecureConnection;
            set
            {
                if (value == _repetierServerPro_EnableSecureConnection)
                    return;

                _repetierServerPro_EnableSecureConnection = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _repetierServerPro_ShowCurrentConnectionInfos = true;
        public bool RepetierServerPro_ShowCurrentConnectionInfos
        {
            get => _repetierServerPro_ShowCurrentConnectionInfos;
            set
            {
                if (value == _repetierServerPro_ShowCurrentConnectionInfos)
                    return;

                _repetierServerPro_ShowCurrentConnectionInfos = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        
        private bool _repetierServerPro_ShowCurrentPrintInfos = true;
        public bool RepetierServerPro_ShowCurrentPrintInfos
        {
            get => _repetierServerPro_ShowCurrentPrintInfos;
            set
            {
                if (value == _repetierServerPro_ShowCurrentPrintInfos)
                    return;

                _repetierServerPro_ShowCurrentPrintInfos = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        
        private bool _repetierServerPro_ShowWebCam = true;
        public bool RepetierServerPro_ShowWebCam
        {
            get => _repetierServerPro_ShowWebCam;
            set
            {
                if (value == _repetierServerPro_ShowWebCam)
                    return;

                _repetierServerPro_ShowWebCam = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _RepetierServerPro_ShowFunctions = GlobalStaticConfiguration.RepetierServerPro_Default_ShowFunctions;
        public bool RepetierServerPro_ShowFunctions
        {
            get => _RepetierServerPro_ShowFunctions;
            set
            {
                if (value == _RepetierServerPro_ShowFunctions)
                    return;

                _RepetierServerPro_ShowFunctions = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _Repetier_ExpandView = true;
        public bool Repetier_ExpandView
        {
            get => _Repetier_ExpandView;
            set
            {
                if (value == _Repetier_ExpandView)
                    return;

                _Repetier_ExpandView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private double _Repetier_ProfileWidth = GlobalStaticConfiguration.Repetier_DefaultWidthExpanded;
        public double Repetier_ProfileWidth
        {
            get => _Repetier_ProfileWidth;
            set
            {
                if (Math.Abs(value - _Repetier_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _Repetier_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private bool _RepetierServerPro_Panel_ExpandView = true;
        public bool RepetierServerPro_Panel_ExpandView
        {
            get => _RepetierServerPro_Panel_ExpandView;
            set
            {
                if (value == _RepetierServerPro_Panel_ExpandView) return;

                _RepetierServerPro_Panel_ExpandView = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private double _RepetierServerPro_Panel_ProfileWidth = GlobalStaticConfiguration.RepetierServerPro_Panel_DefaultWidthExpanded;
        public double RepetierServerPro_Panel_ProfileWidth
        {
            get => _RepetierServerPro_Panel_ProfileWidth;
            set
            {

                if (Math.Abs(value - _RepetierServerPro_Panel_ProfileWidth) < GlobalStaticConfiguration.FloatPointFix)
                    return;

                _RepetierServerPro_Panel_ProfileWidth = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }
        #endregion

        #region Gcode

        private bool _GcodeParser_PreferValuesInCommentsFromKnownSlicers = true;
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

        private bool _GcodeViewer_ExpandProfileView = true;
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

        private double _GcodeViewer_ProfileWidth = GlobalStaticConfiguration.GcodeInfo_DefaultWidthExpanded;
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

        private bool _gcodeMultiParse_ExpandProfileView = true;
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

        private double _gcodeMultiParse_ProfileWidth = GlobalStaticConfiguration.GcodeMultiParser_DefaultWidthExpanded;
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

        private bool _AdvancedViewer_ExpandView = true;
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

        private double _AdvancedViewer_ProfileWidth = GlobalStaticConfiguration.CalculationView_DefaultWidthExpanded;
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
        private ObservableCollection<Models.Slicer.Slicer> _slicers = new ObservableCollection<Models.Slicer.Slicer>();
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
        
        private Models.Slicer.Slicer _slicerLastUsed;
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
        
        private ObservableCollection<SlicerCommand> _slicerCommands = new ObservableCollection<SlicerCommand>();
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
        private ObservableCollection<Printer3d> _printers = new ObservableCollection<Printer3d>();
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

        private ObservableCollection<Material3d> _materials = new ObservableCollection<Material3d>();
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

        private ObservableCollection<Material3dType> _materialTypes = new ObservableCollection<Material3dType>();
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

        private ObservableCollection<HourlyMachineRate> _hourlyMachineRates = new ObservableCollection<HourlyMachineRate>();
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
        private ObservableCollection<Workstep> _worksteps = new ObservableCollection<Workstep>();
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
        
        private ObservableCollection<WorkstepCategory> _workstepCategories = new ObservableCollection<WorkstepCategory>();
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

        private ObservableCollection<Manufacturer> _manufacturers = new ObservableCollection<Manufacturer>();
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

        private ObservableCollection<Supplier> _suppliers = new ObservableCollection<Supplier>();
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
        private ObservableCollection<string> _materialAttributes = new ObservableCollection<string>();
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
        private ObservableCollection<Printer3d> _calculation_DefaultPrintersLib = new ObservableCollection<Printer3d>();
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

        private ObservableCollection<Material3d> _calculation_DefaultMaterialsLib = new ObservableCollection<Material3d>();
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

        private ObservableCollection<Workstep> _calculation_DefaultWorkstepsLib = new ObservableCollection<Workstep>();
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
        private ObservableCollection<_3dPrinterMaterial> _3dMaterials = new ObservableCollection<_3dPrinterMaterial>();
        public ObservableCollection<_3dPrinterMaterial> _3dPrinterMaterials
        {
            get => _3dMaterials;
            set {
                if (value == _3dMaterials)
                    return;

                _3dMaterials = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ObservableCollection<_3dPrinterMaterialTypes> _3dMaterialTypes = new ObservableCollection<_3dPrinterMaterialTypes>();
        public ObservableCollection<_3dPrinterMaterialTypes> _3dPrinterMaterialTypes
        {
            get => _3dMaterialTypes;
            set {
                if (value == _3dMaterialTypes)
                    return;

                _3dMaterialTypes = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ObservableCollection<_3dPrinterModel> _3dprinters = new ObservableCollection<_3dPrinterModel>();
        public ObservableCollection<_3dPrinterModel> _3dPrinters
        {
            get => _3dprinters;
            set
            {
                if (value == _3dprinters)
                    return;

                _3dprinters = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private ObservableCollection<_3dPrinterWorkstep> _3dworksteps = new ObservableCollection<_3dPrinterWorkstep>();
        public ObservableCollection<_3dPrinterWorkstep> _3dWorksteps
        {
            get => _3dworksteps;
            set
            {
                if (value == _3dworksteps)
                    return;

                _3dworksteps = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        private double _energyCosts = 0.25f;
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

        private double _handlingFee = 5.00f;
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

        private bool _showCameraInfo = false;
        public bool ShowCameraInfo
        {
            get => _showCameraInfo;
            set
            {
                if (_showCameraInfo != value)
                {
                    _showCameraInfo = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }

        private bool _applyTaxRate = true;
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
        private bool _applyEnergyCosts = true;
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

        private int _powerLevel = 50;
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

        private double _taxRate = 19;
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
        private bool _Calculation_ApplyCustomAdditions = false;
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
        
        private bool _Calculation_ReloadCalculationsOnStartup = false;
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

        private int _Calculation_SelectedInfoTab = 0;
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

        private double _Calculation_Margin = 30;
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
           
        private double _Calculation_FailRate = 5;
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
        private bool _Calculation_ShowGcodeInfos = true;

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
        private bool _Calculation_ShowGcodeGrid = false;
        
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
        private bool _Calculation_UseVolumeForCalculation = GlobalStaticConfiguration.CalculationView_UseVolumeForCalculation;

        private ObservableCollection<CustomAddition> _Calculation_CustomAdditions = new ObservableCollection<CustomAddition>();
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

        public ObservableCollection<_3dPrinterModel> Calculation_DefaultPrinters
        {
            get => _Calculation_DefaultPrinters;
            set
            {
                if (_Calculation_DefaultPrinters != value)
                {
                    _Calculation_DefaultPrinters = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }
        private ObservableCollection<_3dPrinterModel> _Calculation_DefaultPrinters = new ObservableCollection<_3dPrinterModel>();
        
        public ObservableCollection<_3dPrinterMaterial> Calculation_DefaultMaterials
        {
            get => _Calculation_DefaultMaterials;
            set
            {
                if (_Calculation_DefaultMaterials != value)
                {
                    _Calculation_DefaultMaterials = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }
        private ObservableCollection<_3dPrinterMaterial> _Calculation_DefaultMaterials = new ObservableCollection<_3dPrinterMaterial>();
        
        public ObservableCollection<_3dPrinterWorkstep> Calculation_DefaultWorksteps
        {
            get => _Calculation_DefaultWorksteps;
            set
            {
                if (_Calculation_DefaultWorksteps != value)
                {
                    _Calculation_DefaultWorksteps = value;
                    OnPropertyChanged();
                    SettingsChanged = true;
                }
            }
        }
        private ObservableCollection<_3dPrinterWorkstep> _Calculation_DefaultWorksteps = new ObservableCollection<_3dPrinterWorkstep>();
        
        #endregion

        #region Stock
        private ObservableCollection<MaterialStockItem> _materialstockItems = new ObservableCollection<MaterialStockItem>();
        public ObservableCollection<MaterialStockItem> MaterialStockItems
        {
            get => _materialstockItems;
            set
            {
                if (value == _materialstockItems)
                    return;

                _materialstockItems = value;
                OnPropertyChanged();
                SettingsChanged = true;
            }
        }

        #endregion

        #region EventLogger
        // Overall Logging
        private bool _EventLogger_enableLogging = true;
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
        private int _EventLogger_savedLogs = 150;
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

        private ApplicationName _general_DefaultApplicationViewName = GlobalStaticConfiguration.General_DefaultApplicationViewName;
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

        private int _general_BackgroundJobInterval = GlobalStaticConfiguration.General_BackgroundJobInterval;
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

        private int _general_HistoryListEntries = GlobalStaticConfiguration.General_HistoryListEntries;
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

        private bool _general_OpenCalculationResultView = true;
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

        private bool _general_overwriteCurrencySymbol = false;
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

        private bool _general_overwriteNumberFormats = false;
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

        private string _general_CurrencySymbol = string.Empty;
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

        private string _general_OverwriteCultureCode = string.Empty;
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

        private ObservableCollection<ApplicationViewInfo> _general_ApplicationList = new ObservableCollection<ApplicationViewInfo>();
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
        private ObservableCollection<object> _repos = new ObservableCollection<object>();
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
        private bool _window_ConfirmClose;
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

        private bool _window_MinimizeInsteadOfTerminating;
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

        private bool _window_MultipleInstances;
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

        private bool _window_MinimizeToTrayInsteadOfTaskbar;
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
        private bool _trayIcon_AlwaysShowIcon;
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
        private string _appearance_AppTheme;
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

        private string _appearance_Accent;
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

        private bool _appearance_EnableTransparency;
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

        private double _appearance_Opacity = GlobalStaticConfiguration.Appearance_Opacity;
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
        private string _localization_CultureCode;
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
        private bool _autostart_StartMinimizedInTray;
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
        private bool _update_CheckForUpdatesAtStartup = true;
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
        #endregion

        #region Exporters
        private ObservableCollection<ExporterTemplate> _exporterTemplates = new ObservableCollection<ExporterTemplate>();
        //private ObservableCollection<ExporterTemplate> _exporterTemplates = GlobalStaticConfiguration.defaultTemplates;
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

        private string _exporterExcel_TemplatePath = "export_template.xlsx";
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
        
        private string _exporterExcel_LastExportPath;
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

        private string _exporterExcel_StartColumn;
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
        
        private string _exporterExcel_StartRow;
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

        private bool _exporterExcel_WriteToTemplate;
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
        
        private bool _exporterExcel_LastExportAsPdf = false;
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
        private SlicerPrinterConfiguration _gcodeParser_PrinterConfig;
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

        private ObservableCollection<SlicerPrinterConfiguration> _gcodeParser_PrinterConfigs;
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
        private bool _expandApplicationView;
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

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SettingsChanged = true;
        }
        #endregion
    }
}
