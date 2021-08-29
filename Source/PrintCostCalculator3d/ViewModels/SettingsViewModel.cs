using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using PrintCostCalculator3d.Views;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels
{
    class SettingsViewModel : ViewModelBase
    {
        #region Properties
        public ICollectionView SettingsViews { get; set; }

        string _search;
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

        bool _searchNothingFound;
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

        UserControl _settingsContent;
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

        SettingsViewInfo _selectedSettingsView;
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
        SettingsGeneralView _settingsGerneralView;
        SettingsWindowView _settingsWindowView;
        SettingsAppearanceView _settingsApperanceView;
        SettingsLanguageView _settingsLanguageView;
        SettingsUpdateView _settingsUpdateView;
        SettingsSettingsView _settingsSettingsView;
        SettingsCalculationView _settingsCalculationsView;
        SettingsPrintersView _settingsPrintersView;
        SettingsSlicerView _settingsSlicer;
        SettingsGcodeParserView _settingsGcode;
        SettingsEULAView _settingsEULA;
        SettingsEventLoggerView _settingsEventLogger;
        SettingsPrivacyPolicyView _settingsPrivacy;
        #endregion

        #region Contructor, load settings
        public SettingsViewModel()
        { }
        public SettingsViewModel(ApplicationName applicationName)
        {
            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            IsLicenseValid = false;

            ChangeSettingsView(applicationName);
        }
        public SettingsViewModel(SettingsViewName settingsName)
        {
            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            IsLicenseValid = false;

            ChangeSettingsView(settingsName);
        }

        void LoadSettings()
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

        void ClearSearchAction()
        {
            Search = string.Empty;
        }
        public ICommand OpenSettingsLocationCommand
        {
            get { return new RelayCommand(p => OpenSettingsLocationAction()); }
        }

        void OpenSettingsLocationAction()
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
        public void ChangeSettingsView(ApplicationName applicationName)
        {
            // Don't change the view, if the user has filtered the settings...
            if (!string.IsNullOrEmpty(Search))
                return;

            if (Enum.GetNames(typeof(SettingsViewName)).Contains(applicationName.ToString()) && ApplicationName.None.ToString() != applicationName.ToString())
                SelectedSettingsView = SettingsViews.SourceCollection.Cast<SettingsViewInfo>().FirstOrDefault(x => x.Name.ToString() == applicationName.ToString());
            else
                SelectedSettingsView = SettingsViews.SourceCollection.Cast<SettingsViewInfo>().FirstOrDefault(x => x.Name == SettingsViewName.General);
        }
        public void ChangeSettingsView(SettingsViewName settingsName)
        {
            // Don't change the view, if the user has filtered the settings...
            if (!string.IsNullOrEmpty(Search))
                return;

            if (Enum.GetNames(typeof(SettingsViewName)).Contains(settingsName.ToString()))
                SelectedSettingsView = SettingsViews.SourceCollection.Cast<SettingsViewInfo>().FirstOrDefault(x => x.Name.ToString() == settingsName.ToString());
            else
                SelectedSettingsView = SettingsViews.SourceCollection.Cast<SettingsViewInfo>().FirstOrDefault(x => x.Name == SettingsViewName.General);
        }

        void ChangeSettingsContent(SettingsViewInfo settingsViewInfo)
        {
            switch (settingsViewInfo.Name)
            {
                case SettingsViewName.General:
                    if (_settingsGerneralView == null)
                        _settingsGerneralView = new SettingsGeneralView();

                    SettingsContent = _settingsGerneralView;
                    break;
                
                case SettingsViewName.Window:
                if (_settingsWindowView == null)
                    _settingsWindowView = new SettingsWindowView();

                SettingsContent = _settingsWindowView;
                break;
                
                case SettingsViewName.Appearance:
                    if (_settingsApperanceView == null)
                        _settingsApperanceView = new SettingsAppearanceView();

                    SettingsContent = _settingsApperanceView;
                    break;
                    
                case SettingsViewName.Language:
                    if (_settingsLanguageView == null)
                        _settingsLanguageView = new SettingsLanguageView();

                    SettingsContent = _settingsLanguageView;
                    break;
                case SettingsViewName.Update:
                    if (_settingsUpdateView == null)
                        _settingsUpdateView = new SettingsUpdateView();

                    SettingsContent = _settingsUpdateView;
                    break;
                case SettingsViewName.Settings:
                    if (_settingsSettingsView == null)
                        _settingsSettingsView = new SettingsSettingsView();

                    SettingsContent = _settingsSettingsView;
                    break;
                case SettingsViewName.Calculation:
                    if (_settingsCalculationsView == null)
                        _settingsCalculationsView = new SettingsCalculationView();

                    SettingsContent = _settingsCalculationsView;
                    break;
                    
                case SettingsViewName.Printer:
                    if (_settingsPrintersView == null)
                        _settingsPrintersView = new SettingsPrintersView();

                    SettingsContent = _settingsPrintersView;
                    break;
                    
                
                case SettingsViewName.Slicer:
                    if (_settingsSlicer == null)
                        _settingsSlicer = new SettingsSlicerView();

                    SettingsContent = _settingsSlicer;
                    break;
                case SettingsViewName.Gcode:
                    if (_settingsGcode == null)
                        _settingsGcode = new SettingsGcodeParserView();

                    SettingsContent = _settingsGcode;
                    break;
                case SettingsViewName.EULA:
                    if (_settingsEULA == null)
                        _settingsEULA = new SettingsEULAView();

                    SettingsContent = _settingsEULA;
                    break;
                case SettingsViewName.EventLogger:
                    if (_settingsEventLogger == null)
                        _settingsEventLogger = new SettingsEventLoggerView();

                    SettingsContent = _settingsEventLogger;
                    break;
                case SettingsViewName.PrivacyPolicy:
                    if (_settingsPrivacy == null)
                        _settingsPrivacy = new SettingsPrivacyPolicyView();

                    SettingsContent = _settingsPrivacy;
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
