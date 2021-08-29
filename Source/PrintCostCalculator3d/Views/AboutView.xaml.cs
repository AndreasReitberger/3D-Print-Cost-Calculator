using PrintCostCalculator3d.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;


namespace PrintCostCalculator3d.Views
{
    /// <summary>
    /// Interaktionslogik für AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl
    {
        readonly AboutViewModel _viewModel = new AboutViewModel(DialogCoordinator.Instance);
        public AboutView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }
        void ContextMenu_Opened(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is ContextMenu menu) menu.DataContext = _viewModel;
        }

        // Fix mouse wheel when using DataGrid (https://stackoverflow.com/a/16235785/4986782)
        void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var scv = (ScrollViewer)sender;

            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);

            e.Handled = true;
        }
    }
}
