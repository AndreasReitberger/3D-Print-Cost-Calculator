using AndreasReitberger.Enums;
using AndreasReitberger.Models.CalculationAdditions;
using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using System;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    public class NewCustomAdditionViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Properties
        bool _isEdit;
        public bool IsEdit
        {
            get => _isEdit;
            set
            {
                if (value == _isEdit)
                    return;

                _isEdit = value;
                OnPropertyChanged();
            }
        }

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

        string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        int _order = 0;
        public int Order
        {
            get => _order;
            set
            {
                if (_order == value) return;

                _order = value;
                OnPropertyChanged();
               
            }
        }

        double _percentage = 0;
        public double Percentage
        {
            get => _percentage;
            set
            {
                if (_percentage == value) return;

                _percentage = value;
                OnPropertyChanged();
                
            }
        }

        CustomAdditionCalculationType _calculationType = CustomAdditionCalculationType.AfterApplingMargin;
        public CustomAdditionCalculationType CalculationType
        {
            get => _calculationType;
            set
            {
                if (_calculationType != value)
                {
                    _calculationType = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Constructor

        public NewCustomAdditionViewModel(Action<NewCustomAdditionViewModel> saveCommand, 
            Action<NewCustomAdditionViewModel> cancelHandler, IDialogCoordinator dialogCoordinator, CustomAddition addition = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            _dialogCoordinator = dialogCoordinator;

            IsLicenseValid = false;

            IsEdit = addition != null;
            try
            {
                var customAddition = addition ?? new CustomAddition();
                Name = customAddition.Name;
                Percentage = customAddition.Percentage;
                Order = customAddition.Order;
                CalculationType = customAddition.CalculationType;

                logger.Info(string.Format(Strings.EventViewInitFormated, GetType().Name));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        #endregion

        #region iCommands & Actions
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
