using System.Windows.Controls;
using PrintCostCalculator3d.ViewModels;

namespace PrintCostCalculator3d.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsUpdateView.xaml
    /// </summary>
    public partial class SettingsUpdateView : UserControl
    {
        readonly SettingsUpdateViewModel _viewModel = new SettingsUpdateViewModel();
        public SettingsUpdateView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }
    }
}
