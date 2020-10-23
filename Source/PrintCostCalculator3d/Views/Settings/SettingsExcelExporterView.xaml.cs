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
using PrintCostCalculator3d.ViewModels;

namespace PrintCostCalculator3d.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsExcelExporterView.xaml
    /// </summary>
    public partial class SettingsExcelExporterView : UserControl
    {
        private readonly SettingsExcelExporterViewModel _viewModel = new SettingsExcelExporterViewModel(DialogCoordinator.Instance);

        public SettingsExcelExporterView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        private void TextBoxLocation_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files != null)
                _viewModel.SetLocationPathFromDragDrop(files[0]);
        }

        private void TextBoxLocation_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
    }
}
