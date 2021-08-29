using PrintCostCalculator3d.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;


namespace PrintCostCalculator3d.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsGeneralView.xaml
    /// </summary>
    public partial class SettingsGeneralView : UserControl
    {
        readonly SettingsGeneralViewModel _viewModel = new SettingsGeneralViewModel();

        public SettingsGeneralView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        void ListBoxVisibleToHide_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!_viewModel.IsVisibleToHideApplicationEnabled)
                return;

            if (e.ChangedButton == MouseButton.Left)
                _viewModel.VisibleToHideApplicationCommand.Execute(null);
        }

        void ListBoxHideToVisible_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!_viewModel.IsHideToVisibleApplicationEnabled)
                return;

            if (e.ChangedButton == MouseButton.Left)
                _viewModel.HideToVisibleApplicationCommand.Execute(null);
        }
    }
}
