using HelixToolkit.Wpf;
using IxMilia.Stl;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;
using WpfFramework.Resources.Localization;

namespace WpfFramework.Models
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
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        public string FileName
        {
            get
            {
                return (string.IsNullOrEmpty(this.StlFilePath) ? string.Empty : Path.GetFileName(StlFilePath));
            }
        }

        private string _stlFilePath = string.Empty;
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

        private StlFile _stlFile;
        [XmlIgnore]
        public StlFile StlFile
        {
            get
            {
                if (_stlFile == null && StlFilePath != null)
                {
                    this._stlFile = readStlFile(StlFilePath);
                }
                return _stlFile; 
            }
            /*
            set
            {
                if (!value.Equals(_stlFile))
                {
                    _stlFile = value;
                    _stlFile.Save(StlFileStream);
                    StlFileStream.Position = 0;
                }
            }
            */
        }

        private Model3DGroup _model;
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

        private double _volume = 0;
        public double Volume
        {
            //get { return calculateVolume(); }
            get => _volume;
            private set
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
        public Stl() { 
            
        }
        /// <summary>Initializes a new instance of the <see cref="Stl"/> class.</summary>
        /// <param name="path">The path to the stl file</param>
        public Stl(string path)
        {
            StlFilePath = path;
            Volume = calculateVolume();
            Model = Load(StlFilePath);
        }
        /// <summary>Reads in a STL file.</summary>
        /// <param name="path">The path.</param>
        /// <returns>The stl file</returns>
        public StlFile readStlFile(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(@path, FileMode.Open))
                {
                    StlFile stl = StlFile.Load(fs);
                    stl.SolidName = path.Substring(path.LastIndexOf("\\") + 1);
                    return stl;
                }
            }
            catch(Exception)
            {
                return null;
            }
        }

        public ModelVisual3D get3dVisual()
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

        public bool downloadStlFile(string dir)
        {
            try
            {
                string path = dir;
                if (dir[dir.Length - 1] != '\\')
                    path += @"\";
                path += StlFile.SolidName;
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    this.StlFile.Save(fs);
                    return true;
                }
            }
            catch (Exception exc)
            {
                return false;
            }
        }
        /// <summary>Writes the STL file to the specified path.</summary>
        /// <param name="file">The stl file to be written.</param>
        /// <param name="dir">The path of the directory.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public bool writeStlFile(StlFile file ,string dir, string fileName)
        {
            try
            {
                using (FileStream fs = new FileStream(string.Format("{0}" + (dir[dir.Length - 1].ToString() != "\\" ? "\\{1}" : "{1}"), dir, fileName), FileMode.Open))
                {
                    file.Save(fs);
                    return true;
                }
            }
            catch(Exception)
            {
                return false;
            }
        }

        public static string copyStlFile(string source, string requester)
        {
            string targetFolder = "Uploads\\" + requester;
            string appFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo tempDir = new DirectoryInfo(appFolder + targetFolder);
            Directory.CreateDirectory(tempDir.FullName);
            targetFolder = tempDir.FullName + "\\" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + "_" + source.Substring(source.LastIndexOf("\\") + 1);
            File.Copy(source, targetFolder);

            return targetFolder;
        }

        /// <summary>Calculates the volume of the stl file.</summary>
        /// <returns>The volume in mm³</returns>
        private double calculateVolume()
        {
            List<StlTriangle> triangles = StlFile.Triangles;
            var vols = from t in StlFile.Triangles
                       select SignedVolumeOfTriangle(t.Vertex1, t.Vertex2, t.Vertex3);
            return Math.Abs(vols.Sum());
        }

        /// <summary>  Gets the volume of a triangle.</summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <returns></returns>
        private float SignedVolumeOfTriangle(StlVertex p1, StlVertex p2, StlVertex p3)
        {
            var v321 = p3.X * p2.Y * p1.Z;
            var v231 = p2.X * p3.Y * p1.Z;
            var v312 = p3.X * p1.Y * p2.Z;
            var v132 = p1.X * p3.Y * p2.Z;
            var v213 = p2.X * p1.Y * p3.Z;
            var v123 = p1.X * p2.Y * p3.Z;
            return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
        }
        private static Model3DGroup Load(string path)
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
        private void createStlModel(string StlFilePath)
        {
            try
            {
                Model = Load(StlFilePath);
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
        private MemoryStream m_value = new MemoryStream();

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
