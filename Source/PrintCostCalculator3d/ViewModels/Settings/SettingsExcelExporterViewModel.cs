using log4net;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using System.Windows.Input;
using PrintCostCalculator3d.Models.Exporter;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Models.Syncfusion;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;

namespace PrintCostCalculator3d.ViewModels
{
    class SettingsExcelExporterViewModel : ViewModelBase
    {
        #region Properties
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Variables
        private readonly bool _isLoading;

        private string _LocationExcelTemplatedPath;
        public string LocationExcelTemplatedPath
        {
            get => _LocationExcelTemplatedPath;
            set
            {
                if (value == _LocationExcelTemplatedPath)
                    return;
                if (!_isLoading)
                    SettingsManager.Current.ExporterExcel_TemplatePath = value;

                _LocationExcelTemplatedPath = value;
                OnPropertyChanged();
            }
        }
        
        private ObservableCollection<ExporterTemplate> _templates = new ObservableCollection<ExporterTemplate>();
        public ObservableCollection<ExporterTemplate> Templates
        {
            get => _templates;
            set
            {
                if (value == _templates)
                    return;
                if (!_isLoading)
                    SettingsManager.Current.ExporterExcel_Templates = value;

                _templates = value;
                OnPropertyChanged();
            }
        }
        
        private string _Name;
        public string Name
        {
            get => _Name;
            set
            {
                if (value == _Name)
                    return;
                
                _Name = value;
                OnPropertyChanged();
            }
        }
        private List<string> _excelSheets = new List<string>();
        public List<string> ExcelSheets
        {
            get => _excelSheets;
            set
            {
                if (value == _excelSheets)
                    return;

                _excelSheets = value;
                OnPropertyChanged();
            }
        }

        private bool _writeToTemplate = true;
        public bool WriteToTemplate
        {
            get => _writeToTemplate;
            set
            {
                if (value == _writeToTemplate)
                    return;
                if (!_isLoading)
                    SettingsManager.Current.ExporterExcel_WriteToTemplate = value;

                _writeToTemplate = value;
                OnPropertyChanged();
            }
        }
        
        private bool _showTemplateParameters = true;
        public bool ShowTemplateParameters
        {
            get => _showTemplateParameters;
            set
            {
                if (value == _showTemplateParameters)
                    return;

                _showTemplateParameters = value;
                OnPropertyChanged();
            }
        }

        private ICollectionView _templateViews;
        public ICollectionView TemplateViews
        {
            get => _templateViews;
            private set
            {
                if (_templateViews != value)
                {
                    _templateViews = value;
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
                if (_selectedSetting == value) return;
                _selectedSetting = value;
                OnPropertyChanged();
            }
        }

        private ExporterTemplate _selectedTemplateView;
        public ExporterTemplate SelectedTemplateView
        {
            get => _selectedTemplateView;
            set
            {
                if (_selectedTemplateView != value)
                {
                    _selectedTemplateView = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<Models.Slicer.Slicer> KnownSlicers
        {
            get => Models.Slicer.Slicer.SupportedSlicers;
        }
        #endregion   

        #region Constructor, LoadSettings
        public SettingsExcelExporterViewModel()
        {
            _isLoading = true;

            LoadSettings();


            _isLoading = false;
        }
        public SettingsExcelExporterViewModel(IDialogCoordinator instance)
        {
            _dialogCoordinator = instance;
            _isLoading = true;

            LoadSettings();

            _isLoading = false;
        }


        private void LoadSettings()
        {
            LocationExcelTemplatedPath = SettingsManager.Current.ExporterExcel_TemplatePath;
            WriteToTemplate = SettingsManager.Current.ExporterExcel_WriteToTemplate;
            Templates = SettingsManager.Current.ExporterExcel_Templates;
            Templates.CollectionChanged += Templates_CollectionChanged;
            createTemplateViewSource();
            if (Templates.Count > 0)
                SelectedTemplateView = Templates[0];
            //BackgroundJobInterval = SettingsManager.Current.General_BackgroundJobInterval;
        }

        private void Templates_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            createTemplateViewSource();
            SettingsManager.Save();
            OnPropertyChanged(nameof(Templates));
        }
        #endregion

        #region Private Methods
        private void createTemplateViewSource()
        {
            TemplateViews = new CollectionViewSource
            {
                Source = Templates,
            }.View;
            TemplateViews.SortDescriptions.Add(new SortDescription(nameof(ExporterTemplate.ExporterTarget), ListSortDirection.Ascending));
            TemplateViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ExporterTemplate.ExporterTarget)));
        }
        #endregion

        #region ICommands & Actions
        public ICommand DeleteSelectedTemplateCommand
        {
            get { return new RelayCommand(p => DeleteSelectedTemplateAction()); }
        }
        private async void DeleteSelectedTemplateAction()
        {
            try
            {
                if (SelectedTemplateView == null)
                    return;
                var res = await _dialogCoordinator.ShowMessageAsync(this,
                    Strings.DialogDeleteExportTemplateHeadline,
                    string.Format(Strings.DialogDeleteExportTemplateFormatContent, SelectedTemplateView.Name),
                    MessageDialogStyle.AffirmativeAndNegative
                    );
                if(res == MessageDialogResult.Affirmative)
                {
                    Templates.Remove(SelectedTemplateView);
                    //createTemplateViewSource();
                }
            }
            catch(Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
        public ICommand AddExcelCoordinateCommand
        {
            get { return new RelayCommand(p => AddExcelCoordinateAction()); }
        }

        private async void AddExcelCoordinateAction()
        {

            try
            {
                var _dialog = new CustomDialog() { Title = Strings.NewExcelExporterSettings };
                var newExporterSettingsViewModel = new NewExcelExporterSettingViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    SelectedTemplateView.Settings.Add(new ExporterSettings()
                    {
                        Id = instance.Id,
                        WorkSheetName = instance.Worksheet,
                        Attribute = instance.Property,
                        Coordinates = new ExcelCoordinates() { Column = instance.Column, Row = Convert.ToInt32(instance.Row) },

                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, 
                        SelectedTemplateView.Settings[SelectedTemplateView.Settings.Count - 1].ToString()));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                }
                , ExcelHandler.GetWorksheetsFromFile(SelectedTemplateView.TemplatePath)
                , SelectedTemplateView.ExporterTarget
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
        
        public ICommand AddExcelTemplateCommand
        {
            get { return new RelayCommand(p => AddExcelTemplateAction()); }
        }
        private async void AddExcelTemplateAction()
        {

            try
            {
                var _dialog = new CustomDialog() { Title = Strings.NewExcelExporterTemplate };
                var newExporterViewModel = new NewExcelTemplateViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Templates.Add(new ExporterTemplate()
                    {
                        Name = instance.Name,
                        Id = instance.Id,
                        ExporterTarget = instance.Target,
                        TemplatePath = instance.TemplatePath,
                        Settings = instance.Settings,

                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Templates[Templates.Count - 1].ToString()));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                }
                , _dialogCoordinator
                );

                _dialog.Content = new Views.NewExcelTemplateDialogView()
                {
                    DataContext = newExporterViewModel
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
                    SelectedTemplateView.Settings.Remove(SelectedSetting);
                    SelectedTemplateView.Settings.Add(new ExporterSettings()
                    {
                        Id = instance.Id,
                        WorkSheetName = instance.Worksheet,
                        Attribute = instance.Property,
                        Coordinates = new ExcelCoordinates() { Column = instance.Column, Row = Convert.ToInt32(instance.Row) },

                    });
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                }
                , ExcelHandler.GetWorksheetsFromFile(SelectedTemplateView.TemplatePath)
                , SelectedTemplateView.ExporterTarget
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
                if (SelectedSetting == null) return;
                var res = await _dialogCoordinator.ShowMessageAsync(this,
                    Strings.DialogDeleteHeadline,
                    Strings.DialogDeleteContent,
                    MessageDialogStyle.AffirmativeAndNegative
                    );
                if (res == MessageDialogResult.Affirmative)
                {
                    try
                    {
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, SelectedSetting.ToString()));
                        SelectedTemplateView.Settings.Remove(SelectedSetting);
                        /*
                        var collection = new ArrayList(SelectedSettings);
                        for (int i = 0; i < collection.Count; i++)
                        {
                            var setting = collection[i] as ExporterSettings;
                            if (setting == null)
                                continue;
                            logger.Info(string.Format(Strings.EventDeletedItemFormated, setting.ToString()));
                            SelectedTemplateView.Settings.Remove(setting);
                        }
                        */
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

            if (File.Exists(LocationExcelTemplatedPath))
                dialog.SelectedPath = LocationExcelTemplatedPath;

            var dialogResult = dialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
                LocationExcelTemplatedPath = dialog.SelectedPath;
        }

        public ICommand BrowseFileCommand
        {
            get { return new RelayCommand(p => BrowseFileAction()); }
        }
        private void BrowseFileAction()
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();

            if (File.Exists(SelectedTemplateView.TemplatePath))
                dialog.FileName = SelectedTemplateView.TemplatePath;
            dialog.Filter = "*.xlsx|*.xlsx";
            var dialogResult = dialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                SelectedTemplateView.TemplatePath = dialog.FileName;
                ExcelSheets = ExcelHandler.GetWorksheetsFromFile(SelectedTemplateView.TemplatePath);
            }

        }
        #endregion

        #region Methods
        public void SetLocationPathFromDragDrop(string path)
        {
            LocationExcelTemplatedPath = path;

            OnPropertyChanged(nameof(LocationExcelTemplatedPath));
        }
        public void OnViewVisible()
        {

        }

        public void OnViewHide()
        {

        }
        #endregion
    }

}
