using WpfFramework.ViewModels;

namespace WpfFramework.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsAppearanceView.xaml
    /// </summary>
    public partial class SettingsAppearanceView
    {
        private readonly SettingsAppearanceViewModel _viewModel = new SettingsAppearanceViewModel();
        public SettingsAppearanceView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }
    }
}
