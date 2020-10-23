using System;
using System.Collections.Generic;
using System.Linq;

//Additional
using PrintCostCalculator3d.Utilities;
using PrintCostCalculator3d.Models._3dprinting;
using PrintCostCalculator3d.Models.Settings;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;
using System.Windows.Data;
using System.Collections;
using PrintCostCalculator3d.Resources.Localization;
using log4net;
using MahApps.Metro.SimpleChildWindow;
using System.Windows.Media;
using System.Windows;
using System.Threading.Tasks;
using AndreasReitberger.Models;
using AndreasReitberger.Enums;
using PrintCostCalculator3d.Models;

namespace PrintCostCalculator3d.ViewModels._3dPrinting 
{
    public class _3dPrintingPrinterViewModel : ViewModelBase
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool _isLoading;
        #endregion

        #region Properties
        public bool isLicenseValid
        {
            get => false;
        }

        private ObservableCollection<Printer3d> _printers = new ObservableCollection<Printer3d>();
        public ObservableCollection<Printer3d> Printers
        {
            get => _printers;
            set
            {
                if (_printers == value) return;
                if(!_isLoading)
                    SettingsManager.Current.Printers = value;
                _printers = value;
                OnPropertyChanged();
                
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
            this._dialogCoordinator = dialog;

            _isLoading = true;
            LoadSettings();
            _isLoading = false;

            Printers.CollectionChanged += Printers_CollectionChanged;

            createPrinterViewInfos();
            logger.Info(string.Format(Strings.EventViewInitFormated, this.GetType().Name));
        }

        private void LoadSettings()
        {
            Printers = SettingsManager.Current.Printers;
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
            get { return new RelayCommand(async(p) => await AddNewPrinterAction()); }
        }
        private async Task AddNewPrinterAction()
        {
            try
            {
                /*
                FirebaseHandler fbh = new FirebaseHandler("8R9Eac6MeET453bPR7UkPLxQbZNcIlaRDEGnYDFC");
                var res = await fbh.GetDefaultPrinters();
                await fbh.AddDefaultPrinter(getPrinterFromInstance(instance));
                */
                var _dialog = new CustomDialog() { Title = Strings.NewPrinter };
                var newPrinterViewModel = new New3DPrinterViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    Printers.Add(getPrinterFromInstance(instance));
                    
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
        private async Task AddNewPrinterChildWindowAction()
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
                    OverlayBrush = new SolidColorBrush() { Opacity = 0.7, Color = (Color)Application.Current.Resources["MahApps.Colors.Gray2"] },
                    Padding = new Thickness(50),
                };
                var newPrinterViewModel = new New3DPrinterViewModel(async instance =>
                {
                    _childWindow.Close();
                    Printers.Add(getPrinterFromInstance(instance));
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
            get => new RelayCommand(async (p) => await DeletePrinterAction(p));
        }
        private async Task DeletePrinterAction(object p)
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
        private async Task DeletePrintersAction(object items)
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
        private async Task DeleteSelectedPrintersAction()
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
        private async Task EditSelectedPrinterAction()
        {
            try
            {
                var selectedPrinter = SelectedPrinterView.Printer;
                var _dialog = new CustomDialog() { Title = Strings.EditPrinter };
                var newPrinterViewModel = new New3DPrinterViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    updatePrinterFromInstance(instance, selectedPrinter);
                    //Printers.Remove(selectedPrinter);
                    //Printers.Add(getPrinterFromInstance(instance));
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
        private async Task EditPrinterAction(object printer)
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
                    updatePrinterFromInstance(instance, selectedPrinter);
                    //Printers.Remove(selectedPrinter);
                    //Printers.Add(getPrinterFromInstance(instance));
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
        private async Task ReorderPrinterAction(object parameter)
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
            get => new RelayCommand(async (p) => await DuplicatePrinterAction(p));
        }
        private async Task DuplicatePrinterAction(object p)
        {
            try
            {
                Printer3d printer = p as Printer3d;
                if (printer != null)
                {
                    var duplicates = Printers.Where(prt => prt.Model.StartsWith(prt.Model.Split('_')[0]));

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
        private Printer3d getPrinterFromInstance(New3DPrinterViewModel instance, Printer3d printer = null)
        {
            Printer3d temp = printer ?? new Printer3d();
            try
            {
                temp.Id = instance.Id;
                temp.Price = instance.Price;
                temp.Type = instance.Type;
                //temp.Supplier = instance.Supplier;
                temp.Manufacturer = instance.Manufacturer;
                temp.MaterialType = instance.MaterialFamily;
                temp.Model = instance.Model;
                temp.UseFixedMachineHourRating = instance.UseFixedMachineHourRating;
                if (temp.UseFixedMachineHourRating)
                    temp.HourlyMachineRate = new HourlyMachineRate() { FixMachineHourRate = instance.MachineHourRate };
                else
                    temp.HourlyMachineRate = instance.MachineHourRateCalculation;
                temp.BuildVolume = instance.BuildVolume;
                temp.PowerConsumption = instance.PowerConsumption;
                temp.Uri = instance.LinkToReorder;
                temp.Attributes = instance.Attributes.ToList();
                // Not implemented yet
                temp.Maintenances = new ObservableCollection<Maintenance3d>();
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.DialogExceptionFormatedContent, exc.Message, exc.TargetSite);
            }
            return temp;
        }
        private void updatePrinterFromInstance(New3DPrinterViewModel instance, Printer3d printer)
        {
            try
            {
                getPrinterFromInstance(instance, printer);
                /*
                printer.Id = instance.Id;
                printer.Price = instance.Price;
                printer.Type = instance.Type;
                //printer.Supplier = instance.Supplier;
                printer.Manufacturer = instance.Manufacturer;
                printer.MaterialType = instance.MaterialFamily;
                printer.Model = instance.Model;
                printer.UseFixedMachineHourRating = instance.UseFixedMachineHourRating;
                printer.HourlyMachineRate = instance.MachineHourRate;
                printer.BuildVolume = instance.BuildVolume;
                printer.PowerConsumption = instance.PowerConsumption;
                printer.Uri = instance.LinkToReorder;
                printer.Attributes = instance.Attributes.ToList();
                // Not implemented yet
                printer.Maintenances = new ObservableCollection<Maintenance3d>();
                */

                SettingsManager.Current.Printers = Printers;
                SettingsManager.Save();

                OnPropertyChanged(nameof(Printers));
                createPrinterViewInfos();
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.DialogExceptionFormatedContent, exc.Message, exc.TargetSite);
            }
        }
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
                    Group = (Printer3dType)Enum.Parse(typeof(Printer3dType), p.Type.ToString()),
                })).ToList()
            }.View;
            PrinterViews.SortDescriptions.Add(new SortDescription(nameof(PrinterViewInfo.Group), ListSortDirection.Ascending));
            PrinterViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(PrinterViewInfo.Group)));
        }
        public void OnViewVisible()
        {
            Printers.CollectionChanged += Printers_CollectionChanged;
            createPrinterViewInfos();
            OnPropertyChanged(nameof(isLicenseValid));
        }

        public void OnViewHide()
        {

        }
        #endregion
    }
}
