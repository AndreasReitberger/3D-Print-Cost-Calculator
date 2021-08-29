using ControlzEx.Theming;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Theming;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace PrintCostCalculator3d.Models.Settings
{
    public static class AppearanceManager
    {
        static readonly string ThemesFilePath = Path.Combine(ConfigurationManager.Current.ExecutionPath, "Themes");

        const string CostomThemeFileExtension = @".Theme.xaml";
        const string CostomAccentFileExtension = @".Accent.xaml";

        public static MetroDialogSettings MetroDialog = new MetroDialogSettings();

        /// <summary>
        /// Load Appearance (AppTheme and Accent) from the user settings.
        /// </summary>
        public static void Load()
        {
            // Add custom themes
            foreach (var file in Directory.GetFiles(ThemesFilePath))
            {
                var fileName = Path.GetFileName(file);

                if (fileName.EndsWith(CostomThemeFileExtension))
                    ThemeManager.Current.AddLibraryTheme(
                        new LibraryTheme(new Uri(file), MahAppsLibraryThemeProvider.DefaultInstance));
                /*
                // Theme
                if (fileName.EndsWith(CostomThemeFileExtension))
                    ThemeManager.AddAppTheme(fileName.Substring(0, fileName.Length - CostomThemeFileExtension.Length), new Uri(file));

                // Accent
                if (fileName.EndsWith(CostomAccentFileExtension))
                    ThemeManager.AddAccent(fileName.Substring(0, fileName.Length - CostomAccentFileExtension.Length), new Uri(file));
                */
            }

            // Change the AppTheme if it is not empty and different from the currently loaded
            var appThemeName = SettingsManager.Current.Appearance_AppTheme;

            if (!string.IsNullOrEmpty(appThemeName) && appThemeName != ThemeManager.Current.DetectTheme().Name)
                ChangeAppTheme(appThemeName);

            /* Mahapps 1.6
            // Change the Accent if it is not empty and different from the currently loaded
            var accentName = SettingsManager.Current.Appearance_Accent;

            if (!string.IsNullOrEmpty(accentName) && accentName != ThemeManager.DetectAppStyle().Item2.Name)
                ChangeAccent(accentName);
            */
            MetroDialog.CustomResourceDictionary = new ResourceDictionary
            {
                Source = new Uri("3dPrintCostCalculator2;component/Resources/Styles/MetroDialogStyles.xaml", UriKind.RelativeOrAbsolute)
            };
        }

        /// <summary>
        /// Change the AppTheme
        /// </summary>
        /// <param name="name">Name of the AppTheme</param>
        public static void ChangeAppTheme(string name)
        {
            var theme = ThemeManager.Current.Themes.FirstOrDefault(mahappTheme => mahappTheme.Name == name);

            // If user has renamed / removed a custom theme --> fallback default
            if (theme != null)
            {
                try
                {
                    ThemeManager.Current.ChangeTheme(Application.Current, name);
                }
                catch(Exception)
                {
                    var themes = ThemeManager.Current.Themes;
                    if(themes.Count > 0)
                        ThemeManager.Current.ChangeTheme(Application.Current, themes[0]);
                }
            }
        }

        /// <summary>
        /// Change the Accent
        /// </summary>
        /// <param name="name">Name of the Accent</param>
        /* Mahapps 1.6
        public static void ChangeAccent(string name)
        {
            var appStyle = ThemeManager.DetectAppStyle(Application.Current);
            var accent = ThemeManager.GetAccent(name);

            // If user has renamed / removed a custom theme --> fallback default
            if (accent != null)
                ThemeManager.ChangeAppStyle(Application.Current, accent, appStyle.Item1);
        }
        */
    }
}
