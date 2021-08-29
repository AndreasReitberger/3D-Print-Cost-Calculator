using AndreasReitberger.Models;
using PrintCostCalculator3d.ViewModels;
using System.Windows.Controls;

namespace PrintCostCalculator3d.Templates
{
    /// <summary>
    /// Interaktionslogik für CodeEditorViewTemplate.xaml
    /// </summary>
    public partial class CodeEditorViewTemplate : UserControl
    {
        readonly CodeEditorViewModel _viewModel;
        public CodeEditorViewTemplate(int tabId, Gcode file = null)
        {
            InitializeComponent();
            _viewModel = new CodeEditorViewModel(tabId, file);

            DataContext = _viewModel;
        }

        public void CloseTab()
        {
            _viewModel.OnClose();
        }
    }
}
