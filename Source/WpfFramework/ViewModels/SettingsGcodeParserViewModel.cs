using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;
using System.Windows.Input;
using WpfFramework.Models.Settings;
using WpfFramework.Utilities;

namespace WpfFramework.ViewModels
{
    class SettingsGcodeParserViewModel : ViewModelBase
    {
        #region Properties
        private readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Variables
        private readonly bool _isLoading;

        private bool _PreferValuesInCommentsFromKnownSlicers;
        public bool PreferValuesInCommentsFromKnownSlicers
        {
            get => _PreferValuesInCommentsFromKnownSlicers;
            set
            {
                if (value == _PreferValuesInCommentsFromKnownSlicers)
                    return;
                if (!_isLoading)
                    SettingsManager.Current.GcodeParser_PreferValuesInCommentsFromKnownSlicers = value;

                _PreferValuesInCommentsFromKnownSlicers = value;
                OnPropertyChanged();
            }
        }

        private int _backgroundJobInterval;
        public int BackgroundJobInterval
        {
            get => _backgroundJobInterval;
            set
            {
                if (value == _backgroundJobInterval)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.General_BackgroundJobInterval = value;

                _backgroundJobInterval = value;
                OnPropertyChanged();
            }
        }

        public List<Models.Slicer.Slicer> KnownSlicers
        {
            get => Models.Slicer.Slicer.SupportedSlicers;
        }
        #endregion   

        #region Constructor, LoadSettings
        public SettingsGcodeParserViewModel()
        {
            _isLoading = true;

            LoadSettings();


            _isLoading = false;
        }
        public SettingsGcodeParserViewModel(IDialogCoordinator instance)
        {
            _dialogCoordinator = instance;
            _isLoading = true;

            LoadSettings();

            _isLoading = false;
        }


        private void LoadSettings()
        {
            PreferValuesInCommentsFromKnownSlicers = SettingsManager.Current.GcodeParser_PreferValuesInCommentsFromKnownSlicers;
            //BackgroundJobInterval = SettingsManager.Current.General_BackgroundJobInterval;
        }
        #endregion

        #region ICommands & Actions
        public ICommand HideToVisibleApplicationCommand
        {
            get { return new RelayCommand(p => HideToVisibleApplicationAction()); }
        }

        private void HideToVisibleApplicationAction()
        {
            
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
