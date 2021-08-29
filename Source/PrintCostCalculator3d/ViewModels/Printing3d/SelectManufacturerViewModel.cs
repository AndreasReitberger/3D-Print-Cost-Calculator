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
    class SelectManufacturerViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Properties
        ObservableCollection<Manufacturer> _manufacturers = new ObservableCollection<Manufacturer>();
        public ObservableCollection<Manufacturer> Manufacturers
        {
            get => _manufacturers;
            set
            {
                if (value == _manufacturers)
                    return;

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

                _SelectedManufacturers = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor, LoadSettings
        public SelectManufacturerViewModel(Action<SelectManufacturerViewModel> saveCommand, Action<SelectManufacturerViewModel> cancelHandler)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            IsLicenseValid = false;

            LoadSettings();
        }
        public SelectManufacturerViewModel(Action<SelectManufacturerViewModel> saveCommand, Action<SelectManufacturerViewModel> cancelHandler, IDialogCoordinator dialogCoordinator)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            _dialogCoordinator = dialogCoordinator;

            IsLicenseValid = false;

            LoadSettings();
        }

        void LoadSettings()
        {
            Manufacturers = SettingsManager.Current.Manufacturers;
        }
        #endregion

        #region iCommands & Actions
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
