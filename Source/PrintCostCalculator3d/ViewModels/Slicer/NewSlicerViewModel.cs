using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Models.Slicer;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels.Slicer
{
    class NewSlicerViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

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
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        SlicerName _slicerName = SlicerName.Unkown;
        public SlicerName SlicerName
        {
            get => _slicerName;
            set
            {
                if(_slicerName != value)
                {
                    _slicerName = value;
                    OnPropertyChanged();
                }
            }
        }
        
        SlicerViewManager.Group _slicergroup = SlicerViewManager.Group.GUI;
        public SlicerViewManager.Group SlicerGroup
        {
            get => _slicergroup;
            set
            {
                if(_slicergroup != value)
                {
                    _slicergroup = value;
                    OnPropertyChanged();
                }
            }
        }

        string _slicerPath = string.Empty;
        public string SlicerPath
        {
            get => _slicerPath;
            set
            {
                if(_slicerPath != value)
                {
                    _slicerPath = value;
                    OnPropertyChanged();
                }
            }
        }
        string _downloadUri = string.Empty;
        public string DownloadUri
        {
            get => _downloadUri;
            set
            {
                if(_downloadUri != value)
                {
                    _downloadUri = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Settings
        public ObservableCollection<Models.Slicer.Slicer> Slicers
        {
            get => SettingsManager.Current.Slicers;
            set
            {
                if (value != SettingsManager.Current.Slicers)
                {
                    SettingsManager.Current.Slicers = value;
                    OnPropertyChanged();
                }
            }

        }
        #endregion

        #region Constructor
        public NewSlicerViewModel(Action<NewSlicerViewModel> saveCommand, Action<NewSlicerViewModel> cancelHandler, Models.Slicer.Slicer slicer = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            IsLicenseValid = false;

            Slicers.CollectionChanged += Slicers_CollectionChanged;

            IsEdit = slicer != null;
            try
            {
                var slicerInfo = slicer ?? new Models.Slicer.Slicer();
                if (slicer != null)
                    Id = slicerInfo.Id;
                //Price = printerInfo.Price;
                SlicerName = slicerInfo.SlicerName;
                SlicerGroup = slicerInfo.Group;
                SlicerPath = slicerInfo.InstallationPath;
                DownloadUri = slicerInfo.DownloadUri;


                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }

        }
        
        public NewSlicerViewModel(Action<NewSlicerViewModel> saveCommand, Action<NewSlicerViewModel> cancelHandler, IDialogCoordinator dialogCoordinator, Models.Slicer.Slicer slicer = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            _dialogCoordinator = dialogCoordinator;

            IsLicenseValid = false;

            Slicers.CollectionChanged += Slicers_CollectionChanged;

            IsEdit = slicer != null;
            try
            {
                var slicerInfo = slicer ?? new Models.Slicer.Slicer();
                if (slicer != null)
                    Id = slicerInfo.Id;
                //Price = printerInfo.Price;
                SlicerName = slicerInfo.SlicerName;
                SlicerGroup = slicerInfo.Group;
                SlicerPath = slicerInfo.InstallationPath;
                DownloadUri = slicerInfo.DownloadUri;

                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        public NewSlicerViewModel(Action<NewSlicerViewModel> saveCommand, Action<NewSlicerViewModel> cancelHandler, IDialogCoordinator dialogCoordinator, string path)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            this._dialogCoordinator = dialogCoordinator;

            Slicers.CollectionChanged += Slicers_CollectionChanged;

            IsEdit = false;
            try
            {
                SlicerPath = path;

                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        #endregion

        #region Events
        void Slicers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SettingsManager.Save();
            OnPropertyChanged(nameof(Slicers));
        }
        #endregion

        #region iCommands & Actions
        public ICommand BrowseSlicerAppCommand
        {
            get => new RelayCommand(p => BrowseSlicerAppAction());
        }
        async void BrowseSlicerAppAction()
        {
            try
            {
                var openFileDialog = new System.Windows.Forms.OpenFileDialog
                {
                    Filter = Strings.FilterApplicationFile, 
                };
                if (!string.IsNullOrEmpty(SlicerPath) && File.Exists(SlicerPath))
                {
                    openFileDialog.InitialDirectory = Path.GetDirectoryName(SlicerPath);
                    openFileDialog.FileName = Path.GetFileName(SlicerPath);
                }

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string ext = Path.GetExtension(openFileDialog.FileName);
                    if(ext == ".exe")
                        SlicerPath = openFileDialog.FileName;
                    else
                    {
                        await _dialogCoordinator.ShowMessageAsync(this,
                            Strings.DialogInvalidFileTypeSelectedHeadline,
                            string.Format(Strings.DialogInvalidFileTypeSelectedFormatContent, ext, ".exe"));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }

        }
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
