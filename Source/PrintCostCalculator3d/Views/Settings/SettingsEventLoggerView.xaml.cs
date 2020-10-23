using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;
using PrintCostCalculator3d.ViewModels;

namespace PrintCostCalculator3d.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsEventLoggerView.xaml
    /// </summary>
    public partial class SettingsEventLoggerView : UserControl
    {
        private readonly SettingsEventLoggerViewModel _viewModel = new SettingsEventLoggerViewModel(DialogCoordinator.Instance);
        public SettingsEventLoggerView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }
    }
    
}
