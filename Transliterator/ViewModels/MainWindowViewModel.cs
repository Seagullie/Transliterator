﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Transliterator.Core.Helpers;
using Transliterator.Core.Models;
using Transliterator.Core.Services;
using Transliterator.Models;
using Transliterator.Services;
using Transliterator.Views;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace Transliterator.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private const string transliterationTablesPath = "Resources/TranslitTables";

        // TODO: Refactor into TransliteratorTypeController or something like this
        //private readonly BufferedTransliteratorService bufferedTransliteratorService;
        private readonly UnbufferedTransliteratorService unbufferedTransliteratorService;

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
        private string _toggleAppStateShortcut;

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new();

        [ObservableProperty]
        private string _selectedTransliterationTable;

        [ObservableProperty]
        private List<string> _transliterationTables;

        public MainWindowViewModel()
        {
            settingsService = SettingsService.GetInstance();
            unbufferedTransliteratorService = UnbufferedTransliteratorService.GetInstance();
            hotKeyService = Singleton<HotKeyService>.Instance;

            if (settingsService.LastSelectedTransliterationTable != string.Empty)
            {
                unbufferedTransliteratorService.transliterationTable = new TransliterationTable(ReadReplacementMapFromJson(settingsService.LastSelectedTransliterationTable));
            }

            LoadTransliterationTables();

            AppState = "On";
            ToggleAppStateShortcut = settingsService.ToggleHotKey.ToString();
            HotKey hotKey = settingsService.ToggleHotKey;
            hotKeyService.RegisterHotKey((uint)KeyInterop.VirtualKeyFromKey(hotKey.Key), (uint)hotKey.Modifiers, () => ToggleAppState());
        }

        // TODO: Move to Service
        public Dictionary<string, string> ReadReplacementMapFromJson(string fileName)
        {
            string TableAsString = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{transliterationTablesPath}\\{fileName}.json"));
            dynamic deserializedTableObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(TableAsString);
            Dictionary<string, string> TableAsDictionary = deserializedTableObj;

            return TableAsDictionary;
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
                unbufferedTransliteratorService.TransliterationTable = new TransliterationTable(ReadReplacementMapFromJson(value));
            }
        }

        [RelayCommand]
        private void ToggleAppState()
        {
            unbufferedTransliteratorService.State = !unbufferedTransliteratorService.State;
            AppState = unbufferedTransliteratorService.State ? "On" : "Off";
        }

        public void SaveSettings()
        {
            settingsService.LastSelectedTransliterationTable = SelectedTransliterationTable;
            settingsService.Save();
        }
    }
}