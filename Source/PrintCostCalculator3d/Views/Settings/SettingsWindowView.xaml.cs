using PrintCostCalculator3d.ViewModels;
using System.Windows.Controls;


namespace PrintCostCalculator3d.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsWindowView.xaml
    /// </summary>
    public partial class SettingsWindowView : UserControl
    {
        readonly SettingsWindowViewModel _viewModel = new SettingsWindowViewModel();

        public SettingsWindowView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }
    }
}
