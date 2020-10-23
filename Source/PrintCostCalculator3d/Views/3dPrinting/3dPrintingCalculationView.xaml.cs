using PrintCostCalculator3d.ViewModels._3dPrinting;
using HelixToolkit.Wpf;
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
using GalaSoft.MvvmLight.Messaging;
using PrintCostCalculator3d.ViewModels;
using System.Collections;
using PrintCostCalculator3d.Models.GCode;
using System.Collections.ObjectModel;
using HelixToolkit.Wpf.SharpDX;

namespace PrintCostCalculator3d.Views._3dPrinting
{
    /// <summary>
    /// Interaktionslogik für _3dPrintingCalculationView.xaml
    /// </summary>
    public partial class _3dPrintingCalculationView : UserControl
    {
        #region ViewModel
        private readonly _3dPrintingCalculationViewModel _viewModel = new _3dPrintingCalculationViewModel(DialogCoordinator.Instance);
        #endregion

        public _3dPrintingCalculationView()
        {
            InitializeComponent();
            // Register some events
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);

            DataContext = _viewModel;
        }

        #region Events
        private void ContextMenu_Opened(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is ContextMenu menu)
                menu.DataContext = _viewModel;
        }
        private void mslbStl_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification == "ShowGcodeEditor")
            {
                try
                {
                    var type = msg.Sender.GetType();

                    IList items = (System.Collections.IList)msg.Sender;
                    var collection = items.Cast<GCode>();
                    ObservableCollection<GCode> gcodes = new ObservableCollection<GCode>(collection);

                    GcodeViewerWindow viewer = new GcodeViewerWindow(gcodes);
                    viewer.Owner = Application.Current.MainWindow;
                    viewer.Show();
                }
                catch(Exception exc)
                {

                }
                
            }
            else if (msg.Notification == "ResetCameraGcode")
            {
                try
                {
                    // Helix.Wpf
                    //view2dGcode.Camera.Reset();
                    //view2dGcode.Camera.ZoomExtents(view2dGcode.Viewport, 1);
                }
                catch(Exception exc)
                {

                }
                
            }
            else if (msg.Notification == "ResetCameraGcode3d")
            {
                try
                {
                    view3dGcode.Camera.Reset();
                    view3dGcode.Camera.ZoomExtents(view3dGcode, 1);
                    // Helix.Wpf
                    //view3dGcode.Camera.Reset();
                    //view3dGcode.Camera.ZoomExtents(view3dGcode.Viewport, 1);
                }
                catch (Exception exc)
                {

                }
                
            }
            else if (msg.Notification == "ResetCameraStl")
            {
                try
                {
                    view3d.Camera.Reset();
                    view3d.Camera.ZoomExtents(view3d, 1);
                    // Helix.Wpf
                    //view3d.Camera.Reset();
                    //view3d.Camera.ZoomExtents(view3d.Viewport);
                }
                catch (Exception exc)
                {

                }
                
            }
            else if (msg.Notification == "ZoomToFitGcode")
            {
                try
                {
                    view2dGcode.Camera.ZoomExtents(view2dGcode, 1);
                    // Helix.Wpf
                    //var type = msg.Sender.GetType();
                    //view2dGcode.Camera.ZoomExtents(view2dGcode.Viewport, 1);
                }
                catch(Exception exc)
                {

                }
                
            }
            else if (msg.Notification == "ZoomToFitStl")
            {
                try
                {
                    var type = msg.Sender.GetType();
                    view3d.Camera.ZoomExtents(view3d, 1);
                    // Helix.Wpf
                    //.Camera.ZoomExtents(view3d.Viewport);
                }
                catch(Exception exc)
                {

                }
                
            }
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
