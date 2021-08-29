using AndreasReitberger.Enums;
using AndreasReitberger.Models;
using AndreasReitberger.Models.MaterialAdditions;
using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Models.Settings;
//Additional
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    public class NewMaterialViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Module
        int _defaultAttributesTabIndex = 0;
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

        string _name;
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

        string _sku;
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

        double _price = 25;
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
        
        Unit _unit = Unit.kg;
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
       
        double _size = 1;
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

        double _density = 1.20f;
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

        Material3dType _typeOfMaterial;
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

        ObservableCollection<Material3dAttribute> _attributes = new();
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

        ObservableCollection<Material3dProcedureAttribute> _procedureAttributes = new();
        public ObservableCollection<Material3dProcedureAttribute> ProcedureAttributes
        {
            get => _procedureAttributes;
            set
            {
                if (_procedureAttributes == value) return;
                _procedureAttributes = value;
                OnPropertyChanged();

            }
        }

        Material3dAttribute _selectedMaterialView;
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

        double _refreshingRate = 30;
        public double RefreshingRate
        {
            get => _refreshingRate;
            set
            {
                if (_refreshingRate == value) return;

                _refreshingRate = value;
                OnPropertyChanged();
                
            }
        }

        double _factorLiterToKg = 1;
        public double FactorLiterToKg
        {
            get => _factorLiterToKg;
            set
            {
                if (_factorLiterToKg == value) return;

                _factorLiterToKg = value;
                OnPropertyChanged();
                
            }
        }
        #endregion

        #region EnumCollections

        #region MaterialFamilies
        ObservableCollection<Material3dFamily> _materialFamilies = new(
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
        ObservableCollection<Unit> _units = new(
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
        ObservableCollection<string> _defaultAttributes = new();
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
        ObservableCollection<Manufacturer> _manufactures = new();
        public ObservableCollection<Manufacturer> Manufacturers
        {
            get => _manufactures;
            set
            {
                if (value == _manufactures) return;

                if (!IsLoading)
                    SettingsManager.Current.Manufacturers = value;

                _manufactures = value;
                OnPropertyChanged();
            }

        }

        ObservableCollection<Supplier> _suppliers = new();
        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set
            {
                if (_suppliers == value) return;

                if(!IsLoading)
                    SettingsManager.Current.Suppliers = value;

                _suppliers = value;
                OnPropertyChanged();
                
            }

        }

        ObservableCollection<Material3dType> _materialTypes = new();
        public ObservableCollection<Material3dType> MaterialTypes
        {
            get => _materialTypes;
            set
            {
                if (_materialTypes == value)
                {
                    return;
                }
                if (!IsLoading)
                {
                    SettingsManager.Current.MaterialTypes = value;
                }
                _materialTypes = value;
                OnPropertyChanged();
            }
        }

        public ICollectionView MaterialTypeViews
        {
            get => _materialTypeViews;
            set
            {
                if (_materialTypeViews != value)
                {
                    _materialTypeViews = value;
                    OnPropertyChanged();
                }
            }
        }
        ICollectionView _materialTypeViews;
        #endregion

        #region Constructor, LoadSettings
        public NewMaterialViewModel(Action<NewMaterialViewModel> saveCommand, Action<NewMaterialViewModel> cancelHandler, Material3d material = null)
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

            IsLoading = true;
            LoadSettings();
            IsLoading = false;

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

        void LoadSettings()
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
        void LoadItem(Material3d material)
        {
            // Load Id if material is not null
            if (material != null && material.Id != Guid.Empty)
            {
                Id = material.Id;
            }

            Name = material.Name;
            SKU = material.SKU;
            Price = material.UnitPrice;
            Unit = material.Unit;
            LinkToReorder = material.Uri;
            PackageSize = material.PackageSize;
            Manufacturer = material.Manufacturer;
            Density = material.Density;
            FactorLiterToKg = material.FactorLToKg;
            MaterialFamily = material.MaterialFamily;
            
            TypeOfMaterial = material.TypeOfMaterial;
            Attributes = new ObservableCollection<Material3dAttribute>(material.Attributes);
            if(material.ProcedureAttributes.Count > 0)
            {
                Material3dProcedureAttribute refreshingRatio = material.ProcedureAttributes.FirstOrDefault(attr => attr.Attribute == ProcedureAttribute.MaterialRefreshingRatio);
                if (refreshingRatio != null)
                {
                    RefreshingRate = refreshingRatio.Value;
                }
            }
            BuildCollectionView();

            if (SelectedMaterialTypeChangedCommand.CanExecute(MaterialFamily))
            {
                SelectedMaterialTypeChangedCommand.Execute(MaterialFamily);
            }
        }
        void BuildCollectionView()
        {
            MaterialTypeViews = new CollectionViewSource
            {
                Source = MaterialTypes.Select(m => new Material3dType()
                {
                    Id = m.Id,
                    Type = m.Type,
                    Material = m.Material
                }).ToList()
            }.View;
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
            get => new RelayCommand(async(p) => await NewSupplierNewAttributeAction());
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
            get => new RelayCommand((p) => AddAttributeToListAction());
        }
        void AddAttributeToListAction()
        {
            try
            {
                if(!string.IsNullOrEmpty(AttributeName))
                {
                    Material3dAttribute attribute = Attributes.FirstOrDefault(at => at.Attribute == AttributeName);
                    if (attribute == null)
                    {
                        Attributes.Add(new Material3dAttribute() { Attribute = AttributeName, Value = AttributeValue });
                    }
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
            get => new RelayCommand((p) => SelectedMaterialTypeChangedAction(p));
        }
        void SelectedMaterialTypeChangedAction(object p)
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
