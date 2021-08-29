using log4net;
using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Enums;
using PrintCostCalculator3d.ViewModels.Dashboard;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PrintCostCalculator3d.Views.Dashboard
{
    /// <summary>
    /// Interaktionslogik für DashboardCalculatorView.xaml
    /// </summary>
    public partial class DashboardCalculatorView : UserControl
    {
        #region ViewModel
        readonly DashboardCalculatorViewModel _viewModel; 
        static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public DashboardTabContentType TabContentType { get; set; }
        #endregion

        //public DashboardCalculatorView(string Name = "")
        public DashboardCalculatorView(DashboardTabContentType Name)
        {
            InitializeComponent();
            TabContentType = Name;
            DataContext = _viewModel = new DashboardCalculatorViewModel(DialogCoordinator.Instance, Name);
        }

        #region Methods
        void ContextMenu_Opened(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is ContextMenu menu)
                menu.DataContext = _viewModel;
        }

        public void CloseTab()
        {
            _viewModel.OnClose();
        }
        public void OnViewHide()
        {
            _viewModel.OnViewHide();
        }

        public void OnViewVisible()
        {
            _viewModel.OnViewVisible();
        }
        #endregion
    }
}
