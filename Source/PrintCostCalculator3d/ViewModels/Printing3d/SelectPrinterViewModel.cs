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
    class SelectPrinterViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Properties
        ObservableCollection<Printer3d> _printers = new ObservableCollection<Printer3d>();
        public ObservableCollection<Printer3d> Printers
        {
            get => _printers;
            set
            {
                if (value == _printers)
                    return;

                _printers = value;
                OnPropertyChanged();
            }
        }

        Printer3d _SelectedPrinter;
        public Printer3d SelectedPrinter
        {
            get => _SelectedPrinter;
            set
            {
                if (value == _SelectedPrinter)
                    return;

                _SelectedPrinter = value;
                OnPropertyChanged();
            }
        }

        IList _SelectedPrinters = new ArrayList();
        public IList SelectedPrinters
        {
            get => _SelectedPrinters;
            set
            {
                if (value == _SelectedPrinters)
                    return;

                _SelectedPrinters = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor, LoadSettings
        public SelectPrinterViewModel(Action<SelectPrinterViewModel> saveCommand, Action<SelectPrinterViewModel> cancelHandler)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            IsLicenseValid = false;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;
        }
        public SelectPrinterViewModel(Action<SelectPrinterViewModel> saveCommand, Action<SelectPrinterViewModel> cancelHandler, IDialogCoordinator dialogCoordinator)
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
            Printers = SettingsManager.Current.Printers;
        }
        #endregion

        #region iCommands & Actions
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
