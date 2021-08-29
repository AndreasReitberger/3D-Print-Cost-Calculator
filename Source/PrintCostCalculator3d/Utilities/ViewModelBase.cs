using log4net;
using PrintCostCalculator3d.Models.Settings;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PrintCostCalculator3d.Utilities
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region OnPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (object.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

        #region Logger
        public static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region GlobalProperties
        bool _isLicenseValid = false;
        public bool IsLicenseValid
        {
            get => _isLicenseValid;
            set => SetValue(ref _isLicenseValid, value);
        }

        bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLicenseValid;
            set => SetValue(ref _isLoading, value);
        }
        #endregion
    }
}
