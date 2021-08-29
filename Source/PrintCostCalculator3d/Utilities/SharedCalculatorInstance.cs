using AndreasReitberger;
using AndreasReitberger.Enums;
using AndreasReitberger.Models;
using AndreasReitberger.Models.CalculationAdditions;
using AndreasReitberger.Utilities;
using log4net;
using PrintCostCalculator3d.Models;
using PrintCostCalculator3d.Models._3dprinting;
using PrintCostCalculator3d.Models.Events;
using PrintCostCalculator3d.Models.Exporter;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace PrintCostCalculator3d.Utilities
{
    public class SharedCalculatorInstance : BaseModel
    {
        #region Logger
        static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region SingleInstance
        static SharedCalculatorInstance _instance;
        public static SharedCalculatorInstance Instance
        {
            get {
                if (_instance == null)
                    _instance = new SharedCalculatorInstance();
                return _instance;
            }
        }
        #endregion

        #region Properties

        #region Preferences
        bool _setFirstGcodeAsSelected = true;
        public bool SetFirstGcodeAsSelected
        {
            get => _setFirstGcodeAsSelected;
            set
            {
                if (_setFirstGcodeAsSelected == value) return;
                _setFirstGcodeAsSelected = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Backup
        bool _autoBackupOnChange = true;
        public bool AutoBackupOnChange
        {
            get => _autoBackupOnChange;
            set
            {
                if (_autoBackupOnChange == value) return;
                _autoBackupOnChange = value;
                OnPropertyChanged();
            }
        }

        string _backupPath = string.Empty;
        public string BackupPath
        {
            get => _backupPath;
            set
            {
                if (_backupPath == value) return;
                _backupPath = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Calculations
        Calculation3d _calculation;
        public Calculation3d Calculation
        {
            get => _calculation;
            set
            {
                if (_calculation == value) return;
                // Notify subscribers
                SelectedCalculationChanged(new CalculationChangedEventArgs()
                {
                    NewCalculation = value,
                    PreviousCalculation = _calculation,
                });
                _calculation = value;
                OnPropertyChanged();
            }
        }


        ObservableCollection<Calculation3d> _calculations = new();
        public ObservableCollection<Calculation3d> Calculations
        {
            get => _calculations;
            set
            {
                if (_calculations == value) return;
                _calculations = value;
                if(Calculations != null)
                {
                    Calculations.CollectionChanged -= Calculations_CollectionChanged;
                    Calculations.CollectionChanged += Calculations_CollectionChanged;
                }
                OnPropertyChanged();

            }
        }
        #endregion

        #region Gcodes
        Gcode _gcode;
        public Gcode Gcode
        {
            get => _gcode;
            set
            {
                if (_gcode == value) return;
                // Notify subscribers
                SelectedGcodeChanged(new GcodeChangedEventArgs()
                {
                    NewGcode = value,
                    PreviousGcode = _gcode,
                });
                _gcode = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<Gcode> _gcodes = new();
        public ObservableCollection<Gcode> Gcodes
        {
            get => _gcodes;
            set
            {
                if (_gcodes == value) return;
                _gcodes = value;
                if (Gcodes != null)
                {
                    Gcodes.CollectionChanged -= Gcodes_CollectionChanged;
                    Gcodes.CollectionChanged += Gcodes_CollectionChanged;
                }
                OnPropertyChanged();

            }
        }
        #endregion

        #region Stl
        Stl _stlFile;
        public Stl StlFile
        {
            get => _stlFile;
            set
            {
                if (_stlFile == value) return;
                // Notify subscribers
                SelectedStlChanged(new StlChangedEventArgs()
                {
                    NewStl = value,
                    PreviousStl = _stlFile,
                });
                _stlFile = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<Stl> _stlFiles = new();
        public ObservableCollection<Stl> StlFiles
        {
            get => _stlFiles;
            set
            {
                if (_stlFiles == value) return;
                _stlFiles = value;
                if (StlFiles != null)
                {
                    StlFiles.CollectionChanged -= StlFiles_CollectionChanged;
                    StlFiles.CollectionChanged += StlFiles_CollectionChanged;
                }
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion

        #region EventHandlers
        public event EventHandler<CalculationsChangedEventArgs> OnCalculationsChanged;
        protected virtual void CalculationsChanged(CalculationsChangedEventArgs e)
        {
            OnCalculationsChanged?.Invoke(this, e);
        }

        public event EventHandler<CalculationChangedEventArgs> OnSelectedCalculationChanged;
        protected virtual void SelectedCalculationChanged(CalculationChangedEventArgs e)
        {
            OnSelectedCalculationChanged?.Invoke(this, e);
        }

        public event EventHandler<GcodesChangedEventArgs> OnGcodesChanged;
        protected virtual void GcodesChanged(GcodesChangedEventArgs e)
        {
            OnGcodesChanged?.Invoke(this, e);
        }

        public event EventHandler<GcodeChangedEventArgs> OnSelectedGcodeChanged;
        protected virtual void SelectedGcodeChanged(GcodeChangedEventArgs e)
        {
            OnSelectedGcodeChanged?.Invoke(this, e);
        }

        public event EventHandler<StlsChangedEventArgs> OnStlsChanged;
        protected virtual void StlsChanged(StlsChangedEventArgs e)
        {
            OnStlsChanged?.Invoke(this, e);
        }

        public event EventHandler<StlChangedEventArgs> OnSelectedStlChanged;
        protected virtual void SelectedStlChanged(StlChangedEventArgs e)
        {
            OnSelectedStlChanged?.Invoke(this, e);
        }
        #endregion

        #region Constructor
        public SharedCalculatorInstance() {

            Calculations.CollectionChanged += Calculations_CollectionChanged;
            Gcodes.CollectionChanged += Gcodes_CollectionChanged;
            StlFiles.CollectionChanged += StlFiles_CollectionChanged;
            //_instance = this;
        }

        #endregion

        #region Events
        void Calculations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                CalculationsChanged(new CalculationsChangedEventArgs()
                {
                    Calculations = Calculations,
                    NewItems = e.NewItems != null ? e.NewItems.Cast<Calculation3d>().ToList() : new List<Calculation3d>(),
                    OldItems = e.OldItems != null ? e.OldItems.Cast<Calculation3d>().ToList() : new List<Calculation3d>(),
                    Action = e.Action,
                });
                if(AutoBackupOnChange)
                {
                    if (!string.IsNullOrEmpty(BackupPath))
                        SaveCurrentSessionCalculations(BackupPath);
                }
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        void Gcodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                GcodesChanged(new GcodesChangedEventArgs()
                {
                    //Gcodes = this.Gcodes,
                    NewItems = e.NewItems != null ? e.NewItems.Cast<Gcode>().ToList() : new List<Gcode>(),
                    OldItems = e.OldItems != null ? e.OldItems.Cast<Gcode>().ToList() : new List<Gcode>(),
                    Action = e.Action,
                });
                // Set first item in collection to selected
                if(SetFirstGcodeAsSelected && Gcode == null && e.Action == NotifyCollectionChangedAction.Add)
                {
                    if (e.NewItems != null)
                    {
                        var items = e.NewItems.Cast<Gcode>().ToList();
                        if (items != null && items.Count > 0)
                            Gcode = items[0];
                     }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        void StlFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                StlsChanged(new StlsChangedEventArgs()
                {
                    Stls = StlFiles,
                    NewItems = e.NewItems != null ? e.NewItems.Cast<Stl>().ToList() : new List<Stl>(),
                    OldItems = e.OldItems != null ? e.OldItems.Cast<Stl>().ToList() : new List<Stl>(),
                    Action = e.Action,
                });
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        #endregion

        #region Methods

        #region Public
        public void LoadLastSessionCalculations(string filePath)
        {
            try
            {
                //string filePath = Path.Combine(SettingsManager.GetSettingsLocation(), "calculations.xml");
                if (File.Exists(filePath))
                {
                    var calculations = CalculationFile.DecryptAndDeserializeArray(filePath);
                    // If it's already the new format, this will be null (Remove with next update)
                    if (calculations != null)
                    {
                        foreach (var calc in calculations)
                            Calculations.Add(calc);
                    }
                    else
                    {
                        var calculationsLib = Calculator3dExporter.DecryptAndDeserializeArray(filePath);
                        if (calculationsLib != null)
                        {
                            foreach (var calc in calculationsLib)
                                Calculations.Add(calc);
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        public void SaveCurrentSessionCalculations(string filePath)
        {
            try
            {
                if (Calculations.Count > 0)
                    Calculator3dExporter.EncryptAndSerialize(filePath, Calculations.ToArray());
                else
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        public void RemoveCalculation(Calculation3d calculation)
        {
            try
            {
                var item = Calculations.FirstOrDefault(calc => calc.Id == calculation.Id);
                if (item != null)
                {
                    Calculations.Remove(item);
                    // Clear selection if the current selected item is deleted
                    if (item == Calculation)
                    {
                        Calculation = Calculations.Count > 0 ? Calculations[0] : null;
                    }
                }
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        public void AddCalculation(Calculation3d calculation)
        {
            try
            {
                var item = Calculations.FirstOrDefault(calc => calc.Id == calculation.Id);
                if (item == null)
                    Calculations.Add(calculation);
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public void RemoveGcode(Gcode gcode)
        {
            try
            {
                var item = Gcodes.FirstOrDefault(gc => gc.Id == gcode.Id);
                if (item != null)
                {
                    Gcodes.Remove(item);
                    // Clear selection if the current selected item is deleted
                    if (item == Gcode)
                    {
                        Gcode = Gcodes.Count > 0 ? Gcodes[0] : null;
                    }
                }
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        public void AddGcode(Gcode gcode)
        {
            try
            {
                var item = Gcodes.FirstOrDefault(gc => gc.Id == gcode.Id);
                if (item == null)
                    Gcodes.Add(gcode);
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public void RemoveStl(Stl stl)
        {
            try
            {
                var item = StlFiles.FirstOrDefault(f => f.Id == stl.Id);
                if (item != null)
                {
                    StlFiles.Remove(item);
                    // Clear selection if the current selected item is deleted
                    if (item == StlFile)
                    {
                        StlFile = StlFiles.Count > 0 ? StlFiles[0] : null;
                    }
                }
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        public void AddStl(Stl stl)
        {
            try
            {
                var item = StlFiles.FirstOrDefault(f => f.Id == stl.Id);
                if (item == null)
                    StlFiles.Add(stl);
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        #endregion

        #endregion
    }
}
