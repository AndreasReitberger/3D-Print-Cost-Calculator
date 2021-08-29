using AndreasReitberger.Models;
using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels
{
    class SettingsGcodeParserViewModel : ViewModelBase
    {
        #region Variables
        //readonly IDialogCoordinator _dialogCoordinator;
        //readonly bool _isLoading;
        #endregion

        #region Properties
        bool _PreferValuesInCommentsFromKnownSlicers;
        public bool PreferValuesInCommentsFromKnownSlicers
        {
            get => _PreferValuesInCommentsFromKnownSlicers;
            set
            {
                if (value == _PreferValuesInCommentsFromKnownSlicers)
                    return;
                if (!IsLoading)
                    SettingsManager.Current.GcodeParser_PreferValuesInCommentsFromKnownSlicers = value;

                _PreferValuesInCommentsFromKnownSlicers = value;
                OnPropertyChanged();
            }
        }

        int _backgroundJobInterval;
        public int BackgroundJobInterval
        {
            get => _backgroundJobInterval;
            set
            {
                if (value == _backgroundJobInterval)
                    return;

                if (!IsLoading)
                    SettingsManager.Current.General_BackgroundJobInterval = value;

                _backgroundJobInterval = value;
                OnPropertyChanged();
            }
        }

        SlicerPrinterConfiguration _printerConfig;
        public SlicerPrinterConfiguration PrinterConfig
        {
            get => _printerConfig;
            set
            {
                if (value == _printerConfig)
                    return;
                if (!IsLoading)
                    SettingsManager.Current.GcodeParser_PrinterConfig = value;
                _printerConfig = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<SlicerPrinterConfiguration> _printerConfigs = new();
        public ObservableCollection<SlicerPrinterConfiguration> PrinterConfigs
        {
            get => _printerConfigs;
            set
            {
                if (value == _printerConfigs)
                {
                    return;
                }
                if (!IsLoading)
                {
                    SettingsManager.Current.GcodeParser_PrinterConfigs = value;
                }

                _printerConfigs = value;
                OnPropertyChanged();
            }
        }
        
        string _name = "";
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
        double _aMax_xy = 1000;
        public double AMax_xy
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

        double _aMax_z = 1000;
        public double AMax_z
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

        double _aMax_e = 5000;
        public double AMax_e
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

        double _aMax_eExtrude = 1250;
        public double AMax_eExtrude
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

        double _aMax_eRetract = 1250;
        public double AMax_eRetract
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

        double _printDurationCorrection = 1;
        public double PrintDurationCorrection
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
            IsLoading = true;
            LoadSettings();
            IsLoading = false;
        }
        public SettingsGcodeParserViewModel(IDialogCoordinator instance)
        {
            _ = instance;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;
        }


        void LoadSettings()
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
            get { return new RelayCommand((p) => SelectedPrinterConfigChangedAction(p)); }
        }

        void SelectedPrinterConfigChangedAction(object parameter)
        {
            try
            {
                if (parameter is SlicerPrinterConfiguration config)
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
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        public ICommand DeleteCommand
        {
            get { return new RelayCommand((p) => DeleteAction()); }
        }

        void DeleteAction()
        {
            _ = PrinterConfigs.Remove(PrinterConfig);
            PrinterConfig = PrinterConfigs.Count > 0 ? PrinterConfigs[0] : null;
        }
        public ICommand AddCommand
        {
            get { return new RelayCommand((p) => AddAction(), p => AddCommand_CanExecute()); }
        }
        bool AddCommand_CanExecute()
        {
            bool execute =
                AMax_z > 0 &&
                AMax_xy > 0 &&
                AMax_e > 0 &&
                AMax_eExtrude > 0 &&
                AMax_eRetract > 0;
            return execute;
        }
        void AddAction()
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
