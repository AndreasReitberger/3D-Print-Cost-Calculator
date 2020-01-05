using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using WpfFramework.Models;
using WpfFramework.Utilities;
using log4net;
using System.Windows.Input;
using WpfFramework.Resources.Localization;
using System.Windows.Threading;
using System.Windows;

namespace WpfFramework.ViewModels
{
    public class LogWatcherViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private Logger Logger = Logger.Instance;

        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public virtual Dispatcher DispatcherObject { get; protected set; }

        #endregion

        #region Properties

        #region Events
        private Event _selectedEvent;
        public Event SelectedEvent
        {
            get => _selectedEvent;
            set
            {
                if(_selectedEvent != value)
                {
                    _selectedEvent = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Event> Events
        {
            get => Logger.Events;
            set
            {
                if (Logger.Events != value)
                {
                    Logger.Events = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
        #endregion

        #region Constructor
        public LogWatcherViewModel(IDialogCoordinator dialog)
        {
            this._dialogCoordinator = dialog;
            Events.CollectionChanged += Events_CollectionChanged;

        }

        private void Events_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        #endregion


        #region ICommand & Actions

        public ICommand ClearFormCommand
        {
            get => new RelayCommand(p => ClearFormAction());

        }
        private void ClearFormAction()
        {
            try
            {
                Events = new ObservableCollection<Event>();
                OnPropertyChanged(nameof(Events));
                logger.Info(Strings.EventFormCleared);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
        public ICommand CopyToClipboardCommand
        {
            get => new RelayCommand(p => CopyToClipboardAction());

        }
        private void CopyToClipboardAction()
        {
            try
            {
                if (SelectedEvent != null)
                {
                    Clipboard.SetText(SelectedEvent.FullMessage);
                    //logger.Info(Strings.EventFormCleared);
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }


        #endregion

        #region Methods

        public async void OnViewVisible()
        {

        }

        public void OnViewHide()
        {
        }
        #endregion
    }
}
