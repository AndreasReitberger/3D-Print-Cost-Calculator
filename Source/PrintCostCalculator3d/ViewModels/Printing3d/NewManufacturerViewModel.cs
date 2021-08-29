using AndreasReitberger.Models;
using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using System;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    public class NewManufacturerViewModel : ViewModelBase
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

        string _name = string.Empty;
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

        string _debitorNumber = string.Empty;
        public string DebitorNumber
        {
            get => _debitorNumber;
            set
            {
                if (_debitorNumber == value) return;
                _debitorNumber = value;
                OnPropertyChanged();              
            }
        }

        string _shopUri = string.Empty;
        public string ShopUri
        {
            get => _shopUri;
            set
            {
                if (_shopUri == value) return;
                _shopUri = value;
                OnPropertyChanged();
                
            }
        }

        bool _isActive = true;
        public bool isActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value) return;
                _isActive = value;
                OnPropertyChanged(nameof(isActive));
                
            }
        }
        #endregion

        #region Constructors
        public NewManufacturerViewModel(Action<NewManufacturerViewModel> saveCommand, Action<NewManufacturerViewModel> cancelHandler, IDialogCoordinator dialogCoordinator, Manufacturer manufacturer = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            _dialogCoordinator = dialogCoordinator;

            IsLicenseValid = false;

            IsEdit = manufacturer != null;
            try
            {
                LoadItem(manufacturer ?? new Manufacturer() { isActive = true });
                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        #endregion

        #region iCommands & Actions

        public ICommand LoadFromSupplierCommand
        {
            get => new RelayCommand(p => LoadFromSupplierAction());
        }
        async void LoadFromSupplierAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.Suppliers };
                var newViewModel = new SelectSupplierViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    this.Name = instance.SelectedSupplier.Name;
                    this.DebitorNumber = instance.SelectedSupplier.DebitorNumber;
                    this.isActive = instance.SelectedSupplier.isActive;
                    this.ShopUri = instance.SelectedSupplier.Website;

                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                _dialogCoordinator
                );

                _dialog.Content = new Views._3dPrinting.SelectSupplierDialog()
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
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion

        #region Methods
        void LoadItem(Manufacturer manufacturer)
        {
            // Load Id if material is not null
            if (manufacturer != null && manufacturer.Id != Guid.Empty)
                Id = manufacturer.Id;

            Name = manufacturer.Name;
            DebitorNumber = manufacturer.DebitorNumber;
            isActive = manufacturer.isActive;
            ShopUri = manufacturer.Website;
        }
        #endregion
    }
}
