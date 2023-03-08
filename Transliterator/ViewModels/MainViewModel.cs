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
        // stores either buffered or unbuffered translit
        // (unbuffered version inherits from buffered one and thus can be assigned to this field with more general type)
        private ITransliteratorService transliteratorService;

        private readonly SettingsService settingsService;
        private readonly HotKeyService hotKeyService;

        [ObservableProperty]
        private string _applicationTitle = string.Empty;

        [ObservableProperty]
        private bool _appState;

        [ObservableProperty]
        private ObservableCollection<object> _navigationFooter = new();

        [ObservableProperty]
        private ObservableCollection<object> _navigationItems = new();

        [ObservableProperty]
        private HotKey _toggleAppStateShortcut;

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new();

        [ObservableProperty]
        private TransliterationTable _selectedTransliterationTable;

        [ObservableProperty]
        private ObservableCollection<TransliterationTable> _transliterationTables = new();

        // TODO: Refactor
        public MainViewModel()
        {
            hotKeyService = Singleton<HotKeyService>.Instance;

            settingsService = SettingsService.GetInstance();
            settingsService.SettingsSavedEvent += OnSettingsSaved;

            SetTransliteratorService();
            LoadTransliterationTables();

            transliteratorService.TransliterationEnabled = settingsService.IsTransliteratorEnabledAtStartup;
            AppState = transliteratorService.TransliterationEnabled;

            ToggleAppStateShortcut = settingsService.ToggleHotKey;
            HotKey hotKey = settingsService.ToggleHotKey;
            hotKeyService.RegisterHotKey(hotKey, () => ToggleAppState());
        }

        private void OnSettingsSaved(object? sender, EventArgs e)
        {
            hotKeyService.UnregisterHotKey(ToggleAppStateShortcut);

            HotKey hotKey = settingsService.ToggleHotKey;
            ToggleAppStateShortcut = hotKey;

            hotKeyService.RegisterHotKey(hotKey, () => ToggleAppState());
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
            var tableList = FileService.GetFileNamesWithoutExtension(ITransliteratorService.StandardTransliterationTablesPath);
            

            foreach (var table in tableList)
            {
                string relativePathToJsonFile = Path.Combine(ITransliteratorService.StandardTransliterationTablesPath, table + ".json");

                Dictionary<string, string> replacementMap = FileService.Read<Dictionary<string, string>>(AppDomain.CurrentDomain.BaseDirectory, relativePathToJsonFile);
                TransliterationTables.Add(new TransliterationTable(replacementMap, table));
            }

            SelectedTransliterationTable = TransliterationTables.First(table => table.Name == settingsService.LastSelectedTransliterationTable)
                                                ?? TransliterationTables.FirstOrDefault();
        }

        partial void OnSelectedTransliterationTableChanged(TransliterationTable value)
        {
            transliteratorService.TransliterationTable = value;
        }

        [RelayCommand]
        private void ToggleAppState()
        {
            transliteratorService.TransliterationEnabled = !transliteratorService.TransliterationEnabled;
            AppState = transliteratorService.TransliterationEnabled;

            if (settingsService.IsToggleSoundOn)
            {
                PlayToggleSound();
            }
        }

        private void PlayToggleSound()
        {
            string pathToSoundToPlay = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Resources/Audio/{(transliteratorService.TransliterationEnabled == true ? "cont" : "pause")}.wav");

            if (transliteratorService.TransliterationEnabled == true && !string.IsNullOrEmpty(settingsService.PathToCustomToggleOnSound))
                pathToSoundToPlay = settingsService.PathToCustomToggleOnSound;

            if (transliteratorService.TransliterationEnabled == false && !string.IsNullOrEmpty(settingsService.PathToCustomToggleOffSound))
                pathToSoundToPlay = settingsService.PathToCustomToggleOffSound;

            SoundPlayerService.Play(pathToSoundToPlay);
        }

        public void SaveSettings()
        {
            settingsService.LastSelectedTransliterationTable = SelectedTransliterationTable.ToString();
            settingsService.Save();
        }

        public void SetTransliteratorService()
        {
            bool? isCurrentTranslitServiceEnabled = transliteratorService?.TransliterationEnabled;

            // disable current transliterator service as it won't be used anymore
            if (transliteratorService != null)
            {
                transliteratorService.TransliterationEnabled = false;
            }

            if (settingsService.IsBufferInputEnabled)
            {
                transliteratorService = BufferedTransliteratorService.GetInstance();
            }
            else
            {
                transliteratorService = UnbufferedTransliteratorService.GetInstance();
            }

            // carry over state from previous transliterator service
            transliteratorService.TransliterationEnabled = isCurrentTranslitServiceEnabled ?? settingsService.IsTransliteratorEnabledAtStartup;
            transliteratorService.TransliterationTable = SelectedTransliterationTable;
        }
    }
}