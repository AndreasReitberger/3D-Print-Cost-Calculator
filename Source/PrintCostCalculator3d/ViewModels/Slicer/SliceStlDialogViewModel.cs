using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Models;
using PrintCostCalculator3d.Models.Documentation;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Models.Slicer;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels.Slicer
{
    class SliceStlDialogViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Properties
        Models.Slicer.Slicer _slicerName;
        public Models.Slicer.Slicer SlicerName
        {
            get => _slicerName;
            set
            {
                if (_slicerName == value) return;

                if (!IsLoading)
                    SettingsManager.Current.Slicer_LastUsed = value;

                _slicerName = value;
                createSlicerCommandViewInfos();

                OnPropertyChanged();
                updateExecutionString();
                //OnPropertyChanged(nameof(ExecutionString));
                
            }
        }

        #region Commands 
        

        ObservableCollection<SlicerCommand> _commands = new ObservableCollection<SlicerCommand>();
        public ObservableCollection<SlicerCommand> Commands
        {
            get => _commands;
            set
            {
                if (_commands == value) return;
                if (!IsLoading)
                    SettingsManager.Current.SlicerCommands = value;

                _commands = value;
                createSlicerCommandViewInfos();
                OnPropertyChanged();
            }
        }

        public ICollectionView CommandsView
        {
            get => _commandsView;
            set
            {
                if (_commandsView != value)
                {
                    _commandsView = value;
                    OnPropertyChanged();
                }
            }
        }
        ICollectionView _commandsView;

        SlicerCommand _selectedSlicerCommand;
        public SlicerCommand SelectedSlicerCommand
        {
            get => _selectedSlicerCommand;
            set
            {
                if (_selectedSlicerCommand == value)
                    return;
                _selectedSlicerCommand = value;
                
                OnPropertyChanged();
            }
        }

        IList _SelectedSlicerCommands = new ArrayList();
        public IList SelectedSlicerCommands
        {
            get => _SelectedSlicerCommands;
            set
            {
                if (Equals(value, _SelectedSlicerCommands))
                    return;

                _SelectedSlicerCommands = value;
                OnPropertyChanged();
            }
        }
        #endregion

        string _slicerCommand = string.Empty;
        public string SlicerCommand
        {
            get => _slicerCommand;
            set
            {
                if (_slicerCommand != value)
                {
                    _slicerCommand = value;
                    OnPropertyChanged();
                    updateExecutionString();
                    //OnPropertyChanged(nameof(ExecutionString));
                }
            }
        }

        string _outputFileFormat = "([filename])+([A-Za-z0-9-_.])+(.gcode|.gco|.gc)$";
        public string OutputFileFormat
        {
            get => _outputFileFormat;
            set
            {
                if (_outputFileFormat == value) return;

                _outputFileFormat = value;
                OnPropertyChanged();
                
            }
        }

        bool _isWorking = false;
        public bool IsWorking
        {
            get => _isWorking;
            set
            {
                if (_isWorking == value) return;
                
                _isWorking = value;
                OnPropertyChanged();
                
            }
        }

        bool _importGcode = true;
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

        bool _includeFilePathAutomatically = true;
        public bool IncludeFilePathAutomatically
        {
            get => _includeFilePathAutomatically;
            set
            {
                if (_includeFilePathAutomatically == value) return;
                
                _includeFilePathAutomatically = value;
                updateExecutionString();
                OnPropertyChanged();
                
            }
        }
        
        bool _multipleInstances = true;
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

        string _executionString;
        public string ExecutionString
        {
            get => _executionString;
            set
            {
                if (_executionString == value) return;
                _executionString = value;
                updateExecutionString();
                OnPropertyChanged();
            }
        }

        string _console = string.Empty;
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

        ObservableCollection<string> _filesForImport = new ObservableCollection<string>();
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

        ObservableCollection<Stl> _stlFiles = new ObservableCollection<Stl>();
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
        ObservableCollection<Models.Slicer.Slicer> _slicers = new ObservableCollection<Models.Slicer.Slicer>();
        public ObservableCollection<Models.Slicer.Slicer> Slicers
        {
            get => _slicers;
            set
            {
                if (_slicers == value) return;
                _slicers = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor
        public SliceStlDialogViewModel(Action<SliceStlDialogViewModel> saveCommand, Action<SliceStlDialogViewModel> cancelHandler, ObservableCollection<Stl> stlFiles)
        {
            //Commands.CollectionChanged += Commands_CollectionChanged;

            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));

            IsLicenseValid = false;

            try
            {
                IsLoading = true;
                LoadSettings();
                IsLoading = false;

                StlFiles = stlFiles;
                StlFiles.CollectionChanged += StlFiles_CollectionChanged;
                FilesForImport.CollectionChanged += FilesForImport_CollectionChanged;
                Commands.CollectionChanged += Commands_CollectionChanged;

                logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }

        }

        public SliceStlDialogViewModel(Action<SliceStlDialogViewModel> saveCommand, Action<SliceStlDialogViewModel> cancelHandler, IDialogCoordinator dialogCoordinator, ObservableCollection<Stl> stlFiles)
        {
            _dialogCoordinator = dialogCoordinator;

            Commands.CollectionChanged += Commands_CollectionChanged;

            IsLicenseValid = false;

            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            try
            {
                IsLoading = true;
                LoadSettings();
                IsLoading = false;

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

        void Commands_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            createSlicerCommandViewInfos();            
            //OnPropertyChanged(nameof(Commands));
            SettingsManager.Save();
        }

        void LoadSettings()
        {
            Slicers = SettingsManager.Current.Slicers;
            Commands = SettingsManager.Current.SlicerCommands;
            if (Slicers.Count > 0)
            {
                if (SettingsManager.Current.Slicer_LastUsed != null)
                {
                    SlicerName = Slicers.FirstOrDefault(slicer => slicer.Equals(SettingsManager.Current.Slicer_LastUsed));
                }
                if (SlicerName == null)
                {
                    try
                    {
                        if (Slicers.Count > 0)
                        {
                            SlicerName = Slicers[0];
                            SettingsManager.Current.Slicer_LastUsed = SlicerName;
                        }
                    }
                    catch(Exception exc)
                    {
                        logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.Message, exc.TargetSite);
                    }
                }
            }
        }

        #endregion

        #region Methods
        void updateExecutionString()
        {
            try
            {
                ExecutionString = string.Format("{0} {1} {2}", Path.GetFileName(SlicerName.InstallationPath), SlicerCommand, IncludeFilePathAutomatically ? "[filepath]" : "");

            }
            catch (Exception exc)
            {
                ExecutionString = string.Empty;
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        void createSlicerCommandViewInfos()
        {
            CommandsView = new CollectionViewSource
            {
                Source = (Commands.Select(m => m)).ToList()
            }.View;
            CommandsView.SortDescriptions.Add(new SortDescription(nameof(Models.Slicer.SlicerCommand.Slicer.SlicerName), ListSortDirection.Ascending));
            CommandsView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Models.Slicer.SlicerCommand.Slicer.SlicerName)));
            filterModelView();
        }

        void filterModelView()
        {
            CommandsView.Refresh();

            ICollectionView view = CollectionViewSource.GetDefaultView(CommandsView);
            IEqualityComparer<String> comparer = StringComparer.InvariantCultureIgnoreCase;
            view.Filter = o =>
            {
                Models.Slicer.SlicerCommand cmd = o as Models.Slicer.SlicerCommand;
                return cmd.Slicer.Equals(SlicerName);              
            };
        }

        #endregion

        #region Events
        void StlFiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(StlFiles));
        }
        void FilesForImport_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(FilesForImport));
        }
        #endregion

        #region iCommands & Actions
        public ICommand OpenDocumentationCommand
        {
            get { return new RelayCommand(p => OpenDocumentationAction()); }
        }

        void OpenDocumentationAction()
        {
            DocumentationManager.OpenDocumentation(DocumentationIdentifier.SlicerDialog);
        }
        public ICommand SliceCommand
        {
            get => new RelayCommand(async(p) => await SliceAction());
        }
        async Task SliceAction()
        {
            try
            {
                if(File.Exists(SlicerName.InstallationPath))
                {
                    string filePathPlacehoder = "[filepath]";

                    Console = string.Empty;
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
                                        Arguments = string.IsNullOrEmpty(SlicerCommand) ? string.Format("\"{0}\"", file.StlFilePath) :  SlicerCommand.Replace(filePathPlacehoder, string.Format("\"{0}\"", file.StlFilePath)), //string.Format("{0} \"{1}\"", SlicerCommand, file.StlFilePath),
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
                                multileFiles.AppendFormat("{0} ", string.Format("\"{0}\"", file.StlFilePath));
                            
                        }
                        else if (SlicerName.Group == SlicerViewManager.Group.CLI)
                        {
                            if (string.IsNullOrEmpty(SlicerCommand)) // || !SlicerCommand.Contains(filePathPlacehoder))
                            {
                                await _dialogCoordinator.ShowMessageAsync(this,
                                    Strings.DialogCommandCannotBeEmptyOrMissingTheFilepathTagHeadline,
                                    Strings.DialogCommandCannotBeEmptyOrMissingTheFilepathTagContent
                                    );
                                return;
                            }
                            var slicer = new Process()
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = SlicerName.InstallationPath,
                                    Arguments = SlicerCommand.Replace(filePathPlacehoder, string.Format("\"{0}\"", file.StlFilePath)),
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    CreateNoWindow = true,
                                    ErrorDialog = true,
                                }
                            };
                            slicer.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                            slicer.Start();
                            Console += string.Format("{0}: {1}\n", Strings.Command, slicer.StartInfo.Arguments);
                            // 1 min timeout
                            slicer.WaitForExit(1000 * 60 * 1);
                            Console += string.Format("{0}\n", slicer.StandardOutput.ReadToEnd());
                            if(ImportGcode)
                            {
                                var filename = Path.GetFileNameWithoutExtension(file.StlFilePath);
                                string path = Path.GetDirectoryName(file.StlFilePath);

                                Console += string.Format("{0}: {1}\n", Strings.FileName, filename);
                                Console += string.Format("{0} {1}\n", Strings.LabelCurrentPath, path);

                                var files = Directory
                                    .EnumerateFiles(path) //<--- .NET 4.5
                                    .Where(f => f.ToLower().EndsWith(".gcode") || f.ToLower().EndsWith(".gco") || f.ToLower().EndsWith(".g"))
                                    .ToList();
                                foreach (string f in files)
                                {
                                    var nFilename = Path.GetFileName(f);
                                    Regex r = new Regex(OutputFileFormat);

                                    if (r.IsMatch(nFilename))
                                    {
                                        if (File.Exists(f))
                                        {
                                            if(!FilesForImport.Contains(f))
                                            FilesForImport.Add(f);
                                            Console += string.Format("{0}: {1}\n", Strings.Import, f);
                                        }
                                        else
                                        {
                                            Console += string.Format("{0}: {1}\n", Strings.Import, string.Format(Strings.FileDoesNotExistsFormated, f));
                                        }
                                    }
                                    else
                                    {
                                        Console += string.Format("{0}: {1}\n", Strings.Import, string.Format(Strings.FileNameDoesNotMatchRegexPatternFormated, f, OutputFileFormat));
                                    }
                                }
                                //var fullPath = Path.Combine(Path.GetDirectoryName(file.StlFilePath), newFilename); // string.Format("{0}.{1}", filename, "gcode"));
                                
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
        void ClearConsoleAction()
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
        
        public ICommand SelectedCommandChangedCommand
        {
            get => new RelayCommand(p => SelectedCommandChangedAction(p));

        }
        void SelectedCommandChangedAction(object obj)
        {
            try
            {
                SlicerCommand cmd = obj as SlicerCommand;
                if (cmd == null)
                    return;

                SlicerCommand = cmd.Command;
                IncludeFilePathAutomatically = cmd.AutoAddFilePath;
                OutputFileFormat = cmd.OutputFilePatternString;
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        public ICommand SelectedSlicerChangedCommand
        {
            get => new RelayCommand(p => SelectedSlicerChangedAction(p));

        }
        void SelectedSlicerChangedAction(object obj)
        {
            try
            {

                //slicer
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand SaveSlicerCommandStringCommand
        {
            get => new RelayCommand(p => SaveSlicerCommandStringAction(p));

        }
        async void SaveSlicerCommandStringAction(object obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(SlicerCommand))
                {
                    if (SelectedSlicerCommand != null)
                    {
                        var oldCMD = SelectedSlicerCommand;
                        var res = await _dialogCoordinator.ShowMessageAsync(this,
                            Strings.DialogOverwriteSlicerCommandOrSaveAsNewHeadline,
                            string.Format(Strings.DialogOverwriteSlicerCommandOrSaveAsNewFormatedContent, oldCMD.Name),
                            MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings()
                            {
                                AffirmativeButtonText = Strings.YesOverwrite,
                                NegativeButtonText = Strings.NoSaveAsNew,
                            }
                            );
                        if (res == MessageDialogResult.Affirmative)
                        {
                            Commands.Remove(Commands.First(item => item.Name == SelectedSlicerCommand.Name));
                            Commands.Add(new Models.Slicer.SlicerCommand()
                            {
                                Name = oldCMD.Name,
                                Command = SlicerCommand,
                                AutoAddFilePath = IncludeFilePathAutomatically,
                                OutputFilePatternString = OutputFileFormat,
                                Slicer = SlicerName,
                            });
                            Commands = new ObservableCollection<SlicerCommand>(Commands);
                            createSlicerCommandViewInfos();
                            SelectedSlicerCommand = Commands[Commands.Count - 1];
                            
                        }
                        else
                        {
                            string name = await _dialogCoordinator.ShowInputAsync(this,
                                Strings.AddAndSaveSliceCommandToList,
                                Strings.EnterANameForTheSlicerCommand
                                );
                            if (!string.IsNullOrEmpty(name))
                            {
                                var command = Commands.FirstOrDefault(cmd => cmd.Command == SlicerCommand);
                                if (command == null)
                                {
                                    Commands.Add(new Models.Slicer.SlicerCommand()
                                    {
                                        Command = SlicerCommand,
                                        Slicer = SlicerName,
                                        Name = name,
                                        AutoAddFilePath = IncludeFilePathAutomatically,
                                        OutputFilePatternString = OutputFileFormat,
                                    });
                                    Commands = new ObservableCollection<SlicerCommand>(Commands);
                                    createSlicerCommandViewInfos();
                                    SelectedSlicerCommand = Commands.FirstOrDefault(scmd => scmd.Slicer == command.Slicer && scmd.Command == command.Command);
                                    await _dialogCoordinator.ShowMessageAsync(this,
                                        Strings.DialogSlicerCommandSavedHeadline,
                                        string.Format(Strings.DialogSlicerCommandSavedFormatContent, name)
                                        );
                                }
                                else
                                {
                                    SelectedSlicerCommand = command;
                                    await _dialogCoordinator.ShowMessageAsync(this,
                                        Strings.DialogSlicerCommandAlreadyExistsHeadline,
                                        string.Format(Strings.DialogSlicerCommandAlreadyExistsFormatContent, SlicerCommand, command.Name)
                                        );

                                }
                            }
                        }
                    }
                    else
                    { 
                        string name = await _dialogCoordinator.ShowInputAsync(this,
                            Strings.AddAndSaveSliceCommandToList,
                            Strings.EnterANameForTheSlicerCommand
                            );
                        if (!string.IsNullOrEmpty(name))
                        {
                            var command = Commands.FirstOrDefault(cmd => cmd.Command == SlicerCommand);
                            if (command == null)
                            {
                                Commands.Add(new Models.Slicer.SlicerCommand() {
                                    Command = SlicerCommand,
                                    Slicer = SlicerName,
                                    Name = name,
                                    AutoAddFilePath = IncludeFilePathAutomatically,
                                    OutputFilePatternString = OutputFileFormat,
                                });
                                Commands = new ObservableCollection<SlicerCommand>(Commands);
                                createSlicerCommandViewInfos();
                                SelectedSlicerCommand = Commands.FirstOrDefault(scmd => scmd.Slicer == command.Slicer && scmd.Command == command.Command);
                                await _dialogCoordinator.ShowMessageAsync(this,
                                    Strings.DialogSlicerCommandSavedHeadline,
                                    string.Format(Strings.DialogSlicerCommandSavedFormatContent, name)
                                    );
                            }
                            else
                            {
                                SelectedSlicerCommand = command;
                                await _dialogCoordinator.ShowMessageAsync(this,
                                    Strings.DialogSlicerCommandAlreadyExistsHeadline,
                                    string.Format(Strings.DialogSlicerCommandAlreadyExistsFormatContent, SlicerCommand, command.Name)
                                    );

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

        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
