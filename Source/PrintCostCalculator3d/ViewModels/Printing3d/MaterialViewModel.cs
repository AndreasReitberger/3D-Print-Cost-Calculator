using AndreasReitberger.Enums;
using AndreasReitberger.Models;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using MahApps.Metro.SimpleChildWindow;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
//Additional
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    public class MaterialViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Properties

        #region MaterialViews
        public ICollectionView MaterialViews
        {
            get => _MaterialViews;
            set
            {
                if (_MaterialViews != value)
                {
                    _MaterialViews = value;
                    OnPropertyChanged(nameof(MaterialViews));
                }
            }
        }
        ICollectionView _MaterialViews;

        MaterialViewInfo _selectedMaterialView;
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

        IList _selectedMaterialsView = new ArrayList();
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

        ObservableCollection<Material3d> _material = new();
        public ObservableCollection<Material3d> Materials
        {
            get => _material;
            set
            {
                if (_material == value)
                {
                    return;
                }
                if (!IsLoading)
                {
                    SettingsManager.Current.PrinterMaterials = value;
                }

                _material = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Search
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
        public MaterialViewModel(IDialogCoordinator dialog)
        {
            this._dialogCoordinator = dialog;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            IsLicenseValid = false;

            Materials.CollectionChanged += Materials_CollectionChanged;
            CreateMaterialViewInfos();

            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }

        void LoadSettings()
        {
            Materials = SettingsManager.Current.PrinterMaterials;
        }

        void Materials_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                CreateMaterialViewInfos();
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
        async Task AddNewMaterialAction()
        {
            try
            {

                var _dialog = new CustomDialog() { Title = Strings.NewMaterial };
                var newMaterialViewModel = new NewMaterialViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Materials.Add(InstanceConverter.GetMaterialFromInstance(instance));
                    
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
        async Task AddNewMaterialChildWindowAction()
        {
            try
            {

                ChildWindow _childWindow = new()
                {
                    Title = Strings.NewMaterial,
                    AllowMove = true,
                    ShowCloseButton = false,
                    CloseByEscape = false,
                    IsModal = true,
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (Color)Application.Current.Resources["MahApps.Colors.Gray2"] },
                    Padding = new Thickness(50),
                };
                NewMaterialViewModel newMaterialViewModel = new(instance =>
                {
                    _ = _childWindow.Close();
                    Materials.Add(InstanceConverter.GetMaterialFromInstance(instance));

                    logger.Info(string.Format(Strings.EventAddedItemFormated, Materials[Materials.Count - 1].Name));
                }, instance =>
                {
                    _ = _childWindow.Close();
                },
                    _dialogCoordinator
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
                    MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogDeleteMaterialHeadline, Strings.DialogDeleteMaterialContent,
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        _ = Materials.Remove(material);
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
            get => new RelayCommand((p) => DuplicateMaterialAction(p));
        }
        void DuplicateMaterialAction(object p)
        {
            try
            {
                if (p is Material3d material)
                {
                    IEnumerable<Material3d> duplicates = Materials.Where(mat => mat.Name.StartsWith(material.Name.Split('_')[0]));

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
                CustomDialog _dialog = new() { Title = Strings.EditMaterial };
                NewMaterialViewModel newMaterialViewModel = new (async instance =>
                {
                   await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                   UpdateMaterialFromInstance(instance, selectedMaterial);
                   logger.Info(string.Format(Strings.EventEditedItemFormated, selectedMaterial, Materials[Materials.Count - 1]));
                }, instance =>
                {
                   _ = _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
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
                    MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogOpenExternalUriHeadline,
                        string.Format(Strings.DialogOpenExternalUriFormatedContent, uri),
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        _ = Process.Start(uri);
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
        async Task DeleteSelectedMaterialsAction()
        {
            try
            {
                MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                       Strings.DialogDeleteSelectedMaterialsHeadline, Strings.DialogDeleteSelectedMaterialsContent,
                       MessageDialogStyle.AffirmativeAndNegative
                       );
                if (res == MessageDialogResult.Affirmative)
                {

                    IList collection = new ArrayList(SelectedMaterialsView);
                    for (int i = 0; i < collection.Count; i++)
                    {
                        if (collection[i] is not MaterialViewInfo obj)
                        {
                            continue;
                        }
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, obj.Name));
                        _ = Materials.Remove(obj.Material);
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
        async Task EditSelectedMaterialAction()
        {
            try
            {
                Material3d selectedMaterial = SelectedMaterialView.Material;
                CustomDialog _dialog = new() { Title = Strings.EditMaterial };
                NewMaterialViewModel newMaterialViewModel = new(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    UpdateMaterialFromInstance(instance, selectedMaterial);
                    logger.Info(string.Format(Strings.EventEditedItemFormated, selectedMaterial, Materials[Materials.Count - 1]));
                }, instance =>
                {
                    _ = _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
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
            }
        }
        #endregion

        #region Methods
        /*
        Material3d getMaterialFromInstance(NewMaterialViewModel instance, Material3d material = null)
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
                temp.FactorLToKg = instance.FactorLiterToKg;
                temp.TypeOfMaterial = instance.TypeOfMaterial;
                temp.MaterialFamily = instance.MaterialFamily;
                temp.Attributes = instance.Attributes.ToList();
                if(temp.MaterialFamily == Material3dFamily.Powder)
                {
                    temp.ProcedureAttributes.Add(
                        new AndreasReitberger.Models.MaterialAdditions.Material3dProcedureAttribute()
                        {
                            Family = instance.MaterialFamily,
                            //Procedure = Printer3dType.SLS,
                            Attribute = ProcedureAttribute.MaterialRefreshingRatio,
                            Value = instance.RefreshingRate,
                        });
                }
                    //ColorCode = 
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.DialogExceptionFormatedContent, exc.Message, exc.TargetSite);
            }
            return temp;
        }
        */
        void UpdateMaterialFromInstance(NewMaterialViewModel instance, Material3d material)
        {
            try
            {
                InstanceConverter.GetMaterialFromInstance(instance, material);

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
        void CreateMaterialViewInfos()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
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
                });
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }

        }

        public void OnViewVisible()
        {
            CreateMaterialViewInfos();
            OnPropertyChanged(nameof(IsLicenseValid));
        }

        public void OnViewHide()
        {

        }
        #endregion
    }
}
