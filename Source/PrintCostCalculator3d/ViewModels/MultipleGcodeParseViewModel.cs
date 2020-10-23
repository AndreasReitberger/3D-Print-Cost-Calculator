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
using System.Windows;
using System.Windows.Input;
using PrintCostCalculator3d.Models.GCode;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;

namespace PrintCostCalculator3d.ViewModels
{
    class MultipleGcodeParseViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Properties
        private bool _isUploading = false;
        public bool IsUploading
        {
            get => _isUploading;
            set
            {
                if (_isUploading == value)
                    return;
                _isUploading = value;
                OnPropertyChanged();
            }
        }

        private int _Progress = 0;
        public int Progress
        {
            get => _Progress;
            set
            {
                if (_Progress == value)
                    return;
                _Progress = value;
                OnPropertyChanged();
            }
        }

        private string _uploadMessage = string.Empty;
        public string UploadMessage
        {
            get => _uploadMessage;
            set
            {
                if (_uploadMessage == value)
                    return;
                _uploadMessage = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<GCode> _gcodes = new ObservableCollection<GCode>();
        public ObservableCollection<GCode> Gcodes
        {
            get => _gcodes;
            set
            {
                if (_gcodes != value)
                {
                    _gcodes = value;
                    OnPropertyChanged();
                }
            }
        }

        private IList _selectedGcodeFiles = new ArrayList();
        public IList SelectedGcodeFiles
        {
            get => _selectedGcodeFiles;
            set
            {
                if (Equals(value, _selectedGcodeFiles))
                    return;

                _selectedGcodeFiles = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor
        public MultipleGcodeParseViewModel(Action<MultipleGcodeParseViewModel> saveCommand, Action<MultipleGcodeParseViewModel> cancelHandler, IDialogCoordinator dialogCoordinator, ObservableCollection<GCode> files = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            this._dialogCoordinator = dialogCoordinator;

            try
            {
                Gcodes = files ?? new ObservableCollection<GCode>();
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        #endregion

        #region Events
        private void OnProgressUpdateAction(int progress)
        {
            Progress = progress;
        }
        #endregion

        #region iCommands & Actions
        
        public ICommand ReadGcodeFileCommand
        {
            get => new RelayCommand(p => ReadGcodeFileAction());
        }
        private void ReadGcodeFileAction()
        {
            try
            {
                var openFileDialog = new System.Windows.Forms.OpenFileDialog
                {
                    Filter = StaticStrings.FilterGCodeFile,
                    Multiselect = true,
                };

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    foreach (string file in openFileDialog.FileNames)
                    {
                        string ext = Path.GetExtension(file);
                        if (ext.ToLower() != ".gcode" && ext.ToLower() != ".gc" && ext.ToLower() != ".gco")
                            continue;
                        try
                        {
                            GCode f = new GCode(file);
                            Gcodes.Add(f);
                            logger.Info(string.Format(Strings.EventFileLoadedForamated, file));
                        }
                        catch (Exception exc)
                        {
                            logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                        }
                    }

                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }

        }

        public ICommand OnDropFileCommand
        {
            get => new RelayCommand(p => OnDropFileAction(p));
        }
        private void OnDropFileAction(object p)
        {
            try
            {
                DragEventArgs args = p as DragEventArgs;
                if (p != null)
                {
                    if (args.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        string[] files = (string[])args.Data.GetData(DataFormats.FileDrop);
                        if (files.Count() > 0)
                        {
                            foreach (string file in files)
                            {
                                string ext = Path.GetExtension(file);
                                if (ext.ToLower() != ".gcode" && ext.ToLower() != ".gc" && ext.ToLower() != ".gco")
                                    continue;
                                try
                                {
                                    GCode f = new GCode(file);
                                    Gcodes.Add(f);
                                    logger.Info(string.Format(Strings.EventFileLoadedForamated, file));
                                }
                                catch (Exception exc)
                                {
                                    logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand DeleteSelectedFilesCommand
        {
            get { return new RelayCommand(p => DeleteSelectedFilesAction()); }
        }
        private async void DeleteSelectedFilesAction()
        {
            try
            {
                var res = await _dialogCoordinator.ShowMessageAsync(this,
                    Strings.DialogRemoveSelectedFilesFromListHeadline,
                    Strings.DialogRemoveSelectedFilesFromListContent,
                    MessageDialogStyle.AffirmativeAndNegative
                    );
                if (res == MessageDialogResult.Affirmative)
                {
                    try
                    {
                        IList collection = new ArrayList(SelectedGcodeFiles);
                        for (int i = 0; i < collection.Count; i++)
                        {
                            var file = collection[i] as GCode;
                            if (file == null)
                                continue;
                            logger.Info(string.Format(Strings.EventDeletedItemFormated, file.FileName));
                            Gcodes.Remove(file);
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

        public ICommand ParseFilesCommand
        {
            get => new RelayCommand(async(p) => await UploadFilesAction());
        }
        private async Task UploadFilesAction()
        {
            try
            {
                IsUploading = true;
                IProgress<int> prog = new Progress<int>(percent => OnProgressUpdateAction(percent));
                int i = 0;

                foreach (GCode f in Gcodes)
                {

                }
                Progress = 0;
                Gcodes.Clear();
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
            IsUploading = false;
        }
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
