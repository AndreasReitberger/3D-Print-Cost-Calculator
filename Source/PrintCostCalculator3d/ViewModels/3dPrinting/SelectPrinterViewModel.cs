using log4net;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using AndreasReitberger.Models;

namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    class SelectPrinterViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        private ObservableCollection<Printer3d> _printers = new ObservableCollection<Printer3d>();
        public ObservableCollection<Printer3d> Printers
        {
            get => _printers;
            private set
            {
                if (value == _printers)
                    return;

                _printers = value;
                OnPropertyChanged();
            }
        }

        private Printer3d _SelectedPrinter;
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

        private IList _SelectedPrinters = new ArrayList();
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

            LoadSettings();
        }
        public SelectPrinterViewModel(Action<SelectPrinterViewModel> saveCommand, Action<SelectPrinterViewModel> cancelHandler, IDialogCoordinator dialogCoordinator)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            
            this._dialogCoordinator = dialogCoordinator;

            LoadSettings();
        }

        private void LoadSettings()
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
