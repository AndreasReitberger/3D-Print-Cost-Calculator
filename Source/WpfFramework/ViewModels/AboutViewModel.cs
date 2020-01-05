using WpfFramework.Models.Documentation;
using WpfFramework.Models.Settings;
using WpfFramework.Models.Update;
using WpfFramework.Resources.Localization;
using WpfFramework.Utilities;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Input;
using log4net;

namespace WpfFramework.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Version => $"{Strings.Version} {AssemblyManager.Current.Version}";

        private bool _isUpdateCheckRunning;
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

        private bool _updateAvailable;
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

        private string _updateText;
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

        private bool _showUpdaterMessage;
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

        private string _updaterMessage;
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

        private LibraryInfo _selectedLibraryInfo;
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

        private ResourceInfo _selectedResourceInfo;
        //private object _dialogCoordinator;

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

            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }
        public AboutViewModel(IDialogCoordinator dialogCoordinator)
        {
            LibrariesView = CollectionViewSource.GetDefaultView(LibraryManager.List);
            LibrariesView.SortDescriptions.Add(new SortDescription(nameof(LibraryInfo.Library), ListSortDirection.Ascending));

            ResourcesView = CollectionViewSource.GetDefaultView(ResourceManager.List);
            ResourcesView.SortDescriptions.Add(new SortDescription(nameof(ResourceInfo.Resource), ListSortDirection.Ascending));

            this._dialogCoordinator = dialogCoordinator;

            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }
        #endregion

        #region Commands & Actions
        public ICommand CheckForUpdatesCommand
        {
            get { return new RelayCommand(p => CheckForUpdatesAction()); }
        }

        private void CheckForUpdatesAction()
        {
            CheckForUpdates();
        }

        public ICommand OpenWebsiteCommand => new RelayCommand(OpenWebsiteAction);

        private static void OpenWebsiteAction(object url)
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

        #endregion

        #region Methods
        private void CheckForUpdates()
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
        private void Updater_UpdateAvailable(object sender, UpdateAvailableArgs e)
        {
            UpdateText = string.Format(Strings.VersionxxIsAvailable, e.Version);

            IsUpdateCheckRunning = false;
            UpdateAvailable = true;
        }

        private void Updater_NoUpdateAvailable(object sender, EventArgs e)
        {
            UpdaterMessage = Strings.NoUpdateAvailable;

            IsUpdateCheckRunning = false;
            ShowUpdaterMessage = true;
        }

        private void Updater_ClientIncompatibleWithNewVersion(object sender, EventArgs e)
        {
            UpdaterMessage = Strings.YourSystemOSIsIncompatibleWithTheLatestRelease;

            IsUpdateCheckRunning = false;
            ShowUpdaterMessage = true;
        }

        private void Updater_Error(object sender, EventArgs e)
        {
            UpdaterMessage = Strings.ErrorCheckingApiGithubComVerifyYourNetworkConnection;

            IsUpdateCheckRunning = false;
            ShowUpdaterMessage = true;
        }
        #endregion
    }
}
