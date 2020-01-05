using HelixToolkit.Wpf;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WpfFramework.Models.GCode.Helper;
using WpfFramework.Models.Settings;
using WpfFramework.Models.Slicer;
using WpfFramework.Resources.Localization;

namespace WpfFramework.Models.GCode
{
    public class GCode : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Variables
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties

        #region Internal
        private Guid _id;
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

        private bool _isValid = false;
        public bool IsValid
        {
            get => _isValid;
            private set
            {
                if (_isValid == value)
                    return;
                _isValid = value;
                OnPropertyChanged();
            }
        }

        private bool _isWorking = false;
        public bool IsWorking
        {
            get => _isWorking;
            private set
            {
                if (_isWorking == value)
                    return;
                _isWorking = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region GcodeParser
        private GCodeParser _parser;
        public GCodeParser Parser
        {
            get => _parser;
            private set
            {
                if (_parser == value)
                    return;
                _parser = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region GcodeInformation
        private SlicerName _slicerName = SlicerName.Unkown;
        public SlicerName SlicerName
        {
            get => _slicerName;
            private set
            {
                if (_slicerName == value)
                    return;
                _slicerName = value;
                OnPropertyChanged();
            }
        }


        private string _fileName;
        public string FileName
        {
            get => _fileName;
            private set
            {
                if (_fileName == value)
                    return;
                _fileName = value;
                OnPropertyChanged();
            }
        }

        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            private set
            {
                if (_filePath == value)
                    return;
                _filePath = value;
                OnPropertyChanged();
            }
        }

        private int _lines = 0;
        public int Lines
        {
            get => _lines;
            set
            {
                if (_lines == value)
                    return;
                _lines = value;
                OnPropertyChanged();
            }
        }

        private double _filamentUsed = 0;
        /// <summary>Gets the filament used in "mm"</summary>
        /// <value>The filament used.</value>
        public double FilamentUsed
        {
            get => _filamentUsed;
            private set
            {
                if (_filamentUsed == value)
                    return;
                _filamentUsed = value;
                OnPropertyChanged();
            }
        }

        private double _diameter = 1.75f;
        public double Diameter
        {
            get => _diameter;
            set
            {
                if (_diameter == value)
                    return;
                _diameter = value;
                OnPropertyChanged();
            }
        }

        private double _volume;
        public double ExtrudedFilamentVolume
        {
            get => _volume;
            private set
            {
                if (_volume == value)
                    return;
                _volume = value;
                OnPropertyChanged();
            }
        }

        private double _printTime;
        public double PrintTime
        {
            get => _printTime;
            private set
            {
                if (_printTime == value)
                    return;
                _printTime = value;
                OnPropertyChanged();
            }
        }

        private string _filamentType;
        public string FilamentType
        {
            get => _filamentType;
            set
            {
                if (_filamentType == value)
                    return;
                _filamentType = value;
                OnPropertyChanged();
            }
        }

        private double _filamentDensity;
        public double FilamentDensity
        {
            get => _filamentDensity;
            set
            {
                if (_filamentDensity == value)
                    return;
                _filamentDensity = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region GcodeCommands
        private List<List<GCodeCommand>> _commands = new List<List<GCodeCommand>>();
        public List<List<GCodeCommand>> Commands
        {
            get => _commands;
            private set
            {
                if (_commands == value)
                    return;
                _commands = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region GcodeModel
        private float _width = 0;
        public float Width
        {
            get => _width;
            set
            {
                if (_width == value)
                    return;
                _width = value;
                OnPropertyChanged();
            }
        }
        
        private float _depth = 0;
        public float Depth
        {
            get => _depth;
            set
            {
                if (_depth == value)
                    return;
                _depth = value;
                OnPropertyChanged();
            }
        }
        
        private float _height = 0;
        public float Height
        {
            get => _height;
            set
            {
                if (_height == value)
                    return;
                _height = value;
                OnPropertyChanged();
            }
        }
        
        private int _layers = 0;
        public int Layers
        {
            get => _layers;
            set
            {
                if (_layers == value)
                    return;
                _layers = value;
                OnPropertyChanged();
            }
        }
        
        
        private List<LinesVisual3D> _modelLayers =  new List<LinesVisual3D>();
        public List<LinesVisual3D> ModelLayers
        {
            get => _modelLayers;
            private set
            {
                if (_modelLayers == value)
                    return;
                _modelLayers = value;
                OnPropertyChanged();
            }
        }
        
        private List<LinesVisual3D> _model3d=  new List<LinesVisual3D>();
        public List<LinesVisual3D> Model3D
        {
            get => _model3d;
            private set
            {
                if (_model3d == value)
                    return;
                _model3d = value;
                OnPropertyChanged();
            }
        }
        #endregion
        
        #endregion

        #region Constructor
        public GCode(string file)
        {
            Id = Guid.NewGuid();
            FilePath = file;
            FileName = Path.GetFileName(file);
            Parser = new GCodeParser(file);

        }
        #endregion

        #region Public Methods
        public async Task<bool> ReadGcodeAsync()
        {
            try
            {
                this.SlicerName = getSlicerName(Parser.Lines[0]);
                var slicers = Slicer.Slicer.SupportedSlicers.FirstOrDefault(slicer => slicer.SlicerName == this.SlicerName);
                // If slicer is known, try to read the information from the comments.
                if (slicers != null && SettingsManager.Current.GcodeParser_PreferValuesInCommentsFromKnownSlicers)
                {
                    try
                    {
                        this.ExtrudedFilamentVolume = Convert.ToDouble(getParameterFromSlicer(SlicerParameter.Volume), CultureInfo.GetCultureInfo("en-US"));
                    }
                    catch (Exception)
                    {
                        this.ExtrudedFilamentVolume = 0;
                    }
                    // PrintTime
                    try
                    {
                        this.PrintTime = Convert.ToDouble(convertPrintTimeToDec(
                            getParameterFromSlicer(SlicerParameter.PrintTime)), CultureInfo.GetCultureInfo("en-US"));
                    }
                    catch (Exception)
                    {
                        this.PrintTime = 0;
                    }
                    // Diameter
                    try
                    {
                        this.Diameter = Convert.ToDouble(getParameterFromSlicer(SlicerParameter.FilamentDiameter), CultureInfo.GetCultureInfo("en-US"));
                    }
                    catch (Exception)
                    {
                        this.Diameter = 0;
                    }
                    // Nozzle
                    try
                    {
                        this.Diameter = Convert.ToDouble(getParameterFromSlicer(SlicerParameter.NozzleDiameter), CultureInfo.GetCultureInfo("en-US"));
                    }
                    catch (Exception)
                    {
                        this.Diameter = 0;
                    }
                    // FilamentType
                    try
                    {
                        this.FilamentType = getParameterFromSlicer(SlicerParameter.FilamentType);
                    }
                    catch (Exception)
                    {
                        this.FilamentType = Strings.Unknown;
                    }
                    // FilamentDensity
                    try
                    {
                        this.FilamentDensity = Convert.ToDouble(getParameterFromSlicer(SlicerParameter.FilamentDensity), CultureInfo.GetCultureInfo("en-US"));
                    }
                    catch (Exception)
                    {
                        this.FilamentDensity = 0;
                    }
                    return true;
                }
                else
                {
                    IsWorking = true;
                    var res = await Parser.ParseAsync();
                    Commands = Parser.Model.Commands;
                    PrintTime = Parser.Model.PrintTime / 3600;
                    SlicerName = getSlicerName(Parser.Model.Slicer);
                    FilamentUsed = Parser.Model.TotalFilament;
                    ExtrudedFilamentVolume = (float)(((Math.PI * Math.Sqrt(this.Diameter)) / 4) * FilamentUsed / 1000);

                    IsValid = res;
                    IsWorking = false;
                    return res;
                }
            }
            catch(Exception exc)
            {
                IsWorking = false;
                return false;
                
            }
        }
        public async Task<bool> ReadGcodeAsync(IProgress<int> prog)
        {
            try
            {
                this.SlicerName = getSlicerName(Parser.Lines[0]);
                var slicers = Slicer.Slicer.SupportedSlicers.FirstOrDefault(slicer => slicer.SlicerName == this.SlicerName);
                
                IsWorking = true;
                var res = await Parser.ParseAsync(prog);

                Lines = Parser.Lines.Count;

                Commands = Parser.Model.Commands;
                PrintTime = Math.Round(Parser.Model.PrintTime / 3600, 2);
                
                Layers = Parser.Model.Layers;
                Width = Parser.Model.Width;
                Height = Parser.Model.Height;
                Depth = Parser.Model.Depth;

                FilamentUsed = Math.Round(Parser.Model.TotalFilament, 2);
                ExtrudedFilamentVolume = Math.Round((((Math.PI * this.Diameter * this.Diameter) / 4f) * FilamentUsed / 1000f), 2);
                
                IsValid = res;

                // If slicer is known, try to read the information from the comments.
                if (slicers != null && SettingsManager.Current.GcodeParser_PreferValuesInCommentsFromKnownSlicers)
                {
                    try
                    {
                        this.ExtrudedFilamentVolume = Convert.ToDouble(getParameterFromSlicer(SlicerParameter.Volume), CultureInfo.GetCultureInfo("en-US"));
                    }
                    catch (Exception)
                    {
                        this.ExtrudedFilamentVolume = 0;
                    }
                    // PrintTime
                    try
                    {
                        this.PrintTime = Convert.ToDouble(convertPrintTimeToDec(
                            getParameterFromSlicer(SlicerParameter.PrintTime)), CultureInfo.GetCultureInfo("en-US"));
                    }
                    catch (Exception)
                    {
                        this.PrintTime = 0;
                    }
                    // Diameter
                    try
                    {
                        this.Diameter = Convert.ToDouble(getParameterFromSlicer(SlicerParameter.FilamentDiameter), CultureInfo.GetCultureInfo("en-US"));
                    }
                    catch (Exception)
                    {
                        this.Diameter = 0;
                    }
                    // Nozzle
                    try
                    {
                        this.Diameter = Convert.ToDouble(getParameterFromSlicer(SlicerParameter.NozzleDiameter), CultureInfo.GetCultureInfo("en-US"));
                    }
                    catch (Exception)
                    {
                        this.Diameter = 0;
                    }
                    // FilamentType
                    try
                    {
                        this.FilamentType = getParameterFromSlicer(SlicerParameter.FilamentType);
                    }
                    catch (Exception)
                    {
                        this.FilamentType = Strings.Unknown;
                    }
                    // FilamentDensity
                    try
                    {
                        this.FilamentDensity = Convert.ToDouble(getParameterFromSlicer(SlicerParameter.FilamentDensity), CultureInfo.GetCultureInfo("en-US"));
                    }
                    catch (Exception)
                    {
                        this.FilamentDensity = 0;
                    }
                    IsValid = true;
                }
                
                IsWorking = false;
                return IsValid;
                /*
                else { 
                    IsWorking = true;
                    var res = await Parser.ParseAsync(prog);
                    Commands = Parser.Model.Commands;
                    PrintTime = Math.Round(Parser.Model.PrintTime / 3600, 2);
                    //SlicerName = getSlicerName(Parser.Model.Slicer);
                    FilamentUsed = Math.Round(Parser.Model.TotalFilament, 2);
                    ExtrudedFilamentVolume = Math.Round((((Math.PI * this.Diameter * this.Diameter) / 4f) * FilamentUsed / 1000f), 2);
                    IsValid = res;
                    IsWorking = false;
                    return res;
                }
                */
            }
            catch(Exception exc)
            {
                IsWorking = false;
                return false;
            }
        }

        public async Task<bool> Create3dGcodeModelAsync(IProgress<int> prog)
        {
            try
            {
                //await Parser.Create3dGcodeModelAsync(prog);
                return true;
            }
            catch (Exception exc)
            {
                IsWorking = false;
                return false;
            }
        }
        public async Task<bool> Get2dGcodeLayerModelListAsync(IProgress<int> prog)
        {
            try
            {
                var list = await Parser.Create2dGcodeLayerModelListAsync(prog);
                //var line = await Parser.Create2dGcodeLayerAsync(prog);
                ModelLayers = new List<LinesVisual3D>(list);
                return true;
            }
            catch (Exception exc)
            {
                IsWorking = false;
                return false;
            }
        }
        public async Task<bool> Get3dGcodeLayerModelAsync(IProgress<int> prog)
        {
            try
            {
                var list = await Parser.Create3dGcodeLayerModelListAsync();
                //var line = await Parser.Create2dGcodeLayerAsync(prog);
                Model3D = new List<LinesVisual3D>(list);
                return true;
            }
            catch (Exception exc)
            {
                IsWorking = false;
                return false;
            }
        }

        public async Task<LinesVisual3D> Get2dGcodeLayerAsync(int LayerNumber)
        {
            var lines = new LinesVisual3D();
            try
            {
                if (LayerNumber < ModelLayers.Count)
                {
                    lines = ModelLayers[LayerNumber];
                }
                else
                {
                    //lines = await drawLayerAsync(LayerNumber, 0, Model.Commands[LayerNumber].Count, false);
                }
            }
            catch (Exception exc)
            {

            }
            return lines;
        }
        #endregion

        #region Private Methods
        private SlicerName getSlicerName(string line)
        {
            try
            {
                string slicerLine = line;
                if (slicerLine.Contains("Slic3r"))
                    return SlicerName.Slic3r;
                else if (slicerLine.Contains("PrusaSlicer"))
                    return SlicerName.PrusaSlicer;
                else if (slicerLine.Contains("KISSlicer"))
                    return SlicerName.KISSlicer;
                else if (slicerLine.Contains("skeinforge"))
                    return SlicerName.Skeinforge;
                else if (slicerLine.Contains("CURA_PROFILE_STRING"))
                    return SlicerName.Cura;
                else if (slicerLine.Contains("Miracle"))
                    return SlicerName.Makerbot;
                else if (slicerLine.Contains("ffslicer"))
                    return SlicerName.FlashForge;
                else if (slicerLine.Contains("Simplify3D"))
                    return SlicerName.Simplify3D;
                else if (slicerLine.Contains("Snapmaker"))
                    return SlicerName.Snapmakerjs;
                else if (slicerLine.Contains("ideaMaker"))
                    return SlicerName.ideaMaker;
                else
                {
                    logger.Error(string.Format(Strings.EventUnknownSlicerFormated, slicerLine));
                    return SlicerName.Unkown;

                }
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return SlicerName.Unkown;
            }
        }

        private string getParameterFromSlicer(SlicerParameter Parameter)
        {
            try
            {
                Regex myregex;
                List<string> lines = new List<string>();
                switch (SlicerName)
                {
                    // Old Prusa Slicer
                    case SlicerName.Slic3r:
                        switch (Parameter)
                        {
                            case SlicerParameter.Volume:
                                myregex = new Regex(@"[;]\s*filament used\s*=\s*(\d*\.\d+)mm\s*[(](\d*\.\d+)cm3[)]");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"\(([^)]*)cm3\)").Groups[1].Value;
                            case SlicerParameter.PrintTime:
                                myregex = new Regex(@"[;]\s*estimated printing time \(normal mode\)\s*=*");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"((\d*h\s*\d*m\s\d*s)|(\d*m\s\d*s)|(\d{1,}s))").Groups[1].Value;
                            case SlicerParameter.PrintTimeSilent:
                                myregex = new Regex(@"[;]\s*estimated printing time \(silent mode\)\s*=*");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"((\d*h\s*\d*m\s\d*s)|(\d*m\s\d*s)|(\d{1,}s))").Groups[1].Value;
                            case SlicerParameter.FilamentDiameter:
                                myregex = new Regex(@"[;]\s*filament_diameter\s*=\s*\d*.\d*");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ =\s](\d*.\d\d))").Groups[1].Value;
                            case SlicerParameter.NozzleDiameter:
                                myregex = new Regex(@"[;]\s*nozzle_diameter\s*=\s*\d*.\d*");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ =\s](\d*.\d{1,2}))").Groups[1].Value;
                            case SlicerParameter.FilamentType:
                                myregex = new Regex(@"[;]\s*filament_type\s*=\s*([A-Z])*");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ =\s]([A-Z]\w))").Groups[1].Value;
                            case SlicerParameter.FilamentDensity:
                                myregex = new Regex(@"[;]\s*filament_density\s*=\s*\d*.\d*");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ =\s](\d*.\d{1,2}))|([^ =\s]([0-9]*$))").Groups[1].Value;
                            default:
                                return "Unkown parameter";
                        }
                    // New Prusa Slicer
                    case SlicerName.PrusaSlicer:
                        switch (Parameter)
                        {

                            case SlicerParameter.Volume:
                                myregex = new Regex(@"[;]\s*filament used\s*\[cm3\]\s*=\s*\d*.\d*");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"(\s\d*.\d)").Groups[1].Value;
                            case SlicerParameter.PrintTime:
                                //myregex = new Regex(@"[;]\s*estimated printing time \(normal mode\)\s*=\s*\d*h\s*\d*m\s*\d*s");
                                myregex = new Regex(@"[;]\s*estimated printing time \(normal mode\)\s*=*");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"((\d*d\s\d*h\s*\d*m\s\d*s)|(\d*h\s*\d*m\s\d*s)|(\d*m\s\d*s)|(\d{1,}s))").Groups[1].Value;
                            case SlicerParameter.PrintTimeSilent:
                                myregex = new Regex(@"[;]\s*estimated printing time \(silent mode\)\s*=*");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"((\d*d\s\d*h\s*\d*m\s\d*s)|(\d*h\s*\d*m\s\d*s)|(\d*m\s\d*s)|(\d{1,}s))").Groups[1].Value;
                            case SlicerParameter.FilamentDiameter:
                                myregex = new Regex(@"[;]\s*filament_diameter\s*=\s*\d*.\d*");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ =\s](\d*.\d\d))").Groups[1].Value;
                            case SlicerParameter.NozzleDiameter:
                                myregex = new Regex(@"[;]\s*nozzle_diameter\s*=\s*\d*.\d*");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ =\s](\d*.\d{1,2}))").Groups[1].Value;
                            case SlicerParameter.FilamentType:
                                myregex = new Regex(@"[;]\s*filament_type\s*=\s*([A-Z])*");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ =\s]([A-Z]\w))").Groups[1].Value;
                            case SlicerParameter.FilamentDensity:
                                myregex = new Regex(@"[;]\s*filament_density\s*=\s*\d*.\d*");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ =\s](\d*.\d{1,2}))").Groups[1].Value;
                            default:
                                return "Unkown parameter";
                        }
                    // Simplify3D
                    case SlicerName.Simplify3D:
                        switch (Parameter)
                        {
                            case SlicerParameter.Volume:
                                myregex = new Regex(@"([;]\s*Plastic volume:\s*)");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                //var s = Regex.Match(lines[0], @"(\d*.\d{1,2})\s*(cc)").Groups[1].Value;
                                return Regex.Match(lines[0], @"(\d*.\d{1,2})\s*(cc)").Groups[1].Value;
                            case SlicerParameter.PrintTime:
                                //myregex = new Regex(@"[;]\s*estimated printing time \(normal mode\)\s*=\s*\d*h\s*\d*m\s*\d*s");
                                myregex = new Regex(@"([;]\s*Build time:*)");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                //var s2 = Regex.Match(lines[0], @"((\d*\s(hour|hours)\s\d*\s(minutes|minute)\s\d*\s(seconds|second))|(\d*\s(hour|hours)\s\d*\s(minutes|minute))|(\d*\s(hour|hours))|((minutes|minute)\s\d*\s(seconds|second)))").Groups[1].Value;
                                return Regex.Match(lines[0], @"((\d*\s(hour|hours)\s\d*\s(minutes|minute)\s\d*\s(seconds|second))|(\d*\s(hour|hours)\s\d*\s(minutes|minute))|(\d*\s(hour|hours))|((minutes|minute)\s\d*\s(seconds|second)))").Groups[1].Value;
                            case SlicerParameter.PrintTimeSilent:
                                myregex = new Regex(@"([;]\s*Build time:*)");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                //var s2 = Regex.Match(lines[0], @"((\d*\s(hour|hours)\s\d*\s(minutes|minute)\s\d*\s(seconds|second))|(\d*\s(hour|hours)\s\d*\s(minutes|minute))|(\d*\s(hour|hours))|((minutes|minute)\s\d*\s(seconds|second)))").Groups[1].Value;
                                return Regex.Match(lines[0], @"((\d*\s(hour|hours)\s\d*\s(minutes|minute)\s\d*\s(seconds|second))|(\d*\s(hour|hours)\s\d*\s(minutes|minute))|(\d*\s(hour|hours))|((minutes|minute)\s\d*\s(seconds|second)))").Groups[1].Value;
                            case SlicerParameter.FilamentDiameter:
                                myregex = new Regex(@"[;]\s*filamentDiameters,\d*.\d*");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ ,](\d*.\d\d))").Groups[1].Value;
                            case SlicerParameter.NozzleDiameter:
                                myregex = new Regex(@"[;]\s*extruderDiameter,\d*.\d*");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ ,](\d*.\d\d))").Groups[1].Value;
                            case SlicerParameter.FilamentType:
                                myregex = new Regex(@"[;]\s*printMaterial,\w*");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                var s = Regex.Match(lines[0], @"(,)([^,]*)").Groups[2].Value;
                                return Regex.Match(lines[0], @"(,)([^,]*)").Groups[2].Value;
                            case SlicerParameter.FilamentDensity:
                                myregex = new Regex(@"[;]\s*filamentDensities,\d*.\d*");
                                lines = Parser.Lines.Where(line => !string.IsNullOrEmpty(line) && myregex.IsMatch(line)).ToList();
                                return Regex.Match(lines[0], @"([^ ,](\d*.\d\d))").Groups[1].Value;
                            default:
                                return "Unkown parameter";
                        }
                    default:
                        return "Slicer not tested yet";
                }

            }
            catch (Exception)
            {
                return "Error";
            }

        }

        private decimal convertPrintTimeToDec(string printTime)
        {
            try
            {
                string[] time = printTime.Split(' ');
                string timestring = string.Empty;
                if (time.Contains("hour") || time.Contains("hours")
                    || time.Contains("minutes") || time.Contains("minute")
                    || time.Contains("seconds") || time.Contains("second")
                    )
                {
                    string h = "0", m = "0", s = "0";
                    for (int i = 1; i < time.Length; i += 2)
                    {
                        if (time[i] == "hour" || time[i] == "hours")
                        {
                            h = time[i - 1];
                        }
                        else if (time[i] == "minutes" || time[i] == "minute")
                        {
                            m = time[i - 1];
                        }
                        else if (time[i] == "seconds" || time[i] == "second")
                        {
                            s = time[i - 1];
                        }
                    }
                    timestring = string.Format("{0}:{1}:{2}", h, m, s);
                }
                else
                {
                    switch (time.Count())
                    {
                        case 4:
                            timestring = string.Format("{0}:{1}:{2}:{3}",
                            time[0].Replace("d", string.Empty),
                            time[1].Replace("h", string.Empty),
                            time[2].Replace("m", string.Empty),
                            time[3].Replace("s", string.Empty)
                            );
                            break;
                        case 3:
                            timestring = string.Format("{0}:{1}:{2}",
                            time[0].Replace("h", string.Empty),
                            time[1].Replace("m", string.Empty),
                            time[2].Replace("s", string.Empty)
                            );
                            break;
                        case 2:
                            timestring = string.Format("{0}:{1}:{2}",
                            "0",
                            time[0].Replace("m", string.Empty),
                            time[1].Replace("s", string.Empty)
                            );
                            break;
                        case 1:
                            timestring = string.Format("{0}:{1}:{2}",
                            "0",
                            "0",
                            time[0].Replace("s", string.Empty)
                            );
                            break;
                        default:
                            throw new Exception(string.Format("Unknown time string format: {0}", printTime));
                    }
                }
                TimeSpan dt = TimeSpan.Parse(timestring);
                return Convert.ToDecimal(dt.TotalHours);
            }
            catch (Exception exc)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return -1m;
            }
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return this.FileName;
        }
        public override bool Equals(object obj)
        {
            var item = obj as GCode;
            if (item == null)
                return false;
            return this.Id.Equals(item.Id);
        }
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
        #endregion
    }
    public enum SlicerParameter
    {
        Volume,
        PrintTime,
        PrintTimeSilent,
        FilamentUsed,
        FilamentDiameter,
        FilamentType,
        FilamentDensity,
        NozzleDiameter,

    }
}
