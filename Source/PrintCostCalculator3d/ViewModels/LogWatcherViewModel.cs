using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Models;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace PrintCostCalculator3d.ViewModels
{
    public class LogWatcherViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        Logger Logger = Logger.Instance;
        public virtual Dispatcher DispatcherObject { get; protected set; }

        #endregion

        #region Properties

        #region Events
        Event _selectedEvent;
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

        void Events_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        #endregion

        #region ICommand & Actions

        public ICommand ClearFormCommand
        {
            get => new RelayCommand(p => ClearFormAction());

        }
        void ClearFormAction()
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
        void CopyToClipboardAction()
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

        public void OnViewVisible()
        {

        }

        public void OnViewHide()
        {
        }
        #endregion
    }
}
