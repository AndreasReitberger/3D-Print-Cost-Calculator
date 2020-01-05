using System;
using System.Collections.Generic;
using System.Linq;

//Additional
using WpfFramework.Utilities;
using WpfFramework.Models._3dprinting;
using WpfFramework.Models.Settings;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;
using System.Windows.Data;
using System.Collections;
using WpfFramework.Resources.Localization;
using log4net;
using MahApps.Metro.SimpleChildWindow;
using System.Windows.Media;
using System.Windows;

namespace WpfFramework.ViewModels._3dPrinting 
{
    public class _3dPrintingPrinterViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        public ObservableCollection<_3dPrinterModel> Printers
        {
            get => SettingsManager.Current._3dPrinters;
            set
            {
                if (value != SettingsManager.Current._3dPrinters)
                {
                    SettingsManager.Current._3dPrinters = value;
                    OnPropertyChanged(nameof(Printers));
                }
            }

        }

        // Maye delete
        private PrinterViewInfo _printerView;
        public PrinterViewInfo PrinterView
        {
            get => _printerView;
            private set
            {
                if (_printerView != value)
                {
                    _printerView = value;
                    OnPropertyChanged(nameof(PrinterView));
                }
            }
        }

        public ICollectionView PrinterViews
        {
            get => _PrinterViews;
            private set
            {
                if (_PrinterViews != value)
                {
                    _PrinterViews = value;
                    OnPropertyChanged(nameof(PrinterViews));
                }
            }
        }
        private ICollectionView _PrinterViews;

        private PrinterViewInfo _selectedPrinterView;
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

        private IList _selectedPrintersView = new ArrayList();
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
        private string _searchPrinter = string.Empty;
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
                        return p.Name.Contains(_searchPrinter);
                    };
                    OnPropertyChanged(nameof(SearchPrinter));
                }
            }
        }
        #endregion

        #region Methods
        private void createPrinterViewInfos()
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
                    Group = (PrinterViewManager.Group)Enum.Parse(typeof(PrinterViewManager.Group), p.Type.ToString()),
                })).ToList()
            }.View;
            PrinterViews.SortDescriptions.Add(new SortDescription(nameof(PrinterViewInfo.Group), ListSortDirection.Ascending));
            PrinterViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(PrinterViewInfo.Group)));
        }
        #endregion

        #region Constructor
        public _3dPrintingPrinterViewModel(IDialogCoordinator dialog)
        {
            this._dialogCoordinator = dialog;
            Printers.CollectionChanged += Printers_CollectionChanged;

            createPrinterViewInfos();
            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }

        private void Printers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            createPrinterViewInfos();
            SettingsManager.Save();
        }
        #endregion

        #region ICommand & Actions
        public ICommand AddNewPrinterCommand
        {
            get { return new RelayCommand(p => AddNewPrinterAction()); }
        }
        private async void AddNewPrinterAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.NewPrinter };
                var newPrinterViewModel = new New3DPrinterViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Printers.Add(new _3dPrinterModel()
                    {
                        Id = instance.Id,
                        Price = instance.Price,
                        Type = instance.Type,
                        Supplier = instance.Supplier,
                        Manufacturer = instance.Manufacturer,
                        hasHeatbed = instance.hasHeatbed,
                        MaxHeatbedTemperature = instance.TemperatureHeatbed,
                        Kind = instance.Kind,
                        Model = instance.Model,
                        MachineHourRate = instance.MachineHourRate,
                        MachineHourRateCalc = instance.MachineHourRateCalc,
                        MaxNozzleTemperature = instance.TemperatureNozzle,
                        BuildVolume = instance.BuildVolume,
                        PowerConsumption = instance.PowerConsumption,
                        ShopUri = instance.LinkToReorder,
                    });
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
            get { return new RelayCommand(p => AddNewPrinterChildWindowAction()); }
        }
        private async void AddNewPrinterChildWindowAction()
        {
            try
            {

                var _childWindow = new ChildWindow()
                {
                    Title = Strings.NewPrinter,
                    AllowMove = true,
                    ShowCloseButton = false,
                    CloseByEscape = false,
                    IsModal = true,
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (Color)Application.Current.Resources["Gray2"] },
                    Padding = new Thickness(50),
                };
                var newPrinterViewModel = new New3DPrinterViewModel(async instance =>
                {
                    _childWindow.Close();
                    Printers.Add(new _3dPrinterModel()
                    {
                        Id = instance.Id,
                        Price = instance.Price,
                        Type = instance.Type,
                        Supplier = instance.Supplier,
                        Manufacturer = instance.Manufacturer,
                        hasHeatbed = instance.hasHeatbed,
                        MaxHeatbedTemperature = instance.TemperatureHeatbed,
                        Kind = instance.Kind,
                        Model = instance.Model,
                        MachineHourRate = instance.MachineHourRate,
                        MachineHourRateCalc = instance.MachineHourRateCalc,
                        MaxNozzleTemperature = instance.TemperatureNozzle,
                        BuildVolume = instance.BuildVolume,
                        PowerConsumption = instance.PowerConsumption,
                        ShopUri = instance.LinkToReorder,
                    });
                    logger.Info(string.Format(Strings.EventAddedItemFormated, Printers[Printers.Count - 1].Name));
                }, instance =>
                {
                    _childWindow.Close();
                },
                    this._dialogCoordinator
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
                await _dialogCoordinator.ShowMessageAsync(this,
                        Strings.DialogExceptionHeadline,
                        string.Format(Strings.DialogExceptionFormatedContent, exc.Message)
                        );
            }
        }

        public ICommand DeletePrinterCommand
        {
            get => new RelayCommand(p => DeletePrinterAction(p));
        }
        private async void DeletePrinterAction(object p)
        {
            try
            {
                _3dPrinterModel printer = p as _3dPrinterModel;
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
            get => new RelayCommand(p => DeletePrintersAction(p));
        }
        private async void DeletePrintersAction(object items)
        {
            try
            {
                //var type = items.GetType();
                var printers = items as IList<_3dPrinterModel>;
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
                            _3dPrinterModel printer = printers[i] as _3dPrinterModel;
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
            get => new RelayCommand(p => DeleteSelectedPrintersAction());
        }
        private async void DeleteSelectedPrintersAction()
        {
            try
            {
                var res = await _dialogCoordinator.ShowMessageAsync(this,
                       Strings.DialogDeleteSelectedPrintersHeadline, Strings.DialogDeleteSelectedPrintersContent,
                       MessageDialogStyle.AffirmativeAndNegative
                       );
                if (res == MessageDialogResult.Affirmative)
                {
                    //var selectedPrinters = SelectedPrintersView;
                    //foreach (PrinterViewInfo printer in selectedPrinters)
                    IList collection = new ArrayList(SelectedPrintersView);
                    for (int i = 0; i < collection.Count; i++)
                    {
                        var obj = collection[i] as PrinterViewInfo;
                        if (obj == null)
                            continue;
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, obj.Name));
                        Printers.Remove(obj.Printer);
                    }
                    /*
                    for (int i = 0; i < SelectedPrintersView.Count; i++)
                    {
                        PrinterViewInfo printer = SelectedPrintersView[i] as PrinterViewInfo;
                        logger.Info(string.Format(Strings.EventDeletedItemFormated, printer.Name));
                        Printers.Remove(printer.Printer);
                    }
                    */
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
            get => new RelayCommand(p => EditSelectedPrinterAction());
        }
        private async void EditSelectedPrinterAction()
        {
            try
            {
                var selectedPrinter = SelectedPrinterView.Printer;
                var _dialog = new CustomDialog() { Title = Strings.EditPrinter };
                var newPrinterViewModel = new New3DPrinterViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Printers.Remove(selectedPrinter);
                    Printers.Add(new _3dPrinterModel()
                    {
                        Id = instance.Id,
                        Price = instance.Price,
                        Type = instance.Type,
                        Supplier = instance.Supplier,
                        Manufacturer = instance.Manufacturer,
                        hasHeatbed = instance.hasHeatbed,
                        MaxHeatbedTemperature = instance.TemperatureHeatbed,
                        Kind = instance.Kind,
                        Model = instance.Model,
                        MachineHourRate = instance.MachineHourRate,
                        MachineHourRateCalc = instance.MachineHourRateCalc,
                        MaxNozzleTemperature = instance.TemperatureNozzle,
                        BuildVolume = instance.BuildVolume,
                        PowerConsumption = instance.PowerConsumption,
                        ShopUri = instance.LinkToReorder,

                    });
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
            get => new RelayCommand(p => EditPrinterAction(p));
        }
        private async void EditPrinterAction(object printer)
        {
            try
            {
                var selectedPrinter = printer as _3dPrinterModel;
                if (selectedPrinter == null)
                {
                    return;
                }
                var _dialog = new CustomDialog() { Title = Strings.EditPrinter };
                var newPrinterViewModel = new New3DPrinterViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Printers.Remove(selectedPrinter);
                    Printers.Add(new _3dPrinterModel()
                    {
                        Id = instance.Id,
                        Price = instance.Price,
                        Type = instance.Type,
                        Supplier = instance.Supplier,
                        Manufacturer = instance.Manufacturer,
                        hasHeatbed = instance.hasHeatbed,
                        MaxHeatbedTemperature = instance.TemperatureHeatbed,
                        Kind = instance.Kind,
                        Model = instance.Model,
                        MachineHourRate = instance.MachineHourRate,
                        MachineHourRateCalc = instance.MachineHourRateCalc,
                        MaxNozzleTemperature = instance.TemperatureNozzle,
                        BuildVolume = instance.BuildVolume,
                        PowerConsumption = instance.PowerConsumption,
                        ShopUri = instance.LinkToReorder,

                    });
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
            get => new RelayCommand(p => ReorderPrinterAction(p));
        }
        private async void ReorderPrinterAction(object parameter)
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
        #endregion

        #region Methods
        public void OnViewVisible()
        {
            createPrinterViewInfos();
        }

        public void OnViewHide()
        {

        }
        #endregion
    }
}
