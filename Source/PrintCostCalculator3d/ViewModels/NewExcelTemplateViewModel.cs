using log4net;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using PrintCostCalculator3d.Models.Exporter;
using PrintCostCalculator3d.Models.Syncfusion;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;

namespace PrintCostCalculator3d.ViewModels
{
    class NewExcelTemplateViewModel : ViewModelBase
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

        private string _Name;
        public string Name
        {
            get => _Name;
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _templatePath;
        public string TemplatePath
        {
            get => _templatePath;
            set
            {
                if (_templatePath != value)
                {
                    _templatePath = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _row;
        public string Row
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


        private ExporterSettings _selectedSetting;
        public ExporterSettings SelectedSetting
        {
            get => _selectedSetting;
            set
            {
                if (_selectedSetting != value)
                {
                    _selectedSetting = value;
                    OnPropertyChanged();
                }
            }
        }
        private IList _selectedSettings = new ArrayList();
        public IList SelectedSettings
        {
            get => _selectedSettings;
            set
            {
                if (value == _selectedSettings)
                    return;

                _selectedSettings = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ExporterSettings> _settings = new ObservableCollection<ExporterSettings>();
        public ObservableCollection<ExporterSettings> Settings
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

        private ExporterTemplate _template;
        public ExporterTemplate Template
        {
            get => _template;
            set
            {
                if (_template != value)
                {
                    _template = value;
                    OnPropertyChanged();
                }
            }
        }


        #endregion

        #region Constructor
        public NewExcelTemplateViewModel(Action<NewExcelTemplateViewModel> saveCommand, Action<NewExcelTemplateViewModel> cancelHandler, 
            ExporterTemplate template = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            IsEdit = template != null;
            try
            {
                var temp = template ?? new ExporterTemplate();
                if (temp != null & IsEdit)
                    Id = temp.Id; //materialInfo.Id;
                Name = temp.Name;
                TemplatePath = temp.TemplatePath;
                Settings = temp.Settings;
                Target = temp.ExporterTarget;

                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public NewExcelTemplateViewModel(Action<NewExcelTemplateViewModel> saveCommand, Action<NewExcelTemplateViewModel> cancelHandler, 
            IDialogCoordinator dialogCoordinator, ExporterTemplate template = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            this._dialogCoordinator = dialogCoordinator;

            IsEdit = template != null;
            try
            {
                var temp = template ?? new ExporterTemplate();
                if (temp != null && IsEdit)
                    Id = temp.Id; //materialInfo.Id;
                Name = temp.Name;
                TemplatePath = temp.TemplatePath;
                Settings = temp.Settings;
                Target = temp.ExporterTarget;

                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        #endregion

        #region iCommands & Actions
        public ICommand NewExporterSettingCommand
        {
            get => new RelayCommand(p => NewExporterSettingAction());
        }
        private async void NewExporterSettingAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.NewExcelExporterSettings };
                var newExporterSettingsViewModel = new NewExcelExporterSettingViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Settings.Add(new ExporterSettings()
                    {
                        Id = instance.Id,
                        WorkSheetName = instance.Worksheet,
                        Attribute = instance.Property,
                        Coordinates = new ExcelCoordinates() { Column = instance.Column, Row = Convert.ToInt32(instance.Row) },

                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Settings[Settings.Count - 1].ToString()));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                }
                , ExcelSheets
                , Target
                );

                _dialog.Content = new Views.NewExcelTemplateCoordinateDialogView()
                {
                    DataContext = newExporterSettingsViewModel
                };
                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand EditExporterSettingCommand
        {
            get => new RelayCommand(p => EditExporterSettingAction());
        }
        private async void EditExporterSettingAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.NewExcelExporterSettings };
                var newExporterSettingsViewModel = new NewExcelExporterSettingViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Settings.Remove(SelectedSetting);
                    Settings.Add(new ExporterSettings()
                    {
                        Id = instance.Id,
                        WorkSheetName = instance.Worksheet,
                        Attribute = instance.Property,
                        Coordinates = new ExcelCoordinates() { Column = instance.Column, Row = Convert.ToInt32(instance.Row) },

                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Settings[Settings.Count - 1].ToString()));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                }
                , ExcelSheets
                , Target
                , SelectedSetting
                );

                _dialog.Content = new Views.NewExcelTemplateCoordinateDialogView()
                {
                    DataContext = newExporterSettingsViewModel
                };
                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                await _dialogCoordinator.ShowMessageAsync(this,
                    Strings.DialogExceptionHeadline,
                    string.Format(Strings.DialogExceptionFormatedContent, exc.Message)
                    );
            }
        }

        public ICommand DeleteExporterSettingsCommand
        {
            get { return new RelayCommand(p => DeleteExporterSettingsAction()); }
        }
        private async void DeleteExporterSettingsAction()
        {
            try
            {
                var res = await _dialogCoordinator.ShowMessageAsync(this,
                    Strings.DialogDeleteHeadline,
                    Strings.DialogDeleteContent,
                    MessageDialogStyle.AffirmativeAndNegative
                    );
                if (res == MessageDialogResult.Affirmative)
                {
                    try
                    {
                        IList collection = new ArrayList(SelectedSettings);
                        for (int i = 0; i < collection.Count; i++)
                        {
                            var setting = collection[i] as ExporterSettings;
                            if (setting == null)
                                continue;
                            logger.Info(string.Format(Strings.EventDeletedItemFormated, setting.ToString()));
                            Settings.Remove(setting);
                        }
                    }
                    catch (Exception exc)
                    {
                        logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogExceptionHeadline,
                        string.Format(Strings.DialogExceptionFormatedContent, exc.Message)
                        );
            }
        }
        
        public ICommand BrowseFolderCommand
        {
            get { return new RelayCommand(p => BrowseFolderAction()); }
        }
        private void BrowseFolderAction()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (File.Exists(TemplatePath))
                dialog.SelectedPath = TemplatePath;

            var dialogResult = dialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                TemplatePath = dialog.SelectedPath;
                ExcelSheets = ExcelHandler.GetWorksheetsFromFile(TemplatePath);
            }

        }
        
        public ICommand BrowseFileCommand
        {
            get { return new RelayCommand(p => BrowseFileAction()); }
        }
        private void BrowseFileAction()
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();

            if (File.Exists(TemplatePath))
                dialog.FileName = TemplatePath;
            dialog.Filter = "*.xlsx|*.xlsx";
            var dialogResult = dialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                TemplatePath = dialog.FileName;
                ExcelSheets = ExcelHandler.GetWorksheetsFromFile(TemplatePath);
            }

        }

        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
