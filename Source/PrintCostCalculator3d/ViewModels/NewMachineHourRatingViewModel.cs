using log4net;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using PrintCostCalculator3d.ViewModels._3dPrinting;
using AndreasReitberger.Models;

namespace PrintCostCalculator3d.ViewModels
{
    class NewMachineHourRatingViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool _isLoading = false;
        #endregion

        #region Properties
        private Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get => _id;
            set
            {
                if (_id == value) return;
                _id = value;
                OnPropertyChanged();
                
            }
        }

        private bool _isEdit = false;
        public bool IsEdit
        {
            get => _isEdit;
            set
            {
                if (_isEdit == value) return;
                
                _isEdit = value;
                OnPropertyChanged();
                
            }
        }

        private Printer3d _printer;
        public Printer3d Printer
        {
            get => _printer;
            set
            {
                if (_printer == value) return;
                _printer = value;
                OnPropertyChanged();
            }
        }

        private string _name = string.Empty;
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

        private double _machineHours = 0;
        public double MachineHours
        {
            get => _machineHours;
            set
            {
                if (_machineHours == value) return;

                _machineHours = value;
                OnPropertyChanged();

                // Dependencies
                calculateMachineHourRate();
                calculateTotalCosts();

            }
        }

        private bool _isPerYear = false;
        public bool IsPerYear
        {
            get => _isPerYear;
            set
            {
                if (_isPerYear == value) return;

                _isPerYear = value;
                OnPropertyChanged();

                // Dependencies
                calculateDepreciation();
                calculateInterest();
                calculateMachineHourRate();
                calculateTotalCosts();
            }
        }

        private int _usefulLifeYears = 4;
        public int UsefulLifeYears
        {
            get => _usefulLifeYears;
            set
            {
                if (_usefulLifeYears == value) return;

                _usefulLifeYears = value;
                OnPropertyChanged();

                // Dependencies
                calculateDepreciation();
                calculateMachineHourRate();
                calculateTotalCosts();
            }
        }
        
        private double _depreciation = 0;
        public double Depreciation
        {
            get => _depreciation;
            set
            {
                if (_depreciation == value) return;

                _depreciation = value;
                OnPropertyChanged();
                
            }
        }

        private double _interestRate = 3;
        public double InterestRate
        {
            get => _interestRate;
            set
            {
                if (_interestRate == value) return;

                _interestRate = value;
                OnPropertyChanged();

                // Dependencies
                calculateInterest();
                calculateMachineHourRate();
                calculateTotalCosts();
            }
        }
        private double _interest = 0;
        public double Interest
        {
            get => _interest;
            set
            {
                if (_interest == value) return;

                _interest = value;
                OnPropertyChanged();
            }
        }
        
        private double _replacementCosts = 0;
        public double ReplacementCosts
        {
            get => _replacementCosts;
            set
            {
                if (_replacementCosts == value) return;

                _replacementCosts = value;
                OnPropertyChanged();

                // Dependencies
                calculateDepreciation();
                calculateInterest();
                calculateMachineHourRate();
                calculateTotalCosts();
            }
        }
        
        private double _maintenanceCosts = 0;
        public double MaintenanceCosts
        {
            get => _maintenanceCosts;
            set
            {
                if (_maintenanceCosts == value) return;

                _maintenanceCosts = value;
                OnPropertyChanged();

                // Dependencies
                calculateMachineHourRate();
                calculateTotalCosts();

            }
        }

        private double _locationCosts = 0;
        public double LocationCosts
        {
            get => _locationCosts;
            set
            {
                if (_locationCosts == value) return;

                _locationCosts = value;
                OnPropertyChanged();

                // Dependencies
                calculateMachineHourRate();
                calculateTotalCosts();
            }
        }

        private double _energyCosts = 0;
        public double EnergyCosts
        {
            get => _energyCosts;
            set
            {
                if (_energyCosts == value) return;

                _energyCosts = value;
                OnPropertyChanged();

                // Dependencies
                calculateMachineHourRate();
                calculateTotalCosts();
            }
        }

        private double _additionalCosts = 0;
        public double AdditionalCosts
        {
            get => _additionalCosts;
            set
            {
                if (_additionalCosts == value) return;

                _additionalCosts = value;
                OnPropertyChanged();

                // Dependencies
                calculateMachineHourRate();
                calculateTotalCosts();
            }
        }

        private double _maintenanceCostsVariable = 0;
        public double MaintenanceCostsVariable
        {
            get => _maintenanceCostsVariable;
            set
            {
                if (_maintenanceCostsVariable == value) return;

                _maintenanceCostsVariable = value;
                OnPropertyChanged();

                // Dependencies
                calculateMachineHourRate();
                calculateTotalCosts();
            }
        }

        private double _energyCostsVariable = 0;
        public double EnergyCostsVariable
        {
            get => _energyCostsVariable;
            set
            {
                if (_energyCostsVariable == value) return;

                _energyCostsVariable = value;
                OnPropertyChanged();

                // Dependencies
                calculateMachineHourRate();
                calculateTotalCosts();
            }
        }

        private double _additionalCostsVariable = 0;
        public double AdditionalCostsVariable
        {
            get => _additionalCostsVariable;
            set
            {
                if (_additionalCostsVariable == value) return;

                _additionalCostsVariable = value;
                OnPropertyChanged();

                // Dependencies
                calculateMachineHourRate();
                calculateTotalCosts();
            }
        }

        private double _machineHourRate = 0;
        public double MachineHourRate
        {
            get => _machineHourRate;
            private set
            {
                if (_machineHourRate == value) return;

                _machineHourRate = value;
                OnPropertyChanged();
                
            }
        }
        
        private double _totalCosts = 0;
        public double TotalCosts
        {
            get => _totalCosts;
            private set
            {
                if (_totalCosts == value) return;

                _totalCosts = value;
                OnPropertyChanged();
                
            }
        }

        #endregion

        #region Constructor
        public NewMachineHourRatingViewModel(Action<NewMachineHourRatingViewModel> saveCommand, Action<NewMachineHourRatingViewModel> cancelHandler,
            HourlyMachineRate mhr = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            IsEdit = mhr != null;
            try
            {
                LoadItem(mhr ?? new HourlyMachineRate());
                LoadData();

                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }

        }

        public NewMachineHourRatingViewModel(Action<NewMachineHourRatingViewModel> saveCommand, Action<NewMachineHourRatingViewModel> cancelHandler, 
            IDialogCoordinator dialogCoordinator, HourlyMachineRate mhr = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            this._dialogCoordinator = dialogCoordinator;

            IsEdit = mhr != null;
            try
            {
                LoadItem(mhr ?? new HourlyMachineRate());
                LoadData();

                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public void LoadData()
        {
            calculateDepreciation();
            calculateInterest();
        }

        #endregion

        #region Methods
        private void LoadItem(HourlyMachineRate machineRate)
        {
            // Load Id if material is not null
            if (machineRate != null)
                Id = machineRate.Id;

            Name = machineRate.Name;
            IsPerYear = machineRate.PerYear;
            MachineHours = machineRate.MachineHours;
            ReplacementCosts = machineRate.ReplacementCosts;
            UsefulLifeYears = machineRate.UsefulLifeYears;
            InterestRate = machineRate.InterestRate;
            MaintenanceCosts = machineRate.MaintenanceCosts;
            LocationCosts = machineRate.LocationCosts;
            EnergyCosts = machineRate.EnergyCosts;
            AdditionalCosts = machineRate.AdditionalCosts;
            MaintenanceCostsVariable = machineRate.MaintenanceCostsVariable;
            EnergyCostsVariable = machineRate.EnergyCostsVariable;
            AdditionalCostsVariable = machineRate.AdditionalCostsVariable;
            MachineHourRate = machineRate.CalcMachineHourRate;
        }
        private double calculateDepreciation()
        {
            Depreciation = 0;

            if (ReplacementCosts > 0 && UsefulLifeYears > 0)
                Depreciation = Math.Round(ReplacementCosts / UsefulLifeYears, 2);

            return Depreciation;
        }
        private double calculateInterest()
        {
            Interest = 0;

            if (ReplacementCosts > 0 && InterestRate > 0)
                Interest = Math.Round((ReplacementCosts / 2) / 100 * InterestRate, 2);

            return Interest;
        }
        private double calculateMachineHourRate()
        {
            MachineHourRate = 0;
            try
            {
                MachineHourRate = (Depreciation + Interest + (MaintenanceCosts + LocationCosts + EnergyCosts + AdditionalCosts) * (IsPerYear ? 1 : 12)
                    + (MaintenanceCostsVariable + EnergyCostsVariable + AdditionalCostsVariable) * (IsPerYear ? 1 : 12)) / (MachineHours * (IsPerYear ? 1 : 12));
                return MachineHourRate;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        private double calculateTotalCosts()
        {
            TotalCosts = 0;
            try
            {
                TotalCosts = (ReplacementCosts + (Interest +
                    ((MaintenanceCosts + LocationCosts + EnergyCosts + AdditionalCosts)
                    + (MaintenanceCostsVariable + EnergyCostsVariable + AdditionalCostsVariable)) * (IsPerYear ? 1 : 12))
                    * UsefulLifeYears);
                return TotalCosts;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        #endregion

        #region iCommands & Actions

        public ICommand SelectPrinterToLoadCommand
        {
            get { return new RelayCommand(async(p) => await SelectPrinterToLoadAction()); }
        }
        private async Task SelectPrinterToLoadAction()
        {
            try
            {

                var _dialog = new CustomDialog() { Title = Strings.LabelSelectPrinter };
                var newViewModel = new SelectPrinterViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Printer = instance.SelectedPrinter;
                    Name = Printer.Name;
                    ReplacementCosts = Printer.Price;

                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                _dialogCoordinator
                );

                _dialog.Content = new Views._3dPrinting.SelectPrinterDialog()
                {
                    DataContext = newViewModel
                };
                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                await _dialogCoordinator.ShowMessageAsync(this,
                    Strings.DialogExceptionHeadline,
                    string.Format(Strings.DialogExceptionFormatedContent, exc.Message)
                    );
            }
        }

        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
