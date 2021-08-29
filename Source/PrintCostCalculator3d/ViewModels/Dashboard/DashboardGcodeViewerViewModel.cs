using AndreasReitberger.Models;
using GalaSoft.MvvmLight.Messaging;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Animations;
using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Enums;
using PrintCostCalculator3d.Models.Documentation;
using PrintCostCalculator3d.Models.Events;
using PrintCostCalculator3d.Models.GCode;
using PrintCostCalculator3d.Models.Messaging;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PrintCostCalculator3d.ViewModels.Dashboard
{

    public class DashboardGcodeViewerViewModel : ViewModelBase
    {
        #region Variables
        //readonly IDialogCoordinator _dialogCoordinator;
        readonly SharedCalculatorInstance SharedCalculatorInstance = SharedCalculatorInstance.Instance;
        #endregion

        #region Fields
        public Viewport3DX viewGcode2d;
        public Viewport3DX viewGcode3d;
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
                    SettingsManager.Current.GcodeInfo_ExpandView = value;

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

                if (!IsLoading && Math.Abs(value.Value - GlobalStaticConfiguration.GcodeInfo_WidthCollapsed) > GlobalStaticConfiguration.FloatPointFix)
                    // Do not save the size when collapsed
                    SettingsManager.Current.GcodeInfo_ProfileWidth = value.Value;

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
                Task.Run(async() => await CreateGcodeModelAction(Gcode));
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
                //TotalFileCount = SelectedGcodeFiles.Count + SelectedStlFiles.Count;
                if (Equals(value, _selectedGcodeFiles))
                {
                    return;
                }

                _selectedGcodeFiles = value;
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
        #endregion

        #region 3dModel
        public SceneNodeGroupModel3D GroupModel { get; } = new SceneNodeGroupModel3D();
        public ObservableCollection<Animation> Animations { get; } = new ObservableCollection<Animation>();

        public TextureModel EnvironmentMap { get; }

        bool _renderEnviromentMap = false;
        public bool RenderEnvironmentMap
        {
            get => _renderEnviromentMap;
            set
            {
                if (_renderEnviromentMap == value) return;
                _renderEnviromentMap = value;
                OnPropertyChanged();
            }
        }

        #region GcodeViewer
        EffectsManager _effectManager = GlobalStaticConfiguration.DefaulftEffectManager; //new DefaultEffectsManager();
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

        HelixToolkit.Wpf.SharpDX.PerspectiveCamera _gcodeCamera = new()
        {
            LookDirection = new Vector3D(0, 0, -200),
            Position = new Point3D(100, 100, 200),
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
                {
                    return;
                }

                _gcodeCamera = value;
                OnPropertyChanged();
            }
        }


        HelixToolkit.Wpf.SharpDX.PerspectiveCamera _gcode3dCamera = new()
        {
            LookDirection = new Vector3D(0, 500, -100),
            Position = new Point3D(100, -300, 100),
            UpDirection = new Vector3D(0, -0.5, 1),
            FarPlaneDistance = 5000,
            NearPlaneDistance = 0.1f,
        };
        public HelixToolkit.Wpf.SharpDX.PerspectiveCamera Gcode3dCamera
        {
            get => _gcode3dCamera;
            set
            {
                if (_gcode3dCamera == value)
                {
                    return;
                }
                _gcode3dCamera = value;
                OnPropertyChanged();
            }
        }


        LineGeometryModel3D _gcodeLayerGeometry;
        public LineGeometryModel3D GcodeLayerGeometry
        {
            get => _gcodeLayerGeometry;
            set
            {
                if (_gcodeLayerGeometry == value)
                {
                    return;
                }

                _gcodeLayerGeometry = value;
                OnPropertyChanged();
            }

        }

        ObservableElement3DCollection _gcode3dLayerGeometry;
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

        MeshGeometryModel3D _model;
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

        ObservableCollection<MeshGeometryModel3D> _models = new();
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

        Color _GcodeColor = Colors.Orange;
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

        int _GcodeThickness = 4;
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

        int _GcodeLayer = 0;
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

        int _GcodeMaxLayer = 0;
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

        #endregion

        #endregion

        #region Settings 
        bool _showCameraInfo = false;
        public bool ShowCameraInfo
        {
            get => _showCameraInfo;
            set
            {
                if (_showCameraInfo == value) return;
                if(!IsLoading)
                    SettingsManager.Current.Helix_ShowCameraInfo = value;
                _showCameraInfo = value;
                OnPropertyChanged();
            }
        }

        bool _viewerRotateAroundMouseDownPoint = true;
        public bool ViewerRotateAroundMouseDownPoint
        {
            get => _viewerRotateAroundMouseDownPoint;
            set
            {
                if (_viewerRotateAroundMouseDownPoint == value)
                    return;
                if (!IsLoading)
                    SettingsManager.Current.Helix_RotateAroundMouseDownPoint = value;
                _viewerRotateAroundMouseDownPoint = value;
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

        #region Constructor
        public DashboardGcodeViewerViewModel(IDialogCoordinator dialog, DashboardTabContentType TabName)
        {
            TabContentType = TabName;

            _ = dialog;
            RegisterMessages();

            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            IsLicenseValid = false;

            SharedCalculatorInstance.OnGcodesChanged += SharedCalculatorInstance_OnGcodesChanged;
            SharedCalculatorInstance.OnSelectedGcodeChanged += SharedCalculatorInstance_OnSelectedGcodeChanged;

            Gcode = SharedCalculatorInstance.Gcode;
            Gcodes = new ObservableCollection<Gcode>(SharedCalculatorInstance.Gcodes);
            Gcodes.CollectionChanged += Gcodes_CollectionChanged;

            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }

        void LoadSettings()
        {
            ShowCameraInfo = SettingsManager.Current.Helix_ShowCameraInfo;
            ViewerRotateAroundMouseDownPoint = SettingsManager.Current.Helix_RotateAroundMouseDownPoint;

            ProfileWidth = ExpandProfileView ? 
                new GridLength(SettingsManager.Current.GcodeInfo_ProfileWidth) : 
                new GridLength(GlobalStaticConfiguration.GcodeInfo_WidthCollapsed);
            _tempProfileWidth = SettingsManager.Current.GcodeInfo_ProfileWidth;

            ExpandProfileView = SettingsManager.Current.GcodeInfo_ExpandView;
        }
        void RegisterMessages()
        {
            Messenger.Default.Register<SettingsChangedEventArgs>(this, SettingsChangedAction);
            Messenger.Default.Register<GcodesChangedMessage>(this, GcodesChangedActionAsync);
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
        async void GcodesChangedActionAsync(GcodesChangedMessage msg)
        {
            if (msg != null)
            {
                if (msg.Gcode == null) return;
                switch (msg.Action)
                {
                    case MessagingAction.SetSelected:
                    case MessagingAction.Add:
                        Gcode = msg.Gcode;
                        await CreateGcodeModelAction(Gcode);
                        break;
                    case MessagingAction.ClearHelixView:
                    case MessagingAction.Remove:
                        Gcode = null;
                        GcodeLayerGeometry = new LineGeometryModel3D();
                        Gcode3dLayerGeometry = new ObservableElement3DCollection();
                        break;
                    case MessagingAction.Invalidate:
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region Events

        void OnProgressLayerModelUpdateAction(int progress)
        {
            ProgressLayerModel = progress;
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
                            foreach (var gc in e.NewItems)
                            {
                                Gcode newItem = (Gcode)gc;
                                if (newItem == null)
                                    continue;
                                SharedCalculatorInstance.AddGcode(newItem);
                            }

                            break;
                        case NotifyCollectionChangedAction.Remove:
                            foreach (var gc in e.OldItems)
                            {
                                Gcode newItem = (Gcode)gc;
                                if (newItem == null)
                                    continue;
                                SharedCalculatorInstance.RemoveGcode(newItem);
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

        #region SharedInstance
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
        #endregion

        #endregion

        #region ICommand & Actions

        #region General
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
        #endregion

        #region Helix3dView
        public ICommand ZoomToFitCommand
        {
            get => new RelayCommand((p) => ZoomToFitAction(p));
        }
        void ZoomToFitAction(object p)
        {
            try
            {
                if (p is Viewport3DX v)
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
            get => new RelayCommand((p) => ChangeCameraGridAction(p));
        }
        void ChangeCameraGridAction(object p)
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
        public ICommand Create2dGcodeLayerModelCommand
        {
            get => new RelayCommand(async (p) => await Create2dGcodeLayerModelAction());
        }
        async Task Create2dGcodeLayerModelAction()
        {
            try
            {
                if (Gcode == null || Gcode.LayerModelGenerated)
                    return;
                var prog = new Progress<int>(percent => OnProgressLayerModelUpdateAction(percent));
                var modelLayer = await GcodeModelBuilder.Instance.Create2dGcodeLayerModelListAsync(Gcode, prog);

                //Application.Current.Dispatcher.Invoke(() =>
                viewGcode2d.Dispatcher.Invoke(() =>
                {
                    Gcode.ModelLayers = modelLayer;
                    GcodeMaxLayer = Gcode.ModelLayers.Count - 1;
                    GcodeLayer = 0;
                    GcodeLayer = 1;
                });
                //Messenger.Default.Send(new NotificationMessage(SelectedGcodeFiles, "ZoomToFitGcode"));
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
        async Task Create3dGcodeLayerModelAction()
        {
            try
            {
                if (Gcode == null) return;
                if (!Gcode.LayerModelGenerated)
                {
                    var prog = new Progress<int>(percent => OnProgressLayerModelUpdateAction(percent));
                    var model3d = await GcodeModelBuilder.Instance.BuildGcodeLayerModelListAsync(Gcode, prog);
                    //Application.Current.Dispatcher.Invoke(() =>
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Gcode.Model3D = model3d;
                        Gcode.LayerModelGenerated = true;
                    });
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand CreateGcodeModelCommand
        {
            get => new RelayCommand(async (p) => await CreateGcodeModelAction(p));
        }
        async Task CreateGcodeModelAction(object parameter)
        {
            try
            {
                if (parameter is Gcode gcode)
                {
                    IsWorking = true;
                    // Created layer model if needed
                    if (gcode.Model3D.Count == 0 && !gcode.LayerModelGenerated)
                    {
                        await Create3dGcodeLayerModelAction();
                    }
                    GcodeMaxLayer = Gcode.Model3D.Count - 1;
                    GcodeLayer = 0;
                    GcodeLayer = 1;
                    GcodeColor = (Color)Application.Current.Resources["MahApps.Colors.Accent"];
                    // 2D
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LineBuilder lineBuilder = gcode.GetGcodeLayerLineBuilder(GcodeLayer);
                        LineGeometryModel3D layerGeometry = new()
                        {
                            Color = GcodeColor,
                            Geometry = lineBuilder.ToLineGeometry3D(),
                            Transform = new TranslateTransform3D(new Vector3D(0, 0, 0))
                        };
                        GcodeLayerGeometry = layerGeometry;
                    });
                    // 3D
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ObservableElement3DCollection collection = new();
                        foreach (LineBuilder builder in gcode.Model3D)
                        {
                            LineGeometryModel3D layer = new();
                            layer.Color = GcodeColor;
                            layer.Geometry = builder.ToLineGeometry3D();
                            layer.Transform = new TranslateTransform3D(new Vector3D(0, 0, layer.Bounds.Depth));
                            collection.Add(layer);
                        }
                        Gcode3dLayerGeometry = collection;
                    });
                    Messenger.Default.Send(new NotificationMessage(SelectedGcodeFiles, "ZoomToFitGcode"));
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProgressLayerModel = 0;
                        GcodeLayerGeometry = new LineGeometryModel3D();
                        Gcode3dLayerGeometry = new ObservableElement3DCollection();
                        GcodeLayer = 0;
                        GcodeMaxLayer = 0;
                    });
                }
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
            }
            if(viewGcode2d != null)
            {
                viewGcode2d.ZoomExtents();
            }
            if (viewGcode3d != null)
            {
                viewGcode3d.ZoomExtents();
            }
            IsWorking = false;
        }

        public ICommand SelectedGcodeChangedCommand
        {
            get => new RelayCommand((p) => SelectedGcodeChangedAction(p));
        }
        void SelectedGcodeChangedAction(object p)
        {
            try
            {
                // Replaced with CreateGcodeModelAction()
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand SelectedLayerChangedCommand
        {
            get => new RelayCommand((p) => SelectedLayerChangedAction());
        }
        void SelectedLayerChangedAction()
        {
            try
            {
                if (Gcode == null)
                    return;

                // 2D
                LineBuilder lineBuilder = Gcode.GetGcodeLayerLineBuilder(GcodeLayer);
                GcodeColor = (Color)Application.Current.Resources["MahApps.Colors.Accent"];
                LineGeometryModel3D layerGeometry = new();
                layerGeometry.Color = GcodeColor;
                layerGeometry.Geometry = lineBuilder.ToLineGeometry3D();
                layerGeometry.Transform = new TranslateTransform3D(new Vector3D(0, 0, 0));
                

                GcodeLayerGeometry = layerGeometry;

                //Messenger.Default.Send(new NotificationMessage(SelectedGcodeFiles, "ZoomToFitGcode"));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
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
                    _ = Gcodes.Remove(gcode);
                    logger.Info(string.Format(Strings.EventDeletedItemFormated, gcode.FileName));
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }

        }
        #endregion

        #endregion

        #region Methods

        #region Expander
        void ResizeProfile(bool dueToChangedSize)
        {
            _canProfileWidthChange = false;

            if (dueToChangedSize)
            {
                ExpandProfileView = Math.Abs(ProfileWidth.Value - GlobalStaticConfiguration.GcodeInfo_WidthCollapsed) > GlobalStaticConfiguration.FloatPointFix;
            }
            else
            {
                if (ExpandProfileView)
                {
                    ProfileWidth = Math.Abs(_tempProfileWidth - GlobalStaticConfiguration.GcodeInfo_WidthCollapsed) < GlobalStaticConfiguration.FloatPointFix ?
                        new GridLength(GlobalStaticConfiguration.GcodeInfo_DefaultWidthExpanded) :
                        new GridLength(_tempProfileWidth);
                }
                else
                {
                    _tempProfileWidth = ProfileWidth.Value;
                    ProfileWidth = new GridLength(GlobalStaticConfiguration.GcodeInfo_WidthCollapsed);
                }
            }

            _canProfileWidthChange = true;
        }
        #endregion

        #region Gcode
        
        #endregion

        public void OnViewVisible()
        {
            try
            {
                OnPropertyChanged(nameof(IsLicenseValid));

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
