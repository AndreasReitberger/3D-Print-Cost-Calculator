using WpfFramework.Utilities;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using WpfFramework.Models.Slicer;
using WpfFramework.Models.Settings;
using System.ComponentModel;
using System.Collections;
using System.Windows.Data;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;
using System.Windows.Input;
using WpfFramework.Resources.Localization;
using MahApps.Metro.SimpleChildWindow;
using System.Windows;
using System.Windows.Media;
using WpfFramework.ViewModels.Slicer;
using log4net;
using System.Diagnostics;
using System.IO;
using IWshRuntimeLibrary;

namespace WpfFramework.ViewModels
{
    class SettingsSlicerViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        public ObservableCollection<Models.Slicer.Slicer> Slicers
        {
            get => SettingsManager.Current.Slicers;
            set
            {
                if (SettingsManager.Current.Slicers != value)
                {
                    SettingsManager.Current.Slicers = value;
                    OnPropertyChanged();
                }
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
                    OnPropertyChanged(nameof(_searchSlicer));
                }
            }
        }
        #endregion

        #region Constructor
        public SettingsSlicerViewModel(IDialogCoordinator instance)
        {
            _dialogCoordinator = instance;
            Slicers.CollectionChanged += Slicers_CollectionChanged;
            createSlicerViewInfos();
        }

        private void Slicers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            createSlicerViewInfos();
            SettingsManager.Save();
        }
        #endregion

        #region ICommand & Actions
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

        public ICommand EditSelectedSelectedCommand
        {
            get => new RelayCommand(p => EditSelectedSelectedAction());
        }
        private async void EditSelectedSelectedAction()
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


        public void OnViewVisible()
        {

        }

        public void OnViewHide()
        {

        }
        #endregion
    }
}
