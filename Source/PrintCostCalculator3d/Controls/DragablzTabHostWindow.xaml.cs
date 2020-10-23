using System;
using Dragablz;
using PrintCostCalculator3d.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PrintCostCalculator3d.Utilities;

namespace PrintCostCalculator3d.Controls
{
    /// <summary>
    /// Interaktionslogik für DragablzTabHostWindow.xaml
    /// </summary>
    public partial class DragablzTabHostWindow : INotifyPropertyChanged
    {
        #region PropertyChangedEventHandler
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Variables
        public IInterTabClient InterTabClient { get; }
        private readonly string _applicationName;
        //private readonly ApplicationName _applicationName;

        private string _applicationTitle;
        public string ApplicationTitle
        {
            get => _applicationTitle;
            set
            {
                if (value == _applicationTitle)
                    return;

                _applicationTitle = value;
                OnPropertyChanged();
            }
        }

        private bool _isPuTTYControl;
        public bool IsPuTTYControl
        {
            get => _isPuTTYControl;
            set
            {
                if (value == _isPuTTYControl)
                    return;

                _isPuTTYControl = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor
        public DragablzTabHostWindow(string applicationName)
        {
            InitializeComponent();
            DataContext = this;

            // Transparency
            /*
            if (SettingsManager.Current.Appearance_EnableTransparency)
            {
                AllowsTransparency = true;
                Opacity = SettingsManager.Current.Appearance_Opacity;
            }
            */
            _applicationName = applicationName;

            InterTabClient = new DragablzInterTabClient(applicationName);

            InterTabController.Partition = applicationName.ToString();

            //ApplicationTitle = ApplicationViewManager.GetTranslatedNameByName(applicationName);
            ApplicationTitle = applicationName;
            /*
            if (applicationName == ApplicationViewManager.Name.PuTTY)
                IsPuTTYControl = true;
                */
            //SettingsManager.Current.PropertyChanged += SettingsManager_PropertyChanged;
        }
        #endregion

        #region ICommand & Actions
        public ItemActionCallback CloseItemCommand => CloseItemAction;

        private void CloseItemAction(ItemActionCallbackArgs<TabablzControl> args)
        {
            // Switch between application identifiert...
            /*
            switch (_applicationName)
            {
                case ApplicationName.None:
                    break;
                    
                case ApplicationViewManager.Name.IPScanner:
                    ((IPScannerView)((DragablzTabItem)args.DragablzItem.Content).View).CloseTab();
                    break;
                case ApplicationViewManager.Name.PortScanner:
                    ((PortScannerView)((DragablzTabItem)args.DragablzItem.Content).View).CloseTab();
                    break;
                case ApplicationViewManager.Name.Ping:
                    ((PingView)((DragablzTabItem)args.DragablzItem.Content).View).CloseTab();
                    break;
                case ApplicationViewManager.Name.Traceroute:
                    ((TracerouteView)((DragablzTabItem)args.DragablzItem.Content).View).CloseTab();
                    break;
                case ApplicationViewManager.Name.DNSLookup:
                    ((DNSLookupView)((DragablzTabItem)args.DragablzItem.Content).View).CloseTab();
                    break;
                case ApplicationViewManager.Name.RemoteDesktop:
                    ((RemoteDesktopControl)((DragablzTabItem)args.DragablzItem.Content).View).CloseTab();
                    break;
                case ApplicationViewManager.Name.PuTTY:
                    ((PuTTYControl)((DragablzTabItem)args.DragablzItem.Content).View).CloseTab();
                    break;
                case ApplicationViewManager.Name.TigerVNC:
                    ((TigerVNCControl)((DragablzTabItem)args.DragablzItem.Content).View).CloseTab();
                    break;
                case ApplicationViewManager.Name.SNMP:
                    ((SNMPView)((DragablzTabItem)args.DragablzItem.Content).View).CloseTab();
                    break;
                case ApplicationViewManager.Name.HTTPHeaders:
                    ((HTTPHeadersView)((DragablzTabItem)args.DragablzItem.Content).View).CloseTab();
                    break;
                    

                default:
                    throw new ArgumentOutOfRangeException();
            
            }*/
        }

        #endregion

        #region Events
        private void SettingsManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }
        #endregion

        #region Window helper
        // Move the window when the user hold the title...
        private void HeaderBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
        #endregion

        private void MetroWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (var item in TabsContainer.Items)
            {

            }
        }
    }
}
