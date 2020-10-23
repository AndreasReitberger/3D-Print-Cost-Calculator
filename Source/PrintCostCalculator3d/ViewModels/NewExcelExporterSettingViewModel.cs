using log4net;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PrintCostCalculator3d.Models.Exporter;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;

namespace PrintCostCalculator3d.ViewModels
{
    class NewExcelExporterSettingViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        private bool _isEdit;
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

        private Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _column;
        public string Column
        {
            get => _column;
            set
            {
                if (_column != value)
                {
                    _column = value.ToUpper();
                    OnPropertyChanged();
                }
            }
        }

        private int _row;
        public int Row
        {
            get => _row;
            set
            {
                if (_row != value)
                {
                    _row = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _worksheet;
        public string Worksheet
        {
            get => _worksheet;
            set
            {
                if (_worksheet != value)
                {
                    _worksheet = value;
                    OnPropertyChanged();
                }
            }
        }

        private ExporterAttribute _property;
        public ExporterAttribute Property
        {
            get => _property;
            set
            {
                if (_property != value)
                {
                    _property = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<ExporterAttribute> _attributes = new List<ExporterAttribute>();
        public List<ExporterAttribute> Attributes
        {
            get => _attributes;
            set
            {
                if (_attributes == value) return;
                _attributes = value;
                OnPropertyChanged();
            }
        }

        private ExporterSettings _settings;
        public ExporterSettings Settings
        {
            get => _settings;
            set
            {
                if (_settings != value)
                {
                    _settings = value;
                    OnPropertyChanged();
                }
            }
        }

        private ExporterTarget _target;
        public ExporterTarget Target
        {
            get => _target;
            set
            {
                if (_target != value)
                {
                    _target = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> _excelSheets = new List<string>();
        public List<string> ExcelSheets
        {
            get => _excelSheets;
            set
            {
                if (_excelSheets != value)
                {
                    _excelSheets = value;
                    OnPropertyChanged();
                }
            }
        }


        #endregion

        #region Constructor
        public NewExcelExporterSettingViewModel(Action<NewExcelExporterSettingViewModel> saveCommand, Action<NewExcelExporterSettingViewModel> cancelHandler,
            List<string> ExcelSheets, ExporterTarget target, ExporterSettings settings = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            this.ExcelSheets = ExcelSheets;
            if (this.ExcelSheets.Count > 0)
                Worksheet = this.ExcelSheets[0];

            IsEdit = settings != null;
            try
            {
                Target = target;
                LoadSettings();
                var temp = settings ?? new ExporterSettings()
                {
                    Coordinates = new ExcelCoordinates() { Column = "A", Row = 1}
                };
                if (temp != null && IsEdit)
                    Id = temp.Id; //materialInfo.Id;
                Column = temp.Coordinates.Column;
                Row = temp.Coordinates.Row;
                Worksheet = temp.WorkSheetName;
                Property = temp.Attribute;

                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public NewExcelExporterSettingViewModel(Action<NewExcelExporterSettingViewModel> saveCommand, Action<NewExcelExporterSettingViewModel> cancelHandler, 
            IDialogCoordinator dialogCoordinator, List<string> ExcelSheets, ExporterTarget target, ExporterSettings settings = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            this._dialogCoordinator = dialogCoordinator;

            this.ExcelSheets = ExcelSheets;
            if (this.ExcelSheets.Count > 0)
                Worksheet = this.ExcelSheets[0];

            IsEdit = settings != null;
            try
            {
                Target = target;
                LoadSettings();
                var temp = settings ?? new ExporterSettings()
                {
                    Coordinates = new ExcelCoordinates() { Column = "A", Row = 1 }
                };
                if (temp != null && IsEdit)
                    Id = temp.Id; //materialInfo.Id;
                Column = temp.Coordinates.Column;
                Row = temp.Coordinates.Row;
                Worksheet = temp.WorkSheetName;
                Property = temp.Attribute;

                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
        private void LoadSettings()
        {
            Attributes = ExporterAttribute.Attributes.Where(attr => attr.Target == this.Target).ToList();
            if (!IsEdit && Attributes.Count > 0)
                Property = Attributes[0];
        }
        #endregion


        #region iCommands & Actions
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
