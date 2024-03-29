﻿using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Diagnostics;

namespace PrintCostCalculator3d.ViewModels
{
    public class SettingsLanguageViewModel : ViewModelBase
    {
        #region Variables
        //readonly bool _isLoading;
        #endregion

        #region Properties
        public ICollectionView Languages { get; }

        string _cultureCode = string.Empty;

        LocalizationInfo _selectedLanguage;
        public LocalizationInfo SelectedLangauge
        {
            get => _selectedLanguage;
            set
            {
                if (value == _selectedLanguage)
                    return;

                if (!IsLoading && value != null) // Don't change if the value is null (can happen when a user searchs for a language....)
                {
                    LocalizationManager.GetInstance().Change(value);

                    SettingsManager.Current.Localization_CultureCode = value.Code;

                    RestartRequired = (value.Code != _cultureCode);
                }

                _selectedLanguage = value;
                OnPropertyChanged();
            }
        }

        string _search;
        public string Search
        {
            get => _search;
            set
            {
                if (value == _search)
                    return;

                _search = value;

                Languages.Refresh();

                OnPropertyChanged();
            }
        }

        bool _restartRequired;
        public bool RestartRequired
        {
            get => _restartRequired;
            set
            {
                if (value == _restartRequired)
                    return;

                _restartRequired = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Construtor, LoadSettings
        public SettingsLanguageViewModel()
        {
            IsLoading = true;

            Languages = CollectionViewSource.GetDefaultView(LocalizationManager.List);
            Languages.SortDescriptions.Add(new SortDescription(nameof(LocalizationInfo.IsOfficial), ListSortDirection.Descending));
            Languages.SortDescriptions.Add(new SortDescription(nameof(LocalizationInfo.PercentTranslated), ListSortDirection.Descending));
            Languages.SortDescriptions.Add(new SortDescription(nameof(LocalizationInfo.Name), ListSortDirection.Ascending));

            Languages.Filter = o =>
            {
                if (string.IsNullOrEmpty(Search))
                    return true;

                if (!(o is LocalizationInfo info))
                    return false;

                var search = Search.Trim();

                // Search by: Name
                return info.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) > -1 || info.NativeName.IndexOf(search, StringComparison.OrdinalIgnoreCase) > -1;
            };

            SelectedLangauge = Languages.Cast<LocalizationInfo>().FirstOrDefault(x => x.Code == LocalizationManager.GetInstance().Current.Code);

            LoadSettings();

            IsLoading = false;
        }

        void LoadSettings()
        {
            _cultureCode = SettingsManager.Current.Localization_CultureCode;
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

        public ICommand OpenWebsiteCommand => new RelayCommand(OpenWebsiteAction);

        static void OpenWebsiteAction(object url)
        {
            Process.Start((string)url);
        }
        #endregion
    }
}
