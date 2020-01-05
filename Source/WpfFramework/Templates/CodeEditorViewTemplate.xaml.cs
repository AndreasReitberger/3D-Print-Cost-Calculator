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
using WpfFramework.Models.GCode;
using WpfFramework.ViewModels;

namespace WpfFramework.Templates
{
    /// <summary>
    /// Interaktionslogik für CodeEditorViewTemplate.xaml
    /// </summary>
    public partial class CodeEditorViewTemplate : UserControl
    {
        private readonly CodeEditorViewModel _viewModel;
        public CodeEditorViewTemplate(int tabId, GCode file = null)
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
