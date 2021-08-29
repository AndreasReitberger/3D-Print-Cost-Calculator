using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Utilities;
using AndreasReitberger.Models;

namespace PrintCostCalculator3d.ViewModels
{
    class SettingsCalculationsViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        //readonly bool _isLoading;
        #endregion

        #region Properties
        ObservableCollection<Printer3d> _printers = new ObservableCollection<Printer3d>();
        public ObservableCollection<Printer3d> Printers
        {
            get => _printers;
            set
            {
                if (value == _printers)
                    return;

                _printers = value;
                OnPropertyChanged();
            }
        }

        Printer3d _SelectedPrinter;
        public Printer3d SelectedPrinter
        {
            get => _SelectedPrinter;
            set
            {
                if (value == _SelectedPrinter)
                    return;

                _SelectedPrinter = value;
                OnPropertyChanged();
            }
        }

        IList _SelectedPrinters = new ArrayList();
        public IList SelectedPrinters
        {
            get => _SelectedPrinters;
            set
            {
                //if (_SelectedPrinters.Equals(value))
                //   return;

                if (!IsLoading)
                {
                    ObservableCollection<Printer3d> _prints =
                        new ObservableCollection<Printer3d>(value.Cast<Printer3d>().ToList());
                    SettingsManager.Current.Calculation_DefaultPrintersLib = _prints;
                    SettingsManager.Save();
                }

                _SelectedPrinters = value;
                OnPropertyChanged();
            }
        }
        
        ObservableCollection<Material3d> _materials = new ObservableCollection<Material3d>();
        public ObservableCollection<Material3d> Materials
        {
            get => _materials;
            set
            {
                if (value == _materials)
                    return;

                _materials = value;
                OnPropertyChanged();
            }
        }

        Material3d _SelectedMaterial;
        public Material3d SelectedMaterial
        {
            get => _SelectedMaterial;
            set
            {
                if (value == _SelectedMaterial)
                    return;

                _SelectedMaterial = value;
                OnPropertyChanged();
            }
        }

        IList _SelectedMaterials = new ArrayList();
        public IList SelectedMaterials
        {
            get => _SelectedMaterials;
            set
            {
                //if (_SelectedMaterials.Equals(value))
                //    return;

                if (!IsLoading)
                {
                    ObservableCollection<Material3d> _mats = 
                        new ObservableCollection<Material3d>(value.Cast<Material3d>().ToList());
                    SettingsManager.Current.Calculation_DefaultMaterialsLib = _mats;
                    SettingsManager.Save();
                }

                _SelectedMaterials = value;
                OnPropertyChanged();
            }
        }     
        
        ObservableCollection<Workstep> _worksteps = new ObservableCollection<Workstep>();
        public ObservableCollection<Workstep> Worksteps
        {
            get => _worksteps;
            set
            {
                if (value == _worksteps)
                    return;

                _worksteps = value;
                OnPropertyChanged();
            }
        }

        Workstep _SelectedWorkstep;
        public Workstep SelectedWorkstep
        {
            get => _SelectedWorkstep;
            set
            {
                if (value == _SelectedWorkstep)
                    return;

                _SelectedWorkstep = value;
                OnPropertyChanged();
            }
        }

        IList _SelectedWorksteps = new ArrayList();
        public IList SelectedWorksteps
        {
            get => _SelectedWorksteps;
            set
            {
                //if (_SelectedWorksteps.Equals(value))
                //    return;

                if (!IsLoading)
                {
                    ObservableCollection<Workstep> _ws = 
                        new ObservableCollection<Workstep>(value.Cast<Workstep>().ToList());
                    SettingsManager.Current.Calculation_DefaultWorkstepsLib = _ws;
                    SettingsManager.Save();
                }

                _SelectedWorksteps = value;
                OnPropertyChanged();
            }
        }     

        bool _openCalculationResultView;
        public bool OpenCalculationResultView
        {
            get => _openCalculationResultView;
            set
            {
                if (value == _openCalculationResultView)
                    return;

                if (!IsLoading)
                    SettingsManager.Current.General_OpenCalculationResultView = value;

                _openCalculationResultView = value;
                OnPropertyChanged();
            }
        }
        bool _useVolumeForCalculation = true;
        public bool UseVolumeForCalculation
        {
            get => _useVolumeForCalculation;
            set
            {
                if (value == _useVolumeForCalculation)
                    return;

                if (!IsLoading)
                    SettingsManager.Current.Calculation_UseVolumeForCalculation = value;

                _useVolumeForCalculation = value;
                OnPropertyChanged();
            }
        }

        bool _restartRequired;
        public bool RestartRequired
        {
            get => _restartRequired;
            set
            {
                if (value == _restartRequired)
                    return;

                _restartRequired = value;
                OnPropertyChanged();
            }
        }
        #endregion   

        #region Constructor, LoadSettings
        public SettingsCalculationsViewModel()
        {
            IsLoading = true;
            LoadSettings();
            IsLoading = false;
        }
        public SettingsCalculationsViewModel(IDialogCoordinator instance)
        {
            _dialogCoordinator = instance;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;
        }

        void LoadSettings()
        {
            Printers = SettingsManager.Current.Printers;
            SelectedPrinters = SettingsManager.Current.Calculation_DefaultPrintersLib;
            //SelectedPrinters.CollectionChanged += SelectedPrinters_CollectionChanged;

            Materials = SettingsManager.Current.PrinterMaterials;
            SelectedMaterials = SettingsManager.Current.Calculation_DefaultMaterialsLib;

            Worksteps = SettingsManager.Current.Worksteps;
            SelectedWorksteps = SettingsManager.Current.Calculation_DefaultWorkstepsLib;
            //SelectedMaterials.CollectionChanged += SelectedMaterials_CollectionChanged;       

            OpenCalculationResultView = SettingsManager.Current.General_OpenCalculationResultView;
            UseVolumeForCalculation = SettingsManager.Current.Calculation_UseVolumeForCalculation;
        }

        void SelectedMaterials_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SelectedMaterials));
            SettingsManager.Save();
        }

        void SelectedPrinters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SelectedPrinters));
            SettingsManager.Save();
        }
        #endregion

        #region ICommands & Actions
        public ICommand VisibleToHideApplicationCommand
        {
            get { return new RelayCommand(p => VisibleToHideApplicationAction()); }
        }

        void VisibleToHideApplicationAction()
        {
           
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
