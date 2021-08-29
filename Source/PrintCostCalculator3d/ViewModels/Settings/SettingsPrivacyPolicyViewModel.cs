using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels
{
    class SettingsPrivacyPolicyViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
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
            _dialogCoordinator = instance;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;
        }
        void LoadSettings()
        {
            //SettingsManager.Current.RepetierServerPro = new RepetierServerPro();
        }
        #endregion

        #region Methods

        #endregion

        #region iCommands & Actions
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }

        public ICommand BrowseFileCommand
        {
            get { return new RelayCommand(p => BrowseFileAction()); }
        }

        void BrowseFileAction()
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
