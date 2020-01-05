using WpfFramework.Models.Settings;
using System;
using System.Threading.Tasks;
using log4net;
using WpfFramework.Resources.Localization;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfFramework.Models.Update
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
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        private bool _checksForUpdates = false;
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
            Task.Run(() =>
            {
                try
                {
                    var version = "0.0.0.0"; // Fetch your current version later from your WebAPI
                    Version latestVersion = new Version(version);

                    if (ConfigurationManager.Current.OSVersion < new Version(10, 0) && latestVersion >= new Version(2, 0))
                    {
                        OnClientIncompatibleWithNewVersion();
                        logger.WarnFormat(Strings.EventOSIncompatibleWithNewVersionFormatedEvent, ConfigurationManager.Current.OSVersion);
                        return;
                    }

                    // Compare versions (tag=v1.4.2.0, version=1.4.2.0)
                    if (latestVersion > AssemblyManager.Current.Version)
                        OnUpdateAvailable(new UpdateAvailableArgs(latestVersion));
                    else
                        OnNoUpdateAvailable();
                   
                }
                catch(Exception exc)
                {
                    //logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
                    OnError();
                }
            });
        }
        #endregion
    }
}
