namespace PrintCostCalculator3d.Models.WooCommerce
{
    class WooCommerceSoftwareLicenseAction
    {
        public string Value { get; set; }

        WooCommerceSoftwareLicenseAction(string value) { Value = value; }

        public static WooCommerceSoftwareLicenseAction Activate { get { return new WooCommerceSoftwareLicenseAction("activate"); } }
        public static WooCommerceSoftwareLicenseAction Deactivate { get { return new WooCommerceSoftwareLicenseAction("deactivate"); } }
        public static WooCommerceSoftwareLicenseAction StatusCheck { get { return new WooCommerceSoftwareLicenseAction("status-check"); } }
        public static WooCommerceSoftwareLicenseAction PluginUpdate { get { return new WooCommerceSoftwareLicenseAction("plugin_update"); } }
        public static WooCommerceSoftwareLicenseAction PluginInformation { get { return new WooCommerceSoftwareLicenseAction("plugin_information"); } }
        public static WooCommerceSoftwareLicenseAction ThemeUpdate { get { return new WooCommerceSoftwareLicenseAction("theme_update"); } }
        public static WooCommerceSoftwareLicenseAction CodeVersion { get { return new WooCommerceSoftwareLicenseAction("code_version"); } }
        public static WooCommerceSoftwareLicenseAction DeleteKey { get { return new WooCommerceSoftwareLicenseAction("key_delete"); } }
    }
}
