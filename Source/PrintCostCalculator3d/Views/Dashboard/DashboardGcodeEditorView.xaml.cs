using GalaSoft.MvvmLight.Messaging;
using log4net;
using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Enums;
using PrintCostCalculator3d.ViewModels.Dashboard;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PrintCostCalculator3d.Views.Dashboard
{
    /// <summary>
    /// Interaktionslogik für DashboardGcodeEditorView.xaml
    /// </summary>
    public partial class DashboardGcodeEditorView : UserControl
    {

        #region ViewModel
        readonly DashboardGcodeEditorViewModel _viewModel;
        static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public DashboardTabContentType TabContentType { get; set; }
        #endregion

        public DashboardGcodeEditorView(DashboardTabContentType Name)
        {
            InitializeComponent();
            TabContentType = Name;

            DataContext = _viewModel = new DashboardGcodeEditorViewModel(DialogCoordinator.Instance, Name);

            //Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        #region Methods
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
