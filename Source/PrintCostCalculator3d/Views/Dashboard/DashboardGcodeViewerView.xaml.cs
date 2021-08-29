using GalaSoft.MvvmLight.Messaging;
using HelixToolkit.Wpf.SharpDX;
using log4net;
using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Enums;
using PrintCostCalculator3d.Models.GCode;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.ViewModels.Dashboard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace PrintCostCalculator3d.Views.Dashboard
{
    /// <summary>
    /// Interaktionslogik für DashboardGcodeViewerView.xaml
    /// </summary>
    public partial class DashboardGcodeViewerView : UserControl
    {
        #region ViewModel
        readonly DashboardGcodeViewerViewModel _viewModel;
        static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public DashboardTabContentType TabContentType { get; set; }
        #endregion

        //public DashboardGcodeViewerView(string Name = "")
        public DashboardGcodeViewerView(DashboardTabContentType Name)
        {
            InitializeComponent();
            TabContentType = Name;
            DataContext = _viewModel = new DashboardGcodeViewerViewModel(DialogCoordinator.Instance, Name);
            _viewModel.viewGcode2d = view2dGcode;
            _viewModel.viewGcode3d = view3dGcode;

            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        #region Messages
        void NotificationMessageReceived(NotificationMessage msg)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (msg.Notification == "ResetCameraGcode")
                    {
                        view2dGcode.Camera.Reset();
                        view2dGcode.Camera.ZoomExtents(view2dGcode, 1);
                    }
                    else if (msg.Notification == "ResetCameraGcode3d")
                    {
                        view3dGcode.Camera.Reset();
                        view3dGcode.Camera.ZoomExtents(view3dGcode, 1);
                    }
                    else if (msg.Notification == "ZoomToFitGcode")
                    {
                        view2dGcode.ZoomExtents();
                        //view2dGcode.Camera.ZoomExtents(view2dGcode, 1);
                        view3dGcode.Camera.ZoomExtents(view3dGcode, 1);
                    }
                });
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.Message, exc.TargetSite);
            }
        }
        #endregion

        #region Methods
        public void CloseTab()
        {
            _viewModel.OnClose();
        }
        public void OnViewHide()
        {
            _viewModel.OnViewHide();
        }

        public void OnViewVisible()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                view2dGcode.ZoomExtents();
                //view2dGcode.Camera.ZoomExtents(view2dGcode, 1);
                view3dGcode.Camera.ZoomExtents(view3dGcode, 1);
                _viewModel.OnViewVisible();
            });
        }
        #endregion
    }
}
