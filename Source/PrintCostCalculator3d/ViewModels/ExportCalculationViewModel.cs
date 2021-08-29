using AndreasReitberger.Models;
using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Models.Exporter;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels
{

    class ExportCalculationViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Properties
        ObservableCollection<Calculation3d> _calculations = new ObservableCollection<Calculation3d>();
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

        ExporterTarget _target;
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

        ObservableCollection<ExporterTemplate> _templates = new ObservableCollection<ExporterTemplate>();
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

        ExporterTemplate _selectedTemplate;
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
        
        IList _selectedTemplates = new ArrayList();
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

        string _exportPath;
        public string ExportPath
        {
            get => _exportPath;
            set
            {
                if (_exportPath == value)
                    return;
                if (!IsLoading)
                    SettingsManager.Current.ExporterExcel_LastExportPath = value;
                _exportPath = value;
                OnPropertyChanged();
                
            }
        }

        bool _exportAsPdf = false;
        public bool ExportAsPdf
        {
            get => _exportAsPdf;
            set
            {
                if (_exportAsPdf == value)
                    return;
                if (!IsLoading)
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
            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            IsLicenseValid = false;

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
            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            IsLicenseValid = false;

            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            this.Target = Target;
            this.Calculations = Calculations;
            // Filter templates
            Templates = SettingsManager.Current.ExporterExcel_Templates;
            Templates = new ObservableCollection<ExporterTemplate>(Templates.Where(template => template.ExporterTarget == Target).ToList());
            _dialogCoordinator = dialogCoordinator;
        }

        void LoadSettings()
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
        void BrowseFileAction()
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

        void BrowseFolderAction()
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
