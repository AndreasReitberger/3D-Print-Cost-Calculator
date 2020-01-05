using WpfFramework.Utilities;
using WpfFramework.Views;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Diagnostics;
using WpfFramework.Models.Settings;
using log4net;
using WpfFramework.Resources.Localization;

namespace WpfFramework.ViewModels
{
    class SettingsViewModel : ViewModelBase
    {
        #region Variables
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        public ICollectionView SettingsViews { get; private set; }

        private string _search;
        public string Search
        {
            get => _search;
            set
            {
                if (value == _search)
                    return;

                _search = value;

                SettingsViews.Refresh();

                // Show note when there was nothing found
                SearchNothingFound = !SettingsViews.Cast<SettingsViewInfo>().Any();

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

        private UserControl _settingsContent;
        public UserControl SettingsContent
        {
            get => _settingsContent;
            set
            {
                if (Equals(value, _settingsContent))
                    return;

                _settingsContent = value;
                OnPropertyChanged();
            }
        }

        private SettingsViewInfo _selectedSettingsView;
        public SettingsViewInfo SelectedSettingsView
        {
            get => _selectedSettingsView;
            set
            {
                if (value == _selectedSettingsView)
                    return;

                if (value != null)
                    ChangeSettingsContent(value);

                _selectedSettingsView = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Views
        private SettingsGeneralView _settingsGerneralView;
        private SettingsWindowView _settingsWindowView;
        private SettingsAppearanceView _settingsApperanceView;
        private SettingsLanguageView _settingsLanguageView;
        private SettingsUpdateView _settingsUpdateView;
        private SettingsSettingsView _settingsSettingsView;
        private SettingsSlicerView _settingsSlicer;
        private SettingsGcodeParserView _settingsGcode;
        private SettingsEventLoggerView _settingsEventLogger;
        #endregion

        #region Contructor, load settings
        public SettingsViewModel()
        { }
        public SettingsViewModel(ApplicationViewManager.Name applicationName)
        {
            LoadSettings();

            ChangeSettingsView(applicationName);
        }

        private void LoadSettings()
        {
            SettingsViews = new CollectionViewSource { Source = SettingsViewManager.List }.View;
            SettingsViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(SettingsViewInfo.TranslatedGroup)));
            SettingsViews.SortDescriptions.Add(new SortDescription(nameof(SettingsViewInfo.Name), ListSortDirection.Ascending));
            SettingsViews.Filter = o =>
            {
                if (string.IsNullOrEmpty(Search))
                    return true;

                if (!(o is SettingsViewInfo info))
                    return false;

                var regex = new Regex(@" |-");

                var search = regex.Replace(Search, "");

                // Search by TranslatedName and Name
                return regex.Replace(info.TranslatedName, "").IndexOf(search, StringComparison.OrdinalIgnoreCase) > -1 || (regex.Replace(info.Name.ToString(), "").IndexOf(search, StringComparison.OrdinalIgnoreCase) > -1);
            };
        }
        #endregion

        #region ICommands & Actions
        public ICommand ClearSearchCommand
        {
            get { return new RelayCommand(p => ClearSearchAction()); }
        }

        private void ClearSearchAction()
        {
            Search = string.Empty;
        }
        public ICommand OpenSettingsLocationCommand
        {
            get { return new RelayCommand(p => OpenSettingsLocationAction()); }
        }

        private void OpenSettingsLocationAction()
        {
            try
            {
                Process.Start(SettingsManager.GetSettingsLocation());
                logger.InfoFormat(Strings.EventOpenUri, SettingsManager.GetSettingsLocation());
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
            }
        }
        #endregion

        #region Methods
        public void ChangeSettingsView(ApplicationViewManager.Name applicationName)
        {
            // Don't change the view, if the user has filtered the settings...
            if (!string.IsNullOrEmpty(Search))
                return;

            if (Enum.GetNames(typeof(SettingsViewManager.Name)).Contains(applicationName.ToString()) && ApplicationViewManager.Name.None.ToString() != applicationName.ToString())
                SelectedSettingsView = SettingsViews.SourceCollection.Cast<SettingsViewInfo>().FirstOrDefault(x => x.Name.ToString() == applicationName.ToString());
            else
                SelectedSettingsView = SettingsViews.SourceCollection.Cast<SettingsViewInfo>().FirstOrDefault(x => x.Name == SettingsViewManager.Name.General);
        }

        private void ChangeSettingsContent(SettingsViewInfo settingsViewInfo)
        {
            switch (settingsViewInfo.Name)
            {
                case SettingsViewManager.Name.General:
                    if (_settingsGerneralView == null)
                        _settingsGerneralView = new SettingsGeneralView();

                    SettingsContent = _settingsGerneralView;
                    break;
                
                case SettingsViewManager.Name.Window:
                if (_settingsWindowView == null)
                    _settingsWindowView = new SettingsWindowView();

                SettingsContent = _settingsWindowView;
                break;
                
                case SettingsViewManager.Name.Appearance:
                    if (_settingsApperanceView == null)
                        _settingsApperanceView = new SettingsAppearanceView();

                    SettingsContent = _settingsApperanceView;
                    break;
                    
                case SettingsViewManager.Name.Language:
                    if (_settingsLanguageView == null)
                        _settingsLanguageView = new SettingsLanguageView();

                    SettingsContent = _settingsLanguageView;
                    break;
                case SettingsViewManager.Name.Update:
                    if (_settingsUpdateView == null)
                        _settingsUpdateView = new SettingsUpdateView();

                    SettingsContent = _settingsUpdateView;
                    break;
                case SettingsViewManager.Name.Settings:
                    if (_settingsSettingsView == null)
                        _settingsSettingsView = new SettingsSettingsView();

                    SettingsContent = _settingsSettingsView;
                    break;
                    
                case SettingsViewManager.Name.Slicer:
                    if (_settingsSlicer == null)
                        _settingsSlicer = new SettingsSlicerView();

                    SettingsContent = _settingsSlicer;
                    break;
                case SettingsViewManager.Name.Gcode:
                    if (_settingsGcode == null)
                        _settingsGcode = new SettingsGcodeParserView();

                    SettingsContent = _settingsGcode;
                    break;
                case SettingsViewManager.Name.EventLogger:
                    if (_settingsEventLogger == null)
                        _settingsEventLogger = new SettingsEventLoggerView();

                    SettingsContent = _settingsEventLogger;
                    break;               
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
