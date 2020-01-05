using WpfFramework.Models._3dprinting;
using WpfFramework.Models.Settings;
using WpfFramework.Utilities;
using HelixToolkit.Wpf;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using WpfFramework.Models;
using System.Collections;
using WpfFramework.Resources.Localization;
using log4net;
using WpfFramework.ViewModels.Slicer;
using MahApps.Metro.SimpleChildWindow;
using System.Windows.Media;
using System.Text.RegularExpressions;
using WpfFramework.Models.GCode;
using GalaSoft.MvvmLight.Messaging;

namespace WpfFramework.ViewModels._3dPrinting
{
    public class _3dPrintingCalculationViewModel : ViewModelBase
    {

        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool _isLoading;
        #endregion

        #region Properties
        private _3dPrinterCalculationModel _calculation;
        public _3dPrinterCalculationModel Calculation
        {
            get => _calculation;
            set
            {
                if (_calculation != value)
                {
                    _calculation = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool canBeSaved
        {
            get => Calculation != null;
        }
        public bool canSendGcode
        {
            get => ((Gcode != null));
        }

        private int _progress = 0;
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

        private int _progressLayerModel = 0;
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

        private bool _isWorking = false;
        public bool IsWorking
        {
            get => _isWorking;
            private set
            {
                if (_isWorking == value)
                    return;
                _isWorking = value;
                OnPropertyChanged();
            }
        }

        #region CalculationViews
        public ICollectionView CalculationViews
        {
            get => _CalculationViews;
            private set
            {
                if (_CalculationViews != value)
                {
                    _CalculationViews = value;
                    OnPropertyChanged();
                }
            }
        }
        private ICollectionView _CalculationViews;

        private CalculationViewInfo _selectedCalculationView;
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

        private IList _selectedCalculationsView = new ArrayList();
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
            private set
            {
                if (_PrinterViews != value)
                {
                    _PrinterViews = value;
                    OnPropertyChanged(nameof(PrinterViews));
                }
            }
        }
        private ICollectionView _PrinterViews;

        private PrinterViewInfo _selectedPrinterView;
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

        private IList _selectedPrintersView = new ArrayList();
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
            private set
            {
                if (_MaterialViews != value)
                {
                    _MaterialViews = value;
                    OnPropertyChanged(nameof(MaterialViews));
                }
            }
        }
        private ICollectionView _MaterialViews;

        private MaterialViewInfo _selectedMaterialView;
        public MaterialViewInfo SelectedMaterialView
        {
            get => _selectedMaterialView;
            set
            {
                if (_selectedMaterialView != value)
                {
                    _selectedMaterialView = value;
                    OnPropertyChanged(nameof(SelectedMaterialView));
                }
            }
        }

        private IList _selectedMaterialsView = new ArrayList();
        public IList SelectedMaterialsView
        {
            get => _selectedMaterialsView;
            set
            {
                if (_selectedMaterialsView != value)
                {
                    _selectedMaterialsView = value;
                    OnPropertyChanged(nameof(SelectedMaterialsView));
                }
            }
        }

        #endregion
        
        #region WorkstepViews
        public ICollectionView WorkstepViews
        {
            get => _WorkstepViews;
            private set
            {
                if (_WorkstepViews != value)
                {
                    _WorkstepViews = value;
                    OnPropertyChanged(nameof(WorkstepViews));
                }
            }
        }
        private ICollectionView _WorkstepViews;

        private WorkstepViewInfo _selectedWorkstepView;
        public WorkstepViewInfo SelectedWorkstepView
        {
            get => _selectedWorkstepView;
            set
            {
                if (_selectedWorkstepView != value)
                {
                    _selectedWorkstepView = value;
                    OnPropertyChanged(nameof(SelectedWorkstepView));
                }
            }
        }

        private IList _selectedWorkstepsView = new ArrayList();
        public IList SelectedWorkstepsView
        {
            get => _selectedWorkstepsView;
            set
            {
                if (_selectedWorkstepsView != value)
                {
                    _selectedWorkstepsView = value;
                    OnPropertyChanged(nameof(SelectedWorkstepsView));
                }
            }
        }
        #endregion

        private ObservableCollection<_3dPrinterCalculationModel> _calculations = new ObservableCollection<_3dPrinterCalculationModel>();
        public ObservableCollection<_3dPrinterCalculationModel> Calculations
        {
            get => _calculations;
            set
            {
                if (value != _calculations)
                {
                    _calculations = value;
                    OnPropertyChanged(nameof(Calculations));
                }
            }
        }

        public decimal TotalPrice
        {
            get
            {
                return Calculations.Sum(calc => calc.Total);
             }
        }
        public decimal TotalPrintTime
        {
            get
            {
                return Calculations.Sum(calc => calc.CalculatedPrintTime);
             }
        }

        public ObservableCollection<_3dPrinterMaterial> Materials
        {
            get => SettingsManager.Current._3dPrinterMaterials;
            set
            {
                if (value != SettingsManager.Current._3dPrinterMaterials)
                {
                    SettingsManager.Current._3dPrinterMaterials = value;
                    OnPropertyChanged(nameof(Materials));
                }
            }

        }
        public ObservableCollection<_3dPrinterModel> Printers
        {
            get => SettingsManager.Current._3dPrinters;
            set
            {
                if (value != SettingsManager.Current._3dPrinters)
                {
                    SettingsManager.Current._3dPrinters = value;
                    OnPropertyChanged(nameof(Printers));
                }
            }

        }

        private string _name;
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

        private _3dPrinterMaterial _material;
        public _3dPrinterMaterial Material
        {
            get => _material;
            set
            {
                if(_material != value)
                {
                    _material = value;
                    OnPropertyChanged(nameof(Material));
                }
            }
        }


        private Model3DGroup _model;
        public Model3DGroup Model
        {
            get => _model;
            set
            {
                if (_model != value)
                {
                    _model = value;
                    OnPropertyChanged();
                }
            }
        }
        /*
         * Position="0,0,100" 
            LookDirection="0,0,-100" 
            UpDirection="0,1,0.5" 
            FieldOfView="61" 
            NearPlaneDistance="0.001">
         */
        private PerspectiveCamera _gcodeCamera = new PerspectiveCamera(new Point3D(0,0,100), new Vector3D(0,0, -100), new Vector3D(0,1,0.5), 60)
        {
            NearPlaneDistance = 0.001,
        };
        public PerspectiveCamera GcodeCamera
        {
            get => _gcodeCamera;
            set
            {
                if (_gcodeCamera == value)
                    return;
                _gcodeCamera = value;
                Messenger.Default.Send(new NotificationMessage(SelectedGcodeFiles, "ResetCameraGcode")); 
                OnPropertyChanged();
            }
        }
        /*
         * <OrthographicCamera NearPlaneDistance="-1.7976931348623157E+8" FarPlaneDistance="1.7976931348623157E+8"
            Position="-30,-30,24" 
            LookDirection="70,70,-90"
            UpDirection="0.5,0.5,0.75"
            Width="250"
        />
         */

        private OrthographicCamera _gcode3dCamera = new OrthographicCamera(new Point3D(-30,-30, 24), new Vector3D(70,70, -100), new Vector3D(0.5,0.5,0.75), 60)
        {
            NearPlaneDistance = 0.001,
        };
        public OrthographicCamera Gcode3dCamera
        {
            get => _gcode3dCamera;
            set
            {
                if (_gcode3dCamera == value)
                    return;
                _gcode3dCamera = value;
                Messenger.Default.Send(new NotificationMessage(SelectedGcodeFiles, "ResetCameraGcode3d")); 
                OnPropertyChanged();
            }
        }
        
        
        private Point3DCollection _GcodePoints;
        public Point3DCollection GcodePoints
        {
            get => _GcodePoints;
            set
            {
                if(_GcodePoints != value)
                {
                    _GcodePoints = value;
                    OnPropertyChanged();
                }
            }
        }
        private Point3DCollection _Gcode3dPoints;
        public Point3DCollection Gcode3dPoints
        {
            get => _Gcode3dPoints;
            set
            {
                if(_Gcode3dPoints != value)
                {
                    _Gcode3dPoints = value;
                    OnPropertyChanged();
                }
            }
        }

        private Color _GcodeColor = Colors.Orange;
        public Color GcodeColor
        {
            get => _GcodeColor;
            set
            {
                if (_GcodeColor == value)
                    return;
                _GcodeColor = value;
                OnPropertyChanged();
            }
        }
        
        private int _GcodeThickness = 4;
        public int GcodeThickness
        {
            get => _GcodeThickness;
            set
            {
                if (_GcodeThickness == value)
                    return;
                _GcodeThickness = value;
                OnPropertyChanged();
            }
        }
        
        private int _GcodeLayer = 0;
        public int GcodeLayer
        {
            get => _GcodeLayer;
            set
            {
                if (_GcodeLayer == value)
                    return;
                _GcodeLayer = value;
                OnPropertyChanged();
            }
        }
        
        private int _GcodeMaxLayer = 0;
        public int GcodeMaxLayer
        {
            get => _GcodeMaxLayer;
            set
            {
                if (_GcodeMaxLayer == value)
                    return;
                _GcodeMaxLayer = value;
                OnPropertyChanged();
            }
        }


        private Stl _stlFile;
        public Stl StlFile
        {
            get => _stlFile;
            set
            {
                if (_stlFile != value)
                {
                    _stlFile = value;
                    if (_stlFile != null)
                    {
                        string path = _stlFile.StlFilePath;
                        createStlModel(path);
                    }
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<Stl> _stlFiles = new ObservableCollection<Stl>();
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

        private IList _selectedStlFiles = new ArrayList();
        public IList SelectedStlFiles
        {
            get => _selectedStlFiles;
            set
            {
                if (Equals(value, _selectedStlFiles))
                    return;

                _selectedStlFiles = value;
                OnPropertyChanged();
            }
        }
        
        private GCode _gcode;
        public GCode Gcode
        {
            get => _gcode;
            set
            {
                if(_gcode != value)
                {
                    _gcode = value;
                    if(_gcode != null)
                    { 
                        Volume = Convert.ToDecimal(_gcode.ExtrudedFilamentVolume);
                        Duration = Convert.ToDecimal(_gcode.PrintTime);
                        Name = _gcode.FileName;
                    }
                    else
                    {
                        Volume = 0;
                        Duration = 0;
                        Name = string.Empty;
                    }
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<GCode> _gcodes = new ObservableCollection<GCode>();
        public ObservableCollection<GCode> Gcodes
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

        private IList _selectedGcodeFiles = new ArrayList();
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

        private ObservableCollection<ModelVisual3D> _models = new ObservableCollection<ModelVisual3D>();
        public ObservableCollection<ModelVisual3D> Models
        {
            get => _models;
            set
            {
                if(_models != value)
                {
                    _models = value;
                    OnPropertyChanged(nameof(Models));
                }
            }
        }


        private ObservableCollection<ModelVisual3D> _Gcodemodels = new ObservableCollection<ModelVisual3D>();
        public ObservableCollection<ModelVisual3D> GcodeModels
        {
            get => _Gcodemodels;
            set
            {
                if (_Gcodemodels != value)
                {
                    _Gcodemodels = value;
                    OnPropertyChanged();
                }
            }
        }


        private int _margin = 30;
        public int Margin
        {
            get => _margin;
            set
            {
                if(_margin != value)
                {
                    _margin = value;
                    OnPropertyChanged(nameof(Margin));
                }
            }
        }
        
        private int _failRate = 5;
        public int FailRate
        {
            get => _failRate;
            set
            {
                if(_failRate != value)
                {
                    _failRate = value;
                    OnPropertyChanged();
                }
            }
        }
        

        private int _pieces = 1;
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

        private decimal _duration = 0;
        public decimal Duration
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
        
        private decimal _consumedMaterial = 0;
        public decimal ConsumedMaterial
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
        
        private decimal _volume = 0;
        public decimal Volume
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

        #region Settings 
        public bool ApplyEnergyCosts
        {
            get => SettingsManager.Current.ApplyEnergyCosts;
            set
            {
                if (SettingsManager.Current.ApplyEnergyCosts != value)
                {
                    SettingsManager.Current.ApplyEnergyCosts = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _selectedInfoTab = 0;
        public int SelectedInfoTab
        {
            get => _selectedInfoTab;
            set
            {
                if (_selectedInfoTab == value)
                    return;
                if (!_isLoading)
                    SettingsManager.Current.Calculation_SelectedInfoTab = value;
                _selectedInfoTab = value;
                OnPropertyChanged();
            }
        }

        public int PowerLevel
        {
            get => SettingsManager.Current.PowerLevel;
            set
            {
                if (SettingsManager.Current.PowerLevel != value)
                {
                    SettingsManager.Current.PowerLevel = value;
                    OnPropertyChanged();
                }
            }
        }
        public decimal EnergyCosts
        {
            get => SettingsManager.Current.EnergyCosts;
            set
            {
                if(SettingsManager.Current.EnergyCosts != value)
                {
                    SettingsManager.Current.EnergyCosts = value;
                    OnPropertyChanged(nameof(EnergyCosts));
                }
            }
        }

        public bool ApplyTaxRate
        {
            get => SettingsManager.Current.ApplyTaxRate;
            set
            {
                if (SettingsManager.Current.ApplyTaxRate != value)
                {
                    SettingsManager.Current.ApplyTaxRate = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal Taxrate
        {
            get => SettingsManager.Current.TaxRate;
            set
            {
                if (SettingsManager.Current.TaxRate != value)
                {
                    SettingsManager.Current.TaxRate = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowCameraInfo
        {
            get => SettingsManager.Current.ShowCameraInfo;
            set
            {
                if (SettingsManager.Current.ShowCameraInfo != value)
                {
                    SettingsManager.Current.ShowCameraInfo = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private bool _showGcodeInfos = false;
        public bool ShowGcodeInfos
        {
            get => _showGcodeInfos;
            set
            {
                if (_showGcodeInfos == value)
                    return;
                if (!_isLoading)
                    SettingsManager.Current.Calculation_ShowGcodeInfos = value;
                _showGcodeInfos = value;
                OnPropertyChanged();
            }
        }
        
        private bool _showGcodeGrid = false;
        public bool ShowGcodeGrid
        {
            get => _showGcodeGrid;
            set
            {
                if (_showGcodeGrid == value)
                    return;
                if (!_isLoading)
                    SettingsManager.Current.Calculation_ShowGcodeGrid = value;
                _showGcodeGrid = value;
                OnPropertyChanged();
            }
        }

        public decimal HandlingFee
        {
            get => SettingsManager.Current.HandlingFee;
            set
            {
                if (SettingsManager.Current.HandlingFee != value)
                {
                    SettingsManager.Current.HandlingFee = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Search
        private string _searchPrinter = string.Empty;
        public string SearchPrinter
        {
            get => _searchPrinter;
            set
            {
                if(_searchPrinter != value)
                {
                    _searchPrinter = value;

                    PrinterViews.Refresh();

                    ICollectionView view = CollectionViewSource.GetDefaultView(PrinterViews);
                    IEqualityComparer<String> comparer = StringComparer.InvariantCultureIgnoreCase;
                    view.Filter = o =>
                    {
                        PrinterViewInfo p = o as PrinterViewInfo;
                        return p.Name.Contains(_searchPrinter);
                    };
                    OnPropertyChanged(nameof(SearchPrinter));
                }
            }
        }
        
        private string _searchOffer = string.Empty;
        public string SearchOffer
        {
            get => _searchOffer;
            set
            {
                if(_searchOffer != value)
                {
                    _searchOffer = value;

                    CalculationViews.Refresh();

                    ICollectionView view = CollectionViewSource.GetDefaultView(CalculationViews);
                    IEqualityComparer<String> comparer = StringComparer.InvariantCultureIgnoreCase;
                    view.Filter = o =>
                    {
                        CalculationViewInfo p = o as CalculationViewInfo;
                        return p.Name.Contains(_searchOffer);
                    };
                    OnPropertyChanged();
                }
            }
        }

        private string _searchMaterial = string.Empty;
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
                        return m.Name.Contains(_searchMaterial);
                    };
                    
                    OnPropertyChanged(nameof(SearchMaterial));
                }
            }
        }

        private string _searchWorksteps = string.Empty;
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
                        return m.Name.Contains(_searchWorksteps);
                    };

                    OnPropertyChanged(nameof(SearchWorksteps));
                }
            }
        }
        private string _searchStlFiles = string.Empty;
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

        private string _searchGCodeFiles = string.Empty;
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
                        GCode m = o as GCode;
                        return m.FileName.Contains(_searchGCodeFiles) || m.FilePath.Contains(_searchGCodeFiles) || Regex.IsMatch(m.FileName, _searchGCodeFiles, RegexOptions.IgnoreCase);
                    };

                    OnPropertyChanged();
                }
            }
        }
        #endregion  

        #region Constructor
        public _3dPrintingCalculationViewModel(IDialogCoordinator dialog)
        {
            this._dialogCoordinator = dialog;
            _isLoading = true;

            LoadSettings();

            _isLoading = false;

            Printers.CollectionChanged += Printers_CollectionChanged;
            Materials.CollectionChanged += Materials_CollectionChanged;
            Calculations.CollectionChanged += Calculations_CollectionChanged;
            Gcodes.CollectionChanged += Gcodes_CollectionChanged;
            StlFiles.CollectionChanged += StlFiles_CollectionChanged;

            var lights = new ModelVisual3D();
            lights.Children.Add(new SunLight());
            Models.Add(lights);

            createPrinterViewInfos();
            createMaterialViewInfos();


            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }
        private void LoadSettings()
        {
            SelectedInfoTab = SettingsManager.Current.Calculation_SelectedInfoTab;
            ShowGcodeInfos = SettingsManager.Current.Calculation_ShowGcodeInfos;
            ShowGcodeGrid = SettingsManager.Current.Calculation_ShowGcodeGrid;
        }
        private void StlFiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(StlFiles));
        }

        private void Gcodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Gcodes));
        }

        private void Calculations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Calculations));
            OnPropertyChanged(nameof(TotalPrice));
            OnPropertyChanged(nameof(TotalPrintTime));
            createCalculationViewInfos();
         }

        #endregion

        #region Events
        private void Materials_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            createMaterialViewInfos();
            SettingsManager.Save();
        }
        private void Printers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            createPrinterViewInfos();
            SettingsManager.Save();
        }

        #endregion
        
        #region Private_Methods
        private void createPrinterViewInfos()
        {
            Canvas c = new Canvas();
            c.Children.Add(new PackIconMaterial { Kind = PackIconMaterialKind.Printer3d });
            PrinterViews = new CollectionViewSource
            {
                Source = (Printers.Select(p => new PrinterViewInfo()
                {
                    Name = p.Name,
                    Printer = p,
                    Icon = c,
                    Group = (PrinterViewManager.Group)Enum.Parse(typeof(PrinterViewManager.Group), p.Type.ToString()),
                })).ToList()
            }.View;
            PrinterViews.SortDescriptions.Add(new SortDescription(nameof(PrinterViewInfo.Group), ListSortDirection.Ascending));
            PrinterViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(PrinterViewInfo.Group)));
        }
        private void createCalculationViewInfos()
        {
            Canvas c = new Canvas();
            c.Children.Add(new PackIconMaterial { Kind = PackIconMaterialKind.Calculator });
            CalculationViews = new CollectionViewSource
            {
                Source = (Calculations.Select(calc => new CalculationViewInfo()
                {
                    Name = calc.ToString(),
                    Calculation = calc,
                    Icon = c,
                    Group = CalculationViewManager.Group.MISC,
                })).ToList()
            }.View;
            CalculationViews.SortDescriptions.Add(new SortDescription(nameof(CalculationViewInfo.Group), ListSortDirection.Ascending));
            CalculationViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(CalculationViewInfo.Group)));
        }
        private void createMaterialViewInfos()
        {
            Canvas c = new Canvas();
            c.Children.Add(new PackIconModern { Kind = PackIconModernKind.Box });
            MaterialViews = new CollectionViewSource
            {
                Source = (Materials.Select(p => new MaterialViewInfo()
                {
                    Name = p.Name,
                    Material = p,
                    Icon = c,
                    Group = (MaterialViewManager.Group)Enum.Parse(typeof(MaterialViewManager.Group), p.TypeOfMaterial.Kind.ToString()),
                })).ToList()
            }.View;
            MaterialViews.SortDescriptions.Add(new SortDescription(nameof(MaterialViewManager.Group), ListSortDirection.Ascending));
            MaterialViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(MaterialViewManager.Group)));
        }
        #endregion

        #region ICommand & Actions
        public ICommand GoProCommand
        {
            get => new RelayCommand(p => GoProAction());
        }
        private async void GoProAction()
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
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand ClearFormCommand
        {
            get =>  new RelayCommand(p => ClearFormAction()); 
                
        }
        private void ClearFormAction()
        {
            try
            {
                SelectedPrintersView = new ArrayList();
                SelectedMaterialsView = new ArrayList();
                SelectedWorkstepsView = new ArrayList();
                Volume = 0;
                Duration = 0;
                Pieces = 1;
                Gcode = null;
                Calculation = null;
                Model = null;
                OnPropertyChanged(nameof(canBeSaved));
                logger.Info(Strings.EventFormCleared);
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
        public ICommand CalculateCommand
        {
            get { return new RelayCommand(p => CalculateAction()); }
        }
        private void CalculateAction()
        {
            try
            {
                Calculation = new _3dPrinterCalculationModel()
                {
                    ApplyTax = this.ApplyTaxRate,
                    TaxRate = this.Taxrate,
                    FailRate = this.FailRate,
                    ApplyEnergyCosts = this.ApplyEnergyCosts,
                    PowerLevel = this.PowerLevel,
                    EnergyPrice = this.EnergyCosts,
                    HandlingFee = this.HandlingFee,
                    Profit = this.Margin,
                    Quantity = this.Pieces,
                    Duration = this.Duration,
                    ConsumedMaterial = this.ConsumedMaterial,
                    Volume = this.Volume,
                    GcodePath = this.Gcode != null ? this.Gcode.FilePath : string.Empty,
                    Name = this.Name,
                    Printers = SelectedPrintersView != null ?
                                    new ObservableCollection<_3dPrinterModel>(SelectedPrintersView.OfType<PrinterViewInfo>().ToList().Select(p => p.Printer).ToList()) :
                                    new ObservableCollection<_3dPrinterModel>(),
                    Materials = SelectedMaterialsView != null ?
                                    new ObservableCollection<_3dPrinterMaterial>(SelectedMaterialsView.OfType<MaterialViewInfo>().ToList().Select(p => p.Material).ToList()) :
                                    new ObservableCollection<_3dPrinterMaterial>(),
                };
                OnPropertyChanged(nameof(canBeSaved));
                Calculations.Add(Calculation);
                ShowCalculationResult(Calculation);

                logger.Info(string.Format(Strings.EventCalculationDoneFormated, Calculation.Name));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand ShowCalculationCommand
        {
            get => new RelayCommand(p => ShowCalculationResult(p as _3dPrinterCalculationModel));
        }
        private async void ShowCalculationResult(_3dPrinterCalculationModel calc)
        {
            try
            {
                if (calc == null)
                    return;
                var _dialog = new CustomDialog() { Title = Strings.CalculationResult };
                _dialog.Style = Application.Current.FindResource("FullWidthDialogStyle") as Style;
                var calcResultViewModel = new CalculationResultsViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
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

        #region File Commands
        public ICommand SaveCalculationCommand
        {
            get => new RelayCommand(p => SaveCalculationAction());
        }
        private async void SaveCalculationAction()
        {
            try
            {
                if (Calculation != null)
                {
                    var saveFileDialog = new System.Windows.Forms.SaveFileDialog
                    {
                        Filter = StaticStrings.FilterCalculationFile,
                    };
                    if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {

                        try
                        {
                            CalculationFile.EncryptAndSerialize(saveFileDialog.FileName, this.Calculation);
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
            get => new RelayCommand(p => LoadCalculationAction());
        }
        private async void LoadCalculationAction()
        {
            try
            {
                var openFileDialog = new System.Windows.Forms.OpenFileDialog
                {
                    Filter = StaticStrings.FilterCalculationFile,
                };

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        var res = CalculationFile.DecryptAndDeserialize(openFileDialog.FileName);
                        if (res != null)
                        {
                            Calculation = res;
                            this.ApplyTaxRate = Calculation.ApplyTax;
                            this.Taxrate = Calculation.TaxRate;
                            this.FailRate = Calculation.FailRate;
                            this.EnergyCosts = Calculation.EnergyPrice;
                            this.HandlingFee = Calculation.HandlingFee;
                            this.Margin = Calculation.Profit;
                            this.Pieces = Calculation.Quantity;
                            this.Duration = Calculation.Duration;
                            this.ConsumedMaterial = Calculation.ConsumedMaterial;
                            this.Volume = Calculation.Volume;

                            SelectedPrintersView = new ArrayList(PrinterViews.OfType<PrinterViewInfo>()
                                .Where(printerview => Calculation.Printers.Contains(printerview.Printer))
                                .ToList());
                            SelectedMaterialsView = new ArrayList(MaterialViews.OfType<MaterialViewInfo>()
                                .Where(materialview => Calculation.Materials.Contains(materialview.Material))
                                .ToList());

                            OnPropertyChanged(nameof(Calculation));
                            OnPropertyChanged(nameof(canBeSaved));
                            logger.Info(string.Format(Strings.EventCalculationLoadedFormated, Calculation.Name, openFileDialog.FileName));
                            if (SettingsManager.Current.General_OpenCalculationResultView)
                            {
                                ShowCalculationResult(Calculation);
                            }
                        }
                        else
                        {
                            logger.Warn(string.Format(Strings.EventCalculationLoadFailedFormated, openFileDialog.FileName));
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
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand ReadStlGcodeFileCommand
        {
            get => new RelayCommand(p => ReadStlGcodeFileAction());
        }
        private async void ReadStlGcodeFileAction()
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
                    var res = await readFileInformation(filename);
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
            get => new RelayCommand(p => OnDropStlFileAction(p));
        }
        private async void OnDropStlFileAction(object p)
        {
            try
            {
                DragEventArgs args = p as DragEventArgs;
                if (p != null)
                {
                    if (args.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        string[] files = (string[])args.Data.GetData(DataFormats.FileDrop);
                        if (files.Count() > 0)
                        {
                            foreach (string file in files)
                            {
                                try
                                {
                                    await readFileInformation(file);
                                    logger.Info(string.Format(Strings.EventFileLoadedForamated, file));
                                }
                                catch(Exception exc)
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
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        private async Task<bool> readFileInformation(string filePath)
        {
            try
            {
                string ext = Path.GetExtension(filePath).ToLower();
                string name = Path.GetFileName(filePath);
                switch (ext)
                {
                    case ".gcode":
                        var prog = new Progress<int>(percent => OnProgressUpdateAction(percent));
                        Progress = 0;
                        IsWorking = true;
                        var gcode = new GCode(filePath);
                        await gcode.ReadGcodeAsync(prog);
                        IsWorking = false;
                        if (!gcode.IsValid)
                        {
                            await this._dialogCoordinator.ShowMessageAsync(this,
                                Strings.DialogGcodeSlicerInvalidHeadline,
                                string.Format(Strings.DialogGcodeSlicerInvalidContentFormated, GlobalStaticConfiguration.supportEmail)
                                );
                            Gcode = null;
                            //OnPropertyChanged(nameof(canSendGcode));
                        }
                        else
                        {
                            Gcodes.Add(gcode);
                            Gcode = gcode;
                            Name = Path.GetFileName(Gcode.FilePath);
                            Create2dGcodeLayerModelAction();
                            SelectedInfoTab = 2;
                        }
                        OnPropertyChanged(nameof(canSendGcode));
                        break;
                    case ".stl":
                        var Stl = createStlModel(filePath);
                        if (Stl != null)
                        {
                            StlFiles.Add(Stl);
                            SelectedInfoTab = 1;
                        }
                        break;
                    case ".3dc":
                        var calc = new _3dPrinterCalculationModel();
                        CalculationFile.Load(filePath, out calc);
                        if (calc != null)
                        {
                            Calculations.Add(calc);
                            SelectedInfoTab = 0;
                        }
                        break;
                    default:
                        await this._dialogCoordinator.ShowMessageAsync(this,
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

        private void OnProgressUpdateAction(int progress)
        {
            Progress = progress;
            if(Progress > 5)
            {

            }
        }
        private void OnProgressLayerModelUpdateAction(int progress)
        {
            ProgressLayerModel = progress;
            if(Progress > 5)
            {

            }
        }

        public ICommand Create2dGcodeLayerModelCommand
        {
            get => new RelayCommand(p => Create2dGcodeLayerModelAction());
        }
        private async void Create2dGcodeLayerModelAction()
        {
            try 
            {
                if (Gcode == null || Gcode.Parser.LayerModelGenerated)
                    return;
                var prog = new Progress<int>(percent => OnProgressLayerModelUpdateAction(percent));
                //await Task.Run(() => Gcode.Get2dGcodeLayerModelListAsync(prog));
                await Gcode.Get2dGcodeLayerModelListAsync(prog);
                GcodeMaxLayer = Gcode.ModelLayers.Count;
                GcodeLayer = 0;
                GcodeLayer = 2;
                Messenger.Default.Send(new NotificationMessage(SelectedGcodeFiles, "ZoomToFitGcode"));
                //GcodeCamera.ZoomExtents();
                //Create3dGcodeLayerModelAction();
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        public ICommand Create3dGcodeLayerModelCommand
        {
            get => new RelayCommand(p => Create3dGcodeLayerModelAction());
        }
        private async void Create3dGcodeLayerModelAction()
        {
            try 
            {
                var prog = new Progress<int>(percent => OnProgressLayerModelUpdateAction(percent));
                //LinesVisual3D obj = new LinesVisual3D();
                var lines = await Gcode.Get3dGcodeLayerModelAsync(prog);

                var query = Gcode.Model3D.Select(layer =>
                {
                    return layer.Points;
                });
                var list = query.Cast<Point3DCollection>().ToList();
                var col = new Point3DCollection();
                foreach(Point3DCollection c in list)
                {
                    foreach (Point3D p in c)
                        col.Add(p);
                }
                Gcode3dPoints = new Point3DCollection(col);
                GcodeModels.Add(Gcode.Parser.Visual3dModel);
                //Gcode3dPoints = new Point3DCollection(list[0]);

            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
        public ICommand SelectedGcodeChangedCommand
        {
            get => new RelayCommand(p => SelectedGcodeChangedAction(p));
        }
        private async void SelectedGcodeChangedAction(object p)
        {
            try {
                GCode gcode = p as GCode;
                if (gcode != null)
                {
                    if (gcode.ModelLayers.Count > 0)
                    {
                        var lines = await Gcode.Get2dGcodeLayerAsync(GcodeLayer);
                        GcodePoints = lines.Points;
                        Messenger.Default.Send(new NotificationMessage(SelectedGcodeFiles, "ZoomToFitGcode"));
                    }

                    //this.Gcode = gcode;
                    //this.ReplacementCosts = printer.Price;
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
        public ICommand SelectedLayerChangedCommand
        {
            get => new RelayCommand(p => SelectedLayerChangedAction());
        }
        private async void SelectedLayerChangedAction()
        {
            try {
                if (Gcode == null)
                    return;
                //GcodePoints.Clear();
                var lines = await Gcode.Get2dGcodeLayerAsync(GcodeLayer);
                GcodePoints = lines.Points;
                Messenger.Default.Send(new NotificationMessage(SelectedGcodeFiles, "ZoomToFitGcode"));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
        public ICommand SelectedCalculationsChangedCommand
        {
            get => new RelayCommand(p => SelectedCalculationsChangedAction(p));
        }
        private void SelectedCalculationsChangedAction(object p)
        {
            CalculationViewInfo calc = p as CalculationViewInfo;
            if (calc != null)
            {
                //this.ReplacementCosts = printer.Price;
            }
        }

        public ICommand SliceSelectedStlFilesCommand
        {
            get { return new RelayCommand(p => SliceSelectedStlFilesAction()); }
        }
        private async void SliceSelectedStlFilesAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.PrepareSlicing };
                var newSlicerDialogViewModel = new SliceStlDialogViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    var files = instance.FilesForImport;
                    if (files.Count > 0)
                    {
                        var res = await _dialogCoordinator.ShowMessageAsync(this,
                             Strings.DialogFilesToImportHeadline,
                             string.Format(Strings.DialogFilesToImportFormatedContent, files.Count)
                             );
                        if (res == MessageDialogResult.Affirmative)
                        {
                            foreach (string file in files)
                            {
                                var result = await readFileInformation(file);
                            }
                        }
                    }
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                _dialogCoordinator,
                StlFiles
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
            get { return new RelayCommand(p => SliceSelectedStlFilesChildWindowAction()); }
        }
        private async void SliceSelectedStlFilesChildWindowAction()
        {
            try
            {
                var _childWindow = new ChildWindow()
                {
                    Title = Strings.PrepareSlicing,
                    AllowMove = true,
                    ShowCloseButton = false,
                    CloseByEscape = false,
                    IsModal = true,
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (Color)Application.Current.Resources["Gray2"] },
                    Padding = new Thickness(50),
                };
                var newSlicerDialogViewModel = new SliceStlDialogViewModel(async instance =>
                {
                    _childWindow.Close();
                    var files = instance.FilesForImport;
                    if (files.Count > 0)
                    {
                       var res = await _dialogCoordinator.ShowMessageAsync(this,
                            Strings.DialogFilesToImportHeadline,
                            string.Format(Strings.DialogFilesToImportFormatedContent, files.Count)
                            );
                        if(res == MessageDialogResult.Affirmative)
                        {
                            foreach (string file in files)
                            {
                                var result = await readFileInformation(file);
                            }
                        }
                    }
                }, instance =>
                {
                    _childWindow.Close();
                },
                    this._dialogCoordinator,
                    StlFiles
                );

                _childWindow.Content = new Views.SlicerViews.SliceStlFilesDialog()
                {
                    DataContext = newSlicerDialogViewModel
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
            get { return new RelayCommand(p => OpenStlFileLocationAction()); }
        }
        private async void OpenStlFileLocationAction()
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
            get { return new RelayCommand(p => DeleteSelectedStlFilesAction()); }
        }
        private async void DeleteSelectedStlFilesAction()
        {
            try
            {
                var res = await _dialogCoordinator.ShowMessageAsync(this,
                    Strings.DialogRemoveSelectedFilesFromListHeadline,
                    Strings.DialogRemoveSelectedFilesFromListContent,
                    MessageDialogStyle.AffirmativeAndNegative
                    );
                if(res == MessageDialogResult.Affirmative)
                {
                    try
                    {
                        IList collection = new ArrayList(SelectedStlFiles);
                        for (int i = 0; i < collection.Count; i++)
                        {
                            var file = collection[i] as Stl;
                            if (file == null)
                                continue;
                            logger.Info(string.Format(Strings.EventDeletedItemFormated, file.FileName));
                            StlFiles.Remove(file);
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
                await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogExceptionHeadline,
                        string.Format(Strings.DialogExceptionFormatedContent, exc.Message)
                        );
            }
        }

        public ICommand OpenGCodeFileLocationCommand
        {
            get { return new RelayCommand(p => OpenGCodeFileLocationAction()); }
        }
        private async void OpenGCodeFileLocationAction()
        {
            try
            {
                if (SelectedGcodeFiles.Count == 0)
                    return;

                else if(SelectedGcodeFiles.Count == 1)
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
                    if(res == MessageDialogResult.Affirmative)
                    {
                        foreach(GCode file in SelectedGcodeFiles)
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
            get { return new RelayCommand(p => OpenGCodeViewerAction()); }
        }
        private async void OpenGCodeViewerAction()
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
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (Color)Application.Current.Resources["Gray2"] },
                    Padding = new Thickness(50),
                };
                var newGcodeViewDialogViewModel = new GcodeViewModel(async instance =>
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
            get { return new RelayCommand(p => OpenGCodeViewerNewWindowAction()); }
        }
        private async void OpenGCodeViewerNewWindowAction()
        {
            try
            {
                Messenger.Default.Send(new NotificationMessage(SelectedGcodeFiles, "ShowGcodeEditor"));
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
            get { return new RelayCommand(p => DeleteSelectedGCodeFilesAction()); }
        }
        private async void DeleteSelectedGCodeFilesAction()
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
                            var file = collection[i] as GCode;
                            if (file == null)
                                continue;
                            logger.Info(string.Format(Strings.EventDeletedItemFormated, file.FileName));
                            Gcodes.Remove(file);
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
                await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogExceptionHeadline,
                        string.Format(Strings.DialogExceptionFormatedContent, exc.Message)
                        );
            }
        }
        #endregion

        #region Helix
        public ICommand ZoomToFitCommand
        {
            get => new RelayCommand(p => ZoomToFitAction(p));
        }
        private void ZoomToFitAction(object p)
        {
            try
            {
                HelixViewport3D v = p as HelixViewport3D;
                if (v != null)
                {
                    v.ZoomExtents();
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
        public ICommand ChangeCameraGridCommand
        {
            get => new RelayCommand(p => ChangeCameraGridAction(p));
        }
        private void ChangeCameraGridAction(object p)
        {
            try
            {
                string arg = p as string;
                if (string.IsNullOrEmpty(arg))
                    return;
                switch(arg)
                {
                    case "left":
                        GcodeCamera.UpDirection = new Vector3D(-0.5, 0, 1);
                        GcodeCamera.LookDirection = new Vector3D(0, 0, 100);
                        GcodeCamera.Position = new Point3D(100, 0, 0);
                        GcodeCamera.LookAt(new Point3D(0, 0, 0), 1);
                        break;
                    case "right":
                        GcodeCamera.UpDirection = new Vector3D(0, 1, 0.5);
                        GcodeCamera.LookDirection = new Vector3D(100, 0, 0);
                        GcodeCamera.Position = new Point3D(0, 0, 100);
                        GcodeCamera.LookAt(new Point3D(0, 0, 0), 1);
                        break;
                    case "plate":
                    default:
                        GcodeCamera.UpDirection = new Vector3D(0, 1, 0.5);
                        GcodeCamera.LookDirection = new Vector3D(0, 0, 0);
                        GcodeCamera.Position = new Point3D(0, 0, 100);
                        GcodeCamera.LookAt(new Point3D(0, 0, 0), 1);
                        break;
                }
                Messenger.Default.Send(new NotificationMessage(SelectedGcodeFiles, "ResetCameraGcode"));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        #endregion
        public ICommand DeleteSelectedGcodeFileCommand
        {
            get => new RelayCommand(p => DeleteSelectedGcodeFileAction(p));
        }
        private void DeleteSelectedGcodeFileAction(object p)
        {
            try
            {
                GCode gcode = p as GCode;
                if (gcode != null)
                {
                    Gcodes.Remove(gcode);
                    GcodePoints = new Point3DCollection();
                    ProgressLayerModel = 0;
                    logger.Info(string.Format(Strings.EventDeletedItemFormated, gcode.FileName));
                    //this.ReplacementCosts = printer.Price;
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        
        }

        public ICommand SelectedMaterialChangedCommand
        {
            get => new RelayCommand(p => SelectedMaterialChangedAction(p));
        }
        private void SelectedMaterialChangedAction(object p)
        {
            try
            {
                _3dPrinterMaterial material = p as _3dPrinterMaterial;
                if (material != null)
                {
                    //this.ReplacementCosts = printer.Price;
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand AddNewPrinterCommand
        {
            get { return new RelayCommand(p => AddNewPrinterAction()); }
        }
        private async void AddNewPrinterAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.NewPrinter };
                var newPrinterViewModel = new New3DPrinterViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Printers.Add(new _3dPrinterModel()
                    {
                        Price = instance.Price,
                        Type = instance.Type,
                        Supplier = instance.Supplier,
                        Manufacturer = instance.Manufacturer,
                        hasHeatbed = instance.hasHeatbed,
                        MaxHeatbedTemperature = instance.TemperatureHeatbed,
                        Kind = instance.Kind,
                        Model = instance.Model,
                        MachineHourRate = instance.MachineHourRate

                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Printers[Printers.Count - 1].Name));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                }
                , _dialogCoordinator
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

        public ICommand AddNewMaterialCommand
        {
            get { return new RelayCommand(p => AddNewMaterialAction()); }
        }
        private async void AddNewMaterialAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Resources.Localization.Strings.NewManufacturer };
                var newMaterialViewModel = new NewMaterialViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Materials.Add(new _3dPrinterMaterial()
                    {
                        Id = instance.Id,
                        Name = instance.Name,
                        SKU = instance.SKU,
                        UnitPrice = instance.Price,
                        Unit = instance.Unit,
                        ColorCode = instance.ColorCode,
                        LinkToReorder = instance.LinkToReorder,
                        PackageSize = instance.PackageSize,
                        Supplier = instance.Supplier,
                        Manufacturer = instance.Manufacturer,
                        TemperatureHeatbed = instance.TemperatureHeatbed,
                        TemperatureNozzle = instance.TemperatureNozzle,
                        Density = instance.Density,
                        TypeOfMaterial = instance.TypeOfMaterial,

                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Materials[Materials.Count -1].Name));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                }
                , _dialogCoordinator
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
            }
        }

        // Actions from template
        #region Template Commands
        public ICommand EditPrinterCommand
        {
            get => new RelayCommand(p => EditPrinterAction(p));
        }
        private async void EditPrinterAction(object printer)
        {
            try
            {
                var selectedPrinter = printer as _3dPrinterModel;
                if (selectedPrinter == null)
                {
                    return;
                }
                var _dialog = new CustomDialog() { Title = Strings.EditPrinter };
                var newPrinterViewModel = new New3DPrinterViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Printers.Remove(selectedPrinter);
                    Printers.Add(new _3dPrinterModel()
                    {
                        Id = instance.Id,
                        Price = instance.Price,
                        Type = instance.Type,
                        Supplier = instance.Supplier,
                        Manufacturer = instance.Manufacturer,
                        hasHeatbed = instance.hasHeatbed,
                        MaxHeatbedTemperature = instance.TemperatureHeatbed,
                        Kind = instance.Kind,
                        Model = instance.Model,
                        MachineHourRate = instance.MachineHourRate,
                        MachineHourRateCalc = instance.MachineHourRateCalc,
                        MaxNozzleTemperature = instance.TemperatureNozzle,
                        BuildVolume = instance.BuildVolume,
                        PowerConsumption = instance.PowerConsumption,
                        ShopUri = instance.LinkToReorder,

                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, selectedPrinter, Printers[Printers.Count - 1].Name));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
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
                await _dialogCoordinator.ShowMessageAsync(this,
                    Strings.DialogExceptionHeadline,
                    string.Format(Strings.DialogExceptionFormatedContent, exc.Message)
                    );
            }
        }

        public ICommand DeletePrinterCommand
        {
            get => new RelayCommand(p => DeletePrinterAction(p));
        }
        private async void DeletePrinterAction(object p)
        {
            try
            {
                _3dPrinterModel printer = p as _3dPrinterModel;
                if (printer != null)
                {
                    var res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogDeletePrinterHeadline, Strings.DialogDeletePrinterContent,
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        Printers.Remove(printer);
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, printer.Name));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand EditMaterialCommand
        {
            get => new RelayCommand(p => EditMaterialAction(p));
        }
        private async void EditMaterialAction(object material)
        {
            try
            {
                var selectedMaterial = material as _3dPrinterMaterial;
                if (selectedMaterial == null)
                {
                    return;
                }
                var _dialog = new CustomDialog() { Title = Strings.EditMaterial };
                var newMaterialViewModel = new NewMaterialViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Materials.Remove(selectedMaterial);
                    Materials.Add(new _3dPrinterMaterial()
                    {
                        Id = instance.Id,
                        Name = instance.Name,
                        SKU = instance.SKU,
                        UnitPrice = instance.Price,
                        Unit = instance.Unit,
                        ColorCode = instance.ColorCode,
                        LinkToReorder = instance.LinkToReorder,
                        PackageSize = instance.PackageSize,
                        Supplier = instance.Supplier,
                        Manufacturer = instance.Manufacturer,
                        TemperatureHeatbed = instance.TemperatureHeatbed,
                        TemperatureNozzle = instance.TemperatureNozzle,
                        Density = instance.Density,
                        TypeOfMaterial = instance.TypeOfMaterial,
                    });
                    logger.Info(string.Format(Strings.EventEditedItemFormated, selectedMaterial, Materials[Materials.Count - 1]));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                    this._dialogCoordinator,
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
            get => new RelayCommand(p => DeleteMaterialAction(p));
        }
        private async void DeleteMaterialAction(object p)
        {
            try
            {
                _3dPrinterMaterial material = p as _3dPrinterMaterial;
                if (material != null)
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
        
        public ICommand DeleteCalculationFromTemplateCommand
        {
            get => new RelayCommand(p => DeleteCalculationFromTemplateAction(p));
        }
        private async void DeleteCalculationFromTemplateAction(object p)
        {
            try
            {
                _3dPrinterCalculationModel calc = p as _3dPrinterCalculationModel;
                if (calc != null)
                {
                    var res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogDeleteCalculationHeadline,
                        Strings.DialogDeleteCalculationContent,
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
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

        public ICommand ReorderMaterialCommand
        {
            get => new RelayCommand(p => ReorderMaterialAction(p));
        }
        private async void ReorderMaterialAction(object parameter)
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
        // END - Actions from template
        #endregion

        #region MyRegion

        #endregion
        #endregion

        #region Methods

        private Stl createStlModel(string StlFilePath)
        {
            try
            {
                Stl stl = new Stl(StlFilePath);
                Model = stl.Model;
                if (Model != null)
                {
                    clearStlViewer();
                    Models.Add(new ModelVisual3D() { Content = Model });
                    int countVertices = 0;
                    Visual3DHelper.Traverse<GeometryModel3D>(
                        Models[Models.Count - 1],
                        (geometryModel, transform) =>
                        {
                            var mesh = (MeshGeometry3D)geometryModel.Geometry;
                            countVertices += mesh.Positions.Count;
                        });
                }
                return stl;
            }
            catch(Exception exc)
            {
                logger.ErrorFormat(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return new Stl();
            }
        }

        public void OnViewVisible()
        {

        }

        public void OnViewHide()
        {

        }
        private static Model3DGroup Load(string path)
        {
            if (path == null)
            {
                return null;
            }

            Model3DGroup model = null;
            string ext = Path.GetExtension(path).ToLower();
            switch (ext)
            {
                case ".3ds":
                    {
                        var r = new StudioReader();
                        model = r.Read(path);
                        break;
                    }

                case ".lwo":
                    {
                        var r = new LwoReader();
                        model = r.Read(path);

                        break;
                    }

                case ".obj":
                    {
                        var r = new ObjReader();
                        model = r.Read(path);
                        break;
                    }

                case ".objz":
                    {
                        var r = new ObjReader();
                        model = r.ReadZ(path);
                        break;
                    }

                case ".stl":
                    {
                        var r = new StLReader();
                        model = r.Read(path);
                        break;
                    }

                case ".off":
                    {
                        var r = new OffReader();
                        model = r.Read(path);
                        break;
                    }

                default:
                    throw new InvalidOperationException("File format not supported.");
            }

            return model;
        }
        private static void Rotate(Model3DGroup model)
        {
            var axis = new Vector3D(1, 0, 0);
            var angle = 90;

            var matrix = model.Transform.Value;
            matrix.Rotate(new Quaternion(axis, angle));

            model.Transform = new MatrixTransform3D(matrix);
        }

        private void clearStlViewer()
        {
            Models = new ObservableCollection<ModelVisual3D>();
            var lights = new ModelVisual3D();
            lights.Children.Add(new SunLight());
            Models.Add(lights);
        }
        #endregion
    }

}
