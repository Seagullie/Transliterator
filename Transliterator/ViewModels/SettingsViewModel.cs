using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading;
using System.Threading.Tasks;
using Transliterator.Core.Helpers;
using Transliterator.Core.Models;
using Transliterator.Services;
using Transliterator.Views;

namespace Transliterator.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    // TODO: Uncomment after migrating more things from old project
    //private readonly Main liveTransliterator;

    private readonly TransliteratorSettingsService _settingsService;

    [ObservableProperty]
    private bool isAltShiftGlobalShortcutEnabled;

    [ObservableProperty]
    private bool isAutoStartEnabled;

    [ObservableProperty]
    private bool isBufferInputEnabled;

    [ObservableProperty]
    private bool isMinimizedStartEnabled;

    [ObservableProperty]
    private bool isStateOverlayEnabled;

    [ObservableProperty]
    private bool isToggleSoundOn;

    [ObservableProperty]
    private bool isTranslitEnabledAtStartup;

    [ObservableProperty]
    private string showcaseText;

    [ObservableProperty]
    private HotKey toggleHotKey;

    [ObservableProperty]
    private string toggleTranslitShortcut;

    public SettingsViewModel()
    {
        // TODO: Dependency injection
        // TODO: Uncomment after migrating more things from old project
        //liveTransliterator = Main.GetInstance();
        _settingsService = Singleton<TransliteratorSettingsService>.Instance;

        InitializePropertiesFromSettings();
    }

    public void InitializePropertiesFromSettings()
    {
        // TODO: Uncomment after migrating more things from old project
        _settingsService.Load();
        IsAltShiftGlobalShortcutEnabled = _settingsService.IsAltShiftGlobalShortcutEnabled;
        IsAutoStartEnabled = _settingsService.IsAutoStartEnabled;
        IsBufferInputEnabled = _settingsService.IsBufferInputEnabled;
        IsMinimizedStartEnabled = _settingsService.IsMinimizedStartEnabled;
        IsStateOverlayEnabled = _settingsService.IsStateOverlayEnabled;
        IsToggleSoundOn = _settingsService.IsToggleSoundOn;
        IsTranslitEnabledAtStartup = _settingsService.IsTransliteratorEnabledAtStartup;
        ToggleHotKey = _settingsService.ToggleHotKey;
    }

    [RelayCommand]
    private static void OpenEditToggleSoundsWindow()
    {
        EditToggleSoundsWindow editToggleSoundsWindow = new();
        editToggleSoundsWindow.ShowDialog();
    }

    [RelayCommand]
    private static void OpenTranslitTablesWindow()
    {
        // TODO: Rewrite to NavigateToTranslitTablesPage or prevent the creation of multiple windows
        // TODO: Uncomment after migrating more things from old project
        //TranslitTablesWindow translitTables = new();
        //translitTables.Show();
    }

    // TODO: Show success message via snackbar
    [RelayCommand]
    private void ApplyChanges()
    {
        // TODO: Uncomment after migrating more things from old project

        _settingsService.IsAltShiftGlobalShortcutEnabled = IsAltShiftGlobalShortcutEnabled;
        _settingsService.IsAutoStartEnabled = IsAutoStartEnabled;
        _settingsService.IsBufferInputEnabled = IsBufferInputEnabled;
        _settingsService.IsMinimizedStartEnabled = IsMinimizedStartEnabled;
        _settingsService.IsStateOverlayEnabled = IsStateOverlayEnabled;
        _settingsService.IsToggleSoundOn = IsToggleSoundOn;
        _settingsService.IsTransliteratorEnabledAtStartup = IsTranslitEnabledAtStartup;
        _settingsService.ToggleHotKey = ToggleHotKey;
        _settingsService.Save();
    }

    // TODO: not trigger on window init
    partial void OnIsBufferInputEnabledChanged(bool value)
    {
        // TODO: Uncomment after migrating more things from old project
        //liveTransliterator.displayCombos = value;

        // TODO: Add DebugService
        //if (debugWindow != null && debugWindow.underTestByWinDriverCheckBox.IsChecked == true)
        //{
        //    return;
        //}
        //else
        //{
        if (ShowcaseBufferInputEnabledModeCommand.IsRunning)
            ShowcaseBufferInputEnabledModeCommand.Cancel();

        if (ShowcaseBufferInputDisabledModeCommand.IsRunning)
            ShowcaseBufferInputDisabledModeCommand.Cancel();

        ShowcaseText = "";

        if (value)
            ShowcaseBufferInputEnabledModeCommand.ExecuteAsync(null);
        else
            ShowcaseBufferInputDisabledModeCommand.ExecuteAsync(null);
        //}
    }

    [RelayCommand]
    private async Task ShowcaseBufferInputDisabledMode(CancellationToken cancellationToken)
    {
        const string showcaseString = "^a~o`i\"u";
        const string showcaseString2 = "âõíü";

        for (int i = 0; i < showcaseString.Length; i++)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            ShowcaseText += showcaseString[i];
            await Task.Delay(300);
            if ((i + 1) % 2 == 0)
            {
                await Task.Delay(150);
                ShowcaseText = ShowcaseText.Remove(ShowcaseText.Length - 2, 2);
                ShowcaseText += showcaseString2[i / 2];
                await Task.Delay(300);
            }
        }
    }

    [RelayCommand]
    private async Task ShowcaseBufferInputEnabledMode(CancellationToken cancellationToken)
    {
        const string showcaseString = "âõíü";

        foreach (char charter in showcaseString)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            ShowcaseText += charter;
            await Task.Delay(300);
        }
    }
}