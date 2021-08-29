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
            return string.Format("{0}", Command);
        }

        public override bool Equals(object obj)
        {
            if (obj is not SlicerCommand item)
                return false;
            return (Slicer == item.Slicer && Name == item.Name);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
