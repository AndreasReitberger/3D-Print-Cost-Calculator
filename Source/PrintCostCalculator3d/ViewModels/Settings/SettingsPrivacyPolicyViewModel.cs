using log4net;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;

namespace PrintCostCalculator3d.ViewModels
{
    class SettingsPrivacyPolicyViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly bool _isLoading;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        public bool EnableLogging
        {
            get { return SettingsManager.Current.EventLogger_EnableLogging; }
            set
            {
                if (SettingsManager.Current.EventLogger_EnableLogging != value)
                {
                    SettingsManager.Current.EventLogger_EnableLogging = value;
                    OnPropertyChanged();
                }
            }
        }
        public int StoredEvents
        {
            get { return SettingsManager.Current.EventLogger_AmountSavedLogs; }
            set
            {
                if (SettingsManager.Current.EventLogger_AmountSavedLogs != value)
                {
                    SettingsManager.Current.EventLogger_AmountSavedLogs = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Constructor, Load Settings
        public SettingsPrivacyPolicyViewModel(IDialogCoordinator instance)
        {
            _isLoading = true;

            _dialogCoordinator = instance;
            LoadSettings();
            _isLoading = false;
        }
        private void LoadSettings()
        {
            //SettingsManager.Current.RepetierServerPro = new RepetierServerPro();
        }
        #endregion

        #region Private Methods

        #endregion

        #region iCommands & Actions
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }

        public ICommand BrowseFileCommand
        {
            get { return new RelayCommand(p => BrowseFileAction()); }
        }

        private void BrowseFileAction()
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = StaticStrings.FilterGCodeFile
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

            }

        }

        #endregion

        #region Methods
        public void OnViewVisible()
        {

        }

        public void OnViewHide()
        {

        }
        #endregion
    }
}
