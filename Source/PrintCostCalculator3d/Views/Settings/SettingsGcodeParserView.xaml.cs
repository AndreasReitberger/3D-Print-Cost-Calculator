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
using PrintCostCalculator3d.ViewModels;

namespace PrintCostCalculator3d.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsGcodeParserView.xaml
    /// </summary>
    public partial class SettingsGcodeParserView : UserControl
    {
        readonly SettingsGcodeParserViewModel _viewModel = new SettingsGcodeParserViewModel();

        public SettingsGcodeParserView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }
    }
}
