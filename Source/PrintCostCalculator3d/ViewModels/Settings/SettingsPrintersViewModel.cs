using AndreasReitberger.Models;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using MahApps.Metro.SimpleChildWindow;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using PrintCostCalculator3d.ViewModels._3dPrinting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace PrintCostCalculator3d.ViewModels
{
    class SettingsPrintersViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Properties

        #region Manufacturer
        ObservableCollection<Manufacturer> _manufacturers = new ObservableCollection<Manufacturer>();
        public ObservableCollection<Manufacturer> Manufacturers
        {
            get => _manufacturers;
            set
            {
                if (value == _manufacturers)
                    return;
                if (!IsLoading)
                    SettingsManager.Current.Manufacturers = value;

                _manufacturers = value;
                OnPropertyChanged();
            }
        }

        Manufacturer _SelectedManufacturer;
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

        IList _SelectedManufacturers = new ArrayList();
        public IList SelectedManufacturers
        {
            get => _SelectedManufacturers;
            set
            {
                if (value == _SelectedManufacturers)
                    return;

                if (!IsLoading)
                {

                }

                _SelectedManufacturers = value;
                OnPropertyChanged();
            }
        }

        public ICollectionView ManufacturerViews
        {
            get => _ManufacturerViews;
            set
            {
                if (_ManufacturerViews != value)
                {
                    _ManufacturerViews = value;
                    OnPropertyChanged();
                }
            }
        }
        ICollectionView _ManufacturerViews;

        #endregion

        #region Suppliers
        ObservableCollection<Supplier> _suppliers = new ObservableCollection<Supplier>();
        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set
            {
                if (value == _suppliers)
                    return;
                if (!IsLoading)
                    SettingsManager.Current.Suppliers = value;

                _suppliers = value;
                OnPropertyChanged();
            }
        }

        Supplier _SelectedSupplier;
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

        IList _SelectedSuppliers = new ArrayList();
        public IList SelectedSuppliers
        {
            get => _SelectedSuppliers;
            set
            {
                if (value == _SelectedSuppliers)
                    return;

                if (!IsLoading)
                {

                }

                _SelectedSuppliers = value;
                OnPropertyChanged();
            }
        }

        public ICollectionView SupplierViews
        {
            get => _SupplierViews;
            set
            {
                if (_SupplierViews != value)
                {
                    _SupplierViews = value;
                    OnPropertyChanged();
                }
            }
        }
        ICollectionView _SupplierViews;
        #endregion

        bool _restartRequired;
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
        string _searchManufacturer = string.Empty;
        public string SearchManufacturer
        {
            get => _searchManufacturer;
            set
            {
                if (_searchManufacturer != value)
                {
                    _searchManufacturer = value;

                    if (ManufacturerViews == null) return;
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

        string _searchSupplier = string.Empty;
        public string SearchSupplier
        {
            get => _searchSupplier;
            set
            {
                if (_searchSupplier != value)
                {
                    _searchSupplier = value;

                    if (SupplierViews == null) return;

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
            IsLoading = true;
            LoadSettings();
            IsLoading = false;
        }
        public SettingsPrintersViewModel(IDialogCoordinator instance)
        {
            _dialogCoordinator = instance;
            IsLoading = true;
            LoadSettings();
            IsLoading = false;
        }

        void LoadSettings()
        {
            Manufacturers = SettingsManager.Current.Manufacturers;
            CreateManufacturerViewInfos();

            Suppliers = SettingsManager.Current.Suppliers;
            CreateSupplierViewInfos();

            Manufacturers.CollectionChanged += Manufacturers_CollectionChanged;
            Suppliers.CollectionChanged += Suppliers_CollectionChanged;
        }

        void Suppliers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Suppliers));
            CreateSupplierViewInfos();
            SettingsManager.Save();
        }

        void Manufacturers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Manufacturers));
            CreateManufacturerViewInfos();
            SettingsManager.Save();
        }
        #endregion

        #region Methods
        void CreateManufacturerViewInfos()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Canvas c = new Canvas();
                c.Children.Add(new PackIconMaterial { Kind = PackIconMaterialKind.Factory });
                ManufacturerViews = new CollectionViewSource
                {
                    Source = Manufacturers.ToList()
                }.View;
                ManufacturerViews.SortDescriptions.Add(new SortDescription(nameof(Manufacturer.Name), ListSortDirection.Ascending));
                ManufacturerViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Manufacturer.isActive)));
            });
        }
        void CreateSupplierViewInfos()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Canvas c = new Canvas();
                c.Children.Add(new PackIconMaterial { Kind = PackIconMaterialKind.Store });
                SupplierViews = new CollectionViewSource
                {
                    Source = Suppliers.ToList()
                }.View;
                SupplierViews.SortDescriptions.Add(new SortDescription(nameof(Supplier.Name), ListSortDirection.Ascending));
                SupplierViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Supplier.isActive)));
            });
        }

        Manufacturer GetManufacturerFromInstance(NewManufacturerViewModel instance)
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
        
        Supplier GetSupplierFromInstance(NewSupplierViewModel instance)
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
        async void AddNewManufacturerAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.NewManufacturer };
                var newViewModel = new NewManufacturerViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Manufacturers.Add(GetManufacturerFromInstance(instance));
                    
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
            get { return new RelayCommand(async(p) => await AddNewManufacturerChildWindowAction()); }
        }
        async Task AddNewManufacturerChildWindowAction()
        {
            try
            {
                ChildWindow _childWindow = new()
                {
                    Title = Strings.NewManufacturer,
                    AllowMove = true,
                    ShowCloseButton = false,
                    CloseByEscape = false,
                    IsModal = true,
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (Color)Application.Current.Resources["Gray2"] },
                    Padding = new Thickness(50),
                };
                NewManufacturerViewModel newViewModel = new(instance =>
                {
                    _ = _childWindow.Close();
                    Manufacturers.Add(GetManufacturerFromInstance(instance));
                   
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Manufacturers[Manufacturers.Count - 1].Name));
                }, instance =>
                {
                    _childWindow.Close();
                },
                    _dialogCoordinator
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
                _ = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogExceptionHeadline,
                        string.Format(Strings.DialogExceptionFormatedContent, exc.Message)
                        );
            }
        }

        public ICommand DeleteSelectedManufacturersCommand
        {
            get => new RelayCommand(async(p) => await DeleteSelectedManufacturersAction());
        }
        async Task DeleteSelectedManufacturersAction()
        {
            try
            {
                MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
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
                        Manufacturer obj = collection[i] as Manufacturer;
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
            get => new RelayCommand(async(p) => await EditSelectedManufacturerAction());
        }
        async Task EditSelectedManufacturerAction()
        {
            try
            {
                Manufacturer selection = SelectedManufacturer;
                CustomDialog _dialog = new() { Title = Strings.EditManufacturer };
                NewManufacturerViewModel newViewModel = new (async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Manufacturers.Remove(selection);
                    Manufacturers.Add(GetManufacturerFromInstance(instance));

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
        async void DeleteManufacturerAction(object p)
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
        void DuplicateManufacturerAction(object p)
        {
            try
            {
                if (p is Manufacturer manufacturer)
                {
                    IEnumerable<Manufacturer> duplicates = Manufacturers.Where(mat => mat.Name.StartsWith(manufacturer.Name.Split('_')[0]));

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
            get => new RelayCommand(async(p) => await EditManufacturerAction(p));
        }
        async Task EditManufacturerAction(object material)
        {
            try
            {
                if (material is not Manufacturer selectedManufacturer)
                {
                    return;
                }
                CustomDialog _dialog = new() { Title = Strings.EditManufacturer };
                NewManufacturerViewModel newViewModel = new(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Manufacturers.Remove(selectedManufacturer);
                    Manufacturers.Add(GetManufacturerFromInstance(instance));

                    logger.Info(string.Format(Strings.EventEditedItemFormated, selectedManufacturer, Manufacturers[Manufacturers.Count - 1]));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                    _dialogCoordinator,
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
        async void AddNewSupplierAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.NewSupplier };
                var newViewModel = new NewSupplierViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Suppliers.Add(GetSupplierFromInstance(instance));

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
        async void AddNewSupplierChildWindowAction()
        {
            try
            {
                ChildWindow _childWindow = new()
                {
                    Title = Strings.NewSupplier,
                    AllowMove = true,
                    ShowCloseButton = false,
                    CloseByEscape = false,
                    IsModal = true,
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (Color)Application.Current.Resources["Gray2"] },
                    Padding = new Thickness(50),
                };
                NewSupplierViewModel newViewModel = new(instance =>
                {
                    _ = _childWindow.Close();
                    Suppliers.Add(GetSupplierFromInstance(instance));

                    logger.Info(string.Format(Strings.EventAddedItemFormated, Suppliers[Suppliers.Count - 1].Name));
                }, instance =>
                {
                    _childWindow.Close();
                },
                    _dialogCoordinator
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
                _ = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogExceptionHeadline,
                        string.Format(Strings.DialogExceptionFormatedContent, exc.Message)
                        );
            }
        }

        public ICommand DeleteSelectedSuppliersCommand
        {
            get => new RelayCommand(p => DeleteSelectedSuppliersAction());
        }
        async void DeleteSelectedSuppliersAction()
        {
            try
            {
                MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
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
                        Supplier obj = collection[i] as Supplier;
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
            get => new RelayCommand(async(p) => await EditSelectedSupplierAction());
        }
        async Task EditSelectedSupplierAction()
        {
            try
            {
                Supplier selection = SelectedSupplier;
                CustomDialog _dialog = new() { Title = Strings.EditSupplier };
                NewSupplierViewModel newViewModel = new(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Suppliers.Remove(selection);
                    Suppliers.Add(GetSupplierFromInstance(instance));

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
                _ = await _dialogCoordinator.ShowMessageAsync(this,
                    Strings.DialogExceptionHeadline,
                    string.Format(Strings.DialogExceptionFormatedContent, exc.Message)
                    );
            }
        }

        // Template commands
        public ICommand DeleteSupplierCommand
        {
            get => new RelayCommand(async(p) => await DeleteSupplierAction(p));
        }
        async Task DeleteSupplierAction(object p)
        {
            try
            {
                if (p is Supplier supplier)
                {
                    MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
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
        void DuplicateSupplierAction(object p)
        {
            try
            {
                if (p is Supplier supplier)
                {
                    IEnumerable<Supplier> duplicates = Suppliers.Where(mat => mat.Name.StartsWith(supplier.Name.Split('_')[0]));

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
        async void EditSupplierAction(object supplier)
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
                    Suppliers.Add(GetSupplierFromInstance(instance));

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

        void VisibleToHideApplicationAction()
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
