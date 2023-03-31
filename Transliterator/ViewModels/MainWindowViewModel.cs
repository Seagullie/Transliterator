using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Transliterator.Core.Models;
using Transliterator.Core.Services;
using Transliterator.Services;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace Transliterator.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private const string pathToTables = "Resources/TranslitTables";
    private const string pathToSounds = "Resources/Sounds";

    private readonly IHotKeyService _hotKeyService;
    private readonly SettingsService _settingsService;
    private readonly ITransliteratorService _transliteratorService;
    private readonly TransliteratorServiceContext _transliteratorServiceContext;
    private readonly IThemeService _themeService;

    private bool _isInitialized = false;

    [ObservableProperty]
    private bool _appState;

    // TODO: Use converter in XAML instead
    [ObservableProperty]
    private WindowState _windowState;

    [ObservableProperty]
    private ObservableCollection<INavigationControl> navigationFooter = new();

    [ObservableProperty]
    private ObservableCollection<INavigationControl> navigationItems = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ApplicationTitle))]
    private TransliterationTable? selectedTransliterationTable;

    [ObservableProperty]
    private HotKey? toggleAppStateShortcut;

    [ObservableProperty]
    private ObservableCollection<TransliterationTable>? transliterationTables;

    [ObservableProperty]
    private ObservableCollection<MenuItem> trayMenuItems = new();

    public MainWindowViewModel(SettingsService settingsService, IHotKeyService hotKeyService, TransliteratorServiceContext transliteratorServiceContext, IThemeService themeService)
    {
        _settingsService = settingsService;
        _hotKeyService = hotKeyService;
        _transliteratorServiceContext = transliteratorServiceContext;
        _transliteratorService = transliteratorServiceContext;
        _themeService = themeService;

        _settingsService.Load();
        _settingsService.SettingsSaved += OnSettingsSaved;

        ToggleAppStateShortcut = _settingsService.ToggleHotKey;
        _hotKeyService.RegisterHotKey(ToggleAppStateShortcut, () => AppState = !AppState);

        _transliteratorServiceContext.UseUnbufferedTransliteratorService = !_settingsService.IsBufferInputEnabled;
        _transliteratorService.TransliterationEnabled = _settingsService.IsTransliteratorEnabledAtStartup;
        AppState = _transliteratorService!.TransliterationEnabled;

        bool showMinimized = _settingsService.IsMinimizedStartEnabled;
        WindowState = showMinimized ? WindowState.Minimized : WindowState.Normal;

        LoadTransliterationTables();
        AddTrayMenuItems();
        InitializeViewModel();

        _isInitialized = true;
    }

    public string? ApplicationTitle => App.AppName;

    public void SaveSettings()
    {
        _settingsService.LastSelectedTransliterationTable = SelectedTransliterationTable?.ToString() ?? "";
        _settingsService.Save();
    }

    private void OnSettingsSaved(object? sender, EventArgs e)
    {
        if (ToggleAppStateShortcut is not null)
            _hotKeyService.UnregisterHotKey(ToggleAppStateShortcut);

        ToggleAppStateShortcut = _settingsService.ToggleHotKey;
        _hotKeyService.RegisterHotKey(ToggleAppStateShortcut, () => AppState = !AppState);

        _transliteratorServiceContext.UseUnbufferedTransliteratorService = !_settingsService.IsBufferInputEnabled;
    }

    partial void OnAppStateChanged(bool value)
    {
        _transliteratorService.TransliterationEnabled = value;

        if (_settingsService.IsToggleSoundOn && _isInitialized)
            PlayToggleSound();
    }

    private void PlayToggleSound()
    {
        string pathToSoundToPlay = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{pathToSounds}/{(_transliteratorService.TransliterationEnabled == true ? "cont" : "pause")}.wav");

        if (_transliteratorService.TransliterationEnabled == true && !string.IsNullOrEmpty(_settingsService.PathToCustomToggleOnSound))
            pathToSoundToPlay = _settingsService.PathToCustomToggleOnSound;

        if (_transliteratorService.TransliterationEnabled == false && !string.IsNullOrEmpty(_settingsService.PathToCustomToggleOffSound))
            pathToSoundToPlay = _settingsService.PathToCustomToggleOffSound;

        SoundPlayerService.Play(pathToSoundToPlay);
    }

    partial void OnSelectedTransliterationTableChanged(TransliterationTable? value)
    {
        _transliteratorService.TransliterationTable = value;
    }

    private void LoadTransliterationTables()
    {
        var tableNames = FileService.GetFileNamesWithoutExtension(pathToTables);

        TransliterationTables = new();

        foreach (var tableName in tableNames)
        {
            string relativePathToJsonFile = Path.Combine(pathToTables, tableName + ".json");

            Dictionary<string, string> replacementMap = FileService.Read<Dictionary<string, string>>(AppDomain.CurrentDomain.BaseDirectory, relativePathToJsonFile);

            TransliterationTables.Add(new TransliterationTable(replacementMap, tableName));
        }

        if (TransliterationTables.Count > 0)
            SelectedTransliterationTable = TransliterationTables.FirstOrDefault(table => table.Name == _settingsService.LastSelectedTransliterationTable, TransliterationTables[0]);
    }

    private void AddTrayMenuItems()
    {
        // Doesn't work. As of now, these items are added directly in XAML
        TrayMenuItems = new ObservableCollection<MenuItem>
        {
            new MenuItem
            {
                Header = "Open",
                Tag = "tray_home",
            },
            new MenuItem
            {
                Header = "Settings",
                Tag = "tray_home"
            },
            new MenuItem
            {
                Header = "Close",
                Tag = "tray_home"
            }
        };
    }

    private void InitializeViewModel()
    {
        NavigationItems = new ObservableCollection<INavigationControl>
        {
            new NavigationItem()
            {
                Content = "Snippet Panel",
                PageTag = "snippetPanel",
                Icon = SymbolRegular.Translate24,
                PageType = typeof(Views.Pages.SnippetPanel)
            },
            new NavigationItem()
            {
                Content = "Table View",
                PageTag = "tableView",
                Icon = SymbolRegular.BookLetter24,
                PageType = typeof(Views.Pages.TableViewPage)
            }
        };

        NavigationFooter = new ObservableCollection<INavigationControl>
        {
            new NavigationItem()
            {
                Content = "Settings",
                PageTag = "settings",
                Icon = SymbolRegular.Settings24,
                PageType = typeof(Views.Pages.SettingsPage)
            }
        };
    }

    public void InitializeTheme()
    {
        _themeService.SetTheme(_settingsService.SelectedTheme);
        _themeService.SetSystemAccent();
    }
}