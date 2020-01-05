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
using WpfFramework.Models._3dprinting;

//ADDITIONAL
using WpfFramework.Utilities;

namespace WpfFramework.Models.Settings
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

        #region ThirdParty

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

        #endregion  

        #endregion

        #region 3dPrinting
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


        private ObservableCollection<MachineHourRate> _machineHourRateCalculations = new ObservableCollection<MachineHourRate>();
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
        

        private decimal _energyCosts = 0.25m;
        public decimal EnergyCosts
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

        private decimal _handlingFee = 5.00m;
        public decimal HandlingFee
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

        private decimal _taxRate = 19;
        public decimal TaxRate
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
        private int _Calculation_SelectedInfoTab = 0;
        
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
        #endregion

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


        // General        
        private ApplicationViewManager.Name _general_DefaultApplicationViewName = GlobalStaticConfiguration.General_DefaultApplicationViewName;
        public ApplicationViewManager.Name General_DefaultApplicationViewName
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
