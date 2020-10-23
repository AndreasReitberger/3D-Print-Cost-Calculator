using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//ADDITIONAL
using PrintCostCalculator3d.Utilities;
using PrintCostCalculator3d.Models.Settings;
using MahApps.Metro;
using ControlzEx.Theming;
using System.Collections.ObjectModel;

namespace PrintCostCalculator3d.ViewModels
{
    public class SettingsAppearanceViewModel : ViewModelBase
    {
        #region Variables
        private readonly bool _isLoading;

        private bool _darkThemes = false;
        public bool DarkThemes
        {
            get => _darkThemes;
            set
            {
                if (_darkThemes == value) return;
                _darkThemes = value;
                UpdateTheme();
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Theme> _themes = new ObservableCollection<Theme>();
        public ObservableCollection<Theme> Themes
        {
            get => _themes;
            private set
            {
                if (_themes.Equals(value)) return;
                _themes = value;
                OnPropertyChanged();
            }
        }

        private Theme _appThemeSelectedItem;
        public Theme AppThemeSelectedItem
        {
            get => _appThemeSelectedItem;
            set
            {
                if (value == _appThemeSelectedItem)
                    return;

                if (!_isLoading && value != null)
                {
                    AppearanceManager.ChangeAppTheme(value.Name);
                    SettingsManager.Current.Appearance_AppTheme = value.Name;
                }

                _appThemeSelectedItem = value;
                OnPropertyChanged();
            }
        }
        /*
         * Mahapps 1.6
        private Accent _accentSelectedItem;
        public Accent AccentSelectedItem
        {
            get => _accentSelectedItem;
            set
            {
                if (value == _accentSelectedItem)
                    return;

                if (!_isLoading)
                {

                    AppearanceManager.ChangeAccent(value.Name);
                    SettingsManager.Current.Appearance_Accent = value.Name;
                }

                _accentSelectedItem = value;
                OnPropertyChanged();
            }
        }
        */
        private bool _enableTransparency;
        public bool EnableTransparency
        {
            get => _enableTransparency;
            set
            {
                if (value == _enableTransparency)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.Appearance_EnableTransparency = value;

                _enableTransparency = value;
                OnPropertyChanged();
            }
        }

        private int _opacity;
        public int Opacity
        {
            get => _opacity;
            set
            {
                if (value == _opacity)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.Appearance_Opacity = (double)value / 100;

                _opacity = value;
                OnPropertyChanged();
            }
        }
        #endregion        

        #region Constructor, LoadSettings
        public SettingsAppearanceViewModel()
        {
            _isLoading = true;

            LoadSettings();

            _isLoading = false;
            // Load themes
            Themes = new ObservableCollection<Theme>(ThemeManager.Current.Themes.Where(theme => theme.BaseColorScheme == (DarkThemes ? "Dark" : "Light")));
        }

        private void LoadSettings()
        {
            AppThemeSelectedItem = ThemeManager.Current.DetectTheme();
            if (AppThemeSelectedItem != null)
            {
                if (AppThemeSelectedItem.BaseColorScheme == "Dark")
                    DarkThemes = true;
                else
                    DarkThemes = false;
            }
            // Mahapps 1.6
            //AppThemeSelectedItem = ThemeManager.DetectAppStyle().Item1;
            //AccentSelectedItem = ThemeManager.DetectAppStyle().Item2;
            EnableTransparency = SettingsManager.Current.Appearance_EnableTransparency;
            Opacity = (int)(SettingsManager.Current.Appearance_Opacity * 100);
        }
        #endregion

        #region Methods
        private void UpdateTheme()
        {
            var current = AppThemeSelectedItem;
            Themes = new ObservableCollection<Theme>(ThemeManager.Current.Themes.Where(theme => theme.BaseColorScheme == (_darkThemes ? "Dark" : "Light")));
            // Set theme
            AppThemeSelectedItem = Themes.FirstOrDefault(th => th.ColorScheme == current.ColorScheme);
        }
        #endregion
    }
}
