using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;
using PrintCostCalculator3d.ViewModels;

namespace PrintCostCalculator3d.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsPrivacyPolicyView.xaml
    /// </summary>
    public partial class SettingsPrivacyPolicyView : UserControl
    {
        readonly SettingsPrivacyPolicyViewModel _viewModel = new SettingsPrivacyPolicyViewModel(DialogCoordinator.Instance);
        public SettingsPrivacyPolicyView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }
    }
}
