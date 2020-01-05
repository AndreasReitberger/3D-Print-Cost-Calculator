using System.Windows.Controls;
using WpfFramework.ViewModels;

namespace WpfFramework.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsUpdateView.xaml
    /// </summary>
    public partial class SettingsUpdateView : UserControl
    {
        private readonly SettingsUpdateViewModel _viewModel = new SettingsUpdateViewModel();
        public SettingsUpdateView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }
    }
}
