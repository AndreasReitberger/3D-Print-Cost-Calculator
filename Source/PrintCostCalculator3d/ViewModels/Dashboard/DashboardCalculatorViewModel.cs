using AndreasReitberger;
using AndreasReitberger.Enums;
using AndreasReitberger.Models;
using AndreasReitberger.Models.CalculationAdditions;
using AndreasReitberger.Models.FileAdditions;
using AndreasReitberger.Utilities;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using MahApps.Metro.SimpleChildWindow;
using PrintCostCalculator3d.Enums;
using PrintCostCalculator3d.Models;
using PrintCostCalculator3d.Models.Documentation;
using PrintCostCalculator3d.Models.Events;
using PrintCostCalculator3d.Models.Messaging;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using PrintCostCalculator3d.ViewModels._3dPrinting;
using PrintCostCalculator3d.ViewModels.Slicer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace PrintCostCalculator3d.ViewModels.Dashboard
{
    public class DashboardCalculatorViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        //readonly DispatcherTimer _dispatcherTimer = new();

        CancellationTokenSource cts;
        readonly SharedCalculatorInstance SharedCalculatorInstance = SharedCalculatorInstance.Instance;

        #endregion

        #region Properties

        #region ModulState
        public DashboardTabContentType TabContentType { get; set; }

        bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                if (_isRefreshing == value) return;
                _isRefreshing = value;
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

        bool _showSelectedFiles = false;
        public bool ShowSelectedFiles
        {
            get => _showSelectedFiles;
            set
            {
                if (_showSelectedFiles == value) return;
                _showSelectedFiles = value;
                OnPropertyChanged();
            }
        }

        bool _isShowingCalculationResult = false;
        public bool IsShowingCalculationResult
        {
            get => _isShowingCalculationResult;
            set
            {
                if (_isShowingCalculationResult == value) return;
                _isShowingCalculationResult = value;
                OnPropertyChanged();
            }
        }

        bool _newCalculationWhenCalculate = false;
        public bool NewCalculationWhenCalculate
        {
            get => _newCalculationWhenCalculate;
            set
            {
                if (_newCalculationWhenCalculate == value) return;
                _newCalculationWhenCalculate = value;
                if (!IsLoading)
                    SettingsManager.Current.General_NewCalculationWhenCalculate = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Expander
        bool _expandGcodeMultiParserView;
        public bool ExpandGcodeMultiParserView
        {
            get => _expandGcodeMultiParserView;
            set
            {
                if (value == _expandGcodeMultiParserView)
                    return;

                if (!IsLoading)
                    SettingsManager.Current.GcodeMultiParse_ExpandView = value;

                _expandGcodeMultiParserView = value;


                if (_canGcodeMultiParseWidthChange)
                    ResizeGcodeMultiParse(false);

                OnPropertyChanged();
            }
        }

        bool _canGcodeMultiParseWidthChange = true;
        double _tempGcodeMultiParseWidth;

        GridLength _gcodeMultiParserWidth;
        public GridLength GcodeMultiParserWidth
        {
            get => _gcodeMultiParserWidth;
            set
            {
                if (value == _gcodeMultiParserWidth)
                    return;

                if (!IsLoading && Math.Abs(value.Value - GlobalStaticConfiguration.GcodeMultiParser_WidthCollapsed) > GlobalStaticConfiguration.FloatPointFix)
                    // Do not save the size when collapsed
                    SettingsManager.Current.GcodeMultiParse_ProfileWidth = value.Value;

                _gcodeMultiParserWidth = value;

                if (_canGcodeMultiParseWidthChange)
                    ResizeGcodeMultiParse(true);

                OnPropertyChanged();
            }
        }
        #endregion

        #region Defaults
        ObservableCollection<Printer3d> _defaultPrinters = new();
        public ObservableCollection<Printer3d> DefaultPrinters
        {
            get => _defaultPrinters;
            set
            {
                if (_defaultPrinters == value) return;
                _defaultPrinters = value;

                SelectedPrintersView = new ArrayList(PrinterViews.OfType<PrinterViewInfo>()
                                .Where(printerview => DefaultPrinters.Contains(printerview.Printer))
                                .ToList());
                OnPropertyChanged();
            }
        }

        ObservableCollection<Material3d> _defaultMaterials = new();
        public ObservableCollection<Material3d> DefaultMaterials
        {
            get => _defaultMaterials;
            set
            {
                if (_defaultMaterials == value) return;
                _defaultMaterials = value;

                SelectedMaterialsView = new ArrayList(MaterialViews.OfType<MaterialViewInfo>()
                                .Where(materialview => DefaultMaterials.Contains(materialview.Material))
                                .ToList());
                OnPropertyChanged();
            }
        }

        ObservableCollection<Workstep> _defaultWorksteps = new();
        public ObservableCollection<Workstep> DefaultWorksteps
        {
            get => _defaultWorksteps;
            set
            {
                if (_defaultWorksteps == value) return;
                _defaultWorksteps = value;

                SelectedWorkstepsView = new ArrayList(WorkstepViews.OfType<WorkstepViewInfo>()
                                .Where(workstepview => DefaultWorksteps.Contains(workstepview.Workstep))
                                .ToList());
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
                if(_calculation != SharedCalculatorInstance.Calculation)
                    SharedCalculatorInstance.Calculation = value;
                OnPropertyChanged();
                
            }
        }

        public bool CanBeSaved
        {
            get => Calculation != null;
        }

        int _selectedMainTab = 0;
        public int SelectedMainTab
        {
            get => _selectedMainTab;
            set
            {
                if (_selectedMainTab == value)
                    return;
                _selectedMainTab = value;
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

        #region PrinterViews
        public ICollectionView PrinterViews
        {
            get => _PrinterViews;
            set
            {
                if (_PrinterViews != value)
                {
                    _PrinterViews = value;
                    OnPropertyChanged(nameof(PrinterViews));
                }
            }
        }
        ICollectionView _PrinterViews;

        PrinterViewInfo _selectedPrinterView;
        public PrinterViewInfo SelectedPrinterView
        {
            get => _selectedPrinterView;
            set
            {
                if (_selectedPrinterView != value)
                {
                    _selectedPrinterView = value;
                    OnPropertyChanged();
                }
            }
        }

        IList _selectedPrintersView = new ArrayList();
        public IList SelectedPrintersView
        {
            get => _selectedPrintersView;
            set
            {
                if (Equals(value, _selectedPrintersView))
                    return;

                _selectedPrintersView = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region MaterialViews
        public ICollectionView MaterialViews
        {
            get => _MaterialViews;
            set
            {
                if (_MaterialViews == value) return;
                _MaterialViews = value;
                OnPropertyChanged();

            }
        }
        ICollectionView _MaterialViews;

        MaterialViewInfo _selectedMaterialView;
        public MaterialViewInfo SelectedMaterialView
        {
            get => _selectedMaterialView;
            set
            {
                if (_selectedMaterialView == value) return;
                _selectedMaterialView = value;
                OnPropertyChanged(nameof(SelectedMaterialView));

            }
        }

        IList _selectedMaterialsView = new ArrayList();
        public IList SelectedMaterialsView
        {
            get => _selectedMaterialsView;
            set
            {
                if (Equals(_selectedMaterialsView, value)) return;
                _selectedMaterialsView = value;
                OnPropertyChanged();

            }
        }

        #endregion

        #region WorkstepViews
        public ICollectionView WorkstepViews
        {
            get => _WorkstepViews;
            set
            {
                if (_WorkstepViews == value) return;
                _WorkstepViews = value;
                OnPropertyChanged();

            }
        }
        ICollectionView _WorkstepViews;

        WorkstepViewInfo _selectedWorkstepView;
        public WorkstepViewInfo SelectedWorkstepView
        {
            get => _selectedWorkstepView;
            set
            {
                if (_selectedWorkstepView == value) return;
                _selectedWorkstepView = value;
                OnPropertyChanged();

            }
        }

        IList _selectedWorkstepsView = new ArrayList();
        public IList SelectedWorkstepsView
        {
            get => _selectedWorkstepsView;
            set
            {
                if (Equals(_selectedWorkstepsView, value)) return;
                _selectedWorkstepsView = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Calculation
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

        ObservableCollection<Material3d> _materials = new();
        public ObservableCollection<Material3d> Materials
        {
            get => _materials;
            set
            {
                if (_materials == value) return;
                if (!IsLoading)
                    SettingsManager.Current.PrinterMaterials = value;
                _materials = value;
                OnPropertyChanged();

            }

        }

        ObservableCollection<Printer3d> _printers = new();
        public ObservableCollection<Printer3d> Printers
        {
            get => _printers;
            set
            {
                if (_printers == value) return;
                if (!IsLoading)
                    SettingsManager.Current.Printers = value;
                _printers = value;
                OnPropertyChanged();
            }

        }

        ObservableCollection<Workstep> _worksteps = new();
        public ObservableCollection<Workstep> Worksteps
        {
            get => _worksteps;
            set
            {
                if (_worksteps == value) return;
                if (!IsLoading)
                    SettingsManager.Current.Worksteps = value;
                _worksteps = value;
                OnPropertyChanged();
            }

        }

        string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        Material3d _material;
        public Material3d Material
        {
            get => _material;
            set
            {
                if (_material == value) return;
                _material = value;
                OnPropertyChanged();

            }
        }

        double _margin = 30;
        public double Margin
        {
            get => _margin;
            set
            {
                if (_margin == value)
                    return;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Margin = value;
                _margin = value;
                OnPropertyChanged();
            }
        }

        public bool _applyEnhancedMarginSettings = false;
        public bool ApplyEnhancedMarginSettings
        {
            get => _applyEnhancedMarginSettings;
            set
            {
                if (_applyEnhancedMarginSettings == value) return;
                if (!IsLoading)
                    SettingsManager.Current.ApplyEnhancedMarginSettings = value;
                _applyEnhancedMarginSettings = value;
                OnPropertyChanged();
            }
        }

        public bool _excludeWorkstepsFromMarginCalculation = false;
        public bool ExcludeWorkstepsFromMarginCalculation
        {
            get => _excludeWorkstepsFromMarginCalculation;
            set
            {
                if (_excludeWorkstepsFromMarginCalculation == value) return;
                if (!IsLoading)
                    SettingsManager.Current.ExcludeWorkstepsFromMarginCalculation = value;
                _excludeWorkstepsFromMarginCalculation = value;
                OnPropertyChanged();
            }
        }

        public bool _excludePrinterCostsFromMarginCalculation = false;
        public bool ExcludePrinterCostsFromMarginCalculation
        {
            get => _excludePrinterCostsFromMarginCalculation;
            set
            {
                if (_excludePrinterCostsFromMarginCalculation == value) return;
                if (!IsLoading)
                    SettingsManager.Current.ExcludePrinterCostsFromMarginCalculation = value;
                _excludePrinterCostsFromMarginCalculation = value;
                OnPropertyChanged();
            }
        }

        public bool _excludeMaterialCostsFromMarginCalculation = false;
        public bool ExcludeMaterialCostsFromMarginCalculation
        {
            get => _excludeMaterialCostsFromMarginCalculation;
            set
            {
                if (_excludeMaterialCostsFromMarginCalculation == value) return;
                if (!IsLoading)
                    SettingsManager.Current.ExcludeMaterialCostsFromMarginCalculation = value;
                _excludeMaterialCostsFromMarginCalculation = value;
                OnPropertyChanged();
            }
        }

        double _failRate = 5;
        public double FailRate
        {
            get => _failRate;
            set
            {
                if (_failRate == value)
                    return;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_FailRate = value;
                _failRate = value;
                OnPropertyChanged();

            }
        }

        int _pieces = 1;
        public int Pieces
        {
            get => _pieces;
            set
            {
                if (_pieces != value)
                {
                    _pieces = value;
                    OnPropertyChanged();
                }
            }
        }

        double _duration = 0;
        public double Duration
        {
            get => _duration;
            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    OnPropertyChanged();
                }
            }
        }

        double _consumedMaterial = 0;
        public double ConsumedMaterial
        {
            get => _consumedMaterial;
            set
            {
                if (_consumedMaterial != value)
                {
                    _consumedMaterial = value;
                    OnPropertyChanged();
                }
            }
        }

        double _volume = 0;
        public double Volume
        {
            get => _volume;
            set
            {
                if (_volume != value)
                {
                    _volume = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Procedure additions

        #region Filament
        bool _applyNozzleWearCosts = false;
        public bool ApplyNozzleWearCosts
        {
            get => _applyNozzleWearCosts;
            set
            {
                if (_applyNozzleWearCosts == value) return;
                _applyNozzleWearCosts = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Filament_ApplyNozzleWearCosts = value;
                OnPropertyChanged();
            }
        }

        double _nozzleReplacementCosts = 0;
        public double NozzleReplacementCosts
        {
            get => _nozzleReplacementCosts;
            set
            {
                if (_nozzleReplacementCosts == value) return;
                _nozzleReplacementCosts = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Filament_NozzleReplacementCosts = value;
                if (NozzleReplacementCosts > 0)
                    NozzleWearCostsPerPrintJob = NozzleReplacementCosts * (NozzleWearFactorPerPrintJob / 100);
                else
                    NozzleWearCostsPerPrintJob = 0;
                OnPropertyChanged();

            }
        }

        double _nozzleWearFactorPerPrintJob = 0;
        public double NozzleWearFactorPerPrintJob
        {
            get => _nozzleWearFactorPerPrintJob;
            set
            {
                if (_nozzleWearFactorPerPrintJob == value) return;
                _nozzleWearFactorPerPrintJob = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Filament_NozzleWearFactorPerPrintJob = value;
                if (NozzleReplacementCosts > 0)
                    NozzleWearCostsPerPrintJob = NozzleReplacementCosts * (NozzleWearFactorPerPrintJob / 100);
                else
                    NozzleWearCostsPerPrintJob = 0;
                OnPropertyChanged();

            }
        }

        double _nozzleWearCostsPerPrintJob = 0;
        public double NozzleWearCostsPerPrintJob
        {
            get => _nozzleWearCostsPerPrintJob;
            set
            {
                if (_nozzleWearCostsPerPrintJob == value) return;
                _nozzleWearCostsPerPrintJob = value;
                OnPropertyChanged();

            }
        }

        bool _applyPrintSheetWearCosts = false;
        public bool ApplyPrintSheetWearCosts
        {
            get => _applyPrintSheetWearCosts;
            set
            {
                if (_applyPrintSheetWearCosts == value) return;
                _applyPrintSheetWearCosts = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Filament_ApplyPrintSheetWearCosts = value;
                OnPropertyChanged();
            }
        }

        double _printSheetReplacementCosts = 0;
        public double PrintSheetReplacementCosts
        {
            get => _printSheetReplacementCosts;
            set
            {
                if (_printSheetReplacementCosts == value) return;
                _printSheetReplacementCosts = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Filament_PrintSheetReplacementCosts = value;
                if (PrintSheetReplacementCosts > 0)
                    PrintSheetWearCostsPerPrintJob = PrintSheetReplacementCosts * (PrintSheetWearFactorPerPrintJob / 100);
                else
                    PrintSheetWearCostsPerPrintJob = 0;
                OnPropertyChanged();

            }
        }

        double _printSheetWearFactorPerPrintJob = 0;
        public double PrintSheetWearFactorPerPrintJob
        {
            get => _printSheetWearFactorPerPrintJob;
            set
            {
                if (_printSheetWearFactorPerPrintJob == value) return;
                _printSheetWearFactorPerPrintJob = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Filament_PrintSheetWearFactorPerPrintJob = value;
                if (PrintSheetReplacementCosts > 0)
                    PrintSheetWearCostsPerPrintJob = PrintSheetReplacementCosts * (PrintSheetWearFactorPerPrintJob / 100);
                else
                    PrintSheetWearCostsPerPrintJob = 0;
                OnPropertyChanged();

            }
        }

        double _printSheetWearCostsPerPrintJob = 0;
        public double PrintSheetWearCostsPerPrintJob
        {
            get => _printSheetWearCostsPerPrintJob;
            set
            {
                if (_printSheetWearCostsPerPrintJob == value) return;
                _printSheetWearCostsPerPrintJob = value;
                OnPropertyChanged();

            }
        }
        #endregion

        #region Resin

        bool _applyResinGlovesCosts = false;
        public bool ApplyResinGlovesCosts
        {
            get => _applyResinGlovesCosts;
            set
            {
                if (_applyResinGlovesCosts == value) return;
                _applyResinGlovesCosts = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Resin_ApplyGlovesCosts = value;
                OnPropertyChanged();
            }
        }

        int _glovesPerPrintjob = 2;
        public int GlovesPerPrintJob
        {
            get => _glovesPerPrintjob;
            set
            {
                if (_glovesPerPrintjob == value) return;
                _glovesPerPrintjob = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Resin_GlovesPerPrintJob = value;
                OnPropertyChanged();

            }
        }

        int _glovesInPackage = 100;
        public int GlovesInPackage
        {
            get => _glovesInPackage;
            set
            {
                if (_glovesInPackage == value) return;
                _glovesInPackage = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Resin_GlovesInPackage = value;
                if (GlovesPackagePrice > 0)
                    CostPerGlove = GlovesPackagePrice / GlovesInPackage;
                else
                    CostPerGlove = 0;
                OnPropertyChanged();

            }
        }

        double _glovesPackagePrice = 0;
        public double GlovesPackagePrice
        {
            get => _glovesPackagePrice;
            set
            {
                if (_glovesPackagePrice == value) return;
                _glovesPackagePrice = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Resin_GlovesPackagePrice = value;

                if (GlovesPackagePrice > 0)
                    CostPerGlove = GlovesPackagePrice / GlovesInPackage;
                else
                    CostPerGlove = 0;
                OnPropertyChanged();

            }
        }

        double _costPerGlove = 0;
        public double CostPerGlove
        {
            get => _costPerGlove;
            set
            {
                if (_costPerGlove == value) return;
                _costPerGlove = value;
                OnPropertyChanged();

            }
        }

        bool _applyResinFilterCosts = false;
        public bool ApplyResinFilterCosts
        {
            get => _applyResinFilterCosts;
            set
            {
                if (_applyResinFilterCosts == value) return;
                _applyResinFilterCosts = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Resin_ApplyFilterCosts = value;
                OnPropertyChanged();
            }
        }

        double _filtersPerPrintjob = 0.25f;
        public double FiltersPerPrintJob
        {
            get => _filtersPerPrintjob;
            set
            {
                if (_filtersPerPrintjob == value) return;
                _filtersPerPrintjob = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Resin_FiltersPerPrintJob = value;
                OnPropertyChanged();

            }
        }

        int _filtersInPackage = 150;
        public int FiltersInPackage
        {
            get => _filtersInPackage;
            set
            {
                if (_filtersInPackage == value) return;
                _filtersInPackage = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Resin_FiltersInPackage = value;
                if (GlovesPackagePrice > 0)
                    CostPerFilter = FiltersPackagePrice / FiltersInPackage;
                else
                    CostPerFilter = 0;
                OnPropertyChanged();

            }
        }

        double _filtersPackagePrice = 0;
        public double FiltersPackagePrice
        {
            get => _filtersPackagePrice;
            set
            {
                if (_filtersPackagePrice == value) return;
                _filtersPackagePrice = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Resin_FiltersPackagePrice = value;

                if (FiltersPackagePrice > 0)
                    CostPerFilter = FiltersPackagePrice / FiltersInPackage;
                else
                    CostPerFilter = 0;
                OnPropertyChanged();

            }
        }

        double _costPerFilter = 0;
        public double CostPerFilter
        {
            get => _costPerFilter;
            set
            {
                if (_costPerFilter == value) return;
                _costPerFilter = value;
                OnPropertyChanged();

            }
        }

        bool _applyResinWashingCosts = false;
        public bool ApplyResinWashingCosts
        {
            get => _applyResinWashingCosts;
            set
            {
                if (_applyResinWashingCosts == value) return;
                _applyResinWashingCosts = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Resin_ApplyWashingCosts = value;
                OnPropertyChanged();
            }
        }

        double _isopropanolContainerContent = 5;
        public double IsopropanolContainerContent
        {
            get => _isopropanolContainerContent;
            set
            {
                if (_isopropanolContainerContent == value) return;
                _isopropanolContainerContent = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Resin_IsopropanolContainerContent = value;
                if (IsopropanolContainerContent > 0)
                    CostPerLiterIsopropanol = IsopropanolContainerPrice / IsopropanolContainerContent;
                else
                    CostPerLiterIsopropanol = 0;
                OnPropertyChanged();

            }
        }

        double _isopropanolContainerPrice = 35;
        public double IsopropanolContainerPrice
        {
            get => _isopropanolContainerPrice;
            set
            {
                if (_isopropanolContainerPrice == value) return;
                _isopropanolContainerPrice = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Resin_IsopropanolContainerPrice = value;
                if (IsopropanolContainerContent > 0)
                    CostPerLiterIsopropanol = IsopropanolContainerPrice / IsopropanolContainerContent;
                else
                    CostPerLiterIsopropanol = 0;
                OnPropertyChanged();

            }
        }

        double _costPerLiterIsopropanol = 0;
        public double CostPerLiterIsopropanol
        {
            get => _costPerLiterIsopropanol;
            set
            {
                if (_costPerLiterIsopropanol == value) return;
                _costPerLiterIsopropanol = value;
                OnPropertyChanged();

            }
        }

        double _isopropanolPerPrintJob = 0.25f;
        public double IsopropanolPerPrintJob
        {
            get => _isopropanolPerPrintJob;
            set
            {
                if (_isopropanolPerPrintJob == value) return;
                _isopropanolPerPrintJob = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Resin_IsopropanolPerPrintJob = value;
                OnPropertyChanged();

            }
        }

        bool _applyResinCuringCosts = false;
        public bool ApplyResinCuringCosts
        {
            get => _applyResinCuringCosts;
            set
            {
                if (_applyResinCuringCosts == value) return;
                _applyResinCuringCosts = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Resin_ApplyCuringCosts = value;
                OnPropertyChanged();
            }
        }

        double _curingCostsPerHour = 1;
        public double CuringCostsPerHour
        {
            get => _curingCostsPerHour;
            set
            {
                if (_curingCostsPerHour == value) return;
                _curingCostsPerHour = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Resin_CuringCostsPerHour = value;

                OnPropertyChanged();

            }
        }

        double _curingDurationInMintues = 5;
        public double CuringDurationInMintues
        {
            get => _curingDurationInMintues;
            set
            {
                if (_curingDurationInMintues == value) return;
                _curingDurationInMintues = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_Resin_CuringDurationInMinutes = value;

                OnPropertyChanged();

            }
        }

        double _costsForCuring = 0;
        public double CostsForCuring
        {
            get => _costsForCuring;
            set
            {
                if (_costsForCuring == value) return;
                _costsForCuring = value;
                OnPropertyChanged();

            }
        }

        #endregion

        #region Powder

        bool _applySLSRefreshing = false;
        public bool ApplySLSRefreshing
        {
            get => _applySLSRefreshing;
            set
            {
                if (_applySLSRefreshing == value) return;
                _applySLSRefreshing = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_SLS_ApplyRefreshing = value;
                OnPropertyChanged();
            }
        }

        double _powderInBuildArea = 0;
        public double PowderInBuildArea
        {
            get => _powderInBuildArea;
            set
            {
                if (_powderInBuildArea == value) return;
                _powderInBuildArea = value;
                OnPropertyChanged();

            }
        }

        double _powderRefreshRate = 30;
        public double PowderRefreshRate
        {
            get => _powderRefreshRate;
            set
            {
                if (_powderRefreshRate == value) return;
                _powderRefreshRate = value;
                OnPropertyChanged();

            }
        }

        double _refreshedPowder = 0;
        public double RefreshedPowder
        {
            get => _refreshedPowder;
            set
            {
                if (_refreshedPowder == value) return;
                _refreshedPowder = value;
                OnPropertyChanged();

            }
        }
        #endregion

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
                OnPropertyChanged();
            }
        }

        ObservableCollection<Gcode> _gcodes = new();
        public ObservableCollection<Gcode> Gcodes
        {
            get => _gcodes;
            set
            {
                if (_gcodes != value)
                {
                    _gcodes = value;
                    OnPropertyChanged();
                }
            }
        }

        File3d _file;
        public File3d SingleFile
        {
            get => _file;
            set
            {
                if (_file == value) return;
                _file = value;
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

        IList _selectedGcodeFiles = new ArrayList();
        public IList SelectedGcodeFiles
        {
            get => _selectedGcodeFiles;
            set
            {
                TotalFileCount = SelectedGcodeFiles.Count + SelectedStlFiles.Count;
                if (Equals(value, _selectedGcodeFiles))
                    return;
                _selectedGcodeFiles = value;
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

        #endregion

        #region Stl
        Stl _stlFile;
        public Stl StlFile
        {
            get => _stlFile;
            set
            {
                if (_stlFile != value)
                {
                    _stlFile = value;
                    OnPropertyChanged();
                }
            }
        }

        ObservableCollection<Stl> _stlFiles = new();
        public ObservableCollection<Stl> StlFiles
        {
            get => _stlFiles;
            set
            {
                if (_stlFiles != value)
                {
                    _stlFiles = value;
                    OnPropertyChanged();
                }
            }
        }

        IList _selectedStlFiles = new ArrayList();
        public IList SelectedStlFiles
        {
            get => _selectedStlFiles;
            set
            {
                TotalFileCount = SelectedGcodeFiles.Count + SelectedStlFiles.Count;
                if (Equals(value, _selectedStlFiles))
                    return;

                _selectedStlFiles = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region TotalFiles
        int _totalFilesCount = 0;
        public int TotalFileCount
        {
            get => _totalFilesCount;
            set
            {
                if (_totalFilesCount == value) return;
                _totalFilesCount = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion

        #region Settings 
        bool _showCalculationResult = true;
        public bool ShowCalculationResultPopup
        {
            get => _showCalculationResult;
            set
            {
                if (_showCalculationResult == value) return;

                if (!IsLoading)
                    SettingsManager.Current.General_OpenCalculationResultView = value;
                _showCalculationResult = value;
                OnPropertyChanged();

            }
        }

        bool _applyEnergyCosts = false;
        public bool ApplyEnergyCosts
        {
            get => _applyEnergyCosts;
            set
            {
                if (_applyEnergyCosts == value) return;
                if (!IsLoading)
                    SettingsManager.Current.ApplyEnergyCosts = value;
                _applyEnergyCosts = value;
                OnPropertyChanged();

            }
        }

        bool _applyProcedureSpecificAdditions = false;
        public bool ApplyProcedureSpecificAdditions
        {
            get => _applyProcedureSpecificAdditions;
            set
            {
                if (_applyProcedureSpecificAdditions == value) return;
                if (!IsLoading)
                    SettingsManager.Current.ApplyProcedureSpecificAdditions = value;
                _applyProcedureSpecificAdditions = value;
                OnPropertyChanged();

            }
        }

        Printer3dType _type = Printer3dType.FDM;
        public Printer3dType Type
        {
            get => _type;
            set
            {
                if (_type == value) return;
                _type = value;
                OnPropertyChanged();
            }
        }

        Material3dFamily _procedure = Material3dFamily.Filament;
        public Material3dFamily Procedure
        {
            get => _procedure;
            set
            {
                if (_procedure == value) return;
                _procedure = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_LastProcedure = value;
                OnPropertyChanged();
            }
        }

        IList _proceduresList = new ArrayList();
        public IList ProceduresList
        {
            get => _proceduresList;
            set
            {
                if (_proceduresList.Equals(value)) return;
                _proceduresList = value;
                OnPropertyChanged();
            }
        }

        int _procedureTabIndex = 0;
        public int ProcedureTabIndex
        {
            get => _procedureTabIndex;
            set
            {
                if (_procedureTabIndex == value)
                    return;
                _procedureTabIndex = value;
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

        bool _applyCustomAdditions = false;
        public bool ApplyCustomAdditions
        {
            get => _applyCustomAdditions;
            set
            {
                if (_applyCustomAdditions == value) return;
                _applyCustomAdditions = value;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_ApplyCustomAdditions = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<CustomAddition> _customAdditions = new();
        public ObservableCollection<CustomAddition> CustomAdditions
        {
            get => _customAdditions;
            set
            {
                if (_customAdditions == value) return;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_CustomAdditions = value;
                _customAdditions = value;
                OnPropertyChanged();
            }
        }

        CustomAddition _customAddition;
        public CustomAddition CustomAddition
        {
            get => _customAddition;
            set
            {
                if (_customAddition == value) return;
                _customAddition = value;
                OnPropertyChanged();
            }
        }

        IList _selectedCustomAdditions = new ArrayList();
        public IList SelectedCustomAdditions
        {
            get => _selectedCustomAdditions;
            set
            {
                if (Equals(value, _selectedCustomAdditions)) return;

                _selectedCustomAdditions = value;
                OnPropertyChanged();
            }
        }

        public int _powerLevel = 75;
        public int PowerLevel
        {
            get => _powerLevel;
            set
            {
                if (_powerLevel == value) return;
                if (!IsLoading)
                    SettingsManager.Current.PowerLevel = value;
                _powerLevel = value;
                OnPropertyChanged();

            }
        }

        public double _energyCosts = 0;
        public double EnergyCosts
        {
            get => _energyCosts;
            set
            {
                if (_energyCosts == value) return;
                if (!IsLoading)
                    SettingsManager.Current.EnergyCosts = value;
                _energyCosts = value;
                OnPropertyChanged();

            }
        }
        double _handlingFee = 5;
        public double HandlingFee
        {
            get => _handlingFee;
            set
            {
                if (_handlingFee == value)
                    return;
                if (!IsLoading)
                    SettingsManager.Current.HandlingFee = value;
                _handlingFee = value;
                OnPropertyChanged();
            }
        }

        public bool _applyTaxRate = false;
        public bool ApplyTaxRate
        {
            get => _applyTaxRate;
            set
            {
                if (_applyTaxRate == value) return;
                if (!IsLoading)
                    SettingsManager.Current.ApplyTaxRate = value;
                _applyTaxRate = value;
                OnPropertyChanged();

            }
        }

        public double _taxRate = 19;
        public double Taxrate
        {
            get => _taxRate;
            set
            {
                if (_taxRate == value) return;
                if (!IsLoading)
                    SettingsManager.Current.TaxRate = value;
                _taxRate = value;
                OnPropertyChanged();
            }
        }

        bool _showGcodeInfos = false;
        public bool ShowGcodeInfos
        {
            get => _showGcodeInfos;
            set
            {
                if (_showGcodeInfos == value)
                    return;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_ShowGcodeInfos = value;
                _showGcodeInfos = value;
                OnPropertyChanged();
            }
        }

        bool _showGcodeGrid = false;
        public bool ShowGcodeGrid
        {
            get => _showGcodeGrid;
            set
            {
                if (_showGcodeGrid == value)
                    return;
                if (!IsLoading)
                    SettingsManager.Current.Calculation_ShowGcodeGrid = value;
                _showGcodeGrid = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Search
        string _searchPrinter = string.Empty;
        public string SearchPrinter
        {
            get => _searchPrinter;
            set
            {
                if (_searchPrinter != value)
                {
                    _searchPrinter = value;

                    PrinterViews.Refresh();

                    ICollectionView view = CollectionViewSource.GetDefaultView(PrinterViews);
                    IEqualityComparer<String> comparer = StringComparer.InvariantCultureIgnoreCase;
                    view.Filter = o =>
                    {
                        PrinterViewInfo p = o as PrinterViewInfo;
                        string[] patterns = _searchPrinter.ToLower().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (patterns.Length == 1 || patterns.Length == 0)
                            return p.Name.ToLower().Contains(_searchPrinter.ToLower());
                        else
                        {
                            return patterns.Any(p.Name.ToLower().Contains) || patterns.Any(p.Printer.Manufacturer.Name.ToLower().Contains);
                        }
                    };
                    OnPropertyChanged(nameof(SearchPrinter));
                }
            }
        }

        string _searchOffer = string.Empty;
        public string SearchOffer
        {
            get => _searchOffer;
            set
            {
                if (_searchOffer != value)
                {
                    _searchOffer = value;

                    CalculationViews.Refresh();

                    ICollectionView view = CollectionViewSource.GetDefaultView(CalculationViews);
                    IEqualityComparer<String> comparer = StringComparer.InvariantCultureIgnoreCase;
                    view.Filter = o =>
                    {
                        CalculationViewInfo p = o as CalculationViewInfo;
                        string[] patterns = _searchOffer.ToLower().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (patterns.Length == 1 || patterns.Length == 0)
                            return p.Name.ToLower().Contains(_searchOffer.ToLower());
                        else
                        {
                            return patterns.Any(p.Name.ToLower().Contains) || patterns.Any(p.Calculation.Name.ToLower().Contains);
                        }
                    };
                    OnPropertyChanged();
                }
            }
        }

        string _searchMaterial = string.Empty;
        public string SearchMaterial
        {
            get => _searchMaterial;
            set
            {
                if (_searchMaterial != value)
                {
                    _searchMaterial = value;

                    MaterialViews.Refresh();

                    ICollectionView view = CollectionViewSource.GetDefaultView(MaterialViews);
                    IEqualityComparer<String> comparer = StringComparer.InvariantCultureIgnoreCase;
                    view.Filter = o =>
                    {
                        MaterialViewInfo m = o as MaterialViewInfo;
                        string[] patterns = _searchMaterial.ToLower().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (patterns.Length == 1 || patterns.Length == 0)
                            return m.Name.ToLower().Contains(_searchMaterial.ToLower()) || m.Material.SKU.ToLower().Contains(_searchMaterial.ToLower());
                        else
                        {
                            return patterns.Any(m.Name.ToLower().Contains) || patterns.Any(m.Material.SKU.ToLower().Contains);
                        }
                    };

                    OnPropertyChanged(nameof(SearchMaterial));
                }
            }
        }

        string _searchWorksteps = string.Empty;
        public string SearchWorksteps
        {
            get => _searchWorksteps;
            set
            {
                if (_searchWorksteps != value)
                {
                    _searchWorksteps = value;

                    WorkstepViews.Refresh();

                    ICollectionView view = CollectionViewSource.GetDefaultView(WorkstepViews);
                    IEqualityComparer<String> comparer = StringComparer.InvariantCultureIgnoreCase;
                    view.Filter = o =>
                    {
                        WorkstepViewInfo m = o as WorkstepViewInfo;
                        string[] patterns = _searchWorksteps.ToLower().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (patterns.Length == 1 || patterns.Length == 0)
                            return m.Name.ToLower().Contains(_searchWorksteps.ToLower());
                        else
                        {
                            return patterns.Any(m.Name.ToLower().Contains) || patterns.Any(m.Workstep.Category.Name.ToLower().Contains);
                        }
                    };

                    OnPropertyChanged(nameof(SearchWorksteps));
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

        #region EnumCollections

        #region CalculationTypes
        ObservableCollection<CustomAdditionCalculationType> _calculationTypes = new(
            Enum.GetValues(typeof(CustomAdditionCalculationType)).Cast<CustomAdditionCalculationType>().ToList()
            );
        public ObservableCollection<CustomAdditionCalculationType> CalculationTypes
        {
            get => _calculationTypes;
            set
            {
                if (_calculationTypes == value) return;

                _calculationTypes = value;
                OnPropertyChanged();

            }
        }

        #endregion

        #region PrinterTypes
        ObservableCollection<Printer3dType> _printerTypes = new(
            Enum.GetValues(typeof(Printer3dType)).Cast<Printer3dType>().ToList()
            );
        public ObservableCollection<Printer3dType> PrinterTypes
        {
            get => _printerTypes;
            set
            {
                if (_printerTypes == value) return;

                _printerTypes = value;
                OnPropertyChanged();

            }
        }

        #endregion

        #region Procedures
        ObservableCollection<Material3dFamily> _procedures = new(
            Enum.GetValues(typeof(Material3dFamily)).Cast<Material3dFamily>().ToList()
            );
        public ObservableCollection<Material3dFamily> Procedures
        {
            get => _procedures;
            set
            {
                if (_procedures == value) return;
                _procedures = value;
                OnPropertyChanged();

            }
        }

        #endregion

        #region Units
        ObservableCollection<Unit> _units = new(
            Enum.GetValues(typeof(Unit)).Cast<Unit>().ToList()
            );
        public ObservableCollection<Unit> Units
        {
            get => _units;
            set
            {
                if (_units == value) return;

                _units = value;
                OnPropertyChanged();

            }
        }

        #endregion

        #endregion

        #region Constructor
        public DashboardCalculatorViewModel(IDialogCoordinator dialog, DashboardTabContentType TabName)
        {
            TabContentType = TabName;

            _dialogCoordinator = dialog;
            RegisterMessages();

            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            IsLicenseValid = false;

            SharedCalculatorInstance.OnCalculationsChanged += SharedCalculatorInstance_OnCalculationsChanged;
            SharedCalculatorInstance.OnSelectedCalculationChanged += SharedCalculatorInstance_OnSelectedCalculationChanged;

            SharedCalculatorInstance.OnGcodesChanged += SharedCalculatorInstance_OnGcodesChanged;
            SharedCalculatorInstance.OnSelectedGcodeChanged += SharedCalculatorInstance_OnSelectedGcodeChanged;

            Printers.CollectionChanged += Printers_CollectionChanged;
            Materials.CollectionChanged += Materials_CollectionChanged;
            Worksteps.CollectionChanged += Worksteps_CollectionChanged;

            Calculations = SharedCalculatorInstance.Calculations;
            Calculations.CollectionChanged += Calculations_CollectionChanged;

            Gcode = SharedCalculatorInstance.Gcode;
            Gcodes = new ObservableCollection<Gcode>(SharedCalculatorInstance.Gcodes);
            Gcodes.CollectionChanged += Gcodes_CollectionChanged;
            //StlFiles.CollectionChanged += StlFiles_CollectionChanged;

            CreatePrinterViewInfos();
            CreateMaterialViewInfos();
            CreateWorkstepViewInfos();

            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }

        void LoadDefaults()
        {
            DefaultMaterials = SettingsManager.Current.Calculation_DefaultMaterialsLib;
            DefaultPrinters = SettingsManager.Current.Calculation_DefaultPrintersLib;
            DefaultWorksteps = SettingsManager.Current.Calculation_DefaultWorkstepsLib;

            //PrinterConfigs = SettingsManager.Current.GcodeParser_PrinterConfigs;
            //PrinterConfig = SettingsManager.Current.GcodeParser_PrinterConfig;
        }

        void LoadSettings()
        {
            Printers = SettingsManager.Current.Printers;
            Materials = SettingsManager.Current.PrinterMaterials;
            Worksteps = SettingsManager.Current.Worksteps;

            NewCalculationWhenCalculate = SettingsManager.Current.General_NewCalculationWhenCalculate;
            SelectedInfoTab = SettingsManager.Current.Calculation_SelectedInfoTab;
            ShowGcodeInfos = SettingsManager.Current.Calculation_ShowGcodeInfos;
            ShowGcodeGrid = SettingsManager.Current.Calculation_ShowGcodeGrid;
            ShowCalculationResultPopup = SettingsManager.Current.General_OpenCalculationResultView;

            ExpandGcodeMultiParserView = SettingsManager.Current.GcodeMultiParse_ExpandView;
            GcodeMultiParserWidth = ExpandGcodeMultiParserView ? new GridLength(SettingsManager.Current.GcodeMultiParse_ProfileWidth) : new GridLength(GlobalStaticConfiguration.GcodeMultiParser_WidthCollapsed);
            _tempGcodeMultiParseWidth = SettingsManager.Current.GcodeMultiParse_ProfileWidth;

            Margin = SettingsManager.Current.Calculation_Margin;
            ApplyEnhancedMarginSettings = SettingsManager.Current.ApplyEnhancedMarginSettings;
            ExcludeWorkstepsFromMarginCalculation = SettingsManager.Current.ExcludeWorkstepsFromMarginCalculation;
            ExcludePrinterCostsFromMarginCalculation = SettingsManager.Current.ExcludePrinterCostsFromMarginCalculation;
            ExcludeMaterialCostsFromMarginCalculation = SettingsManager.Current.ExcludeMaterialCostsFromMarginCalculation;
            FailRate = SettingsManager.Current.Calculation_FailRate;

            ApplyTaxRate = SettingsManager.Current.ApplyTaxRate;
            Taxrate = SettingsManager.Current.TaxRate;

            ApplyEnergyCosts = SettingsManager.Current.ApplyEnergyCosts;
            EnergyCosts = SettingsManager.Current.EnergyCosts;
            PowerLevel = SettingsManager.Current.PowerLevel;

            ApplyCustomAdditions = SettingsManager.Current.Calculation_ApplyCustomAdditions;
            CustomAdditions = SettingsManager.Current.Calculation_CustomAdditions;
            CustomAdditions.CollectionChanged += CustomAdditions_CollectionChanged;

            Procedure = SettingsManager.Current.Calculation_LastProcedure;
            ApplyProcedureSpecificAdditions = SettingsManager.Current.ApplyProcedureSpecificAdditions;
            //Powder
            ApplySLSRefreshing = SettingsManager.Current.Calculation_SLS_ApplyRefreshing;
            //Resin
            ApplyResinGlovesCosts = SettingsManager.Current.Calculation_Resin_ApplyGlovesCosts;
            GlovesPerPrintJob = SettingsManager.Current.Calculation_Resin_GlovesPerPrintJob;
            GlovesInPackage = SettingsManager.Current.Calculation_Resin_GlovesInPackage;
            GlovesPackagePrice = SettingsManager.Current.Calculation_Resin_GlovesPackagePrice;

            ApplyResinFilterCosts = SettingsManager.Current.Calculation_Resin_ApplyFilterCosts;
            FiltersPerPrintJob = SettingsManager.Current.Calculation_Resin_FiltersPerPrintJob;
            FiltersInPackage = SettingsManager.Current.Calculation_Resin_FiltersInPackage;
            FiltersPackagePrice = SettingsManager.Current.Calculation_Resin_FiltersPackagePrice;

            ApplyResinWashingCosts = SettingsManager.Current.Calculation_Resin_ApplyWashingCosts;
            IsopropanolContainerContent = SettingsManager.Current.Calculation_Resin_IsopropanolContainerContent;
            IsopropanolContainerPrice = SettingsManager.Current.Calculation_Resin_IsopropanolContainerPrice;
            IsopropanolPerPrintJob = SettingsManager.Current.Calculation_Resin_IsopropanolPerPrintJob;

            ApplyResinCuringCosts = SettingsManager.Current.Calculation_Resin_ApplyCuringCosts;
            CuringCostsPerHour = SettingsManager.Current.Calculation_Resin_CuringCostsPerHour;
            CuringDurationInMintues = SettingsManager.Current.Calculation_Resin_CuringDurationInMinutes;

            ApplyNozzleWearCosts = SettingsManager.Current.Calculation_Filament_ApplyNozzleWearCosts;
            NozzleReplacementCosts = SettingsManager.Current.Calculation_Filament_NozzleReplacementCosts;
            NozzleWearFactorPerPrintJob = SettingsManager.Current.Calculation_Filament_NozzleWearFactorPerPrintJob;

            ApplyPrintSheetWearCosts = SettingsManager.Current.Calculation_Filament_ApplyPrintSheetWearCosts;
            PrintSheetReplacementCosts = SettingsManager.Current.Calculation_Filament_PrintSheetReplacementCosts;
            PrintSheetWearFactorPerPrintJob = SettingsManager.Current.Calculation_Filament_PrintSheetWearFactorPerPrintJob;

        }

        void RegisterMessages()
        {
            //Messenger.Default.Register<GcodesChangedMessage>(this, GcodesChangedAction);
            Messenger.Default.Register<SettingsChangedEventArgs>(this, SettingsChangedAction);

            Messenger.Default.Register<Calculation3d>(this, CalculationReceivedAsync);
            Messenger.Default.Register<CalculationsChangedMessage>(this, CalculationsChangedAction);
            Messenger.Default.Register<GcodesParseMessage>(this, GcodeParseActionAsync);
            Messenger.Default.Register<CalculationActionMessage>(this, CalculationActionAsync);
        }
        #endregion

        #region Messages
        void SettingsChangedAction(SettingsChangedEventArgs msg)
        {
            if (msg != null)
            {
                IsLoading = true;
                LoadSettings();
                IsLoading = false;
            }
        }
        async void CalculationReceivedAsync(Calculation3d msg)
        {
            if(msg != null)
            {
                await LoadCalculation(msg);
            }
        }
        async void GcodeParseActionAsync(GcodesParseMessage msg)
        {
            if(msg != null)
            {
                cts = new CancellationTokenSource();
                foreach (List<string> files in msg.GcodeFiles)
                {
                    List<Task> tasks = new();
                    foreach (string file in files)
                    {
                        Gcode gc = new(file);
                        GcodesQueue.Add(gc);
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                Progress<int> prog = new(percent =>
                                {
                                    gc.Progress = percent;
                                });
                                gc = await GcodeParser.Instance.FromGcodeAsync(
                                    gc, prog, cts.Token, SettingsManager.Current.GcodeParser_PreferValuesInCommentsFromKnownSlicers, msg.SlicerConfig);

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    logger.Info(string.Format(Strings.EventGcodeFileParseSucceededFormated, gc.FileName, gc.ParsingDuration.ToString()));
                                    Files.Add(new File3d()
                                    {
                                        FileName = gc.FileName,
                                        Name = gc.FileName,
                                        FilePath = gc.FilePath,
                                        PrintTime = gc.PrintTime,
                                        Volume = gc.ExtrudedFilamentVolume,
                                    });
                                    Gcodes.Add(gc);
                                    _ = GcodesQueue.Remove(gc);
                                });

                            }
                            catch (Exception exc)
                            {
                                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                            }
                        }));
                    }
                    await Task.WhenAll(tasks);
                }

            }
        }
        void CalculationsChangedAction(CalculationsChangedMessage msg)
        {
            if (msg != null)
            {
                if (msg.Calculation == null)
                {
                    return;
                }

                switch (msg.Action)
                {
                    case MessagingAction.Add:
                        if (!Calculations.Contains(msg.Calculation))
                        {
                            Calculations.Add(msg.Calculation);
                        }

                        break;
                    case MessagingAction.Remove:
                        Calculation3d calculation = Calculations.FirstOrDefault(calc => calc.Id == msg.Calculation.Id);
                        if (calculation != null)
                        {
                            _ = Calculations.Remove(calculation);
                        }
                        break;
                    case MessagingAction.Invalidate:
                        CreateCalculationViewInfos();
                        break;
                    case MessagingAction.SetSelected:
                        break;
                    case MessagingAction.ClearHelixView:
                        break;
                    default:
                        break;
                }
            }
        }
        async void CalculationActionAsync(CalculationActionMessage msg)
        {
            if (msg != null)
            {
                if (msg.CalculationId == Guid.Empty)
                {
                    return;
                }
                Calculation3d calc = Calculations.FirstOrDefault(c => c.Id == msg.CalculationId);
                if (calc == null)
                {
                    return;
                }

                switch (msg.Action)
                {
                    case CalculationMessagingAction.Add:
                        break;
                    case CalculationMessagingAction.Remove:
                        break;
                    case CalculationMessagingAction.SetSelected:
                        Calculation = calc;
                        break;
                    case CalculationMessagingAction.LoadIntoCalculator:
                        LoadCalculationIntoCalculatorFromTemplateAction(calc);
                        break;
                    case CalculationMessagingAction.Show:
                        await ShowCalculationResult(calc);
                        break;
                    case CalculationMessagingAction.Invalidate:
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region Events
        void CustomAdditions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CustomAdditions));
            SettingsManager.Current.Calculation_CustomAdditions = CustomAdditions;
            SettingsManager.Save();
        }

        void StlFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(StlFiles));
        }

        void Gcodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Gcodes));
        }

        void Calculations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Calculations));
            OnPropertyChanged(nameof(TotalPrice));
            OnPropertyChanged(nameof(TotalPrintTime));
            CreateCalculationViewInfos();
            try
            {
                if (e != null)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach (var calc in e.NewItems)
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

        void Worksteps_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CreateWorkstepViewInfos();
            SettingsManager.Save();
        }

        void Materials_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CreateMaterialViewInfos();
            SettingsManager.Save();
        }
        void Printers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CreatePrinterViewInfos();
            SettingsManager.Save();
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
                                Files.Add(new File3d()
                                {
                                    Id = gc.Id,
                                    FileName = gc.FileName,
                                    Name = gc.FileName,
                                    FilePath = gc.FilePath,
                                    PrintTime = gc.PrintTime,
                                    Volume = gc.ExtrudedFilamentVolume,
                                });
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
        #endregion
        #endregion

        #region ICommand & Actions

        #region HostView
        public ICommand RefreshDashboardCommand
        {
            get { return new RelayCommand((p) => RefreshDashboardAction()); }
        }
        void RefreshDashboardAction()
        {
            IsRefreshing = true;
            try
            {
                

            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
            IsRefreshing = false;
        }

        public ICommand OpenDocumentationCommand
        {
            get { return new RelayCommand((p) => OpenDocumentationAction(p)); }
        }

        void OpenDocumentationAction(object parameter)
        {
            string modul = parameter as string;
            if (string.IsNullOrEmpty(modul)) return;
            switch (modul)
            {
                case "GcodeParser":
                    DocumentationManager.OpenDocumentation(DocumentationIdentifier.GcodeParser);
                    break;

                default:
                    DocumentationManager.OpenDocumentation(DocumentationIdentifier.Default);
                    break;
            }
        }

        public ICommand GoProCommand
        {
            get => new RelayCommand(async (p) => await GoProAction());
        }
        async Task GoProAction()
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

        public ICommand ClearFormCommand
        {
            get => new RelayCommand((p) => ClearFormAction());

        }
        void ClearFormAction()
        {
            try
            {
                Name = string.Empty;
                SelectedPrintersView = DefaultPrinters.Count == 0 ? new ArrayList() : new ArrayList(PrinterViews.OfType<PrinterViewInfo>()
                                .Where(printerview => DefaultPrinters.Contains(printerview.Printer))
                                .ToList());
                SelectedMaterialsView = DefaultMaterials.Count == 0 ? new ArrayList() : new ArrayList(MaterialViews.OfType<MaterialViewInfo>()
                                .Where(materialview => DefaultMaterials.Contains(materialview.Material))
                                .ToList());
                SelectedWorkstepsView = DefaultWorksteps.Count == 0 ? new ArrayList() : new ArrayList(WorkstepViews.OfType<WorkstepViewInfo>()
                                .Where(workstepview => DefaultWorksteps.Contains(workstepview.Workstep))
                                .ToList());
                Files = new ObservableCollection<File3d>();
                Volume = 0;
                Duration = 0;
                Pieces = 1;
                Gcode = null;
                Calculation = null;
                OnPropertyChanged(nameof(CanBeSaved));
                //logger.Info(Strings.EventFormCleared);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        #endregion

        #region Calculation
        public ICommand SendGCodeFilesCommand
        {
            get { return new RelayCommand((p) => SendGCodeFilesAction(p)); }
        }
        void SendGCodeFilesAction(object parameter)
        {
            try
            {
                List<Gcode> failedGcodes = new();

                string printServer = parameter as string;
                if (string.IsNullOrEmpty(printServer)) return;

                //Message to Host to send files

            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand CalculateCommand
        {
            get { return new RelayCommand(async (p) => await CalculateAction()); }
        }
        async Task CalculateAction()
        {
            try
            {

                var printers = SelectedPrintersView != null ?
                                new ObservableCollection<Printer3d>(SelectedPrintersView.OfType<PrinterViewInfo>().ToList().Select(p => p.Printer).ToList()) :
                                new ObservableCollection<Printer3d>();
                var materials = SelectedMaterialsView != null ?
                                new ObservableCollection<Material3d>(SelectedMaterialsView.OfType<MaterialViewInfo>().ToList().Select(p => p.Material).ToList()) :
                                new ObservableCollection<Material3d>();

                if (Calculation == null || NewCalculationWhenCalculate)
                {
                    Calculation = new Calculation3d()
                    {
                        Id = Guid.NewGuid(),
                    };
                }
                Calculation.Name = this.Name;
                Calculation.FailRate = this.FailRate;
                Calculation.PowerLevel = this.PowerLevel;
                Calculation.EnergyCostsPerkWh = Convert.ToDouble(this.EnergyCosts);
                Calculation.ApplyenergyCost = this.ApplyEnergyCosts;

                Calculation.Printers = printers;
                Calculation.Materials = materials;

                Calculation.WorkSteps = SelectedWorkstepsView != null ?
                                new ObservableCollection<Workstep>(SelectedWorkstepsView.OfType<WorkstepViewInfo>().ToList().Select(p => p.Workstep).ToList()) :
                                new ObservableCollection<Workstep>();
                Calculation.CustomAdditions = SelectedCustomAdditions != null && ApplyCustomAdditions ?
                                new ObservableCollection<CustomAddition>(SelectedCustomAdditions.OfType<CustomAddition>().ToList()) :
                                new ObservableCollection<CustomAddition>();
                Calculation.Rates = new ObservableCollection<CalculationAttribute>()
                {
                    new CalculationAttribute() {Attribute = "TaxRate", Type = CalculationAttributeType.Tax, Value = this.Taxrate, IsPercentageValue = true, SkipForCalculation = !ApplyTaxRate},
                    new CalculationAttribute() {Attribute = "Margin", Type = CalculationAttributeType.Margin, Value = this.Margin, IsPercentageValue = true},
                };
                Calculation.FixCosts = new ObservableCollection<CalculationAttribute>()
                {
                    new CalculationAttribute() { Attribute = "HandlingFee", Type= CalculationAttributeType.FixCost, Value = this.HandlingFee},
                };
                Calculation.Files = Files;

                Calculation.ProcedureAttributes = new ObservableCollection<CalculationProcedureAttribute>();

                Calculation.ApplyProcedureSpecificAdditions = ApplyProcedureSpecificAdditions;
                if (ApplyProcedureSpecificAdditions)
                {
                    Calculation.Procedure = Procedure;
                    List<CalculationProcedureParameter> parameters = new();
                    SerializableDictionary<string, double> additionalInfo = new();
                    //case Material3dFamily.Filament:
                    if (ApplyNozzleWearCosts)
                    {
                        if (NozzleWearFactorPerPrintJob > 0 && NozzleReplacementCosts > 0)
                        {
                            // Needed if the calculation is reloaded later
                            additionalInfo = new();
                            additionalInfo.Add("replacementcosts", NozzleReplacementCosts);
                            additionalInfo.Add("wearfactor", NozzleWearFactorPerPrintJob);

                            //List<CalculationProcedureParameter> paramters = new List<CalculationProcedureParameter>();
                            parameters = new List<CalculationProcedureParameter>
                            {
                                new CalculationProcedureParameter()
                                {
                                    Type = ProcedureParameter.NozzleWearCosts,
                                    Value = NozzleWearCostsPerPrintJob,
                                    AdditionalInformation = additionalInfo,
                                }
                            };

                            Calculation.ProcedureAttributes.Add(
                                new CalculationProcedureAttribute()
                                {
                                    Attribute = ProcedureAttribute.NozzleWear,
                                    Family = Material3dFamily.Filament,
                                    Parameters = parameters,
                                    Level = CalculationLevel.Printer,

                                });
                        }
                    }
                    if (ApplyPrintSheetWearCosts)
                    {
                        if (PrintSheetWearFactorPerPrintJob > 0 && PrintSheetReplacementCosts > 0)
                        {
                            // Needed if the calculation is reloaded later
                            additionalInfo = new SerializableDictionary<string, double>
                            {
                                { "replacementcosts", PrintSheetReplacementCosts },
                                { "wearfactor", PrintSheetWearFactorPerPrintJob }
                            };

                            //List<CalculationProcedureParameter> paramters = new List<CalculationProcedureParameter>();
                            parameters = new List<CalculationProcedureParameter>
                            {
                                new CalculationProcedureParameter()
                                {
                                    Type = ProcedureParameter.PrintSheetWearCosts,
                                    Value = PrintSheetWearCostsPerPrintJob,
                                    AdditionalInformation = additionalInfo,
                                }
                            };

                            Calculation.ProcedureAttributes.Add(
                                new CalculationProcedureAttribute()
                                {
                                    Attribute = ProcedureAttribute.PrintSheetWear,
                                    Family = Material3dFamily.Filament,
                                    Parameters = parameters,
                                    Level = CalculationLevel.Printer,
                                });
                        }
                    }
                    //case Material3dFamily.Resin:
                    if (ApplyResinGlovesCosts)
                    {
                        if (CostPerGlove > 0 && GlovesPerPrintJob > 0)
                        {
                            // Needed if the calculation is reloaded later
                            additionalInfo = new SerializableDictionary<string, double>
                            {
                                { "amount", GlovesInPackage },
                                { "price", GlovesPackagePrice },
                                { "perjob", GlovesPerPrintJob }
                            };

                            parameters = new List<CalculationProcedureParameter>
                            {
                                new CalculationProcedureParameter()
                                {
                                    Type = ProcedureParameter.GloveCosts,
                                    Value = GlovesPerPrintJob * CostPerGlove,
                                    AdditionalInformation = additionalInfo,
                                }
                            };
                            Calculation.ProcedureAttributes.Add(
                                new CalculationProcedureAttribute()
                                {
                                    Attribute = ProcedureAttribute.GlovesCosts,
                                    Family = Material3dFamily.Resin,
                                    Parameters = parameters,
                                    Level = CalculationLevel.Printer,
                                });
                        }
                    }
                    if (ApplyResinWashingCosts)
                    {
                        if (CostPerLiterIsopropanol > 0 && IsopropanolPerPrintJob > 0)
                        {
                            // Needed if the calculation is reloaded later
                            additionalInfo = new SerializableDictionary<string, double>
                            {
                                { "amount", IsopropanolContainerContent },
                                { "price", IsopropanolContainerPrice },
                                { "perjob", IsopropanolPerPrintJob }
                            };

                            parameters = new List<CalculationProcedureParameter>
                            {
                                new CalculationProcedureParameter()
                                {
                                    Type = ProcedureParameter.WashingCosts,
                                    Value = CostPerLiterIsopropanol * IsopropanolPerPrintJob,
                                    AdditionalInformation = additionalInfo,
                                }
                            };
                            Calculation.ProcedureAttributes.Add(
                                new CalculationProcedureAttribute()
                                {
                                    Attribute = ProcedureAttribute.WashingCosts,
                                    Family = Material3dFamily.Resin,
                                    Parameters = parameters,
                                    Level = CalculationLevel.Printer,
                                });
                        }
                    }
                    if (ApplyResinFilterCosts)
                    {
                        if (CostPerFilter > 0 && FiltersPerPrintJob > 0)
                        {
                            // Needed if the calculation is reloaded later
                            additionalInfo = new SerializableDictionary<string, double>
                            {
                                { "amount", FiltersInPackage },
                                { "price", FiltersPackagePrice },
                                { "perjob", FiltersPerPrintJob }
                            };

                            parameters = new List<CalculationProcedureParameter>
                            {
                                new CalculationProcedureParameter()
                                {
                                    Type = ProcedureParameter.FilterCosts,
                                    Value = CostPerFilter * FiltersPerPrintJob,
                                    AdditionalInformation = additionalInfo,
                                }
                            };
                            Calculation.ProcedureAttributes.Add(
                                new CalculationProcedureAttribute()
                                {
                                    Attribute = ProcedureAttribute.FilterCosts,
                                    Family = Material3dFamily.Resin,
                                    Parameters = parameters,
                                    Level = CalculationLevel.Printer,
                                });
                        }
                    }

                    //case Material3dFamily.Powder:
                    if (ApplySLSRefreshing)
                    {
                        if (PowderInBuildArea > 0)
                        {
                            // Needed if the calculation is reloaded later
                            additionalInfo = new();

                            //List<CalculationProcedureParameter> paramters = new List<CalculationProcedureParameter>();
                            parameters = new List<CalculationProcedureParameter>
                            {
                                new CalculationProcedureParameter()
                                {
                                    Type = ProcedureParameter.MinPowderNeeded,
                                    Value = PowderInBuildArea,
                                    AdditionalInformation = additionalInfo,
                                }
                            };
                            Calculation.ProcedureAttributes.Add(
                                new CalculationProcedureAttribute()
                                {
                                    Attribute = ProcedureAttribute.MaterialRefreshingRatio,
                                    Family = Material3dFamily.Powder,
                                    Parameters = parameters,
                                    Level = CalculationLevel.Material,
                                });
                        }
                    }

                    //case Material3dFamily.Misc:
                }

                Calculation.ApplyEnhancedMarginSettings = ApplyEnhancedMarginSettings;
                Calculation.ExcludeWorkstepsFromMarginCalculation = ExcludeWorkstepsFromMarginCalculation;
                Calculation.ExcludePrinterCostsFromMarginCalculation = ExcludePrinterCostsFromMarginCalculation;
                Calculation.ExcludeMaterialCostsFromMarginCalculation = ExcludeMaterialCostsFromMarginCalculation;


                Calculation.Calculate();
                OnPropertyChanged(nameof(Calculation));
                OnPropertyChanged(nameof(TotalPrice));
                OnPropertyChanged(nameof(TotalPrintTime));
                OnPropertyChanged(nameof(CanBeSaved));

                if (!Calculations.Contains(Calculation))
                    Calculations.Add(Calculation);

                try
                {
                    string filePath = Path.Combine(SettingsManager.GetSettingsLocation(), "calculations.xml");
                    SharedCalculatorInstance.SaveCurrentSessionCalculations(filePath);
                }
                catch (Exception exc)
                {
                    logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                }
                
                SelectedInfoTab = 0;
                logger.Info(string.Format(Strings.EventCalculationDoneFormated, Calculation.Name));
                await ShowCalculationResult(Calculation);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand NewCalculationCommand
        {
            get { return new RelayCommand((p) => NewCalculationAction()); }
        }
        void NewCalculationAction()
        {
            try
            {
                Calculation = null;
                ClearFormAction();
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand SelectedCalculationChangedCommand
        {
            get => new RelayCommand(async (p) => await SelectedCalculationChangedAction(p));
        }
        async Task SelectedCalculationChangedAction(object p)
        {
            try
            {
                //Calculation3d calculation = p as Calculation3d;
                if (Calculation != null)
                {
                    await LoadCalculation(Calculation);
                }
                else
                {

                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand ShowCalculationCommand
        {
            get => new RelayCommand(async (p) => await ShowCalculationResult(p as Calculation3d));
        }
        async Task ShowCalculationResult(Calculation3d calc)
        {
            try
            {
                if (calc == null || IsShowingCalculationResult)
                    return;

                IsShowingCalculationResult = true;
                var _dialog = new CustomDialog()
                {
                    Title = Strings.CalculationResult,

                };
                //_dialog.Style = Application.Current.FindResource("FullWidthDialogStyle") as Style;
                var calcResultViewModel = new CalculationResultsViewModel(async instance =>
                {

                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    IsShowingCalculationResult = false;

                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    IsShowingCalculationResult = false;
                }
                , calc
                , _dialogCoordinator
                );

                _dialog.Content = new Views._3dPrinting.CalculationResultDialogView()
                {
                    DataContext = calcResultViewModel
                };
                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        #endregion

        #region File Commands
        public ICommand SaveCalculationCommand
        {
            get => new RelayCommand(async (p) => await SaveCalculationAction());
        }
        async Task SaveCalculationAction()
        {
            try
            {
                if (Calculation != null)
                {
                    var saveFileDialog = new System.Windows.Forms.SaveFileDialog
                    {
                        Filter = StaticStrings.FilterCalculationFileLibrary,
                    };
                    if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        try
                        {
                            Calculator3dExporter.EncryptAndSerialize(saveFileDialog.FileName, this.Calculation);
                            logger.Info(string.Format(Strings.EventCalculationSavedFormated, Calculation.Name, saveFileDialog.FileName));
                        }
                        catch (Exception exc)
                        {
                            await this._dialogCoordinator.ShowMessageAsync(this,
                                Strings.DialogSaveCalculationFailedHeadline,
                                Strings.DialogSaveCalculationFailedContent
                                );
                            logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                        }
                    }
                }
                else
                    await this._dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogRunCalculationFirstHeadline,
                        Strings.DialogRunCalculationFirstContent
                        );
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand LoadCalculationCommand
        {
            get => new RelayCommand(async (p) => await LoadCalculationAction());
        }
        async Task LoadCalculationAction()
        {
            try
            {
                System.Windows.Forms.OpenFileDialog openFileDialog = new()
                {
                    // Allow to also load all calculations for awhile
                    Filter = StaticStrings.FilterCalculationFileAll,
                };

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        Calculation3d temp = new();
                        string fileExtension = Path.GetExtension(openFileDialog.FileName);
                        switch (fileExtension)
                        {
                            // Old calculation format
                            case ".3dc":
                                throw new NotSupportedException(string.Format(Strings.FormatNotSupportedAnyMore, fileExtension));
                            case ".3dcx":
                                temp = Calculator3dExporter.DecryptAndDeserialize(openFileDialog.FileName);
                                break;
                            default:
                                break;
                        }
                        await LoadCalculation(temp);
                    }
                    catch (Exception exc)
                    {
                        await this._dialogCoordinator.ShowMessageAsync(this,
                            Strings.DialogLoadCalculationFailedHeadline,
                            Strings.DialogLoadCalculationFailedContent
                        );
                        logger.Error(exc.Message);
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand SelectedGcodeChangedCommand
        {
            get => new RelayCommand((p) => SelectedGcodeChangedAction(p));
        }
        void SelectedGcodeChangedAction(object p)
        {
            try
            {
                Gcode gcode = Gcode;
                // Update 2D/3D viewers
                Messenger.Default.Send(new GcodesChangedMessage() { Gcode = gcode, Action = MessagingAction.SetSelected });
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand AddCustomAdditionCommand
        {
            get { return new RelayCommand(async (p) => await AddCustomAdditionAction()); }
        }
        async Task AddCustomAdditionAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.NewCustomAddition };
                var newViewModel = new NewCustomAdditionViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    CustomAdditions.Add(new CustomAddition()
                    {
                        Name = instance.Name,
                        Order = instance.Order,
                        Percentage = instance.Percentage,
                        CalculationType = instance.CalculationType,
                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, CustomAdditions[CustomAdditions.Count - 1].Name));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                    this._dialogCoordinator
                );

                _dialog.Content = new Views._3dPrinting.NewCustomAdditionView()
                {
                    DataContext = newViewModel
                };

                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
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
                await _dialogCoordinator.ShowMessageAsync(this,
                    Strings.DialogExceptionHeadline,
                    string.Format(Strings.DialogExceptionFormatedContent, exc.Message)
                    );
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
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (Color)Application.Current.Resources["MahApps.Colors.Gray2"] },
                    Padding = new Thickness(50),
                    //Width = Application.Current.MainWindow.ActualWidth * 0.8,
                    //Height = Application.Current.MainWindow.ActualHeight * 0.8,

                };
                //_childWindow.Style = Application.Current.FindResource("FullWidthDialogStyle") as Style;
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
                    _ = _childWindow.Close();
                },
                    _dialogCoordinator,
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
                {
                    return;
                }
                else if (SelectedStlFiles.Count == 1)
                {
                    string path = Path.GetDirectoryName(StlFile.StlFilePath);
                    _ =Process.Start(path);
                    logger.InfoFormat(Strings.EventOpenFolderFormated, path);
                }
                else
                {
                    MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogOpenMultipleFileLocationsHeadline,
                        Strings.DialogOpenMultipleFileLocationsContent
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        foreach (Stl file in SelectedStlFiles)
                        {
                            string path = Path.GetDirectoryName(file.StlFilePath);
                            _ = Process.Start(path);
                            logger.InfoFormat(Strings.EventOpenFolderFormated, path);
                        }
                        OnPropertyChanged(nameof(StlFiles));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
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
                    string path = Path.GetDirectoryName(Gcode.FilePath);
                    _ = Process.Start(path);
                    logger.InfoFormat(Strings.EventOpenFolderFormated, path);
                }
                else
                {
                    MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogOpenMultipleFileLocationsHeadline,
                        Strings.DialogOpenMultipleFileLocationsContent
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        foreach (Gcode file in SelectedGcodeFiles)
                        {
                            string path = Path.GetDirectoryName(file.FilePath);
                            _ =  Process.Start(path);
                            logger.InfoFormat(Strings.EventOpenFolderFormated, path);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
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
                ChildWindow _childWindow = new()
                {
                    Title = Strings.GCodeInfo,
                    AllowMove = true,
                    ShowCloseButton = false,
                    CloseByEscape = false,
                    IsModal = true,
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (System.Windows.Media.Color)Application.Current.Resources["MahApps.Colors.Gray2"] },
                    Padding = new Thickness(50),
                };
                GcodeViewModel newGcodeViewDialogViewModel = new(instance =>
                {
                    _ = _childWindow.Close();

                }, instance =>
                {
                    _ = _childWindow.Close();
                },
                    _dialogCoordinator,
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
            }
        }

        public ICommand OpenGCodeViewerNewWindowCommand
        {
            get { return new RelayCommand((p) => OpenGCodeViewerNewWindowAction()); }
        }
        void OpenGCodeViewerNewWindowAction()
        {
            try
            {
                Messenger.Default.Send(new NotificationMessage(SelectedGcodeFiles, "ShowGcodeEditor"));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
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
                MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
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
                            Messenger.Default.Send(new GcodesChangedMessage() { Gcode = file, Action = MessagingAction.Remove });
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

        public ICommand LoadCalculationIntoCalculatorFromTemplateCommand
        {
            get => new RelayCommand((p) => LoadCalculationIntoCalculatorFromTemplateAction(p));
        }
        void LoadCalculationIntoCalculatorFromTemplateAction(object calc)
        {
            try
            {
                if (calc is not Calculation3d currentCalculation)
                {
                    return;
                }
                Calculation = currentCalculation;
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        #endregion

        #region Gcode
        public ICommand SendGCodeFileCommand
        {
            get { return new RelayCommand(async (p) => await SendGCodeFileAction(p)); }
        }
        async Task SendGCodeFileAction(object parameter)
        {
            try
            {
                if (Gcode == null)
                {
                    await _dialogCoordinator.ShowMessageAsync(this,
                                    Strings.DialogNoFileSelectedHeadline,
                                    Strings.DialogNoFileSelectedContent,
                                    MessageDialogStyle.Affirmative);
                    return;
                }

                if (Gcode.IsOctoPrintGcodeAnalysis)
                {
                    await _dialogCoordinator.ShowMessageAsync(this,
                                    Strings.DialogGcodeCannotBeSentHeadline,
                                    string.Format(Strings.DialogGcodeCannotBeSentFormatContent, Gcode.FileName)
                                    , MessageDialogStyle.Affirmative);
                    return;
                }

                string printServer = parameter as string;
                if (string.IsNullOrEmpty(printServer)) return;
                IsSendingGcode = true;

                // Use messenger for this


                IsSendingGcode = false;
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                IsSendingGcode = false;
            }
        }

        public ICommand DeleteSelectedGcodeFileCommand
        {
            get => new RelayCommand((p) => DeleteSelectedGcodeFileAction(p));
        }
        void DeleteSelectedGcodeFileAction(object p)
        {
            try
            {
                if (p is Gcode gcode)
                {
                    if (Gcode == gcode)
                    {
                        // Clear 2D/3D views on deletion
                        Messenger.Default.Send(new GcodesChangedMessage() { Gcode = gcode, Action = MessagingAction.ClearHelixView });
                    }
                    Messenger.Default.Send(new GcodesChangedMessage() { Gcode = gcode, Action = MessagingAction.Remove });

                    Gcodes.Remove(gcode);
                    logger.Info(string.Format(Strings.EventDeletedItemFormated, gcode.FileName));
                    //this.ReplacementCosts = printer.Price;
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }

        }

        public ICommand AddPrintInformationManuallyCommand
        {
            get => new RelayCommand(async (p) => await AddPrintInformationManuallyAction());
        }
        async Task AddPrintInformationManuallyAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.ManualPrintJobInformation };
                var newViewModel = new ManualPrintJobInfoDialogViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    // Either use the volume or the weight
                    double volume = -1;
                    ModelWeight weight = null;
                    if (instance.UseVolumeForCalculation)
                        volume = instance.Volume;
                    else
                        weight = new ModelWeight()
                        {
                            Weight = instance.Weight,
                            Unit = instance.Unit
                        };

                    Files.Add(new File3d()
                    {
                        Name = instance.Name,
                        FileName = instance.Name,
                        PrintTime = instance.Duration,
                        Volume = volume,
                        Weight = weight,
                        Quantity = instance.Quantity,
                    });
                    if(Files.Count > 0)
                        logger.Info(string.Format(Strings.EventAddedItemFormated, Files[Files.Count - 1].Name));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                }
                , this._dialogCoordinator
                );

                _dialog.Content = new Views._3dPrinting.ManualPrintJobInfoDialog()
                {
                    DataContext = newViewModel
                };

                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
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

        public ICommand SelectGcodeFromListCommand
        {
            get => new RelayCommand(async (p) => await SelectGcodeFromListAction());
        }
        async Task SelectGcodeFromListAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.SelectedGcode };
                var newViewModel = new SelectGcodesDialogViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    foreach (var file in instance.Gcodes)
                    {
                        try
                        {
                            Files.Add(new File3d()
                            {
                                Id = file.Id,
                                FileName = file.FileName,
                                Name = file.FileName,
                                FilePath = file.FilePath,
                                PrintTime = file.PrintTime,
                                Volume = file.ExtrudedFilamentVolume,
                            });
                        }
                        catch (Exception exc) 
                        {
                            logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                            continue; 
                        }
                    }
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                    this._dialogCoordinator
                );

                _dialog.Content = new Views._3dPrinting.SelectGcodesDialogView()
                {
                    DataContext = newViewModel
                };

                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
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

        // Actions from template
        #region Template Commands

        #region Printer
        public ICommand EditPrinterCommand
        {
            get => new RelayCommand(async (p) => await EditPrinterAction(p));
        }
        async Task EditPrinterAction(object printer)
        {
            try
            {
                if (printer is not Printer3d selectedPrinter)
                {
                    return;
                }
                CustomDialog _dialog = new() { Title = Strings.EditPrinter };
                New3DPrinterViewModel newPrinterViewModel = new(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    UpdatePrinterFromInstance(instance, selectedPrinter);
                    logger.Info(string.Format(Strings.EventAddedItemFormated, selectedPrinter, Printers[Printers.Count - 1].Name));
                }, instance =>
                {
                    _ = _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                _dialogCoordinator,
                selectedPrinter
                );

                _dialog.Content = new Views._3dPrinting.NewPrinterDialog()
                {
                    DataContext = newPrinterViewModel
                };
                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand DeletePrinterCommand
        {
            get => new RelayCommand(async (p) => await DeletePrinterAction(p));
        }
        async Task DeletePrinterAction(object p)
        {
            try
            {
                if (p is Printer3d printer)
                {
                    MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogDeletePrinterHeadline, Strings.DialogDeletePrinterContent,
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        _ = Printers.Remove(printer);
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, printer.Name));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand DuplicatePrinterCommand
        {
            get => new RelayCommand((p) => DuplicatePrinterAction(p));
        }
        void DuplicatePrinterAction(object p)
        {
            try
            {
                if (p is Printer3d printer)
                {
                    IEnumerable<Printer3d> duplicates = Printers.Where(prt => prt.Model.StartsWith(prt.Model.Split('_')[0]));

                    Printer3d newPrinter = (Printer3d)printer.Clone();
                    newPrinter.Id = Guid.NewGuid();
                    newPrinter.Model = string.Format("{0}_{1}", newPrinter.Model.Split('_')[0], duplicates.Count());
                    Printers.Add(newPrinter);
                    PrinterViews.Refresh();

                    logger.Info(string.Format(Strings.EventAddedItemFormated, newPrinter.Name));
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        #endregion

        #region Material
        public ICommand DuplicateMaterialCommand
        {
            get => new RelayCommand((p) => DuplicateMaterialAction(p));
        }
        void DuplicateMaterialAction(object p)
        {
            try
            {
                if (p is Material3d material)
                {
                    var duplicates = Materials.Where(mat => mat.Name.StartsWith(material.Name.Split('_')[0]));

                    Material3d newMaterial = (Material3d)material.Clone();
                    newMaterial.Id = Guid.NewGuid();
                    newMaterial.Name = string.Format("{0}_{1}", newMaterial.Name.Split('_')[0], duplicates.Count());
                    Materials.Add(newMaterial);
                    MaterialViews.Refresh();

                    logger.Info(string.Format(Strings.EventAddedItemFormated, newMaterial.Name));
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand EditMaterialCommand
        {
            get => new RelayCommand(async (p) => await EditMaterialAction(p));
        }
        async Task EditMaterialAction(object material)
        {
            try
            {
                if (material is not Material3d selectedMaterial)
                {
                    return;
                }
                var _dialog = new CustomDialog() { Title = Strings.EditMaterial };
                var newMaterialViewModel = new NewMaterialViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    UpdateMaterialFromInstance(instance, selectedMaterial);
                    logger.Info(string.Format(Strings.EventEditedItemFormated, selectedMaterial, Materials[Materials.Count - 1]));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                    _dialogCoordinator,
                    selectedMaterial
                );

                _dialog.Content = new Views._3dPrinting.NewMaterialDialog()
                {
                    DataContext = newMaterialViewModel
                };
                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
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

        public ICommand DeleteMaterialCommand
        {
            get => new RelayCommand(async (p) => await DeleteMaterialAction(p));
        }
        async Task DeleteMaterialAction(object p)
        {
            try
            {
                if (p is Material3d material)
                {
                    var res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogDeleteMaterialHeadline, Strings.DialogDeleteMaterialContent,
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        Materials.Remove(material);
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, Material.Name));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand ReorderMaterialCommand
        {
            get => new RelayCommand(async (p) => await ReorderMaterialAction(p));
        }
        async Task ReorderMaterialAction(object parameter)
        {
            try
            {
                string uri = parameter as string;
                if (!string.IsNullOrEmpty(uri))
                {
                    var res = await this._dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogOpenExternalUriHeadline,
                        string.Format(Strings.DialogOpenExternalUriFormatedContent, uri),
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
        #endregion

        #region Calculation
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
                    var res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogDeleteCalculationHeadline,
                        Strings.DialogDeleteCalculationContent,
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        // Clear also the form if it is the selected calculation
                        if (calc.Equals(Calculation))
                            ClearFormAction();
                        Calculations.Remove(calc);
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
                if (Calculations.Count == 0) return;
                if (AllCalcsSelected)
                {
                    SelectedCalculationsView = new ArrayList();
                }
                else
                {
                    SelectedCalculationsView = new ArrayList(CalculationViews.OfType<CalculationViewInfo>()
                                .ToList());
                }
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

        #endregion

        #region Workstep
        public ICommand EditWorkstepCommand
        {
            get => new RelayCommand(async (p) => await EditWorkstepAction(p));
        }
        async Task EditWorkstepAction(object workstep)
        {
            try
            {
                if (workstep is not Workstep selectedWorkstep)
                {
                    return;
                }
                CustomDialog _dialog = new() { Title = Strings.EditWorkstep };
                NewWorkstepViewModel newWorkstepViewModel = new(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    UpdateWorkstepFromInstance(instance, selectedWorkstep);
                    logger.Info(string.Format(Strings.EventEditedItemFormated, selectedWorkstep, Worksteps[Worksteps.Count - 1].Name));
                }, instance =>
                {
                    _ = _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                _dialogCoordinator,
                selectedWorkstep
                );

                _dialog.Content = new Views._3dPrinting.NewWorkstepDialog()
                {
                    DataContext = newWorkstepViewModel
                };
                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand DeleteWorkstepCommand
        {
            get => new RelayCommand(async (p) => await DeleteWorkstepAction(p));
        }
        async Task DeleteWorkstepAction(object p)
        {
            if (p is Workstep workstep)
            {
                MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                    Strings.DialogDeleteWorkstepHeadline, Strings.DialogDeleteWorkstepContent,
                    MessageDialogStyle.AffirmativeAndNegative
                    );
                if (res == MessageDialogResult.Affirmative)
                {
                    _ = Worksteps.Remove(workstep);
                    logger.Info(string.Format(Strings.EventDeletedItemFormated, workstep.Name));
                }
            }
        }

        public ICommand DuplicateWorkstepCommand
        {
            get => new RelayCommand((p) => DuplicateWorkstepAction(p));
        }
        void DuplicateWorkstepAction(object p)
        {
            try
            {
                if (p is Workstep workstep)
                {
                    IEnumerable<Workstep> duplicates = Worksteps.Where(ws => ws.Name.StartsWith(workstep.Name.Split('_')[0]));

                    Workstep newWorkstep = (Workstep)workstep.Clone();
                    newWorkstep.Id = Guid.NewGuid();
                    newWorkstep.Name = string.Format("{0}_{1}", newWorkstep.Name.Split('_')[0], duplicates.Count());
                    Worksteps.Add(newWorkstep);
                    WorkstepViews.Refresh();

                    logger.Info(string.Format(Strings.EventAddedItemFormated, newWorkstep.Name));
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        #endregion
        // END - Actions from template
        #endregion

        #endregion

        #region Methods

        #region UpdateInstances
        void UpdateMaterialFromInstance(NewMaterialViewModel instance, Material3d material)
        {
            try
            {
                _ = InstanceConverter.GetMaterialFromInstance(instance, material);

                SettingsManager.Current.PrinterMaterials = Materials;
                SettingsManager.Save();

                OnPropertyChanged(nameof(Materials));
                CreateMaterialViewInfos();
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.DialogExceptionFormatedContent, exc.Message, exc.TargetSite);
            }
        }
        void UpdatePrinterFromInstance(New3DPrinterViewModel instance, Printer3d printer)
        {
            try
            {
                _ = InstanceConverter.GetPrinterFromInstance(instance, printer);
                SettingsManager.Current.Printers = Printers;
                SettingsManager.Save();

                OnPropertyChanged(nameof(Printers));
                CreatePrinterViewInfos();
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.DialogExceptionFormatedContent, exc.Message, exc.TargetSite);
            }
        }
        void UpdateWorkstepFromInstance(NewWorkstepViewModel instance, Workstep workstep)
        {
            try
            {
                _ = InstanceConverter.GetWorkstepFromInstance(instance, workstep);
                SettingsManager.Current.Worksteps = Worksteps;
                SettingsManager.Save();

                OnPropertyChanged(nameof(Worksteps));
                CreateWorkstepViewInfos();
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.DialogExceptionFormatedContent, exc.Message, exc.TargetSite);
            }
        }
        #endregion

        #region Expander
        void ResizeGcodeMultiParse(bool dueToChangedSize)
        {
            _canGcodeMultiParseWidthChange = false;

            if (dueToChangedSize)
            {
                ExpandGcodeMultiParserView = Math.Abs(GcodeMultiParserWidth.Value - GlobalStaticConfiguration.GcodeMultiParser_WidthCollapsed) > GlobalStaticConfiguration.FloatPointFix;
            }
            else
            {
                if (ExpandGcodeMultiParserView)
                {
                    GcodeMultiParserWidth = Math.Abs(_tempGcodeMultiParseWidth - GlobalStaticConfiguration.GcodeMultiParser_WidthCollapsed) < GlobalStaticConfiguration.FloatPointFix ?
                        new GridLength(GlobalStaticConfiguration.GcodeMultiParser_DefaultWidthExpanded) :
                        new GridLength(_tempGcodeMultiParseWidth);
                }
                else
                {
                    _tempGcodeMultiParseWidth = GcodeMultiParserWidth.Value;
                    GcodeMultiParserWidth = new GridLength(GlobalStaticConfiguration.GcodeMultiParser_WidthCollapsed);
                }
            }

            _canGcodeMultiParseWidthChange = true;
        }
        #endregion

        #region Views
        void CreatePrinterViewInfos()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var selection = new ObservableCollection<Printer3d>(SelectedPrintersView.OfType<PrinterViewInfo>().ToList().Select(p => p.Printer).ToList());
                Canvas c = new();
                c.Children.Add(new PackIconMaterial { Kind = PackIconMaterialKind.Printer3d });
                PrinterViews = new CollectionViewSource
                {
                    Source = Printers.Select(p => new PrinterViewInfo()
                    {
                        Name = p.Name,
                        Printer = p,
                        Icon = c,
                        Group = (Printer3dType)Enum.Parse(typeof(Printer3dType), p.Type.ToString()),
                    }).ToList()
                }.View;
                PrinterViews.SortDescriptions.Add(new SortDescription(nameof(PrinterViewInfo.Group), ListSortDirection.Ascending));
                PrinterViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(PrinterViewInfo.Group)));
                // Restore selection
                SelectedPrintersView = new ArrayList(PrinterViews.OfType<PrinterViewInfo>()
                    .Where(printerview => selection.Contains(printerview.Printer))
                    .ToList());
            });
        }
        void CreateCalculationViewInfos()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var selection = new ObservableCollection<Calculation3d>(SelectedCalculationsView.OfType<CalculationViewInfo>().ToList().Select(calc => calc.Calculation).ToList());
                Canvas c = new();
                c.Children.Add(new PackIconMaterial { Kind = PackIconMaterialKind.Calculator });
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
        void CreateMaterialViewInfos()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ObservableCollection<Material3d> selection = new(SelectedMaterialsView.OfType<MaterialViewInfo>().ToList().Select(mat => mat.Material).ToList());
                Canvas c = new();
                _ = c.Children.Add(new PackIconModern { Kind = PackIconModernKind.Box });
                MaterialViews = new CollectionViewSource
                {
                    Source = Materials.Select(p => new MaterialViewInfo()
                    {
                        Name = p.Name,
                        Material = p,
                        Icon = c,
                        Group = (Material3dTypes)Enum.Parse(typeof(Material3dTypes), p.MaterialFamily.ToString()),
                    }).ToList()
                }.View;
                MaterialViews.SortDescriptions.Add(new SortDescription(nameof(MaterialViewInfo.Group), ListSortDirection.Ascending));
                MaterialViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(MaterialViewInfo.Group)));
                // Restore selection
                SelectedMaterialsView = new ArrayList(MaterialViews.OfType<MaterialViewInfo>()
                    .Where(view => selection.Contains(view.Material))
                    .ToList());
            });
        }
        void CreateWorkstepViewInfos()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ObservableCollection<Workstep> selection = new(SelectedWorkstepsView.OfType<WorkstepViewInfo>().ToList().Select(ws => ws.Workstep).ToList());
                Canvas c = new();
                _ = c.Children.Add(new PackIconModern { Kind = PackIconModernKind.Clock });
                WorkstepViews = new CollectionViewSource
                {
                    Source = Worksteps.Select(p => new WorkstepViewInfo()
                    {
                        Name = p.Name,
                        Workstep = p,
                        Icon = c,
                        Group = (WorkstepType)Enum.Parse(typeof(WorkstepType), p.Type.ToString()),
                    }).ToList()
                }.View;
                WorkstepViews.SortDescriptions.Add(new SortDescription(nameof(WorkstepViewInfo.Group), ListSortDirection.Ascending));
                WorkstepViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(WorkstepViewInfo.Group)));
                // Restore selection
                SelectedWorkstepsView = new ArrayList(WorkstepViews.OfType<WorkstepViewInfo>()
                    .Where(view => selection.Contains(view.Workstep))
                    .ToList());
            });
        }
        #endregion

        #region Calculation
        async Task LoadCalculation(Calculation3d calculation)
        {
            try
            {
                if (calculation != null)
                {

                    Calculation = calculation;
                    Name = Calculation.Name;
                    if (!Calculations.Contains(Calculation))
                        Calculations.Add(Calculation);

                    this.FailRate = Calculation.FailRate;
                    this.EnergyCosts = Calculation.EnergyCostsPerkWh;

                    ApplyProcedureSpecificAdditions = Calculation.ApplyProcedureSpecificAdditions;
                    if (Calculation.Procedure != Material3dFamily.Misc)
                        Procedure = Calculation.Procedure;
                    if (Calculation.ProcedureAttributes.Count > 0)
                    {
                        foreach (var attr in Calculation.ProcedureAttributes)
                        {
                            switch (attr.Attribute)
                            {
                                case ProcedureAttribute.NozzleTemperature:
                                    break;
                                case ProcedureAttribute.HeatedBedTemperature:
                                    break;
                                case ProcedureAttribute.MaterialRefreshingRatio:
                                    var ratio = attr.Parameters.FirstOrDefault(para => para.Type == ProcedureParameter.MinPowderNeeded);
                                    if (ratio != null)
                                    {
                                        PowderInBuildArea = ratio.Value;
                                        ApplySLSRefreshing = true;
                                    }
                                    else
                                        ApplySLSRefreshing = false;
                                    break;
                                case ProcedureAttribute.CuringCosts:
                                    var curingCosts = attr.Parameters.FirstOrDefault(para => para.Type == ProcedureParameter.CuringCosts);
                                    if (curingCosts != null)
                                    {
                                        ApplyResinCuringCosts = true;
                                    }
                                    else
                                        ApplyResinCuringCosts = false;
                                    break;
                                case ProcedureAttribute.GlovesCosts:
                                    var gloveCosts = attr.Parameters.FirstOrDefault(para => para.Type == ProcedureParameter.GloveCosts);
                                    if (gloveCosts != null)
                                    {
                                        ApplyResinGlovesCosts = true;
                                    }
                                    else
                                        ApplyResinGlovesCosts = false;
                                    break;
                                case ProcedureAttribute.WashingCosts:
                                    var washingCost = attr.Parameters.FirstOrDefault(para => para.Type == ProcedureParameter.WashingCosts);
                                    if (washingCost != null)
                                    {
                                        ApplyResinWashingCosts = true;
                                    }
                                    else
                                        ApplyResinWashingCosts = false;
                                    break;
                                case ProcedureAttribute.FilterCosts:
                                    var filterCosts = attr.Parameters.FirstOrDefault(para => para.Type == ProcedureParameter.FilterCosts);
                                    if (filterCosts != null)
                                    {
                                        ApplyResinFilterCosts = true;
                                    }
                                    else
                                        ApplyResinFilterCosts = false;
                                    break;
                                case ProcedureAttribute.Granulation:
                                    break;
                                case ProcedureAttribute.TensileStrength:
                                    break;
                                case ProcedureAttribute.ElongationAtBreak:
                                    break;
                                case ProcedureAttribute.ImpactResistance:
                                    break;
                                case ProcedureAttribute.ShoreHardness:
                                    break;
                                case ProcedureAttribute.MeltingPoint:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    var margin = Calculation.Rates.FirstOrDefault(rate => rate.Type == CalculationAttributeType.Margin);
                    if (margin != null)
                        Margin = margin.Value;
                    var handling = Calculation.FixCosts.FirstOrDefault(rate => rate.Type == CalculationAttributeType.FixCost && rate.Attribute == "HandlingFee");
                    if (handling != null)
                        HandlingFee = handling.Value;

                    var tax = Calculation.Rates.FirstOrDefault(rate => rate.Type == CalculationAttributeType.Tax);
                    if (tax != null)
                    {
                        ApplyTaxRate = !tax.SkipForCalculation;
                        Taxrate = tax.Value;
                    }

                    SelectedPrintersView = new ArrayList(PrinterViews.OfType<PrinterViewInfo>()
                        .Where(printerview => Calculation.Printers.Contains(printerview.Printer))
                        .ToList());

                    SelectedMaterialsView = new ArrayList(MaterialViews.OfType<MaterialViewInfo>()
                        .Where(materialview => Calculation.Materials.Contains(materialview.Material))
                        .ToList());

                    SelectedWorkstepsView = new ArrayList(WorkstepViews.OfType<WorkstepViewInfo>()
                        .Where(workstepview => Calculation.WorkSteps.Contains(workstepview.Workstep))
                        .ToList());
                    Files = Calculation.Files;

                    SelectedInfoTab = 0;
                    OnPropertyChanged(nameof(Calculation));
                    OnPropertyChanged(nameof(CanBeSaved));
                    logger.Info(string.Format(Strings.EventCalculationLoadedFormated, Calculation.Name, calculation.Name));
                    if (ShowCalculationResultPopup)
                    {
                        await ShowCalculationResult(Calculation);
                    }
                }
                else
                {
                    logger.Warn(string.Format(Strings.EventCalculationLoadFailedFormated, calculation.Name));
                }
            }
            catch (Exception exc)
            {
                await this._dialogCoordinator.ShowMessageAsync(this,
                    Strings.DialogLoadCalculationFailedHeadline,
                    Strings.DialogLoadCalculationFailedContent
                );
                logger.Error(exc.Message);
            }
        }

        #endregion
        
        public void OnViewVisible()
        {
            try
            {
                OnPropertyChanged(nameof(IsLicenseValid));
                LoadDefaults();

                CreateMaterialViewInfos();
                CreatePrinterViewInfos();
                CreateWorkstepViewInfos();
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
            }
        }

        public void OnViewHide()
        {

        }

        public void OnClose()
        {

        }
        #endregion
    }
}
