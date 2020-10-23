using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintCostCalculator3d.Resources.Localization;

namespace PrintCostCalculator3d.Models.Slicer
{
    public class SlicerActions
    {
        //https://manual.slic3r.org/advanced/command-line
        #region Properties
        public static List<SlicerActions> Commands = new List<SlicerActions>()
        {
            // slic3r [ ACTION ] [ OPTIONS ] [ model1.stl model2.stl ... ]
            new SlicerActions() { Slicers = new SlicerName[] { SlicerName.Slic3r, SlicerName.PrusaSlicer}, Action = "-help" },
            new SlicerActions() { Slicers = new SlicerName[] { SlicerName.Slic3r, SlicerName.PrusaSlicer}, Action = "-gcode" },
            new SlicerActions() { Slicers = new SlicerName[] { SlicerName.Slic3r, SlicerName.PrusaSlicer}, Action = "-export-stl" },
            new SlicerActions() { Slicers = new SlicerName[] { SlicerName.Slic3r, SlicerName.PrusaSlicer}, Action = "-export-amf" },
            new SlicerActions() { Slicers = new SlicerName[] { SlicerName.Slic3r, SlicerName.PrusaSlicer}, Action = "-export-3mf" },
            new SlicerActions() { Slicers = new SlicerName[] { SlicerName.Slic3r, SlicerName.PrusaSlicer}, Action = "-export-obj" },
            new SlicerActions() { Slicers = new SlicerName[] { SlicerName.Slic3r, SlicerName.PrusaSlicer}, Action = "-export-pov" },
            new SlicerActions() { Slicers = new SlicerName[] { SlicerName.Slic3r, SlicerName.PrusaSlicer}, Action = "-export-svg" },
            new SlicerActions() { Slicers = new SlicerName[] { SlicerName.Slic3r, SlicerName.PrusaSlicer}, Action = "-sla" },
            new SlicerActions() { Slicers = new SlicerName[] { SlicerName.Slic3r, SlicerName.PrusaSlicer}, Action = "-info" },
            new SlicerActions() { Slicers = new SlicerName[] { SlicerName.Slic3r, SlicerName.PrusaSlicer}, Action = "" },
            new SlicerActions() { Slicers = new SlicerName[] { SlicerName.Slic3r, SlicerName.PrusaSlicer}, Action = "" },
        };
        public string Action
        { get; set; }
        
        public SlicerName[] Slicers
        { get; set; }

        #endregion

        #region Constructor
    public SlicerActions() { }
        #endregion
    }
    public class Slicer
    {
        #region Static 
        public static List<Slicer> SupportedSlicers = new List<Slicer>()
        {
            new Slicer() { SlicerName = SlicerName.Slic3r },
            new Slicer() { SlicerName = SlicerName.PrusaSlicer },
            new Slicer() { SlicerName = SlicerName.Simplify3D },
            new Slicer() { SlicerName = SlicerName.Voxelizer2 },
        };

        #endregion

        #region Properties
        public Guid Id
        { get; set; }
        public SlicerName SlicerName { get; set;}
        
        public SlicerViewManager.Group Group { get; set;}

        public string InstallationPath
        { get; set; }

        public string DownloadUri
        { get; set; }
        #endregion

        #region Constructor 
        public Slicer() { }
        #endregion

        #region Override
        public override string ToString()
        {
            return string.Format("{0} ({1})", SlicerName, Group) ;
        }
        public override bool Equals(object obj)
        {
            var item = obj as Slicer;
            if (item == null)
                return false;
            return this.Id.Equals(item.Id) || (this.Group == item.Group && this.SlicerName == item.SlicerName);
        }
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
        #endregion
    }

    public enum SlicerName
    {
        [LocalizedDescription("Unknown", typeof(Strings))]
        Unkown,
        [LocalizedDescription("Slic3r", typeof(Strings))]
        Slic3r,
        [LocalizedDescription("PrusaSlicer", typeof(Strings))]
        PrusaSlicer,
        [LocalizedDescription("KISSlicer", typeof(Strings))]
        KISSlicer,
        [LocalizedDescription("Skeinforge", typeof(Strings))]
        Skeinforge,
        [LocalizedDescription("Cura", typeof(Strings))]
        Cura,
        [LocalizedDescription("Makerbot", typeof(Strings))]
        Makerbot,
        [LocalizedDescription("FlashForge", typeof(Strings))]
        FlashForge,
        [LocalizedDescription("Simplify3D", typeof(Strings))]
        Simplify3D,
        [LocalizedDescription("Snapmakerjs", typeof(Strings))]
        Snapmakerjs,
        [LocalizedDescription("ideaMaker", typeof(Strings))]
        ideaMaker,
        [LocalizedDescription("Voxelizer2", typeof(Strings))]
        Voxelizer2,
    }
}
