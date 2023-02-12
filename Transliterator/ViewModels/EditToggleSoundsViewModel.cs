using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;

// TODO: Uncomment after migrating other things from old project

// using Transliterator.Services;

namespace Transliterator.ViewModels
{
    public partial class EditToggleSoundsViewModel : ObservableObject
    {
        // TODO: Uncomment after migrating other things from old project

        // private readonly SettingsService settingsService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ToggleOnSoundFileName))]
        private string toggleOnSoundFilePath;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ToggleOffSoundFileName))]
        private string toggleOffSoundFilePath;

        public string ToggleOnSoundFileName { get => Path.GetFileName(ToggleOnSoundFilePath) ?? "<None>"; }

        public string ToggleOffSoundFileName { get => Path.GetFileName(ToggleOffSoundFilePath) ?? "<None>"; }

        public EditToggleSoundsViewModel()
        {
            // TODO: Uncomment after migrating other things from old project

            //settingsService = SettingsService.GetInstance();
            //ToggleOnSoundFilePath = settingsService.PathToCustomToggleOnSound;
            //ToggleOffSoundFilePath = settingsService.PathToCustomToggleOffSound;
        }

        [RelayCommand]
        private void ChangeToggleOnSound()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Audio documents (.wav)|*.wav" // Filter files by extension
            };

            bool? result = dialog.ShowDialog();

            if (result != null && result == true)
            {
                // Open document
                string pathToFile = dialog.FileName;
                ToggleOnSoundFilePath = pathToFile;
                //settingsService.PathToCustomToggleOnSound = pathToFile;
            }
        }

        // TODO: Uncomment after migrating other things from old project
        [RelayCommand]
        private void ChangeToggleOffSound()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Audio documents (.wav)|*.wav" // Filter files by extension
            };

            bool? result = dialog.ShowDialog();

            if (result != null && result == true)
            {
                // Open document
                string pathToFile = dialog.FileName;
                ToggleOffSoundFilePath = pathToFile;
                //settingsService.PathToCustomToggleOffSound = pathToFile;
            }
        }

        // TODO: Uncomment after migrating other things from old project

        [RelayCommand]
        private void DeleteToggleOnSound()
        {
            ToggleOnSoundFilePath = null;
            //settingsService.PathToCustomToggleOnSound = null;
        }

        [RelayCommand]
        private void DeleteToggleOffSound()
        {
            ToggleOffSoundFilePath = null;
            //settingsService.PathToCustomToggleOffSound = null;
        }
    }
}