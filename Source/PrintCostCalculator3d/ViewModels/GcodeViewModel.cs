using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using log4net;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using PrintCostCalculator3d.Models;
using PrintCostCalculator3d.Models.GCode;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Utilities;
using System.Windows;
using ICSharpCode.AvalonEdit.CodeCompletion;
using PrintCostCalculator3d.Models.SyntaxHighlighting;
using ICSharpCode.AvalonEdit.Utils;
using PrintCostCalculator3d.Resources.Localization;
using Dragablz;
using System.Collections.ObjectModel;
using PrintCostCalculator3d.Controls;
using PrintCostCalculator3d.Templates;
using PrintCostCalculator3d.Models.GCode.Helper;

namespace PrintCostCalculator3d.ViewModels
{
    class GcodeViewModel : PaneViewModel
    {
        #region Variables
        private readonly IDialogCoordinator _dialogCoordinator;
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        

        #endregion

        #region Properties
        private bool _isEdit;
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
        public bool isLicenseValid
        {
            get => false;
        }

        private Guid _id = Guid.NewGuid();
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

        private ObservableCollection<GCode> _gcodes = new ObservableCollection<GCode>();
        public ObservableCollection<GCode> Gcodes
        {
            get => _gcodes;
            set
            {
                if (_gcodes == value)
                    return;
                _gcodes = value;
                OnPropertyChanged();
            }
        }

        private GCode _gcode;
        public GCode Gcode
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
        private string _filePath = string.Empty;
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
                readFile(FilePath);
            }
        }

        private string _fileName = string.Empty;
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(FilePath))
                    return string.Format("{0}{1}", Strings.New, IsDirty ? "*" : string.Empty);
                return string.Format(Path.GetFileName(FilePath), IsDirty ? "*" : string.Empty);
            }
        }

        private TextDocument _document = new TextDocument();
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

        private IHighlightingDefinition _highlightdef = null;
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

        public string Title
        {
            get
            {
                return Path.GetFileName(this.FilePath) + (IsDirty ? "*" : string.Empty);
            }

            set
            {
                base.Title = value;
            }
        }

        private bool _isDirty = false;
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

        private bool _isReadOnly = false;
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

        private string _IsReadOnlyReason = string.Empty;
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

        private bool _WordWrap = false;
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

        private bool _ShowLineNumbers = false;
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

        private ICSharpCode.AvalonEdit.TextEditorOptions _TextOptions  = new ICSharpCode.AvalonEdit.TextEditorOptions()
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

        private readonly bool _isLoading;

        private int _tabId;

        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (value == _selectedTabIndex)
                    return;

                _selectedTabIndex = value;
                Gcode = Gcodes[SelectedTabIndex];
                OnPropertyChanged();
            }
        }
        #endregion

        #region Expander
        private bool _expandProfileView;
        public bool ExpandProfileView
        {
            get => _expandProfileView;
            set
            {
                if (value == _expandProfileView)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.GcodeViewer_ExpandProfileView = value;

                _expandProfileView = value;

                /*
                if (_canProfileWidthChange)
                    ResizeProfile(false);
                */
                OnPropertyChanged();
            }
        }

        private GridLength _profileWidth;
        public GridLength ProfileWidth
        {
            get => _profileWidth;
            set
            {
                if (value == _profileWidth)
                    return;

                if (!_isLoading && Math.Abs(value.Value - GlobalStaticConfiguration.GcodeInfo_WidthCollapsed) > GlobalStaticConfiguration.FloatPointFix) // Do not save the size when collapsed
                    SettingsManager.Current.GcodeViewer_ProfileWidth = value.Value;

                _profileWidth = value;
                /*
                if (_canProfileWidthChange)
                    ResizeProfile(true);
                    */
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Settings

        #endregion

        #region Constructor
        public GcodeViewModel(IDialogCoordinator dialogCoordinator,  IList<GCode> files = null)
        {
            try
            {
                Gcodes = new ObservableCollection<GCode>(files);
                this._dialogCoordinator = dialogCoordinator;
                getHighlightingDefinitions();
                InterTabClient = new DragablzInterTabClient(ApplicationName._3dPrintingCalcualtion);
                if (files != null)
                {
                    TabItems = new ObservableCollection<DragablzTabItem>();
                    foreach (GCode file in files)
                    {
                        TabItems.Add(new DragablzTabItem(file.FileName, new CodeEditorViewTemplate(_tabId, file), _tabId));
                        _tabId++;
                    }
                }
                else
                {
                    TabItems = new ObservableCollection<DragablzTabItem>
                    {
                        new DragablzTabItem(Strings.NewTab, new CodeEditorViewTemplate(_tabId), _tabId)
                    };
                }
                Gcode = Gcodes[SelectedTabIndex];
            }
            catch(Exception exc)
            {
                logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
            }
        }
        public GcodeViewModel(Action<GcodeViewModel> saveCommand, Action<GcodeViewModel> cancelHandler, GCode gcode = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            getHighlightingDefinitions();
            if(gcode != null)
            {
                try
                {
                    FilePath = gcode.FilePath;
                }
                catch(Exception exc)
                {
                    logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
                }
            }
        }
        public GcodeViewModel(Action<GcodeViewModel> saveCommand, Action<GcodeViewModel> cancelHandler, IDialogCoordinator dialogCoordinator, GCode gcode = null)
        {
            SaveCommand = new RelayCommand(p => saveCommand(this));
            CancelCommand = new RelayCommand(p => cancelHandler(this));
            getHighlightingDefinitions();
            if (gcode != null)
            {
                try
                {
                    FilePath = gcode.FilePath;
                }
                catch (Exception exc)
                {
                    logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
                }
            }

            this._dialogCoordinator = dialogCoordinator;
        }

        #endregion

        #region Methods
        private void getHighlightingDefinitions()
        {
            // Load our custom highlighting definition
            IHighlightingDefinition customHighlighting;
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("PrintCostCalculator3d.Resources.SyntaxHighlighter.GcodeHighlighting.xshd"))
            {
                string[] names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Gcode Highlighting", new string[] { ".gcode", ".gc" }, customHighlighting);

        }

        private void readFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var ext = Path.GetExtension(filePath);
                    this.Document = new TextDocument();
                    this.HighlightDef = HighlightingManager.Instance.GetDefinitionByExtension(ext);
                    this.IsDirty = false;
                    this.IsReadOnly = false;
                    this.ShowLineNumbers = false;
                    this.WordWrap = false;

                    // Check file attributes and set to read-only if file attributes indicate that
                    if ((File.GetAttributes(filePath) & FileAttributes.ReadOnly) != 0)
                    {
                        this.IsReadOnly = true;
                        this.IsReadOnlyReason = "This file cannot be edit because another process is currently writting to it.\n" +
                                                "Change the file access permissions or save the file in a different location if you want to edit it.";
                    }

                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (StreamReader reader = FileReader.OpenStream(fs, Encoding.UTF8))
                        {
                            this._document = new TextDocument(reader.ReadToEnd());
                        }
                    }

                    ContentId = _filePath;
                }
            }
            catch(Exception exc)
            {
                logger.ErrorFormat(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message);
            }
        }

        public void OnViewVisible()
        {

        }

        public void OnViewHide()
        {

        }
        public void AddTab(string host = null)
        {
            _tabId++;

            TabItems.Add(new DragablzTabItem(Resources.Localization.Strings.NewTab, new CodeEditorViewTemplate(_tabId, null), _tabId));

            SelectedTabIndex = TabItems.Count - 1;
        }
        #endregion  

        #region Events

        #endregion

        #region iCommands & Actions
        public ICommand AddTabCommand
        {
            get { return new RelayCommand(p => AddTabAction()); }
        }

        private void AddTabAction()
        {
            _tabId++;

            TabItems.Add(new DragablzTabItem(Strings.NewTab, new CodeEditorViewTemplate(_tabId), _tabId));

            SelectedTabIndex = TabItems.Count - 1;
        }

        public ItemActionCallback CloseItemCommand => CloseItemAction;

        private static void CloseItemAction(ItemActionCallbackArgs<TabablzControl> args)
        {
            ((args.DragablzItem.Content as DragablzTabItem)?.View as CodeEditorViewTemplate)?.CloseTab();
        }
        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }
        #endregion
    }
}
