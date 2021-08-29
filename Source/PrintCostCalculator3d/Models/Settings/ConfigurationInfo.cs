﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PrintCostCalculator3d.Models.Settings
{
    public class ConfigurationInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsAdmin { get; set; }
        public string ExecutionPath { get; set; }
        public string ApplicationFullName { get; set; }
        public string ApplicationName { get; set; }
        public Version OSVersion { get; set; }
        public bool IsTransparencyEnabled { get; set; }
        public bool ShowSettingsResetNoteOnStartup { get; set; }

        bool _isDialogOpen;
        public bool FixAirspace
        {
            get => _isDialogOpen;
            set
            {
                if (value == _isDialogOpen)
                    return;

                _isDialogOpen = value;
                OnPropertyChanged();
            }
        }

        public ConfigurationInfo()
        {

        }
    }
}
