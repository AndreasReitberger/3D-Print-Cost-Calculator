using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Utilities;
using PrintCostCalculator3d.Views;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.ViewModels;
using log4net;
using PrintCostCalculator3d.Models;
using PrintCostCalculator3d.Models.Update;
using GalaSoft.MvvmLight.Messaging;
using System.Threading.Tasks;
using PrintCostCalculator3d.Models.Documentation;
using PrintCostCalculator3d.Models.nUpdate;
using HelixToolkit.Wpf.SharpDX.Utilities;

namespace PrintCostCalculator3d
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {

        #region PropertyChangedEventHandler
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Variables
        private Logger Logger = Logger.Instance;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static NVOptimusEnabler nvEnabler = new NVOptimusEnabler();
        #endregion

        #region Properties
        private readonly bool _isLoading;
        private bool _isInTray;
        private bool _closeApplication;

        // Indicates a restart message, when settings changed
        private string _cultureCode;

        private bool _overwriteCurrency = false;

        private ObservableCollection<ApplicationViewInfo> _Applications;
        public ICollectionView Applications { get; private set; }

        public bool IsCheckingLicenseInfo
        {
            get => _isCheckingLicenseInfo;
            set
            {
                if (value == _isCheckingLicenseInfo)
                    return;
                _isCheckingLicenseInfo = value;
                OnPropertyChanged();
            }
        }
        private bool _isCheckingLicenseInfo = false;
        public bool ExpandApplicationView
        {
            get => _expandApplicationView;
            set
            {
                if (value == _expandApplicationView)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.ExpandApplicationView = value;

                if (!value)
                    ClearSearchOnApplicationListMinimize();

                _expandApplicationView = value;
                OnPropertyChanged();
            }
        }
        private bool _expandApplicationView;

        private bool _isApplicationListOpen;
        public bool IsApplicationListOpen
        {
            get => _isApplicationListOpen;
            set
            {
                if (value == _isApplicationListOpen)
                    return;

                if (!value)
                    ClearSearchOnApplicationListMinimize();

                _isApplicationListOpen = value;
                OnPropertyChanged();
            }
        }

        private bool _isTextBoxSearchFocused;
        public bool IsTextBoxSearchFocused
        {
            get => _isTextBoxSearchFocused;
            set
            {
                if (value == _isTextBoxSearchFocused)
                    return;

                if (!value)
                    ClearSearchOnApplicationListMinimize();

                _isTextBoxSearchFocused = value;
                OnPropertyChanged();
            }
        }

        private bool _isMouseOverApplicationList;
        public bool IsMouseOverApplicationList
        {
            get => _isMouseOverApplicationList;
            set
            {
                if (value == _isMouseOverApplicationList)
                    return;

                if (!value)
                    ClearSearchOnApplicationListMinimize();

                _isMouseOverApplicationList = value;
                OnPropertyChanged();
            }
        }

        private ApplicationViewInfo _selectedApplication;
        public ApplicationViewInfo SelectedApplication
        {
            get => _selectedApplication;
            set
            {
                if (value == _selectedApplication)
                    return;

                if (value != null)
                    ChangeApplicationView(value.Name);

                _selectedApplication = value;
                OnPropertyChanged();
            }
        }

        private ApplicationName _filterLastViewName;
        private int? _filterLastCount;

        private string _search = string.Empty;
        public string Search
        {
            get => _search;
            set
            {
                if (value == _search)
                    return;

                _search = value;

                if (SelectedApplication != null)
                    _filterLastViewName = SelectedApplication.Name;

                Applications.Refresh();

                var sourceCollection = Applications.SourceCollection.Cast<ApplicationViewInfo>();
                var filteredCollection = Applications.Cast<ApplicationViewInfo>();

                var sourceInfos = sourceCollection as ApplicationViewInfo[] ?? sourceCollection.ToArray();
                var filteredInfos = filteredCollection as ApplicationViewInfo[] ?? filteredCollection.ToArray();

                if (_filterLastCount == null)
                    _filterLastCount = sourceInfos.Length;

                SelectedApplication = _filterLastCount > filteredInfos.Length ? filteredInfos.FirstOrDefault() : sourceInfos.FirstOrDefault(x => x.Name == _filterLastViewName);

                _filterLastCount = filteredInfos.Length;

                // Show note when there was nothing found
                SearchNothingFound = filteredInfos.Length == 0;

                OnPropertyChanged();
            }
        }

        private bool _searchNothingFound;
        public bool SearchNothingFound
        {
            get => _searchNothingFound;
            set
            {
                if (value == _searchNothingFound)
                    return;

                _searchNothingFound = value;
                OnPropertyChanged();
            }
        }

        private bool _isUpdateAvailable;
        public bool IsUpdateAvailable
        {
            get => _isUpdateAvailable;
            set
            {
                if (value == _isUpdateAvailable)
                    return;

                _isUpdateAvailable = value;
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

        #endregion

        #region Constructor, window load and close events
        public MainWindow()
        {
            _isLoading = true;

            InitializeComponent();

            DataContext = this;

            // Load appearance
            AppearanceManager.Load();

            // Transparency
            if (SettingsManager.Current.Appearance_EnableTransparency)
            {
                AllowsTransparency = true;
                Opacity = SettingsManager.Current.Appearance_Opacity;

                Models.Settings.ConfigurationManager.Current.IsTransparencyEnabled = true;
            }

            LoadApplicationList();

            // Load settings
            ExpandApplicationView = SettingsManager.Current.ExpandApplicationView;

            // Register some events
            EventSystem.RedirectToApplicationEvent += EventSystem_RedirectToApplicationEvent;
            EventSystem.RedirectToSettingsEvent += EventSystem_RedirectToSettingsEvent;
            SettingsManager.Current.PropertyChanged += SettingsManager_PropertyChanged;

            // Load settings
            ExpandApplicationView = SettingsManager.Current.ExpandApplicationView;
            //ChangeApplicationView(ApplicationViewManager.Name.PrintJobs, true);
            //ChangeApplicationView(ApplicationViewManager.Name.MassSearchAndReplace, true);

            // Set windows title if admin
            if (ConfigurationManager.Current.IsAdmin)
            {
                Title = $"[{"Administrator"}] {Title}";
                logger.Debug(string.Format(Strings.EventUserIsAdministratorFormated, Environment.UserName));
            }
            _isLoading = false;
            logger.Info(Strings.EventApplicationStartup);
        }

        // Hide window after it shows up... not nice, but otherwise the hotkeys do not work
        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (ConfigurationManager.Current.ShowSettingsResetNoteOnStartup)
            {
                var settings = AppearanceManager.MetroDialog;
                settings.AffirmativeButtonText = Strings.OK;

                ConfigurationManager.Current.FixAirspace = true;

                await this.ShowMessageAsync(Strings.SettingsReseted, Strings.SettingsFileNotFoundOrCorrupted, MessageDialogStyle.Affirmative, settings);

                ConfigurationManager.Current.FixAirspace = false;
            }


            // Search for updates...
            if (SettingsManager.Current.Update_CheckForUpdatesAtStartup)
            {
                CheckForUpdates();
            }
            ChangeApplicationView(SelectedApplication.Name, true);

        }
        private void BringWindowToFront()
        {
            if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;

            Activate();
        }

        #endregion

        #region Update check

        private void CheckForUpdates()
        {
            var updater = new Updater();

            //var nUpdater = new nUpdateManager();
            //nUpdater.ShowUpdateUi();
            updater.UpdateAvailable += Updater_UpdateAvailable;
            updater.Error += Updater_Error;
            updater.Check();
        }

        private static void Updater_Error(object sender, EventArgs e)
        {
            //logger.ErrorFormat(Strings.EventUpdateErrorFormated, e.ToString());
            //  Log
        }

        private void Updater_UpdateAvailable(object sender, UpdateAvailableArgs e)
        {
            UpdateText = string.Format(Strings.VersionxxIsAvailable, e.Version);
            IsUpdateAvailable = true;
        }

        #endregion

        #region ICommands & Actions
        public ICommand OpenWebsiteCommand => new RelayCommand(OpenWebsiteAction);

        private static void OpenWebsiteAction(object url)
        {
            try
            {
                Process.Start((string)url);
                logger.InfoFormat(Strings.EventOpenUri, (string)url);
            }
            catch(Exception exc)
            {
                logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
            }
        }
        public ICommand CloseApplicationCommand
        {
            get { return new RelayCommand(p => CloseApplicationAction()); }
        }

        private void CloseApplicationAction()
        {
            _closeApplication = true;
            Close();
        }

        private void RestartApplication(bool closeApplication = true)
        {
            try
            {
                new Process
                {
                    StartInfo =
                {
                    FileName = Models.Settings.ConfigurationManager.Current.ApplicationFullName,
                    Arguments = $"--restart-pid:{Process.GetCurrentProcess().Id}"
                }
                }.Start();

                if (!closeApplication)
                    return;

                _closeApplication = true;
                Close();
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
            }
        }

        public ICommand ApplicationListMouseEnterCommand
        {
            get { return new RelayCommand(p => ApplicationListMouseEnterAction()); }
        }

        private void ApplicationListMouseEnterAction()
        {
            IsMouseOverApplicationList = true;
        }

        public ICommand ApplicationListMouseLeaveCommand
        {
            get { return new RelayCommand(p => ApplicationListMouseLeaveAction()); }
        }

        private void ApplicationListMouseLeaveAction()
        {
            // Don't minmize the list, if the user has accidently moved the mouse while searching
            if (!IsTextBoxSearchFocused)
                IsApplicationListOpen = false;

            IsMouseOverApplicationList = false;
        }
        public ICommand OpenDocsCommand
        {
            get => new RelayCommand(p => OpenDocsAction());
        }
        private void OpenDocsAction()
        {
            try
            {
                Process.Start(GlobalStaticConfiguration.documentationUri);
                logger.Info(string.Format(Strings.EventOpenUri, GlobalStaticConfiguration.documentationUri));
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        public ICommand OpenDocumentationCommand
        {
            get { return new RelayCommand(p => OpenDocumentationAction()); }
        }

        private void OpenDocumentationAction()
        {
            DocumentationManager.OpenDocumentation(ShowSettingsView ? DocumentationIdentifier.Default : DocumentationManager.GetIdentifierByAppliactionName(SelectedApplication.Name));
        }
       

        public ICommand OpenApplicationListCommand
        {
            get { return new RelayCommand(p => OpenApplicationListAction()); }
        }
        private void OpenApplicationListAction()
        {
            IsApplicationListOpen = true;
            TextBoxSearch.Focus();
        }
        
        public ICommand OpenSettingsCommand
        {
            get { return new RelayCommand(p => OpenSettingsAction()); }
        }
        private void OpenSettingsAction()
        {
            OpenSettings();
        }
       
        public ICommand CloseSettingsCommand
        {
            get { return new RelayCommand(p => CloseSettingsAction()); }
        }
        private void CloseSettingsAction()
        {
            CloseSettings();
        }

        public ICommand TextBoxSearchGotKeyboardFocusCommand
        {
            get { return new RelayCommand(p => TextBoxSearchGotKeyboardFocusAction()); }
        }

        private void TextBoxSearchGotKeyboardFocusAction()
        {
            IsTextBoxSearchFocused = true;
        }

        public ICommand TextBoxSearchLostKeyboardFocusCommand
        {
            get { return new RelayCommand(p => TextBoxSearchLostKeyboardFocusAction()); }
        }

        private void TextBoxSearchLostKeyboardFocusAction()
        {
            if (!IsMouseOverApplicationList)
                IsApplicationListOpen = false;

            IsTextBoxSearchFocused = false;
        }

        public ICommand ClearSearchCommand
        {
            get { return new RelayCommand(p => ClearSearchAction()); }
        }
        private void ClearSearchAction()
        {
            Search = string.Empty;
        }
       public ICommand ShowWindowCommand
        {
            get { return new RelayCommand(p => ShowWindowAction()); }
        }
        private void ShowWindowAction()
        {
            if (!IsActive)
                BringWindowToFront();
        }
        #endregion

        #region Events
        private void SettingsManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }
        private void LoadApplicationList()
        {
            // Need to add items here... if in SettingsInfo/Constructor --> same item will appear multiple times...
            
            if (_Applications == null || _Applications.Count == 0)
                _Applications = new ObservableCollection<ApplicationViewInfo>(ApplicationViewManager.GetList());
                
            if (SettingsManager.Current.General_ApplicationList.Count == 0)
                SettingsManager.Current.General_ApplicationList = new ObservableCollection<ApplicationViewInfo>(ApplicationViewManager.GetList());

            Applications = new CollectionViewSource { Source = _Applications }.View;

            Applications.SortDescriptions.Add(new SortDescription(nameof(ApplicationViewInfo.Name), ListSortDirection.Ascending)); // Always have the same order, even if it is translated...
            Applications.Filter = o =>
            {
                if (!(o is ApplicationViewInfo info))
                    return false;

                if (string.IsNullOrEmpty(Search))
                    return info.IsVisible;

                var regex = new Regex(@" |-");

                var search = regex.Replace(Search, "");

                // Search by TranslatedName and Name
                return info.IsVisible && (regex.Replace(ApplicationViewManager.GetTranslatedNameByName(info.Name), "").IndexOf(search, StringComparison.OrdinalIgnoreCase) > -1 || regex.Replace(info.Name.ToString(), "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
            };

            _Applications.CollectionChanged += (sender, args) => Applications.Refresh();

            SettingsManager.Current.General_ApplicationList.CollectionChanged += (sender, args) => Applications.Refresh();

            // Get application from settings
            SelectedApplication = Applications.SourceCollection.Cast<ApplicationViewInfo>().FirstOrDefault(x => x.Name == SettingsManager.Current.General_DefaultApplicationViewName);

            // Scroll into view
            if (SelectedApplication != null)
                ListViewApplication.ScrollIntoView(SelectedApplication);
        }
        private async void MetroWindowMain_Closing(object sender, CancelEventArgs e)
        {
            // Force restart (if user has reset the settings or import them)
            if (SettingsManager.ForceRestart)
            {
                RestartApplication(false);

                _closeApplication = true;
            }

            // Hide the application to tray
            if (!_closeApplication && (SettingsManager.Current.Window_MinimizeInsteadOfTerminating && WindowState != WindowState.Minimized))
            {
                e.Cancel = true;

                WindowState = WindowState.Minimized;

                return;
            }

            // Confirm close
            if (!_closeApplication && SettingsManager.Current.Window_ConfirmClose)
            {
                e.Cancel = true;

                // If the window is minimized, bring it to front
                if (WindowState == WindowState.Minimized)
                    BringWindowToFront();

                var settings = AppearanceManager.MetroDialog;

                settings.AffirmativeButtonText = Strings.Close;
                settings.NegativeButtonText = Strings.Cancel;
                settings.DefaultButtonFocus = MessageDialogResult.Affirmative;

                // Fix airspace issues
                Models.Settings.ConfigurationManager.Current.FixAirspace = true;

                var result = await this.ShowMessageAsync(Strings.DialogConfirmCloseHeadline,
                    Strings.DialogConfirmCloseContent, 
                    MessageDialogStyle.AffirmativeAndNegative, settings);

                Models.Settings.ConfigurationManager.Current.FixAirspace = false;

                if (result != MessageDialogResult.Affirmative)
                    return;

                _closeApplication = true;
                Close();

                return;
            }

            // Dispose the notify icon to prevent errors
            //_notifyIcon?.Dispose();
        }

        private void MetroWindowMain_StateChanged(object sender, EventArgs e)
        {
            if (WindowState != WindowState.Minimized)
                return;
            /*
            if (SettingsManager.Current.Window_MinimizeToTrayInsteadOfTaskbar)
                HideWindowToTray();
                */
        }
        private void ClearSearchOnApplicationListMinimize()
        {
            if (ExpandApplicationView)
                return;

            if (IsApplicationListOpen && IsTextBoxSearchFocused)
                return;

            if (IsApplicationListOpen && IsMouseOverApplicationList)
                return;

            Search = string.Empty;

            // Scroll into view
            ListViewApplication.ScrollIntoView(SelectedApplication);
        }

        #endregion

        #region Settings View
        private SettingsView _settingsView;

        private bool _showSettingsView;
        public bool ShowSettingsView
        {
            get => _showSettingsView;
            set
            {


                if (value == _showSettingsView)
                    return;

                _showSettingsView = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Application Views
        private Views._3dPrinting._3dPrintingMaterialView _materialView;
        private Views._3dPrinting._3dPrintingPrinterView _printerView;
        private Views._3dPrinting._3dPrintingCalculationView _printCalcView;
        private LogWatcherView _logWatcherView;

        private ApplicationName? _currentApplicationViewName;

        private void ChangeApplicationView(ApplicationName name, bool refresh = false)
        {
            if (!refresh && _currentApplicationViewName == name)
                return;

            switch (_currentApplicationViewName)
            {
                
            }

            // Create new view / start some functions
            switch (name)
            {
                case ApplicationName._3dPrintingMaterial:
                    if (_materialView == null)
                        _materialView = new Views._3dPrinting._3dPrintingMaterialView();
                    else
                        _materialView.OnViewVisible();

                    ContentControlApplication.Content = _materialView;
                    break;
                case ApplicationName._3dPrintingPrinter:
                    if (_printerView == null)
                        _printerView = new Views._3dPrinting._3dPrintingPrinterView();
                    else
                        _printerView.OnViewVisible();

                    ContentControlApplication.Content = _printerView;
                    break;
                case ApplicationName._3dPrintingCalcualtion:
                    if (_printCalcView == null)
                        _printCalcView = new Views._3dPrinting._3dPrintingCalculationView();
                    else
                        _printCalcView.OnViewVisible();

                    ContentControlApplication.Content = _printCalcView;
                    break;
                case ApplicationName.EventLog:
                    if (_logWatcherView == null)
                    {
                        _logWatcherView = new LogWatcherView();
                        _logWatcherView.OnViewVisible();
                    }
                    else
                        _logWatcherView.OnViewVisible();

                    ContentControlApplication.Content = _logWatcherView;
                    break;
                case ApplicationName.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(name), name, null);
            }

            _currentApplicationViewName = name;
        }


        #endregion

        #region Bugfixes
        private void ScrollViewer_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

        #region Settings
        private void OpenSettings()
        {
            // Save current language code
            if (string.IsNullOrEmpty(_cultureCode))
                _cultureCode = SettingsManager.Current.Localization_CultureCode;
            _overwriteCurrency = SettingsManager.Current.General_OverwriteCurrencySymbol;
            // Init settings view
            if (_settingsView == null)
            {
                _settingsView = new SettingsView(SelectedApplication.Name);
                ContentControlSettings.Content = _settingsView;
            }
            else // Change view
            {
                _settingsView.ChangeSettingsView(SelectedApplication.Name);
                _settingsView.Refresh();
            }

            // Show the view (this will hide other content)
            ShowSettingsView = true;

            // Bring window to front
            ShowWindowAction();

        }
        private void OpenSettings(SettingsViewName name)
        {
            // Save current language code
            if (string.IsNullOrEmpty(_cultureCode))
                _cultureCode = SettingsManager.Current.Localization_CultureCode;
            _overwriteCurrency = SettingsManager.Current.General_OverwriteCurrencySymbol;
            // Init settings view
            if (_settingsView == null)
            {
                _settingsView = new SettingsView(name);
                ContentControlSettings.Content = _settingsView;
            }
            else // Change view
            {
                _settingsView.ChangeSettingsView(name);
                _settingsView.Refresh();
            }

            // Show the view (this will hide other content)
            ShowSettingsView = true;

            // Bring window to front
            ShowWindowAction();

        }

        private void EventSystem_RedirectToApplicationEvent(object sender, EventArgs e)
        {
            if (!(e is EventSystemRedirectApplicationArgs data))
                return;

            // Change view
            SelectedApplication = Applications.SourceCollection.Cast<ApplicationViewInfo>().FirstOrDefault(x => x.Name == data.Application);

            // Crate a new tab / perform action
            switch (data.Application)
            {
                /*
                case ApplicationViewManager.Name.IPScanner:
                    _ipScannerHostView.AddTab(data.Args);
                    break;
                    */
                case ApplicationName.EventLog:
                    break;
                case ApplicationName._3dPrintingCalcualtion:
                    break;
                case ApplicationName._3dPrintingMaterial:
                    break;
                case ApplicationName._3dPrintingPrinter:
                    break;
                case ApplicationName.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private void EventSystem_RedirectToSettingsEvent(object sender, EventArgs e)
        {
            if (!(e is EventSystemRedirectSettingsArgs data))
                OpenSettings();
            else
                OpenSettings(data.Setting);
        }

        private async void CloseSettings()
        {
            ShowSettingsView = false;

            // Enable/disable tray iconFC

            // Ask the user to restart (if he has changed the language)
            if (_cultureCode != SettingsManager.Current.Localization_CultureCode
                || _overwriteCurrency != SettingsManager.Current.General_OverwriteCurrencySymbol
                || AllowsTransparency != SettingsManager.Current.Appearance_EnableTransparency)
            {
                ShowWindowAction();

                var settings = AppearanceManager.MetroDialog;

                settings.AffirmativeButtonText = Strings.RestartNow;
                settings.NegativeButtonText = Strings.RestartLater;
                settings.DefaultButtonFocus = MessageDialogResult.Affirmative;

                Models.Settings.ConfigurationManager.Current.FixAirspace = true;

                if (await this.ShowMessageAsync(Strings.DialogRestartRequiredHeadline, 
                    Strings.DialogRestartRequiredContent, MessageDialogStyle.AffirmativeAndNegative, settings) == MessageDialogResult.Affirmative)
                {
                    RestartApplication();
                    return;
                }

                Models.Settings.ConfigurationManager.Current.FixAirspace = false;
            }

            // Change the transparency
            if (AllowsTransparency != SettingsManager.Current.Appearance_EnableTransparency || (Opacity != SettingsManager.Current.Appearance_Opacity))
            {
                if (!AllowsTransparency || !SettingsManager.Current.Appearance_EnableTransparency)
                    Opacity = 1;
                else
                    Opacity = SettingsManager.Current.Appearance_Opacity;
            }

            // Save the settings
            if (SettingsManager.Current.SettingsChanged)
                SettingsManager.Save();

            // Refresh the view
            ChangeApplicationView(SelectedApplication.Name, true);
        }
        #endregion

        #region Window helper
        // Move the window when the user hold the title...
        private void HeaderBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
        #endregion
    }
}
