using System;

namespace PrintCostCalculator3d.Utilities
{
    public class EventSystem
    {
        // This will notify the mail window, to change the view to another application and redirect some data (hostname, ip)
        public static event EventHandler RedirectToApplicationEvent;

        public static void RedirectToApplication(ApplicationName application, string data)
        {
            RedirectToApplicationEvent?.Invoke(typeof(string), new EventSystemRedirectApplicationArgs(application, data));
        }

        // This will notify the main window, to change the view to the settings...
        public static event EventHandler RedirectToSettingsEvent;

        public static void RedirectToSettings()
        {
            RedirectToSettingsEvent?.Invoke(typeof(string), EventArgs.Empty);
        }
        public static void RedirectToSettings(SettingsViewName name)
        {
            RedirectToSettingsEvent?.Invoke(typeof(string), new EventSystemRedirectSettingsArgs(name));
        }
    }
}