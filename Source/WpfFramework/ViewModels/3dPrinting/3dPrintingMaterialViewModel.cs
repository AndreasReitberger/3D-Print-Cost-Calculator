using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Additional
using WpfFramework.Utilities;
using WpfFramework.Models._3dprinting;
using WpfFramework.Models.Settings;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using System.Windows.Input;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;
using System.Windows.Data;
using System.Collections;
using WpfFramework.Resources.Localization;
using log4net;
using System.Windows;
using System.Windows.Media;

namespace WpfFramework.ViewModels._3dPrinting
{
    public class _3dPrintingMaterialViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion
        
        #region Properties

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
        public ObservableCollection<_3dPrinterMaterial> Materials
        {
            get => SettingsManager.Current._3dPrinterMaterials;
            set
            {
                if(value != SettingsManager.Current._3dPrinterMaterials)
                {
                    SettingsManager.Current._3dPrinterMaterials = value;
                    OnPropertyChanged(nameof(Materials));
                }
            }
                
        }
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
                        return m.Name.Contains(_searchMaterial);
                    };

                    OnPropertyChanged(nameof(SearchMaterial));
                }
            }
        }
        #endregion

        #region Constructor
        public _3dPrintingMaterialViewModel(IDialogCoordinator dialog)
        {
            this._dialogCoordinator = dialog;
            Materials.CollectionChanged += Materials_CollectionChanged;

            createMaterialViewInfos();
            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
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

        #region Methods
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
                        Group = (MaterialViewManager.Group)Enum.Parse(typeof(MaterialViewManager.Group), p.TypeOfMaterial.Kind.ToString()),
                    })).ToList()
                }.View;
                MaterialViews.SortDescriptions.Add(new SortDescription(nameof(MaterialViewManager.Group), ListSortDirection.Ascending));
                MaterialViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(MaterialViewManager.Group)));
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
            get { return new RelayCommand(p => AddNewMaterialAction()); }
        }
        private async void AddNewMaterialAction()
        {
            try
            {

                var _dialog = new CustomDialog() { Title = Strings.NewMaterial };
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
            get { return new RelayCommand(p => AddNewMaterialChildWindowAction()); }
        }
        private async void AddNewMaterialChildWindowAction()
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
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (Color)Application.Current.Resources["Gray2"] },
                    Padding = new Thickness(50),
                };
                var newMaterialViewModel = new NewMaterialViewModel(async instance =>
                {
                    _childWindow.Close();
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
                        logger.Info(string.Format(Strings.EventAddedItemFormated, material.Name));
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
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand DeleteSelectedMaterialsCommand
        {
            get => new RelayCommand(p => DeleteSelectedMaterialsAction());
        }
        private async void DeleteSelectedMaterialsAction()
        {
            try
            {
                var res = await _dialogCoordinator.ShowMessageAsync(this,
                       Strings.DialogDeleteSelectedMaterialsHeadline, Strings.DialogDeleteSelectedMaterialsContent,
                       MessageDialogStyle.AffirmativeAndNegative
                       );
                if (res == MessageDialogResult.Affirmative)
                {
                    //var selectedMaterials = SelectedMaterialsView;
                    // change to for
                    IList collection = new ArrayList(SelectedMaterialsView);
                    for (int i = 0; i < collection.Count; i++)
                    {
                        var obj = collection[i] as MaterialViewInfo;
                        if (obj == null)
                            continue;
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, obj.Name));
                        Materials.Remove(obj.Material);
                    }
                    //foreach (MaterialViewInfo material in selectedMaterials)
                    /*
                    for (int i = 0; i < selectedMaterials.Count; i++)
                    {
                        MaterialViewInfo material = selectedMaterials[i] as MaterialViewInfo;
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, material.Name));
                        Materials.Remove(material.Material);
                    }*/
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
            get => new RelayCommand(p => EditSelectedMaterialAction());
        }
        private async void EditSelectedMaterialAction()
        {
            try
            {
                var selectedMaterial = SelectedMaterialView.Material;
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
            }
        }
        #endregion

        #region Methods
        public void OnViewVisible()
        {
            createMaterialViewInfos();
        }

        public void OnViewHide()
        {

        }
        #endregion
    }
}
