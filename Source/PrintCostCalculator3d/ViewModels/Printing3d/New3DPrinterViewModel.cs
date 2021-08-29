using AndreasReitberger.Enums;
using AndreasReitberger.Models;
using AndreasReitberger.Models.PrinterAdditions;
using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    public class New3DPrinterViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Properties
        bool _isEdit;
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

        Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get => _id;
            set
            {
                if (_id == value) return;
                _id = value;
                OnPropertyChanged();

            }
        }

        Printer3dType _type = Printer3dType.FDM;
        public Printer3dType Type
        {
            get => _type;
            set
            {
                if (_type == value) return;
                _type = value;
                OnPropertyChanged();              
            }
        }

        Material3dFamily _materialFamily = Material3dFamily.Filament;
        public Material3dFamily MaterialFamily
        {
            get => _materialFamily;
            set
            {
                if (_materialFamily == value) return;
                _materialFamily = value;
                OnPropertyChanged();
                
            }
        }

        Manufacturer _manufacturer;
        public Manufacturer Manufacturer
        {
            get => _manufacturer;
            set
            {
                if (_manufacturer == value) return;

                _manufacturer = value;
                OnPropertyChanged();

            }
        }

        Supplier _supplier;
        public Supplier Supplier
        {
            get => _supplier;
            set
            {
                if (_supplier == value) return;
                _supplier = value;
                OnPropertyChanged();
            }
        }

        HourlyMachineRate _machineHourRateCalculation;
        public HourlyMachineRate MachineHourRateCalculation
        {
            get => _machineHourRateCalculation;
            set
            {
                if (_machineHourRateCalculation == value) return;
                _machineHourRateCalculation = value;
                OnPropertyChanged();

            }
        }

        string _model;
        public string Model
        {
            get => _model;
            set
            {
                if (_model == value) return;        
                _model = value;
                OnPropertyChanged();
                
            }
        }

        double _powerConsumption = 100;
        public double PowerConsumption
        {
            get => _powerConsumption;
            set
            {
                if (_powerConsumption == value) return;
                _powerConsumption = value;
                OnPropertyChanged();           
            }
        }

        double _price = 0;
        public double Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        string _linkShop;
        public string LinkToReorder
        {
            get => _linkShop;
            set
            {
                if (_linkShop == value) return;       
                _linkShop = value;
                OnPropertyChanged();
                
            }
        }

        double _width = 0;
        public double Width
        {
            get => _width;
            set
            {
                if (_width == value) return;
                _width = value;
                OnPropertyChanged();

            }
        }

        double _height = 0;
        public double Height
        {
            get => _height;
            set
            {
                if (_height == value) return;
                _height = value;
                OnPropertyChanged();

            }
        }

        double _depth = 0;
        public double Depth
        {
            get => _depth;
            set
            {
                if (_depth == value) return;
                _depth = value;
                OnPropertyChanged();

            }
        }

        BuildVolume _volume = new BuildVolume(1,1,1);
        public BuildVolume BuildVolume
        {
            get => _volume;
            set
            {
                if (_volume == value) return;
                _volume = value;
                OnPropertyChanged();
            }
        }

        bool _useFixedMachineHourRating = false;
        public bool UseFixedMachineHourRating
        {
            get => _useFixedMachineHourRating;
            set
            {
                if (_useFixedMachineHourRating == value) return;

                _useFixedMachineHourRating = value;
                OnPropertyChanged();
                
            }
        }

        double _machineHourRate = 0;
        public double MachineHourRate
        {
            get => _machineHourRate;
            set
            {
                if (_machineHourRate == value) return;
                _machineHourRate = value;
                OnPropertyChanged();

            }
        }

        string _attributeName = "";
        public string AttributeName
        {
            get => _attributeName;
            set
            {
                if (_attributeName == value) return;
                _attributeName = value;
                OnPropertyChanged();
            }
        }

        double _attributeValue = 0;
        public double AttributeValue
        {
            get => _attributeValue;
            set
            {
                if (_attributeValue == value) return;
                _attributeValue = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<Printer3dAttribute> _attributes = new ObservableCollection<Printer3dAttribute>();
        public ObservableCollection<Printer3dAttribute> Attributes
        {
            get => _attributes;
            set
            {
                if (_attributes == value) return;
                _attributes = value;
                OnPropertyChanged();

            }
        }

        Printer3dAttribute _selectedMaterialView;
        public Printer3dAttribute SelectedAttribute
        {
            get => _selectedMaterialView;
            set
            {
                if (_selectedMaterialView == value) return;

                _selectedMaterialView = value;
                OnPropertyChanged();

            }
        }

        IList _selectedAttributes = new ArrayList();
        public IList SelectedAttributes
        {
            get => _selectedAttributes;
            set
            {
                if (_selectedAttributes == value) return;

                _selectedAttributes = value;
                OnPropertyChanged();

            }
        }

        #region SlicerConfig
        double _aMax_xy = 1000;
        public double AMax_xy
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

        double _aMax_z = 1000;
        public double AMax_z
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

        double _aMax_e = 5000;
        public double AMax_e
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

        double _aMax_eExtrude = 1250;
        public double AMax_eExtrude
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

        double _aMax_eRetract = 1250;
        public double AMax_eRetract
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

        double _printDurationCorrection = 1;
        public double PrintDurationCorrection
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

        #endregion

        #region EnumCollections

        #region MaterialFamilies
        ObservableCollection<Material3dFamily> _materialFamilies = new ObservableCollection<Material3dFamily>(
            Enum.GetValues(typeof(Material3dFamily)).Cast<Material3dFamily>().ToList()
            );
        public ObservableCollection<Material3dFamily> MaterialFamilies
        {
            get => _materialFamilies;
            set
            {
                if (_materialFamilies == value) return;

                _materialFamilies = value;
                OnPropertyChanged();

            }
        }

        #endregion
        
        #region PrinterTypes
        ObservableCollection<Printer3dType> _printerTypes = new ObservableCollection<Printer3dType>(
            Enum.GetValues(typeof(Printer3dType)).Cast<Printer3dType>().ToList()
            );
        public ObservableCollection<Printer3dType> PrinterTypes
        {
            get => _printerTypes;
            set
            {
                if (_printerTypes == value) return;

                _printerTypes = value;
                OnPropertyChanged();

            }
        }

        #endregion

        #endregion

        #region DefaultAttributes
        ObservableCollection<string> _defaultAttributes = new ObservableCollection<string>();
        public ObservableCollection<string> DefaultAttributes
        {
            get => _defaultAttributes;
            set
            {
                if (_defaultAttributes == value) return;
                if (!IsLoading)
                    SettingsManager.Current.MaterialAttributes = value;
                _defaultAttributes = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Settings
        ObservableCollection<Manufacturer> _manufacturers = new ObservableCollection<Manufacturer>();
        public ObservableCollection<Manufacturer> Manufacturers
        {
            get => _manufacturers;
            set
            {
                if (_manufacturers == value) return;
                if(!IsLoading)
                    SettingsManager.Current.Manufacturers = value;
                _manufacturers = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<Supplier> _suplliers = new ObservableCollection<Supplier>();
        public ObservableCollection<Supplier> Suppliers
        {
            get => _suplliers;
            set
            {
                if (_suplliers == value) return;
                if (!IsLoading)
                    SettingsManager.Current.Suppliers = value;
                _suplliers = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructor, LoadSettings
        public New3DPrinterViewModel(Action<New3DPrinterViewModel> saveCommand, Action<New3DPrinterViewModel> cancelHandler, Printer3d printer = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            IsLicenseValid = false;

            Suppliers.CollectionChanged += Suppliers_CollectionChanged;
            Manufacturers.CollectionChanged += Manufacturers_CollectionChanged;
            DefaultAttributes.CollectionChanged += DefaultAttributes_CollectionChanged;

            IsEdit = printer != null;
            try
            {
                LoadItem(printer ?? new Printer3d());
                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }

        }
        public New3DPrinterViewModel(Action<New3DPrinterViewModel> saveCommand, Action<New3DPrinterViewModel> cancelHandler, IDialogCoordinator dialogCoordinator, Printer3d printer = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            _dialogCoordinator = dialogCoordinator;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            IsLicenseValid = false;

            Suppliers.CollectionChanged += Suppliers_CollectionChanged;
            Manufacturers.CollectionChanged += Manufacturers_CollectionChanged;
            DefaultAttributes.CollectionChanged += DefaultAttributes_CollectionChanged;

            IsEdit = printer != null;
            try {
                LoadItem(printer ?? new Printer3d());
                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        void LoadSettings()
        {
            DefaultAttributes = SettingsManager.Current.MaterialAttributes;
            if (DefaultAttributes.Count > 0)
                AttributeName = DefaultAttributes[0];

            Manufacturers = SettingsManager.Current.Manufacturers;
            Suppliers = SettingsManager.Current.Suppliers;
        }
        #endregion

        #region Events
        void Manufacturers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SettingsManager.Save();
            OnPropertyChanged(nameof(Manufacturers));
        }

        void Suppliers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SettingsManager.Save();
            OnPropertyChanged(nameof(Suppliers));
        }
        void DefaultAttributes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SettingsManager.Save();
            OnPropertyChanged(nameof(DefaultAttributes));
        }
        #endregion

        #region Methods
        void LoadItem(Printer3d printer)
        {
            // Load Id if material is not null
            if (printer != null && printer.Id != Guid.Empty)
                Id = printer.Id;

            Price = printer.Price;
            Type = printer.Type;
            //Supplier = printer.Supplier;
            Manufacturer = printer.Manufacturer;
            MaterialFamily = printer.MaterialType;
            Model = printer.Model;
            PowerConsumption = printer.PowerConsumption;
            LinkToReorder = printer.Uri;
            UseFixedMachineHourRating = printer.UseFixedMachineHourRating;
            if (UseFixedMachineHourRating && printer.HourlyMachineRate != null)
                MachineHourRate = printer.HourlyMachineRate.FixMachineHourRate;
            MachineHourRateCalculation = printer.HourlyMachineRate;
            Attributes = new ObservableCollection<Printer3dAttribute>(printer.Attributes);
            if (printer.BuildVolume != null)
            {
                BuildVolume = printer.BuildVolume;
                Width = BuildVolume.X;
                Depth = BuildVolume.Y;
                Height = BuildVolume.Z;
            }
            if(printer.SlicerConfig != null)
            {
                AMax_xy = printer.SlicerConfig.AMax_xy;
                AMax_z = printer.SlicerConfig.AMax_z;
                AMax_e = printer.SlicerConfig.AMax_e;
                AMax_eExtrude = printer.SlicerConfig.AMax_eExtrude;
                AMax_eRetract = printer.SlicerConfig.AMax_eRetract;
            }
        }
        #endregion


        #region iCommands & Actions
        public ICommand NewManufacturerCommand
        {
            get => new RelayCommand(async(p) => await NewManufacturerAction());
        }
        async Task NewManufacturerAction()
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
                },
                _dialogCoordinator
                );

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
            get => new RelayCommand(async(p) => await NewSupplierAction());
        }
        async Task NewSupplierAction()
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
                },
                _dialogCoordinator
                );

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

        public ICommand NewAttributeCommand
        {
            get => new RelayCommand(async (p) => await NewSupplierNewAttributeAction());
        }
        async Task NewSupplierNewAttributeAction()
        {
            try
            {
                var attribute = await _dialogCoordinator.ShowInputAsync(this,
                    Strings.DialogAddNewAttributeHeadline,
                    Strings.DialogAddNewAttributeContent,
                    new MetroDialogSettings()
                    {
                        AffirmativeButtonText = Strings.AddAttribute,
                        NegativeButtonText = Strings.Cancel,
                    });
                if (!string.IsNullOrEmpty(attribute))
                {
                    DefaultAttributes.Add(attribute);
                    AttributeName = attribute;
                }
                else
                    logger.WarnFormat(Strings.EventEnteredValueWasInvalidFormated, attribute);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand AddAttributeToListCommand
        {
            get => new RelayCommand((p) => AddAttributeToListAction());
        }
        void AddAttributeToListAction()
        {
            try
            {
                if (!string.IsNullOrEmpty(AttributeName))
                {
                    Printer3dAttribute attribute = Attributes.FirstOrDefault(at => at.Attribute == AttributeName);
                    if (attribute == null)
                    {
                        Attributes.Add(new Printer3dAttribute() { Attribute = AttributeName, Value = AttributeValue });
                    }
                    else
                    {
                        attribute.Value = AttributeValue;
                        OnPropertyChanged(nameof(Attributes));
                    }

                }
                else
                {
                    logger.WarnFormat(Strings.EventEnteredValueWasInvalidFormated, AttributeName);
                }
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
