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

namespace PrintCostCalculator3d.Templates
{
    /// <summary>
    /// Interaktionslogik für CalculationViewTemplate.xaml
    /// </summary>
    public partial class CalculationViewCardTemplate : UserControl
    {
        public CalculationViewCardTemplate()
        {
            InitializeComponent();
        }

        #region Events
        void ContextMenu_Opened(object sender, System.Windows.RoutedEventArgs e)
        {
            /*
            if (sender is ContextMenu menu)
                menu.DataContext = _viewModel;
                */
        }
        #endregion
    }
}
