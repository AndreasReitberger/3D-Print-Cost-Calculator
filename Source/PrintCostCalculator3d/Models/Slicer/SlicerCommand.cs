using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintCostCalculator3d.Models.Slicer
{
    public class SlicerCommand
    {
        #region Properties
        public string Name { get; set; }
        public Slicer Slicer { get; set; }
        public string Command { get; set; }
        public string OutputFilePatternString { get; set; }
        public bool AutoAddFilePath { get; set; }
        #endregion

        #region Constructor
        public SlicerCommand() { }
        #endregion

        #region Overrides
        public override string ToString()
        {
            //return string.Format("{0} ({1})", this.Command, this.Slicer);
            return string.Format("{0}", this.Command);
        }

        public override bool Equals(object obj)
        {
            var item = obj as SlicerCommand;
            if (item == null)
                return false;
            return (this.Slicer == item.Slicer && this.Name == item.Name);
        }
        #endregion
    }
}
