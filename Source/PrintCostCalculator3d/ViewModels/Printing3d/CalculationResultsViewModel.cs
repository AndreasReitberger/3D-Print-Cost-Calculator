using AndreasReitberger;
using AndreasReitberger.Models;
using AndreasReitberger.Utilities;
using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    class CalculationResultsViewModel : ViewModelBase
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
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        Calculation3d _selectedCalculation;
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

        bool _hasWorksteps = false;
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

        double _price = 0;
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

        double _totalMachineCosts = 0;
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

        double _totalMaterialCosts = 0;
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

        double _totalWorkstepCosts = 0;
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

        double _totalCustomAdditionsCosts = 0;
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

        double _totalRatesCosts = 0;
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

        ObservableCollection<Calculation3d> _calculations;
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
        ObservableCollection<Calculation3dChartItem> _costs = new ObservableCollection<Calculation3dChartItem>();
        public ObservableCollection<Calculation3dChartItem> Costs
        {
            get => _costs;
            set
            {
                if (_costs == value) return;

                _costs = value;
                OnPropertyChanged();

            }
        }

        ObservableCollection<Calculation3dChartItem> _machineCosts = new ObservableCollection<Calculation3dChartItem>();
        public ObservableCollection<Calculation3dChartItem> MachineCosts
        {
            get => _machineCosts;
            set
            {
                if (_machineCosts == value) return;

                _machineCosts = value;
                OnPropertyChanged();

            }
        }

        ObservableCollection<Calculation3dChartItem> _materialUsage = new ObservableCollection<Calculation3dChartItem>();
        public ObservableCollection<Calculation3dChartItem> MaterialUsage
        {
            get => _materialUsage;
            set
            {
                if (_materialUsage == value) return;

                _materialUsage = value;
                OnPropertyChanged();

            }
        }

        ObservableCollection<Calculation3dChartItem> _materialCosts = new ObservableCollection<Calculation3dChartItem>();
        public ObservableCollection<Calculation3dChartItem> MaterialCosts
        {
            get => _materialCosts;
            set
            {
                if (_materialCosts == value) return;

                _materialCosts = value;
                OnPropertyChanged();

            }
        }

        ObservableCollection<Calculation3dChartItem> _workstepCosts = new ObservableCollection<Calculation3dChartItem>();
        public ObservableCollection<Calculation3dChartItem> WorkstepCosts
        {
            get => _workstepCosts;
            set
            {
                if (_workstepCosts == value) return;

                _workstepCosts = value;
                OnPropertyChanged();

            }
        }

        ObservableCollection<Calculation3dChartItem> _customAdditionsCosts = new ObservableCollection<Calculation3dChartItem>();
        public ObservableCollection<Calculation3dChartItem> CustomAdditionsCosts
        {
            get => _customAdditionsCosts;
            set
            {
                if (_customAdditionsCosts == value) return;

                _customAdditionsCosts = value;
                OnPropertyChanged();

            }
        }

        ObservableCollection<Calculation3dChartItem> _ratesCosts = new ObservableCollection<Calculation3dChartItem>();
        public ObservableCollection<Calculation3dChartItem> RatesCosts
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
            SelectedCalculation = Calculation;

            IsLicenseValid = false;

            LoadDetails();
        }
        public CalculationResultsViewModel(Action<CalculationResultsViewModel> saveCommand, Action<CalculationResultsViewModel> cancelHandler, Calculation3d Calculation, IDialogCoordinator dialogCoordinator)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            SelectedCalculation = Calculation;
            _dialogCoordinator = dialogCoordinator;

            IsLicenseValid = false;

            LoadDetails();
            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }

        #endregion

        #region Events
        void Calculations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
        async Task SaveCalculationAction()
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
            get => new RelayCommand((p) => SelectedPrinterChangedAction());
        }
        void SelectedPrinterChangedAction()
        {
            try
            {
                if(SelectedCalculation != null)
                {
                    // Reload details
                    if (SelectedCalculation.Printer != null)
                    {
                        LoadDetails();
                    }
                }
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        public ICommand SelectedMaterialChangedCommand
        {
            get => new RelayCommand((p) => SelectedMaterialChangedAction());
        }
        void SelectedMaterialChangedAction()
        {
            try
            {
                if(SelectedCalculation != null)
                {
                    // Reload details
                    if (SelectedCalculation.Material != null)
                    {
                        LoadDetails();
                    }
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
        void LoadDetails()
        {
            HasWorksteps = SelectedCalculation.WorkSteps.Count > 0;

            Costs = PrintCalculator3d.GetCosts(SelectedCalculation);

            MaterialCosts = PrintCalculator3d.GetMaterialCosts(SelectedCalculation);
            TotalMaterialCosts = MaterialCosts.Sum(i => i.Value);
            foreach (var cost in MaterialCosts)
            {
                Costs.Add(cost);
            }

            MachineCosts = PrintCalculator3d.GetMachineCosts(SelectedCalculation);
            TotalMachineCosts = MachineCosts.Sum(i => i.Value);
            foreach (var cost in MachineCosts)
            {
                Costs.Add(cost);
            }

            WorkstepCosts = PrintCalculator3d.GetWorkstepCosts(SelectedCalculation);
            TotalWorkstepCosts = WorkstepCosts.Sum(i => i.Value);

            CustomAdditionsCosts = PrintCalculator3d.GetCustomAdditionsCosts(SelectedCalculation);
            TotalCustomAdditionsCosts = CustomAdditionsCosts.Sum(i => i.Value);

            RatesCosts = PrintCalculator3d.GetRatesCosts(SelectedCalculation);
            TotalRatesCosts = RatesCosts.Sum(i => i.Value);
            
        }
        #endregion
    }
}
