using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Utilities;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using log4net;
using PrintCostCalculator3d.Resources.Localization;
using AndreasReitberger.Models;
using AndreasReitberger;
using AndreasReitberger.Utilities;

namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    class CalculationResultsViewModel : ViewModelBase
    {

        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        private Calculation3d _selectedCalculation;
        public Calculation3d SelectedCalculation
        {
            get => _selectedCalculation;
            set
            {
                if (_selectedCalculation != value)
                {
                    _selectedCalculation = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _hasWorksteps = false;
        public bool HasWorksteps
        {
            get => _hasWorksteps;
            set
            {
                if (_hasWorksteps == value) return;

                _hasWorksteps = value;
                OnPropertyChanged();

            }
        }

        private double _price = 0;
        public double Price
        {
            get => _price;
            set
            {
                if (_price == value) return;

                _price = value;
                OnPropertyChanged();

            }
        }

        private double _totalMachineCosts = 0;
        public double TotalMachineCosts
        {
            get => _totalMachineCosts;
            set
            {
                if (_totalMachineCosts == value) return;

                _totalMachineCosts = value;
                OnPropertyChanged();

            }
        }

        private double _totalMaterialCosts = 0;
        public double TotalMaterialCosts
        {
            get => _totalMaterialCosts;
            set
            {
                if (_totalMaterialCosts == value) return;

                _totalMaterialCosts = value;
                OnPropertyChanged();

            }
        }

        private double _totalWorkstepCosts = 0;
        public double TotalWorkstepCosts
        {
            get => _totalWorkstepCosts;
            set
            {
                if (_totalWorkstepCosts == value) return;

                _totalWorkstepCosts = value;
                OnPropertyChanged();

            }
        }

        private double _totalCustomAdditionsCosts = 0;
        public double TotalCustomAdditionsCosts
        {
            get => _totalCustomAdditionsCosts;
            set
            {
                if (_totalCustomAdditionsCosts == value) return;

                _totalCustomAdditionsCosts = value;
                OnPropertyChanged();

            }
        }

        private double _totalRatesCosts = 0;
        public double TotalRatesCosts
        {
            get => _totalRatesCosts;
            set
            {
                if (_totalRatesCosts == value) return;

                _totalRatesCosts = value;
                OnPropertyChanged();

            }
        }

        private ObservableCollection<Calculation3d> _calculations;
        public ObservableCollection<Calculation3d> Calculations
        {
            get => _calculations;
            set
            {
                if (_calculations != value)
                {
                    _calculations = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Chart
        private ObservableCollection<ChartItem> _costs = new ObservableCollection<ChartItem>();
        public ObservableCollection<ChartItem> Costs
        {
            get => _costs;
            set
            {
                if (_costs == value) return;

                _costs = value;
                OnPropertyChanged();

            }
        }

        private ObservableCollection<ChartItem> _machineCosts = new ObservableCollection<ChartItem>();
        public ObservableCollection<ChartItem> MachineCosts
        {
            get => _machineCosts;
            set
            {
                if (_machineCosts == value) return;

                _machineCosts = value;
                OnPropertyChanged();

            }
        }

        private ObservableCollection<ChartItem> _materialUsage = new ObservableCollection<ChartItem>();
        public ObservableCollection<ChartItem> MaterialUsage
        {
            get => _materialUsage;
            set
            {
                if (_materialUsage == value) return;

                _materialUsage = value;
                OnPropertyChanged();

            }
        }

        private ObservableCollection<ChartItem> _materialCosts = new ObservableCollection<ChartItem>();
        public ObservableCollection<ChartItem> MaterialCosts
        {
            get => _materialCosts;
            set
            {
                if (_materialCosts == value) return;

                _materialCosts = value;
                OnPropertyChanged();

            }
        }

        private ObservableCollection<ChartItem> _workstepCosts = new ObservableCollection<ChartItem>();
        public ObservableCollection<ChartItem> WorkstepCosts
        {
            get => _workstepCosts;
            set
            {
                if (_workstepCosts == value) return;

                _workstepCosts = value;
                OnPropertyChanged();

            }
        }

        private ObservableCollection<ChartItem> _customAdditionsCosts = new ObservableCollection<ChartItem>();
        public ObservableCollection<ChartItem> CustomAdditionsCosts
        {
            get => _customAdditionsCosts;
            set
            {
                if (_customAdditionsCosts == value) return;

                _customAdditionsCosts = value;
                OnPropertyChanged();

            }
        }

        private ObservableCollection<ChartItem> _ratesCosts = new ObservableCollection<ChartItem>();
        public ObservableCollection<ChartItem> RatesCosts
        {
            get => _ratesCosts;
            set
            {
                if (_ratesCosts == value) return;

                _ratesCosts = value;
                OnPropertyChanged();

            }
        }
        #endregion

        #region Constructor
        public CalculationResultsViewModel(Action<CalculationResultsViewModel> saveCommand, Action<CalculationResultsViewModel> cancelHandler, Calculation3d Calculation)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            this.SelectedCalculation = Calculation;

            loadDetails();

            //Calculations.CollectionChanged += Calculations_CollectionChanged;
        }
        public CalculationResultsViewModel(Action<CalculationResultsViewModel> saveCommand, Action<CalculationResultsViewModel> cancelHandler, Calculation3d Calculation, IDialogCoordinator dialogCoordinator)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            this.SelectedCalculation = Calculation;
            this._dialogCoordinator = dialogCoordinator;

            loadDetails();
            //Calculations.CollectionChanged += Calculations_CollectionChanged;
            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }

        #endregion

        #region Events
        private void Calculations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //SettingsManager.Save();
            OnPropertyChanged(nameof(Calculations));
        }
        #endregion

        #region ICommands & Actions
        public ICommand SaveCalculationCommand
        {
            get => new RelayCommand(async(p) => await SaveCalculationAction());
        }
        private async Task SaveCalculationAction()
        {
            try
            {
                var saveFileDialog = new System.Windows.Forms.SaveFileDialog
                {
                    Filter = StaticStrings.FilterCalculationFileLibrary,
                };
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var res = Calculator3dExporter.EncryptAndSerialize(saveFileDialog.FileName, this.SelectedCalculation);
                    if (res)
                    {
                        await this._dialogCoordinator.ShowMessageAsync(this,
                            Strings.DialogCalculationSaveSuccessHeadline,
                            Strings.DialogCalculationSaveSuccessContent
                            );
                        logger.Info(string.Format(Strings.EventCalculationSavedFormated, SelectedCalculation.Name, saveFileDialog.FileName));
                    }
                }
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        public ICommand SelectedPrinterChangedCommand
        {
            get => new RelayCommand(async(p) => await SelectedPrinterChangedAction());
        }
        private async Task SelectedPrinterChangedAction()
        {
            try
            {
                if(SelectedCalculation != null)
                {
                    // Reload details
                    if (SelectedCalculation.Printer != null)
                        loadDetails();

                }
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        public ICommand SelectedMaterialChangedCommand
        {
            get => new RelayCommand(async(p) => await SelectedMaterialChangedAction());
        }
        private async Task SelectedMaterialChangedAction()
        {
            try
            {
                if(SelectedCalculation != null)
                {
                    // Reload details
                    if (SelectedCalculation.Material != null)
                        loadDetails();
                }
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion

        #region Methods
        private void loadDetails()
        {
            HasWorksteps = SelectedCalculation.WorkSteps.Count > 0;

            Costs = PrintCalculator3d.getCosts(SelectedCalculation);

            MaterialCosts = PrintCalculator3d.getMaterialCosts(SelectedCalculation);
            TotalMaterialCosts = MaterialCosts.Sum(i => i.Value);
            foreach (var cost in MaterialCosts)
                Costs.Add(cost);

            MachineCosts = PrintCalculator3d.getMachineCosts(SelectedCalculation);
            TotalMachineCosts = MachineCosts.Sum(i => i.Value);
            foreach (var cost in MachineCosts)
                Costs.Add(cost);

            WorkstepCosts = PrintCalculator3d.getWorkstepCosts(SelectedCalculation);
            TotalWorkstepCosts = WorkstepCosts.Sum(i => i.Value);

            CustomAdditionsCosts = PrintCalculator3d.getCustomAdditionsCosts(SelectedCalculation);
            TotalCustomAdditionsCosts = CustomAdditionsCosts.Sum(i => i.Value);

            RatesCosts = PrintCalculator3d.getRatesCosts(SelectedCalculation);
            TotalRatesCosts = RatesCosts.Sum(i => i.Value);
            
        }
        #endregion
    }
}
