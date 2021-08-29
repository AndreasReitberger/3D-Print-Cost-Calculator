using AndreasReitberger.Models;
using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    class SelectSupplierViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Properties
        ObservableCollection<Supplier> _suppliers = new ObservableCollection<Supplier>();
        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set
            {
                if (value == _suppliers)
                    return;

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

                _SelectedSuppliers = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor
        public SelectSupplierViewModel(Action<SelectSupplierViewModel> saveCommand, Action<SelectSupplierViewModel> cancelHandler)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            IsLicenseValid = false;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;
        }
        public SelectSupplierViewModel(Action<SelectSupplierViewModel> saveCommand, Action<SelectSupplierViewModel> cancelHandler, IDialogCoordinator dialogCoordinator)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            _dialogCoordinator = dialogCoordinator;

            IsLicenseValid = false;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;
        }

        void LoadSettings()
        {
            Suppliers = SettingsManager.Current.Suppliers;
        }
        #endregion

        #region iCommands & Actions
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
