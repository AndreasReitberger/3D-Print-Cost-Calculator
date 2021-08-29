using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintCostCalculator3d.ViewModels
{
    class SettingsWindowViewModel : ViewModelBase
    {
        #region Properties
        //readonly bool _isLoading;

        bool _minimizeInsteadOfTerminating;
        public bool MinimizeInsteadOfTerminating
        {
            get => _minimizeInsteadOfTerminating;
            set
            {
                if (value == _minimizeInsteadOfTerminating)
                    return;

                if (!IsLoading)
                    SettingsManager.Current.Window_MinimizeInsteadOfTerminating = value;

                _minimizeInsteadOfTerminating = value;
                OnPropertyChanged();
            }
        }

        bool _minimizeToTrayInsteadOfTaskbar;
        public bool MinimizeToTrayInsteadOfTaskbar
        {
            get => _minimizeToTrayInsteadOfTaskbar;
            set
            {
                if (value == _minimizeToTrayInsteadOfTaskbar)
                    return;

                if (!IsLoading)
                    SettingsManager.Current.Window_MinimizeToTrayInsteadOfTaskbar = value;

                _minimizeToTrayInsteadOfTaskbar = value;
                OnPropertyChanged();
            }
        }

        bool _confirmClose;
        public bool ConfirmClose
        {
            get => _confirmClose;
            set
            {
                if (value == _confirmClose)
                    return;

                if (!IsLoading)
                    SettingsManager.Current.Window_ConfirmClose = value;

                OnPropertyChanged();
                _confirmClose = value;
            }
        }

        bool _multipleInstances = GlobalStaticConfiguration.Window_AllowMultipleInstances;
        public bool MultipleInstances
        {
            get => _multipleInstances;
            set
            {
                if (value == _multipleInstances)
                    return;

                if (!IsLoading)
                    SettingsManager.Current.Window_MultipleInstances = value;
                
                OnPropertyChanged();
                _multipleInstances = value;
            }
        }

        bool _alwaysShowIconInTray;
        public bool AlwaysShowIconInTray
        {
            get => _alwaysShowIconInTray;
            set
            {
                if (value == _alwaysShowIconInTray)
                    return;

                if (!IsLoading)
                    SettingsManager.Current.TrayIcon_AlwaysShowIcon = value;

                _alwaysShowIconInTray = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor, LoadSettings
        public SettingsWindowViewModel()
        {
            IsLoading = true;
            LoadSettings();
            IsLoading = false;
        }

        void LoadSettings()
        {
            AlwaysShowIconInTray = SettingsManager.Current.TrayIcon_AlwaysShowIcon;
            MinimizeInsteadOfTerminating = SettingsManager.Current.Window_MinimizeInsteadOfTerminating;
            ConfirmClose = SettingsManager.Current.Window_ConfirmClose;
            MultipleInstances = SettingsManager.Current.Window_MultipleInstances;
            MinimizeToTrayInsteadOfTaskbar = SettingsManager.Current.Window_MinimizeToTrayInsteadOfTaskbar;
        }
        #endregion
    }
}
