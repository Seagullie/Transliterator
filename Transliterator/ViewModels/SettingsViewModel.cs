using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Transliterator.Core.Models;
using Transliterator.Core.Services;
using Transliterator.Services;
using Transliterator.Views.Windows;
using Wpf.Ui.Appearance;

namespace Transliterator.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly bool _isInitialized = false;

    private readonly SettingsService _settingsService;
    private readonly IGlobalHotKeyService _globalHotkeyService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ThemeType _currentTheme;

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
    [NotifyPropertyChangedFor(nameof(ToggleOffSoundFileName))]
    private string toggleOffSoundFilePath;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ToggleOnSoundFileName))]
    private string toggleOnSoundFilePath;

    [ObservableProperty]
    private string toggleTranslitShortcut;

    public bool IsHotkeySuppressionEnabled
    {
        get => !_globalHotkeyService.IsHotkeyHandlingEnabled;
        set => _globalHotkeyService.IsHotkeyHandlingEnabled = !value;
    }

    public List<ThemeType> Themes { get; private set; } = new() { ThemeType.Dark, ThemeType.Light };

    public string ToggleOffSoundFileName { get => Path.GetFileName(ToggleOffSoundFilePath) ?? "<None>"; }

    public string ToggleOnSoundFileName { get => Path.GetFileName(ToggleOnSoundFilePath) ?? "<None>"; }

    public SettingsViewModel(SettingsService settingsService, IGlobalHotKeyService globalHotKeyService, IServiceProvider serviceProvider)
    {
        _settingsService = settingsService;
        _globalHotkeyService = globalHotKeyService;
        _serviceProvider = serviceProvider;

        InitializePropertiesFromSettings();

        _isInitialized = true;
    }

    public void InitializePropertiesFromSettings()
    {
        _settingsService.Load();
        CurrentTheme = _settingsService.SelectedTheme;
        IsAltShiftGlobalShortcutEnabled = _settingsService.IsAltShiftGlobalShortcutEnabled;
        IsAutoStartEnabled = _settingsService.IsAutoStartEnabled;
        IsBufferInputEnabled = _settingsService.IsBufferInputEnabled;
        IsMinimizedStartEnabled = _settingsService.IsMinimizedStartEnabled;
        IsStateOverlayEnabled = _settingsService.IsStateOverlayEnabled;
        IsToggleSoundOn = _settingsService.IsToggleSoundOn;
        IsTranslitEnabledAtStartup = _settingsService.IsTransliteratorEnabledAtStartup;
        ToggleHotKey = _settingsService.ToggleHotKey;
    }

    private void SavePropertiesToSettings()
    {
        if (_isInitialized)
        {
            _settingsService.IsAltShiftGlobalShortcutEnabled = IsAltShiftGlobalShortcutEnabled;
            _settingsService.IsAutoStartEnabled = IsAutoStartEnabled;
            _settingsService.IsBufferInputEnabled = IsBufferInputEnabled;
            _settingsService.IsMinimizedStartEnabled = IsMinimizedStartEnabled;
            _settingsService.IsStateOverlayEnabled = IsStateOverlayEnabled;
            _settingsService.IsToggleSoundOn = IsToggleSoundOn;
            _settingsService.IsTransliteratorEnabledAtStartup = IsTranslitEnabledAtStartup;
            _settingsService.SelectedTheme = CurrentTheme;
            _settingsService.ToggleHotKey = ToggleHotKey;
            _settingsService.Save();
        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (_isInitialized)
            SavePropertiesToSettings();
    }

    [RelayCommand]
    private void OpenDebugWindow()
    {
        // TODO: Prevent the creation of multiple debug windows
        var debugWindow = _serviceProvider.GetService<DebugWindow>();
        debugWindow?.Show();
    }

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
            _settingsService.PathToCustomToggleOffSound = pathToFile;
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
            _settingsService.PathToCustomToggleOnSound = pathToFile;
        }
    }

    [RelayCommand]
    private void DeleteToggleOffSound()
    {
        ToggleOffSoundFilePath = null;
        _settingsService.PathToCustomToggleOffSound = null;
    }

    [RelayCommand]
    private void DeleteToggleOnSound()
    {
        ToggleOnSoundFilePath = null;
        _settingsService.PathToCustomToggleOnSound = null;
    }

    partial void OnCurrentThemeChanged(ThemeType value)
    {
        Theme.Apply(value);
    }

    partial void OnIsBufferInputEnabledChanged(bool value)
    {
        if (ShowcaseBufferInputEnabledModeCommand.IsRunning)
            ShowcaseBufferInputEnabledModeCommand.Cancel();

        if (ShowcaseBufferInputDisabledModeCommand.IsRunning)
            ShowcaseBufferInputDisabledModeCommand.Cancel();

        ShowcaseText = "";

        if (value)
            ShowcaseBufferInputEnabledModeCommand.ExecuteAsync(null);
        else
            ShowcaseBufferInputDisabledModeCommand.ExecuteAsync(null);
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
            await Task.Delay(300, cancellationToken);
            if ((i + 1) % 2 == 0)
            {
                await Task.Delay(150, cancellationToken);
                ShowcaseText = ShowcaseText.Remove(ShowcaseText.Length - 2, 2);
                ShowcaseText += showcaseString2[i / 2];
                await Task.Delay(300, cancellationToken);
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
            await Task.Delay(600, cancellationToken);
        }
    }
}