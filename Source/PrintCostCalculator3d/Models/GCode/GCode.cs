using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PrintCostCalculator3d.Models.GCode.Helper;
using PrintCostCalculator3d.Models.Slicer;

namespace PrintCostCalculator3d.Models.GCode
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
            set
            {
                if (_isValid == value)
                    return;
                _isValid = value;
                OnPropertyChanged();
            }
        }
        
        private bool _isOctoPrintGcodeAnalysis = false;
        public bool IsOctoPrintGcodeAnalysis
        {
            get => _isOctoPrintGcodeAnalysis;
            set
            {
                if (_isOctoPrintGcodeAnalysis == value)
                    return;
                _isOctoPrintGcodeAnalysis = value;
                OnPropertyChanged();
            }
        }

        private bool _isWorking = false;
        public bool IsWorking
        {
            get => _isWorking;
            set
            {
                if (_isWorking == value)
                    return;
                _isWorking = value;
                OnPropertyChanged();
            }
        }

        private int _progress = 0;
        public int Progress
        {
            get => _progress;
            set
            {
                if (_progress == value)
                    return;
                _progress = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region GcodeParser
        private TimeSpan _parsingDuration;
        public TimeSpan ParsingDuration
        {
            get => _parsingDuration;
            set
            {
                if (_parsingDuration == value) return;
                _parsingDuration = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<float, int> _zHeights = new Dictionary<float, int>();
        public Dictionary<float, int> ZHeights
        {
            get => _zHeights;
            set
            {
                if (_zHeights == value)
                    return;
                _zHeights = value;
                OnPropertyChanged();
            }
        }


        #endregion

        #region GcodeInformation
        private SlicerName _slicerName = SlicerName.Unkown;
        public SlicerName SlicerName
        {
            get => _slicerName;
            set
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
        private bool _layerModelGenerated = false;
        public bool LayerModelGenerated
        {
            get => _layerModelGenerated;
            set
            {
                if (value == _layerModelGenerated)
                    return;
                _layerModelGenerated = value;
                OnPropertyChanged();
            }
        }

        private double _filamentUsed = 0;
        /// <summary>Gets the filament used in "mm"</summary>
        /// <value>The filament used.</value>
        public double FilamentUsed
        {
            get => _filamentUsed;
            set
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
            set
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
            set
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
        
        private double _NozzleDiameter;
        public double NozzleDiameter
        {
            get => _NozzleDiameter;
            set
            {
                if (_NozzleDiameter == value)
                    return;
                _NozzleDiameter = value;
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

        private double _filamentWeight;
        public double FilamentWeight
        {
            get => _filamentWeight;
            set
            {
                if (_filamentWeight == value)
                    return;
                _filamentWeight = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region GcodeCommands
        private List<List<GCodeCommand>> _commands = new List<List<GCodeCommand>>();
        public List<List<GCodeCommand>> Commands
        {
            get => _commands;
            set
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
            set
            {
                if (_modelLayers == value)
                    return;
                _modelLayers = value;
                OnPropertyChanged();
            }
        }
        
        private List<LineBuilder> _model3d =  new List<LineBuilder>();
        public List<LineBuilder> Model3D
        {
            get => _model3d;
            set
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

        }
        #endregion

        #region Public Methods
        /*
        public async Task<bool> ReadOctoPrintGcodeAnalysisAsync(OctoPrintFile file)
        {
            try
            {
                if(file.GcodeAnalysis == null)
                    return false;

                var volume = Math.Round(file.GcodeAnalysis.Filament.Select(tool => tool.Value).Sum(filament => filament.Volume), 2);
                var length = Math.Round(file.GcodeAnalysis.Filament.Select(tool => tool.Value).Sum(filament => filament.Length), 2);

                this.Width = (float)Math.Round(file.GcodeAnalysis.Dimensions.Width, 2);
                this.Height = (float)Math.Round(file.GcodeAnalysis.Dimensions.Height, 2);
                this.Depth = (float)Math.Round(file.GcodeAnalysis.Dimensions.Depth, 2);

                if(file.Statistics != null)
                {
                    if(file.Statistics.AveragePrintTime != null && file.Statistics.AveragePrintTime.Default > 0)
                        this.PrintTime = Math.Round(file.Statistics.AveragePrintTime.Default / 3600, 2);
                    else
                        this.PrintTime = Math.Round(file.GcodeAnalysis.EstimatedPrintTime / 3600, 2);
                }
                else 
                    this.PrintTime = Math.Round(file.GcodeAnalysis.EstimatedPrintTime / 3600, 2);

                this.ExtrudedFilamentVolume = volume;
                this.FilamentUsed = length;

                IsValid = true;
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
        */
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
        public async Task<LineBuilder> GetGcodeLayerLineBuilderAsync(int LayerNumber)
        {
            var lineBuilder = new LineBuilder();
            try
            {
                if (LayerNumber < Model3D.Count)
                {
                    lineBuilder = Model3D[LayerNumber];
                }
                else
                {
                    //lines = await drawLayerAsync(LayerNumber, 0, Model.Commands[LayerNumber].Count, false);
                }
            }
            catch (Exception exc)
            {

            }
            return lineBuilder;
        }
        #endregion

        #region Private Methods
        private void OnProgressUpdateAction(int progress)
        {
            Progress = progress;
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
