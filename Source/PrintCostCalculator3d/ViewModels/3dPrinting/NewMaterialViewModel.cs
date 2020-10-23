using PrintCostCalculator3d.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

//Additional
using PrintCostCalculator3d.Resources.Localization;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using PrintCostCalculator3d.Models.Settings;
using System.ComponentModel;
using System.Windows.Data;
using log4net;
using AndreasReitberger.Models;
using AndreasReitberger.Models.MaterialAdditions;
using AndreasReitberger.Enums;
using System.Threading.Tasks;
using System.Collections;
using PrintCostCalculator3d.Models;

namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    public class NewMaterialViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool _isLoading;
        #endregion

        #region Module
        private int _defaultAttributesTabIndex = 0;
        public int DefaultAttributesTabIndex
        {
            get => _defaultAttributesTabIndex;
            set
            {
                if (value == _defaultAttributesTabIndex)
                    return;

                _defaultAttributesTabIndex = value;
                OnPropertyChanged();
            }
        }
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
                if (_id == value) return; 
                _id = value;
                OnPropertyChanged();
                
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged();
                
            }
        }

        private string _sku;
        public string SKU
        {
            get => _sku;
            set
            {
                if (_sku == value) return; 
                _sku = value;
                OnPropertyChanged();
                
            }
        }

        private double _price = 25;
        public double Price
        {
            get { return _price; }
            set
            {
                if (_price == value) return;
                _price = value;
                OnPropertyChanged();
            }
        }
        
        private Unit _unit = Unit.kg;
        public Unit Unit
        {
            get => _unit;
            set
            {
                if (_unit == value) return;
                _unit = value;
                OnPropertyChanged();

            }
        }
       
        private double _size = 1;
        public double PackageSize
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

        private double _density = 1.20f;
        public double Density
        {
            get => _density;
            set
            {
                if (_density == value) return;
                _density = value;
                OnPropertyChanged();
                
            }
        }

        private Material3dType _typeOfMaterial;
        public Material3dType TypeOfMaterial
        {
            get => _typeOfMaterial;
            set
            {
                if (_typeOfMaterial == value) return;
                _typeOfMaterial = value;
                OnPropertyChanged();
                
            }
        }
      

        private Material3dFamily _materialFamily = Material3dFamily.Filament;
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

        private Manufacturer _manufacturer;
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
        
        private Supplier _supplier;
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
        
        private string _linkShop;
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

        private string _attributeName = "";
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

        private double _attributeValue = 0;
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

        private ObservableCollection<Material3dAttribute> _attributes = new ObservableCollection<Material3dAttribute>();
        public ObservableCollection<Material3dAttribute> Attributes
        {
            get => _attributes;
            set
            {
                if (_attributes == value) return;
                _attributes = value;
                OnPropertyChanged();

            }
        }

        private Material3dAttribute _selectedMaterialView;
        public Material3dAttribute SelectedAttribute
        {
            get => _selectedMaterialView;
            set
            {
                if (_selectedMaterialView == value) return;
                
                _selectedMaterialView = value;
                OnPropertyChanged();
                
            }
        }

        private IList _selectedAttributes = new ArrayList();
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
        #endregion

        #region EnumCollections

        #region MaterialFamilies
        private ObservableCollection<Material3dFamily> _materialFamilies = new ObservableCollection<Material3dFamily>(
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

        #region Units
        private ObservableCollection<Unit> _units = new ObservableCollection<Unit>(
            Enum.GetValues(typeof(Unit)).Cast<Unit>().ToList()
            );
        public ObservableCollection<Unit> Units
        {
            get => _units;
            set
            {
                if (_units == value) return;

                _units = value;
                OnPropertyChanged();

            }
        }

        #endregion

        #endregion

        #region DefaultAttributes
        private ObservableCollection<string> _defaultAttributes = new ObservableCollection<string>();
        public ObservableCollection<string> DefaultAttributes
        {
            get => _defaultAttributes;
            set
            {
                if (_defaultAttributes == value) return;
                if (!_isLoading)
                    SettingsManager.Current.MaterialAttributes = value;
                _defaultAttributes = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Settings
        private ObservableCollection<Manufacturer> _manufactures = new ObservableCollection<Manufacturer>();
        public ObservableCollection<Manufacturer> Manufacturers
        {
            get => _manufactures;
            set
            {
                if (value == _manufactures) return;

                if (!_isLoading)
                    SettingsManager.Current.Manufacturers = value;

                _manufactures = value;
                OnPropertyChanged();
            }

        }

        private ObservableCollection<Supplier> _suppliers = new ObservableCollection<Supplier>();
        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set
            {
                if (_suppliers == value) return;

                if(!_isLoading)
                    SettingsManager.Current.Suppliers = value;

                _suppliers = value;
                OnPropertyChanged();
                
            }

        }

        private ObservableCollection<Material3dType> _materialTypes = new ObservableCollection<Material3dType>();
        public ObservableCollection<Material3dType> MaterialTypes
        {
            get => _materialTypes;
            set
            {
                if (_materialTypes == value) return;
                if (!_isLoading)
                    SettingsManager.Current.MaterialTypes = value;
                _materialTypes = value;
                OnPropertyChanged();
            }
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
        #endregion

        #region Constructor, LoadSettings
        public NewMaterialViewModel(Action<NewMaterialViewModel> saveCommand, Action<NewMaterialViewModel> cancelHandler, Material3d material = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            _isLoading = true;
            LoadSettings();
            _isLoading = false;

            Suppliers.CollectionChanged += Suppliers_CollectionChanged;
            Manufacturers.CollectionChanged += Manufacturers_CollectionChanged;
            DefaultAttributes.CollectionChanged += DefaultAttributes_CollectionChanged;

            IsEdit = material != null;
            try
            {
                LoadItem(material ?? new Material3d());
                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public NewMaterialViewModel(Action<NewMaterialViewModel> saveCommand, Action<NewMaterialViewModel> cancelHandler, IDialogCoordinator dialogCoordinator, Material3d material = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            this._dialogCoordinator = dialogCoordinator;

            _isLoading = true;
            LoadSettings();
            _isLoading = false;

            Suppliers.CollectionChanged += Suppliers_CollectionChanged;
            Manufacturers.CollectionChanged += Manufacturers_CollectionChanged;
            DefaultAttributes.CollectionChanged += DefaultAttributes_CollectionChanged;

            IsEdit = material != null;
            try
            {
                LoadItem(material ?? new Material3d());
                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        private void LoadSettings()
        {
            MaterialTypes = SettingsManager.Current.MaterialTypes;
            DefaultAttributes = SettingsManager.Current.MaterialAttributes;
            if (DefaultAttributes.Count > 0)
                AttributeName = DefaultAttributes[0];
            //buildCollectionView();
            Manufacturers = SettingsManager.Current.Manufacturers;
            Suppliers = SettingsManager.Current.Suppliers;
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

        private void DefaultAttributes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SettingsManager.Save();
            OnPropertyChanged(nameof(DefaultAttributes));
        }
        #endregion

        #region Methods
        private void LoadItem(Material3d material)
        {
            // Load Id if material is not null
            if (material != null && material.Id != Guid.Empty)
                Id = material.Id;

            Name = material.Name;
            SKU = material.SKU;
            Price = material.UnitPrice;
            Unit = material.Unit;
            LinkToReorder = material.Uri;
            PackageSize = material.PackageSize;
            Manufacturer = material.Manufacturer;
            Density = material.Density;
            MaterialFamily = material.MaterialFamily;
            
            TypeOfMaterial = material.TypeOfMaterial;
            Attributes = new ObservableCollection<Material3dAttribute>(material.Attributes);
            buildCollectionView();

            if (SelectedMaterialTypeChangedCommand.CanExecute(MaterialFamily))
                SelectedMaterialTypeChangedCommand.Execute(MaterialFamily);
        }
        private void buildCollectionView()
        {
            MaterialTypeViews = new CollectionViewSource
            {
                Source = (MaterialTypes.Select(m => new Material3dType()
                {
                    Id = m.Id,
                    Type = m.Type,
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
            get => new RelayCommand(async(p) => await NewManufacturerAction());
        }
        private async Task NewManufacturerAction()
        {
            try
            {

                var _dialog = new CustomDialog() { Title = Strings.NewManufacturer };
                var newManufacturerViewModel = new NewManufacturerViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    var manufacturer = new Manufacturer()
                    {
                        Id = instance.Id,
                        Name = instance.Name,
                        DebitorNumber = instance.DebitorNumber,
                        isActive = instance.isActive,
                        Website = instance.ShopUri,

                    };
                    Manufacturers.Add(manufacturer);
                    
                    logger.Info(string.Format(Strings.EventAddedItemFormated, manufacturer.Name));
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
        private async Task NewSupplierAction()
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
            get => new RelayCommand(async(p) => await NewSupplierNewAttributeAction());
        }
        private async Task NewSupplierNewAttributeAction()
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
                if(!string.IsNullOrEmpty(attribute))
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
            get => new RelayCommand(async(p) => await AddAttributeToListAction());
        }
        private async Task AddAttributeToListAction()
        {
            try
            {
                if(!string.IsNullOrEmpty(AttributeName))
                {
                    var attribute = Attributes.FirstOrDefault(at => at.Attribute == AttributeName);
                    if (attribute == null)
                        Attributes.Add(new Material3dAttribute() { Attribute = AttributeName, Value = AttributeValue });
                    else
                    {
                        attribute.Value = AttributeValue;
                        OnPropertyChanged(nameof(Attributes));
                    }

                }
                else
                    logger.WarnFormat(Strings.EventEnteredValueWasInvalidFormated, AttributeName);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand SelectedMaterialTypeChangedCommand
        {
            get => new RelayCommand(async(p) => await SelectedMaterialTypeChangedAction(p));
        }
        private async Task SelectedMaterialTypeChangedAction(object p)
        {
            try
            {
                Material3dFamily materialKind = (Material3dFamily)Enum.Parse(typeof(Material3dFamily), p.ToString());

                MaterialTypeViews.Refresh();
                ICollectionView view = CollectionViewSource.GetDefaultView(MaterialTypeViews);
                IEqualityComparer<String> comparer = StringComparer.InvariantCultureIgnoreCase;
                view.Filter = o =>
                {
                    Material3dType type = o as Material3dType;
                    return type.Type == materialKind;
                };
                var filteredItems = view.Cast<Material3dType>();
                if (filteredItems.Count() > 0)
                {
                    if(!filteredItems.Contains(TypeOfMaterial))
                        TypeOfMaterial = filteredItems.ElementAt(0);
                }
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
