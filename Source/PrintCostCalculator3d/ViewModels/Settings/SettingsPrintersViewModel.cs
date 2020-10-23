using log4net;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using MahApps.Metro.SimpleChildWindow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using PrintCostCalculator3d.ViewModels._3dPrinting;
using AndreasReitberger.Models;

namespace PrintCostCalculator3d.ViewModels
{
    class SettingsPrintersViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool _isLoading;
        #endregion

        #region Properties

        #region Manufacturer
        private ObservableCollection<Manufacturer> _manufacturers = new ObservableCollection<Manufacturer>();
        public ObservableCollection<Manufacturer> Manufacturers
        {
            get => _manufacturers;
            private set
            {
                if (value == _manufacturers)
                    return;
                if (!_isLoading)
                    SettingsManager.Current.Manufacturers = value;

                _manufacturers = value;
                OnPropertyChanged();
            }
        }

        private Manufacturer _SelectedManufacturer;
        public Manufacturer SelectedManufacturer
        {
            get => _SelectedManufacturer;
            set
            {
                if (value == _SelectedManufacturer)
                    return;

                _SelectedManufacturer = value;
                OnPropertyChanged();
            }
        }

        private IList _SelectedManufacturers = new ArrayList();
        public IList SelectedManufacturers
        {
            get => _SelectedManufacturers;
            set
            {
                if (value == _SelectedManufacturers)
                    return;

                if (!_isLoading)
                {

                }

                _SelectedManufacturers = value;
                OnPropertyChanged();
            }
        }

        public ICollectionView ManufacturerViews
        {
            get => _ManufacturerViews;
            private set
            {
                if (_ManufacturerViews != value)
                {
                    _ManufacturerViews = value;
                    OnPropertyChanged();
                }
            }
        }
        private ICollectionView _ManufacturerViews;

        #endregion

        #region Suppliers
        private ObservableCollection<Supplier> _suppliers = new ObservableCollection<Supplier>();
        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            private set
            {
                if (value == _suppliers)
                    return;
                if (!_isLoading)
                    SettingsManager.Current.Suppliers = value;

                _suppliers = value;
                OnPropertyChanged();
            }
        }

        private Supplier _SelectedSupplier;
        public Supplier SelectedSupplier
        {
            get => _SelectedSupplier;
            set
            {
                if (value == _SelectedSupplier)
                    return;

                _SelectedSupplier = value;
                OnPropertyChanged();
            }
        }

        private IList _SelectedSuppliers = new ArrayList();
        public IList SelectedSuppliers
        {
            get => _SelectedSuppliers;
            set
            {
                if (value == _SelectedSuppliers)
                    return;

                if (!_isLoading)
                {

                }

                _SelectedSuppliers = value;
                OnPropertyChanged();
            }
        }

        public ICollectionView SupplierViews
        {
            get => _SupplierViews;
            private set
            {
                if (_SupplierViews != value)
                {
                    _SupplierViews = value;
                    OnPropertyChanged();
                }
            }
        }
        private ICollectionView _SupplierViews;
        #endregion

        private bool _restartRequired;
        public bool RestartRequired
        {
            get => _restartRequired;
            set
            {
                if (value == _restartRequired)
                    return;

                _restartRequired = value;
                OnPropertyChanged();
            }
        }
        #endregion   

        #region Search
        private string _searchManufacturer = string.Empty;
        public string SearchManufacturer
        {
            get => _searchManufacturer;
            set
            {
                if (_searchManufacturer != value)
                {
                    _searchManufacturer = value;

                    ManufacturerViews.Refresh();

                    ICollectionView view = CollectionViewSource.GetDefaultView(ManufacturerViews);
                    IEqualityComparer<String> comparer = StringComparer.InvariantCultureIgnoreCase;
                    view.Filter = o =>
                    {
                        Manufacturer p = o as Manufacturer;
                        string[] patterns = _searchManufacturer.ToLower().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (patterns.Length == 1 || patterns.Length == 0)
                            return p.Name.ToLower().Contains(_searchManufacturer.ToLower());
                        else
                        {
                            return patterns.Any(p.Name.ToLower().Contains) || patterns.Any(p.DebitorNumber.ToLower().Contains);
                        }
                    };
                    OnPropertyChanged();
                }
            }
        }

        private string _searchSupplier = string.Empty;
        public string SearchSupplier
        {
            get => _searchSupplier;
            set
            {
                if (_searchSupplier != value)
                {
                    _searchSupplier = value;

                    SupplierViews.Refresh();

                    ICollectionView view = CollectionViewSource.GetDefaultView(SupplierViews);
                    IEqualityComparer<String> comparer = StringComparer.InvariantCultureIgnoreCase;
                    view.Filter = o =>
                    {
                        Supplier p = o as Supplier;
                        string[] patterns = _searchSupplier.ToLower().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (patterns.Length == 1 || patterns.Length == 0)
                            return p.Name.ToLower().Contains(_searchSupplier.ToLower());
                        else
                        {
                            return patterns.Any(p.Name.ToLower().Contains) || patterns.Any(p.DebitorNumber.ToLower().Contains);
                        }
                    };
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Constructor, LoadSettings
        public SettingsPrintersViewModel()
        {
            _isLoading = true;

            LoadSettings();

            _isLoading = false;
        }
        public SettingsPrintersViewModel(IDialogCoordinator instance)
        {
            _dialogCoordinator = instance;
            _isLoading = true;

            LoadSettings();

            _isLoading = false;
        }

        private void LoadSettings()
        {
            Manufacturers = SettingsManager.Current.Manufacturers;
            createManufacturerViewInfos();

            Suppliers = SettingsManager.Current.Suppliers;
            createSupplierViewInfos();

            Manufacturers.CollectionChanged += Manufacturers_CollectionChanged;
            Suppliers.CollectionChanged += Suppliers_CollectionChanged;
        }

        private void Suppliers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Suppliers));
            createSupplierViewInfos();
            SettingsManager.Save();
        }

        private void Manufacturers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Manufacturers));
            createManufacturerViewInfos();
            SettingsManager.Save();
        }
        #endregion

        #region Methods
        private void createManufacturerViewInfos()
        {
            Canvas c = new Canvas();
            c.Children.Add(new PackIconMaterial { Kind = PackIconMaterialKind.Factory });
            ManufacturerViews = new CollectionViewSource
            {
                Source = Manufacturers.ToList()
            }.View;
            ManufacturerViews.SortDescriptions.Add(new SortDescription(nameof(Manufacturer.Name), ListSortDirection.Ascending));
            ManufacturerViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Manufacturer.isActive)));
        }
        private void createSupplierViewInfos()
        {
            Canvas c = new Canvas();
            c.Children.Add(new PackIconMaterial { Kind = PackIconMaterialKind.Store });
            SupplierViews = new CollectionViewSource
            {
                Source = Suppliers.ToList()
            }.View;
            SupplierViews.SortDescriptions.Add(new SortDescription(nameof(Supplier.Name), ListSortDirection.Ascending));
            SupplierViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Supplier.isActive)));
        }

        private Manufacturer getManufacturerFromInstance(NewManufacturerViewModel instance)
        {
            Manufacturer temp = new Manufacturer();
            try
            {
                temp = new Manufacturer()
                {
                    Id = instance.Id,
                    Name = instance.Name,
                    DebitorNumber = instance.DebitorNumber,
                    isActive = instance.isActive,
                    Website = instance.ShopUri,
                };
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.DialogExceptionFormatedContent, exc.Message, exc.TargetSite);
            }
            return temp;
        }
        
        private Supplier getSupplierFromInstance(NewSupplierViewModel instance)
        {
            Supplier temp = new Supplier();
            try
            {
                temp = new Supplier()
                {
                    Id = instance.Id,
                    Name = instance.Name,
                    DebitorNumber = instance.DebitorNumber,
                    isActive = instance.isActive,
                    Website = instance.ShopUri,
                };
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.DialogExceptionFormatedContent, exc.Message, exc.TargetSite);
            }
            return temp;
        }
        #endregion

        #region ICommands & Actions

        #region Manufacturer
        public ICommand AddNewManufacturerCommand
        {
            get { return new RelayCommand(p => AddNewManufacturerAction()); }
        }
        private async void AddNewManufacturerAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.NewManufacturer };
                var newViewModel = new NewManufacturerViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Manufacturers.Add(getManufacturerFromInstance(instance));
                    
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Manufacturers[Manufacturers.Count - 1].Name));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                _dialogCoordinator
                );

                _dialog.Content = new Views._3dPrinting.NewManufacturerDialog()
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

        public ICommand AddNewManufacturerChildWindowCommand
        {
            get { return new RelayCommand(p => AddNewManufacturerChildWindowAction()); }
        }
        private async void AddNewManufacturerChildWindowAction()
        {
            try
            {

                var _childWindow = new ChildWindow()
                {
                    Title = Strings.NewManufacturer,
                    AllowMove = true,
                    ShowCloseButton = false,
                    CloseByEscape = false,
                    IsModal = true,
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (Color)Application.Current.Resources["Gray2"] },
                    Padding = new Thickness(50),
                };
                var newViewModel = new NewManufacturerViewModel(async instance =>
                {
                    _childWindow.Close();
                    Manufacturers.Add(getManufacturerFromInstance(instance));
                   
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Manufacturers[Manufacturers.Count - 1].Name));
                }, instance =>
                {
                    _childWindow.Close();
                },
                    this._dialogCoordinator
                );

                _childWindow.Content = new Views._3dPrinting.NewManufacturerDialog()
                {
                    DataContext = newViewModel
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

        public ICommand DeleteSelectedManufacturersCommand
        {
            get => new RelayCommand(p => DeleteSelectedManufacturersAction());
        }
        private async void DeleteSelectedManufacturersAction()
        {
            try
            {
                var res = await _dialogCoordinator.ShowMessageAsync(this,
                       string.Format(Strings.DialogDeleteSelectionFormatedHeadline, Strings.Manufacturers),
                       string.Format(Strings.DialogDeleteSelectionFormatedContent, Strings.Manufacturers),
                       MessageDialogStyle.AffirmativeAndNegative
                       );
                if (res == MessageDialogResult.Affirmative)
                {
                    //var selectedPrinters = SelectedPrintersView;
                    //foreach (PrinterViewInfo printer in selectedPrinters)
                    IList collection = new ArrayList(SelectedManufacturers);
                    for (int i = 0; i < collection.Count; i++)
                    {
                        var obj = collection[i] as Manufacturer;
                        if (obj == null)
                            continue;
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, obj.Name));
                        Manufacturers.Remove(obj);
                    }
  
                    OnPropertyChanged(nameof(Manufacturers));
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand EditSelectedManufacturerCommand
        {
            get => new RelayCommand(p => EditSelectedManufacturerAction());
        }
        private async void EditSelectedManufacturerAction()
        {
            try
            {
                var selection = SelectedManufacturer;
                var _dialog = new CustomDialog() { Title = Strings.EditManufacturer };
                var newViewModel = new NewManufacturerViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Manufacturers.Remove(selection);
                    Manufacturers.Add(getManufacturerFromInstance(instance));

                    logger.Info(string.Format(Strings.EventAddedItemFormated, selection, Manufacturers[Manufacturers.Count - 1].Name));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                _dialogCoordinator,
                selection
                );

                _dialog.Content = new Views._3dPrinting.NewManufacturerDialog()
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

        // Template commands
        public ICommand DeleteManufacturerCommand
        {
            get => new RelayCommand(p => DeleteManufacturerAction(p));
        }
        private async void DeleteManufacturerAction(object p)
        {
            try
            {
                Manufacturer manufacturer = p as Manufacturer;
                if (manufacturer != null)
                {
                    var res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogDeleteManufacturerHeadline, 
                        string.Format(Strings.DialogDeleteManufacturerFormatedContent, manufacturer.Name),
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        Manufacturers.Remove(manufacturer);
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, manufacturer.Name));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand DuplicateManufacturerCommand
        {
            get => new RelayCommand(p => DuplicateManufacturerAction(p));
        }
        private async void DuplicateManufacturerAction(object p)
        {
            try
            {
                Manufacturer manufacturer = p as Manufacturer;
                if (manufacturer != null)
                {
                    var duplicates = Manufacturers.Where(mat => mat.Name.StartsWith(manufacturer.Name.Split('_')[0]));

                    Manufacturer newManufacturer = (Manufacturer)manufacturer.Clone();
                    newManufacturer.Id = Guid.NewGuid();
                    newManufacturer.Name = string.Format("{0}_{1}", newManufacturer.Name.Split('_')[0], duplicates.Count());
                    Manufacturers.Add(newManufacturer);
                    ManufacturerViews.Refresh();

                    logger.Info(string.Format(Strings.EventAddedItemFormated, newManufacturer.Name));
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand EditManufacturerCommand
        {
            get => new RelayCommand(p => EditManufacturerAction(p));
        }
        private async void EditManufacturerAction(object material)
        {
            try
            {
                var selectedManufacturer = material as Manufacturer;
                if (selectedManufacturer == null)
                {
                    return;
                }
                var _dialog = new CustomDialog() { Title = Strings.EditManufacturer };
                var newViewModel = new NewManufacturerViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Manufacturers.Remove(selectedManufacturer);
                    Manufacturers.Add(getManufacturerFromInstance(instance));

                    logger.Info(string.Format(Strings.EventEditedItemFormated, selectedManufacturer, Manufacturers[Manufacturers.Count - 1]));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                    this._dialogCoordinator,
                    selectedManufacturer
                );

                _dialog.Content = new Views._3dPrinting.NewManufacturerDialog()
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

        #region Supplier
        public ICommand AddNewSupplierCommand
        {
            get { return new RelayCommand(p => AddNewSupplierAction()); }
        }
        private async void AddNewSupplierAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.NewSupplier };
                var newViewModel = new NewSupplierViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Suppliers.Add(getSupplierFromInstance(instance));

                    logger.Info(string.Format(Strings.EventAddedItemFormated, Suppliers[Suppliers.Count - 1].Name));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                _dialogCoordinator
                );

                _dialog.Content = new Views._3dPrinting.NewSupplierDialog()
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

        public ICommand AddNewSupplierChildWindowCommand
        {
            get { return new RelayCommand(p => AddNewSupplierChildWindowAction()); }
        }
        private async void AddNewSupplierChildWindowAction()
        {
            try
            {

                var _childWindow = new ChildWindow()
                {
                    Title = Strings.NewSupplier,
                    AllowMove = true,
                    ShowCloseButton = false,
                    CloseByEscape = false,
                    IsModal = true,
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (Color)Application.Current.Resources["Gray2"] },
                    Padding = new Thickness(50),
                };
                var newViewModel = new NewSupplierViewModel(async instance =>
                {
                    _childWindow.Close();
                    Suppliers.Add(getSupplierFromInstance(instance));

                    logger.Info(string.Format(Strings.EventAddedItemFormated, Suppliers[Suppliers.Count - 1].Name));
                }, instance =>
                {
                    _childWindow.Close();
                },
                    this._dialogCoordinator
                );

                _childWindow.Content = new Views._3dPrinting.NewSupplierDialog()
                {
                    DataContext = newViewModel
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

        public ICommand DeleteSelectedSuppliersCommand
        {
            get => new RelayCommand(p => DeleteSelectedSuppliersAction());
        }
        private async void DeleteSelectedSuppliersAction()
        {
            try
            {
                var res = await _dialogCoordinator.ShowMessageAsync(this,
                       string.Format(Strings.DialogDeleteSelectionFormatedHeadline, Strings.Suppliers),
                       string.Format(Strings.DialogDeleteSelectionFormatedContent, Strings.Suppliers),
                       MessageDialogStyle.AffirmativeAndNegative
                       );
                if (res == MessageDialogResult.Affirmative)
                {
                    //var selectedPrinters = SelectedPrintersView;
                    //foreach (PrinterViewInfo printer in selectedPrinters)
                    IList collection = new ArrayList(SelectedSuppliers);
                    for (int i = 0; i < collection.Count; i++)
                    {
                        var obj = collection[i] as Supplier;
                        if (obj == null)
                            continue;
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, obj.Name));
                        Suppliers.Remove(obj);
                    }

                    OnPropertyChanged(nameof(Suppliers));
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand EditSelectedSupplierCommand
        {
            get => new RelayCommand(p => EditSelectedSupplierAction());
        }
        private async void EditSelectedSupplierAction()
        {
            try
            {
                var selection = SelectedSupplier;
                var _dialog = new CustomDialog() { Title = Strings.EditSupplier };
                var newViewModel = new NewSupplierViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Suppliers.Remove(selection);
                    Suppliers.Add(getSupplierFromInstance(instance));

                    logger.Info(string.Format(Strings.EventAddedItemFormated, selection, Suppliers[Suppliers.Count - 1].Name));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                _dialogCoordinator,
                selection
                );

                _dialog.Content = new Views._3dPrinting.NewSupplierDialog()
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

        // Template commands
        public ICommand DeleteSupplierCommand
        {
            get => new RelayCommand(p => DeleteSupplierAction(p));
        }
        private async void DeleteSupplierAction(object p)
        {
            try
            {
                Supplier supplier = p as Supplier;
                if (supplier != null)
                {
                    var res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogDeleteSupplierHeadline, 
                        string.Format(Strings.DialogDeleteSupplierFormatedContent, supplier.Name),
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        Suppliers.Remove(supplier);
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, supplier.Name));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand DuplicateSupplierCommand
        {
            get => new RelayCommand(p => DuplicateSupplierAction(p));
        }
        private async void DuplicateSupplierAction(object p)
        {
            try
            {
                Supplier supplier = p as Supplier;
                if (supplier != null)
                {
                    var duplicates = Suppliers.Where(mat => mat.Name.StartsWith(supplier.Name.Split('_')[0]));

                    Supplier newSupplier = (Supplier)supplier.Clone();
                    newSupplier.Id = Guid.NewGuid();
                    newSupplier.Name = string.Format("{0}_{1}", newSupplier.Name.Split('_')[0], duplicates.Count());
                    Suppliers.Add(newSupplier);
                    SupplierViews.Refresh();

                    logger.Info(string.Format(Strings.EventAddedItemFormated, newSupplier.Name));
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand EditSupplierCommand
        {
            get => new RelayCommand(p => EditSupplierAction(p));
        }
        private async void EditSupplierAction(object supplier)
        {
            try
            {
                var selectedSupplier = supplier as Supplier;
                if (selectedSupplier == null)
                {
                    return;
                }
                var _dialog = new CustomDialog() { Title = Strings.EditSupplier };
                var newViewModel = new NewSupplierViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Suppliers.Remove(selectedSupplier);
                    Suppliers.Add(getSupplierFromInstance(instance));

                    logger.Info(string.Format(Strings.EventEditedItemFormated, selectedSupplier, Manufacturers[Manufacturers.Count - 1]));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                    this._dialogCoordinator,
                    selectedSupplier
                );

                _dialog.Content = new Views._3dPrinting.NewSupplierDialog()
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

        public ICommand VisibleToHideApplicationCommand
        {
            get { return new RelayCommand(p => VisibleToHideApplicationAction()); }
        }

        private void VisibleToHideApplicationAction()
        {

        }

        #endregion

        #region Methods
        public void OnViewVisible()
        {

        }

        public void OnViewHide()
        {

        }
        #endregion
    }
}
