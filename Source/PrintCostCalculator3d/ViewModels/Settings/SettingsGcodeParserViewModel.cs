using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Models.Slicer;
using PrintCostCalculator3d.Utilities;
using System;

namespace PrintCostCalculator3d.ViewModels
{
    class SettingsGcodeParserViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly bool _isLoading;
        #endregion

        #region Properties
        

        private bool _PreferValuesInCommentsFromKnownSlicers;
        public bool PreferValuesInCommentsFromKnownSlicers
        {
            get => _PreferValuesInCommentsFromKnownSlicers;
            set
            {
                if (value == _PreferValuesInCommentsFromKnownSlicers)
                    return;
                if (!_isLoading)
                    SettingsManager.Current.GcodeParser_PreferValuesInCommentsFromKnownSlicers = value;

                _PreferValuesInCommentsFromKnownSlicers = value;
                OnPropertyChanged();
            }
        }

        private int _backgroundJobInterval;
        public int BackgroundJobInterval
        {
            get => _backgroundJobInterval;
            set
            {
                if (value == _backgroundJobInterval)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.General_BackgroundJobInterval = value;

                _backgroundJobInterval = value;
                OnPropertyChanged();
            }
        }

        private SlicerPrinterConfiguration _printerConfig;
        public SlicerPrinterConfiguration PrinterConfig
        {
            get => _printerConfig;
            set
            {
                if (value == _printerConfig)
                    return;
                if (!_isLoading)
                    SettingsManager.Current.GcodeParser_PrinterConfig = value;
                _printerConfig = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<SlicerPrinterConfiguration> _printerConfigs;
        public ObservableCollection<SlicerPrinterConfiguration> PrinterConfigs
        {
            get => _printerConfigs;
            set
            {
                if (value == _printerConfigs)
                    return;
                if (!_isLoading)
                    SettingsManager.Current.GcodeParser_PrinterConfigs = value;
                _printerConfigs = value;
                OnPropertyChanged();
            }
        }
        
        private string _name = "";
        public string Name
        {
            get => _name;
            set
            {
                if (value == _name)
                    return;
                _name = value;
                OnPropertyChanged();
            }
        }
        private float _aMax_xy = 1000;
        public float AMax_xy
        {
            get => _aMax_xy;
            set
            {
                if (value == _aMax_xy)
                    return;
                _aMax_xy = value;
                OnPropertyChanged();
            }
        }

        private float _aMax_z = 1000;
        public float AMax_z
        {
            get => _aMax_z;
            set
            {
                if (value == _aMax_z)
                    return;
                _aMax_z = value;
                OnPropertyChanged();
            }
        }

        private float _aMax_e = 5000;
        public float AMax_e
        {
            get => _aMax_e;
            set
            {
                if (value == _aMax_e)
                    return;
                _aMax_e = value;
                OnPropertyChanged();
            }
        }

        private float _aMax_eExtrude = 1250;
        public float AMax_eExtrude
        {
            get => _aMax_eExtrude;
            set
            {
                if (value == _aMax_eExtrude)
                    return;
                _aMax_eExtrude = value;
                OnPropertyChanged();
            }
        }

        private float _aMax_eRetract = 1250;
        public float AMax_eRetract
        {
            get => _aMax_eRetract;
            set
            {
                if (value == _aMax_eRetract)
                    return;
                _aMax_eRetract = value;
                OnPropertyChanged();
            }
        }

        private float _printDurationCorrection = 1;
        public float PrintDurationCorrection
        {
            get => _printDurationCorrection;
            set
            {
                if (value == _printDurationCorrection)
                    return;
                _printDurationCorrection = value;
                OnPropertyChanged();
            }
        }

        public List<Models.Slicer.Slicer> KnownSlicers
        {
            get => Models.Slicer.Slicer.SupportedSlicers;
        }
        #endregion   

        #region Constructor, LoadSettings
        public SettingsGcodeParserViewModel()
        {
            _isLoading = true;

            LoadSettings();


            _isLoading = false;
        }
        public SettingsGcodeParserViewModel(IDialogCoordinator instance)
        {
            _dialogCoordinator = instance;
            _isLoading = true;

            LoadSettings();

            _isLoading = false;
        }


        private void LoadSettings()
        {
            PreferValuesInCommentsFromKnownSlicers = SettingsManager.Current.GcodeParser_PreferValuesInCommentsFromKnownSlicers;
            PrinterConfigs = SettingsManager.Current.GcodeParser_PrinterConfigs;
            PrinterConfig = SettingsManager.Current.GcodeParser_PrinterConfig;
            //BackgroundJobInterval = SettingsManager.Current.General_BackgroundJobInterval;
        }
        #endregion

        #region ICommands & Actions
        public ICommand SelectedPrinterConfigChangedCommand
        {
            get { return new RelayCommand(async(p) => await SelectedPrinterConfigChangedAction(p)); }
        }

        private async Task SelectedPrinterConfigChangedAction(object parameter)
        {
            try
            {
                SlicerPrinterConfiguration config = parameter as SlicerPrinterConfiguration;
                if(config != null)
                {
                    Name = config.PrinterName;
                    PrintDurationCorrection = config.PrintDurationCorrection;
                    AMax_z = config.AMax_z;
                    AMax_xy = config.AMax_xy;
                    AMax_e = config.AMax_e;
                    AMax_eExtrude = config.AMax_eExtrude;
                    AMax_eRetract = config.AMax_eRetract;
                }
            }
            catch(Exception exc)
            {

            }
        }
        public ICommand DeleteCommand
        {
            get { return new RelayCommand(async(p) => await DeleteAction()); }
        }

        private async Task DeleteAction()
        {
            PrinterConfigs.Remove(PrinterConfig);
            if (PrinterConfigs.Count > 0)
                PrinterConfig = PrinterConfigs[0];
            else
                PrinterConfig = null;
        }
        public ICommand AddCommand
        {
            get { return new RelayCommand(async(p) => await AddAction()); }
        }
        private bool AddCommand_CanExecute()
        {
            bool execute =
                AMax_z > 0 &&
                AMax_xy > 0 &&
                AMax_e > 0 &&
                AMax_eExtrude > 0 &&
                AMax_eRetract > 0;
            return execute;
        }
        private async Task AddAction()
        {
            PrinterConfig = new SlicerPrinterConfiguration()
            {
                PrinterName = Name,
                PrintDurationCorrection = PrintDurationCorrection,
                AMax_z = AMax_z,
                AMax_xy = AMax_xy,
                AMax_e = AMax_e,
                AMax_eExtrude = AMax_eExtrude,
                AMax_eRetract = AMax_eRetract,
            };
            PrinterConfigs.Add(PrinterConfig);
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
