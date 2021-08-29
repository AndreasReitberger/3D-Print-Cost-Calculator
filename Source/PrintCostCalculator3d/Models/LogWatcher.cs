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
        string logContent;
        
        MemoryAppenderWithEvents memoryAppender;

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
            IAppender[] repos = LogManager.GetRepository().GetAppenders();

            // Read in the log content
            logContent = GetEvents(memoryAppender);

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
            logContent = GetEvents(memoryAppender);
            // Then alert the Updated event that the LogWatcher has been updated
            Updated?.Invoke(this, new EventArgs());
        }


        static bool GetMemoryAppender(IAppender appender)
        {
            // Returns the IAppender named MemoryAppender in the Log4Net.config file
            return appender.Name.Equals("MemoryAppender");
        }

        /// <summary>
        /// Get any events that may have occurred, check that there are events to return
        /// </summary>
        /// <param name="memoryAppender"></param>
        /// <returns></returns>
        public string GetEvents(MemoryAppenderWithEvents memoryAppender)
        {
            StringBuilder output = new();

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
                    _ = output.Append(line);
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
        LogWatcher logWatcher = new();
        //ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public virtual Dispatcher DispatcherObject 
        { 
            get; 
            protected set; 
        }

        static Logger _instance;
        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                }
                return _instance;
            }
        }
        #endregion

        #region Properties
        bool _writeToFile = false;
        public bool WriteToFile
        {
            get => _writeToFile;
            set
            {
                if (_writeToFile == value) return;
                _writeToFile = value;
                OnPropertyChanged();
            }
        }

        string _debugLogFile = "";
        public string DebugLogFile
        {
            get => _debugLogFile;
            set
            {
                if (_debugLogFile == value) return;
                _debugLogFile = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<Event> _events = new();
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
            logWatcher.Updated += LogWatcher_Updated;

            WriteToFile = true;

            CreateLogDir();
            DeleteLogFile();
        }
        #endregion

        #region Methods
        void LogWatcher_Updated(object sender, EventArgs e)
        {
            if (!SettingsManager.Current.EventLogger_EnableLogging)
            {
                return;
            }
            if (string.IsNullOrEmpty(logWatcher.LogContent))
            {
                return;
            }

            string[] log = logWatcher.LogContent.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
            if (log.Count() == 0)
            {
                return;
            }

            Event tempEvent = new();
            tempEvent.FullMessage = (log.Count() >= 3) ? log[2] : "";
            tempEvent.Error = log[1].ToLower().Contains("error");
            tempEvent.Category = log[1].ToLower() switch
            {
                "info" => Strings.Info,
                "warning" => Strings.Warning,
                "error" => Strings.Error,
                _ => log[1],
            };
            if (DispatcherObject.Thread != Thread.CurrentThread)
            {
                DispatcherObject.Invoke(new Action(() => Events.Add(tempEvent)));
                if (WriteToFile)
                {
                    DispatcherObject.Invoke(new Action(() => WriteEventToFile(tempEvent)));
                }
                if (Events.Count >= SettingsManager.Current.EventLogger_AmountSavedLogs)
                {
                    DispatcherObject.Invoke(new Action(() => Events.RemoveAt(0)));
                }
            }
            else
            {
                Events.Insert(0, tempEvent);
                if (WriteToFile)
                {
                    WriteEventToFile(tempEvent);
                }
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

        void DeleteLogFile()
        {
            try
            {
                if (File.Exists(DebugLogFile))
                {
                    File.Delete(DebugLogFile);
                }
            }
            catch (Exception)
            {

            }
        }

        void CreateLogDir()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"3dPrintCostCalculator\Log");
                if (!Directory.Exists(path))
                {
                    _ = Directory.CreateDirectory(path);
                }
                DebugLogFile = Path.Combine(path, "debug.log");
            }
            catch(Exception)
            {

            }
        }

        void WriteEventToFile(Event curEvent)
        {
            try
            {
                //if (File.Exists(file))
                //    File.Delete(file);
                //using FileStream fs = new(DebugLogFile, FileMode.OpenOrCreate, FileAccess.Write);
                using StreamWriter sw = new(DebugLogFile, true);
                sw.WriteLine($"{DateTime.Now}: {curEvent.Category} - {curEvent.FullMessage}");

            }
            catch (Exception)
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
        protected override void Append(LoggingEvent loggingEvent)
        {
            // Append the event as usual
            base.Append(loggingEvent);
            // Then alert the Updated event that an event has occurred
            Updated?.Invoke(this, new EventArgs());
        }
    }

    public class Event
    {
        string message;
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

        string timeStamp;
        /// <summary>
        /// The TimeStamp contains the time when the message appeared. The format of is "dd.MM.yyyy HH:mm:ss"
        /// </summary>
        public string TimeStamp
        {
            get { return timeStamp; }
        }

        bool error;
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

        string category;
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
