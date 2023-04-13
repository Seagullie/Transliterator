using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Transliterator.Core.Helpers.Events;
using Transliterator.Core.Models;
using Transliterator.Core.Services;
using Transliterator.Services;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace Transliterator.ViewModels;

public partial class MainWindowViewModel : ObservableObject, IDisposable
{
    private const string pathToSounds = "Resources/Sounds";
    private const string pathToTables = "Resources/TranslitTables";

    private readonly bool _isInitialized = false;
    private readonly SettingsService _settingsService;
    private readonly ITransliteratorServiceContext _transliteratorServiceContext;
    private readonly IHotKeyService _hotKeyService;   
    private readonly IThemeService _themeService;
    
    // TODO: Use converter in XAML instead
    [ObservableProperty]
    private WindowState _windowState;

    [ObservableProperty]
    private ObservableCollection<INavigationControl> navigationFooter = new();

    [ObservableProperty]
    private ObservableCollection<INavigationControl> navigationItems = new();

    [ObservableProperty]
    private ObservableCollection<MenuItem> trayMenuItems = new();

    [ObservableProperty]
    private HotKey? toggleAppStateShortcut;

    public MainWindowViewModel(SettingsService settingsService, ITransliteratorServiceContext transliteratorServiceContext, IHotKeyService hotKeyService, IThemeService themeService)
    {
        _settingsService = settingsService;
        _transliteratorServiceContext = transliteratorServiceContext;
        _hotKeyService = hotKeyService;
        _themeService = themeService;

        _settingsService.Load();
        _settingsService.SettingsSaved += OnSettingsSaved;

        _transliteratorServiceContext.UseUnbufferedTransliteratorService = !_settingsService.IsBufferInputEnabled;
        _transliteratorServiceContext.TransliterationEnabledChanged += OnTransliterationEnabledChanged;
        _transliteratorServiceContext.TransliterationTableChanged += OnTransliterationTableChanged;
        _transliteratorServiceContext.TransliterationTablesChanged += OnTransliterationTablesChanged;

        AppState = _settingsService.IsTransliteratorEnabledAtStartup;

        ToggleAppStateShortcut = _settingsService.ToggleHotKey;
        if (ToggleAppStateShortcut != null)
            _hotKeyService.RegisterHotKey(ToggleAppStateShortcut, () => AppState = !AppState);

        WindowState = _settingsService.IsMinimizedStartEnabled ? WindowState.Minimized : WindowState.Normal;

        LoadTransliterationTables();
        AddTrayMenuItems();
        AddNavigationItems();

        _isInitialized = true;
    }

    public string? ApplicationTitle => App.AppName;

    public bool AppState
    {
        get => _transliteratorServiceContext.TransliterationEnabled;
        set => _transliteratorServiceContext.TransliterationEnabled = value;
    }

    public TransliterationTable? SelectedTransliterationTable
    {
        get => _transliteratorServiceContext.TransliterationTable;
        set => _transliteratorServiceContext.TransliterationTable = value;
    }

    public ObservableCollection<TransliterationTable>? TransliterationTables
    {
        get => _transliteratorServiceContext.TransliterationTables;
        set => _transliteratorServiceContext.TransliterationTables = value;
    }

    public void Dispose()
    {
        _transliteratorServiceContext.TransliterationEnabledChanged -= OnTransliterationEnabledChanged;
        _transliteratorServiceContext.TransliterationTableChanged -= OnTransliterationTableChanged;
        _transliteratorServiceContext.TransliterationTablesChanged -= OnTransliterationTablesChanged;
    }

    public void InitializeTheme()
    {
        _themeService.SetTheme(_settingsService.SelectedTheme);
        _themeService.SetSystemAccent();
    }

    public void SaveSettings()
    {
        _settingsService.LastSelectedTransliterationTable = SelectedTransliterationTable?.ToString() ?? "";
        _settingsService.Save();
    }

    private void AddNavigationItems()
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

    private void OnSettingsSaved(object? sender, EventArgs e)
    {
        if (ToggleAppStateShortcut is not null)
            _hotKeyService.UnregisterHotKey(ToggleAppStateShortcut);

        ToggleAppStateShortcut = _settingsService.ToggleHotKey;
        if (ToggleAppStateShortcut != null)
            _hotKeyService.RegisterHotKey(ToggleAppStateShortcut, () => AppState = !AppState);

        _transliteratorServiceContext.UseUnbufferedTransliteratorService = !_settingsService.IsBufferInputEnabled;
    }

    private void OnTransliterationEnabledChanged(object? sender, TransliterationEnabledChangedEventArgs e)
    {
        OnPropertyChanged(nameof(AppState));

        if (_settingsService.IsToggleSoundOn && _isInitialized)
            PlayToggleSound();
    }

    private void OnTransliterationTableChanged(object? sender, TransliterationTableChangedEventArgs e)
    {
        OnPropertyChanged(nameof(SelectedTransliterationTable));
    }

    private void OnTransliterationTablesChanged(object? sender, TransliterationTablesChangedEventArgs e)
    {
        OnPropertyChanged(nameof(TransliterationTables));
    }

    private void PlayToggleSound()
    {
        string pathToSoundToPlay = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{pathToSounds}/{(AppState ? "cont" : "pause")}.wav");

        if (AppState == true && !string.IsNullOrEmpty(_settingsService.PathToCustomToggleOnSound))
            pathToSoundToPlay = _settingsService.PathToCustomToggleOnSound;

        if (AppState == false && !string.IsNullOrEmpty(_settingsService.PathToCustomToggleOffSound))
            pathToSoundToPlay = _settingsService.PathToCustomToggleOffSound;

        SoundPlayerService.Play(pathToSoundToPlay);
    }
}