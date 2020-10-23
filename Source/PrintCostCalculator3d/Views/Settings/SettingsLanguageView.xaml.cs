using PrintCostCalculator3d.ViewModels;
using System.Windows.Controls;


namespace PrintCostCalculator3d.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsLanguageView.xaml
    /// </summary>
    public partial class SettingsLanguageView : UserControl
    {
        private readonly SettingsLanguageViewModel _viewModel = new SettingsLanguageViewModel();
        public SettingsLanguageView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }
    }
}
