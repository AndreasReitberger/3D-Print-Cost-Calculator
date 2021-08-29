using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Models.Documentation;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Models.Update;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;

        public string Version => $"{Strings.Version} {AssemblyManager.Current.Version}";

        bool _useNewUpdateManager;
        public bool UseNewUpdateManager
        {
            get => _useNewUpdateManager;
            set
            {
                if (value == _useNewUpdateManager)
                    return;

                _useNewUpdateManager = value;
                OnPropertyChanged();
            }
        }

        bool _isUpdateCheckRunning;
        public bool IsUpdateCheckRunning
        {
            get => _isUpdateCheckRunning;
            set
            {
                if (value == _isUpdateCheckRunning)
                    return;

                _isUpdateCheckRunning = value;
                OnPropertyChanged();

            }
        }

        bool _updateAvailable;
        public bool UpdateAvailable
        {
            get => _updateAvailable;
            set
            {
                if (value == _updateAvailable)
                    return;

                _updateAvailable = value;
                OnPropertyChanged();
            }
        }

        string _updateText;
        public string UpdateText
        {
            get => _updateText;
            set
            {
                if (value == _updateText)
                    return;

                _updateText = value;
                OnPropertyChanged();
            }
        }

        bool _showUpdaterMessage;
        public bool ShowUpdaterMessage
        {
            get => _showUpdaterMessage;
            set
            {
                if (value == _showUpdaterMessage)
                    return;

                _showUpdaterMessage = value;
                OnPropertyChanged();
            }
        }

        string _updaterMessage;
        public string UpdaterMessage
        {
            get => _updaterMessage;
            set
            {
                if (value == _updaterMessage)
                    return;

                _updaterMessage = value;
                OnPropertyChanged();
            }
        }

        public ICollectionView LibrariesView { get; }

        LibraryInfo _selectedLibraryInfo;
        public LibraryInfo SelectedLibraryInfo
        {
            get => _selectedLibraryInfo;
            set
            {
                if (value == _selectedLibraryInfo)
                    return;

                _selectedLibraryInfo = value;
                OnPropertyChanged();
            }
        }

        public ICollectionView ResourcesView { get; }

        ResourceInfo _selectedResourceInfo;
        //object _dialogCoordinator;

        public ResourceInfo SelectedResourceInfo
        {
            get => _selectedResourceInfo;
            set
            {
                if (value == _selectedResourceInfo)
                    return;

                _selectedResourceInfo = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor
        public AboutViewModel()
        {
            LibrariesView = CollectionViewSource.GetDefaultView(LibraryManager.List);
            LibrariesView.SortDescriptions.Add(new SortDescription(nameof(LibraryInfo.Library), ListSortDirection.Ascending));

            ResourcesView = CollectionViewSource.GetDefaultView(ResourceManager.List);
            ResourcesView.SortDescriptions.Add(new SortDescription(nameof(ResourceInfo.Resource), ListSortDirection.Ascending));

            IsLicenseValid = false;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }
        public AboutViewModel(IDialogCoordinator dialogCoordinator)
        {
            LibrariesView = CollectionViewSource.GetDefaultView(LibraryManager.List);
            LibrariesView.SortDescriptions.Add(new SortDescription(nameof(LibraryInfo.Library), ListSortDirection.Ascending));

            ResourcesView = CollectionViewSource.GetDefaultView(ResourceManager.List);
            ResourcesView.SortDescriptions.Add(new SortDescription(nameof(ResourceInfo.Resource), ListSortDirection.Ascending));

            _dialogCoordinator = dialogCoordinator;

            IsLicenseValid = false;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }

        void LoadSettings()
        {
            UseNewUpdateManager = SettingsManager.Current.Update_UseNewUpdater;
        }
        #endregion

        #region Commands & Actions
        public ICommand CheckForUpdatesCommand
        {
            get { return new RelayCommand(async(p) => await CheckForUpdatesAction()); }
        }

        async Task CheckForUpdatesAction()
        {
            await CheckForUpdates();
        }

        public ICommand OpenWebsiteCommand => new RelayCommand(OpenWebsiteAction);

        static void OpenWebsiteAction(object url)
        {
            try
            {
                Process.Start((string)url);
                logger.Info(string.Format(Strings.EventOpenUri, (string)url));
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand OpenLicenseFolderCommand
        {
            get { return new RelayCommand(p => OpenLicenseFolderAction()); }
        }

        void OpenLicenseFolderAction()
        {
            OpenLicenseFolder();
        }
        #endregion

        #region Methods
        async Task CheckForUpdates()
        {
            UpdateAvailable = false;
            ShowUpdaterMessage = false;

            IsUpdateCheckRunning = true;

            var updater = new Updater();
            updater.UpdateAvailable += Updater_UpdateAvailable;
            updater.NoUpdateAvailable += Updater_NoUpdateAvailable;
            updater.ClientIncompatibleWithNewVersion += Updater_ClientIncompatibleWithNewVersion; ;
            updater.Error += Updater_Error;
            updater.Check();
            
        }

        public void OpenLicenseFolder()
        {
            try
            {
                Process.Start(LibraryManager.GetLicenseLocation());
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        #endregion

        #region Events
        void Updater_UpdateAvailable(object sender, UpdateAvailableArgs e)
        {
            UpdateText = string.Format(Strings.VersionxxIsAvailable, e.Version);

            IsUpdateCheckRunning = false;
            UpdateAvailable = true;
        }

        void Updater_NoUpdateAvailable(object sender, EventArgs e)
        {
            UpdaterMessage = Strings.NoUpdateAvailable;

            IsUpdateCheckRunning = false;
            ShowUpdaterMessage = true;
        }

        void Updater_ClientIncompatibleWithNewVersion(object sender, EventArgs e)
        {
            UpdaterMessage = Strings.YourSystemOSIsIncompatibleWithTheLatestRelease;

            IsUpdateCheckRunning = false;
            ShowUpdaterMessage = true;
        }

        void Updater_Error(object sender, EventArgs e)
        {
            UpdaterMessage = Strings.ErrorCheckingApiGithubComVerifyYourNetworkConnection;

            IsUpdateCheckRunning = false;
            ShowUpdaterMessage = true;
        }
        #endregion
    }
}
