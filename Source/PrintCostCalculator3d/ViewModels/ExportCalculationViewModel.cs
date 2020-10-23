using log4net;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PrintCostCalculator3d.Models.Exporter;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Utilities;
using AndreasReitberger.Models;

namespace PrintCostCalculator3d.ViewModels
{

    class ExportCalculationViewModel : ViewModelBase
    {
        #region Variables
        private readonly bool _isLoading;
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Properties
        private ObservableCollection<Calculation3d> _calculations = new ObservableCollection<Calculation3d>();
        public ObservableCollection<Calculation3d> Calculations
        {
            get => _calculations;
            set
            {
                if (_calculations == value)
                    return;
                _calculations = value;
                OnPropertyChanged();
            }
        }

        private ExporterTarget _target;
        public ExporterTarget Target
        {
            get => _target;
            set
            {
                if (_target == value)
                    return;
                _target = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ExporterTemplate> _templates = new ObservableCollection<ExporterTemplate>();
        public ObservableCollection<ExporterTemplate> Templates
        {
            get => _templates;
            set
            {
                if (_templates == value)
                    return;
                _templates = value;
                OnPropertyChanged();
            }
        }

        private ExporterTemplate _selectedTemplate;
        public ExporterTemplate SelectedTemplate
        {
            get => _selectedTemplate;
            set
            {
                if (_selectedTemplate == value)
                    return;
                _selectedTemplate = value;
                OnPropertyChanged();
            }
        }
        
        private IList _selectedTemplates = new ArrayList();
        public IList SelectedTemplates
        {
            get => _selectedTemplates;
            set
            {
                if (_selectedTemplates == value)
                    return;
                _selectedTemplates = value;
                OnPropertyChanged();
            }
        }

        private string _exportPath;
        public string ExportPath
        {
            get => _exportPath;
            set
            {
                if (_exportPath == value)
                    return;
                if (!_isLoading)
                    SettingsManager.Current.ExporterExcel_LastExportPath = value;
                _exportPath = value;
                OnPropertyChanged();
                
            }
        }

        private bool _exportAsPdf = false;
        public bool ExportAsPdf
        {
            get => _exportAsPdf;
            set
            {
                if (_exportAsPdf == value)
                    return;
                if (!_isLoading)
                    SettingsManager.Current.ExporterExcel_LastExportAsPdf = value;
                _exportAsPdf = value;
                OnPropertyChanged();
                
            }
        }

        #endregion

        #region Constructor
        public ExportCalculationViewModel(Action<ExportCalculationViewModel> saveCommand, Action<ExportCalculationViewModel> cancelHandler, 
            ObservableCollection<Calculation3d> Calculations, ExporterTarget Target)
        {
            _isLoading = true;
            LoadSettings();
            _isLoading = false;

            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            this.Calculations = Calculations;
            this.Target = Target;
            // Filter templates
            Templates = SettingsManager.Current.ExporterExcel_Templates;
            Templates = new ObservableCollection<ExporterTemplate>(Templates.Where(template => template.ExporterTarget == Target).ToList());
            

        }
        public ExportCalculationViewModel(Action<ExportCalculationViewModel> saveCommand, Action<ExportCalculationViewModel> cancelHandler, 
            ObservableCollection<Calculation3d> Calculations, ExporterTarget Target, IDialogCoordinator dialogCoordinator)
        {
            _isLoading = true;
            LoadSettings();
            _isLoading = false;

            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            this.Target = Target;
            this.Calculations = Calculations;
            // Filter templates
            Templates = SettingsManager.Current.ExporterExcel_Templates;
            Templates = new ObservableCollection<ExporterTemplate>(Templates.Where(template => template.ExporterTarget == Target).ToList());
            this._dialogCoordinator = dialogCoordinator;
        }

        private void LoadSettings()
        {
            ExportPath = SettingsManager.Current.ExporterExcel_LastExportPath;
            ExportAsPdf = SettingsManager.Current.ExporterExcel_LastExportAsPdf;
        }
        #endregion

        #region iCommands & Actions
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }

        public ICommand BrowseFileCommand
        {
            get { return new RelayCommand(p => BrowseFileAction()); }
        }
        private void BrowseFileAction()
        {
            var dialog = new System.Windows.Forms.SaveFileDialog();

            if (File.Exists(ExportPath))
                dialog.FileName = ExportPath;
            dialog.Filter = "*.xlsx|*.xlsx";
            var dialogResult = dialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                ExportPath = dialog.FileName;
            }

        }
        public ICommand BrowseFolderCommand
        {
            get { return new RelayCommand(p => BrowseFolderAction()); }
        }

        private void BrowseFolderAction()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (Directory.Exists(ExportPath))
                dialog.SelectedPath = ExportPath;

            var dialogResult = dialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
                ExportPath = dialog.SelectedPath;
        }
        #endregion
    }
}
