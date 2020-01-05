﻿using System;

namespace WpfFramework.Models.Update
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
