using PrintCostCalculator3d.Models.Settings;
using System;
using System.Threading.Tasks;
using log4net;
using PrintCostCalculator3d.Resources.Localization;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PrintCostCalculator3d.Models.Update
{
    public class Updater : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Variables
        static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        bool _checksForUpdates = false;
        public bool ChecksForUpdates
        {
            get => _checksForUpdates;
            set
            {
                if(_checksForUpdates != value)
                {
                    _checksForUpdates = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Events
        public event EventHandler<UpdateAvailableArgs> UpdateAvailable;

        protected virtual void OnUpdateAvailable(UpdateAvailableArgs e)
        {
            UpdateAvailable?.Invoke(this, e);
        }

        public event EventHandler NoUpdateAvailable;

        protected virtual void OnNoUpdateAvailable()
        {
            NoUpdateAvailable?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ClientIncompatibleWithNewVersion;

        protected virtual void OnClientIncompatibleWithNewVersion()
        {
            ClientIncompatibleWithNewVersion?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Error;

        protected virtual void OnError()
        {
            Error?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Methods
        public void Check()
        {
            _ = Task.Run(() =>
              {
                  try
                  {
                      string version = "1.0.0"; //Fetch latest version here
                      Version latestVersion = new(version);

                      if (ConfigurationManager.Current.OSVersion < new Version(10, 0) && latestVersion >= new Version(2, 0))
                      {
                          OnClientIncompatibleWithNewVersion();
                          logger.WarnFormat(Strings.EventOSIncompatibleWithNewVersionFormatedEvent, ConfigurationManager.Current.OSVersion);
                          return;
                      }

                    if (latestVersion > AssemblyManager.Current.Version)
                          OnUpdateAvailable(new UpdateAvailableArgs(latestVersion));
                      else
                          OnNoUpdateAvailable();

                  }
                  catch (Exception)
                  {
                    OnError();
                  }
              });
        }
        #endregion
    }
}
