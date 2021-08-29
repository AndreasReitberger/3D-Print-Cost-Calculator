using HelixToolkit.Wpf;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;
using PrintCostCalculator3d.Resources.Localization;

namespace PrintCostCalculator3d.Models
{
    public class Stl : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Variables
        static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        Guid _id = Guid.Empty;
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

        public string FileName
        {
            get
            {
                return (string.IsNullOrEmpty(this.StlFilePath) ? string.Empty : Path.GetFileName(StlFilePath));
            }
        }

        string _stlFilePath = string.Empty;
        public string StlFilePath
        {
            get => _stlFilePath;
            set
            {
                if(_stlFilePath != value)
                {
                    _stlFilePath = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }

        Model3DGroup _model;
        public Model3DGroup Model
        {
            get => _model;
            set
            {
                if (_model != value)
                {
                    _model = value;
                    OnPropertyChanged();
                }
            }
        }

        double _volume = 0;
        public double Volume
        {
            //get { return calculateVolume(); }
            get => _volume;
            set
            {
                if (_volume != value)
                {
                    _volume = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        /// <summary>Initializes a new instance of the <see cref="Stl"/> class.</summary>
        public Stl() 
        {
            Id = Guid.NewGuid();
        }
        /// <summary>Initializes a new instance of the <see cref="Stl"/> class.</summary>
        /// <param name="path">The path to the stl file</param>
        public Stl(string path)
        {
            Id = Guid.NewGuid();
            StlFilePath = path;
            //Model = Load(StlFilePath);
        }

        public ModelVisual3D Get3dVisual()
        {
            ModelVisual3D mod = new ModelVisual3D();
            try
            {
                if (Model == null)
                    return mod;
                mod = new ModelVisual3D() { Content = Model };
                return mod;
            }
            catch(Exception exc)
            {
                logger.ErrorFormat(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
                return mod;
            }
        }

        public static string CopyStlFile(string source, string requester)
        {
            string targetFolder = "Uploads\\" + requester;
            string appFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo tempDir = new DirectoryInfo(appFolder + targetFolder);
            Directory.CreateDirectory(tempDir.FullName);
            targetFolder = tempDir.FullName + "\\" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + "_" + source.Substring(source.LastIndexOf("\\") + 1);
            File.Copy(source, targetFolder);

            return targetFolder;
        }

        static Model3DGroup Load(string path)
        {
            if (path == null)
            {
                return null;
            }

            Model3DGroup model = null;
            string ext = Path.GetExtension(path).ToLower();
            switch (ext)
            {
                case ".3ds":
                    {
                        var r = new StudioReader();
                        model = r.Read(path);
                        break;
                    }

                case ".lwo":
                    {
                        var r = new LwoReader();
                        model = r.Read(path);

                        break;
                    }

                case ".obj":
                    {
                        var r = new ObjReader();
                        model = r.Read(path);
                        break;
                    }

                case ".objz":
                    {
                        var r = new ObjReader();
                        model = r.ReadZ(path);
                        break;
                    }

                case ".stl":
                    {
                        var r = new StLReader();
                        model = r.Read(path);
                        break;
                    }

                case ".off":
                    {
                        var r = new OffReader();
                        model = r.Read(path);
                        break;
                    }

                default:
                    throw new InvalidOperationException("File format not supported.");
            }

            return model;
        }
        void CreateStlModel(string StlFilePath)
        {
            try
            {
                //Model = Load(StlFilePath);
            }
            catch (Exception exc)
            {
                logger.ErrorFormat(string.Format(Strings.EventExceptionOccurredFormated, exc.TargetSite, exc.Message));
            }
        }

        #region Override
        public override string ToString()
        {
            return FileName;
        }
        #endregion
    }
    public class XmlMemoryStream
    {
        MemoryStream m_value = new MemoryStream();

        public XmlMemoryStream() { }
        public XmlMemoryStream(MemoryStream source) { m_value = source; }

        /*
        public static implicit operator MemoryStream? (XmlMemoryStream o)
        {
            return o == null ? default(MemoryStream?) : o.m_value;
        }

        public static implicit operator XmlMemoryStream(MemoryStream? o)
        {
            return o == null ? null : new XmlMemoryStream(o.Value);
        }
        */
        public static implicit operator MemoryStream(XmlMemoryStream o)
        {
            return o == null ? default(MemoryStream) : o.m_value;
        }

        public static implicit operator XmlMemoryStream(MemoryStream o)
        {
            return o == default(MemoryStream) ? null : new XmlMemoryStream(o);
        }

        [XmlArray]
        public byte[] Default
        {
            get {
                return m_value.ToArray();
            }
            set { m_value = new MemoryStream(value, true); }
        }
    }
}
