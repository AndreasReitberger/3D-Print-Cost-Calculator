using log4net;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using PrintCostCalculator3d.Models._3dprinting;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using AndreasReitberger.Models;

namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    public class sfChartItem
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }
    class CalculationResultEnhancedViewModel : ViewModelBase
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
        private List<sfChartItem> _costs = new List<sfChartItem>();
        public List<sfChartItem> Costs
        {
            get => _costs;
            private set
            {
                if (_costs == value) return;
                _costs = value;
                OnPropertyChanged();
            }
        }

        private _3dPrinterModel _selectedPrinter;
        public _3dPrinterModel SelectedPrinter
        {
            get => _selectedPrinter;
            set
            {
                if (_selectedPrinter != value)
                {
                    _selectedPrinter = value;
                    OnPropertyChanged();
                }
            }
        }

        private _3dPrinterMaterial _selectedMaterial;
        public _3dPrinterMaterial SelectedMaterial
        {
            get => _selectedMaterial;
            set
            {
                if (_selectedMaterial != value)
                {
                    _selectedMaterial = value;
                    OnPropertyChanged();
                }
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

        #region Settings

        #endregion

        #region Constructor
        public CalculationResultEnhancedViewModel(
            Action<CalculationResultEnhancedViewModel> saveCommand, 
            Action<CalculationResultEnhancedViewModel> cancelHandler,
            Calculation3d Calculation)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            this.SelectedCalculation = Calculation;

            /*
            if (SelectedCalculation.Printer != null)
                SelectedPrinter = SelectedCalculation.Printer;
            if (SelectedCalculation.Material != null)
                SelectedMaterial = SelectedCalculation.Material;
            */
            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            //Calculations.CollectionChanged += Calculations_CollectionChanged;
        }
        public CalculationResultEnhancedViewModel(
            Action<CalculationResultEnhancedViewModel> saveCommand, 
            Action<CalculationResultEnhancedViewModel> cancelHandler,
            Calculation3d Calculation, 
            IDialogCoordinator dialogCoordinator)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            this.SelectedCalculation = Calculation;
            this._dialogCoordinator = dialogCoordinator;
            /*
            if (SelectedCalculation.Printer != null)
                SelectedPrinter = SelectedCalculation.Printer;
            if (SelectedCalculation.Material != null)
                SelectedMaterial = SelectedCalculation.Material;
            */
            Costs = new List<sfChartItem>(
                SelectedCalculation.Costs.Select(
                    cost => new sfChartItem() { Name = cost.Attribute, Value = Math.Round(Convert.ToDouble(cost.Value), 2) }
                    )); 
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
            get => new RelayCommand(p => SaveCalculationAction());
        }
        private async void SaveCalculationAction()
        {
            try
            {
                var saveFileDialog = new System.Windows.Forms.SaveFileDialog
                {
                    Filter = StaticStrings.FilterCalculationFile,
                };
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    /*
                    var res = CalculationFile.EncryptAndSerialize(saveFileDialog.FileName, this.SelectedCalculation);
                    if (res)
                    {
                        await this._dialogCoordinator.ShowMessageAsync(this,
                            Strings.DialogCalculationSaveSuccessHeadline,
                            Strings.DialogCalculationSaveSuccessContent
                            );
                        logger.Info(string.Format(Strings.EventCalculationSavedFormated, SelectedCalculation.Name, saveFileDialog.FileName));
                    }
                    */
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
