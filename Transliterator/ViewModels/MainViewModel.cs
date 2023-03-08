using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        private BufferedTransliteratorService transliteratorService;

        private readonly SettingsService settingsService;
        private readonly HotKeyService hotKeyService;

        [ObservableProperty]
        private string _applicationTitle = string.Empty;

        // TODO: Rewrite to bool
        [ObservableProperty]
        private string _appState;

        [ObservableProperty]
        private ObservableCollection<object> _navigationFooter = new();

        [ObservableProperty]
        private ObservableCollection<object> _navigationItems = new();

        [ObservableProperty]
        private HotKey _toggleAppStateShortcut;

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new();

        [ObservableProperty]
        private string _selectedTransliterationTable;

        [ObservableProperty]
        private List<string> _transliterationTables;

        // TODO: Refactor
        public MainViewModel()
        {
            hotKeyService = Singleton<HotKeyService>.Instance;

            settingsService = SettingsService.GetInstance();
            settingsService.SettingsSavedEvent += (o, e) =>
            {
                hotKeyService.UnregisterHotKey(ToggleAppStateShortcut);

                HotKey hotKey = settingsService.ToggleHotKey;
                ToggleAppStateShortcut = hotKey;

                hotKeyService.RegisterHotKey(hotKey, () => ToggleAppState());
                SetTransliteratorService();
            };

            SetTransliteratorService();
            LoadTransliterationTables();

            AppState = "On";
            ToggleAppStateShortcut = settingsService.ToggleHotKey;
            HotKey hotKey = settingsService.ToggleHotKey;
            hotKeyService.RegisterHotKey(hotKey, () => ToggleAppState());
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
            TransliterationTables = FileService.GetFileNamesWithoutExtension(BufferedTransliteratorService.pathToTransliterationTables);

            if (TransliterationTables.Contains(settingsService.LastSelectedTransliterationTable))
                SelectedTransliterationTable = settingsService.LastSelectedTransliterationTable;
            else if (TransliterationTables.Count > 0)
                SelectedTransliterationTable = TransliterationTables[0];
            else
                SelectedTransliterationTable = "";
        }

        partial void OnSelectedTransliterationTableChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                //transliteratorService.TransliterationTable = new TransliterationTable(ReadReplacementMapFromJson(value));
                transliteratorService.SetTableModel($"{value}.json");
            }
        }

        [RelayCommand]
        private void ToggleAppState()
        {
            transliteratorService.State = !transliteratorService.State;
            AppState = transliteratorService.State ? "On" : "Off";

            if (settingsService.IsToggleSoundOn)
            {
                PlayToggleSound();
            }
        }

        private void PlayToggleSound()
        {
            string pathToSoundToPlay = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Resources/Audio/{(transliteratorService.State == true ? "cont" : "pause")}.wav");

            if (transliteratorService.State == true && !string.IsNullOrEmpty(settingsService.PathToCustomToggleOnSound))
                pathToSoundToPlay = settingsService.PathToCustomToggleOnSound;

            if (transliteratorService.State == false && !string.IsNullOrEmpty(settingsService.PathToCustomToggleOffSound))
                pathToSoundToPlay = settingsService.PathToCustomToggleOffSound;

            SoundPlayerService.Play(pathToSoundToPlay);
        }

        public void SaveSettings()
        {
            settingsService.LastSelectedTransliterationTable = SelectedTransliterationTable;
            settingsService.Save();
        }

        public void SetTransliteratorService()
        {
            bool? isCurrentTranslitServiceEnabled = transliteratorService?.State;

            // disable current transliterator service as it won't be used anymore
            if (transliteratorService != null)
            {
                transliteratorService.State = false;
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
            transliteratorService.State = isCurrentTranslitServiceEnabled ?? settingsService.IsTransliteratorEnabledAtStartup;
            // TODO: Annotate
            if (SelectedTransliterationTable != null)
            {
                transliteratorService.SetTableModel(SelectedTransliterationTable + ".json");
            }
        }
    }
}