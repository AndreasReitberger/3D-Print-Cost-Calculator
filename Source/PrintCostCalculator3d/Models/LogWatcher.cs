using log4net;
using log4net.Appender;
using log4net.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;

namespace PrintCostCalculator3d.Models
{
    /// <summary>
    /// Allows you to log your events to memory
    /// See: http://www.thepicketts.org/2012/12/how-to-watch-your-log-through-your-application-in-log4net/
    /// </summary>
    public class LogWatcher
    {
        private string logContent;
        
        private MemoryAppenderWithEvents memoryAppender;

        /// <summary>
        /// Represents the method that will handle new logevents
        /// </summary>
        public event EventHandler Updated;

        /// <summary>
        /// Gets the content of the logentry.
        /// </summary>
        public string LogContent
        {
            get { return logContent; }
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        public LogWatcher()
        {
            // Get the memory appender
            memoryAppender = (MemoryAppenderWithEvents)Array.Find(LogManager.GetRepository().GetAppenders(), GetMemoryAppender);
            var repos = LogManager.GetRepository().GetAppenders();

            // Read in the log content
            this.logContent = GetEvents(memoryAppender);

            // Add an event handler to handle updates from the MemoryAppender
            memoryAppender.Updated += HandleUpdate;
        }

        /// <summary>
        /// Set LogContent and alert the Updated event that the LogWatcher has been updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleUpdate(object sender, EventArgs e)
        {
            this.logContent = GetEvents(memoryAppender);

            // Then alert the Updated event that the LogWatcher has been updated
            var handler = Updated;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }


        private static bool GetMemoryAppender(IAppender appender)
        {
            // Returns the IAppender named MemoryAppender in the Log4Net.config file
            if (appender.Name.Equals("MemoryAppender"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get any events that may have occurred, check that there are events to return
        /// </summary>
        /// <param name="memoryAppender"></param>
        /// <returns></returns>
        public string GetEvents(MemoryAppenderWithEvents memoryAppender)
        {
            StringBuilder output = new StringBuilder();

            // Get any events that may have occurred
            LoggingEvent[] events = memoryAppender.GetEvents();

            // Check that there are events to return
            if (events != null && events.Length > 0)
            {
                // If there are events, we clear them from the logger, since we're done with them  
                memoryAppender.Clear();

                // Iterate through each event
                foreach (LoggingEvent ev in events)
                {
                    // Construct the line we want to log
                    //string line = ev.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss,fff") + " [" + ev.ThreadName + "] " + ev.Level + " " + ev.LoggerName + ": " + ev.RenderedMessage + "\r\n";
                    string line = ev.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss") + "||" + ev.Level + "||" + ev.RenderedMessage;

                    // Append to the StringBuilder
                    output.Append(line);
                }
            }

            // Return the constructed output
            return output.ToString();
        }
    }

    public class Logger : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Variables
        LogWatcher logWatcher = new LogWatcher();
        //ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public virtual Dispatcher DispatcherObject 
        { 
            get; 
            protected set; 
        }

        private static Logger _instance;
        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Logger();
                return _instance;
            }
        }
        #endregion

        #region Properties
        private bool _writeToFile = false;
        public bool WriteToFile
        {
            get => _writeToFile;
        }

        private ObservableCollection<Event> _events = new ObservableCollection<Event>();
        public ObservableCollection<Event> Events
        {
            get => _events;
            set
            {
                if (_events != value)
                {
                    _events = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Constructor
        public Logger()
        {
            DispatcherObject = Dispatcher.CurrentDispatcher;

            logWatcher.Updated += logWatcher_Updated;
        }
        #endregion

        #region Private Methods
        private void logWatcher_Updated(object sender, EventArgs e)
        {
            if (!SettingsManager.Current.EventLogger_EnableLogging)
                return;

            if (string.IsNullOrEmpty(logWatcher.LogContent))
                return;
            string[] log = logWatcher.LogContent.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
            if (log.Count() == 0)
                return;
            Event tempEvent = new Event();
            tempEvent.FullMessage = (log.Count() >= 3) ? log[2] : "";
            tempEvent.Error = log[1].ToLower().Contains("error");
            switch(log[1].ToLower())
            {
                case "info":
                    tempEvent.Category = Strings.Info;
                    break;
                case "warning":
                    tempEvent.Category = Strings.Warning;
                    break;
                case "error":
                    tempEvent.Category = Strings.Error;
                    break;
                default:
                    tempEvent.Category = log[1];
                    break;
            }
            if (DispatcherObject.Thread != Thread.CurrentThread)
            {
                DispatcherObject.Invoke(new Action(() => Events.Add(tempEvent)));
                if (WriteToFile)
                    DispatcherObject.Invoke(new Action(() => writeToFile(tempEvent)));
                if (Events.Count >= SettingsManager.Current.EventLogger_AmountSavedLogs)
                {
                    DispatcherObject.Invoke(new Action(() => Events.RemoveAt(0)));
                }
            }
            else
            {
                Events.Insert(0, tempEvent);
                if (WriteToFile)
                    writeToFile(tempEvent);
                if (Events.Count >= SettingsManager.Current.EventLogger_AmountSavedLogs)
                {
                    Events.RemoveAt(0);
                }
            }

            //No infos to user about debugging
            if (log[1].ToLower().Equals("error") || log[1].ToLower().Equals("info"))
            {
                //CurrentEvent = tempEvent;
            }
        }

        private void writeToFile(Event curEvent)
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                using (StreamWriter sw = new StreamWriter(Path.Combine(path, "debug.log"), true))
                {
                    sw.WriteLine(string.Format("{0}: {1}", curEvent.Category, curEvent.FullMessage));
                }

            }
            catch (Exception exc)
            {
                //Events.Add(new Event() { Category = "Error", FullMessage = exc.Message});
            }
        }
        #endregion
    }
    /// <summary>
    /// It simply carries out the usual action of adding the event that occurs but it also calls the Updated event with blank EventArgs 
    /// </summary>
    public class MemoryAppenderWithEvents : MemoryAppender
    {

        /// <summary>
        /// Represents the method that will handle new logevents
        /// </summary>
        public event EventHandler Updated;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggingEvent"></param>
        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            // Append the event as usual
            base.Append(loggingEvent);

            // Then alert the Updated event that an event has occurred
            var handler = Updated;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
    }

    public class Event
    {
        private string message;
        /// <summary>
        /// The Message contains the textinformation for the UI. This can be errormessages or activity informations.
        /// </summary>
        public string FullMessage
        {
            get { return message; }
            set
            {
                message = value;
                timeStamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            }
        }

        /// <summary>
        /// The ShortMessage contains a limited information of max 60 characters
        /// </summary>
        public string ShortMessage
        {
            get
            {
                return FullMessage.Length > 60 ? FullMessage.Substring(0, Math.Min(60, FullMessage.Length)) + "..." : FullMessage;
            }
        }

        private string timeStamp;
        /// <summary>
        /// The TimeStamp contains the time when the message appeared. The format of is "dd.MM.yyyy HH:mm:ss"
        /// </summary>
        public string TimeStamp
        {
            get { return timeStamp; }
        }

        private bool error;
        /// <summary>
        /// The Error contains true if the message has the category error.
        /// </summary>
        public bool Error
        {
            get { return error; }
            set
            {
                error = value;
            }
        }

        private string category;
        /// <summary>
        /// The Category contains the kind of message e.g Error, Debug
        /// </summary>
        public string Category
        {
            get { return category; }
            set { category = value; }
        }


    }
}
