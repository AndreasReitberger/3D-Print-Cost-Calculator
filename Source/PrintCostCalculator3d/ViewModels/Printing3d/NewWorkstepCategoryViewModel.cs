using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using System;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    public class NewWorkstepCategoryViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Properties
        string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged();
                
            }
        }
        #endregion

        #region Constructor
        public NewWorkstepCategoryViewModel(Action<NewWorkstepCategoryViewModel> saveCommand, Action<NewWorkstepCategoryViewModel> cancelHandler)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            IsLicenseValid = false;

            logger.Info(string.Format(Strings.EventViewInitFormated, GetType().Name));
        }
        public NewWorkstepCategoryViewModel(Action<NewWorkstepCategoryViewModel> saveCommand, Action<NewWorkstepCategoryViewModel> cancelHandler, IDialogCoordinator dialogCoordinator)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            _dialogCoordinator = dialogCoordinator;

            IsLicenseValid = false;

            logger.Info(string.Format(Strings.EventViewInitFormated, GetType().Name));
        }

        #endregion

        #region ICommands & Actions
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
