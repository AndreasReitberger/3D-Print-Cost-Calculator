using PrintCostCalculator3d.Utilities;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using PrintCostCalculator3d.Models.Slicer;
using PrintCostCalculator3d.Models.Settings;
using System.ComponentModel;
using System.Collections;
using System.Windows.Data;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;
using System.Windows.Input;
using PrintCostCalculator3d.Resources.Localization;
using MahApps.Metro.SimpleChildWindow;
using System.Windows;
using System.Windows.Media;
using PrintCostCalculator3d.ViewModels.Slicer;
using log4net;
using System.Diagnostics;
using System.IO;
using IWshRuntimeLibrary;
using PrintCostCalculator3d.Converters;

namespace PrintCostCalculator3d.ViewModels
{
    class SettingsSlicerViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool _isLoading = false;
        #endregion

        #region Properties

        #region Slicers
        private ObservableCollection<Models.Slicer.Slicer> _slicers = new ObservableCollection<Models.Slicer.Slicer>();
        public ObservableCollection<Models.Slicer.Slicer> Slicers
        {
            get => _slicers;
            set
            {
                if (_slicers == value) return;

                if (!_isLoading)
                    SettingsManager.Current.Slicers = value;

                _slicers = value;
                OnPropertyChanged();
                
            }
        }

        private ICollectionView _SlicerViews;
        public ICollectionView SlicerViews
        {
            get => _SlicerViews;
            private set
            {
                if (_SlicerViews != value)
                {
                    _SlicerViews = value;
                    OnPropertyChanged();
                }
            }
        }
        

        private SlicerViewInfo _selectedSlicerView;
        public SlicerViewInfo SelectedSlicerView
        {
            get => _selectedSlicerView;
            set
            {
                if (_selectedSlicerView != value)
                {
                    _selectedSlicerView = value;
                    OnPropertyChanged();
                }
            }
        }

        private IList _selectedSlicersView = new ArrayList();
        public IList SelectedSlicersView
        {
            get => _selectedSlicersView;
            set
            {
                if (Equals(value, _selectedSlicersView))
                    return;

                _selectedSlicersView = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        private ObservableCollection<SlicerCommand> _slicerCommands;
        public ObservableCollection<SlicerCommand> SlicerCommands
        {
            get => _slicerCommands;
            private set
            {
                if (_slicerCommands == value) return;

                if (!_isLoading)
                    SettingsManager.Current.SlicerCommands = value;

                _slicerCommands = value;
                OnPropertyChanged();
                
            }
        }

        private ICollectionView _SlicerCommandViews;
        public ICollectionView SlicerCommandViews
        {
            get => _SlicerCommandViews;
            private set
            {
                if (_SlicerCommandViews == value) return;
                
                _SlicerCommandViews = value;
                OnPropertyChanged();
                
            }
        }


        private SlicerCommand _selectedSlicerCommandView;
        public SlicerCommand SelectedSlicerCommandView
        {
            get => _selectedSlicerCommandView;
            set
            {
                if (_selectedSlicerCommandView == value) return;
                
                _selectedSlicerCommandView = value;
                OnPropertyChanged();
                
            }
        }

        private IList _selectedSlicerCommandsView = new ArrayList();
        public IList SelectedSlicerCommandsView
        {
            get => _selectedSlicerCommandsView;
            set
            {
                if (Equals(value, _selectedSlicerCommandsView))
                    return;

                _selectedSlicerCommandsView = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #endregion

        #region Search
        private string _searchSlicer = string.Empty;
        public string SearchSlicer
        {
            get => _searchSlicer;
            set
            {
                if (_searchSlicer != value)
                {
                    _searchSlicer = value;

                    SlicerViews.Refresh();

                    ICollectionView view = CollectionViewSource.GetDefaultView(SlicerViews);
                    IEqualityComparer<String> comparer = StringComparer.InvariantCultureIgnoreCase;
                    view.Filter = o =>
                    {
                        SlicerViewInfo p = o as SlicerViewInfo;
                        return p.Name.Contains(_searchSlicer);
                    };
                    OnPropertyChanged();
                }
            }
        }

        private string _searchSlicerCommand = string.Empty;
        public string SearchSlicerCommand
        {
            get => _searchSlicerCommand;
            set
            {
                if (_searchSlicerCommand != value)
                {
                    _searchSlicerCommand = value;

                    SlicerCommandViews.Refresh();

                    ICollectionView view = CollectionViewSource.GetDefaultView(SlicerCommandViews);
                    IEqualityComparer<String> comparer = StringComparer.InvariantCultureIgnoreCase;
                    view.Filter = o =>
                    {
                        try
                        {
                            SlicerCommand scmd = o as SlicerCommand;
                            if (scmd == null) return false;

                            string[] patterns = _searchSlicerCommand.ToLower().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            if (patterns.Length == 1 || patterns.Length == 0)
                                return scmd.Name.ToLower().Contains(_searchSlicerCommand.ToLower());                           
                            else
                            {
                                return (!string.IsNullOrEmpty(scmd.Name) && patterns.Any(scmd.Name.ToLower().Contains)) || 
                                       (!string.IsNullOrEmpty(scmd.Name) && patterns.Any(scmd.Slicer.SlicerName.ToString().ToLower().Contains));
                            }
                        }
                        catch(Exception)
                        { return false; }
                    };
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Constructor
        public SettingsSlicerViewModel(IDialogCoordinator instance)
        {
            _isLoading = true;
            LoadSettings();
            _isLoading = false;

            _dialogCoordinator = instance;
            Slicers.CollectionChanged += Slicers_CollectionChanged;
            SlicerCommands.CollectionChanged += SlicerCommands_CollectionChanged; 
            createSlicerViewInfos();
            createSlicerCommandViewInfos();
        }

        private void SlicerCommands_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            createSlicerCommandViewInfos();
            SettingsManager.Save();
        }

        private void LoadSettings()
        {
            SlicerCommands = SettingsManager.Current.SlicerCommands;
            Slicers = SettingsManager.Current.Slicers;
        }

        private void Slicers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            createSlicerViewInfos();
            SettingsManager.Save();
        }
        #endregion

        #region ICommand & Actions

        #region Slicer
        public ICommand OnDropLnkFileCommand
        {
            get => new RelayCommand(p => OnDropLnkFileAction(p));
        }
        private async void OnDropLnkFileAction(object p)
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
                                if (ext.ToLower() != ".lnk")
                                {
                                    await _dialogCoordinator.ShowMessageAsync(this,
                                        Strings.DialogFileTypeNotSupportedHeadline,
                                        string.Format(Strings.DialogFileTypeNotSupportedFormatedContent, ext, file)
                                        );
                                    continue;
                                }
                                else
                                {
                                    try
                                    {
                                        WshShell shell = new WshShell(); //Create a new WshShell Interface
                                        IWshShortcut link = (IWshShortcut)shell.CreateShortcut(file);

                                        var _dialog = new CustomDialog() { Title = Strings.AddSlicer };
                                        var newSlicerViewModel = new NewSlicerViewModel(async instance =>
                                        {
                                            await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                                            Slicers.Add(new Models.Slicer.Slicer()
                                            {
                                                Id = instance.Id,
                                                SlicerName = instance.SlicerName,
                                                Group = instance.SlicerGroup,
                                                InstallationPath = instance.SlicerPath,
                                                DownloadUri = instance.SlicerPath,

                                            });
                                            logger.Info(string.Format(Strings.EventAddedItemFormated, Slicers[Slicers.Count - 1].SlicerName));
                                        }, instance =>
                                        {
                                            _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                                        },
                                        _dialogCoordinator,
                                        link.TargetPath
                                        );

                                        _dialog.Content = new Views.SlicerViews.NewSlicerDialog()
                                        {
                                            DataContext = newSlicerViewModel
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

        public ICommand AddNewSlicerCommand
        {
            get { return new RelayCommand(p => AddNewSlicerAction()); }
        }
        private async void AddNewSlicerAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.AddSlicer };
                var newSlicerViewModel = new NewSlicerViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Slicers.Add(new Models.Slicer.Slicer()
                    {
                        Id = instance.Id,
                        SlicerName = instance.SlicerName,
                        Group = instance.SlicerGroup,
                        InstallationPath = instance.SlicerPath,
                        DownloadUri = instance.SlicerPath,

                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Slicers[Slicers.Count - 1].SlicerName));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                _dialogCoordinator
                );

                _dialog.Content = new Views.SlicerViews.NewSlicerDialog()
                {
                    DataContext = newSlicerViewModel
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

        public ICommand AddNewSlicerChildWindowCommand
        {
            get { return new RelayCommand(p => AddNewSlicerChildWindowAction()); }
        }
        private async void AddNewSlicerChildWindowAction()
        {
            try
            {

                var _childWindow = new ChildWindow()
                {
                    Title = Strings.AddSlicer,
                    AllowMove = true,
                    ShowCloseButton = false,
                    CloseByEscape = false,
                    IsModal = true,
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (Color)Application.Current.Resources["Gray2"] },
                    Padding = new Thickness(50),
                };
                var newSlicerViewModel = new NewSlicerViewModel(async instance =>
                {
                    _childWindow.Close();
                    Slicers.Add(new Models.Slicer.Slicer()
                    {
                        Id = instance.Id,
                        SlicerName = instance.SlicerName,
                        Group = instance.SlicerGroup,
                        InstallationPath = instance.SlicerPath,
                        DownloadUri = instance.SlicerPath,
                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Slicers[Slicers.Count - 1].SlicerName));
                }, instance =>
                {
                    _childWindow.Close();
                },
                    this._dialogCoordinator
                );

                _childWindow.Content = new Views.SlicerViews.NewSlicerDialog()
                {
                    DataContext = newSlicerViewModel
                };
                await ChildWindowManager.ShowChildWindowAsync(Application.Current.MainWindow, _childWindow);
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

        public ICommand DeleteSlicerCommand
        {
            get => new RelayCommand(p => DeleteSlicerAction(p));
        }
        private async void DeleteSlicerAction(object p)
        {
            try
            {
                Models.Slicer.Slicer slicer = p as Models.Slicer.Slicer;
                if (slicer != null)
                {
                    var res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogDeleteSlicerHeadline, 
                        string.Format(Strings.DialogDeleteSlicerFormatedContent, slicer.SlicerName),
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        Slicers.Remove(slicer);
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, slicer.SlicerName));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand DeleteSlicersCommand
        {
            get => new RelayCommand(p => DeleteSlicersAction(p));
        }
        private async void DeleteSlicersAction(object items)
        {
            try
            {
                //var type = items.GetType();
                var slicers = items as IList<Models.Slicer.Slicer>;
                if (slicers != null)
                {
                    var res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogDeleteSlicersHeadline,
                        Strings.DialogDeleteSlicersContent,
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        for (int i = 0; i < slicers.Count; i++)
                        {
                            Models.Slicer.Slicer slicer = slicers[i] as Models.Slicer.Slicer;
                            logger.Info(string.Format(Strings.EventDeletedItemFormated, slicer.SlicerName));
                            Slicers.Remove(slicer);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand DeleteSelectedSlicersCommand
        {
            get => new RelayCommand(p => DeleteSelectedSlicersAction());
        }
        private async void DeleteSelectedSlicersAction()
        {
            try
            {
                var res = await _dialogCoordinator.ShowMessageAsync(this,
                       Strings.DialogDeleteSlicersHeadline, Strings.DialogDeleteSlicersContent,
                       MessageDialogStyle.AffirmativeAndNegative
                       );
                if (res == MessageDialogResult.Affirmative)
                {
                    IList collection = new ArrayList(SelectedSlicersView);
                    for (int i = 0; i < collection.Count; i++)
                    {
                        var obj = collection[i] as SlicerViewInfo;
                        if (obj == null)
                            continue;
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, obj.Name));
                        Slicers.Remove(obj.Slicer);
                    }
                    OnPropertyChanged(nameof(Slicers));
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand EditSelectedSlicerCommand
        {
            get => new RelayCommand(p => EditSelectedSlicerAction());
        }
        private async void EditSelectedSlicerAction()
        {
            try
            {
                var selectedSlicer = SelectedSlicerView.Slicer;
                var _dialog = new CustomDialog() { Title = Strings.EditPrinter };
                var newSlicerViewModel = new NewSlicerViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Slicers.Remove(selectedSlicer);
                    Slicers.Add(new Models.Slicer.Slicer()
                    {
                        Id = instance.Id,
                        SlicerName = instance.SlicerName,
                        Group = instance.SlicerGroup,
                        InstallationPath = instance.SlicerPath,
                        DownloadUri = instance.DownloadUri,
                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Slicers[Slicers.Count - 1].SlicerName));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                _dialogCoordinator,
                selectedSlicer
                );

                _dialog.Content = new Views.SlicerViews.NewSlicerDialog()
                {
                    DataContext = newSlicerViewModel
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

        // Edit Action from Template
        public ICommand EditSlicerCommand
        {
            get => new RelayCommand(p => EditSlicerAction(p));
        }
        private async void EditSlicerAction(object slicer)
        {
            try
            {
                var selectedSlicer = slicer as Models.Slicer.Slicer;
                if (selectedSlicer == null)
                {
                    return;
                }
                var _dialog = new CustomDialog() { Title = Strings.EditSlicer };
                var newSlicerViewModel = new NewSlicerViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Slicers.Remove(selectedSlicer);
                    Slicers.Add(new Models.Slicer.Slicer()
                    {
                        Id = instance.Id,
                        SlicerName = instance.SlicerName,
                        Group = instance.SlicerGroup,
                        InstallationPath = instance.SlicerPath,
                        DownloadUri = instance.DownloadUri,

                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Slicers[Slicers.Count - 1].SlicerName));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                _dialogCoordinator,
                selectedSlicer
                );

                _dialog.Content = new Views.SlicerViews.NewSlicerDialog()
                {
                    DataContext = newSlicerViewModel
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
        
        public ICommand RunSlicerFromTemplateCommand
        {
            get => new RelayCommand(p => RunSlicerFromTemplateAction(p));
        }
        private async void RunSlicerFromTemplateAction(object slicer)
        {
            try
            {
                var selectedSlicer = slicer as Models.Slicer.Slicer;
                if (selectedSlicer == null)
                {
                    return;
                }
                Process.Start(selectedSlicer.InstallationPath);
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
        public ICommand DeleteSlicerFromTemplateCommand
        {
            get => new RelayCommand(p => DeleteSlicerFromTemplateAction(p));
        }
        private async void DeleteSlicerFromTemplateAction(object obj)
        {
            try
            {
                var res = await _dialogCoordinator.ShowMessageAsync(this,
                       Strings.DialogDeleteSlicersHeadline, Strings.DialogDeleteSlicersContent,
                       MessageDialogStyle.AffirmativeAndNegative
                       );
                if (res == MessageDialogResult.Affirmative)
                {
                    Models.Slicer.Slicer slicer = obj as Models.Slicer.Slicer;
                    if(slicer != null)
                    {
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, slicer.SlicerName));
                        Slicers.Remove(slicer);
                        OnPropertyChanged(nameof(Slicers));
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

        #endregion

        #region SlicerCommands
        public ICommand AddNewSlicerCommandCommand
        {
            get { return new RelayCommand(p => AddNewSlicerCommandAction()); }
        }
        private async void AddNewSlicerCommandAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.AddSlicerCommand };
                var newViewModel = new NewSlicerCommandDialogViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    SlicerCommands.Add(new Models.Slicer.SlicerCommand()
                    {
                        Name = instance.Name,
                        Slicer = instance.SlicerName,
                        Command = instance.SlicerCommand,
                        AutoAddFilePath = instance.AutoAddFilePath,
                        OutputFilePatternString = instance.OutputFileFormat,
                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Slicers[Slicers.Count - 1].SlicerName));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                _dialogCoordinator
                );

                _dialog.Content = new Views.SlicerViews.NewSlicerCommandDialogView()
                {
                    DataContext = newViewModel
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

        public ICommand AddNewSlicerCommandChildWindowCommand
        {
            get { return new RelayCommand(p => AddNewSlicerCommandChildWindowAction()); }
        }
        private async void AddNewSlicerCommandChildWindowAction()
        {
            try
            {
                var _childWindow = new ChildWindow()
                {
                    Title = Strings.AddSlicerCommand,
                    AllowMove = true,
                    ShowCloseButton = false,
                    CloseByEscape = false,
                    IsModal = true,
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (Color)Application.Current.Resources["Gray2"] },
                    Padding = new Thickness(50),
                };
                var newViewModel = new NewSlicerCommandDialogViewModel(async instance =>
                {
                    _childWindow.Close();
                    SlicerCommands.Add(new Models.Slicer.SlicerCommand()
                    {
                        Name = instance.Name,
                        Slicer = instance.SlicerName,
                        Command = instance.SlicerCommand,
                        AutoAddFilePath = instance.AutoAddFilePath,
                        OutputFilePatternString = instance.OutputFileFormat,
                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Slicers[Slicers.Count - 1].SlicerName));
                }, instance =>
                {
                    _childWindow.Close();
                },
                    this._dialogCoordinator
                );

                _childWindow.Content = new Views.SlicerViews.NewSlicerCommandDialogView()
                {
                    DataContext = newViewModel
                };
                await ChildWindowManager.ShowChildWindowAsync(Application.Current.MainWindow, _childWindow);
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

        public ICommand DeleteSlicerCommandCommand
        {
            get => new RelayCommand(p => DeleteSlicerCommandAction(p));
        }
        private async void DeleteSlicerCommandAction(object p)
        {
            try
            {
                SlicerCommand scmd = p as SlicerCommand;
                if (scmd != null)
                {
                    var res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogDeleteSlicerHeadline,
                        string.Format(Strings.DialogDeleteSlicerFormatedContent, scmd.Name),
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        SlicerCommands.Remove(scmd);
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, scmd.Name));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand DeleteSlicerCommandsCommand
        {
            get => new RelayCommand(p => DeleteSlicerCommandsAction(p));
        }
        private async void DeleteSlicerCommandsAction(object items)
        {
            try
            {
                //var type = items.GetType();
                var scmds = items as IList<SlicerCommand>;
                if (scmds != null)
                {
                    var res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogDeleteSlicersHeadline,
                        Strings.DialogDeleteSlicersContent,
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        for (int i = 0; i < scmds.Count; i++)
                        {
                            SlicerCommand scmd = scmds[i] as SlicerCommand;
                            logger.Info(string.Format(Strings.EventDeletedItemFormated, scmd.Name));
                            SlicerCommands.Remove(scmd);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand DeleteSelectedSlicerCommandsCommand
        {
            get => new RelayCommand(p => DeleteSelectedSlicerCommandsAction());
        }
        private async void DeleteSelectedSlicerCommandsAction()
        {
            try
            {
                var res = await _dialogCoordinator.ShowMessageAsync(this,
                       Strings.DialogDeleteSlicersHeadline, Strings.DialogDeleteSlicersContent,
                       MessageDialogStyle.AffirmativeAndNegative
                       );
                if (res == MessageDialogResult.Affirmative)
                {
                    IList collection = new ArrayList(SelectedSlicerCommandsView);
                    for (int i = 0; i < collection.Count; i++)
                    {
                        var obj = collection[i] as SlicerCommand;
                        if (obj == null)
                            continue;
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, obj.Name));
                        SlicerCommands.Remove(obj);
                    }
                    OnPropertyChanged(nameof(Slicers));
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand EditSelectedSlicerCommandCommand
        {
            get => new RelayCommand(p => EditSelectedSlicerCommandAction());
        }
        private async void EditSelectedSlicerCommandAction()
        {
            try
            {
                var selectedSlicerCommand = SelectedSlicerCommandView;
                var _dialog = new CustomDialog() { Title = Strings.EditSlicerCommand };
                var newViewModel = new NewSlicerCommandDialogViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    SlicerCommands.Remove(selectedSlicerCommand);
                    SlicerCommands.Add(new SlicerCommand()
                    {
                        Name = instance.Name,
                        Slicer = instance.SlicerName,
                        Command = instance.SlicerCommand,
                        AutoAddFilePath = instance.AutoAddFilePath,
                        OutputFilePatternString = instance.OutputFileFormat,
                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, SlicerCommands[SlicerCommands.Count - 1].Name));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                _dialogCoordinator,
                selectedSlicerCommand
                );

                _dialog.Content = new Views.SlicerViews.NewSlicerCommandDialogView()
                {
                    DataContext = newViewModel
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

        // Edit Action from Template
        public ICommand EditSlicerCommandCommand
        {
            get => new RelayCommand(p => EditSlicerCommandAction(p));
        }
        private async void EditSlicerCommandAction(object slicerCommand)
        {
            try
            {
                var selectedSlicerCommand = slicerCommand as SlicerCommand;
                if (selectedSlicerCommand == null)
                {
                    return;
                }
                var _dialog = new CustomDialog() { Title = Strings.EditSlicerCommand };
                var newViewModel = new NewSlicerCommandDialogViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    SlicerCommands.Remove(selectedSlicerCommand);
                    SlicerCommands.Add(new SlicerCommand()
                    {
                        Name = instance.Name,
                        Slicer = instance.SlicerName,
                        Command = instance.SlicerCommand,
                        AutoAddFilePath = instance.AutoAddFilePath,
                        OutputFilePatternString = instance.OutputFileFormat,

                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, SlicerCommands[SlicerCommands.Count - 1].Name));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                _dialogCoordinator,
                selectedSlicerCommand
                );

                _dialog.Content = new Views.SlicerViews.NewSlicerCommandDialogView()
                {
                    DataContext = newViewModel
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

        public ICommand DeleteSlicerCommandFromTemplateCommand
        {
            get => new RelayCommand(p => DeleteSlicerCommandFromTemplateAction(p));
        }
        private async void DeleteSlicerCommandFromTemplateAction(object obj)
        {
            try
            {
                var res = await _dialogCoordinator.ShowMessageAsync(this,
                       Strings.DialogDeleteSlicersHeadline, Strings.DialogDeleteSlicersContent,
                       MessageDialogStyle.AffirmativeAndNegative
                       );
                if (res == MessageDialogResult.Affirmative)
                {
                    SlicerCommand scmd = obj as SlicerCommand;
                    if (scmd != null)
                    {
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, scmd.Name));
                        SlicerCommands.Remove(scmd);
                        OnPropertyChanged(nameof(SlicerCommands));
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
        #endregion

        #endregion

        #region Methods
        private void createSlicerViewInfos()
        {
            Canvas c = new Canvas();
            c.Children.Add(new PackIconMaterial { Kind = PackIconMaterialKind.Printer3d });
            SlicerViews = new CollectionViewSource
            {
                Source = (Slicers.Select(p => new SlicerViewInfo()
                {
                    Name = p.SlicerName.ToString(),
                    Slicer = p,
                    Icon = c,
                    Group = p.Group,
                })).ToList()
            }.View;
            SlicerViews.SortDescriptions.Add(new SortDescription(nameof(SlicerViewManager.Group), ListSortDirection.Ascending));
            SlicerViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(SlicerViewManager.Group)));
        }
        
        private void createSlicerCommandViewInfos()
        {
            SlicerCommandViews = new CollectionViewSource
            {
                Source = (SlicerCommands.Select(p => p)).ToList()
            }.View;
            SlicerCommandViews.SortDescriptions.Add(new SortDescription(nameof(SlicerCommand.Name), ListSortDirection.Ascending));
            SlicerCommandViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(SlicerCommand.Slicer)));
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
