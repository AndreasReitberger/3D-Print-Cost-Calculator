using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels
{

    class AgreeEulaDialogViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Properties
        Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        bool _eula;
        public bool EULA
        {
            get => _eula;
            set
            {
                if (_eula == value) return;
                if (!IsLoading)
                    SettingsManager.Current.AgreedEULA = value;
                _eula = value;
                OnPropertyChanged();              
            }
        }

        DateTime _eulaDate = DateTime.Now;
        public DateTime EULADate
        {
            get => _eulaDate;
            set
            {
                if (_eulaDate == value) return;
                if(!IsLoading!)
                    SettingsManager.Current.AgreedEULAOn = value;
                _eulaDate = value;
                OnPropertyChanged();
            }
        }

        string _eulaContent = string.Empty;
        public string EULAContent
        {
            get => _eulaContent;
            set
            {
                _eulaContent = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructor, LoadSettings
        public AgreeEulaDialogViewModel(Action<AgreeEulaDialogViewModel> saveCommand, Action<AgreeEulaDialogViewModel> cancelHandler)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }
        public AgreeEulaDialogViewModel(Action<AgreeEulaDialogViewModel> saveCommand, Action<AgreeEulaDialogViewModel> cancelHandler, IDialogCoordinator dialogCoordinator)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            _dialogCoordinator = dialogCoordinator;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }

        void LoadSettings()
        {
            try
            {
                EULA = SettingsManager.Current.AgreedEULA;
                EULADate = SettingsManager.Current.AgreedEULAOn;
                EULAContent = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GlobalStaticConfiguration.eulaLocalPath));
            }
            catch (Exception)
            {
                EULAContent = "";
            }

        }
        #endregion

        #region iCommands & Actions
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
