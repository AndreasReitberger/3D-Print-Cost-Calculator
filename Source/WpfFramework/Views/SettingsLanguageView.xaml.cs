using WpfFramework.ViewModels;
using System.Windows.Controls;


namespace WpfFramework.Views
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
