using AndreasReitberger.Enums;
using AndreasReitberger.Models;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using MahApps.Metro.SimpleChildWindow;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
//Additional
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace PrintCostCalculator3d.ViewModels._3dPrinting
{
    public class _3dPrintingPrinterViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Properties

        ObservableCollection<Printer3d> _printers = new ObservableCollection<Printer3d>();
        public ObservableCollection<Printer3d> Printers
        {
            get => _printers;
            set
            {
                if (_printers == value) return;
                if(!IsLoading)
                    SettingsManager.Current.Printers = value;
                _printers = value;
                OnPropertyChanged();
                
            }

        }

        public ICollectionView PrinterViews
        {
            get => _PrinterViews;
            set
            {
                if (_PrinterViews != value)
                {
                    _PrinterViews = value;
                    OnPropertyChanged(nameof(PrinterViews));
                }
            }
        }
        ICollectionView _PrinterViews;

        PrinterViewInfo _selectedPrinterView;
        public PrinterViewInfo SelectedPrinterView
        {
            get => _selectedPrinterView;
            set
            {
                if (_selectedPrinterView != value)
                {
                    _selectedPrinterView = value;
                    OnPropertyChanged(nameof(SelectedPrinterView));
                }
            }
        }

        IList _selectedPrintersView = new ArrayList();
        public IList SelectedPrintersView
        {
            get => _selectedPrintersView;
            set
            {
                if (Equals(value, _selectedPrintersView))
                    return;

                _selectedPrintersView = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Search
        string _searchPrinter = string.Empty;
        public string SearchPrinter
        {
            get => _searchPrinter;
            set
            {
                if (_searchPrinter != value)
                {
                    _searchPrinter = value;

                    PrinterViews.Refresh();

                    ICollectionView view = CollectionViewSource.GetDefaultView(PrinterViews);
                    IEqualityComparer<String> comparer = StringComparer.InvariantCultureIgnoreCase;
                    view.Filter = o =>
                    {
                        PrinterViewInfo p = o as PrinterViewInfo;
                        string[] patterns = _searchPrinter.ToLower().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (patterns.Length == 1 || patterns.Length == 0)
                            return p.Name.ToLower().Contains(_searchPrinter.ToLower());
                        else
                        {
                            return patterns.Any(p.Name.ToLower().Contains) || patterns.Any(p.Printer.Manufacturer.Name.ToLower().Contains);
                        }
                    };
                    OnPropertyChanged(nameof(SearchPrinter));
                }
            }
        }
        #endregion

        #region Constructor, LoadSettings
        public _3dPrintingPrinterViewModel(IDialogCoordinator dialog)
        {
            _dialogCoordinator = dialog;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;

            IsLicenseValid = false;

            Printers.CollectionChanged += Printers_CollectionChanged;

            CreatePrinterViewInfos();
            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }

        void LoadSettings()
        {
            Printers = SettingsManager.Current.Printers;
        }

        void Printers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CreatePrinterViewInfos();
            SettingsManager.Save();
        }
        #endregion

        #region ICommand & Actions
        public ICommand AddNewPrinterCommand
        {
            get { return new RelayCommand(async(p) => await AddNewPrinterAction()); }
        }
        async Task AddNewPrinterAction()
        {
            try
            {
                /*
                FirebaseHandler fbh = new FirebaseHandler("8R9Eac6MeET453bPR7UkPLxQbZNcIlaRDEGnYDFC");
                var res = await fbh.GetDefaultPrinters();
                await fbh.AddDefaultPrinter(InstanceConverter.GetPrinterFromInstance(instance));
                */
                var _dialog = new CustomDialog() { Title = Strings.NewPrinter };
                var newPrinterViewModel = new New3DPrinterViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Printers.Add(InstanceConverter.GetPrinterFromInstance(instance));
                    
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Printers[Printers.Count - 1].Name));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                _dialogCoordinator
                );

                _dialog.Content = new Views._3dPrinting.NewPrinterDialog()
                {
                    DataContext = newPrinterViewModel
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

        public ICommand AddNewPrinterChildWindowCommand
        {
            get { return new RelayCommand(async (p) => await AddNewPrinterChildWindowAction()); }
        }
        async Task AddNewPrinterChildWindowAction()
        {
            try
            {

                ChildWindow _childWindow = new ChildWindow()
                {
                    Title = Strings.NewPrinter,
                    AllowMove = true,
                    ShowCloseButton = false,
                    CloseByEscape = false,
                    IsModal = true,
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (Color)Application.Current.Resources["MahApps.Colors.Gray2"] },
                    Padding = new Thickness(50),
                };
                New3DPrinterViewModel newPrinterViewModel = new New3DPrinterViewModel(instance =>
                {
                    _childWindow.Close();
                    Printers.Add(InstanceConverter.GetPrinterFromInstance(instance));
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Printers[Printers.Count - 1].Name));
                }, instance =>
                {
                    _childWindow.Close();
                },
                    _dialogCoordinator
                );

                _childWindow.Content = new Views._3dPrinting.NewPrinterDialog()
                {
                    DataContext = newPrinterViewModel
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

        public ICommand DeletePrinterCommand
        {
            get => new RelayCommand(async (p) => await DeletePrinterAction(p));
        }
        async Task DeletePrinterAction(object p)
        {
            try
            {
                Printer3d printer = p as Printer3d;
                if (printer != null)
                {
                    var res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogDeletePrinterHeadline, Strings.DialogDeletePrinterContent,
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        Printers.Remove(printer);
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, printer.Name));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        
        public ICommand DeletePrintersCommand
        {
            get => new RelayCommand(async (p) => await DeletePrintersAction(p));
        }
        async Task DeletePrintersAction(object items)
        {
            try
            {
                //var type = items.GetType();
                var printers = items as IList<Printer3d>;
                if (printers != null)
                {
                    var res = await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogDeletePrinterHeadline, Strings.DialogDeletePrinterContent,
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        //foreach (_3dPrinterModel printer in printers)
                        for (int i = 0; i < printers.Count; i++)
                        {
                            Printer3d printer = printers[i] as Printer3d;
                            logger.Info(string.Format(Strings.EventDeletedItemFormated, printer.Name));
                            Printers.Remove(printer);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand DeleteSelectedPrintersCommand
        {
            get => new RelayCommand(async (p) => await DeleteSelectedPrintersAction());
        }
        async Task DeleteSelectedPrintersAction()
        {
            try
            {
                var res = await _dialogCoordinator.ShowMessageAsync(this,
                       Strings.DialogDeleteSelectedPrintersHeadline, Strings.DialogDeleteSelectedPrintersContent,
                       MessageDialogStyle.AffirmativeAndNegative
                       );
                if (res == MessageDialogResult.Affirmative)
                {
                    IList collection = new ArrayList(SelectedPrintersView);
                    for (int i = 0; i < collection.Count; i++)
                    {
                        var obj = collection[i] as PrinterViewInfo;
                        if (obj == null)
                            continue;
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, obj.Name));
                        Printers.Remove(obj.Printer);
                    }
                    OnPropertyChanged(nameof(Printers));
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        public ICommand EditSelectedPrinterCommand
        {
            get => new RelayCommand(async (p) => await EditSelectedPrinterAction());
        }
        async Task EditSelectedPrinterAction()
        {
            try
            {
                var selectedPrinter = SelectedPrinterView.Printer;
                var _dialog = new CustomDialog() { Title = Strings.EditPrinter };
                var newPrinterViewModel = new New3DPrinterViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    UpdatePrinterFromInstance(instance, selectedPrinter);
                    //Printers.Remove(selectedPrinter);
                    //Printers.Add(InstanceConverter.GetPrinterFromInstance(instance));
                    logger.Info(string.Format(Strings.EventAddedItemFormated, selectedPrinter,Printers[Printers.Count - 1].Name));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                _dialogCoordinator,
                selectedPrinter
                );

                _dialog.Content = new Views._3dPrinting.NewPrinterDialog()
                {
                    DataContext = newPrinterViewModel
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
        public ICommand EditPrinterCommand
        {
            get => new RelayCommand(async (p) => await EditPrinterAction(p));
        }
        async Task EditPrinterAction(object printer)
        {
            try
            {
                var selectedPrinter = printer as Printer3d;
                if (selectedPrinter == null)
                {
                    return;
                }
                var _dialog = new CustomDialog() { Title = Strings.EditPrinter };
                var newPrinterViewModel = new New3DPrinterViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    UpdatePrinterFromInstance(instance, selectedPrinter);
                    //Printers.Remove(selectedPrinter);
                    //Printers.Add(InstanceConverter.GetPrinterFromInstance(instance));
                    logger.Info(string.Format(Strings.EventAddedItemFormated, selectedPrinter, Printers[Printers.Count - 1].Name));
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                _dialogCoordinator,
                selectedPrinter
                );

                _dialog.Content = new Views._3dPrinting.NewPrinterDialog()
                {
                    DataContext = newPrinterViewModel
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

        public ICommand ReorderPrinterCommand
        {
            get => new RelayCommand(async (p) => await ReorderPrinterAction(p));
        }
        async Task ReorderPrinterAction(object parameter)
        {
            try
            {
                string uri = parameter as string;
                if (!string.IsNullOrEmpty(uri))
                {
                    var res = await this._dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogOpenExternalUriHeadline,
                        string.Format(Strings.DialogOpenExternalUriFormatedContent, uri),
                        MessageDialogStyle.AffirmativeAndNegative
                        );
                    if (res == MessageDialogResult.Affirmative)
                    {
                        Process.Start(uri);
                        logger.Info(string.Format(Strings.EventOpenUri, uri));
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                
            }
        }

        public ICommand DuplicatePrinterCommand
        {
            get => new RelayCommand((p) => DuplicatePrinterAction(p));
        }
        void DuplicatePrinterAction(object p)
        {
            try
            {
                if (p is Printer3d printer)
                {
                    IEnumerable<Printer3d> duplicates = Printers.Where(prt => prt.Model.StartsWith(prt.Model.Split('_')[0]));

                    Printer3d newPrinter = (Printer3d)printer.Clone();
                    newPrinter.Id = Guid.NewGuid();
                    newPrinter.Model = string.Format("{0}_{1}", newPrinter.Model.Split('_')[0], duplicates.Count());
                    Printers.Add(newPrinter);
                    PrinterViews.Refresh();

                    logger.Info(string.Format(Strings.EventAddedItemFormated, newPrinter.Name));
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        #endregion

        #region Methods
        void UpdatePrinterFromInstance(New3DPrinterViewModel instance, Printer3d printer)
        {
            try
            {
                InstanceConverter.GetPrinterFromInstance(instance, printer);

                SettingsManager.Current.Printers = Printers;
                SettingsManager.Save();

                OnPropertyChanged(nameof(Printers));
                CreatePrinterViewInfos();
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.DialogExceptionFormatedContent, exc.Message, exc.TargetSite);
            }
        }
        void CreatePrinterViewInfos()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Canvas c = new Canvas();
                c.Children.Add(new PackIconMaterial { Kind = PackIconMaterialKind.Printer3d });
                PrinterViews = new CollectionViewSource
                {
                    Source = (Printers.Select(p => new PrinterViewInfo()
                    {
                        Name = p.Name,
                        Printer = p,
                        Icon = c,
                        Group = (Printer3dType)Enum.Parse(typeof(Printer3dType), p.Type.ToString()),
                    })).ToList()
                }.View;
                PrinterViews.SortDescriptions.Add(new SortDescription(nameof(PrinterViewInfo.Group), ListSortDirection.Ascending));
                PrinterViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(PrinterViewInfo.Group)));
            });
        }
        public void OnViewVisible()
        {
            Printers.CollectionChanged += Printers_CollectionChanged;
            CreatePrinterViewInfos();
            OnPropertyChanged(nameof(IsLicenseValid));
        }

        public void OnViewHide()
        {

        }
        #endregion
    }
}
