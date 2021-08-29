using AndreasReitberger.Models;
using Dragablz;
using GalaSoft.MvvmLight.Messaging;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;
using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Controls;
using PrintCostCalculator3d.Enums;
using PrintCostCalculator3d.Models.Messaging;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Models.SyntaxHighlighting;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Templates;
using PrintCostCalculator3d.Utilities;
using PrintCostCalculator3d.ViewModels._3dPrinting;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace PrintCostCalculator3d.ViewModels.Dashboard
{
    public class DashboardGcodeEditorViewModel : PaneViewModel
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        readonly SharedCalculatorInstance SharedCalculatorInstance = SharedCalculatorInstance.Instance;
        #endregion

        #region Properties
        public DashboardTabContentType TabContentType { get; set; }

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

        ObservableCollection<Gcode> _gcodes = new();
        public ObservableCollection<Gcode> Gcodes
        {
            get => _gcodes;
            set
            {
                if (_gcodes == value)
                {
                    return;
                }

                _gcodes = value;
                SharedCalculatorInstance.Gcodes = _gcodes;
                OnPropertyChanged();
            }
        }

        Gcode _gcode;
        public Gcode Gcode
        {
            get => _gcode;
            set
            {
                if (_gcode != value)
                {
                    _gcode = value;
                    OnPropertyChanged();
                }
            }
        }

        #region SyntaxHighlighter
        string _filePath = string.Empty;
        public string FilePath
        {
            get => _filePath;
            set
            {
                if (_filePath == value)
                    return;
                _filePath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FileName));
                OnPropertyChanged(nameof(Title));
                ReadFile(FilePath);
            }
        }

        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(FilePath))
                    return string.Format("{0}{1}", Strings.New, IsDirty ? "*" : string.Empty);
                return string.Format(Path.GetFileName(FilePath), IsDirty ? "*" : string.Empty);
            }
        }

        TextDocument _document = new();
        public TextDocument Document
        {
            get => _document;
            set
            {
                if (_document == value)
                    return;
                _document = value;
                OnPropertyChanged();
                IsDirty = true;
            }
        }

        IHighlightingDefinition _highlightdef = null;
        public IHighlightingDefinition HighlightDef
        {
            get => _highlightdef;
            set
            {
                if (_highlightdef == value)
                    return;
                _highlightdef = value;
                OnPropertyChanged();
                IsDirty = true;
            }
        }

        public new string Title
        {
            get => Path.GetFileName(this.FilePath) + (IsDirty ? "*" : string.Empty);
            set => base.Title = value;
        }

        bool _isDirty = false;
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Title));
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }

        bool _isReadOnly = false;
        public bool IsReadOnly
        {
            get => _isReadOnly;

            protected set
            {
                if (_isReadOnly == value)
                    return;
                _isReadOnly = value;
                OnPropertyChanged();
            }
        }

        string _IsReadOnlyReason = string.Empty;
        public string IsReadOnlyReason
        {
            get => _IsReadOnlyReason;

            protected set
            {
                if (_IsReadOnlyReason == value)
                    return;
                _IsReadOnlyReason = value;
                OnPropertyChanged();
            }
        }

        ICSharpCode.AvalonEdit.TextEditorOptions _TextOptions = new()
        {
            ConvertTabsToSpaces = false,
            IndentationSize = 2
        };

        public ICSharpCode.AvalonEdit.TextEditorOptions TextOptions
        {
            get => _TextOptions;

            set
            {
                if (_TextOptions == value)
                    return;
                _TextOptions = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Tabs
        public IInterTabClient InterTabClient { get; }
        public ObservableCollection<DragablzTabItem> TabItems { get; }

        int _tabId;

        int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (value == _selectedTabIndex)
                    return;

                _selectedTabIndex = value;
                //Gcode = Gcodes[SelectedTabIndex];
                OnPropertyChanged();
            }
        }
        #endregion

        #region Expander
        bool _expandProfileView;
        public bool ExpandProfileView
        {
            get => _expandProfileView;
            set
            {
                if (value == _expandProfileView)
                    return;

                if (!IsLoading)
                    SettingsManager.Current.GcodeViewer_ExpandProfileView = value;

                _expandProfileView = value;


                if (_canProfileWidthChange)
                    ResizeProfile(false);

                OnPropertyChanged();
            }
        }

        bool _canProfileWidthChange = true;
        double _tempProfileWidth;

        GridLength _profileWidth;
        public GridLength ProfileWidth
        {
            get => _profileWidth;
            set
            {
                if (value == _profileWidth)
                    return;

                if (!IsLoading && Math.Abs(value.Value - GlobalStaticConfiguration.GcodeInfo_WidthCollapsed) > GlobalStaticConfiguration.FloatPointFix)
                    // Do not save the size when collapsed
                    SettingsManager.Current.GcodeViewer_ProfileWidth = value.Value;

                _profileWidth = value;

                if (_canProfileWidthChange)
                    ResizeProfile(true);

                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Settings
        bool _WordWrap = false;
        public bool WordWrap
        {
            get => _WordWrap;

            set
            {
                if (_WordWrap == value)
                    return;
                _WordWrap = value;
                OnPropertyChanged();
            }
        }

        bool _ShowLineNumbers = false;
        public bool ShowLineNumbers
        {
            get => _ShowLineNumbers;

            set
            {
                if (_ShowLineNumbers == value)
                    return;
                _ShowLineNumbers = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor
        //public DashboardGcodeEditorViewModel(IDialogCoordinator dialogCoordinator, DashboardTabContentType TabName, IList<GCode> files = null)
        public DashboardGcodeEditorViewModel(IDialogCoordinator dialogCoordinator, DashboardTabContentType TabName)
        {
            try
            {
                TabContentType = TabName;
                _dialogCoordinator = dialogCoordinator;
                GetHighlightingDefinitions();

                IsLicenseValid = false;

                IsLoading = true;
                LoadSettings();
                IsLoading = false;

                //Gcodes = new ObservableCollection<GCode>(files);
                SharedCalculatorInstance.OnGcodesChanged += SharedCalculatorInstance_OnGcodesChanged;
                SharedCalculatorInstance.OnSelectedGcodeChanged += SharedCalculatorInstance_OnSelectedGcodeChanged;

                Gcode = SharedCalculatorInstance.Gcode;
                Gcodes = new ObservableCollection<Gcode>(SharedCalculatorInstance.Gcodes);
                Gcodes.CollectionChanged += Gcodes_CollectionChanged;

                InterTabClient = new DragablzInterTabClient(ApplicationName.Dashboard);
                if (Gcodes != null && Gcodes.Count > 0)
                {
                    TabItems = new ObservableCollection<DragablzTabItem>();
                    foreach (Gcode file in Gcodes)
                    {
                        TabItems.Add(new DragablzTabItem(file.FileName, new CodeEditorViewTemplate(_tabId, file), _tabId));
                        _tabId++;
                    }
                }
                else
                {
                    TabItems = new ObservableCollection<DragablzTabItem>
                    {
                        //new DragablzTabItem(Strings.NewTab, new CodeEditorViewTemplate(_tabId), _tabId)
                    };
                }
                RegisterMessages();
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
            }
        }

        void LoadSettings()
        {
            ProfileWidth = ExpandProfileView ?
                new GridLength(SettingsManager.Current.GcodeViewer_ProfileWidth) :
                new GridLength(GlobalStaticConfiguration.GcodeInfo_WidthCollapsed);
            _tempProfileWidth = SettingsManager.Current.GcodeViewer_ProfileWidth;

            ExpandProfileView = SettingsManager.Current.GcodeViewer_ExpandProfileView;
        }

        void RegisterMessages()
        {
            Messenger.Default.Register<GcodesEditActionMessage>(this, OnGcodesEditActionReceived);
        }
        #endregion

        #region Messages
        void OnGcodesEditActionReceived(GcodesEditActionMessage msg)
        {
            try
            {
                if (msg != null && msg.GcodeFiles.Count > 0)
                {
                    foreach (var gcode in msg.GcodeFiles)
                    {
                        var tabItem = TabItems.FirstOrDefault(tab => tab.Header == gcode.FileName);
                        if (tabItem == null)
                        {
                            TabItems.Add(new DragablzTabItem(gcode.FileName, new CodeEditorViewTemplate(_tabId, gcode), _tabId++));
                            SelectedTabIndex = TabItems.Count - 1;
                        }
                        else
                            SelectedTabIndex = tabItem.Id;
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }
        #endregion

        #region Events
        void Gcodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Gcodes));
            try
            {
                if (e != null)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach (var gc in e.NewItems)
                            {
                                Gcode newItem = (Gcode)gc;
                                if (newItem == null)
                                    continue;
                                SharedCalculatorInstance.AddGcode(newItem);
                            }

                            break;
                        case NotifyCollectionChangedAction.Remove:
                            foreach (var gc in e.OldItems)
                            {
                                Gcode newItem = (Gcode)gc;
                                if (newItem == null)
                                    continue;
                                SharedCalculatorInstance.RemoveGcode(newItem);
                            }
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            break;
                        case NotifyCollectionChangedAction.Move:
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            SharedCalculatorInstance.Gcodes.Clear();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        #region SharedInstance
        void SharedCalculatorInstance_OnSelectedGcodeChanged(object sender, Models.Events.GcodeChangedEventArgs e)
        {
            if (e != null)
            {
                Gcode = e.NewGcode;
            }
        }

        void SharedCalculatorInstance_OnGcodesChanged(object sender, Models.Events.GcodesChangedEventArgs e)
        {
            if (e != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewItems != null)
                        {
                            foreach (Gcode gc in e.NewItems)
                            {
                                // Check if item has not been added already
                                var item = Gcodes.FirstOrDefault(c => c.Id == gc.Id);
                                if (item != null) continue;
                                Gcodes.Add(gc);

                                // Add Tab
                                TabItems.Add(new DragablzTabItem(
                                    gc.FileName, 
                                    new CodeEditorViewTemplate(_tabId, gc), 
                                    _tabId++)
                                    );
                                SelectedTabIndex = TabItems.Count - 1;
                            }

                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        if (e.OldItems != null)
                        {
                            foreach (Gcode gc in e.OldItems)
                            {
                                var item = Gcodes.FirstOrDefault(c => c.Id == gc.Id);
                                if (item == null) continue;

                                // Remove Tab
                                var tabItem = TabItems.FirstOrDefault(tab => tab.Header == item.FileName);
                                TabItems.Remove(tabItem);
                                Gcodes.Remove(item);

                                SelectedTabIndex = TabItems.Count - 1;
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        Gcodes.Clear();
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
        #endregion

        #region iCommands & Actions

        #region TabControl

        public ICommand AddTabCommand
        {
            get => new RelayCommand(async(p) => await AddTabAction()); 
        }
        async Task AddTabAction()
        {
            try
            {
                var _dialog = new CustomDialog() { Title = Strings.SelectedGcode };
                var newViewModel = new SelectGcodesDialogViewModel(async instance =>
                {
                    await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    foreach (var file in instance.Gcodes)
                    {
                        var tab = TabItems.FirstOrDefault(tabItem => tabItem.Header == file.FileName);
                        if (tab == null)
                        {
                            TabItems.Add(new DragablzTabItem(file.FileName, new CodeEditorViewTemplate(_tabId, file), _tabId++));
                            SelectedTabIndex = TabItems.Count - 1;
                        }
                        else
                            SelectedTabIndex = tab.Id;
                    }
                }, instance =>
                {
                    _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                },
                    this._dialogCoordinator
                );

                _dialog.Content = new Views._3dPrinting.SelectGcodesDialogView()
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

        public ItemActionCallback CloseItemCommand => CloseItemAction;

        static void CloseItemAction(ItemActionCallbackArgs<TabablzControl> args)
        {
            ((args.DragablzItem.Content as DragablzTabItem)?.View as CodeEditorViewTemplate)?.CloseTab();
        }
        #endregion

        #region DragnDrop
        public ICommand OnDropFileCommand
        {
            get => new RelayCommand(async (p) => await OnDropFileAction(p));
        }
        async Task OnDropFileAction(object p)
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
                                try
                                {
                                    
                                    logger.Info(string.Format(Strings.EventFileLoadedForamated, file));
                                }
                                catch (Exception exc)
                                {
                                    logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                                }

                            }
                            await this._dialogCoordinator.ShowMessageAsync(this,
                                Strings.DialogFileLoadedHeadline,
                                Strings.DialogFileLoadedContent
                                );
                        }
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

        #region Expander
        void ResizeProfile(bool dueToChangedSize)
        {
            _canProfileWidthChange = false;

            if (dueToChangedSize)
            {
                ExpandProfileView = Math.Abs(ProfileWidth.Value - GlobalStaticConfiguration.GcodeInfo_WidthCollapsed) > GlobalStaticConfiguration.FloatPointFix;
            }
            else
            {
                if (ExpandProfileView)
                {
                    ProfileWidth = Math.Abs(_tempProfileWidth - GlobalStaticConfiguration.GcodeInfo_WidthCollapsed) < GlobalStaticConfiguration.FloatPointFix ?
                        new GridLength(GlobalStaticConfiguration.GcodeInfo_DefaultWidthExpanded) :
                        new GridLength(_tempProfileWidth);
                }
                else
                {
                    _tempProfileWidth = ProfileWidth.Value;
                    ProfileWidth = new GridLength(GlobalStaticConfiguration.GcodeInfo_WidthCollapsed);
                }
            }

            _canProfileWidthChange = true;
        }
        #endregion

        #region GcodeEditor
        void GetHighlightingDefinitions()
        {
            // Load our custom highlighting definition
            IHighlightingDefinition customHighlighting;
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("PrintCostCalculator3d.Resources.SyntaxHighlighter.GcodeHighlighting.xshd"))
            {
                string[] names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                if (s == null)
                {
                    throw new InvalidOperationException("Could not find embedded resource");
                }

                using XmlReader reader = new XmlTextReader(s);
                customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                    HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Gcode Highlighting", GlobalStaticConfiguration.Gcode_ValidFileTypes, customHighlighting);

        }

        void ReadFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string ext = Path.GetExtension(filePath);
                    Document = new TextDocument();
                    HighlightDef = HighlightingManager.Instance.GetDefinitionByExtension(ext);
                    IsDirty = false;
                    IsReadOnly = false;
                    ShowLineNumbers = false;
                    WordWrap = false;

                    // Check file attributes and set to read-only if file attributes indicate that
                    if ((File.GetAttributes(filePath) & FileAttributes.ReadOnly) != 0)
                    {
                        IsReadOnly = true;
                        IsReadOnlyReason = "This file cannot be edit because another process is currently writting to it.\n" +
                                                "Change the file access permissions or save the file in a different location if you want to edit it.";
                    }

                    using (FileStream fs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using StreamReader reader = FileReader.OpenStream(fs, Encoding.UTF8);
                        _document = new TextDocument(reader.ReadToEnd());
                    }

                    ContentId = _filePath;
                }
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
            }
        }
        #endregion

        public void AddTab(string host = null)
        {
            _tabId++;

            TabItems.Add(new DragablzTabItem(Strings.NewTab, new CodeEditorViewTemplate(_tabId, null), _tabId));

            SelectedTabIndex = TabItems.Count - 1;
        }

        public void OnViewVisible()
        {
            try
            {
                OnPropertyChanged(nameof(IsLicenseValid));

            }
            catch (Exception exc)
            {
                logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
            }
        }

        public void OnViewHide()
        {

        }

        public void OnClose()
        {

        }

        #endregion  

    }
}
