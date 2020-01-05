using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfFramework.Models.Settings;
using WpfFramework.Utilities;

namespace WpfFramework.ViewModels
{
    public class SettingsUpdateViewModel : ViewModelBase
    {
        #region Variables
        private readonly bool _isLoading;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        private bool _checkForUpdatesAtStartup;
        public bool CheckForUpdatesAtStartup
        {
            get => _checkForUpdatesAtStartup;
            set
            {
                if (value == _checkForUpdatesAtStartup)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.Update_CheckForUpdatesAtStartup = value;

                _checkForUpdatesAtStartup = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor, LoadSettings
        public SettingsUpdateViewModel()
        {
            _isLoading = true;

            LoadSettings();

            _isLoading = false;
        }

        private void LoadSettings()
        {
            CheckForUpdatesAtStartup = SettingsManager.Current.Update_CheckForUpdatesAtStartup;
        }
        #endregion
    }
}
