using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PrintCostCalculator3d.Models.GCode;
using PrintCostCalculator3d.ViewModels;

namespace PrintCostCalculator3d
{
    /// <summary>
    /// Interaktionslogik für GcodeViewerWindow.xaml
    /// </summary>
    public partial class GcodeViewerWindow
    {
        private readonly GcodeViewModel _viewModel;

        public GcodeViewerWindow(IList<GCode> gcodes)
        {
            InitializeComponent();
            _viewModel = new GcodeViewModel(DialogCoordinator.Instance, gcodes);
            DataContext = _viewModel;
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (sender is ContextMenu menu)
                menu.DataContext = _viewModel;
        }

        private void ListBoxItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
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
