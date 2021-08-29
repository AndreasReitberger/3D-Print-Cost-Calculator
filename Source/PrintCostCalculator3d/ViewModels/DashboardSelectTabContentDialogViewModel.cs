using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Enums;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels
{
    public class DashboardSelectTabContentDialogViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Properties

        ObservableCollection<string> _printers = new ObservableCollection<string>();
        public ObservableCollection<string> TabContents
        {
            get => _printers;
            set
            {
                if (_printers != value)
                {
                    _printers = value;
                    OnPropertyChanged();
                }
            }
        }

        DashboardTabContentType _tabContent;
        public DashboardTabContentType TabContent
        {
            get => _tabContent;
            set
            {
                if (_tabContent == value) return;
                _tabContent = value;
                OnPropertyChanged();
                
            }
        }
        #endregion

        #region EnumCollections

        #region ContentTypes
        ObservableCollection<DashboardTabContentType> _contentTypes = new ObservableCollection<DashboardTabContentType>(
            Enum.GetValues(typeof(DashboardTabContentType)).Cast<DashboardTabContentType>().ToList()
            );
        public ObservableCollection<DashboardTabContentType> ContentTypes
        {
            get => _contentTypes;
            set
            {
                if (_contentTypes == value) return;
                _contentTypes = value;
                OnPropertyChanged();

            }
        }

        #endregion

        #endregion

        #region Constructor
        public DashboardSelectTabContentDialogViewModel(
            Action<DashboardSelectTabContentDialogViewModel> saveCommand, Action<DashboardSelectTabContentDialogViewModel> cancelHandler)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            IsLicenseValid = false;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;
        }
        public DashboardSelectTabContentDialogViewModel(
            Action<DashboardSelectTabContentDialogViewModel> saveCommand, Action<DashboardSelectTabContentDialogViewModel> cancelHandler, IDialogCoordinator dialogCoordinator)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            IsLicenseValid = false;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;
            _dialogCoordinator = dialogCoordinator;
        }
        void LoadSettings()
        {

        }
        #endregion

        #region iCommands & Actions
        public ICommand SelectedPrinterChangedCommand
        {
            get => new RelayCommand((p) => SelectedPrinterChangedAction(p));
        }
        void SelectedPrinterChangedAction(object p)
        {
            
        }

        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
