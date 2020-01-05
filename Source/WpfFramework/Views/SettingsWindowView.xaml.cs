using WpfFramework.ViewModels;
using System.Windows.Controls;


namespace WpfFramework.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsWindowView.xaml
    /// </summary>
    public partial class SettingsWindowView : UserControl
    {
        private readonly SettingsWindowViewModel _viewModel = new SettingsWindowViewModel();

        public SettingsWindowView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }
    }
}
