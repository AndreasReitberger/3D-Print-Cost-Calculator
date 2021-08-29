using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Utilities;

namespace PrintCostCalculator3d.ViewModels
{
    public class SettingsUpdateViewModel : ViewModelBase
    {
        #region Properties
        bool _checkForUpdatesAtStartup;
        public bool CheckForUpdatesAtStartup
        {
            get => _checkForUpdatesAtStartup;
            set
            {
                if (value == _checkForUpdatesAtStartup)
                    return;

                if (!IsLoading)
                    SettingsManager.Current.Update_CheckForUpdatesAtStartup = value;

                _checkForUpdatesAtStartup = value;
                OnPropertyChanged();
            }
        }

        bool _useNewUpdater = true;
        public bool UseNewUpdater
        {
            get => _useNewUpdater;
            set
            {
                if (value == _useNewUpdater)
                    return;

                if (!IsLoading)
                    SettingsManager.Current.Update_UseNewUpdater = value;

                _useNewUpdater = value;
                OnPropertyChanged();
            }
        }

        bool _includeAlphaVersions = false;
        public bool IncludeAlphaVersions
        {
            get => _includeAlphaVersions;
            set
            {
                if (value == _includeAlphaVersions)
                    return;

                if (!IsLoading)
                    SettingsManager.Current.Update_IncludeBetaVersions = value;

                _includeAlphaVersions = value;
                OnPropertyChanged();
            }
        }

        bool _includeBetaVersions = true;
        public bool IncludeBetaVersions
        {
            get => _includeBetaVersions;
            set
            {
                if (value == _includeBetaVersions)
                    return;

                if (!IsLoading)
                    SettingsManager.Current.Update_IncludeBetaVersions = value;

                _includeBetaVersions = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor, LoadSettings
        public SettingsUpdateViewModel()
        {
            IsLoading = true;
            LoadSettings();
            IsLoading = false;
        }

        void LoadSettings()
        {
            CheckForUpdatesAtStartup = SettingsManager.Current.Update_CheckForUpdatesAtStartup;
            UseNewUpdater = SettingsManager.Current.Update_UseNewUpdater;
            IncludeAlphaVersions = SettingsManager.Current.Update_IncludeAlphaVersions;
            IncludeBetaVersions = SettingsManager.Current.Update_IncludeBetaVersions;
        }
        #endregion
    }
}
