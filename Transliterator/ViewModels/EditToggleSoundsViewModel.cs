using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using Transliterator.Core.Helpers;
using Transliterator.Services;

namespace Transliterator.ViewModels;

public partial class EditToggleSoundsViewModel : ObservableObject
{
    private readonly SettingsService settingsService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ToggleOffSoundFileName))]
    private string toggleOffSoundFilePath;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ToggleOnSoundFileName))]
    private string toggleOnSoundFilePath;

    public EditToggleSoundsViewModel()
    {
        //settingsService = Singleton<SettingsService>.Instance;
        // TODO: Use singleton wrapper
        settingsService = SettingsService.GetInstance();
        ToggleOnSoundFilePath = settingsService.PathToCustomToggleOnSound;
        ToggleOffSoundFilePath = settingsService.PathToCustomToggleOffSound;
    }

    public string ToggleOffSoundFileName { get => Path.GetFileName(ToggleOffSoundFilePath) ?? "<None>"; }
    public string ToggleOnSoundFileName { get => Path.GetFileName(ToggleOnSoundFilePath) ?? "<None>"; }

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
            settingsService.PathToCustomToggleOffSound = pathToFile;
        }
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
            settingsService.PathToCustomToggleOnSound = pathToFile;
        }
    }

    [RelayCommand]
    private void DeleteToggleOffSound()
    {
        ToggleOffSoundFilePath = null;
        settingsService.PathToCustomToggleOffSound = null;
    }

    [RelayCommand]
    private void DeleteToggleOnSound()
    {
        ToggleOnSoundFilePath = null;
        settingsService.PathToCustomToggleOnSound = null;
    }
}