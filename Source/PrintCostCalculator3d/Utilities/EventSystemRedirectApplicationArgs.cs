using System;

namespace PrintCostCalculator3d.Utilities
{
    public class EventSystemRedirectApplicationArgs : EventArgs
    {
        public ApplicationName Application { get; set; }
        public string Args { get; set; }

        public EventSystemRedirectApplicationArgs(ApplicationName application, string args)
        {
            Application = application;
            Args = args;
        }
    }
}
