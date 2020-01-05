using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfFramework.Models._3dprinting
{
    public class _3dFile
    {
        #region Properties
        public string FileName
        { get; set; }

        public string FilePath
        { get; set; }

        public MaterialExtension Icon
        {
            get
            {
                switch (Type)
                {
                    case FileType.Gcode:
                        return _iconGcode;
                    case FileType.Stl:
                        return _iconStl;
                    case FileType.CalculationFile:
                        return _iconCalc;
                    default:
                        return _iconCalc;
                }
            }
        }

        public FileType Type
        {
            get => _state;
            set => _state = value;
        }
        #endregion

        #region Private Properties
        private FileType _state = FileType.Gcode;

        private int _matches
        { get; set; }

        private MaterialExtension _iconStl = new MaterialExtension(PackIconMaterialKind.File)
        {
            Height = 32,
            Width = 32,
        };
        private MaterialExtension _iconGcode = new MaterialExtension(PackIconMaterialKind.File);
        private MaterialExtension _iconCalc = new MaterialExtension(PackIconMaterialKind.File);
        #endregion

        #region Constructor
        public _3dFile() { }

        public _3dFile(string FileName, string Path)
        {
            this.FileName = FileName;
            this.FilePath = Path;
        }
        #endregion
    }
    public enum FileType
    {
        Gcode,
        Stl,
        CalculationFile,
    }
}
