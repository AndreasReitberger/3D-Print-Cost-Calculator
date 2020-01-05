using WpfFramework.ViewModels._3dPrinting;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfFramework.Views._3dPrinting
{
    /// <summary>
    /// Interaktionslogik für _3dPrintingPrinterView.xaml
    /// </summary>
    public partial class _3dPrintingPrinterView : UserControl
    {
        #region ViewModel
        private readonly _3dPrintingPrinterViewModel _viewModel = new _3dPrintingPrinterViewModel(DialogCoordinator.Instance);
        #endregion
        public _3dPrintingPrinterView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        #region Events
        private void ContextMenu_Opened(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is ContextMenu menu)
                menu.DataContext = _viewModel;
        }
        #endregion

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
