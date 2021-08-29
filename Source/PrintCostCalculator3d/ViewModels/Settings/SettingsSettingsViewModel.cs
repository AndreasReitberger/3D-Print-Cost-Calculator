using MahApps.Metro.Controls.Dialogs;
using PrintCostCalculator3d.Models.Settings;
using PrintCostCalculator3d.Resources.Localization;
using PrintCostCalculator3d.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PrintCostCalculator3d.ViewModels
{
    public class SettingsSettingsViewModel : ViewModelBase
    {
        #region Variables
        readonly IDialogCoordinator _dialogCoordinator;
        #endregion

        #region Properties
        public Action CloseAction { get; set; }

        string _locationSelectedPath;
        public string LocationSelectedPath
        {
            get => _locationSelectedPath;
            set
            {
                if (value == _locationSelectedPath)
                    return;

                _locationSelectedPath = value;
                OnPropertyChanged();
            }
        }

        bool _movingFiles;
        public bool MovingFiles
        {
            get => _movingFiles;
            set
            {
                if (value == _movingFiles)
                    return;

                _movingFiles = value;
                OnPropertyChanged();
            }
        }

        bool _isPortable;
        public bool IsPortable
        {
            get => _isPortable;
            set
            {
                if (value == _isPortable)
                    return;

                if (!IsLoading)
                    MakePortable(value);

                _isPortable = value;
                OnPropertyChanged();
            }
        }

        bool _makingPortable;
        public bool MakingPortable
        {
            get => _makingPortable;
            set
            {
                if (value == _makingPortable)
                    return;

                _makingPortable = value;
                OnPropertyChanged();
            }
        }

        bool _resetEverything;
        public bool ResetEverything
        {
            get => _resetEverything;
            set
            {
                if (value == _resetEverything)
                    return;

                _resetEverything = value;
                OnPropertyChanged();
            }
        }

        bool _settingsExists;
        public bool SettingsExists
        {
            get => _settingsExists;
            set
            {
                if (value == _settingsExists)
                    return;

                _settingsExists = value;
                OnPropertyChanged();
            }
        }

        bool _resetSettings;
        public bool ResetSettings
        {
            get => _resetSettings;
            set
            {
                if (value == _resetSettings)
                    return;

                _resetSettings = value;
                OnPropertyChanged();
            }
        }

        bool _profilesExists;
        public bool ProfilesExists
        {
            get => _profilesExists;
            set
            {
                if (value == _profilesExists)
                    return;

                _profilesExists = value;
                OnPropertyChanged();
            }
        }

        bool _resetProfiles;
        public bool ResetProfiles
        {
            get => _resetProfiles;
            set
            {
                if (value == _resetProfiles)
                    return;

                _resetProfiles = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor, LoadSettings
        public SettingsSettingsViewModel(IDialogCoordinator instance)
        {
            _dialogCoordinator = instance;

            IsLoading = true;
            LoadSettings();
            IsLoading = false;
        }

        void LoadSettings()
        {
            LocationSelectedPath = SettingsManager.GetSettingsLocationNotPortable();
            IsPortable = SettingsManager.GetIsPortable();
        }
        #endregion

        #region ICommands & Actions
        public ICommand BrowseFolderCommand
        {
            get { return new RelayCommand(p => BrowseFolderAction()); }
        }

        void BrowseFolderAction()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (Directory.Exists(LocationSelectedPath))
                dialog.SelectedPath = LocationSelectedPath;

            var dialogResult = dialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
                LocationSelectedPath = dialog.SelectedPath;
        }

        public ICommand OpenLocationCommand
        {
            get { return new RelayCommand(p => OpenLocationAction()); }
        }

        static void OpenLocationAction()
        {
            Process.Start("explorer.exe", SettingsManager.GetSettingsLocation());
        }

        public ICommand ChangeSettingsCommand
        {
            get { return new RelayCommand(p => ChangeSettingsAction()); }
        }

        // Check if a file(name) is a settings file
        static bool FilesContainsSettingsFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);

                if (SettingsManager.GetSettingsFileName() == fileName)
                    return true;
                /*
                if (ProfileManager.ProfilesFileName == fileName)
                    return true;
                    */
            }

            return false;
        }

        async void ChangeSettingsAction()
        {
            MovingFiles = true;
            var overwrite = false;
            var forceRestart = false;

            var filesTargedLocation = Directory.GetFiles(LocationSelectedPath);

            // Check if there are any settings files in the folder...
            if (FilesContainsSettingsFiles(filesTargedLocation))
            {
                var settings = AppearanceManager.MetroDialog;

                settings.AffirmativeButtonText = Strings.Overwrite;
                settings.NegativeButtonText = Strings.Cancel;
                settings.FirstAuxiliaryButtonText = Strings.MoveAndRestart;
                settings.DefaultButtonFocus = MessageDialogResult.FirstAuxiliary;

                var result = await _dialogCoordinator.ShowMessageAsync(this, 
                    Strings.DialogOverwriteExistingSettingsHeadline, 
                    string.Format(Strings.DialogOverwriteExistingSettingsFormatedContent, filesTargedLocation), 
                    MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, AppearanceManager.MetroDialog);

                switch (result)
                {
                    case MessageDialogResult.Negative:
                        MovingFiles = false;
                        return;
                    case MessageDialogResult.Affirmative:
                        overwrite = true;
                        break;
                    case MessageDialogResult.FirstAuxiliary:
                        forceRestart = true;
                        break;
                }
            }

            // Try moving files (permissions, file is in use...)
            try
            {
                await SettingsManager.MoveSettingsAsync(SettingsManager.GetSettingsLocation(), LocationSelectedPath, overwrite, filesTargedLocation);

                Properties.Settings.Default.Settings_CustomSettingsLocation = LocationSelectedPath;

                // Show the user some awesome animation to indicate we are working on it :)
                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                var settings = AppearanceManager.MetroDialog;

                settings.AffirmativeButtonText = Strings.OK;
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, ex.TargetSite, ex.Message));
                await _dialogCoordinator.ShowMessageAsync(this, Strings.Error, ex.Message, MessageDialogStyle.Affirmative, settings);
            }

            LocationSelectedPath = string.Empty;
            LocationSelectedPath = Properties.Settings.Default.Settings_CustomSettingsLocation;

            if (forceRestart)
            {
                SettingsManager.ForceRestart = true;
                CloseAction();
            }

            MovingFiles = false;
        }

        public ICommand RestoreDefaultSettingsLocationCommand
        {
            get { return new RelayCommand(p => RestoreDefaultSettingsLocationAction()); }
        }

        void RestoreDefaultSettingsLocationAction()
        {
            LocationSelectedPath = SettingsManager.GetDefaultSettingsLocation();
        }

        public ICommand ResetSettingsCommand
        {
            get { return new RelayCommand(p => ResetSettingsAction()); }
        }

        public async void ResetSettingsAction()
        {
            var settings = AppearanceManager.MetroDialog;

            settings.AffirmativeButtonText = Strings.Continue;
            settings.NegativeButtonText = Strings.Cancel;

            settings.DefaultButtonFocus = MessageDialogResult.Affirmative;

            var message = Strings.SelectedSettingsAreReset;

            if (ResetEverything || ResetSettings)
            {
                message += Environment.NewLine + Environment.NewLine + $"* {Strings.TheSettingsLocationIsNotAffected}";
                message += Environment.NewLine + $"* {Strings.ApplicationIsRestartedAfterwards}";
            }

            if (await _dialogCoordinator.ShowMessageAsync(this, Strings.AreYouSure, message, MessageDialogStyle.AffirmativeAndNegative, settings) != MessageDialogResult.Affirmative)
                return;

            var forceRestart = false;

            if (SettingsExists && (ResetEverything || ResetSettings))
            {
                SettingsManager.Reset();
                forceRestart = true;
            }

            if (ProfilesExists && (ResetEverything || ResetProfiles))
            {
                //ProfileManager.Reset();
            }

            // Restart after reset or show a completed message
            if (forceRestart)
            {
                CloseAction();
            }
            else
            {
                settings.AffirmativeButtonText = Strings.OK;

                await _dialogCoordinator.ShowMessageAsync(this, 
                    Strings.DialogSettingsResetSucceededHeadline, 
                    Strings.DialogSettingsResetSucceededContent, 
                    MessageDialogStyle.Affirmative, settings
                    );
            }
        }
        #endregion

        #region Methods
        async void MakePortable(bool isPortable)
        {
            MakingPortable = true;

            // Save settings before moving them
            if (SettingsManager.Current.SettingsChanged)
                SettingsManager.Save();

            // Try moving files (permissions, file is in use...)
            try
            {
                await SettingsManager.MakePortableAsync(isPortable, true);

                Properties.Settings.Default.Settings_CustomSettingsLocation = string.Empty;
                LocationSelectedPath = SettingsManager.GetSettingsLocationNotPortable();

                // Show the user some awesome animation to indicate we are working on it :)
                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format(Strings.EventExceptionOccurredFormated, ex.TargetSite, ex.Message));
                await _dialogCoordinator.ShowMessageAsync(this, Strings.Error, ex.Message, MessageDialogStyle.Affirmative, AppearanceManager.MetroDialog);
            }

            MakingPortable = false;
        }

        public void SaveAndCheckSettings()
        {
            // Save everything
            if (SettingsManager.Current.SettingsChanged)
                SettingsManager.Save();

            // Check if files exist
            SettingsExists = File.Exists(SettingsManager.GetSettingsFilePath());
            //ProfilesExists = File.Exists(ProfileManager.GetProfilesFilePath());
        }

        public void SetLocationPathFromDragDrop(string path)
        {
            LocationSelectedPath = path;

            OnPropertyChanged(nameof(LocationSelectedPath));
        }
        #endregion
    }
}
