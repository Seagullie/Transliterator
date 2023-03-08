using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Transliterator.Core.Helpers;
using Transliterator.Core.Models;
using Transliterator.Core.Services;
using Transliterator.Services;
using Transliterator.Views;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace Transliterator.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // Stores either buffered or unbuffered transliteration service
        // (unbuffered version inherits from buffered one and thus can be assigned to this field with more general type)
        private ITransliteratorService _transliteratorService;
        private readonly SettingsService _settingsService;
        private readonly HotKeyService _hotKeyService;

        [ObservableProperty]
        private string? applicationTitle;

        [ObservableProperty]
        private bool appState;

        [ObservableProperty]
        private ObservableCollection<object> navigationFooter = new();

        [ObservableProperty]
        private ObservableCollection<object> navigationItems = new();

        [ObservableProperty]
        private HotKey? toggleAppStateShortcut;

        [ObservableProperty]
        private ObservableCollection<MenuItem> trayMenuItems = new();

        [ObservableProperty]
        private TransliterationTable? selectedTransliterationTable;

        [ObservableProperty]
        private ObservableCollection<TransliterationTable>? transliterationTables;

        // TODO: Refactor
        public MainViewModel()
        {
            _hotKeyService = Singleton<HotKeyService>.Instance;

            _settingsService = SettingsService.GetInstance();
            _settingsService.SettingsSavedEvent += OnSettingsSaved;

            SetTransliteratorService();
            LoadTransliterationTables();

            AppState = _transliteratorService.TransliterationEnabled;

            ToggleAppStateShortcut = _settingsService.ToggleHotKey;
            HotKey hotKey = _settingsService.ToggleHotKey;
            _hotKeyService.RegisterHotKey(hotKey, () => ToggleAppState());
        }

        private void OnSettingsSaved(object? sender, EventArgs e)
        {
            _hotKeyService.UnregisterHotKey(ToggleAppStateShortcut);

            HotKey hotKey = _settingsService.ToggleHotKey;
            ToggleAppStateShortcut = hotKey;

            _hotKeyService.RegisterHotKey(hotKey, () => ToggleAppState());
            SetTransliteratorService();
        }

        [RelayCommand]
        private static void OpenDebugWindow()
        {
            // TODO: Rewrite to NavigateToDebugPage or prevent the creation of multiple windows
            DebugWindow debugWindow = new();
            debugWindow.Show();
        }

        [RelayCommand]
        private static void OpenSettingsWindow()
        {
            // TODO: Rewrite to NavigateToSettingsPage or prevent the creation of multiple windows
            SettingsWindow settings = new();
            settings.Show();
        }

        [RelayCommand]
        private static void OpenSnippetTranslitWindow()
        {
            // TODO: Rewrite to NavigateToSettingsPage or prevent the creation of multiple windows
            SnippetTranslitWindow snippetTranslitWindow = new();
            snippetTranslitWindow.Show();
        }

        private void InitializeViewModel()
        {
            ApplicationTitle = "Transliterator";

            TrayMenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem
                {
                    Header = "Home",
                    Tag = "tray_home"
                }
            };
        }

        private void LoadTransliterationTables()
        {
            var tableNames = FileService.GetFileNamesWithoutExtension(ITransliteratorService.StandardTransliterationTablesPath);

            TransliterationTables = new();

            foreach (var tableName in tableNames)
            {
                string relativePathToJsonFile = Path.Combine(ITransliteratorService.StandardTransliterationTablesPath, tableName + ".json");

                Dictionary<string, string> replacementMap = FileService.Read<Dictionary<string, string>>(AppDomain.CurrentDomain.BaseDirectory, relativePathToJsonFile);

                TransliterationTables.Add(new TransliterationTable(replacementMap, tableName));
            }

            SelectedTransliterationTable = TransliterationTables.First(table => table.Name == _settingsService.LastSelectedTransliterationTable) ?? TransliterationTables.FirstOrDefault();
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
            _settingsService.LastSelectedTransliterationTable = SelectedTransliterationTable?.ToString() ?? ""; ;
            _settingsService.Save();
        }

        public void SetTransliteratorService()
        {
            bool isCurrentTransliterationServiceEnabled = _transliteratorService?.TransliterationEnabled ?? _settingsService.IsTransliteratorEnabledAtStartup;

            // disable current transliterator service as it won't be used anymore
            if (_transliteratorService != null)
                _transliteratorService.TransliterationEnabled = false;

            if (_settingsService.IsBufferInputEnabled)
                _transliteratorService = BufferedTransliteratorService.GetInstance();
            else
                _transliteratorService = UnbufferedTransliteratorService.GetInstance();

            if (SelectedTransliterationTable != null)
                _transliteratorService.TransliterationTable = SelectedTransliterationTable;

            _transliteratorService.TransliterationEnabled = isCurrentTransliterationServiceEnabled;
        }
    }
}