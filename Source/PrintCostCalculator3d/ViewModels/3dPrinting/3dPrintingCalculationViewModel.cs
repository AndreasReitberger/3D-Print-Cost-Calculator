using PrintCostCalculator3d.Models._3dprinting;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Utilities;
//using HelixToolkit.Wpf;
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
using PrintCostCalculator3d.Models;
using System.Collections;
using PrintCostCalculator3d.Resources.Localization;
using log4net;
using PrintCostCalculator3d.ViewModels.Slicer;
using MahApps.Metro.SimpleChildWindow;
using System.Windows.Media;
using System.Text.RegularExpressions;
using PrintCostCalculator3d.Models.GCode;
using GalaSoft.MvvmLight.Messaging;
using PrintCostCalculator3d.Models.Syncfusion;
using PrintCostCalculator3d.Models.Exporter;
using System.Text;
using PrintCostCalculator3d.Models.Documentation;
using System.Threading;
using System.Xml.Linq;
using System.Windows.Threading;
using PrintCostCalculator3d.Models.Slicer;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Assimp;
using AndreasReitberger;
using AndreasReitberger.Models;
using AndreasReitberger.Models.CalculationAdditions;
using AndreasReitberger.Enums;
using AndreasReitberger.Models.FileAdditions;

namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    public class _3dPrintingCalculationViewModel : ViewModelBase
    {

        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;        
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool _isLoading;
        private CancellationTokenSource cts;

        public bool isLicenseValid
        {
            get => false;
        }
        #endregion

        #region Module
        private bool _showSelectedFiles = false;
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

        private bool _isShowingCalculationResult = false;
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
        #endregion

        #region Properties

        #region Defaults
        private ObservableCollection<Printer3d> _defaultPrinters = new ObservableCollection<Printer3d>();
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

        private ObservableCollection<Material3d> _defaultMaterials = new ObservableCollection<Material3d>();
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

        private ObservableCollection<Workstep> _defaultWorksteps = new ObservableCollection<Workstep>();
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
        private Calculation3d _calculation;
        public Calculation3d Calculation
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
            get => Gcode != null;
        }

        private ObservableCollection<GCode> _gcodesQueue = new ObservableCollection<GCode>();
        public ObservableCollection<GCode> GcodesQueue
        {
            get => _gcodesQueue;
            set
            {
                if (_gcodesQueue == value) return;
                _gcodesQueue = value;
                OnPropertyChanged();
                
            }
        }

        private bool _reloadCalculationsOnStartup = true;
        public bool ReloadCalculationsOnStartup
        {
            get => _reloadCalculationsOnStartup;
            set
            {
                if (_reloadCalculationsOnStartup == value)
                    return;
                if (!_isLoading)
                    SettingsManager.Current.Calculation_ReloadCalculationsOnStartup = value;
                _reloadCalculationsOnStartup = value;
                OnPropertyChanged();
            }
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

        private int _selectedMainTab = 0;
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

        private bool _isSendingGcode = false;
        public bool IsSendingGcode
        {
            get => _isSendingGcode;
            private set
            {
                if (_isSendingGcode == value)
                    return;
                _isSendingGcode = value;
                OnPropertyChanged();
            }
        }

        private bool _allCalcsSelected = false;
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

        #region Expander
        private bool _expandProfileView;
        public bool ExpandProfileView
        {
            get => _expandProfileView;
            set
            {
                if (value == _expandProfileView)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.AdvancedViewer_ExpandView = value;

                _expandProfileView = value;

                
                if (_canProfileWidthChange)
                    ResizeProfile(false);
                
                OnPropertyChanged();
            }
        }

        private bool _canProfileWidthChange = true;
        private double _tempProfileWidth;

        private GridLength _profileWidth;
        public GridLength ProfileWidth
        {
            get => _profileWidth;
            set
            {
                if (value == _profileWidth)
                    return;

                if (!_isLoading && Math.Abs(value.Value - GlobalStaticConfiguration.CalculationView_WidthCollapsed) > GlobalStaticConfiguration.FloatPointFix) 
                    // Do not save the size when collapsed
                    SettingsManager.Current.AdvancedViewer_ProfileWidth = value.Value;

                _profileWidth = value;
                
                if (_canProfileWidthChange)
                    ResizeProfile(true);
                    
                OnPropertyChanged();
            }
        }

        private bool _expandGcodeMultiParserView;
        public bool ExpandGcodeMultiParserView
        {
            get => _expandGcodeMultiParserView;
            set
            {
                if (value == _expandGcodeMultiParserView)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.GcodeMultiParse_ExpandView = value;

                _expandGcodeMultiParserView = value;

                
                if (_canGcodeMultiParseWidthChange)
                    ResizeGcodeMultiParse(false);
                
                OnPropertyChanged();
            }
        }

        private bool _canGcodeMultiParseWidthChange = true;
        private double _tempGcodeMultiParseWidth;

        private GridLength _gcodeMultiParserWidth;
        public GridLength GcodeMultiParserWidth
        {
            get => _gcodeMultiParserWidth;
            set
            {
                if (value == _gcodeMultiParserWidth)
                    return;

                if (!_isLoading && Math.Abs(value.Value - GlobalStaticConfiguration.GcodeMultiParser_WidthCollapsed) > GlobalStaticConfiguration.FloatPointFix) 
                    // Do not save the size when collapsed
                    SettingsManager.Current.GcodeMultiParse_ProfileWidth = value.Value;

                _gcodeMultiParserWidth = value;
                
                if (_canGcodeMultiParseWidthChange)
                    ResizeGcodeMultiParse(true);
                    
                OnPropertyChanged();
            }
        }

        #endregion

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
            set
            {
                if (_MaterialViews == value) return;
                _MaterialViews = value;
                OnPropertyChanged();
                
            }
        }
        private ICollectionView _MaterialViews;

        private MaterialViewInfo _selectedMaterialView;
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

        private IList _selectedMaterialsView = new ArrayList();
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
        private ICollectionView _WorkstepViews;

        private WorkstepViewInfo _selectedWorkstepView;
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

        private IList _selectedWorkstepsView = new ArrayList();
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
        private ObservableCollection<Calculation3d> _calculations = new ObservableCollection<Calculation3d>();
        public ObservableCollection<Calculation3d> Calculations
        {
            get => _calculations;
            set
            {
                if (_calculations == value) return;
                _calculations = value;
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

        private ObservableCollection<Material3d> _materials = new ObservableCollection<Material3d>();
        public ObservableCollection<Material3d> Materials
        {
            get => _materials;
            set
            {
                if (_materials == value) return;
                if(!_isLoading)
                    SettingsManager.Current.PrinterMaterials = value;
                _materials = value;
                OnPropertyChanged();
                
            }

        }

        private ObservableCollection<Printer3d> _printers = new ObservableCollection<Printer3d>();
        public ObservableCollection<Printer3d> Printers
        {
            get => _printers;
            set
            {
                if (_printers == value) return;
                if(!_isLoading)
                    SettingsManager.Current.Printers = value;
                _printers = value;
                OnPropertyChanged();
            }

        }

        private ObservableCollection<Workstep> _worksteps = new ObservableCollection<Workstep>();
        public ObservableCollection<Workstep> Worksteps
        {
            get => _worksteps;
            set
            {
                if (_worksteps == value) return;
                if (!_isLoading)
                    SettingsManager.Current.Worksteps = value;
                _worksteps = value;
                OnPropertyChanged();
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

        private Material3d _material;
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
        
        private double _margin = 30;
        public double Margin
        {
            get => _margin;
            set
            {
                if (_margin == value)
                    return;
                if (!_isLoading)
                    SettingsManager.Current.Calculation_Margin = value;
                _margin = value;
                OnPropertyChanged();
                
            }
        }
        
        private double _failRate = 5;
        public double FailRate
        {
            get => _failRate;
            set
            {
                if (_failRate == value)
                    return;
                if (!_isLoading)
                    SettingsManager.Current.Calculation_FailRate = value;
                _failRate = value;
                OnPropertyChanged();

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

        private double _duration = 0;
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
        
        private double _consumedMaterial = 0;
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
        
        private double _volume = 0;
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

        #region 3dModel
        //public Transform3D ModelTransform { get; private set; }
        private HelixToolkitScene scene;
        public SceneNodeGroupModel3D GroupModel { get; } = new SceneNodeGroupModel3D();
        /*
         * Media3D
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
        */
        #region GcodeViewer
        private EffectsManager _effectManager = new DefaultEffectsManager();
        public EffectsManager EffectsManager
        {
            get => _effectManager;
            set
            {
                if (_effectManager == value)
                    return;
                _effectManager = value;
                OnPropertyChanged();
            }
        }
        
        private HelixToolkit.Wpf.SharpDX.PerspectiveCamera _gcodeCamera = new HelixToolkit.Wpf.SharpDX.PerspectiveCamera()
        {
            LookDirection = new Vector3D(0, 0, -10),
            Position = new Point3D(10, 10, 10),
            UpDirection = new Vector3D(0, 1, 0),
            FarPlaneDistance = 5000,
            NearPlaneDistance = 0.1f
        };
        public HelixToolkit.Wpf.SharpDX.PerspectiveCamera GcodeCamera
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


        private HelixToolkit.Wpf.SharpDX.OrthographicCamera _gcode3dCamera = new HelixToolkit.Wpf.SharpDX.OrthographicCamera()
        {
            LookDirection = new Vector3D(0, 0, -10),
            Position = new Point3D(10, 10, 10),
            UpDirection = new Vector3D(0, 1, 0),
            FarPlaneDistance = 5000,
            NearPlaneDistance = 0.1f
        };
        public HelixToolkit.Wpf.SharpDX.OrthographicCamera Gcode3dCamera
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

        private HelixToolkit.Wpf.SharpDX.PerspectiveCamera _stl3dCamera = new HelixToolkit.Wpf.SharpDX.PerspectiveCamera()
        {
            LookDirection = new Vector3D(0, -10, -10),
            Position = new Point3D(0, 10, 10),
            UpDirection = new Vector3D(0, 1, 0),
            FarPlaneDistance = 5000,
            NearPlaneDistance = 0.1f
        };
        public HelixToolkit.Wpf.SharpDX.PerspectiveCamera Stl3dCamera
        {
            get => _stl3dCamera;
            set
            {
                if (_stl3dCamera == value)
                    return;
                _stl3dCamera = value;
                //Messenger.Default.Send(new NotificationMessage(SelectedGcodeFiles, "ResetCameraGcode3d")); 
                OnPropertyChanged();
            }
        }

        /*
         * Media3D
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
        */

        private LineGeometryModel3D _gcodeLayerGeometry;
        public LineGeometryModel3D GcodeLayerGeometry
        {
            get => _gcodeLayerGeometry;
            set
            {
                if (_gcodeLayerGeometry == value) return;
                _gcodeLayerGeometry = value;
                OnPropertyChanged();
            }

        }
        private ObservableElement3DCollection _gcode3dLayerGeometry;
        public ObservableElement3DCollection Gcode3dLayerGeometry
        {
            get => _gcode3dLayerGeometry;
            set
            {
                if (_gcode3dLayerGeometry == value) return;
                _gcode3dLayerGeometry = value;
                OnPropertyChanged();
            }
        }

        private MeshGeometryModel3D _model;
        public MeshGeometryModel3D Model
        {
            get => _model;
            set
            {
                if (_model == value) return;
                _model = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MeshGeometryModel3D> _models = new ObservableCollection<MeshGeometryModel3D>();
        public ObservableCollection<MeshGeometryModel3D> Models
        {
            get => _models;
            set
            {
                if (_models == value) return;
                _models = value;
                OnPropertyChanged();
            }
        }

        /* Helix.WPF
        private ObservableCollection<LinesVisual3D> _Gcode3dModel;
        public ObservableCollection<LinesVisual3D> Gcode3dModel
        {
            get => _Gcode3dModel;
            set
            {
                if(_Gcode3dModel != value)
                {
                    _Gcode3dModel = value;
                    OnPropertyChanged();
                }
            }
        }
        */
        private System.Windows.Media.Color _GcodeColor = Colors.Orange;
        public System.Windows.Media.Color GcodeColor
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
        #endregion

        #region Stl
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
                TotalFileCount = SelectedGcodeFiles.Count + SelectedStlFiles.Count;
                if (Equals(value, _selectedStlFiles))
                    return;

                _selectedStlFiles = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion

        #region Gcode
        private GCode _gcode;
        public GCode Gcode
        {
            get => _gcode;
            set
            {
                if (_gcode == value) return;
                
                _gcode = value;
                OnPropertyChanged();
                /*
                if(_gcode != null)
                { 
                    Volume = _gcode.ExtrudedFilamentVolume;
                    Duration = _gcode.PrintTime;
                    Name = _gcode.FileName;
                }
                else
                {
                    Volume = 0;
                    Duration = 0;
                    Name = string.Empty;
                }
                */


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

        private File3d _file;
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

        private ObservableCollection<File3d> _files = new ObservableCollection<File3d>();
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

        private IList _selectedGcodeFiles = new ArrayList();
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
        #endregion

        #region GcodeParser
        private SlicerPrinterConfiguration _printerConfig;
        public SlicerPrinterConfiguration PrinterConfig
        {
            get => _printerConfig;
            set
            {
                if (value == _printerConfig)
                    return;
                if (!_isLoading)
                    SettingsManager.Current.GcodeParser_PrinterConfig = value;
                _printerConfig = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<SlicerPrinterConfiguration> _printerConfigs;
        public ObservableCollection<SlicerPrinterConfiguration> PrinterConfigs
        {
            get => _printerConfigs;
            set
            {
                if (value == _printerConfigs)
                    return;
                if (!_isLoading)
                    SettingsManager.Current.GcodeParser_PrinterConfigs = value;
                _printerConfigs = value;
                OnPropertyChanged();
            }
        }

        private float _aMax_xy = 1000;
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

        private float _aMax_z = 1000;
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

        private float _aMax_e = 5000;
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

        private float _aMax_eExtrude = 1250;
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

        private float _aMax_eRetract = 1250;
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

        private float _printDurationCorrection = 1;
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

        #region TotalFiles
        private int _totalFilesCount = 0;
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
        /*
         * Media3D
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
        */
        #endregion

        #region Settings 
        private bool _showCalculationResult = true;
        public bool ShowCalculationResultPopup
        {
            get => _showCalculationResult;
            set
            {
                if (_showCalculationResult == value) return;

                if (!_isLoading)
                    SettingsManager.Current.General_OpenCalculationResultView = value;
                _showCalculationResult = value;
                 OnPropertyChanged();
                
            }
        }
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

        private bool _applyCustomAdditions = false;
        public bool ApplyCustomAdditions
        {
            get => _applyCustomAdditions;
            set
            {
                if (_applyCustomAdditions == value) return;
                _applyCustomAdditions = value;
                if (!_isLoading)
                    SettingsManager.Current.Calculation_ApplyCustomAdditions = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CustomAddition> _customAdditions = new ObservableCollection<CustomAddition>();
        public ObservableCollection<CustomAddition> CustomAdditions
        {
            get => _customAdditions;
            set
            {
                if (_customAdditions == value) return;
                if (!_isLoading)
                    SettingsManager.Current.Calculation_CustomAdditions = value;
                _customAdditions = value;
                OnPropertyChanged();  
            }
        }

        private CustomAddition _customAddition;
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

        private IList _selectedCustomAdditions = new ArrayList();
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
        public double EnergyCosts
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

        public double Taxrate
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

        public double HandlingFee
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

        #region EnumCollections

        #region CalculationTypes
        private ObservableCollection<CustomAdditionCalculationType> _calculationTypes = new ObservableCollection<CustomAdditionCalculationType>(
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

        #endregion

        #region Constructor, LoadSettings
        public _3dPrintingCalculationViewModel(IDialogCoordinator dialog)
        {
            this._dialogCoordinator = dialog;
            _isLoading = true;

            LoadSettings();

            _isLoading = false;

            Printers.CollectionChanged += Printers_CollectionChanged;
            Materials.CollectionChanged += Materials_CollectionChanged;
            Worksteps.CollectionChanged += Worksteps_CollectionChanged;
            Calculations.CollectionChanged += Calculations_CollectionChanged;
            Gcodes.CollectionChanged += Gcodes_CollectionChanged;
            StlFiles.CollectionChanged += StlFiles_CollectionChanged;

            createPrinterViewInfos();
            createMaterialViewInfos();
            createWorkstepViewInfos();

            if (ReloadCalculationsOnStartup)
            {
                try
                {
                    string filePath = Path.Combine(SettingsManager.GetSettingsLocation(), "calculations.xml");
                    if (File.Exists(filePath))
                    {
                        var calculationsLib = Calculator3dExporter.DecryptAndDeserializeArray(filePath);
                        foreach (var calc in calculationsLib)
                            Calculations.Add(calc);
                        
                    }

                }
                catch (Exception exc)
                {
                    logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                }
            }
            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }
        private void LoadDefaults()
        {
            DefaultMaterials = SettingsManager.Current.Calculation_DefaultMaterialsLib;
            DefaultPrinters = SettingsManager.Current.Calculation_DefaultPrintersLib;
            DefaultWorksteps = SettingsManager.Current.Calculation_DefaultWorkstepsLib;

            PrinterConfigs = SettingsManager.Current.GcodeParser_PrinterConfigs;
            PrinterConfig = SettingsManager.Current.GcodeParser_PrinterConfig;
        }
        private void LoadSettings()
        {
            Printers = SettingsManager.Current.Printers;
            Materials = SettingsManager.Current.PrinterMaterials;
            Worksteps = SettingsManager.Current.Worksteps;

            ReloadCalculationsOnStartup = SettingsManager.Current.Calculation_ReloadCalculationsOnStartup;
            SelectedInfoTab = SettingsManager.Current.Calculation_SelectedInfoTab;
            ShowGcodeInfos = SettingsManager.Current.Calculation_ShowGcodeInfos;
            ShowGcodeGrid = SettingsManager.Current.Calculation_ShowGcodeGrid;
            ShowCalculationResultPopup = SettingsManager.Current.General_OpenCalculationResultView;

            //ProfileWidth = new GridLength(SettingsManager.Current.AdvancedViewer_ProfileWidth);
            ExpandProfileView = SettingsManager.Current.AdvancedViewer_ExpandView;

            ProfileWidth = ExpandProfileView ? new GridLength(SettingsManager.Current.AdvancedViewer_ProfileWidth) : new GridLength(GlobalStaticConfiguration.GcodeInfo_WidthCollapsed);
            _tempProfileWidth = SettingsManager.Current.AdvancedViewer_ProfileWidth;

            ExpandGcodeMultiParserView = SettingsManager.Current.GcodeMultiParse_ExpandView;

            GcodeMultiParserWidth = ExpandGcodeMultiParserView ? new GridLength(SettingsManager.Current.GcodeMultiParse_ProfileWidth) : new GridLength(GlobalStaticConfiguration.GcodeMultiParser_WidthCollapsed);
            _tempGcodeMultiParseWidth = SettingsManager.Current.GcodeMultiParse_ProfileWidth;

            Margin = SettingsManager.Current.Calculation_Margin;
            FailRate = SettingsManager.Current.Calculation_FailRate;

            ApplyCustomAdditions = SettingsManager.Current.Calculation_ApplyCustomAdditions;
            CustomAdditions = SettingsManager.Current.Calculation_CustomAdditions;
            CustomAdditions.CollectionChanged += CustomAdditions_CollectionChanged;
        }

        private void CustomAdditions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CustomAdditions));
            SettingsManager.Current.Calculation_CustomAdditions = CustomAdditions;
            SettingsManager.Save();
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
            try
            {
                string filePath = Path.Combine(SettingsManager.GetSettingsLocation(), "calculations.xml");
                if(Calculations.Count > 0)
                Calculator3dExporter.EncryptAndSerialize(filePath, Calculations.ToArray());
                else
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
         }

        #endregion

        #region Events
        private void Worksteps_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            createWorkstepViewInfos();
            SettingsManager.Save();
        }

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
            var selection = new ObservableCollection<Printer3d>(SelectedPrintersView.OfType<PrinterViewInfo>().ToList().Select(p => p.Printer).ToList());
            Canvas c = new Canvas();
            c.Children.Add(new PackIconMaterial { Kind = PackIconMaterialKind.Printer3d });
            PrinterViews = new CollectionViewSource
            {
                Source = (Printers.Select(p => new PrinterViewInfo()
                {
                    Name = p.Name,
                    Printer = p,
                    Icon = c,
                    Group = (Printer3dType)Enum.Parse(typeof(Printer3dType), p.Type.ToString()),
                })).ToList()
            }.View;
            PrinterViews.SortDescriptions.Add(new SortDescription(nameof(PrinterViewInfo.Group), ListSortDirection.Ascending));
            PrinterViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(PrinterViewInfo.Group)));
            // Restore selection
            SelectedPrintersView = new ArrayList(PrinterViews.OfType<PrinterViewInfo>()
                .Where(printerview => selection.Contains(printerview.Printer))
                .ToList());
        }
        private void createCalculationViewInfos()
        {
            var selection = new ObservableCollection<Calculation3d>(SelectedCalculationsView.OfType<CalculationViewInfo>().ToList().Select(calc => calc.Calculation).ToList());
            Canvas c = new Canvas();
            c.Children.Add(new PackIconMaterial { Kind = PackIconMaterialKind.Calculator });
            CalculationViews = new CollectionViewSource
            {
                Source = (Calculations.Select(calc => new CalculationViewInfo()
                {
                    Name = calc.Files.Count > 0 ? calc.Files[0].Name : "",
                    Calculation = calc,
                    Icon = c,
                    Group = CalculationViewManager.Group.MISC,
                })).ToList()
            }.View;
            CalculationViews.SortDescriptions.Add(new SortDescription(nameof(CalculationViewInfo.Group), ListSortDirection.Ascending));
            CalculationViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(CalculationViewInfo.Group)));
            // Restore selection
            SelectedCalculationsView = new ArrayList(CalculationViews.OfType<CalculationViewInfo>()
                .Where(view => selection.Contains(view.Calculation))
                .ToList());
        }
        private void createMaterialViewInfos()
        {
            var selection = new ObservableCollection<Material3d>(SelectedMaterialsView.OfType<MaterialViewInfo>().ToList().Select(mat => mat.Material).ToList());
            Canvas c = new Canvas();
            c.Children.Add(new PackIconModern { Kind = PackIconModernKind.Box });
            MaterialViews = new CollectionViewSource
            {
                Source = (Materials.Select(p => new MaterialViewInfo()
                {
                    Name = p.Name,
                    Material = p,
                    Icon = c,
                    Group = (Material3dTypes)Enum.Parse(typeof(Material3dTypes), p.MaterialFamily.ToString()),
                })).ToList()
            }.View;
            MaterialViews.SortDescriptions.Add(new SortDescription(nameof(MaterialViewInfo.Group), ListSortDirection.Ascending));
            MaterialViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(MaterialViewInfo.Group)));
            // Restore selection
            SelectedMaterialsView = new ArrayList(MaterialViews.OfType<MaterialViewInfo>()
                .Where(view => selection.Contains(view.Material))
                .ToList());
        }
        private void createWorkstepViewInfos()
        {
            var selection = new ObservableCollection<Workstep>(SelectedWorkstepsView.OfType<WorkstepViewInfo>().ToList().Select(ws => ws.Workstep).ToList());
            Canvas c = new Canvas();
            c.Children.Add(new PackIconModern { Kind = PackIconModernKind.Clock });
            WorkstepViews = new CollectionViewSource
            {
                Source = (Worksteps.Select(p => new WorkstepViewInfo()
                {
                    Name = p.Name,
                    Workstep = p,
                    Icon = c,
                    Group = (AndreasReitberger.Enums.WorkstepType)Enum.Parse(typeof(AndreasReitberger.Enums.WorkstepType), p.Type.ToString()),
                })).ToList()
            }.View;
            WorkstepViews.SortDescriptions.Add(new SortDescription(nameof(WorkstepViewInfo.Group), ListSortDirection.Ascending));
            WorkstepViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(WorkstepViewInfo.Group)));
            // Restore selection
            SelectedWorkstepsView = new ArrayList(WorkstepViews.OfType<WorkstepViewInfo>()
                .Where(view => selection.Contains(view.Workstep))
                .ToList());
        }
        #endregion

        #region ICommand & Actions
        public ICommand OpenDocumentationCommand
        {
            get { return new RelayCommand(async(p) => await OpenDocumentationAction(p)); }
        }

        private async Task OpenDocumentationAction(object parameter)
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
            get => new RelayCommand(async(p) => await GoProAction());
        }
        private async Task GoProAction()
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
            get =>  new RelayCommand(async(p) => await ClearFormAction()); 
                
        }
        private async Task ClearFormAction()
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
                //SelectedWorkstepsView = new ArrayList();
                Volume = 0;
                Duration = 0;
                Pieces = 1;
                Gcode = null;
                Calculation = null;
                //Model = null;  //Media3D
                OnPropertyChanged(nameof(canBeSaved));
                logger.Info(Strings.EventFormCleared);
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
        

        #region Calculation
        public ICommand SendGCodeFilesCommand
        {
            get { return new RelayCommand(async(p) => await SendGCodeFilesAction(p)); }
        }
        private async Task SendGCodeFilesAction(object parameter)
        {
            try
            {
                List<GCode> failedGcodes = new List<GCode>();

                string printServer = parameter as string;
                if (string.IsNullOrEmpty(printServer)) return;

                switch (printServer)
                {
                    case "OctoPrint":                        
                        break;
                    case "RepetierServerPro":
                        break;
                    default:
                        logger.Error(string.Format(Strings.EventUnknownPrintServerFormated, printServer));
                        break;
                }

                if (failedGcodes.Count == 0)
                {
                    await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogGcodeFileSentSuccessfullyHeadline,
                        string.Format(Strings.DialogGcodeFileSentSuccessfullyFormatedContent, Gcode.FilePath)
                        , MessageDialogStyle.Affirmative);
                    logger.Info(string.Format(Strings.EventGcodeSentToRepetierServerPro, Gcode.FilePath, ""));
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    for(int i = 0; i < failedGcodes.Count; i++)
                    {
                        sb.Append(failedGcodes[i].FileName);
                        if (i < failedGcodes.Count - 1)
                            sb.Append("; ");
                    }
                    await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogGcodeFileSentFailedHeadline,
                        string.Format(Strings.DialogGcodeFileSentFailedFormatedContent, sb.ToString())
                        , MessageDialogStyle.Affirmative);
                }

            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
        public ICommand CalculateCommand
        {
            get { return new RelayCommand(async(p) => await CalculateAction()); }
        }
        private async Task CalculateAction()
        {
            try
            {
                if (Calculation == null)
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

                Calculation.Printers = SelectedPrintersView != null ?
                                new ObservableCollection<Printer3d>(SelectedPrintersView.OfType<PrinterViewInfo>().ToList().Select(p => p.Printer).ToList()) :
                                new ObservableCollection<Printer3d>();
                Calculation.WorkSteps = SelectedWorkstepsView != null ?
                                new ObservableCollection<Workstep>(SelectedWorkstepsView.OfType<WorkstepViewInfo>().ToList().Select(p => p.Workstep).ToList()) :
                                new ObservableCollection<Workstep>();
                Calculation.Materials = SelectedMaterialsView != null ?
                                new ObservableCollection<Material3d>(SelectedMaterialsView.OfType<MaterialViewInfo>().ToList().Select(p => p.Material).ToList()) :
                                new ObservableCollection<Material3d>();
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
                /*
                Calculation.Files = new ObservableCollection<File3d>()
                {
                    new File3d() {
                        Name = this.Gcode != null ? Gcode.FileName : this.Name,
                        //File = this.Gcode,
                        File = this.Gcode != null ? Gcode.FileName : this.Name,
                        FileName = this.Gcode != null ? Gcode.FileName : string.Empty,
                        FilePath = this.Gcode != null ? Gcode.FilePath : string.Empty,
                        PrintTime = this.Duration,
                        Volume = this.Volume,
                        Quantity = this.Pieces,
                    },
                };
                */
                Calculation.Calculate();
                OnPropertyChanged(nameof(Calculation));
                OnPropertyChanged(nameof(TotalPrice));
                OnPropertyChanged(nameof(TotalPrintTime));
                OnPropertyChanged(nameof(canBeSaved));

                if(!Calculations.Contains(Calculation))
                    Calculations.Add(Calculation);
                else
                {
                    try
                    {
                        string filePath = Path.Combine(SettingsManager.GetSettingsLocation(), "calculations.xml");
                        if (Calculations.Count > 0)
                            Calculator3dExporter.EncryptAndSerialize(filePath, Calculations.ToArray());
                        else
                        {
                            if (File.Exists(filePath))
                                File.Delete(filePath);
                        }
                    }
                    catch(Exception exc)
                    {
                        logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                    }
                }
                SelectedInfoTab = 0;
                logger.Info(string.Format(Strings.EventCalculationDoneFormated, Calculation.Name));
                if (SettingsManager.Current.General_OpenCalculationResultView)
                {
                    await ShowCalculationResult(Calculation);
                    //ShowEnhancedCalculationAction(newCalc);
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand NewCalculationCommand
        {
            get { return new RelayCommand(async(p) => await NewCalculationAction()); }
        }
        private async Task NewCalculationAction()
        {
            try
            {
                Calculation = null;
                await ClearFormAction();
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
        private async Task SelectedCalculationChangedAction(object p)
        {
            try
            {
                //Calculation3d calculation = p as Calculation3d;
                if (Calculation != null)
                {
                    await loadCalculation(Calculation);
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
            get => new RelayCommand(async(p) => await ShowCalculationResult(p as Calculation3d));
        }
        private async Task ShowCalculationResult(Calculation3d calc)
        {
            try
            {
                if (calc == null || IsShowingCalculationResult)
                    return;

                IsShowingCalculationResult = true;
                var _dialog = new CustomDialog() { 
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
            get => new RelayCommand(async(p) => await SaveCalculationAction());
        }
        private async Task SaveCalculationAction()
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
                            //CalculationFile.EncryptAndSerialize(saveFileDialog.FileName, this.Calculation);
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
            get => new RelayCommand(async(p) => await LoadCalculationAction());
        }
        private async Task LoadCalculationAction()
        {
            try
            {
                var openFileDialog = new System.Windows.Forms.OpenFileDialog
                {
                    // Allow to also load all calculations for awhile
                    Filter = StaticStrings.FilterCalculationFileAll,
                };

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        Calculation3d temp = new Calculation3d();
                        string fileExtension = Path.GetExtension(openFileDialog.FileName);
                        switch(fileExtension)
                        {
                            // Old calculation format
                            case ".3dc":
                                _3dPrinterCalculationModel old = CalculationFile.DecryptAndDeserialize(openFileDialog.FileName);
                                temp = await parseCalculationAsync(old);
                                
                                break;
                            case ".3dcx":
                                temp = Calculator3dExporter.DecryptAndDeserialize(openFileDialog.FileName);
                                break;
                            default:
                                break;
                        }
                        await loadCalculation(temp);
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
            get => new RelayCommand(async(p) => await ReadStlGcodeFileAction());
        }
        private async Task ReadStlGcodeFileAction()
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
        public ICommand ParseMultipleGcodesCommand
        {
            get => new RelayCommand(async(p) => await ParseMultipleGcodesAction());
        }
        private async Task ParseMultipleGcodesAction()
        {
            try
            {
                if (PrinterConfig == null)
                {
                    var res = await this._dialogCoordinator.ShowMessageAsync(this,
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
                    ObservableCollection<GCode> _files = new ObservableCollection<GCode>();
                    var openFileDialog = new System.Windows.Forms.OpenFileDialog
                    {
                        Filter = StaticStrings.FilterGCodeFile,
                        Multiselect = true,
                    };
                    cts = new CancellationTokenSource();
                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        // Split lists in small sizes
                        List<List<string>> filesList = CollectionHelper.Split<string>(openFileDialog.FileNames.ToList(), 4).ToList();
                        int lastTab = SelectedMainTab;
                        SelectedMainTab = 1;
                        ExpandGcodeMultiParserView = true;

                        foreach (List<string> files in filesList)
                        {
                            List<Task> tasks = new List<Task>();
                            foreach (string file in files)
                            {
                                GCode gc = new GCode(file);
                                GcodesQueue.Add(gc);
                                tasks.Add(Task.Run(async () =>
                                 {
                                     try
                                     {
                                         var prog = new Progress<int>(percent =>
                                         {
                                             gc.Progress = percent;
                                         });
                                         gc = await GCodeHelper.FromGCodeFile(gc, PrinterConfig, prog, cts.Token, SettingsManager.Current.GcodeParser_PreferValuesInCommentsFromKnownSlicers);

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
                                             GcodesQueue.Remove(gc);
                                         // return to previous tab
                                         if (GcodesQueue.Count == 0)
                                                 SelectedMainTab = lastTab;
                                         });

                                     }
                                     catch (Exception exc)
                                     {

                                     }
                                 }));
                            }
                            await Task.WhenAll(tasks);
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
        
        public ICommand OnDropStlFileCommand
        {
            get => new RelayCommand(async(p) => await OnDropStlFileAction(p));
        }
        private async Task OnDropStlFileAction(object p)
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
                        cts = new CancellationTokenSource();
                        if (PrinterConfig == null)
                        {
                            var res = await this._dialogCoordinator.ShowMessageAsync(this,
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
                            //var gcode = new GCode(filePath, AMax_xy, AMax_z, AMax_e, AMax_eExtrude, AMax_eRetract, PrintDurationCorrection);
                            var gcode = await GCodeHelper.FromFile(filePath, PrinterConfig, prog, cts.Token, SettingsManager.Current.GcodeParser_PreferValuesInCommentsFromKnownSlicers);

                            //await gcode.ReadGcodeAsync(prog, cts.Token);
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
                                Files.Add(new File3d()
                                {
                                    Name = gcode.FileName,
                                    FileName = gcode.FileName,
                                    FilePath = gcode.FilePath,
                                    PrintTime = gcode.PrintTime,
                                    Volume = gcode.ExtrudedFilamentVolume,
                                });
                                Gcode = gcode;
                                logger.Info(string.Format(Strings.EventGcodeFileParseSucceededFormated, Gcode.FileName, Gcode.ParsingDuration.ToString()));
                                if (string.IsNullOrEmpty(Name))
                                    Name = Path.GetFileName(Gcode.FilePath);
                                //await Create3dGcodeLayerModelAction();
                                //await Create2dGcodeLayerModelAction();
                                SelectedInfoTab = 2;
                            }
                            OnPropertyChanged(nameof(canSendGcode));
                        }
                        break;
                    case ".stl":
                        var Stl = createStlModel(filePath);
                        if (Stl != null)
                        {
                            StlFiles.Add(Stl);
                            StlFile = Stl;
                            SelectedInfoTab = 1;
                        }
                        break;
                    case ".3dcx":
                        //var calc = new Calculation3d();
                        //CalculationFile.Load(filePath, out calc);
                        var calc = Calculator3dExporter.DecryptAndDeserialize(filePath);
                        if (calc != null)
                        {
                            await loadCalculation(calc);
                        }
                        break;
                    case ".3dc":
                        var calcOld = new _3dPrinterCalculationModel();
                        CalculationFile.Load(filePath, out calcOld);
                        //var calc = Calculator3dExporter.DecryptAndDeserialize(filePath);
                        if (calcOld != null)
                        {
                            // Convert to new one
                            //Calculations.Add(calc);
                            //SelectedInfoTab = 0;
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
        }
        private void OnProgressLayerModelUpdateAction(int progress)
        {
            ProgressLayerModel = progress;
        }

        public ICommand CancelGcodeAnalyseCommand
        {
            get => new RelayCommand(async (p) => await CancelGcodeAnalyseAction());
        }
        private async Task CancelGcodeAnalyseAction()
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
        public ICommand Create2dGcodeLayerModelCommand
        {
            get => new RelayCommand(async(p) => await Create2dGcodeLayerModelAction());
        }
        private async Task Create2dGcodeLayerModelAction()
        {
            try 
            {
                if (Gcode == null || Gcode.LayerModelGenerated)
                    return;
                var prog = new Progress<int>(percent => OnProgressLayerModelUpdateAction(percent));
                //await Task.Run(() => Gcode.Get2dGcodeLayerModelListAsync(prog));
                //await Gcode.Get2dGcodeLayerModelListAsync(prog);
                Gcode.ModelLayers = await GCodeHelper.Create2dGcodeLayerModelListAsync(Gcode, prog);
                GcodeMaxLayer = Gcode.ModelLayers.Count - 1;
                GcodeLayer = 0;
                GcodeLayer = 1;
                Messenger.Default.Send(new NotificationMessage(SelectedGcodeFiles, "ZoomToFitGcode"));
                //GcodeCamera.ZoomExtents();
                //Create3dGcodeLayerModelAction();
                //var model3d = await GCodeHelper.Create3dGcodeLayerModelListAsync(Gcode, prog);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        public ICommand Create3dGcodeLayerModelCommand
        {
            get => new RelayCommand(async (p) => await Create3dGcodeLayerModelAction());
        }
        private async Task Create3dGcodeLayerModelAction()
        {
            try 
            {
                if (Gcode == null) return;
                if(!Gcode.LayerModelGenerated)
                {
                    var prog = new Progress<int>(percent => OnProgressLayerModelUpdateAction(percent));
                    Gcode.Model3D = await GCodeHelper.BuildGcodeLayerModelListAsync(Gcode, prog);
                    //Gcode.ModelLayers = await GCodeHelper.Create2dGcodeLayerModelListAsync(Gcode, prog);
                    Gcode.LayerModelGenerated = true;
                }
                //Gcode3dModel = new ObservableCollection<LinesVisual3D>(Gcode.ModelLayers);
                //Gcode3dPoints = new Point3DCollection(Gcode.ModelLayers.SelectMany(lines => lines.Points));

            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
        public ICommand SelectedGcodeChangedCommand
        {
            get => new RelayCommand(async(p) => await SelectedGcodeChangedAction(p));
        }
        private async Task SelectedGcodeChangedAction(object p)
        {
            try {
                GCode gcode = Gcode;
                
                if (gcode != null)
                {
                    // Created layer model if needed
                    if (gcode.Model3D.Count == 0 && !gcode.LayerModelGenerated)
                    {
                        await Create3dGcodeLayerModelAction();
                        
                    }
                    GcodeMaxLayer = Gcode.Model3D.Count - 1;
                    GcodeLayer = 0;
                    GcodeLayer = 1;

                    // 2D
                    var lineBuilder = await gcode.GetGcodeLayerLineBuilderAsync(GcodeLayer);
                    var layerGeometry = new LineGeometryModel3D();
                    layerGeometry.Color = GcodeColor;
                    layerGeometry.Geometry = lineBuilder.ToLineGeometry3D();
                    layerGeometry.Transform = new TranslateTransform3D(new Vector3D(0, 0, 0));
                    GcodeLayerGeometry = layerGeometry;

                    // 3D
                    var collection = new ObservableElement3DCollection();
                    foreach(var builder in gcode.Model3D)
                    {
                        var layer = new LineGeometryModel3D();
                        layer.Color = GcodeColor;
                        layer.Geometry = builder.ToLineGeometry3D();
                        layer.Transform = new TranslateTransform3D(new Vector3D(0, 0, layer.Bounds.Depth));
                        collection.Add(layer);
                    }
                    Gcode3dLayerGeometry = collection;

                    Messenger.Default.Send(new NotificationMessage(SelectedGcodeFiles, "ZoomToFitGcode"));
                }
                else
                {
                    GcodeLayer = 0;
                    GcodeMaxLayer = 0;
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
        public ICommand SelectedLayerChangedCommand
        {
            get => new RelayCommand(async (p) => await SelectedLayerChangedAction());
        }
        private async Task SelectedLayerChangedAction()
        {
            try {
                if (Gcode == null)
                    return;

                // 2D
                var lineBuilder = await Gcode.GetGcodeLayerLineBuilderAsync(GcodeLayer);

                var layerGeometry = new LineGeometryModel3D();
                layerGeometry.Color = GcodeColor;
                layerGeometry.Geometry = lineBuilder.ToLineGeometry3D();
                layerGeometry.Transform = new TranslateTransform3D(new Vector3D(0, 0, 0));
                GcodeLayerGeometry = layerGeometry;

                Messenger.Default.Send(new NotificationMessage(SelectedGcodeFiles, "ZoomToFitGcode"));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
        public ICommand SelectedCalculationsChangedCommand
        {
            get => new RelayCommand(async (p) => await SelectedCalculationsChangedAction(p));
        }
        private async Task SelectedCalculationsChangedAction(object p)
        {
            CalculationViewInfo calc = p as CalculationViewInfo;
            if (calc != null)
            {
                //this.ReplacementCosts = printer.Price;
            }
        }

        public ICommand AddCustomAdditionCommand
        {
            get { return new RelayCommand(async (p) => await AddCustomAdditionAction()); }
        }
        private async Task AddCustomAdditionAction()
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
        private async Task SliceSelectedStlFilesAction()
        {
            try
            {
                ObservableCollection<Stl> _files = new ObservableCollection<Stl>(SelectedStlFiles.Cast<Stl>().ToList());

                var _dialog = new CustomDialog() { Title = Strings.PrepareSlicing };
                _dialog.Style = Application.Current.FindResource("FullWidthDialogStyle") as Style;

                var newSlicerDialogViewModel = new SliceStlDialogViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    if(instance.SlicerName.Group == SlicerViewManager.Group.CLI)
                    { 
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
                    }
                    else
                    {
                        var res = await _dialogCoordinator.ShowMessageAsync(this,
                            Strings.DialogImportedSlicedGcodeFilesHeadline,
                            Strings.DialogImportedSlicedGcodeFilesContent,
                            MessageDialogStyle.AffirmativeAndNegative);
                        if (res == MessageDialogResult.Affirmative)
                        {
                            var openFileDialog = new System.Windows.Forms.OpenFileDialog
                            {
                                Filter = StaticStrings.FilterGCodeFile,
                            };

                            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                foreach (string file in openFileDialog.FileNames)
                                {
                                    var result = await readFileInformation(file);
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
        private async Task SliceSelectedStlFilesChildWindowAction()
        {
            try
            {
                ObservableCollection<Stl> _files = new ObservableCollection<Stl>(SelectedStlFiles.Cast<Stl>().ToList());

                var _childWindow = new ChildWindow()
                {
                    Title = Strings.PrepareSlicing,
                    AllowMove = true,
                    ShowCloseButton = false,
                    CloseByEscape = false,
                    IsModal = true,
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (System.Windows.Media.Color)Application.Current.Resources["MahApps.Colors.Gray2"] },
                    Padding = new Thickness(50),
                    //Width = Application.Current.MainWindow.ActualWidth * 0.8,
                    //Height = Application.Current.MainWindow.ActualHeight * 0.8,
                    
                };
                //_childWindow.Style = Application.Current.FindResource("FullWidthDialogStyle") as Style;
                var newSlicerDialogViewModel = new SliceStlDialogViewModel(async instance =>
                {
                    _childWindow.Close();
                    if (instance.SlicerName.Group == SlicerViewManager.Group.CLI)
                    {
                        var files = instance.FilesForImport;
                        if (files.Count > 0)
                        {
                            var res = await _dialogCoordinator.ShowMessageAsync(this,
                                 Strings.DialogFilesToImportHeadline,
                                 string.Format(Strings.DialogFilesToImportFormatedContent, files.Count),
                                 MessageDialogStyle.AffirmativeAndNegative                           
                                 );
                            if (res == MessageDialogResult.Affirmative)
                            {
                                foreach (string file in files)
                                {
                                    var result = await readFileInformation(file);
                                }
                            }
                        }
                    }
                    else
                    {
                        var res = await _dialogCoordinator.ShowMessageAsync(this,
                            Strings.DialogImportedSlicedGcodeFilesHeadline,
                            Strings.DialogImportedSlicedGcodeFilesContent,
                            MessageDialogStyle.AffirmativeAndNegative);
                        if (res == MessageDialogResult.Affirmative)
                        {
                            var openFileDialog = new System.Windows.Forms.OpenFileDialog
                            {
                                Filter = StaticStrings.FilterGCodeFile,
                            };

                            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                foreach (string file in openFileDialog.FileNames)
                                {
                                    var result = await readFileInformation(file);
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
        private async Task OpenStlFileLocationAction()
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
        private async Task DeleteSelectedStlFilesAction()
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
            get { return new RelayCommand(async (p) => await OpenGCodeFileLocationAction()); }
        }
        private async Task OpenGCodeFileLocationAction()
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
            get { return new RelayCommand(async (p) => await OpenGCodeViewerAction()); }
        }
        private async Task OpenGCodeViewerAction()
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
            get { return new RelayCommand(async (p) => await OpenGCodeViewerNewWindowAction()); }
        }
        private async Task OpenGCodeViewerNewWindowAction()
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
            get { return new RelayCommand(async (p) => await DeleteSelectedGCodeFilesAction()); }
        }
        private async Task DeleteSelectedGCodeFilesAction()
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
                            if(Gcode == file)
                            {
                                // Clear 2D/3D views on deletion
                                GcodeLayerGeometry = new LineGeometryModel3D();
                                Gcode3dLayerGeometry = new ObservableElement3DCollection();
                            }
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

        public ICommand ExportCalculationsCommand
        {
            get => new RelayCommand(async (p) => await ExportCalculationsAction());
        }
        private async Task ExportCalculationsAction()
        {
            try
            {
                ObservableCollection<Calculation3d> calculations = SelectedCalculationsView != null ?
                    new ObservableCollection<Calculation3d>(SelectedCalculationsView.OfType<CalculationViewInfo>().ToList().Select(i => i.Calculation).ToList()) :
                    new ObservableCollection<Calculation3d>();
                if (calculations.Count == 0) return;

                //If write to template is set
                if (SettingsManager.Current.ExporterExcel_WriteToTemplate)
                {
                    var _dialog = new CustomDialog() { Title = Strings.Export };
                    var exportCalculationsViewModel = new ExportCalculationViewModel(async instance =>
                    {
                        await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                        bool result = false;
                        foreach (ExporterTemplate template in instance.Templates)
                        {
                            ExcelHandler.WriteCalculationsToExporterTemplate(calculations, instance.ExportPath, template, instance.ExportAsPdf);
                            if (!result)
                                break;
                        }
                        if (result)
                        {
                            var res = await _dialogCoordinator.ShowMessageAsync(this, Strings.DialogExportToFileSucceededHeadline,
                                    string.Format(Strings.DialogExportToFileSucceededForamtedContent, instance.ExportPath),
                                    MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings()
                                    {
                                        AffirmativeButtonText = Strings.OpenFolder,
                                        NegativeButtonText = Strings.Close
                                    });
                            if (res == MessageDialogResult.Affirmative)
                            {
                                string folder = Path.GetDirectoryName(instance.ExportPath);
                                if (Directory.Exists(folder))
                                    Process.Start(folder);
                            }
                        }
                    }, instance =>
                    {
                        _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    }
                    , Calculations
                    , ExporterTarget.List
                    , _dialogCoordinator
                    );

                    _dialog.Content = new Views.ExportCalculationsDialogView()
                    {
                        DataContext = exportCalculationsViewModel
                    };
                    await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
                }
                //
                else
                {
                    var saveFileDialog = new System.Windows.Forms.SaveFileDialog
                    {
                        Filter = StaticStrings.FilterExcelFile,
                    };
                    if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {

                        try
                        {
                            var result = ExcelHandler.ExportCaclulations(calculations, saveFileDialog.FileName, false);
                            if(result)
                            {
                                var res = await _dialogCoordinator.ShowMessageAsync(this, Strings.DialogExportToFileSucceededHeadline,
                                    string.Format(Strings.DialogExportToFileSucceededForamtedContent, saveFileDialog.FileName),
                                    MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, new MetroDialogSettings()
                                    {
                                        AffirmativeButtonText = Strings.OpenFile,
                                        FirstAuxiliaryButtonText = Strings.OpenFolder,
                                        NegativeButtonText = Strings.Close
                                    });
                                if(res == MessageDialogResult.Affirmative)
                                {
                                    if(File.Exists(saveFileDialog.FileName))
                                        Process.Start(saveFileDialog.FileName);
                                }
                                else if(res == MessageDialogResult.FirstAuxiliary)
                                {
                                    string folder = Path.GetDirectoryName(saveFileDialog.FileName);
                                    if (Directory.Exists(folder))
                                        Process.Start(folder);
                                }
                            }
                            //logger.Info(string.Format(Strings.EventCalculationSavedFormated, Calculation.Name, saveFileDialog.FileName));
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
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
        public ICommand ExportCalculationCommand
        {
            get => new RelayCommand(async(p) => await ExportCalculationAction(p));
        }
        private async Task ExportCalculationAction(object calc)
        {
            try
            {
                Calculation3d currentCalculation = calc as Calculation3d;
                if (currentCalculation == null) return;
                
                if (SettingsManager.Current.ExporterExcel_WriteToTemplate)
                {
                    var _dialog = new CustomDialog() { Title = Strings.Export };
                    var exportCalculationsViewModel = new ExportCalculationViewModel(async instance =>
                    {
                        await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                        bool result = false;
                        foreach (ExporterTemplate template in instance.SelectedTemplates)
                        {
                            result = ExcelHandler.WriteCalculationToExporterTemplate(currentCalculation, instance.ExportPath, template, instance.ExportAsPdf);
                            if (!result)
                                break;
                        }
                        if(result)
                        {
                            var res = await _dialogCoordinator.ShowMessageAsync(this, Strings.DialogExportToFileSucceededHeadline,
                                    string.Format(Strings.DialogExportToFileSucceededForamtedContent, instance.ExportPath),
                                    MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings()
                                    {
                                        AffirmativeButtonText = Strings.OpenFolder,
                                        NegativeButtonText = Strings.Close
                                    });
                            if (res == MessageDialogResult.Affirmative)
                            {
                                string folder = instance.ExportPath;
                                if (Directory.Exists(folder))
                                    Process.Start(folder);
                            }
                        }
                    }, instance =>
                    {
                        _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    }
                    , Calculations
                    , ExporterTarget.Single
                    , _dialogCoordinator
                    );

                    _dialog.Content = new Views.ExportCalculationsDialogView()
                    {
                        DataContext = exportCalculationsViewModel
                    };
                    await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
                }
                //
                else
                {
                    var saveFileDialog = new System.Windows.Forms.SaveFileDialog
                    {
                        Filter = StaticStrings.FilterExcelFile,
                    };
                    if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {

                        try
                        {
                            var calculations = new ObservableCollection<Calculation3d>();
                            calculations.Add(currentCalculation);
                            var result = ExcelHandler.ExportCaclulations(calculations, saveFileDialog.FileName, false);
                            if (result)
                            {
                                var res = await _dialogCoordinator.ShowMessageAsync(this, Strings.DialogExportToFileSucceededHeadline,
                                    string.Format(Strings.DialogExportToFileSucceededForamtedContent, saveFileDialog.FileName),
                                    MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, new MetroDialogSettings()
                                    {
                                        AffirmativeButtonText = Strings.OpenFile,
                                        FirstAuxiliaryButtonText = Strings.OpenFolder,
                                        NegativeButtonText = Strings.Close
                                    });
                                if (res == MessageDialogResult.Affirmative)
                                {
                                    if (File.Exists(saveFileDialog.FileName))
                                        Process.Start(saveFileDialog.FileName);
                                }
                                else if (res == MessageDialogResult.FirstAuxiliary)
                                {
                                    string folder = Path.GetDirectoryName(saveFileDialog.FileName);
                                    if (Directory.Exists(folder))
                                        Process.Start(folder);
                                }
                            }
                            //logger.Info(string.Format(Strings.EventCalculationSavedFormated, Calculation.Name, saveFileDialog.FileName));
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
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand SaveCalculationFromTemplateCommand
        {
            get => new RelayCommand(async(p) => await SaveCalculationFromTemplateAction(p));
        }
        private async Task SaveCalculationFromTemplateAction(object calc)
        {
            try
            {
                Calculation3d currentCalculation = calc as Calculation3d;
                if (currentCalculation == null) return;

                var saveFileDialog = new System.Windows.Forms.SaveFileDialog
                {
                    Filter = StaticStrings.FilterCalculationFile,
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
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand LoadCalculationIntoCalculatorFromTemplateCommand
        {
            get => new RelayCommand(async(p) => await LoadCalculationIntoCalculatorFromTemplateAction(p));
        }
        private async Task LoadCalculationIntoCalculatorFromTemplateAction(object calc)
        {
            try
            {
                Calculation3d currentCalculation = calc as Calculation3d;
                if (currentCalculation == null) return;
                // Calculation will be loaded automatically if changed
                Calculation = currentCalculation;
                
                //await loadCalculation(currentCalculation);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
        public ICommand ExportCalculationToExcelCommand
        {
            get => new RelayCommand(async(p) => await ExportCalculationToExcelAction());
        }
        private async Task ExportCalculationToExcelAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.Export };
                var exportCalculationsViewModel = new ExportCalculationViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    foreach (ExporterTemplate template in instance.SelectedTemplates)
                    {
                        ExcelHandler.WriteCalculationToExporterTemplate(SelectedCalculationView.Calculation, instance.ExportPath, template, instance.ExportAsPdf);
                    }
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                }
                , Calculations
                , ExporterTarget.Single
                , _dialogCoordinator
                );

                _dialog.Content = new Views.ExportCalculationsDialogView()
                {
                    DataContext = exportCalculationsViewModel
                };
                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
        #endregion

        #region Helix
        public ICommand ZoomToFitCommand
        {
            get => new RelayCommand(async(p) => await ZoomToFitAction(p));
        }
        private async Task ZoomToFitAction(object p)
        {
            try
            {

                Viewport3DX v = p as Viewport3DX;
                if (v != null)
                {
                    v.ZoomExtents();
                }
                /* Helix.Wpf
                HelixViewport3D v = p as HelixViewport3D;
                if (v != null)
                {
                    v.ZoomExtents();
                }
                */
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
        public ICommand ChangeCameraGridCommand
        {
            get => new RelayCommand(async(p) => await ChangeCameraGridAction(p));
        }
        private async Task ChangeCameraGridAction(object p)
        {
            try
            {
                /* Media3D
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
                */
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
            get { return new RelayCommand(async(p) => await SendGCodeFileAction(p)); }
        }
        private async Task SendGCodeFileAction(object parameter)
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
                switch (printServer)
                {
                    case "OctoPrint":
                        break;
                    case "RepetierServerPro":
                        break;
                    default:
                        logger.Error(string.Format(Strings.EventUnknownPrintServerFormated, printServer));
                        break;
                }
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
            get => new RelayCommand(async(p) => await DeleteSelectedGcodeFileAction(p));
        }
        private async Task DeleteSelectedGcodeFileAction(object p)
        {
            try
            {
                GCode gcode = p as GCode;
                if (gcode != null)
                {
                    Gcodes.Remove(gcode);
                    // Media3D
                    //GcodePoints = new Point3DCollection();
                    GcodeLayerGeometry = new LineGeometryModel3D();
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

        public ICommand AddPrintInformationManuallyCommand
        {
            get => new RelayCommand(async(p) => await AddPrintInformationManuallyAction());
        }
        private async Task AddPrintInformationManuallyAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.ManualPrintJobInformation };
                var newViewModel = new ManualPrintJobInfoDialogViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    // Either use the volume or the weight
                    double volume = 0;
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

                    logger.Info(string.Format(Strings.EventAddedItemFormated, Materials[Materials.Count - 1].Name));
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

        #endregion

        public ICommand SelectedMaterialChangedCommand
        {
            get => new RelayCommand(async(p) => await SelectedMaterialChangedAction(p));
        }
        private async Task SelectedMaterialChangedAction(object p)
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
            get { return new RelayCommand(async(p) => await AddNewPrinterAction()); }
        }
        private async Task AddNewPrinterAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.NewPrinter };
                var newPrinterViewModel = new New3DPrinterViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Printers.Add(getPrinterFromInstance(instance));
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
            get { return new RelayCommand(async(p) => await AddNewMaterialAction()); }
        }
        private async Task AddNewMaterialAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Resources.Localization.Strings.NewManufacturer };
                var newMaterialViewModel = new NewMaterialViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Materials.Add(getMaterialFromInstance(instance));

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
        
        #region Printer
        public ICommand EditPrinterCommand
        {
            get => new RelayCommand(async(p) => await EditPrinterAction(p));
        }
        private async Task EditPrinterAction(object printer)
        {
            try
            {
                var selectedPrinter = printer as Printer3d;
                if (selectedPrinter == null)
                {
                    return;
                }
                var _dialog = new CustomDialog() { Title = Strings.EditPrinter };
                var newPrinterViewModel = new New3DPrinterViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    updatePrinterFromInstance(instance, selectedPrinter);
                    /*
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
                    */
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
            get => new RelayCommand(async(p) => await DeletePrinterAction(p));
        }
        private async Task DeletePrinterAction(object p)
        {
            try
            {
                Printer3d printer = p as Printer3d;
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

        public ICommand DuplicatePrinterCommand
        {
            get => new RelayCommand(async(p) => await DuplicatePrinterAction(p));
        }
        private async Task DuplicatePrinterAction(object p)
        {
            try
            {
                Printer3d printer = p as Printer3d;
                if (printer != null)
                {
                    var duplicates = Printers.Where(prt => prt.Model.StartsWith(prt.Model.Split('_')[0]));

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
            get => new RelayCommand(async (p) => await DuplicateMaterialAction(p));
        }
        private async Task DuplicateMaterialAction(object p)
        {
            try
            {
                Material3d material = p as Material3d;
                if (material != null)
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
        private async Task EditMaterialAction(object material)
        {
            try
            {
                var selectedMaterial = material as Material3d;
                if (selectedMaterial == null)
                {
                    return;
                }
                var _dialog = new CustomDialog() { Title = Strings.EditMaterial };
                var newMaterialViewModel = new NewMaterialViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    updateMaterialFromInstance(instance, selectedMaterial);
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
            get => new RelayCommand(async(p) => await DeleteMaterialAction(p));
        }
        private async Task DeleteMaterialAction(object p)
        {
            try
            {
                Material3d material = p as Material3d;
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
        
        public ICommand ReorderMaterialCommand
        {
            get => new RelayCommand(async(p) => await ReorderMaterialAction(p));
        }
        private async Task ReorderMaterialAction(object parameter)
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
        private async Task DeleteCalculationFromTemplateAction(object p)
        {
            try
            {
                Calculation3d calc = p as Calculation3d;
                if (calc != null)
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
                            await ClearFormAction();
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
            get => new RelayCommand(async (p) => await SelectAllCalculationsAction());
        }
        private async Task SelectAllCalculationsAction()
        {
            try
            {
                if (Calculations.Count == 0) return;
                if(AllCalcsSelected)
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
        private async Task DeleteSelectedCalculationsAction()
        {
            try
            {
                var res = await _dialogCoordinator.ShowMessageAsync(this,
                       Strings.DialogDeleteSelectedCalculationsHeadline, Strings.DialogDeleteSelectedCalculationsContent,
                       MessageDialogStyle.AffirmativeAndNegative
                       );
                if (res == MessageDialogResult.Affirmative)
                {
                    IList collection = new ArrayList(SelectedCalculationsView);
                    for (int i = 0; i < collection.Count; i++)
                    {
                        var obj = collection[i] as CalculationViewInfo;
                        if (obj == null)
                            continue;
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, obj.Name));
                        Calculations.Remove(obj.Calculation);
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

        // END - Actions from template
        #endregion

        #endregion

        #region Methods
        private Material3d getMaterialFromInstance(NewMaterialViewModel instance, Material3d material = null)
        {
            Material3d temp = material ?? new Material3d();
            try
            {
                temp.Id = instance.Id;
                temp.Name = instance.Name;
                temp.SKU = instance.SKU;
                temp.UnitPrice = instance.Price;
                temp.Unit = instance.Unit;
                temp.Uri = instance.LinkToReorder;
                temp.PackageSize = instance.PackageSize;
                //Supplier = instance.Supplier;
                temp.Manufacturer = instance.Manufacturer;
                temp.Density = instance.Density;
                temp.TypeOfMaterial = instance.TypeOfMaterial;
                temp.MaterialFamily = instance.MaterialFamily;
                temp.Attributes = instance.Attributes.ToList();
                //ColorCode = 
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.DialogExceptionFormatedContent, exc.Message, exc.TargetSite);
            }
            return temp;
        }
        private void updateMaterialFromInstance(NewMaterialViewModel instance, Material3d material)
        {
            try
            {
                getMaterialFromInstance(instance, material);

                SettingsManager.Current.PrinterMaterials = Materials;
                SettingsManager.Save();

                OnPropertyChanged(nameof(Materials));
                createMaterialViewInfos();
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.DialogExceptionFormatedContent, exc.Message, exc.TargetSite);
            }
        }
        
        private Printer3d getPrinterFromInstance(New3DPrinterViewModel instance, Printer3d printer = null)
        {
            Printer3d temp = printer ?? new Printer3d();
            try
            {
                temp.Id = instance.Id;
                temp.Price = instance.Price;
                temp.Type = instance.Type;
                //temp.Supplier = instance.Supplier;
                temp.Manufacturer = instance.Manufacturer;
                temp.MaterialType = instance.MaterialFamily;
                temp.Model = instance.Model;
                temp.UseFixedMachineHourRating = instance.UseFixedMachineHourRating;
                if (temp.UseFixedMachineHourRating)
                    temp.HourlyMachineRate = new HourlyMachineRate() { FixMachineHourRate = instance.MachineHourRate };
                else
                    temp.HourlyMachineRate = instance.MachineHourRateCalculation;
                temp.BuildVolume = instance.BuildVolume;
                temp.PowerConsumption = instance.PowerConsumption;
                temp.Uri = instance.LinkToReorder;
                temp.Attributes = instance.Attributes.ToList();
                // Not implemented yet
                temp.Maintenances = new ObservableCollection<Maintenance3d>();
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.DialogExceptionFormatedContent, exc.Message, exc.TargetSite);
            }
            return temp;
        }
        private void updatePrinterFromInstance(New3DPrinterViewModel instance, Printer3d printer)
        {
            try
            {
                getPrinterFromInstance(instance, printer);
                SettingsManager.Current.Printers = Printers;
                SettingsManager.Save();

                OnPropertyChanged(nameof(Printers));
                createPrinterViewInfos();
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.DialogExceptionFormatedContent, exc.Message, exc.TargetSite);
            }
        }

        private async Task loadCalculation(Calculation3d calculation)
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
                    OnPropertyChanged(nameof(canBeSaved));
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
        private async Task<Calculation3d> parseCalculationAsync(_3dPrinterCalculationModel calculation)
        {
            try
            {
                // Parse calculation to new format
                var temp = new Calculation3d()
                {
                    Id = calculation.Id,
                    Name = calculation.Name,
                    ApplyenergyCost = calculation.ApplyEnergyCosts,
                    EnergyCostsPerkWh = Convert.ToDouble(calculation.EnergyPrice),
                    PowerLevel = calculation.PowerLevel,
                    Printers = new ObservableCollection<Printer3d>(calculation.Printers.Select(printer => new Printer3d()
                    {
                        Id = printer.Id,
                        Model = printer.Model,
                        Price = Convert.ToDouble(printer.Price),
                        PowerConsumption = Convert.ToDouble(printer.PowerConsumption),
                        UseFixedMachineHourRating = printer.UseFixedMachineHourRating,
                        Uri = printer.ShopUri,
                        Type = (Printer3dType)Enum.Parse(typeof(Printer3dType), printer.Type.ToString()),
                        MaterialType = (Material3dFamily)Enum.Parse(typeof(Material3dFamily), printer.Kind.ToString()),
                        Attributes = printer.Kind == _3dPrinterMaterialKind.Filament ?
                                            new List<AndreasReitberger.Models.PrinterAdditions.Printer3dAttribute>()
                                            {
                                            new AndreasReitberger.Models.PrinterAdditions.Printer3dAttribute() { Attribute = Strings.TemperatureNozzle, Value = printer.MaxNozzleTemperature},
                                            new AndreasReitberger.Models.PrinterAdditions.Printer3dAttribute() { Attribute = Strings.TemperatureHeatedBed, Value = printer.MaxHeatbedTemperature},
                                            } :
                                            new List<AndreasReitberger.Models.PrinterAdditions.Printer3dAttribute>()
                                            ,
                        Manufacturer = printer.Manufacturer != null ?
                                            SettingsManager.Current.Manufacturers.FirstOrDefault(Manufacturer => Manufacturer.Id == printer.Manufacturer.Id) :
                                            new AndreasReitberger.Models.Manufacturer()
                                            ,
                        BuildVolume = printer.BuildVolume != null ?
                                            new AndreasReitberger.Models.PrinterAdditions.BuildVolume(
                                                Convert.ToDouble(printer.BuildVolume.X),
                                                Convert.ToDouble(printer.BuildVolume.Y),
                                                Convert.ToDouble(printer.BuildVolume.Z)) :
                                            new AndreasReitberger.Models.PrinterAdditions.BuildVolume(0, 0, 0)
                                            ,
                    })),
                    Materials = new ObservableCollection<Material3d>(calculation.Materials.Select(material => new Material3d()
                    {
                        Id = material.Id,
                        Name = material.Name,
                        SKU = material.SKU,
                        Uri = material.LinkToReorder,
                        Density = Convert.ToDouble(material.Density),
                        ColorCode = material.ColorCode,
                        MaterialFamily = (Material3dFamily)Enum.Parse(typeof(Material3dFamily), material.TypeOfMaterial.Kind.ToString()),
                        TypeOfMaterial = SettingsManager.Current.MaterialTypes.FirstOrDefault(type => type.Material == material.TypeOfMaterial.Material),
                        PackageSize = Convert.ToDouble(material.PackageSize),
                        Unit = (AndreasReitberger.Enums.Unit)Enum.Parse(typeof(AndreasReitberger.Enums.Unit), material.Unit.ToString()),
                        Attributes = material.TypeOfMaterial.Kind == _3dPrinterMaterialKind.Filament ?
                                            new List<AndreasReitberger.Models.MaterialAdditions.Material3dAttribute>()
                                            {
                                            new AndreasReitberger.Models.MaterialAdditions.Material3dAttribute() { Attribute = Strings.TemperatureNozzle, Value = material.TemperatureNozzle},
                                            new AndreasReitberger.Models.MaterialAdditions.Material3dAttribute() { Attribute = Strings.TemperatureHeatedBed, Value = material.TemperatureHeatbed},
                                            } :
                                            new List<AndreasReitberger.Models.MaterialAdditions.Material3dAttribute>()
                                            ,
                        Manufacturer = material.Manufacturer != null ?
                                            SettingsManager.Current.Manufacturers.FirstOrDefault(Manufacturer => Manufacturer.Id == material.Manufacturer.Id) :
                                            new AndreasReitberger.Models.Manufacturer()
                                            ,

                    })),
                    WorkSteps = new ObservableCollection<Workstep>(calculation.Worksteps.Select(workstep => new Workstep()
                    {
                        Id = workstep.Id,
                        Name = workstep.Name,
                        Duration = workstep.Duration != null ? workstep.Duration.TotalHours : 0,
                        Price = Convert.ToDouble(workstep.Price),
                        Category = new AndreasReitberger.Models.WorkstepAdditions.WorkstepCategory()
                        {
                            Name = workstep.Category.Name,
                        },
                        CalculationType = (AndreasReitberger.Enums.CalculationType)Enum.Parse(typeof(AndreasReitberger.Enums.CalculationType), workstep.CalculationType.ToString()),
                        Type = (AndreasReitberger.Enums.WorkstepType)Enum.Parse(typeof(AndreasReitberger.Enums.WorkstepType), workstep.Type.ToString()),

                    })),
                    Rates = new ObservableCollection<CalculationAttribute>()
                    {
                        new CalculationAttribute() { Attribute = "Tax", Type = CalculationAttributeType.Tax, 
                            Value = Convert.ToDouble(calculation.TaxRate), IsPercentageValue = true, SkipForCalculation = !calculation.ApplyTax},
                        new CalculationAttribute() {Attribute = "Margin", Type = CalculationAttributeType.Margin, 
                            Value = calculation.Profit, IsPercentageValue = true},
                    },
                    FixCosts = new ObservableCollection<CalculationAttribute>()
                    {
                        new CalculationAttribute() { Attribute = "HandlingFee", Type = CalculationAttributeType.FixCost,
                            Value = Convert.ToDouble(calculation.HandlingFee), IsPercentageValue = false},
                    },
                    Files = new ObservableCollection<File3d>()
                    {
                        new File3d()
                        {
                            FileName = calculation.Name,
                            File = calculation.Name,
                            Name = calculation.Name,
                            Volume = Convert.ToDouble(calculation.Volume),
                            PrintTime = Convert.ToDouble(calculation.Duration),
                            Quantity = calculation.Quantity,
                        }
                    }
                };
                return temp;
            }
            catch (Exception exc)
            {
                return null;
            }
        }

        private void ResizeGcodeMultiParse(bool dueToChangedSize)
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
        private void ResizeProfile(bool dueToChangedSize)
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
                    ProfileWidth = new GridLength(GlobalStaticConfiguration.CalculationView_WidthCollapsed);
                }
            }

            _canProfileWidthChange = true;
        }
        private Stl createStlModel(string StlFilePath)
        {
            try
            {
                Stl stl = new Stl(StlFilePath);
                read3dFile(StlFilePath);
                /*
                Models = new ObservableCollection<MeshGeometryModel3D>();
                var stlReader = new StLReader();
                var model = stlReader.Read(StlFilePath);
                foreach(var part in model)
                {
                    var geometry = new MeshGeometryModel3D()
                    {
                        Geometry = part.Geometry,
                        //Material = part.Material,
                    };
                    Models.Add(geometry);
                }
                */
                /*
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
                */
                return stl;
            }
            catch(Exception exc)
            {
                logger.ErrorFormat(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return new Stl();
            }
        }
        /**/
        private async Task read3dFile(string path)
        {
            if (path == null)
            {
                return;
            }
            //StopAnimation();

            //IsLoading = true;
            await Task.Run(() =>
            {
                var loader = new Importer();
                return loader.Load(path);
            }).ContinueWith((result) =>
            {
                //IsLoading = false;
                if (result.IsCompleted)
                {
                    scene = result.Result;
                    GroupModel.Clear();
                    if (scene != null)
                    {
                        GroupModel.AddNode(scene.Root);
                    }
                }
                else if (result.IsFaulted && result.Exception != null)
                {
                    MessageBox.Show(result.Exception.Message);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            
        }
        
        public void OnViewVisible()
        {
            OnPropertyChanged(nameof(isLicenseValid));
            LoadDefaults();

            createMaterialViewInfos();
            createPrinterViewInfos();
            createWorkstepViewInfos();
        }

        public void OnViewHide()
        {

        }

        #endregion
    }

}
