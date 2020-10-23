using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Additional
using PrintCostCalculator3d.Utilities;
using PrintCostCalculator3d.Models.Settings;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using System.Windows.Input;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;
using System.Windows.Data;
using System.Collections;
using PrintCostCalculator3d.Resources.Localization;
using log4net;
using System.Windows;
using System.Windows.Media;
using AndreasReitberger.Models;
using AndreasReitberger.Enums;
using PrintCostCalculator3d.Models;

namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    public class _3dPrintingMaterialViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool _isLoading;
        #endregion

        #region Properties
        public bool isLicenseValid
        {
            get => false;
        }

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

        private ObservableCollection<Material3d> _material = new ObservableCollection<Material3d>();
        public ObservableCollection<Material3d> Materials
        {
            get => _material;
            set
            {
                if (_material == value) return;
                if (!_isLoading)
                    SettingsManager.Current.PrinterMaterials = value;
                _material = value;
                OnPropertyChanged();
            }
        }
        /*
        public ObservableCollection<_3dPrinterMaterial> Materials
        {
            get => SettingsManager.Current._3dPrinterMaterials;
            set
            {
                if(value != SettingsManager.Current._3dPrinterMaterials)
                {
                    SettingsManager.Current._3dPrinterMaterials = value;
                    OnPropertyChanged();
                }
            }
                
        }
        */
        #endregion

        #region Search
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
                        if(patterns.Length == 1 || patterns.Length == 0)
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
        #endregion

        #region Constructor, LoadSettings
        public _3dPrintingMaterialViewModel(IDialogCoordinator dialog)
        {
            this._dialogCoordinator = dialog;

            _isLoading = true;
            LoadSettings();
            _isLoading = false;

            Materials.CollectionChanged += Materials_CollectionChanged;
            createMaterialViewInfos();

            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }

        private void LoadSettings()
        {
            Materials = SettingsManager.Current.PrinterMaterials;
        }

        private void Materials_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                createMaterialViewInfos();
                SettingsManager.Save();
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }

        }
        #endregion

        #region ICommand & Actions
        public ICommand AddNewMaterialCommand
        {
            get { return new RelayCommand(async (p) => await AddNewMaterialAction()); }
        }
        private async Task AddNewMaterialAction()
        {
            try
            {

                var _dialog = new CustomDialog() { Title = Strings.NewMaterial };
                var newMaterialViewModel = new NewMaterialViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Materials.Add(getMaterialFromInstance(instance));
                    
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Materials[Materials.Count - 1].Name));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                    this._dialogCoordinator
                );

                _dialog.Content = new Views._3dPrinting.NewMaterialDialog()
                {
                    DataContext = newMaterialViewModel
                };
                
                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogExceptionHeadline,
                        string.Format(Strings.DialogExceptionFormatedContent, exc.Message)
                        );
            }
        }
        
        public ICommand AddNewMaterialChildWindowCommand
        {
            get { return new RelayCommand(async (p) => await AddNewMaterialChildWindowAction()); }
        }
        private async Task AddNewMaterialChildWindowAction()
        {
            try
            {

                var _childWindow = new ChildWindow()
                {
                    Title = Strings.NewMaterial,
                    AllowMove = true,
                    ShowCloseButton = false,
                    CloseByEscape = false,
                    IsModal = true,
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (Color)Application.Current.Resources["MahApps.Colors.Gray2"] },
                    Padding = new Thickness(50),
                };
                var newMaterialViewModel = new NewMaterialViewModel(async instance =>
                {
                    _childWindow.Close();
                    Materials.Add(getMaterialFromInstance(instance));

                    logger.Info(string.Format(Strings.EventAddedItemFormated, Materials[Materials.Count - 1].Name));
                }, instance =>
                {
                    _childWindow.Close();
                },
                    this._dialogCoordinator
                );

                _childWindow.Content = new Views._3dPrinting.NewMaterialDialog()
                {
                    DataContext = newMaterialViewModel
                };
                await ChildWindowManager.ShowChildWindowAsync(Application.Current.MainWindow, _childWindow);
            }
            catch(Exception exc)
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
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, material.Name));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
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

        public ICommand ReorderMaterialCommand
        {
            get => new RelayCommand(async (p) => await ReorderMaterialAction(p));
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
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand DeleteSelectedMaterialsCommand
        {
            get => new RelayCommand(async (p) => await DeleteSelectedMaterialsAction());
        }
        private async Task DeleteSelectedMaterialsAction()
        {
            try
            {
                var res = await _dialogCoordinator.ShowMessageAsync(this,
                       Strings.DialogDeleteSelectedMaterialsHeadline, Strings.DialogDeleteSelectedMaterialsContent,
                       MessageDialogStyle.AffirmativeAndNegative
                       );
                if (res == MessageDialogResult.Affirmative)
                {

                    IList collection = new ArrayList(SelectedMaterialsView);
                    for (int i = 0; i < collection.Count; i++)
                    {
                        var obj = collection[i] as MaterialViewInfo;
                        if (obj == null)
                            continue;
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, obj.Name));
                        Materials.Remove(obj.Material);
                    }

                    OnPropertyChanged(nameof(Materials));
                }
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand EditSelectedMaterialCommand
        {
            get => new RelayCommand(async (p) => await EditSelectedMaterialAction());
        }
        private async Task EditSelectedMaterialAction()
        {
            try
            {
                var selectedMaterial = SelectedMaterialView.Material;
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
            }
        }
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
        private void createMaterialViewInfos()
        {
            try
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
                        Group = (Material3dTypes)Enum.Parse(typeof(Material3dTypes), p.MaterialFamily.ToString()),
                    })).ToList()
                }.View;
                MaterialViews.SortDescriptions.Add(new SortDescription(nameof(MaterialViewInfo.Group), ListSortDirection.Ascending));
                MaterialViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(MaterialViewInfo.Group)));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }

        }

        public void OnViewVisible()
        {
            createMaterialViewInfos();
            OnPropertyChanged(nameof(isLicenseValid));
        }

        public void OnViewHide()
        {

        }
        #endregion
    }
}
