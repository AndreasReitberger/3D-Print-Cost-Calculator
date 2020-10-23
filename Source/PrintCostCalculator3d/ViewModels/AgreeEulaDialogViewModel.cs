using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Utilities;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using log4net;
using PrintCostCalculator3d.Resources.Localization;

namespace PrintCostCalculator3d.ViewModels
{

    class AgreeEulaDialogViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        private Guid _id = Guid.NewGuid();
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

        private bool _eula;
        public bool EULA
        {
            get => SettingsManager.Current.AgreedEULA;
            set
            {
                if (SettingsManager.Current.AgreedEULA != value)
                {
                    SettingsManager.Current.AgreedEULA = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime _eulaDate;
        public DateTime EULADate
        {
            get => SettingsManager.Current.AgreedEULAOn;
            set
            {
                if (SettingsManager.Current.AgreedEULAOn != value)
                {
                    SettingsManager.Current.AgreedEULAOn = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _eulaContent = string.Empty;
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

        #region Constructor
        public AgreeEulaDialogViewModel(Action<AgreeEulaDialogViewModel> saveCommand, Action<AgreeEulaDialogViewModel> cancelHandler)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            LoadSettings();
            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }
        public AgreeEulaDialogViewModel(Action<AgreeEulaDialogViewModel> saveCommand, Action<AgreeEulaDialogViewModel> cancelHandler, IDialogCoordinator dialogCoordinator)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            this._dialogCoordinator = dialogCoordinator;

            LoadSettings();
            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }

        private void LoadSettings()
        {
            try
            {
                EULAContent = File.ReadAllText(
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GlobalStaticConfiguration.eulaLocalPath)
                    );
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
