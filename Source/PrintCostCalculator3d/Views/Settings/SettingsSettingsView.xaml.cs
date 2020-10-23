using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Windows.Controls;
using PrintCostCalculator3d.ViewModels;

namespace PrintCostCalculator3d.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsSettingsView.xaml
    /// </summary>
    public partial class SettingsSettingsView : UserControl
    {
        private readonly SettingsSettingsViewModel _viewModel = new SettingsSettingsViewModel(DialogCoordinator.Instance);
        public SettingsSettingsView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel.CloseAction != null)
                return;

            var window = Window.GetWindow(this);

            if (window != null)
                _viewModel.CloseAction = window.Close;
        }

        public void SaveAndCheckSettings()
        {
            _viewModel.SaveAndCheckSettings();
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
