using PrintCostCalculator3d.ViewModels;
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
using MahApps.Metro.Controls.Dialogs;

namespace PrintCostCalculator3d.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsEULAView.xaml
    /// </summary>
    public partial class SettingsEULAView : UserControl
    {
        readonly SettingsEULAViewModel _viewModel = new SettingsEULAViewModel(DialogCoordinator.Instance);
        public SettingsEULAView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }
    }
}
