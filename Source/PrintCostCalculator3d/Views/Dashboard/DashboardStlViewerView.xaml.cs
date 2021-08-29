using GalaSoft.MvvmLight.Messaging;
using HelixToolkit.Wpf.SharpDX;
using log4net;
using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Enums;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.ViewModels.Dashboard;
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

namespace PrintCostCalculator3d.Views.Dashboard
{
    /// <summary>
    /// Interaktionslogik für DashboardStlViewerView.xaml
    /// </summary>
    public partial class DashboardStlViewerView : UserControl
    {
        #region ViewModel
        readonly DashboardStlViewerViewModel _viewModel; 
        static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public DashboardTabContentType TabContentType { get; set; }
        #endregion


        //public DashboardStlViewerView(string Name = "")
        public DashboardStlViewerView(DashboardTabContentType Name)
        {
            InitializeComponent();
            TabContentType = Name;
            DataContext = _viewModel = new DashboardStlViewerViewModel(DialogCoordinator.Instance, Name);

            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        #region Messages
        void NotificationMessageReceived(NotificationMessage msg)
        {
            if (msg.Notification == "ResetCameraStl")
            {
                try
                {
                    view3d.Camera.Reset();
                    view3d.Camera.ZoomExtents(view3d, 1);
                }
                catch (Exception exc)
                {
                    logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.Message, exc.TargetSite);
                }

            }
            else if (msg.Notification == "ZoomToFitStl")
            {
                try
                {
                    view3d.Camera.ZoomExtents(view3d, 1);
                }
                catch (Exception exc)
                {
                    logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.Message, exc.TargetSite);
                }

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
            _viewModel.OnViewVisible();
        }
        #endregion
    }
}
