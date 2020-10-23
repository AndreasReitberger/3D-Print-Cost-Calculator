using PrintCostCalculator3d.ViewModels;

namespace PrintCostCalculator3d.Views
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
