using WpfFramework.ViewModels;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace WpfFramework.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsSlicerView.xaml
    /// </summary>
    public partial class SettingsSlicerView : UserControl
    {
        private readonly SettingsSlicerViewModel _viewModel = new SettingsSlicerViewModel(DialogCoordinator.Instance);

        public SettingsSlicerView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

    }
}
