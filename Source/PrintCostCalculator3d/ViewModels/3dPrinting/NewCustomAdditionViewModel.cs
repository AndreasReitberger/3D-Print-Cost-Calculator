using log4net;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using AndreasReitberger.Enums;
using AndreasReitberger.Models.CalculationAdditions;

namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    public class NewCustomAdditionViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        private bool _isEdit;
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

        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private int _order = 0;
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

        private double _percentage = 0;
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

        private CustomAdditionCalculationType _calculationType = CustomAdditionCalculationType.AfterApplingMargin;
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

            this._dialogCoordinator = dialogCoordinator;

            IsEdit = addition != null;
            try
            {
                var customAddition = addition ?? new CustomAddition();
                Name = customAddition.Name;
                Percentage = customAddition.Percentage;
                Order = customAddition.Order;
                CalculationType = customAddition.CalculationType;

                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
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
