using AndreasReitberger.Enums;
using AndreasReitberger.Models;
using AndreasReitberger.Utilities;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    public class CalculateVolumeDialogViewModel : ViewModelBase
    {
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
                if (_id == value) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        Material3d _material;
        public Material3d Material
        {
            get => _material;
            set
            {
                if (_material == value) return;
                _material = value;
                calculateVolume();
                OnPropertyChanged();
            }
        }

        Unit _unit = Unit.g;
        public Unit Unit
        {
            get => _unit;
            set
            {
                if (_unit == value) return;
                _unit = value;
                calculateVolume();
                OnPropertyChanged();
            }
        }

        double _weight = 0;
        public double Weight
        {
            get => _weight;
            set
            {
                if (_weight == value) return;
                _weight = value;
                calculateVolume();
                OnPropertyChanged();
            }
        }

        double _volume = 0;
        public double Volume
        {
            get => _volume;
            set
            {
                if (_volume == value) return;
                _volume = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Materials
        ObservableCollection<Material3d> _materials = new ObservableCollection<Material3d>();
        public ObservableCollection<Material3d> Materials 
        {
            get => _materials;
            set
            {
                if (_materials == value) return;
                _materials = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region EnumCollections

        #region Units
        ObservableCollection<Unit> _units = new ObservableCollection<Unit>(
            Enum.GetValues(typeof(Unit)).Cast<Unit>().ToList()
            );
        public ObservableCollection<Unit> Units
        {
            get => _units;
            set
            {
                if (_units == value) return;

                _units = value;
                OnPropertyChanged();

            }
        }

        #endregion

        #endregion

        #region Constructor, LoadSettings
        public CalculateVolumeDialogViewModel(Action<CalculateVolumeDialogViewModel> saveCommand, Action<CalculateVolumeDialogViewModel> cancelHandler)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            IsLicenseValid = false;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;
        }

        void LoadSettings()
        {
            Materials = SettingsManager.Current.PrinterMaterials;
            if (Materials.Count > 0)
                Material = Materials[0];
        }
        #endregion

        #region ICommands & Actions
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }

        public ICommand SelectedMaterialChangedCommand
        {
            get => new RelayCommand((p) => SelectedMaterialChangedAction(p));
        }
        void SelectedMaterialChangedAction(object material)
        {
            try
            {

            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        #endregion

        #region Methods
        void calculateVolume()
        {
            if (Material == null)
                Volume = 0;
            else
            {
                Volume = Weight / Material.Density * UnitFactor.GetUnitFactor(Unit);
            }
        }
        #endregion
    }
}
