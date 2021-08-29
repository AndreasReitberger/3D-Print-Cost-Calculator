using PrintCostCalculator3d.ViewModels._3dPrinting;
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

//Additional
using MahApps.Metro.Controls.Dialogs;

namespace PrintCostCalculator3d.Views._3dPrinting
{
    /// <summary>
    /// Interaktionslogik für _3dPrintingMaterialView.xaml
    /// </summary>
    public partial class _3dPrintingMaterialView : UserControl
    {
        #region ViewModel
        readonly MaterialViewModel _viewModel = new MaterialViewModel(DialogCoordinator.Instance);
        #endregion
        public _3dPrintingMaterialView()
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
