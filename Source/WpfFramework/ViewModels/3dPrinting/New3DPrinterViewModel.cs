using WpfFramework.Models;
using WpfFramework.Models._3dprinting;
using WpfFramework.Models.Settings;
using WpfFramework.Utilities;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using log4net;
using WpfFramework.Resources.Localization;

namespace WpfFramework.ViewModels._3dPrinting
{
    class New3DPrinterViewModel : ViewModelBase
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

        private _3dPrinterType _type = _3dPrinterType.FDM;
        public _3dPrinterType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }

        private Manufacturer _manufacturer;
        public Manufacturer Manufacturer
        {
            get => _manufacturer;
            set
            {
                if (_manufacturer != value)
                {
                    _manufacturer = value;
                    OnPropertyChanged(nameof(Manufacturer));
                }
            }
        }

        private string _model;
        public string Model
        {
            get => _model;
            set
            {
                if(_model != value)
                {
                    _model = value;
                    OnPropertyChanged(nameof(Model));
                }
            }
        }

        private decimal _price = 0;
        public decimal Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        private int _size = 1;
        public int PackageSize
        {
            get => _size;
            set
            {
                if (_size != value)
                {
                    _size = value;
                    OnPropertyChanged(nameof(PackageSize));
                }
            }
        }
        
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
        
        private Supplier _supplier;
        public Supplier Supplier
        {
            get => _supplier;
            set
            {
                if (_supplier != value)
                {
                    _supplier = value;
                    OnPropertyChanged(nameof(Supplier));
                }
            }
        }

        private string _linkShop;
        public string LinkToReorder
        {
            get => _linkShop;
            set
            {
                if (_linkShop != value)
                {
                    _linkShop = value;
                    OnPropertyChanged(nameof(LinkToReorder));
                }
            }
        }

        private bool _hasHeatbed = true;
        public bool hasHeatbed
        {
            get => Type == _3dPrinterType.FDM ? _hasHeatbed : false;
            set
            {
                if (_hasHeatbed != value)
                {
                    _hasHeatbed = value;
                    OnPropertyChanged(nameof(hasHeatbed));
                }
            }
        }

        private int _powerConsumption = 100;
        public int PowerConsumption
        {
            get => _powerConsumption;
            set
            {
                if (_powerConsumption != value)
                {
                    _powerConsumption = value;
                    OnPropertyChanged(nameof(PowerConsumption));
                }
            }
        }

        private int _temperatureHeatbed = 80;
        public int TemperatureHeatbed
        {
            get => Type == _3dPrinterType.FDM ? _temperatureHeatbed : 0;
            set
            {
                if (_temperatureHeatbed != value)
                {
                    _temperatureHeatbed = value;
                    OnPropertyChanged(nameof(TemperatureHeatbed));
                }
            }
        }

        private int _temperatureNozzle = 280;
        public int TemperatureNozzle
        {
            get => Type == _3dPrinterType.FDM ? _temperatureNozzle : 0;
            set
            {
                if (_temperatureNozzle != value)
                {
                    _temperatureNozzle = value;
                    OnPropertyChanged(nameof(TemperatureNozzle));
                }
            }
        }

        private BuildVolume _volume = new BuildVolume(1,1,1);
        public BuildVolume BuildVolume
        {
            get => _volume;
            set
            {
                if(_volume != value)
                {
                    _volume = value;
                    OnPropertyChanged(nameof(BuildVolume));
                }
            }
        }

        private decimal  _machineHourRate;
        public decimal MachineHourRate
        {
            get => MachineHourRateCalc != null ? MachineHourRateCalc.CalcMachineHourRate : _machineHourRate;
            set
            {
                if (_machineHourRate != value)
                {
                    _machineHourRate = value;
                    OnPropertyChanged(nameof(MachineHourRate));
                }
            }
        }

        private MachineHourRate _mhr;
        public MachineHourRate MachineHourRateCalc
        {
            get => _mhr;
            set
            {
                if(_mhr != value)
                {
                    _mhr = value;
                    OnPropertyChanged(nameof(MachineHourRateCalc));
                    OnPropertyChanged(nameof(MachineHourRate));
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
                    OnPropertyChanged(nameof(Manufacturers));
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
                    OnPropertyChanged(nameof(Suppliers));
                }
            }

        }
        #endregion

        #region Constructor
        public New3DPrinterViewModel(Action<New3DPrinterViewModel> saveCommand, Action<New3DPrinterViewModel> cancelHandler, _3dPrinterModel printer = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            Suppliers.CollectionChanged += Suppliers_CollectionChanged;
            Manufacturers.CollectionChanged += Manufacturers_CollectionChanged;

            IsEdit = printer != null;
            try
            {
                var printerInfo = printer ?? new _3dPrinterModel();
                if (printer != null)
                    Id = printerInfo.Id;
                Price = printerInfo.Price;
                Type = printerInfo.Type;
                Supplier = printerInfo.Supplier;
                Manufacturer = printerInfo.Manufacturer;
                hasHeatbed = printerInfo.hasHeatbed;
                TemperatureHeatbed = printerInfo.MaxHeatbedTemperature;
                Kind = printerInfo.Kind;
                Model = printerInfo.Model;
                MachineHourRate = printerInfo.MachineHourRate;
                MachineHourRateCalc = printerInfo.MachineHourRateCalc;
                TemperatureNozzle = printerInfo.MaxNozzleTemperature;
                if (printerInfo.BuildVolume != null)
                    BuildVolume = printerInfo.BuildVolume;
                PowerConsumption = printerInfo.PowerConsumption;
                LinkToReorder = printerInfo.ShopUri;
                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }

        }
        public New3DPrinterViewModel(Action<New3DPrinterViewModel> saveCommand, Action<New3DPrinterViewModel> cancelHandler, IDialogCoordinator dialogCoordinator, _3dPrinterModel printer = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            this._dialogCoordinator = dialogCoordinator;

            Suppliers.CollectionChanged += Suppliers_CollectionChanged;
            Manufacturers.CollectionChanged += Manufacturers_CollectionChanged;

            IsEdit = printer != null;
            try { 
                var printerInfo = printer ?? new _3dPrinterModel();
                if(printer != null)
                    Id = printerInfo.Id;
                Price = printerInfo.Price;
                Type = printerInfo.Type;
                Supplier = printerInfo.Supplier;
                Manufacturer = printerInfo.Manufacturer;
                hasHeatbed = printerInfo.hasHeatbed;
                TemperatureHeatbed = printerInfo.MaxHeatbedTemperature;
                Kind = printerInfo.Kind;
                Model = printerInfo.Model;
                MachineHourRate = printerInfo.MachineHourRate;
                MachineHourRateCalc = printerInfo.MachineHourRateCalc;
                TemperatureNozzle = printerInfo.MaxNozzleTemperature;
                if (printerInfo.BuildVolume != null)
                    BuildVolume = printerInfo.BuildVolume;
                PowerConsumption = printerInfo.PowerConsumption;
                LinkToReorder = printerInfo.ShopUri;
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

        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
