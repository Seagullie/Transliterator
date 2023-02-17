using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Transliterator.Core;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Models;
using Transliterator.Core.Services;
using Transliterator.Core.Structs;
using Transliterator.Services;
using Transliterator.Views;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace Transliterator.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private const string transliterationTablesPath = "Resources/TranslitTables";

        // TODO: Rewrite to bool
        [ObservableProperty]
        private string _appState;

        [ObservableProperty]
        private string _toggleAppStateShortcut;

        [ObservableProperty]
        private string _applicationTitle = string.Empty;

        [ObservableProperty]
        private ObservableCollection<object> _navigationItems = new();

        [ObservableProperty]
        private ObservableCollection<object> _navigationFooter = new();

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new();

        [ObservableProperty]
        private List<string> transliterationTables;

        [ObservableProperty]
        private string selectedTransliterationTable;

        // TODO: Refactor into TransliteratorTypeController or something like this
        private readonly BufferedTransliteratorService bufferedTransliteratorService;

        private readonly SettingsService settingsService;

        public MainWindowViewModel()
        {
            settingsService = SettingsService.GetInstance();
            bufferedTransliteratorService = new BufferedTransliteratorService();

            LoadTransliterationTables();

            // TODO: Connect to backend
            AppState = "On";
            ToggleAppStateShortcut = "Alt + T";
        }

        private void LoadTransliterationTables()
        {
            TransliterationTables = FileService.GetFileNamesWithoutExtension(transliterationTablesPath);

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
                bufferedTransliteratorService.TransliterationTable = new TransliterationTable(ReadReplacementMapFromJson(value));
            }
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

        [RelayCommand]
        private static void OpenDebugWindow()
        {
            // TODO: Rewrite to NavigateToDebugPage or prevent the creation of multiple windows
            DebugWindow debugWindow = new();
            debugWindow.Show();
        }

        [RelayCommand]
        private void ToggleAppState()
        {
            bufferedTransliteratorService.State = !bufferedTransliteratorService.State;
        }

        public Dictionary<string, string> ReadReplacementMapFromJson(string fileName)
        {
            string TableAsString = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{transliterationTablesPath}\\{fileName}.json"));
            dynamic deserializedTableObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(TableAsString);
            Dictionary<string, string> TableAsDictionary = deserializedTableObj;

            return TableAsDictionary;
        }
    }
}