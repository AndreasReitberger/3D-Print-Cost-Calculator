﻿using PrintCostCalculator3d.ViewModels;
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

namespace PrintCostCalculator3d.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        readonly SettingsViewModel _viewModel = new SettingsViewModel();

        public SettingsView(ApplicationName applicationName)
        {
            InitializeComponent();
            _viewModel = new SettingsViewModel(applicationName);
            DataContext = _viewModel;
        }
        public SettingsView(SettingsViewName settingsName)
        {
            InitializeComponent();
            _viewModel = new SettingsViewModel(settingsName);
            DataContext = _viewModel;
        }

        void ScrollViewer_ManipulationBoundaryFeedback(object sender, System.Windows.Input.ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        public void ChangeSettingsView(ApplicationName name)
        {
            _viewModel.ChangeSettingsView(name);

            // Scroll into view
            //ListBoxSettings.ScrollIntoView(_viewModel.SelectedSettingsView);
        }
        public void ChangeSettingsView(SettingsViewName name)
        {
            _viewModel.ChangeSettingsView(name);

            // Scroll into view
            //ListBoxSettings.ScrollIntoView(_viewModel.SelectedSettingsView);
        }

        public void Refresh()
        {
            //ProfilesView.Refresh();
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
