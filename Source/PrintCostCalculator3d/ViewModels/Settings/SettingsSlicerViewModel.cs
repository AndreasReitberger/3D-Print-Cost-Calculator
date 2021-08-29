using IWshRuntimeLibrary;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using MahApps.Metro.SimpleChildWindow;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Models.Slicer;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using PrintCostCalculator3d.ViewModels.Slicer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace PrintCostCalculator3d.ViewModels
{
    class SettingsSlicerViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Properties

        #region Slicers
        ObservableCollection<Models.Slicer.Slicer> _slicers = new ObservableCollection<Models.Slicer.Slicer>();
        public ObservableCollection<Models.Slicer.Slicer> Slicers
        {
            get => _slicers;
            set
            {
                if (_slicers == value) return;

                if (!IsLoading)
                    SettingsManager.Current.Slicers = value;

                _slicers = value;
                OnPropertyChanged();
                
            }
        }

        ICollectionView _SlicerViews;
        public ICollectionView SlicerViews
        {
            get => _SlicerViews;
            set
            {
                if (_SlicerViews != value)
                {
                    _SlicerViews = value;
                    OnPropertyChanged();
                }
            }
        }
        

        SlicerViewInfo _selectedSlicerView;
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

        IList _selectedSlicersView = new ArrayList();
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
        ObservableCollection<SlicerCommand> _slicerCommands;
        public ObservableCollection<SlicerCommand> SlicerCommands
        {
            get => _slicerCommands;
            set
            {
                if (_slicerCommands == value) return;

                if (!IsLoading)
                    SettingsManager.Current.SlicerCommands = value;

                _slicerCommands = value;
                OnPropertyChanged();
                
            }
        }

        ICollectionView _SlicerCommandViews;
        public ICollectionView SlicerCommandViews
        {
            get => _SlicerCommandViews;
            set
            {
                if (_SlicerCommandViews == value) return;
                
                _SlicerCommandViews = value;
                OnPropertyChanged();
                
            }
        }


        SlicerCommand _selectedSlicerCommandView;
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

        IList _selectedSlicerCommandsView = new ArrayList();
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
        string _searchSlicer = string.Empty;
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

        string _searchSlicerCommand = string.Empty;
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
            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            _dialogCoordinator = instance;
            Slicers.CollectionChanged += Slicers_CollectionChanged;
            SlicerCommands.CollectionChanged += SlicerCommands_CollectionChanged; 
            CreateSlicerViewInfos();
            CreateSlicerCommandViewInfos();
        }

        void SlicerCommands_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CreateSlicerCommandViewInfos();
            SettingsManager.Save();
        }

        void LoadSettings()
        {
            SlicerCommands = SettingsManager.Current.SlicerCommands;
            Slicers = SettingsManager.Current.Slicers;
        }

        void Slicers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CreateSlicerViewInfos();
            SettingsManager.Save();
        }
        #endregion

        #region ICommand & Actions

        #region Slicer
        public ICommand OnDropLnkFileCommand
        {
            get => new RelayCommand(async(p) => await OnDropLnkFileAction(p));
        }
        async Task OnDropLnkFileAction(object p)
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
                                    _ = await _dialogCoordinator.ShowMessageAsync(this,
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

                                        CustomDialog _dialog = new() { Title = Strings.AddSlicer };
                                        NewSlicerViewModel newSlicerViewModel = new(async instance =>
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
            get { return new RelayCommand(async(p) => await AddNewSlicerAction()); }
        }
        async Task AddNewSlicerAction()
        {
            try
            {
                CustomDialog _dialog = new() { Title = Strings.AddSlicer };
                NewSlicerViewModel newSlicerViewModel = new(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Slicers.Add(new Models.Slicer.Slicer()
                    {
                        Id = instance.Id,
                        SlicerName = instance.SlicerName,
                        Group = instance.SlicerGroup,
                        InstallationPath = instance.SlicerPath,
                        DownloadUri = instance.SlicerPath
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
            }
        }

        public ICommand AddNewSlicerChildWindowCommand
        {
            get { return new RelayCommand(async(p) => await AddNewSlicerChildWindowAction()); }
        }
        async Task AddNewSlicerChildWindowAction()
        {
            try
            {
                ChildWindow _childWindow = new()
                {
                    Title = Strings.AddSlicer,
                    AllowMove = true,
                    ShowCloseButton = false,
                    CloseByEscape = false,
                    IsModal = true,
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (Color)Application.Current.Resources["Gray2"] },
                    Padding = new Thickness(50),
                };
                NewSlicerViewModel newSlicerViewModel = new(instance =>
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
                    _dialogCoordinator
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
            }
        }

        public ICommand DeleteSlicerCommand
        {
            get => new RelayCommand(async(p) => await DeleteSlicerAction(p));
        }
        async Task DeleteSlicerAction(object p)
        {
            try
            {
                if (p is Models.Slicer.Slicer slicer)
                {
                    MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
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
            get => new RelayCommand(async(p) => await DeleteSlicersAction(p));
        }
        async Task DeleteSlicersAction(object items)
        {
            try
            {
                //var type = items.GetType();
                if (items is IList<Models.Slicer.Slicer> slicers)
                {
                    MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogDeleteSlicersHeadline,
                        Strings.DialogDeleteSlicersContent,
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        for (int i = 0; i < slicers.Count; i++)
                        {
                            Models.Slicer.Slicer slicer = slicers[i];
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
            get => new RelayCommand(async(p) => await DeleteSelectedSlicersAction());
        }
        async Task DeleteSelectedSlicersAction()
        {
            try
            {
                MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                       Strings.DialogDeleteSlicersHeadline, Strings.DialogDeleteSlicersContent,
                       MessageDialogStyle.AffirmativeAndNegative
                       );
                if (res == MessageDialogResult.Affirmative)
                {
                    IList collection = new ArrayList(SelectedSlicersView);
                    for (int i = 0; i < collection.Count; i++)
                    {
                        if (collection[i] is not SlicerViewInfo obj)
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
            get => new RelayCommand(async(p) => await EditSelectedSlicerAction());
        }
        async Task EditSelectedSlicerAction()
        {
            try
            {
                Models.Slicer.Slicer selectedSlicer = SelectedSlicerView.Slicer;
                CustomDialog _dialog = new() { Title = Strings.EditPrinter };
                NewSlicerViewModel newSlicerViewModel = new(async instance =>
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
            }
        }

        // Edit Action from Template
        public ICommand EditSlicerCommand
        {
            get => new RelayCommand(async(p) => await EditSlicerAction(p));
        }
        async Task EditSlicerAction(object slicer)
        {
            try
            {
                if (slicer is not Models.Slicer.Slicer selectedSlicer)
                {
                    return;
                }
                CustomDialog _dialog = new() { Title = Strings.EditSlicer };
                NewSlicerViewModel newSlicerViewModel = new(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Slicers.Remove(selectedSlicer);
                    Slicers.Add(new Models.Slicer.Slicer()
                    {
                        Id = instance.Id,
                        SlicerName = instance.SlicerName,
                        Group = instance.SlicerGroup,
                        InstallationPath = instance.SlicerPath,
                        DownloadUri = instance.DownloadUri
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
            }
        }
        
        public ICommand RunSlicerFromTemplateCommand
        {
            get => new RelayCommand((p) => RunSlicerFromTemplateAction(p));
        }
        void RunSlicerFromTemplateAction(object slicer)
        {
            try
            {
                if (slicer is not Models.Slicer.Slicer selectedSlicer)
                {
                    return;
                }
                Process.Start(selectedSlicer.InstallationPath);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        public ICommand DeleteSlicerFromTemplateCommand
        {
            get => new RelayCommand(async(p) => await DeleteSlicerFromTemplateAction(p));
        }
        async Task DeleteSlicerFromTemplateAction(object obj)
        {
            try
            {
                MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                       Strings.DialogDeleteSlicersHeadline, Strings.DialogDeleteSlicersContent,
                       MessageDialogStyle.AffirmativeAndNegative
                       );
                if (res == MessageDialogResult.Affirmative)
                {
                    if (obj is Models.Slicer.Slicer slicer)
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
            }
        }

        #endregion

        #region SlicerCommands
        public ICommand AddNewSlicerCommandCommand
        {
            get { return new RelayCommand(async(p) => await AddNewSlicerCommandAction()); }
        }
        async Task AddNewSlicerCommandAction()
        {
            try
            {
                CustomDialog _dialog = new() { Title = Strings.AddSlicerCommand };
                NewSlicerCommandDialogViewModel newViewModel = new(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    SlicerCommands.Add(new SlicerCommand()
                    {
                        Name = instance.Name,
                        Slicer = instance.SlicerName,
                        Command = instance.SlicerCommand,
                        AutoAddFilePath = instance.AutoAddFilePath,
                        OutputFilePatternString = instance.OutputFileFormat
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
            }
        }

        public ICommand AddNewSlicerCommandChildWindowCommand
        {
            get { return new RelayCommand(async(p) => await AddNewSlicerCommandChildWindowAction()); }
        }
        async Task AddNewSlicerCommandChildWindowAction()
        {
            try
            {
                ChildWindow _childWindow = new()
                {
                    Title = Strings.AddSlicerCommand,
                    AllowMove = true,
                    ShowCloseButton = false,
                    CloseByEscape = false,
                    IsModal = true,
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (Color)Application.Current.Resources["Gray2"] },
                    Padding = new Thickness(50),
                };
                NewSlicerCommandDialogViewModel newViewModel = new(instance =>
                {
                    _childWindow.Close();
                    SlicerCommands.Add(new SlicerCommand()
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
                    _dialogCoordinator
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
                _ = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogExceptionHeadline,
                        string.Format(Strings.DialogExceptionFormatedContent, exc.Message)
                        );
            }
        }

        public ICommand DeleteSlicerCommandCommand
        {
            get => new RelayCommand(async(p) => await DeleteSlicerCommandAction(p));
        }
        async Task DeleteSlicerCommandAction(object p)
        {
            try
            {
                if (p is SlicerCommand scmd)
                {
                    MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
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
            get => new RelayCommand(async(p) => await DeleteSlicerCommandsAction(p));
        }
        async Task DeleteSlicerCommandsAction(object items)
        {
            try
            {
                //var type = items.GetType();
                if (items is IList<SlicerCommand> scmds)
                {
                    MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogDeleteSlicersHeadline,
                        Strings.DialogDeleteSlicersContent,
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        for (int i = 0; i < scmds.Count; i++)
                        {
                            SlicerCommand scmd = scmds[i];
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
            get => new RelayCommand(async(p) => await DeleteSelectedSlicerCommandsAction());
        }
        async Task DeleteSelectedSlicerCommandsAction()
        {
            try
            {
                MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                       Strings.DialogDeleteSlicersHeadline, Strings.DialogDeleteSlicersContent,
                       MessageDialogStyle.AffirmativeAndNegative
                       );
                if (res == MessageDialogResult.Affirmative)
                {
                    IList collection = new ArrayList(SelectedSlicerCommandsView);
                    for (int i = 0; i < collection.Count; i++)
                    {
                        if (collection[i] is not SlicerCommand obj)
                        {
                            continue;
                        }
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
            get => new RelayCommand(async(p) => await EditSelectedSlicerCommandAction());
        }
        async Task EditSelectedSlicerCommandAction()
        {
            try
            {
                SlicerCommand selectedSlicerCommand = SelectedSlicerCommandView;
                CustomDialog _dialog = new() { Title = Strings.EditSlicerCommand };
                NewSlicerCommandDialogViewModel newViewModel = new(async instance =>
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
            }
        }

        // Edit Action from Template
        public ICommand EditSlicerCommandCommand
        {
            get => new RelayCommand(async(p) => await EditSlicerCommandAction(p));
        }
        async Task EditSlicerCommandAction(object slicerCommand)
        {
            try
            {
                if (slicerCommand is not SlicerCommand selectedSlicerCommand)
                {
                    return;
                }
                CustomDialog _dialog = new() { Title = Strings.EditSlicerCommand };
                NewSlicerCommandDialogViewModel newViewModel = new(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    SlicerCommands.Remove(selectedSlicerCommand);
                    SlicerCommands.Add(new SlicerCommand()
                    {
                        Name = instance.Name,
                        Slicer = instance.SlicerName,
                        Command = instance.SlicerCommand,
                        AutoAddFilePath = instance.AutoAddFilePath,
                        OutputFilePatternString = instance.OutputFileFormat
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
            }
        }

        public ICommand DeleteSlicerCommandFromTemplateCommand
        {
            get => new RelayCommand(async(p) => await DeleteSlicerCommandFromTemplateAction(p));
        }
        async Task DeleteSlicerCommandFromTemplateAction(object obj)
        {
            try
            {
                MessageDialogResult res = await _dialogCoordinator.ShowMessageAsync(this,
                       Strings.DialogDeleteSlicersHeadline, Strings.DialogDeleteSlicersContent,
                       MessageDialogStyle.AffirmativeAndNegative
                       );
                if (res == MessageDialogResult.Affirmative)
                {
                    if (obj is SlicerCommand scmd)
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
            }
        }
        #endregion

        #endregion

        #region Methods
        void CreateSlicerViewInfos()
        {
            Application.Current.Dispatcher.Invoke(() =>
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
            });
        }
        
        void CreateSlicerCommandViewInfos()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SlicerCommandViews = new CollectionViewSource
                {
                    Source = (SlicerCommands.Select(p => p)).ToList()
                }.View;
                SlicerCommandViews.SortDescriptions.Add(new SortDescription(nameof(SlicerCommand.Name), ListSortDirection.Ascending));
                SlicerCommandViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(SlicerCommand.Slicer)));
            });
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
