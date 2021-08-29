using AndreasReitberger.Models;
using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    public class SelectGcodesDialogViewModel : ViewModelBase
    {
        #region Variables
        //readonly IDialogCoordinator _dialogCoordinator;
        readonly SharedCalculatorInstance SharedCalculatorInstance = SharedCalculatorInstance.Instance;
        #endregion

        #region Properties

        #region Gcode

        Gcode _gcode;
        public Gcode Gcode
        {
            get => _gcode;
            set
            {
                if (_gcode == value) return;
                _gcode = value;
                if (_gcode != SharedCalculatorInstance.Gcode)
                    SharedCalculatorInstance.Gcode = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<Gcode> _gcodes = new();
        public ObservableCollection<Gcode> Gcodes
        {
            get => _gcodes;
            set
            {
                if (_gcodes == value)
                {
                    return;
                }

                _gcodes = value;
                SharedCalculatorInstance.Gcodes = _gcodes;
                OnPropertyChanged();
            }
        }

        IList _selectedGcodeFiles = new ArrayList();
        public IList SelectedGcodeFiles
        {
            get => _selectedGcodeFiles;
            set
            {
                if (Equals(value, _selectedGcodeFiles))
                {
                    return;
                }
                _selectedGcodeFiles = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion

        #region Constructor, LoadSettings
        public SelectGcodesDialogViewModel(Action<SelectGcodesDialogViewModel> saveCommand, Action<SelectGcodesDialogViewModel> cancelHandler)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            IsLicenseValid = false;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            SharedCalculatorInstance.OnGcodesChanged += SharedCalculatorInstance_OnGcodesChanged;
            //SharedCalculatorInstance.OnSelectedGcodeChanged += SharedCalculatorInstance_OnSelectedGcodeChanged;

            Gcodes = SharedCalculatorInstance.Gcodes;
            Gcodes.CollectionChanged += Gcodes_CollectionChanged;
        }
        public SelectGcodesDialogViewModel(Action<SelectGcodesDialogViewModel> saveCommand, Action<SelectGcodesDialogViewModel> cancelHandler, IDialogCoordinator dialogCoordinator)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            _ = dialogCoordinator;

            IsLicenseValid = false;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            SharedCalculatorInstance.OnGcodesChanged += SharedCalculatorInstance_OnGcodesChanged;

            Gcodes = SharedCalculatorInstance.Gcodes;
            Gcodes.CollectionChanged += Gcodes_CollectionChanged;
        }

        void LoadSettings()
        {
            
        }
        #endregion

        #region Events
        void Gcodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Gcodes));
            try
            {
                if (e != null)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach (var gc in e.NewItems)
                            {
                                Gcode newItem = (Gcode)gc;
                                if (newItem == null)
                                    continue;
                                SharedCalculatorInstance.AddGcode(newItem);
                            }

                            break;
                        case NotifyCollectionChangedAction.Remove:
                            foreach (var gc in e.OldItems)
                            {
                                Gcode newItem = (Gcode)gc;
                                if (newItem == null)
                                    continue;
                                SharedCalculatorInstance.RemoveGcode(newItem);
                            }
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            break;
                        case NotifyCollectionChangedAction.Move:
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            SharedCalculatorInstance.Gcodes.Clear();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        #region SharedInstance
        void SharedCalculatorInstance_OnSelectedGcodeChanged(object sender, Models.Events.GcodeChangedEventArgs e)
        {
            if (e != null)
            {
                Gcode = e.NewGcode;
            }
        }

        void SharedCalculatorInstance_OnGcodesChanged(object sender, Models.Events.GcodesChangedEventArgs e)
        {
            if (e != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewItems != null)
                        {
                            foreach (Gcode gc in e.NewItems)
                            {
                                // Check if item has not been added already
                                var item = Gcodes.FirstOrDefault(c => c.Id == gc.Id);
                                if (item != null) continue;
                                Gcodes.Add(gc);
                            }

                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        if (e.OldItems != null)
                        {
                            foreach (Gcode gc in e.OldItems)
                            {
                                var item = Gcodes.FirstOrDefault(c => c.Id == gc.Id);
                                if (item == null) continue;
                                Gcodes.Remove(item);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        Gcodes.Clear();
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #endregion

        #region iCommands & Actions
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
