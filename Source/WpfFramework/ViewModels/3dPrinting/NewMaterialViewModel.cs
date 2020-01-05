using WpfFramework.Models._3dprinting;
using WpfFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

//Additional
using WpfFramework.Resources.Localization;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using WpfFramework.Models.Settings;
using System.ComponentModel;
using System.Windows.Data;
using log4net;

namespace WpfFramework.ViewModels._3dPrinting
{
    class NewMaterialViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);        
        #endregion

        #region Properties
        private bool _isEdit;
        public bool IsEdit
        {
            get => _isEdit;
            set
            {
                if (value == _isEdit)
                    return;

                _isEdit = value;
                OnPropertyChanged();
            }
        }
        private Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if(_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string _sku;
        public string SKU
        {
            get => _sku;
            set
            {
                if (_sku != value)
                {
                    _sku = value;
                    OnPropertyChanged(nameof(SKU));
                }
            }
        }

        private decimal _price = 25;
        public decimal Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }
        private Unit _unit = Unit.kg;
        public Unit Unit
        {
            get => _unit;
            set
            {
                if(_unit != value)
                {
                    _unit = value;
                    OnPropertyChanged(nameof(Unit));
                }
            }
        }
        private decimal _size = 1;
        public decimal PackageSize
        {
            get => _size;
            set
            {
                if(_size != value)
                {
                    _size = value;
                    OnPropertyChanged(nameof(PackageSize));
                }
            }
        }

        private decimal _density = 1.27m;
        public decimal Density
        {
            get => _density;
            set
            {
                if(_density != value)
                {
                    _density = value;
                    OnPropertyChanged(nameof(Density));
                }
            }
        }

        private _3dPrinterMaterialTypes _typeOfMaterial;
        public _3dPrinterMaterialTypes TypeOfMaterial
        {
            get => _typeOfMaterial;
            set
            {
                if(_typeOfMaterial != value)
                {
                    _typeOfMaterial = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<_3dPrinterMaterialTypes> MaterialTypes
        {
            get => SettingsManager.Current._3dPrinterMaterialTypes;
        }
        public ICollectionView MaterialTypeViews
        {
            get => _materialTypeViews;
            private set
            {
                if (_materialTypeViews != value)
                {
                    _materialTypeViews = value;
                    OnPropertyChanged();
                }
            }
        }
        private ICollectionView _materialTypeViews;
        

        private _3dPrinterMaterialKind _kind = _3dPrinterMaterialKind.Filament;
        public _3dPrinterMaterialKind Kind
        {
            get => _kind;
            set
            {
                if (_kind != value)
                {
                    _kind = value;
                    OnPropertyChanged(nameof(Kind));
                }
            }
        }
        //

        private Manufacturer _manufacturer;
        public Manufacturer Manufacturer
        {
            get => _manufacturer;
            set
            {
                if(_manufacturer != value)
                {
                    _manufacturer = value;
                    OnPropertyChanged();
                }
            }
        }
        private Supplier _supplier;
        public Supplier Supplier
        {
            get => _supplier;
            set
            {
                if(_supplier != value)
                {
                    _supplier = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private string _linkShop;
        public string LinkToReorder
        {
            get => _linkShop;
            set
            {
                if(_linkShop != value)
                {
                    _linkShop = value;
                    OnPropertyChanged(nameof(LinkToReorder));
                }
            }
        }

        private int _temperatureNozzle = 230;
        public int TemperatureNozzle
        {
            get => _temperatureNozzle;
            set
            {
                if(_temperatureNozzle != value)
                {
                    _temperatureNozzle = value;
                    OnPropertyChanged(nameof(TemperatureNozzle));
                }
            }
        }

        private int _temperatureHeatbed = 80;
        public int TemperatureHeatbed
        {
            get => _temperatureHeatbed;
            set
            {
                if (_temperatureHeatbed != value)
                {
                    _temperatureHeatbed = value;
                    OnPropertyChanged(nameof(TemperatureHeatbed));
                }
            }
        }

        private string _colorCode;
        public string ColorCode
        {
            get => _colorCode;
            set
            {
                if (_colorCode != value)
                {
                    _colorCode = value;
                    OnPropertyChanged(nameof(ColorCode));
                }
            }
        }
        #endregion

        #region Settings
        public ObservableCollection<Manufacturer> Manufacturers
        {
            get => SettingsManager.Current.Manufacturers;
            set
            {
                if (value != SettingsManager.Current.Manufacturers)
                {
                    SettingsManager.Current.Manufacturers = value;
                    OnPropertyChanged();
                }
            }

        }
        public ObservableCollection<Supplier> Suppliers
        {
            get => SettingsManager.Current.Suppliers;
            set
            {
                if (value != SettingsManager.Current.Suppliers)
                {
                    SettingsManager.Current.Suppliers = value;
                    OnPropertyChanged();
                }
            }

        }
        #endregion

        #region Constructor
        public NewMaterialViewModel(Action<NewMaterialViewModel> saveCommand, Action<NewMaterialViewModel> cancelHandler, _3dPrinterMaterial material = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            Suppliers.CollectionChanged += Suppliers_CollectionChanged;
            Manufacturers.CollectionChanged += Manufacturers_CollectionChanged;

            IsEdit = material != null;
            try
            {
                var materialInfo = material ?? new _3dPrinterMaterial();
                if (material != null)
                    Id = materialInfo.Id;
                Name = materialInfo.Name;
                SKU = materialInfo.SKU;
                Price = materialInfo.UnitPrice;
                Unit = materialInfo.Unit;
                ColorCode = materialInfo.ColorCode;
                LinkToReorder = materialInfo.LinkToReorder;
                PackageSize = materialInfo.PackageSize;
                Supplier = materialInfo.Supplier;
                Manufacturer = materialInfo.Manufacturer;
                TemperatureHeatbed = materialInfo.TemperatureHeatbed;
                TemperatureNozzle = materialInfo.TemperatureNozzle;
                Density = materialInfo.Density;
                TypeOfMaterial = materialInfo.TypeOfMaterial;
                buildCollectionView();
                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public NewMaterialViewModel(Action<NewMaterialViewModel> saveCommand, Action<NewMaterialViewModel> cancelHandler, IDialogCoordinator dialogCoordinator, _3dPrinterMaterial material = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            this._dialogCoordinator = dialogCoordinator;

            Suppliers.CollectionChanged += Suppliers_CollectionChanged;
            Manufacturers.CollectionChanged += Manufacturers_CollectionChanged;

            IsEdit = material != null;
            try { 
                var materialInfo = material ?? new _3dPrinterMaterial();
                if (material != null)
                    Id = materialInfo.Id;
                Name = materialInfo.Name;
                SKU = materialInfo.SKU;
                Price = materialInfo.UnitPrice;
                Unit = materialInfo.Unit;
                ColorCode = materialInfo.ColorCode;
                LinkToReorder = materialInfo.LinkToReorder;
                PackageSize = materialInfo.PackageSize;
                Supplier = materialInfo.Supplier;
                Manufacturer = materialInfo.Manufacturer;
                TemperatureHeatbed = materialInfo.TemperatureHeatbed;
                TemperatureNozzle = materialInfo.TemperatureNozzle;
                Density = materialInfo.Density;
                TypeOfMaterial = materialInfo.TypeOfMaterial;
                buildCollectionView();
                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        #endregion

        #region Events
        private void Manufacturers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SettingsManager.Save();
            OnPropertyChanged(nameof(Manufacturers));
        }

        private void Suppliers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SettingsManager.Save();
            OnPropertyChanged(nameof(Suppliers));
        }
        #endregion

        #region Methods
        private void buildCollectionView()
        {
            MaterialTypeViews = new CollectionViewSource
            {
                Source = (MaterialTypes.Select(m => new _3dPrinterMaterialTypes()
                {
                    Id = m.Id,
                    Kind = m.Kind,
                    Material = m.Material
                })).ToList()
            }.View;
            //MaterialTypeViews.SortDescriptions.Add(new SortDescription(nameof(PrinterViewInfo.Group), ListSortDirection.Ascending));
            //MaterialTypeViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(PrinterViewInfo.Group)));
        }
        #endregion

        #region iCommands & Actions
        public ICommand NewManufacturerCommand
        {
            get => new RelayCommand(p => NewManufacturerAction());
        }
        private async void NewManufacturerAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.NewManufacturer };
                var newManufacturerViewModel = new NewManufacturerViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Manufacturers.Add(new Manufacturer()
                    {
                        Id = instance.Id,
                        Name = instance.Name,
                        DebitorNumber = instance.DebitorNumber,
                        isActive = instance.isActive,
                        Website = instance.ShopUri,

                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Manufacturers[Manufacturers.Count - 1].Name));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                });

                _dialog.Content = new Views._3dPrinting.NewManufacturerDialog()
                {
                    DataContext = newManufacturerViewModel
                };
                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand NewSupplierCommand
        {
            get => new RelayCommand(p => NewSupplierAction());
        }
        private async void NewSupplierAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.NewSupplier };
                var newSupplierViewModel = new NewSupplierViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Suppliers.Add(new Supplier()
                    {
                        Id = instance.Id,
                        Name = instance.Name,
                        DebitorNumber = instance.DebitorNumber,
                        isActive = instance.isActive,
                        Website = instance.ShopUri,
                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Suppliers[Suppliers.Count - 1].Name));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                });

                _dialog.Content = new Views._3dPrinting.NewSupplierDialog()
                {
                    DataContext = newSupplierViewModel
                };
                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand SelectedMaterialTypeChangedCommand
        {
            get => new RelayCommand(p => SelectedMaterialTypeChangedAction(p));
        }
        private void SelectedMaterialTypeChangedAction(object p)
        {
            try
            {
                _3dPrinterMaterialKind materialKind = (_3dPrinterMaterialKind)Enum.Parse(typeof(_3dPrinterMaterialKind), p.ToString());

                MaterialTypeViews.Refresh();
                ICollectionView view = CollectionViewSource.GetDefaultView(MaterialTypeViews);
                IEqualityComparer<String> comparer = StringComparer.InvariantCultureIgnoreCase;
                view.Filter = o =>
                {
                    _3dPrinterMaterialTypes type = o as _3dPrinterMaterialTypes;
                    return type.Kind == materialKind;
                };
                var filteredItems = view.Cast<_3dPrinterMaterialTypes>();
                if (filteredItems.Count() > 0)
                    TypeOfMaterial = filteredItems.ElementAt(0) as _3dPrinterMaterialTypes;
                else
                    TypeOfMaterial = null;
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }


        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
