using System;

namespace PrintCostCalculator3d.Models.Update
{
    public class UpdateAvailableArgs : EventArgs
    {
        public Version Version { get; set; }

        public UpdateAvailableArgs(Version version)
        {
            Version = version;
        }
    }
}
