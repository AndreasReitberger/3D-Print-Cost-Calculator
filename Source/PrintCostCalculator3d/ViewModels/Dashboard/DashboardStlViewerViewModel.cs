using GalaSoft.MvvmLight.Messaging;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Animations;
using HelixToolkit.Wpf.SharpDX.Assimp;
using HelixToolkit.Wpf.SharpDX.Controls;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using PrintCostCalculator3d.Enums;
using PrintCostCalculator3d.Models;
using PrintCostCalculator3d.Models.Events;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using PrintCostCalculator3d.ViewModels.Helix;
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
    public class DashboardStlViewerViewModel : ViewModelBase
    {
        #region Variables
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
                    SettingsManager.Current.GcodeViewer_ExpandProfileView = value;

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

                if (!IsLoading && Math.Abs(value.Value - GlobalStaticConfiguration.GcodeInfo_WidthCollapsed) > GlobalStaticConfiguration.FloatPointFix) // Do not save the size when collapsed
                    SettingsManager.Current.GcodeViewer_ProfileWidth = value.Value;

                _profileWidth = value;

                if (_canProfileWidthChange)
                    ResizeProfile(true);

                OnPropertyChanged();
            }
        }

        #endregion

        #region 3dModel
        HelixToolkitScene scene;
        public SceneNodeGroupModel3D GroupModel { get; } = new SceneNodeGroupModel3D();
        public ObservableCollection<Animation> Animations { get; } = new ObservableCollection<Animation>();

        //NodeAnimationUpdater animationUpdater;
        readonly CompositionTargetEx compositeHelper = new();

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

        EffectsManager _effectManager = GlobalStaticConfiguration.DefaulftEffectManager;
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

        HelixToolkit.Wpf.SharpDX.PerspectiveCamera _stl3dCamera = new()
        {
            LookDirection = new Vector3D(0, -10, -10),
            Position = new Point3D(0, 125, 125),
            UpDirection = new Vector3D(0, 1, 0),
            FarPlaneDistance = 5000,
            NearPlaneDistance = 0.1f,
            
        };
        public HelixToolkit.Wpf.SharpDX.PerspectiveCamera Stl3dCamera
        {
            get => _stl3dCamera;
            set
            {
                if (_stl3dCamera == value)
                    return;
                _stl3dCamera = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Stl

        bool _isLoadingStl = false;
        public bool IsLoadingStl
        {
            get => _isLoadingStl;
            set
            {
                if (_isLoadingStl == value)
                    return;
                _isLoadingStl = value;
                OnPropertyChanged();
            }
        }

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
                if (_stlFile != null)
                {
                    if (!IsLoadingStl)
                    {
                        string path = _stlFile.StlFilePath;
                        Read3dFile(path);
                    }
                }
                else
                    Clear3dViewer();

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
                if (Equals(value, _selectedStlFiles))
                    return;
                _selectedStlFiles = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion

        #region Settings 
        bool _showCameraInfo = false;
        public bool ShowCameraInfo
        {
            get => _showCameraInfo;
            set
            {
                if (_showCameraInfo == value)
                    return;
                if (!IsLoading)
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
        public DashboardStlViewerViewModel(IDialogCoordinator dialog, DashboardTabContentType TabName)
        {
            TabContentType = TabName;

            _ = dialog;
            RegisterMessages();
            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            IsLicenseValid = false;

            SharedCalculatorInstance.OnStlsChanged += SharedCalculatorInstance_OnStlsChanged;
            SharedCalculatorInstance.OnSelectedStlChanged += SharedCalculatorInstance_OnSelectedStlChanged;

            StlFile = SharedCalculatorInstance.StlFile;
            StlFiles.CollectionChanged += StlFiles_CollectionChanged;

            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }

        void LoadSettings()
        {
            ShowCameraInfo = SettingsManager.Current.Helix_ShowCameraInfo;
            ViewerRotateAroundMouseDownPoint = SettingsManager.Current.Helix_RotateAroundMouseDownPoint;

            ExpandProfileView = SettingsManager.Current.GcodeViewer_ExpandProfileView;

            ProfileWidth = ExpandProfileView ? new GridLength(SettingsManager.Current.GcodeViewer_ProfileWidth) : new GridLength(GlobalStaticConfiguration.GcodeInfo_WidthCollapsed);
            _tempProfileWidth = SettingsManager.Current.GcodeViewer_ProfileWidth;
        }
        void RegisterMessages()
        {
            Messenger.Default.Register<Stl>(this, StlReceivedAsync);
            Messenger.Default.Register<SettingsChangedEventArgs>(this, SettingsChangedAction);
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

        void StlReceivedAsync(Stl msg)
        {
            if (msg != null)
            {
                StlFile = msg;
                Read3dFile(StlFile.StlFilePath);
            }
        }
        #endregion

        #region Events
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
                            foreach (var item in e.NewItems)
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
        void SharedCalculatorInstance_OnSelectedStlChanged(object sender, Models.Events.StlChangedEventArgs e)
        {
            if (e != null)
            {
                StlFile = e.NewStl;
            }
        }
        void SharedCalculatorInstance_OnStlsChanged(object sender, Models.Events.StlsChangedEventArgs e)
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

        #region ICommand & Actions

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
        #endregion

        #endregion

        #region Methods

        #region Expander
        void ResizeProfile(bool dueToChangedSize)
        {
            _canProfileWidthChange = false;

            if (dueToChangedSize)
            {
                ExpandProfileView = Math.Abs(ProfileWidth.Value - GlobalStaticConfiguration.RepetierServerPro_Panel_WidthCollapsed) > GlobalStaticConfiguration.FloatPointFix;
            }
            else
            {
                if (ExpandProfileView)
                {
                    ProfileWidth = Math.Abs(_tempProfileWidth - GlobalStaticConfiguration.RepetierServerPro_Panel_WidthCollapsed) < GlobalStaticConfiguration.FloatPointFix ?
                        new GridLength(GlobalStaticConfiguration.RepetierServerPro_Panel_DefaultWidthExpanded) :
                        new GridLength(_tempProfileWidth);
                }
                else
                {
                    _tempProfileWidth = ProfileWidth.Value;
                    ProfileWidth = new GridLength(GlobalStaticConfiguration.RepetierServerPro_Panel_WidthCollapsed);
                }
            }

            _canProfileWidthChange = true;
        }
        #endregion

        #region Helix

        public void StartAnimation()
        {
            compositeHelper.Rendering += CompositeHelper_Rendering;
        }

        public void StopAnimation()
        {
            compositeHelper.Rendering -= CompositeHelper_Rendering;
        }

        void CompositeHelper_Rendering(object sender, RenderingEventArgs e)
        {
            /*
            if (animationUpdater != null)
            {
                animationUpdater.Update(Stopwatch.GetTimestamp(), Stopwatch.Frequency);
            }
            */
        }

        /**/
        void Read3dFile(string path)
        {
            if (path == null || IsLoadingStl)
            {
                return;
            }
            StopAnimation();

            IsLoadingStl = true;
            logger.Info($"Start loading stl file for 3d viewer: {path}");
            _ = Task.Run(() =>
              {
                  Importer loader = new();
                  loader.AssimpExceptionOccurred += Loader_AssimpExceptionOccurred;
                  HelixToolkitScene res = loader.Load(path);
                  logger.Info($"Result from stl file: {res?.Root?.Name}");
                  return res;
              }).ContinueWith((result) =>
              {
                  IsLoadingStl = false;
                  logger.Info($"Loading stl file done, processing result now");
                  if (result.IsCompleted)
                  {
                      if (result.IsCompleted)
                      {
                          logger.Info($"Stl result completed, creating 3d scene: completed? => {result.IsCompleted}");
                          scene = result.Result;
                          Animations.Clear();
                          GroupModel.Clear();
                          if (scene != null)
                          {
                              if (scene.Root != null)
                              {
                                  foreach (SceneNode node in scene.Root.Traverse())
                                  {
                                      if (node is MaterialGeometryNode m)
                                      {
                                          if (m.Material is PBRMaterialCore pbr)
                                          {
                                              pbr.RenderEnvironmentMap = RenderEnvironmentMap;
                                          }
                                          else if (m.Material is PhongMaterialCore phong)
                                          {
                                              phong.RenderEnvironmentMap = RenderEnvironmentMap;
                                          }
                                      }
                                  }

                                  _ = GroupModel.AddNode(scene.Root);
                                  if (scene.HasAnimation)
                                  {
                                      foreach (Animation ani in scene.Animations)
                                      {
                                          Animations.Add(ani);
                                      }
                                  }
                                  foreach (SceneNode n in scene.Root.Traverse())
                                  {
                                      n.Tag = new AttachedNodeViewModel(n);
                                  }
                                  logger.Info("Stl loading completed successfully!");
                              }
                              else
                              {
                                  logger.Info("The root for the scene was null");
                              }
                          }
                          else
                          {
                              logger.Error($"Loading stl result was null");
                          }
                      }
                  }
                  else if (result.IsFaulted && result.Exception != null)
                  {
                      logger.Error($"Loading stl was faulty: {JsonConvert.SerializeObject(result.Exception)}");
                      MessageBox.Show(result.Exception?.Message);
                  }

              }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        void Loader_AssimpExceptionOccurred(object sender, Exception e)
        {
            logger.Error(e.Message, e);
            //throw new NotImplementedException();
        }

        void Clear3dViewer()
        {
            try
            {
                if (scene != null)
                {
                    Animations.Clear();
                    GroupModel.Clear();
                }
            }
            catch(Exception exc)
            {
                logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
            }
        }
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
