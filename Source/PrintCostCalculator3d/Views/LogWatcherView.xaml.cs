using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;
using PrintCostCalculator3d.ViewModels;

namespace PrintCostCalculator3d.Views
{
    /// <summary>
    /// Interaktionslogik für LogWatcherView.xaml
    /// </summary>
    public partial class LogWatcherView : UserControl
    {
        #region ViewModel
        private readonly LogWatcherViewModel _viewModel = new LogWatcherViewModel(DialogCoordinator.Instance);
        #endregion

        public LogWatcherView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        #region Methods
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
