using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Utilities;
using System;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels
{
    public class QuickSettingsDialogViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Properties

        #region General
        int _selectedTabIndex = 0;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (_selectedTabIndex == value) return;
                _selectedTabIndex = value;
                OnPropertyChanged();
            }
        }
        bool _newCalculationWhenCalculate = false;
        public bool NewCalculationWhenCalculate
        {
            get => _newCalculationWhenCalculate;
            set
            {
                if (_newCalculationWhenCalculate == value) return;
                _newCalculationWhenCalculate = value;
                if (!IsLoading)
                    SettingsManager.Current.General_NewCalculationWhenCalculate = value;
                OnPropertyChanged();
            }
        }

        bool _showCalculationResultPopup = true;
        public bool ShowCalculationResultPopup
        {
            get => _showCalculationResultPopup;
            set
            {
                if (_showCalculationResultPopup == value) return;
                _showCalculationResultPopup = value;
                if (!IsLoading)
                    SettingsManager.Current.General_OpenCalculationResultView = value;
                OnPropertyChanged();
            }
        }

        bool _setLoadedCalculationAsSelected = true;
        public bool SetLoadedCalculationAsSelected
        {
            get => _setLoadedCalculationAsSelected;
            set
            {
                if (_setLoadedCalculationAsSelected == value) return;
                _setLoadedCalculationAsSelected = value;
                if (!IsLoading)
                    SettingsManager.Current.General_SetLoadedCalculationAsSelected = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region ModelViewers
        bool _showCameraInfo = false;
        public bool ShowCameraInfo
        {
            get => _showCameraInfo;
            set
            {
                if (_showCameraInfo == value) return;
                if (!IsLoading)
                    SettingsManager.Current.Helix_ShowCameraInfo = value;
                _showCameraInfo = value;
                OnPropertyChanged();
            }
        }

        bool _viewerRotateAroundMouseDownPoint = true;
        public bool ViewerRotateAroundMouseDownPoint
        {
            get => _viewerRotateAroundMouseDownPoint;
            set
            {
                if (_viewerRotateAroundMouseDownPoint == value)
                    return;
                if (!IsLoading)
                    SettingsManager.Current.Helix_RotateAroundMouseDownPoint = value;
                _viewerRotateAroundMouseDownPoint = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion

        #region Constructor, LoadSettings
        public QuickSettingsDialogViewModel(Action<QuickSettingsDialogViewModel> saveCommand, Action<QuickSettingsDialogViewModel> cancelHandler)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            IsLicenseValid = false;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;

        }
        public QuickSettingsDialogViewModel(Action<QuickSettingsDialogViewModel> saveCommand, Action<QuickSettingsDialogViewModel> cancelHandler, IDialogCoordinator dialogCoordinator)
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
            NewCalculationWhenCalculate = SettingsManager.Current.General_NewCalculationWhenCalculate;
            ShowCalculationResultPopup = SettingsManager.Current.General_OpenCalculationResultView;
            SetLoadedCalculationAsSelected = SettingsManager.Current.General_SetLoadedCalculationAsSelected;

            ShowCameraInfo = SettingsManager.Current.Helix_ShowCameraInfo;
            ViewerRotateAroundMouseDownPoint = SettingsManager.Current.Helix_RotateAroundMouseDownPoint;
        }
        #endregion

        #region iCommands & Actions
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
