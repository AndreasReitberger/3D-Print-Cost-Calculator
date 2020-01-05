using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MahApps.Metro.IconPacks;

namespace WpfFramework.Models
{
    public class _File
    {
        #region Properties
        public string FileName
        { get; set; }

        public string FilePath
        { get; set; }

        public int Matches
        {
            get => _matches;
            set => _matches = value;
        }
        public MaterialDesignExtension Icon
        {
            get
            {
                switch(State)
                {
                    case FileState.Fresh:
                        return _iconNew;
                    case FileState.CheckCompleted:
                        return _iconCheckCompleted;
                    case FileState.ReplacementCompleted:
                        return _iconRunCompleted;
                    default:
                        return _iconNew;
                }
            }
        }

        public FileState State
        {
            get => _state;
            set => _state = value;
        }
        #endregion

        #region Private Properties
        private FileState _state = FileState.Fresh;

        private int _matches
        { get; set; }

        private MaterialDesignExtension _iconNew = new MaterialDesignExtension(PackIconMaterialDesignKind.NewReleases)
        {
            Height = 32,
            Width = 32,
        };
        private MaterialDesignExtension _iconCheckCompleted = new MaterialDesignExtension(PackIconMaterialDesignKind.Check);
        private MaterialDesignExtension _iconRunCompleted = new MaterialDesignExtension(PackIconMaterialDesignKind.DoneAll);
        #endregion

        #region Constructor
        public _File() { }

        public _File(string FileName, string Path)
        {
            this.FileName = FileName;
            this.FilePath = Path;
        }
        #endregion
    }

    public enum FileState
    {
        Fresh,
        CheckCompleted,
        ReplacementCompleted,
        Error,
    }
}
