using AndreasReitberger;
using AndreasReitberger.Enums;
using AndreasReitberger.Models;
using AndreasReitberger.Models.FileAdditions;
using Dragablz;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using MahApps.Metro.SimpleChildWindow;
using PrintCostCalculator3d.Controls;
using PrintCostCalculator3d.Enums;
using PrintCostCalculator3d.Models;
using PrintCostCalculator3d.Models.Events;
using PrintCostCalculator3d.Models.Messaging;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using PrintCostCalculator3d.ViewModels.Slicer;
using PrintCostCalculator3d.Views;
using PrintCostCalculator3d.Views.Dashboard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace PrintCostCalculator3d.ViewModels
{
    class DashboardHostViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        CancellationTokenSource cts;
        readonly SharedCalculatorInstance SharedCalculatorInstance = SharedCalculatorInstance.Instance;
        static readonly object Lock = new();
        #endregion

        #region Fields
        public Grid mainDropGrid;
        #endregion

        #region Properties

        #region ModulState
        bool _isEdit;
        public bool IsEdit
        {
            get => _isEdit;
            set
            {
                if (value == _isEdit)
                    return;

                _isEdit = value;
                OnPropertyChanged();
            }
        }

        bool _isLocked = false;
        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                if (_isLocked == value)
                    return;
                _isLocked = value;
                OnPropertyChanged();
            }
        }

        bool _isBeta = true;
        public bool IsBeta
        {
            get => _isBeta;
            set
            {
                if (_isBeta == value)
                    return;
                _isBeta = value;
                OnPropertyChanged();
            }
        }

        bool _isConnecting = false;
        public bool IsConnecting
        {
            get => _isConnecting;
            set
            {
                if (_isConnecting == value)
                    return;

                _isConnecting = value;
                OnPropertyChanged();
            }
        }

        bool _isOnline = false;
        public bool IsOnline
        {
            get => _isOnline;
            set
            {
                if (_isOnline == value)
                    return;

                _isOnline = value;
                OnPropertyChanged();
            }
        }

        bool _isDragging = false;
        public bool IsDragging
        {
            get => _isDragging;
            set
            {
                if (_isDragging == value)
                    return;

                _isDragging = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Tabs
        public IInterTabClient InterTabClient { get; }
        public IInterLayoutClient InterLayoutClient { get; }
        public ObservableCollection<DragablzTabItem> TabItems { get; }

        int _tabId;

        int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (value == _selectedTabIndex)
                    return;

                _selectedTabIndex = value;
                OnPropertyChanged();
            }
        }

        int _selectedInfoTab = 0;
        public int SelectedInfoTab
        {
            get => _selectedInfoTab;
            set
            {
                if (_selectedInfoTab == value)
                    return;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_SelectedInfoTab = value;
                _selectedInfoTab = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Expander
        bool _expandProfileView;
        public bool ExpandProfileView
        {
            get => _expandProfileView;
            set
            {
                if (value == _expandProfileView)
                    return;

                if (!IsLoading)
                    SettingsManager.Current.AdvancedViewer_ExpandView = value;

                _expandProfileView = value;


                if (_canProfileWidthChange)
                    ResizeProfile(false);

                OnPropertyChanged();
            }
        }

        bool _canProfileWidthChange = true;
        double _tempProfileWidth;

        GridLength _profileWidth;
        public GridLength ProfileWidth
        {
            get => _profileWidth;
            set
            {
                if (value == _profileWidth)
                    return;

                if (!IsLoading && Math.Abs(value.Value - GlobalStaticConfiguration.CalculationView_WidthCollapsed) > GlobalStaticConfiguration.FloatPointFix)
                    // Do not save the size when collapsed
                    SettingsManager.Current.AdvancedViewer_ProfileWidth = value.Value;

                _profileWidth = value;

                if (_canProfileWidthChange)
                    ResizeProfile(true);

                OnPropertyChanged();
            }
        }

        #endregion

        #region Gcode
        Gcode _gcode;
        public Gcode Gcode
        {
            get => _gcode;
            set
            {
                if (_gcode == value) return;
                _gcode = value;
                if (_gcode != SharedCalculatorInstance.Gcode)
                    SharedCalculatorInstance.Gcode = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<Gcode> _gcodes = new();
        public ObservableCollection<Gcode> Gcodes
        {
            get => _gcodes;
            set
            {
                if (_gcodes == value) return;
                _gcodes = value;
                SharedCalculatorInstance.Gcodes = _gcodes;
                OnPropertyChanged();
            }
        }

        IList _selectedGcodeFiles = new ArrayList();
        public IList SelectedGcodeFiles
        {
            get => _selectedGcodeFiles;
            set
            {
                if (Equals(value, _selectedGcodeFiles))
                    return;
                _selectedGcodeFiles = value;
                OnPropertyChanged();
            }
        }

        bool _isSendingGcode = false;
        public bool IsSendingGcode
        {
            get => _isSendingGcode;
            set
            {
                if (_isSendingGcode == value)
                    return;
                _isSendingGcode = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Stl
        Stl _stlFile;
        public Stl StlFile
        {
            get => _stlFile;
            set
            {
                if (_stlFile == value) return;
                _stlFile = value;
                if (_stlFile != SharedCalculatorInstance.StlFile)
                    SharedCalculatorInstance.StlFile = value;
                OnPropertyChanged();
                
            }
        }

        ObservableCollection<Stl> _stlFiles = new();
        public ObservableCollection<Stl> StlFiles
        {
            get => _stlFiles;
            set
            {
                if (_stlFiles == value) return;
                _stlFiles = value;
                SharedCalculatorInstance.StlFiles = _stlFiles;
                OnPropertyChanged();
            }
        }

        IList _selectedStlFiles = new ArrayList();
        public IList SelectedStlFiles
        {
            get => _selectedStlFiles;
            set
            {
                //TotalFileCount = SelectedGcodeFiles.Count + SelectedStlFiles.Count;
                if (Equals(value, _selectedStlFiles))
                    return;

                _selectedStlFiles = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region GcodeParser
        int _progress = 0;
        public int Progress
        {
            get => _progress;
            set
            {
                if (_progress == value)
                    return;
                _progress = value;
                OnPropertyChanged();
            }
        }

        int _progressLayerModel = 0;
        public int ProgressLayerModel
        {
            get => _progressLayerModel;
            set
            {
                if (_progressLayerModel == value)
                    return;
                _progressLayerModel = value;
                OnPropertyChanged();
            }
        }

        bool _isWorking = false;
        public bool IsWorking
        {
            get => _isWorking;
            set
            {
                if (_isWorking == value)
                    return;
                _isWorking = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<Gcode> _gcodesQueue = new();
        public ObservableCollection<Gcode> GcodesQueue
        {
            get => _gcodesQueue;
            set
            {
                if (_gcodesQueue == value) return;
                _gcodesQueue = value;
                OnPropertyChanged();

            }
        }

        ObservableCollection<File3d> _files = new();
        public ObservableCollection<File3d> Files
        {
            get => _files;
            set
            {
                if (_files != value)
                {
                    _files = value;
                    OnPropertyChanged();
                }
            }
        }

        SlicerPrinterConfiguration _printerConfig;
        public SlicerPrinterConfiguration PrinterConfig
        {
            get => _printerConfig;
            set
            {
                if (value == _printerConfig)
                    return;
                if (!IsLoading)
                    SettingsManager.Current.GcodeParser_PrinterConfig = value;
                _printerConfig = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<SlicerPrinterConfiguration> _printerConfigs;
        public ObservableCollection<SlicerPrinterConfiguration> PrinterConfigs
        {
            get => _printerConfigs;
            set
            {
                if (value == _printerConfigs)
                    return;
                if (!IsLoading)
                    SettingsManager.Current.GcodeParser_PrinterConfigs = value;
                _printerConfigs = value;
                OnPropertyChanged();
            }
        }

        float _aMax_xy = 1000;
        public float AMax_xy
        {
            get => _aMax_xy;
            set
            {
                if (value == _aMax_xy)
                    return;
                _aMax_xy = value;
                OnPropertyChanged();
            }
        }

        float _aMax_z = 1000;
        public float AMax_z
        {
            get => _aMax_z;
            set
            {
                if (value == _aMax_z)
                    return;
                _aMax_z = value;
                OnPropertyChanged();
            }
        }

        float _aMax_e = 5000;
        public float AMax_e
        {
            get => _aMax_e;
            set
            {
                if (value == _aMax_e)
                    return;
                _aMax_e = value;
                OnPropertyChanged();
            }
        }

        float _aMax_eExtrude = 1250;
        public float AMax_eExtrude
        {
            get => _aMax_eExtrude;
            set
            {
                if (value == _aMax_eExtrude)
                    return;
                _aMax_eExtrude = value;
                OnPropertyChanged();
            }
        }

        float _aMax_eRetract = 1250;
        public float AMax_eRetract
        {
            get => _aMax_eRetract;
            set
            {
                if (value == _aMax_eRetract)
                    return;
                _aMax_eRetract = value;
                OnPropertyChanged();
            }
        }

        float _printDurationCorrection = 1;
        public float PrintDurationCorrection
        {
            get => _printDurationCorrection;
            set
            {
                if (value == _printDurationCorrection)
                    return;
                _printDurationCorrection = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Calculation
        Calculation3d _calculation;
        public Calculation3d Calculation
        {
            get => _calculation;
            set
            {
                if (_calculation == value) return;
                _calculation = value;
                if (_calculation != SharedCalculatorInstance.Calculation)
                    SharedCalculatorInstance.Calculation = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<Calculation3d> _calculations = new();
        public ObservableCollection<Calculation3d> Calculations
        {
            get => _calculations;
            set
            {
                if (_calculations == value) return;
                _calculations = value;
                SharedCalculatorInstance.Calculations = _calculations;
                OnPropertyChanged();

            }
        }

        public double TotalPrice
        {
            get
            {
                return Calculations.Sum(calc => calc.TotalCosts);
            }
        }
        public double TotalPrintTime
        {
            get
            {
                return Calculations.Sum(calc => calc.TotalPrintTime);
            }
        }

        bool _allCalcsSelected = false;
        public bool AllCalcsSelected
        {
            get => _allCalcsSelected;
            set
            {
                if (_allCalcsSelected == value)
                    return;
                _allCalcsSelected = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region CalculationViews
        public ICollectionView CalculationViews
        {
            get => _CalculationViews;
            set
            {
                if (_CalculationViews != value)
                {
                    _CalculationViews = value;
                    OnPropertyChanged();
                }
            }
        }
        ICollectionView _CalculationViews;

        CalculationViewInfo _selectedCalculationView;
        public CalculationViewInfo SelectedCalculationView
        {
            get => _selectedCalculationView;
            set
            {
                if (_selectedCalculationView != value)
                {
                    _selectedCalculationView = value;
                    OnPropertyChanged();
                }
            }
        }

        IList _selectedCalculationsView = new ArrayList();
        public IList SelectedCalculationsView
        {
            get => _selectedCalculationsView;
            set
            {
                if (Equals(value, _selectedCalculationsView))
                    return;

                _selectedCalculationsView = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion

        #region PrintServer
        public bool CanBeSaved
        {
            get => Calculation != null;
        }
        public bool CanSendGcode
        {
            get => false;
        }

        #endregion

        #region Session
        ObservableCollection<DashboardTabContentType> _tabs = new();
        public ObservableCollection<DashboardTabContentType> Tabs
        {
            get => _tabs;
            set
            {
                if (_tabs == value)
                    return;
                if (!IsLoading)
                    SettingsManager.Current.Tabs = value;
                _tabs = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Search
        string _searchCalculation = string.Empty;
        public string SearchCalculation
        {
            get => _searchCalculation;
            set
            {
                if (_searchCalculation != value)
                {
                    _searchCalculation = value;

                    CalculationViews.Refresh();

                    ICollectionView view = CollectionViewSource.GetDefaultView(CalculationViews);
                    IEqualityComparer<String> comparer = StringComparer.InvariantCultureIgnoreCase;
                    view.Filter = o =>
                    {
                        CalculationViewInfo p = o as CalculationViewInfo;
                        string[] patterns = _searchCalculation.ToLower().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (patterns.Length == 1 || patterns.Length == 0)
                            return p.Name.ToLower().Contains(_searchCalculation.ToLower());
                        else
                        {
                            return patterns.Any(p.Name.ToLower().Contains) || patterns.Any(p.Calculation.Name.ToLower().Contains);
                        }
                    };
                    OnPropertyChanged();
                }
            }
        }

        string _searchStlFiles = string.Empty;
        public string SearchStlFiles
        {
            get => _searchStlFiles;
            set
            {
                if (_searchStlFiles != value)
                {
                    _searchStlFiles = value;

                    ICollectionView view = CollectionViewSource.GetDefaultView(StlFiles);
                    IEqualityComparer<String> comparer = StringComparer.InvariantCultureIgnoreCase;
                    view.Filter = o =>
                    {
                        Stl m = o as Stl;
                        return m.FileName.Contains(_searchStlFiles) || m.StlFilePath.Contains(_searchStlFiles) || Regex.IsMatch(m.FileName, _searchGCodeFiles, RegexOptions.IgnoreCase);
                    };

                    OnPropertyChanged();
                }
            }
        }

        string _searchGCodeFiles = string.Empty;
        public string SearchGCodeFiles
        {
            get => _searchGCodeFiles;
            set
            {
                if (_searchGCodeFiles != value)
                {
                    _searchGCodeFiles = value;

                    ICollectionView view = CollectionViewSource.GetDefaultView(Gcodes);
                    IEqualityComparer<String> comparer = StringComparer.InvariantCultureIgnoreCase;
                    view.Filter = o =>
                    {
                        Gcode gc = o as Gcode;
                        return gc.FileName.Contains(_searchGCodeFiles) || gc.FilePath.Contains(_searchGCodeFiles) || Regex.IsMatch(gc.FileName, _searchGCodeFiles, RegexOptions.IgnoreCase);
                    };
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Settings
        bool _reloadCalculationsOnStartup = true;
        public bool ReloadCalculationsOnStartup
        {
            get => _reloadCalculationsOnStartup;
            set
            {
                if (_reloadCalculationsOnStartup == value)
                    return;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_ReloadCalculationsOnStartup = value;
                _reloadCalculationsOnStartup = value;
                OnPropertyChanged();
            }
        }

        bool _setLoadedCalculationAsSelected = true;
        public bool SetLoadedCalculationAsSelected
        {
            get => _setLoadedCalculationAsSelected;
            set
            {
                if (_setLoadedCalculationAsSelected == value) return;
                _setLoadedCalculationAsSelected = value;
                if (!IsLoading)
                    SettingsManager.Current.General_SetLoadedCalculationAsSelected = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor, LoadSettings
        public DashboardHostViewModel(IDialogCoordinator dialogCoordinator)
        {
            try
            {
                IsLoading = true;
                LoadSettings();
                IsLoading = false;

                IsLicenseValid = false;

                SharedCalculatorInstance.AutoBackupOnChange = true;
                SharedCalculatorInstance.BackupPath = Path.Combine(SettingsManager.GetSettingsLocation(), "calculations.xml");

                SharedCalculatorInstance.OnCalculationsChanged += SharedCalculatorInstance_OnCalculationsChanged;
                SharedCalculatorInstance.OnSelectedCalculationChanged += SharedCalculatorInstance_OnSelectedCalculationChanged;

                SharedCalculatorInstance.OnGcodesChanged += SharedCalculatorInstance_OnGcodesChanged;
                SharedCalculatorInstance.OnSelectedGcodeChanged += SharedCalculatorInstance_OnSelectedGcodeChanged;

                SharedCalculatorInstance.OnStlsChanged += SharedCalculatorInstance_OnStlsChanged;
                SharedCalculatorInstance.OnSelectedStlChanged += SharedCalculatorInstance_OnSelectedStlChanged;

                LoadDefaults();
                RegisterMessages();

                this._dialogCoordinator = dialogCoordinator;
                InterTabClient = new DragablzInterTabClient(ApplicationName.Dashboard);
                InterLayoutClient = new DragablzInterLayoutClient(ApplicationName.Dashboard);
                //InterLayoutClient = new DefaultInterLayoutClient();

                if(Tabs == null || Tabs.Count == 0)
                    TabItems = new ObservableCollection<DragablzTabItem>
                    {
                        new DragablzTabItem(Strings.Calculator, new DashboardCalculatorView(DashboardTabContentType.Calculator), _tabId),
                        new DragablzTabItem(Strings.STLViewer, new DashboardStlViewerView(DashboardTabContentType.StlViewer), _tabId),
                        new DragablzTabItem(Strings.GcodeViewer, new DashboardGcodeViewerView(DashboardTabContentType.GcodeViewer), _tabId),
                    };
                else
                {
                    TabItems = new ObservableCollection<DragablzTabItem>();
                    foreach(var type in Tabs)
                    {
                        switch (type)
                        {
                            case DashboardTabContentType.Calculator:
                                TabItems.Add(
                                    new DragablzTabItem(Strings.Calculator, new DashboardCalculatorView(DashboardTabContentType.Calculator), _tabId)
                                    );
                                break;
                            case DashboardTabContentType.StlViewer:
                                TabItems.Add(
                                    new DragablzTabItem(Strings.STLViewer, new DashboardStlViewerView(DashboardTabContentType.StlViewer), _tabId)
                                    );
                                break;
                            case DashboardTabContentType.GcodeViewer:
                                TabItems.Add(
                                    new DragablzTabItem(Strings.GcodeViewer, new DashboardGcodeViewerView(DashboardTabContentType.GcodeViewer), _tabId)
                                    );
                                break;
                            case DashboardTabContentType.GcodeEditor:
                                TabItems.Add(
                                    new DragablzTabItem(Strings.GcodeCodeLineEditor, new DashboardGcodeEditorView(DashboardTabContentType.GcodeEditor), _tabId)
                                    );
                                break;
                            default:
                                break;
                        }
                    }
                }
                Tabs.CollectionChanged += Tabs_CollectionChanged;
                TabItems.CollectionChanged += TabItems_CollectionChanged;
                if (TabItems.Count > 0)
                {
                    foreach(DragablzTabItem tab in TabItems)
                    {
                        switch (tab.View)
                        {
                            case DashboardCalculatorView c1:
                                c1.OnViewVisible();
                                break;
                            case DashboardGcodeEditorView c2:
                                c2.OnViewVisible();
                                break;
                            case DashboardGcodeViewerView c3:
                                c3.OnViewVisible();
                                break;
                            case DashboardStlViewerView c4:
                                c4.OnViewVisible();
                                break;
                            default:
                                break;
                        }
                    }
                }

                Calculation = SharedCalculatorInstance.Calculation;
                Calculations.CollectionChanged += Calculations_CollectionChanged;

                Gcode = SharedCalculatorInstance.Gcode;
                Gcodes.CollectionChanged += Gcodes_CollectionChanged;

                StlFile = SharedCalculatorInstance.StlFile;
                StlFiles.CollectionChanged += StlFiles_CollectionChanged;
                if (ReloadCalculationsOnStartup)
                {
                    try
                    {
                        string filePath = Path.Combine(SettingsManager.GetSettingsLocation(), "calculations.xml");
                        SharedCalculatorInstance.LoadLastSessionCalculations(filePath);
                    }
                    catch (Exception exc)
                    {
                        logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                    }
                }

                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
            }
        }

        void TabItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsLoading) return;
            try
            {
                Tabs.Clear();
                foreach (var tab in TabItems)
                {
                    switch (tab.View)
                    {
                        case DashboardCalculatorView c1:
                            Tabs.Add(DashboardTabContentType.Calculator);
                            break;
                        case DashboardGcodeEditorView c2:
                            Tabs.Add(DashboardTabContentType.GcodeEditor);
                            break;
                        case DashboardGcodeViewerView c3:
                            Tabs.Add(DashboardTabContentType.GcodeViewer);
                            break;
                        case DashboardStlViewerView c4:
                            Tabs.Add(DashboardTabContentType.StlViewer);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch(Exception exc)
            {
                logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
            }
        }

        void Tabs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SettingsManager.Current.Tabs = Tabs;
        }

        void LoadDefaults()
        {
            PrinterConfigs = SettingsManager.Current.GcodeParser_PrinterConfigs;
            PrinterConfig = SettingsManager.Current.GcodeParser_PrinterConfig;
        }

        void LoadSettings()
        {
            Tabs = new ObservableCollection<DashboardTabContentType>(SettingsManager.Current.Tabs.Distinct());
            SelectedInfoTab = SettingsManager.Current.Calculation_SelectedInfoTab;

            SetLoadedCalculationAsSelected = SettingsManager.Current.General_SetLoadedCalculationAsSelected;
            ReloadCalculationsOnStartup = SettingsManager.Current.Calculation_ReloadCalculationsOnStartup;

            ProfileWidth = ExpandProfileView ? 
                new GridLength(SettingsManager.Current.AdvancedViewer_ProfileWidth) : 
                new GridLength(GlobalStaticConfiguration.CalculationView_WidthCollapsed);
            _tempProfileWidth = SettingsManager.Current.AdvancedViewer_ProfileWidth;

            ExpandProfileView = SettingsManager.Current.AdvancedViewer_ExpandView;
        }

        void RegisterMessages()
        {
            Messenger.Default.Register<GcodesChangedMessage>(this, GcodesChangedAction);
            Messenger.Default.Register<CalculationsChangedMessage>(this, CalculationsChangedAction);
            Messenger.Default.Register<SettingsChangedEventArgs>(this, SettingsChangedAction);
        }

        #endregion

        #region Messages
        void SettingsChangedAction(SettingsChangedEventArgs msg)
        {
            if(msg != null)
            {
                IsLoading = true;
                LoadSettings();
                IsLoading = false;
            }
        }
        void GcodesChangedAction(GcodesChangedMessage msg)
        {
            if(msg != null)
            {
                if (msg.Gcode == null) return;
                switch (msg.Action)
                {
                    case MessagingAction.Add:

                        break;
                    case MessagingAction.Remove:
                        var gcode = Gcodes.FirstOrDefault(gc => gc.Id == msg.Gcode.Id);
                        if(gcode != null)
                        {
                            Gcodes.Remove(gcode);
                        }
                        break;
                    case MessagingAction.Invalidate:
                        break;
                    default:
                        break;
                }
            }
        }
        void CalculationsChangedAction(CalculationsChangedMessage msg)
        {
            if(msg != null)
            {
                if (msg.Calculation == null) return;
                switch (msg.Action)
                {
                    case MessagingAction.Add:
                        if (!Calculations.Contains(msg.Calculation))
                            Calculations.Add(msg.Calculation);
                        break;
                    case MessagingAction.Remove:
                        var calculation = Calculations.FirstOrDefault(calc => calc.Id == msg.Calculation.Id);
                        if(calculation != null)
                        {
                            Calculations.Remove(calculation);
                        }
                        break;
                    case MessagingAction.Invalidate:
                        CreateCalculationViewInfos();
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region Events
        void OnProgressUpdateAction(int progress)
        {
            Progress = progress;
        }
        
        void Calculations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Calculations));
            OnPropertyChanged(nameof(TotalPrice));
            OnPropertyChanged(nameof(TotalPrintTime));
            CreateCalculationViewInfos();
            
            try
            {
                if(e != null)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach(var calc in e.NewItems)
                            {
                                Calculation3d newItem = (Calculation3d)calc;
                                if (newItem == null)
                                    continue;
                                SharedCalculatorInstance.AddCalculation(newItem);
                                //Messenger.Default.Send(new CalculationsChangedMessage() { Calculation = newItem, Action = MessagingAction.Add });
                            }
                            
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            foreach (var calc in e.OldItems)
                            {
                                Calculation3d newItem = (Calculation3d)calc;
                                if (newItem == null)
                                    continue;
                                SharedCalculatorInstance.RemoveCalculation(newItem);
                                //Messenger.Default.Send(new CalculationsChangedMessage() { Calculation = newItem, Action = MessagingAction.Remove });
                            }
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            break;
                        case NotifyCollectionChangedAction.Move:
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            SharedCalculatorInstance.Calculations.Clear();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
            
        }

        void Gcodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Gcodes));
            try
            {
                if (e != null)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach (var item in e.NewItems)
                            {
                                Gcode newItem = (Gcode)item;
                                if (newItem == null)
                                    continue;
                                SharedCalculatorInstance.AddGcode(newItem);
                                //Messenger.Default.Send(new GcodesChangedMessage() { Gcode = newItem, Action = MessagingAction.Add });
                            }

                            break;
                        case NotifyCollectionChangedAction.Remove:
                            foreach (var item in e.OldItems)
                            {
                                Gcode newItem = (Gcode)item;
                                if (newItem == null)
                                    continue;
                                SharedCalculatorInstance.RemoveGcode(newItem);
                                //Messenger.Default.Send(new GcodesChangedMessage() { Gcode = newItem, Action = MessagingAction.Remove });
                            }
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            break;
                        case NotifyCollectionChangedAction.Move:
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            SharedCalculatorInstance.Gcodes.Clear();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        void StlFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(StlFiles));
            try
            {
                if (e != null)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach (var item in e.NewItems)
                            {
                                Stl newItem = (Stl)item;
                                if (newItem == null)
                                    continue;
                                SharedCalculatorInstance.AddStl(newItem);
                                //Messenger.Default.Send(new GcodesChangedMessage() { Gcode = newItem, Action = MessagingAction.Add });
                            }

                            break;
                        case NotifyCollectionChangedAction.Remove:
                            foreach (var item in e.OldItems)
                            {
                                Stl newItem = (Stl)item;
                                if (newItem == null)
                                    continue;
                                SharedCalculatorInstance.RemoveStl(newItem);
                                //Messenger.Default.Send(new GcodesChangedMessage() { Gcode = newItem, Action = MessagingAction.Remove });
                            }
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            break;
                        case NotifyCollectionChangedAction.Move:
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            SharedCalculatorInstance.StlFiles.Clear();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        #region SharedInstance

        void SharedCalculatorInstance_OnSelectedCalculationChanged(object sender, Models.Events.CalculationChangedEventArgs e)
        {
            if (e != null)
            {
                Calculation = e.NewCalculation;
            }
        }
        void SharedCalculatorInstance_OnCalculationsChanged(object sender, Models.Events.CalculationsChangedEventArgs e)
        {
            if (e != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewItems != null)
                        {
                            foreach (Calculation3d calc in e.NewItems)
                            {
                                // Check if item has not been added already
                                var item = Calculations.FirstOrDefault(c => c.Id == calc.Id);
                                if (item != null) continue;
                                Calculations.Add(calc);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        if (e.OldItems != null)
                        {
                            foreach (Calculation3d calc in e.OldItems)
                            {
                                var item = Calculations.FirstOrDefault(c => c.Id == calc.Id);
                                if (item == null) continue;
                                Calculations.Remove(item);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        Calculations.Clear();
                        break;
                    default:
                        break;
                }
            }
        }

        void SharedCalculatorInstance_OnSelectedGcodeChanged(object sender, Models.Events.GcodeChangedEventArgs e)
        {
            if (e != null)
            {
                Gcode = e.NewGcode;
            }
        }
        void SharedCalculatorInstance_OnGcodesChanged(object sender, Models.Events.GcodesChangedEventArgs e)
        {
            if (e != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewItems != null)
                        {
                            foreach (Gcode gc in e.NewItems)
                            {
                                // Check if item has not been added already
                                var item = Gcodes.FirstOrDefault(c => c.Id == gc.Id);
                                if (item != null) continue;
                                    Gcodes.Add(gc);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        if (e.OldItems != null)
                        {
                            foreach (Gcode gc in e.OldItems)
                            {
                                var item = Gcodes.FirstOrDefault(c => c.Id == gc.Id);
                                if (item == null) continue;
                                    Gcodes.Remove(item);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        Gcodes.Clear();
                        break;
                    default:
                        break;
                }
            }
        }

        void SharedCalculatorInstance_OnSelectedStlChanged(object sender, StlChangedEventArgs e)
        {
            if (e != null)
            {
                StlFile = e.NewStl;
            }
        }
        void SharedCalculatorInstance_OnStlsChanged(object sender, StlsChangedEventArgs e)
        {
            if (e != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewItems != null)
                        {
                            foreach (Stl file in e.NewItems)
                            {
                                // Check if item has not been added already
                                var item = StlFiles.FirstOrDefault(c => c.Id == file.Id);
                                if (item != null) continue;
                                StlFiles.Add(file);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        if (e.OldItems != null)
                        {
                            foreach (Stl file in e.OldItems)
                            {
                                var item = StlFiles.FirstOrDefault(f => f.Id == file.Id);
                                if (item == null) continue;
                                StlFiles.Remove(item);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        StlFiles.Clear();
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #endregion

        #region Commands
        public ICommand GoProCommand
        {
            get => new RelayCommand(p => GoProAction());
        }
        async void GoProAction()
        {
            try
            {
                string uri = GlobalStaticConfiguration.goProUri;
                if (!string.IsNullOrEmpty(uri))
                {
                    var res = await this._dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogGoProHeadline,
                        Strings.DialogGoProContent,
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        Process.Start(uri);
                        logger.Info(string.Format(Strings.EventOpenUri, uri));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        #region FileHandling
        public ICommand ReadStlGcodeFileCommand
        {
            get => new RelayCommand(async (p) => await ReadStlGcodeFileAction());
        }
        async Task ReadStlGcodeFileAction()
        {
            try
            {
                var openFileDialog = new System.Windows.Forms.OpenFileDialog
                {
                    Filter = StaticStrings.FilterGCodeSTLCalculationFile,
                };

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var filename = openFileDialog.FileName;
                    var res = await ReadFileInformation(filename);
                    if (res)
                    {
                        logger.Info(string.Format(Strings.EventFileLoadedForamated, filename));
                    }
                    else
                    {
                        logger.Warn(string.Format(Strings.EventCouldNotReadFileInformation, filename));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }

        }

        public ICommand OnDropStlFileCommand
        {
            get => new RelayCommand(async (p) => await OnDropStlFileAction(p));
        }
        async Task OnDropStlFileAction(object p)
        {
            try
            {
                if (p is DragEventArgs args)
                {
                    IsDragging = false;
                    if (args.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        string[] files = (string[])args.Data.GetData(DataFormats.FileDrop);
                        if (files.Count() > 0)
                        {
                            foreach (string file in files)
                            {
                                try
                                {
                                    await ReadFileInformation(file);
                                    logger.Info(string.Format(Strings.EventFileLoadedForamated, file));
                                }
                                catch (Exception exc)
                                {
                                    logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                                }

                            }
                            await this._dialogCoordinator.ShowMessageAsync(this,
                                Strings.DialogFileLoadedHeadline,
                                Strings.DialogFileLoadedContent
                                );
                        }
                    }
                    args.Handled = true;
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }


        public ICommand OnDragOverCommand
        {
            get => new RelayCommand((p) => OnDragOverAction(p));
        }
        void OnDragOverAction(object p)
        {
            try
            {
                if (p is DragEventArgs args)
                {
                    if (mainDropGrid == null) return;
                    var ptTargetClient = args.GetPosition(mainDropGrid);

                    if (ptTargetClient.X >= 0 && ptTargetClient.X <= mainDropGrid.ActualWidth
                        && ptTargetClient.Y >= 0 && ptTargetClient.Y <= mainDropGrid.ActualHeight)
                    {
                        args.Handled = true;
                        args.Effects = DragDropEffects.Move;
                    }

                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
        public ICommand OnDragEnterCommand
        {
            get => new RelayCommand((p) => OnDragEnterAction(p));
        }
        void OnDragEnterAction(object p)
        {
            try
            {
                if (p is DragEventArgs args)
                {
                    IsDragging = true;
                    args.Effects = DragDropEffects.Move;
                    args.Handled = true;
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        public ICommand OnDragLeaveCommand
        {
            get => new RelayCommand((p) => OnDragLeaveAction(p));
        }
        void OnDragLeaveAction(object p)
        {
            try
            {
                if (p is DragEventArgs args)
                {
                    IsDragging = false;
                    args.Effects = DragDropEffects.None;
                    args.Handled = true;
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand ParseMultipleGcodesCommand
        {
            get => new RelayCommand(async (p) => await ParseMultipleGcodesAction());
        }
        async Task ParseMultipleGcodesAction()
        {
            try
            {
                if (PrinterConfig == null)
                {
                    var res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogSlicerPrinterConfigIsMissingHeadline,
                        Strings.DialogSlicerPrinterConfigIsMissingContent,
                        MessageDialogStyle.AffirmativeAndNegative,
                        new MetroDialogSettings()
                        {
                            AffirmativeButtonText = Strings.GoToSettings,
                            NegativeButtonText = Strings.Cancel,
                        });
                    if (res == MessageDialogResult.Affirmative)
                    {
                        EventSystem.RedirectToSettings(SettingsViewName.Gcode);
                    }
                    return;
                }
                else
                {
                    ObservableCollection<Gcode> _files = new();
                    System.Windows.Forms.OpenFileDialog openFileDialog = new()
                    {
                        Filter = StaticStrings.FilterGCodeFile,
                        Multiselect = true,
                    };
                    cts = new CancellationTokenSource();
                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        // Split lists in small sizes
                        Progress<int> overallProg = new(percent => OnProgressUpdateAction(percent));
                        List<List<string>> filesList = CollectionHelper.Split(openFileDialog.FileNames.ToList(), 4).ToList();
                        await ParseGcodeFiles(filesList, overallProg);
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }

        }

        public ICommand CancelGcodeAnalyseCommand
        {
            get => new RelayCommand((p) => CancelGcodeAnalyseAction());
        }
        void CancelGcodeAnalyseAction()
        {
            try
            {
                if (cts != null)
                {
                    cts.Cancel();
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        #endregion

        #region GoToSettings
        public ICommand OpenSettingsCommand
        {
            get { return new RelayCommand(p => OpenSettingsAction()); }
        }

        static void OpenSettingsAction()
        {
            EventSystem.RedirectToSettings(SettingsViewName.General);
        }
        #endregion

        #region Settings
        public ICommand ShowQuickSettingsCommand
        {
            get { return new RelayCommand(async (p) => await ShowQuickSettingsAction()); }
        }
        async Task ShowQuickSettingsAction()
        {
            try
            {
                var _childWindow = new ChildWindow()
                {
                    Title = Strings.Settings,
                    AllowMove = true,
                    ShowCloseButton = false,
                    CloseByEscape = false,
                    IsModal = true,
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (Color)Application.Current.Resources["MahApps.Colors.Gray2"] },
                    Padding = new Thickness(50),
                };
                var viewModel = new QuickSettingsDialogViewModel(instance =>
                {
                    _childWindow.Close();
                    Messenger.Default.Send(new SettingsChangedEventArgs());
                    //LoadSettings();
                }, instance =>
                {
                    _childWindow.Close();
                    Messenger.Default.Send(new SettingsChangedEventArgs());
                    //LoadSettings();
                },
                    _dialogCoordinator
                );

                _childWindow.Content = new QuickSettingsDialogView()
                {
                    DataContext = viewModel
                };
                await ChildWindowManager.ShowChildWindowAsync(Application.Current.MainWindow, _childWindow);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }

        }
        #endregion

        #region TabControl
        public ICommand AddTabCommand
        {
            get { return new RelayCommand(async(p) => await AddTabAction()); }
        }

        async Task AddTabAction()
        {
            try
            {
                _tabId++;

                var _dialog = new CustomDialog() { Title = Strings.NewTab };
                var viewModel = new DashboardSelectTabContentDialogViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    DragablzTabItem item = TabItems.FirstOrDefault(i => i.Header == instance.TabContent.ToString());
                    if (item == null)
                    {
                        UserControl view = null;
                        switch (instance.TabContent)
                        {
                            case DashboardTabContentType.Calculator:
                                view = new DashboardCalculatorView(instance.TabContent);
                                if(view != null)
                                {
                                    ((DashboardCalculatorView)view).OnViewVisible();
                                }
                                break;
                            case DashboardTabContentType.StlViewer:
                                view = new DashboardStlViewerView(instance.TabContent);
                                if (view != null)
                                {
                                    ((DashboardStlViewerView)view).OnViewVisible();
                                }
                                break;
                            case DashboardTabContentType.GcodeViewer:
                                view = new DashboardGcodeViewerView(instance.TabContent);
                                if (view != null)
                                {
                                    ((DashboardGcodeViewerView)view).OnViewVisible();
                                }
                                break;
                            case DashboardTabContentType.GcodeEditor:
                                view = new DashboardGcodeEditorView(instance.TabContent);
                                if (view != null)
                                {
                                    ((DashboardGcodeEditorView)view).OnViewVisible();
                                }
                                break;
                            default:
                                view = new DashboardCalculatorView(instance.TabContent);
                                if (view != null)
                                {
                                    ((DashboardCalculatorView)view).OnViewVisible();
                                }
                                break;
                        }
                        if (view != null)
                            TabItems.Add(new DragablzTabItem(GetLocalizedTabName(instance.TabContent), view, _tabId));
                        SelectedTabIndex = TabItems.Count - 1;
                    }
                    else
                        SelectedTabIndex = TabItems.IndexOf(item);

                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                }
                , _dialogCoordinator
                );

                _dialog.Content = new DashboardSelectTabDialogView()
                {
                    DataContext = viewModel
                };
                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
            }
        }

        public ItemActionCallback CloseItemCommand => CloseItemAction;

        static void CloseItemAction(ItemActionCallbackArgs<TabablzControl> args)
        {
            try
            {
                //((args.DragablzItem.Content as DragablzTabItem)?.View as DashboardHostViewModel)?.CloseTab();
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
            }
        }

        #endregion

        #region Helix
        Stl CreateStlModel(string StlFilePath)
        {
            try
            {
                return new(StlFilePath);
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return new Stl();
            }
        }
        #endregion

        #region Expander

        #region Stl
        public ICommand SliceSelectedStlFilesCommand
        {
            get { return new RelayCommand(async (p) => await SliceSelectedStlFilesAction()); }
        }
        async Task SliceSelectedStlFilesAction()
        {
            try
            {
                ObservableCollection<Stl> _files = new(SelectedStlFiles.Cast<Stl>().ToList());

                CustomDialog _dialog = new() { Title = Strings.PrepareSlicing };
                _dialog.Style = Application.Current.FindResource("FullWidthDialogStyle") as Style;

                SliceStlDialogViewModel newSlicerDialogViewModel = new(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    if (instance.SlicerName.Group == SlicerViewManager.Group.CLI)
                    {
                        ObservableCollection<string> files = instance.FilesForImport;
                        if (files.Count > 0)
                        {
                            MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                                 Strings.DialogFilesToImportHeadline,
                                 string.Format(Strings.DialogFilesToImportFormatedContent, files.Count)
                                 );
                            if (res == MessageDialogResult.Affirmative)
                            {
                                foreach (string file in files)
                                {
                                    //var result = await readFileInformation(file);
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                            Strings.DialogImportedSlicedGcodeFilesHeadline,
                            Strings.DialogImportedSlicedGcodeFilesContent,
                            MessageDialogStyle.AffirmativeAndNegative);
                        if (res == MessageDialogResult.Affirmative)
                        {
                            System.Windows.Forms.OpenFileDialog openFileDialog = new()
                            {
                                Filter = StaticStrings.FilterGCodeFile,
                            };

                            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                foreach (string file in openFileDialog.FileNames)
                                {

                                    bool result = true;//await readFileInformation(file);
                                    if (result)
                                    {
                                        logger.Info(string.Format(Strings.EventFileLoadedForamated, file));
                                    }
                                    else
                                    {
                                        logger.Warn(string.Format(Strings.EventCouldNotReadFileInformation, file));
                                    }
                                }

                            }
                        }
                    }
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                _dialogCoordinator,
                _files
                );

                _dialog.Content = new Views.SlicerViews.SliceStlFilesDialog()
                {
                    DataContext = newSlicerDialogViewModel
                };
                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand SliceSelectedStlFilesChildWindowCommand
        {
            get { return new RelayCommand(async (p) => await SliceSelectedStlFilesChildWindowAction()); }
        }
        async Task SliceSelectedStlFilesChildWindowAction()
        {
            try
            {
                ObservableCollection<Stl> _files = new(SelectedStlFiles.Cast<Stl>().ToList());

                ChildWindow _childWindow = new()
                {
                    Title = Strings.PrepareSlicing,
                    AllowMove = true,
                    ShowCloseButton = false,
                    CloseByEscape = false,
                    IsModal = true,
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (System.Windows.Media.Color)Application.Current.Resources["MahApps.Colors.Gray2"] },
                    Padding = new Thickness(50),
                };
                SliceStlDialogViewModel newSlicerDialogViewModel = new(async instance =>
                {
                    _ = _childWindow.Close();
                    if (instance.SlicerName.Group == SlicerViewManager.Group.CLI)
                    {
                        ObservableCollection<string> files = instance.FilesForImport;
                        if (files.Count > 0)
                        {
                            MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                                 Strings.DialogFilesToImportHeadline,
                                 string.Format(Strings.DialogFilesToImportFormatedContent, files.Count),
                                 MessageDialogStyle.AffirmativeAndNegative
                                 );
                            if (res == MessageDialogResult.Affirmative)
                            {
                                foreach (string file in files)
                                {
                                    //var result = await readFileInformation(file);
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                            Strings.DialogImportedSlicedGcodeFilesHeadline,
                            Strings.DialogImportedSlicedGcodeFilesContent,
                            MessageDialogStyle.AffirmativeAndNegative);
                        if (res == MessageDialogResult.Affirmative)
                        {
                            System.Windows.Forms.OpenFileDialog openFileDialog = new()
                            {
                                Filter = StaticStrings.FilterGCodeFile,
                            };

                            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                foreach (string file in openFileDialog.FileNames)
                                {
                                    bool result = true; //await readFileInformation(file);
                                    if (result)
                                    {
                                        logger.Info(string.Format(Strings.EventFileLoadedForamated, file));
                                    }
                                    else
                                    {
                                        logger.Warn(string.Format(Strings.EventCouldNotReadFileInformation, file));
                                    }
                                }

                            }
                        }
                    }
                }, instance =>
                {
                    _childWindow.Close();
                },
                    this._dialogCoordinator,
                    _files
                );

                _childWindow.Content = new Views.SlicerViews.SliceStlFilesDialog()
                {
                    DataContext = newSlicerDialogViewModel,
                    Width = Application.Current.MainWindow.ActualWidth * 0.8,
                    Height = Application.Current.MainWindow.ActualHeight * 0.8,
                };
                await ChildWindowManager.ShowChildWindowAsync(Application.Current.MainWindow, _childWindow);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogExceptionHeadline,
                        string.Format(Strings.DialogExceptionFormatedContent, exc.Message)
                        );
            }
        }
        
        public ICommand OpenStlFileLocationCommand
        {
            get { return new RelayCommand(async (p) => await OpenStlFileLocationAction()); }
        }
        async Task OpenStlFileLocationAction()
        {
            try
            {
                if (SelectedStlFiles.Count == 0)
                    return;

                else if (SelectedStlFiles.Count == 1)
                {
                    var path = Path.GetDirectoryName(StlFile.StlFilePath);
                    Process.Start(path);
                    logger.InfoFormat(Strings.EventOpenFolderFormated, path);
                }
                else
                {
                    var res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogOpenMultipleFileLocationsHeadline,
                        Strings.DialogOpenMultipleFileLocationsContent
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        foreach (Stl file in SelectedStlFiles)
                        {
                            var path = Path.GetDirectoryName(file.StlFilePath);
                            Process.Start(path);
                            logger.InfoFormat(Strings.EventOpenFolderFormated, path);
                        }
                        OnPropertyChanged(nameof(StlFiles));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogExceptionHeadline,
                        string.Format(Strings.DialogExceptionFormatedContent, exc.Message)
                        );
            }
        }

        public ICommand DeleteSelectedStlFilesCommand
        {
            get { return new RelayCommand(async (p) => await DeleteSelectedStlFilesAction()); }
        }
        async Task DeleteSelectedStlFilesAction()
        {
            try
            {
                MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                    Strings.DialogRemoveSelectedFilesFromListHeadline,
                    Strings.DialogRemoveSelectedFilesFromListContent,
                    MessageDialogStyle.AffirmativeAndNegative
                    );
                if (res == MessageDialogResult.Affirmative)
                {
                    try
                    {
                        IList collection = new ArrayList(SelectedStlFiles);
                        for (int i = 0; i < collection.Count; i++)
                        {
                            if (collection[i] is not Stl file)
                            {
                                continue;
                            }
                            logger.Info(string.Format(Strings.EventDeletedItemFormated, file.FileName));
                            _ = StlFiles.Remove(file);
                        }
                    }
                    catch (Exception exc)
                    {
                        logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        #endregion

        #region Gcode
        public ICommand OpenGCodeFileLocationCommand
        {
            get { return new RelayCommand(async (p) => await OpenGCodeFileLocationAction()); }
        }
        async Task OpenGCodeFileLocationAction()
        {
            try
            {
                if (SelectedGcodeFiles.Count == 0)
                    return;

                else if (SelectedGcodeFiles.Count == 1)
                {
                    var path = Path.GetDirectoryName(Gcode.FilePath);
                    Process.Start(path);
                    logger.InfoFormat(Strings.EventOpenFolderFormated, path);
                }
                else
                {
                    var res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogOpenMultipleFileLocationsHeadline,
                        Strings.DialogOpenMultipleFileLocationsContent
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        foreach (Gcode file in SelectedGcodeFiles)
                        {
                            var path = Path.GetDirectoryName(file.FilePath);
                            Process.Start(path);
                            logger.InfoFormat(Strings.EventOpenFolderFormated, path);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogExceptionHeadline,
                        string.Format(Strings.DialogExceptionFormatedContent, exc.Message)
                        );
            }
        }

        public ICommand OpenGCodeViewerCommand
        {
            get { return new RelayCommand(async (p) => await OpenGCodeViewerAction()); }
        }
        async Task OpenGCodeViewerAction()
        {
            try
            {
                var _childWindow = new ChildWindow()
                {
                    Title = Strings.GCodeInfo,
                    AllowMove = true,
                    ShowCloseButton = false,
                    CloseByEscape = false,
                    IsModal = true,
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (System.Windows.Media.Color)Application.Current.Resources["MahApps.Colors.Gray2"] },
                    Padding = new Thickness(50),
                };
                var newGcodeViewDialogViewModel = new GcodeViewModel(instance =>
                {
                    _childWindow.Close();

                }, instance =>
                {
                    _childWindow.Close();
                },
                    this._dialogCoordinator,
                    Gcode
                );

                _childWindow.Content = new Views.GcodeViewDialog()
                {
                    DataContext = newGcodeViewDialogViewModel
                };
                await ChildWindowManager.ShowChildWindowAsync(Application.Current.MainWindow, _childWindow);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogExceptionHeadline,
                        string.Format(Strings.DialogExceptionFormatedContent, exc.Message)
                        );
            }
        }

        public ICommand OpenGCodeViewerNewWindowCommand
        {
            get { return new RelayCommand(async (p) => await OpenGCodeViewerNewWindowAction()); }
        }
        async Task OpenGCodeViewerNewWindowAction()
        {
            try
            {

                if (SelectedGcodeFiles != null && SelectedGcodeFiles.Count > 0)
                {
                    Messenger.Default.Send(new GcodesEditActionMessage()
                    {
                        GcodeFiles = SelectedGcodeFiles.Cast<Gcode>().ToList(),
                    });
                }
                //Messenger.Default.Send(new NotificationMessage(SelectedGcodeFiles, "ShowGcodeEditor"));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogExceptionHeadline,
                        string.Format(Strings.DialogExceptionFormatedContent, exc.Message)
                        );
            }
        }

        public ICommand DeleteSelectedGCodeFilesCommand
        {
            get { return new RelayCommand(async (p) => await DeleteSelectedGCodeFilesAction()); }
        }
        async Task DeleteSelectedGCodeFilesAction()
        {
            try
            {
                var res = await _dialogCoordinator.ShowMessageAsync(this,
                    Strings.DialogRemoveSelectedFilesFromListHeadline,
                    Strings.DialogRemoveSelectedFilesFromListContent,
                    MessageDialogStyle.AffirmativeAndNegative
                    );
                if (res == MessageDialogResult.Affirmative)
                {
                    try
                    {
                        IList collection = new ArrayList(SelectedGcodeFiles);
                        for (int i = 0; i < collection.Count; i++)
                        {
                            if (collection[i] is not Gcode file)
                            {
                                continue;
                            }

                            logger.Info(string.Format(Strings.EventDeletedItemFormated, file.FileName));
                            _ = Gcodes.Remove(file);
                        }
                        OnPropertyChanged(nameof(Gcodes));
                    }
                    catch (Exception exc)
                    {
                        logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        public ICommand AddSelectedGcodesToCalculationCommand
        {
            get { return new RelayCommand(async (p) => await AddSelectedGcodesToCalculationAction()); }
        }
        async Task AddSelectedGcodesToCalculationAction()
        {
            try
            {

            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogExceptionHeadline,
                        string.Format(Strings.DialogExceptionFormatedContent, exc.Message)
                        );
            }
        }
        #endregion

        #endregion

        #region Template Commands

        #region Calculation
        public ICommand ShowCalculationCommand
        {
            get => new RelayCommand((p) => ShowCalculationResult(p as Calculation3d));
        }
        void ShowCalculationResult(Calculation3d calc)
        {
            try
            {
                if (calc == null)
                    return;
                Messenger.Default.Send(new CalculationActionMessage()
                {
                    CalculationId = calc.Id,
                    Action = CalculationMessagingAction.Show,
                });
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand DeleteCalculationFromTemplateCommand
        {
            get => new RelayCommand(async (p) => await DeleteCalculationFromTemplateAction(p));
        }
        async Task DeleteCalculationFromTemplateAction(object p)
        {
            try
            {
                if (p is Calculation3d calc)
                {
                    MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogDeleteCalculationHeadline,
                        Strings.DialogDeleteCalculationContent,
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        _ = Calculations.Remove(calc);
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, calc.Name));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand SelectAllCalculationsCommand
        {
            get => new RelayCommand((p) => SelectAllCalculationsAction());
        }
        void SelectAllCalculationsAction()
        {
            try
            {
                if (Calculations.Count == 0)
                {
                    return;
                }

                SelectedCalculationsView = AllCalcsSelected
                    ? new ArrayList()
                    : new ArrayList(CalculationViews.OfType<CalculationViewInfo>()
                                .ToList());
                AllCalcsSelected = !AllCalcsSelected;
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand DeleteSelectedCalculationsCommand
        {
            get => new RelayCommand(async (p) => await DeleteSelectedCalculationsAction());
        }
        async Task DeleteSelectedCalculationsAction()
        {
            try
            {
                MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                       Strings.DialogDeleteSelectedCalculationsHeadline, Strings.DialogDeleteSelectedCalculationsContent,
                       MessageDialogStyle.AffirmativeAndNegative
                       );
                if (res == MessageDialogResult.Affirmative)
                {
                    IList collection = new ArrayList(SelectedCalculationsView);
                    for (int i = 0; i < collection.Count; i++)
                    {
                        if (collection[i] is not CalculationViewInfo obj)
                        {
                            continue;
                        }

                        logger.Info(string.Format(Strings.EventDeletedItemFormated, obj.Name));
                        _ = Calculations.Remove(obj.Calculation);
                    }
                    OnPropertyChanged(nameof(Calculations));
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand LoadCalculationIntoCalculatorFromTemplateCommand
        {
            get => new RelayCommand((p) => LoadCalculationIntoCalculatorFromTemplateAction(p));
        }
        void LoadCalculationIntoCalculatorFromTemplateAction(object calc)
        {
            try
            {
                if (calc is not Calculation3d currentCalculation) return;
                Messenger.Default.Send(new CalculationActionMessage()
                {
                    CalculationId = currentCalculation.Id,
                    Action = CalculationMessagingAction.LoadIntoCalculator,
                });
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        public ICommand SelectedCalculationsChangedCommand
        {
            get => new RelayCommand((p) => SelectedCalculationsChangedAction(p));
        }
        void SelectedCalculationsChangedAction(object p)
        {
            if (p is CalculationViewInfo calc)
            {

            }
        }
        #endregion

        #endregion

        #endregion

        #region Methods

        #region Tabs
        string GetLocalizedTabName(DashboardTabContentType tab)
        {
            string localizedTabName = string.Empty;
            switch (tab)
            {
                case DashboardTabContentType.Calculator:
                    localizedTabName = Strings.Calculator;
                    break;
                case DashboardTabContentType.StlViewer:
                    localizedTabName = Strings.STLViewer;
                    break;
                case DashboardTabContentType.GcodeViewer:
                    localizedTabName = Strings.GcodeViewer;
                    break;
                case DashboardTabContentType.GcodeEditor:
                    localizedTabName = Strings.GcodeCodeLineEditor;
                    break;
                default:
                    tab.ToString();
                    break;
            }
            return localizedTabName;
        }
        #endregion

        #region Expander
        void ResizeProfile(bool dueToChangedSize)
        {
            _canProfileWidthChange = false;

            if (dueToChangedSize)
            {
                ExpandProfileView = Math.Abs(ProfileWidth.Value - GlobalStaticConfiguration.CalculationView_WidthCollapsed) > GlobalStaticConfiguration.FloatPointFix;
            }
            else
            {
                if (ExpandProfileView)
                {
                    ProfileWidth = Math.Abs(_tempProfileWidth - GlobalStaticConfiguration.CalculationView_WidthCollapsed) < GlobalStaticConfiguration.FloatPointFix ? 
                        new GridLength(GlobalStaticConfiguration.CalculationView_DefaultWidthExpanded) : 
                        new GridLength(_tempProfileWidth);
                }
                else
                {
                    _tempProfileWidth = ProfileWidth.Value;
                    ProfileWidth = new GridLength(GlobalStaticConfiguration.Repetier_WidthCollapsed);
                }
            }

            _canProfileWidthChange = true;
        }
        #endregion

        #region GcodeParser
        async Task ParseGcodeFiles(List<List<string>> filesList, IProgress<int> overallProg)
        {
            //await GcodeChannelWorker.ProcessGcodesAsync(filesList, overallProg);
            int bufferSize = 100;
            Channel<Gcode> channel = Channel.CreateBounded<Gcode>(bufferSize);

            ChannelReader<Gcode> reader = channel.Reader;
            ChannelWriter<Gcode> writer = channel.Writer;

            Task listener = Task.Run(() => ListenToGcodeChannel(channel.Reader));

            Progress = 0;
            IsWorking = true;
            await Task.Delay(10);

            cts = new CancellationTokenSource();
            try
            {
                int filesCount = filesList.SelectMany(list => list).Distinct().Count();
                int filesDone = 0;
                Dictionary<int, int> overallProgress = new();
                for (int i = 0; i < filesCount; i++)
                {
                    overallProgress.Add(i, 0);
                }

                foreach (List<string> files in filesList)
                {
                    List<Task> tasks = new();
                    for(int i = 0; i < files.Count; i++)
                    {
                        string file = files[i];
                        Gcode gc = new(file)
                        {
                            ProcessOrder = i,
                        };
                        GcodesQueue.Add(gc);
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                Progress<int> prog = new(percent =>
                                {
                                    gc.Progress = percent;
                                    if (overallProgress.ContainsKey(gc.ProcessOrder))
                                    {
                                        lock (Lock)
                                        {
                                            overallProgress[gc.ProcessOrder] = percent;
                                            int completeProgress = overallProgress.Sum(pair => pair.Value) / filesCount;
                                            overallProg.Report(completeProgress);
                                        }
                                    }
                                });

                                gc = await GcodeParser.Instance.FromGcodeAsync(
                                    gc, filesCount == 1 ? overallProg : prog, cts.Token, SettingsManager.Current.GcodeParser_PreferValuesInCommentsFromKnownSlicers, PrinterConfig);

                                if (cts.Token.IsCancellationRequested)
                                {
                                    return;
                                }
                                if (gc != null)
                                {
                                    await writer.WriteAsync(gc);
                                    filesDone++;
                                }
                            }
                            catch (Exception exc)
                            {
                                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                            }
                        }));
                    }
                    await Task.WhenAll(tasks);
                    GcodesQueue.Clear();
                }
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
            IsWorking = false;
        }

        async Task ListenToGcodeChannel(ChannelReader<Gcode> reader)
        {
            //because async methods use a state machine to handle awaits
            //it is safe to await in an infinte loop. Thank you C# compiler gods!

            while (await reader.WaitToReadAsync())//if this returns false the channel is completed
            {
                //as a note, if there are multiple readers but only one message, only one reader
                //wakes up. This prevents inefficent races. 
                while (reader.TryRead(out Gcode parsedGcoce))//yes, yes I know about 'out var messageString'...
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        logger.Info(string.Format(Strings.EventGcodeFileParseSucceededFormated, parsedGcoce.FileName, parsedGcoce.ParsingDuration.ToString()));
                        Gcodes.Add(parsedGcoce);
                        _ = GcodesQueue.Remove(parsedGcoce);
                    });
                }
            }
        }
        async Task<bool> ReadFileInformation(string filePath)
        {
            try
            {
                string ext = Path.GetExtension(filePath).ToLower();
                string name = Path.GetFileName(filePath);
                switch (ext)
                {
                    case ".gcode":
                        if (PrinterConfig == null)
                        {
                            MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                                Strings.DialogSlicerPrinterConfigIsMissingHeadline,
                                Strings.DialogSlicerPrinterConfigIsMissingContent,
                                MessageDialogStyle.AffirmativeAndNegative,
                                new MetroDialogSettings()
                                {
                                    AffirmativeButtonText = Strings.GoToSettings,
                                    NegativeButtonText = Strings.Cancel,
                                });
                            if (res == MessageDialogResult.Affirmative)
                            {
                                EventSystem.RedirectToSettings(SettingsViewName.Gcode);
                            }
                            return false;
                        }
                        else
                        {
                            
                            List<List<string>> filesList = CollectionHelper.Split(
                                new List<string>() { filePath }
                                , 4).ToList();
                            Progress<int> overallProg = new(percent => OnProgressUpdateAction(percent));

                            await ParseGcodeFiles(filesList, overallProg);

                        }
                        break;
                    case ".stl":
                        Stl Stl = CreateStlModel(filePath);
                        if (Stl != null)
                        {
                            StlFiles.Add(Stl);
                            StlFile = Stl;
                        }
                        break;
                    case ".3dcx":
                        Calculation3d calc = Calculator3dExporter.DecryptAndDeserialize(filePath);
                        if (calc != null)
                        {
                            Calculation3d item = Calculations.FirstOrDefault(c => c.Id == calc.Id);
                            if (item == null)
                            {
                                Calculations.Add(calc);
                            }
                            else
                            {
                                item = calc;
                            }
                            // Set as selected
                            if (SetLoadedCalculationAsSelected)
                            {
                                Calculation = calc;
                            }
                        }
                        break;
                    case ".3dc":
                    default:
                        _ = await _dialogCoordinator.ShowMessageAsync(this,
                            Strings.DialogFileTypeNotSupportedHeadline,
                            string.Format(Strings.DialogFileTypeNotSupportedFormatedContent, ext, name));
                        return false;
                        //break;
                }
                return true;
            }
            catch (Exception exc)
            {
                IsWorking = false;
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return false;
            }
        }
        #endregion

        #region Calculations

        void CreateCalculationViewInfos()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ObservableCollection<Calculation3d> selection = new(SelectedCalculationsView.OfType<CalculationViewInfo>().ToList().Select(calc => calc.Calculation).ToList());
                Canvas c = new();
                _ = c.Children.Add(new PackIconMaterial { Kind = PackIconMaterialKind.Calculator });
                CalculationViews = new CollectionViewSource
                {
                    Source = Calculations.Select(calc => new CalculationViewInfo()
                    {
                        Name = calc.Files.Count > 0 ? calc.Files[0].Name : "",
                        Calculation = calc,
                        Icon = c,
                        Group = CalculationViewManager.Group.MISC,
                    }).ToList()
                }.View;
                CalculationViews.SortDescriptions.Add(new SortDescription(nameof(CalculationViewInfo.Group), ListSortDirection.Ascending));
                CalculationViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(CalculationViewInfo.Group)));
                // Restore selection
                SelectedCalculationsView = new ArrayList(CalculationViews.OfType<CalculationViewInfo>()
                    .Where(view => selection.Contains(view.Calculation))
                    .ToList());
            });
        }
        #endregion

        public void OnViewVisible()
        {
            try
            {
                OnPropertyChanged(nameof(IsLicenseValid));
                LoadDefaults();
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
            }
        }

        public void OnViewHide()
        {

        }
        
        public async void AddTab(string host = null)
        {
            try
            {
                _tabId++;

                CustomDialog _dialog = new() { Title = Strings.Printers };
                DashboardSelectTabContentDialogViewModel viewModel = new(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    DragablzTabItem item = TabItems.FirstOrDefault(i => i.Header == instance.TabContent.ToString());
                    if (item == null)
                    {
                        UserControl view = null;
                        view = instance.TabContent switch
                        {
                            DashboardTabContentType.Calculator => new DashboardCalculatorView(instance.TabContent),
                            DashboardTabContentType.StlViewer => new DashboardStlViewerView(instance.TabContent),
                            DashboardTabContentType.GcodeViewer => new DashboardGcodeViewerView(instance.TabContent),
                            DashboardTabContentType.GcodeEditor => new DashboardGcodeEditorView(instance.TabContent),
                            _ => new DashboardCalculatorView(instance.TabContent),
                        };
                        if (view != null)
                        {
                            TabItems.Add(new DragablzTabItem(GetLocalizedTabName(instance.TabContent), view, _tabId));
                        }
                        SelectedTabIndex = TabItems.Count - 1;
                    }
                    else
                    {
                        SelectedTabIndex = TabItems.IndexOf(item);
                    }
                }, instance =>
                {
                    _ = _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                }
                , _dialogCoordinator
                );

                _dialog.Content = new DashboardSelectTabDialogView()
                {
                    DataContext = viewModel
                };
                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
            }
        }
        #endregion

    }
}
