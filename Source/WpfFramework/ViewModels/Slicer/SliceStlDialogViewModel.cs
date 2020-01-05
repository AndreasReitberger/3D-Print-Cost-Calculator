using log4net;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfFramework.Utilities;
using WpfFramework.Models.Slicer;
using WpfFramework.Models;
using System.Collections.ObjectModel;
using WpfFramework.Models.Settings;
using WpfFramework.Resources.Localization;
using System.Windows.Input;
using System.IO;
using System.Diagnostics;
using System.Windows.Controls;

namespace WpfFramework.ViewModels.Slicer
{
    class SliceStlDialogViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        private Models.Slicer.Slicer _slicerName;
        public Models.Slicer.Slicer SlicerName
        {
            get => _slicerName;
            set
            {
                if (_slicerName != value)
                {
                    _slicerName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ExecutionString));
                }
            }
        }

        private string _slicerCommand = string.Empty;
        public string SlicerCommand
        {
            get => _slicerCommand;
            set
            {
                if (_slicerCommand != value)
                {
                    _slicerCommand = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ExecutionString));
                }
            }
        }

        private bool _isWorking = false;
        public bool IsWorking
        {
            get => _isWorking;
            set
            {
                if (_isWorking != value)
                {
                    _isWorking = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _importGcode = true;
        public bool ImportGcode
        {
            get => _importGcode;
            set
            {
                if (_importGcode != value)
                {
                    _importGcode = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private bool _multipleInstances = true;
        public bool MultipleInstances
        {
            get => _multipleInstances;
            set
            {
                if (_multipleInstances != value)
                {
                    _multipleInstances = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ExecutionString
        {
            get
            {
                try
                {
                    var ret = string.Format("{0} {1} {2}", Path.GetFileName(SlicerName.InstallationPath), SlicerCommand, "[filepath]");
                    return ret;
                }
                catch(Exception)
                {
                    return string.Empty;
                }
            }
        }

        private string _console = string.Empty;
        public string Console
        {
            get => _console;
            set
            {
                if (_console != value)
                {
                    _console = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<string> _filesForImport = new ObservableCollection<string>();
        public ObservableCollection<string> FilesForImport
        {
            get => _filesForImport;
            set
            {
                if (_filesForImport != value)
                {
                    _filesForImport = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<Stl> _stlFiles = new ObservableCollection<Stl>();
        public ObservableCollection<Stl> StlFiles
        {
            get => _stlFiles;
            set
            {
                if (_stlFiles != value)
                {
                    _stlFiles = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Settings
        public ObservableCollection<Models.Slicer.Slicer> Slicers
        {
            get => SettingsManager.Current.Slicers;
        }
        #endregion

        #region Constructor
        public SliceStlDialogViewModel(Action<SliceStlDialogViewModel> saveCommand, Action<SliceStlDialogViewModel> cancelHandler, ObservableCollection<Stl> stlFiles)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            try
            {
                StlFiles = stlFiles;
                StlFiles.CollectionChanged += StlFiles_CollectionChanged;
                FilesForImport.CollectionChanged += FilesForImport_CollectionChanged;

                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }

        }

        

        public SliceStlDialogViewModel(Action<SliceStlDialogViewModel> saveCommand, Action<SliceStlDialogViewModel> cancelHandler, IDialogCoordinator dialogCoordinator, ObservableCollection<Stl> stlFiles)
        {
            this._dialogCoordinator = dialogCoordinator;

            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            try
            {
                StlFiles = stlFiles;
                StlFiles.CollectionChanged += StlFiles_CollectionChanged;
                FilesForImport.CollectionChanged += FilesForImport_CollectionChanged;

                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }



        #endregion

        #region Events
        private void StlFiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(StlFiles));
        }
        private void FilesForImport_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(FilesForImport));
        }
        #endregion

        #region iCommands & Actions
        public ICommand SliceCommand
        {
            get => new RelayCommand(p => SliceAction());
        }
        private async void SliceAction()
        {
            try
            {
                if(File.Exists(SlicerName.InstallationPath))
                {
                    IsWorking = true;
                    StringBuilder multileFiles = new StringBuilder();
                    Console += string.Format("{0}: {1}\n", Strings.Slicer, SlicerName.SlicerName);
                    foreach (Stl file in StlFiles)
                    {
                        if (SlicerName.Group == SlicerViewManager.Group.GUI)
                        {
                            
                            if (MultipleInstances)
                            {
                                var slicerGui = new Process()
                                {
                                    StartInfo = new ProcessStartInfo
                                    {
                                        FileName = SlicerName.InstallationPath,
                                        Arguments = string.Format("{0} {1}", SlicerCommand, file.StlFilePath),
                                        UseShellExecute = false,
                                        RedirectStandardOutput = true,
                                        CreateNoWindow = false,
                                        ErrorDialog = true,
                                    }
                                };
                                slicerGui.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                                slicerGui.Start();
                            }
                            else
                                multileFiles.AppendFormat("{0} ", file.StlFilePath);
                            
                        }
                        else if (SlicerName.Group == SlicerViewManager.Group.CLI)
                        {
                            var slicer = new Process()
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = SlicerName.InstallationPath,
                                    Arguments = string.Format("{0} {1}", SlicerCommand, file.StlFilePath),
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    CreateNoWindow = true,
                                    ErrorDialog = true,
                                }
                            };
                            slicer.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                            slicer.Start();
                            Console += string.Format("{0}: {1}\n", Strings.Command, SlicerCommand);
                            // 1 min timeout
                            slicer.WaitForExit(1000 * 60 * 1);
                            Console += string.Format("{0}\n", slicer.StandardOutput.ReadToEnd());
                            if(ImportGcode)
                            {
                                var filename = Path.GetFileNameWithoutExtension(file.StlFilePath);
                                var path = Path.Combine(Path.GetDirectoryName(file.StlFilePath), string.Format("{0}.{1}", filename, "gcode"));
                                if (File.Exists(path))
                                {
                                    FilesForImport.Add(path);
                                    Console += string.Format("{0}: {1}\n", Strings.Import, path);
                                }
                                else
                                {
                                    Console += string.Format("{0}: {1}\n", Strings.Import, string.Format(Strings.FileDoesNotExistsFormated, path));
                                }
                            }
                        }
                    }
                    if(!MultipleInstances && multileFiles.Length > 0)
                    {
                        var slicerGuiSingle = new Process()
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = SlicerName.InstallationPath,
                                Arguments = string.Format("{0} {1}", SlicerCommand, multileFiles.ToString()),
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = false,
                                ErrorDialog = true,
                            }
                        };
                        Console += string.Format("{0}: {1}\n", Strings.Command, slicerGuiSingle.StartInfo.Arguments);
                        slicerGuiSingle.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                        slicerGuiSingle.Start();
                    }
                    IsWorking = false;
                }
                else
                {
                    IsWorking = false;
                    await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogSlicerApplicationNotFoundHeadline,
                        string.Format(Strings.DialogSlicerApplicationNotFoundFormatedContent, SlicerName.SlicerName, SlicerName.InstallationPath)
                        );
                }
            }
            catch(Exception exc)
            {
                IsWorking = false;
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }

        }

        public ICommand ClearConsoleCommand
        {
            get => new RelayCommand(p => ClearConsoleAction());

        }
        private void ClearConsoleAction()
        {
            try
            {
                Console = string.Empty;
                logger.Info(Strings.EventFormCleared);
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
