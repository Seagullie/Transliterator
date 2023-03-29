using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Transliterator.Core.Models;
using Transliterator.Core.Services;
using Transliterator.Services;
using Transliterator.Views;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;

namespace Transliterator.ViewModels;

public partial class MainViewModel : ObservableObject
{
    // Stores either buffered or unbuffered transliteration service
    private ITransliteratorService _transliteratorService;

    private readonly TransliteratorServiceStrategy _transliteratorServiceStrategy;
    private readonly SettingsService _settingsService;
    private readonly IHotKeyService _hotKeyService;

    private SettingsWindow _settingsWindow;
    private TableViewWindow _tableViewWindow;

    public string? ApplicationTitle => App.AppName + " (" + SelectedTransliterationTable.Name + ")";

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
    private HotKey? toggleAppStateShortcut;

    [ObservableProperty]
    private ObservableCollection<MenuItem> trayMenuItems = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ApplicationTitle))]
    private TransliterationTable? selectedTransliterationTable;

    [ObservableProperty]
    private ObservableCollection<TransliterationTable>? transliterationTables;

    public MainViewModel(SettingsService settingsService, IHotKeyService hotKeyService, TransliteratorServiceStrategy transliteratorServiceStrategy)
    {
        _settingsService = settingsService;
        _hotKeyService = hotKeyService;
        _transliteratorServiceStrategy = transliteratorServiceStrategy;

        _transliteratorServiceStrategy.UseUnbufferedTransliteratorService = !_settingsService.IsBufferInputEnabled;
        _transliteratorService = _transliteratorServiceStrategy.CurrentService;
        _transliteratorService.TransliterationEnabled = _settingsService.IsTransliteratorEnabledAtStartup;

        _settingsService.SettingsSaved += OnSettingsSaved;
        _transliteratorServiceStrategy.TransliteratorServiceChanged += OnTransliteratorServiceChanged;

        ToggleAppStateShortcut = _settingsService.ToggleHotKey;
        _hotKeyService.RegisterHotKey(ToggleAppStateShortcut, () => ToggleAppState());

        LoadTransliterationTables();
        AddTrayMenuItems();

        //Theme.Apply(_settingsService.SelectedTheme, BackgroundType.Mica);

        AppState = _transliteratorService!.TransliterationEnabled;

        bool showMinimized = _settingsService.IsMinimizedStartEnabled;
        WindowState = showMinimized ? WindowState.Minimized : WindowState.Normal;
        InitializeViewModel();
    }

    private void InitializeViewModel()
    {
        NavigationItems = new ObservableCollection<INavigationControl>
        {
            new NavigationItem()
            {
                Content = "Home",
                PageTag = "dashboard",
                Icon = SymbolRegular.Home24,
                PageType = typeof(Views.Pages.MainPage)
            },
            new NavigationItem()
            {
                Content = "Snippet Panel",
                PageTag = "snippetPanel",
                Icon = SymbolRegular.DataHistogram24,
                PageType = typeof(Views.Pages.SnippetPanel)
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

    private void OnSettingsSaved(object? sender, EventArgs e)
    {
        if (ToggleAppStateShortcut != null)
            _hotKeyService.UnregisterHotKey(ToggleAppStateShortcut);

        HotKey hotKey = _settingsService.ToggleHotKey;
        ToggleAppStateShortcut = hotKey;

        _hotKeyService.RegisterHotKey(hotKey, () => ToggleAppState());
        _transliteratorServiceStrategy.UseUnbufferedTransliteratorService = !_settingsService.IsBufferInputEnabled;
    }

    [RelayCommand]
    private void OpenSettingsWindow()
    {
        // TODO: Rewrite to NavigateToSettingsPage

        if (_settingsWindow == null || !_settingsWindow.IsLoaded)
            _settingsWindow = new();

        _settingsWindow.Show();
        _settingsWindow.Focus();
    }

    [RelayCommand]
    private void OpenTableViewWindow()
    {
        // TODO: Rewrite to NavigateToTableViewPage
        if (_tableViewWindow == null || !_tableViewWindow.IsLoaded)
        {
            var vm = new TableViewModel()
            {
                TransliterationTables = TransliterationTables,
                SelectedTransliterationTable = SelectedTransliterationTable
            };

            _tableViewWindow = new()
            {
                DataContext = vm
            };
        }

        _tableViewWindow.Show();
        _tableViewWindow.Focus();
    }

    private void AddTrayMenuItems()
    {
        //ApplicationTitle = "Transliterator";

        // doesn't work. As of now, these items are added directly in XAML
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
        string pathToTables = BufferedTransliteratorService.StandardTransliterationTablesPath;
        var tableNames = FileService.GetFileNamesWithoutExtension(pathToTables);

        TransliterationTables = new();

        foreach (var tableName in tableNames)
        {
            string relativePathToJsonFile = Path.Combine(pathToTables, tableName + ".json");

            Dictionary<string, string> replacementMap = FileService.Read<Dictionary<string, string>>(AppDomain.CurrentDomain.BaseDirectory, relativePathToJsonFile);

            TransliterationTables.Add(new TransliterationTable(replacementMap, tableName));
        }

        if (TransliterationTables.Count != 0)
        {
            TransliterationTable lastSelectedOrFirstTransliterationTable = TransliterationTables.FirstOrDefault(table => table.Name == _settingsService.LastSelectedTransliterationTable, TransliterationTables[0]);
            SelectedTransliterationTable = lastSelectedOrFirstTransliterationTable;
        }
    }

    partial void OnSelectedTransliterationTableChanged(TransliterationTable? value)
    {
        if (value != null)
            _transliteratorService.TransliterationTable = value;
    }

    [RelayCommand]
    private void ToggleAppState()
    {
        _transliteratorService.TransliterationEnabled = !_transliteratorService.TransliterationEnabled;
        AppState = _transliteratorService.TransliterationEnabled;

        if (_settingsService.IsToggleSoundOn)
        {
            PlayToggleSound();
        }
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        var currentTheme = Theme.GetAppTheme();

        if (currentTheme == ThemeType.Light || currentTheme == ThemeType.HighContrast)
            Theme.Apply(ThemeType.Dark, BackgroundType.Mica);

        if (currentTheme == ThemeType.Dark || currentTheme == ThemeType.Unknown)
            Theme.Apply(ThemeType.Light, BackgroundType.Mica);
    }

    private void PlayToggleSound()
    {
        string pathToSoundToPlay = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Resources/Audio/{(_transliteratorService.TransliterationEnabled == true ? "cont" : "pause")}.wav");

        if (_transliteratorService.TransliterationEnabled == true && !string.IsNullOrEmpty(_settingsService.PathToCustomToggleOnSound))
            pathToSoundToPlay = _settingsService.PathToCustomToggleOnSound;

        if (_transliteratorService.TransliterationEnabled == false && !string.IsNullOrEmpty(_settingsService.PathToCustomToggleOffSound))
            pathToSoundToPlay = _settingsService.PathToCustomToggleOffSound;

        SoundPlayerService.Play(pathToSoundToPlay);
    }

    public void SaveSettings()
    {
        _settingsService.LastSelectedTransliterationTable = SelectedTransliterationTable?.ToString() ?? "";
        _settingsService.SelectedTheme = Theme.GetAppTheme();
        _settingsService.Save();
    }

    private void OnTransliteratorServiceChanged(object? sender, TransliteratorServiceChangedEventArgs e)
    {
        _transliteratorService = e.NewService;
    }
}