using PrintCostCalculator3d.Models;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels
{
    class SettingsEULAViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        //readonly bool _isLoading;

        #endregion

        #region Properties
        public bool EULA
        {
            get => SettingsManager.Current.AgreedEULA;
            set
            {
                SettingsManager.Current.AgreedEULA = value;
                OnPropertyChanged();
            }
        }
        public DateTime EULADate
        {
            get => SettingsManager.Current.AgreedEULAOn;
            set
            {
                SettingsManager.Current.AgreedEULAOn = value;
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

        #region Constructor, Load Settings
        public SettingsEULAViewModel(IDialogCoordinator instance)
        {
            _dialogCoordinator = instance;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;
        }
        void LoadSettings()
        {
            try
            {
                EULAContent = File.ReadAllText(
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GlobalStaticConfiguration.eulaLocalPath)
                    );
            }
            catch(Exception)
            {
                EULAContent = "";
            }

        }
        #endregion

        #region iCommands & Actions
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }

        public ICommand ShowEULAOnlineCommand
        {
            get { return new RelayCommand(p => ShowEULAOnlineAction()); }
        }

        void ShowEULAOnlineAction()
        {
            try
            {
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GlobalStaticConfiguration.eulaLocalPath);
                Process.Start(path);
            }
            catch(Exception)
            {

            }
        }
        public ICommand ShowEULACommand
        {
            get { return new RelayCommand(p => ShowEULAOnlineAction()); }
        }

        void ShowEULAAction()
        {
            try
            {
                Process.Start(GlobalStaticConfiguration.eulaOnlinePath);
            }
            catch (Exception)
            {

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
