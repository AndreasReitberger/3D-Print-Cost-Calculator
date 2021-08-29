using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.ViewModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PrintCostCalculator3d.Views
{
    /// <summary>
    /// Interaktionslogik für DashboardHostView.xaml
    /// </summary>
    public partial class DashboardHostView : UserControl
    {
        readonly DashboardHostViewModel _viewModel;

        public string Partition = ApplicationName.Dashboard.ToString();

        public DashboardHostView()
        {
            InitializeComponent(); 
            _viewModel = new DashboardHostViewModel(DialogCoordinator.Instance);
            _viewModel.mainDropGrid = this.mainGrid;
            DataContext = _viewModel;

            InterTabController.Partition = ApplicationName.Dashboard.ToString();
            //InterLayoutController.Partition = ApplicationName.Dashboard.ToString();
            //InterTabControllerExpander.Partition = ApplicationName.Dashboard.ToString();
        }


        void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (sender is ContextMenu menu)
                menu.DataContext = _viewModel;
        }

        void ListBoxItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {

            }
        }

        public void AddTab(string host)
        {
            _viewModel.AddTab(host);
        }

        public void OnViewHide()
        {
            _viewModel.OnViewHide();
        }

        public void OnViewVisible()
        {
            _viewModel.OnViewVisible();
        }
    }
}
