//using nUpdate.Updating;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Models.Update;

namespace PrintCostCalculator3d.Models.nUpdate
{
    public class nUpdateManager : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Properties

        private bool _checksForUpdates = false;
        public bool ChecksForUpdates
        {
            get => _checksForUpdates;
            set
            {
                if (_checksForUpdates != value)
                {
                    _checksForUpdates = value;
                    OnPropertyChanged();
                }
            }
        }
/*
        private UpdateManager _manager;
        public UpdateManager Manager
        {
            get => _manager;
            set
            {
                if(_manager == value) return;
                _manager = value;
                OnPropertyChanged();
            }
        }

        private UpdaterUI _ui;
        public UpdaterUI UI
        { 
            get => _ui; 
            set
            {
                if (_ui == value) return;
                _ui = value;
                OnPropertyChanged();
            }
        }
*/
        #endregion

        public nUpdateManager()
        { 
            /*
            Manager = new UpdateManager(new Uri("https://shatter-box.com/updater/updates.json"), 
                "<RSAKeyValue><Modulus>tJ0A55/uUqPE4yuBBP4oRyE/zx3vm11CU2+3LgDLr/PJp3zteJGZuz7+m81dC0F9JF5Uw8TJqeMd/98hpTgwWK4w1ny+qS81Ph09n7IIeyv9qC3poxVELwOzYCnYG9G2XoBY/88uPHFX7wclXxDugu6CXh4RyeM6BPVFir276lUy0Znj1tuHJkpVVgj9wGa6qtJkwxTfu/3M1jhveJS1QBxsJSvvHlztRPv1nRg20k2PpXrArwoYLP34PU3HkHl0YqeN1T8/1VHqSiu8T/hHLnRy9SyfkhnYrbSJYtdHdB9eSQgLNusLIHsB5nO1OHiClRrHLgUvEuzOHEvEUoMthEy2CH0PH4RuFxvreqLmcnUqAQDxCWYLrlKldGTRyqQNw5RlP/61dT9KTVmJp+p2YgiZUE6Uc+Ehhf1KyyBcdBiBOX041HRA6V2bD7ZWe6GUev0FlVGro2xgpNYhhWVOgB0q0MtAaA5qvZQuHCPSkD2QutkJrmsSDrEF1QW4v0FoHpa3qwqAJLYQ0fA0nu6rc+gjNVFu5u6EvY7vOGs4qQevd6NLSxRxK4XJYCpCg+tu8XvUUcvlc4Zln3eXzu19utsoS5Xf2BTvtIbOUiJQfKmYxhgNDQ/QbMFdOO4sCuGPaaH4IfwxvlpX2vvJvPaXsJUgDR+LGMt7Iu7yyzgS+VxSQxRPk5/p3yks03uUj1xi7WRDtPFaTJ7jXqouLibLFNVQDojspvgjINKBB0zaqquKeIeMjHXtjYxOe0J8+lKZ8BJbfWnV3WMZVPnLejLWB+ngaFwutgdB1/PmcCwe4MKv+IuOWHJZ0WKrdmCWZMm5i25mQg6PDRY6z8FnFUI7AQRO3C0BoUSLrpOpcr69WHdxfRiHrhLJDwxpCT0nQkVkr6ZvkL6g8lq/ZUk7N/ht5WwzBarZjqjCgQaBklE+jf50V8VY9iTNHuuYsRKFrWpugG9FMnpeDVIJGen/VAhW6xuetKiwZe/q9z7ZDuYk6Qid7R4obRQGV8a0UAWIbvzq5kw+3sHk1oRh14EEs7V6dqRigVDpSy0GU/qCOT2LvQn271G7YM/ZP5yzVVFQ0V6Ifn1vxe3hEoDlu7T96WFzqxbl7jKWxOky3IrGNpAKHwSOepPCz++aVT0kEsgqkE0NW3bp6zHmAIRncchfDQegwqLN3guz0yYjRgUeAo0yMpWKJSLHk5EWtM648xGc+muxfyBSKGksmPSeXuhFk47Jpb4cHpTr00+OROaF5EjHZgn9NL0LRVdJUMe4ShctABlsVcZ1CLXnEUAy5OsSN82PqUql4B4iyMt82evtcw7uMKZN8lCZ4CjUUJXZF69YSICC7GsClEth53ZV55RBWei3LQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>",
               new CultureInfo("en"));
            // LocalizationManager.GetInstance(SettingsManager.Current.Localization_CultureCode).Culture
            UI = new UpdaterUI(Manager, SynchronizationContext.Current);
            */
        }

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
        public void ShowUpdateUi()
        {
            //UI.ShowUserInterface();
        }
        #endregion
    }
}
