using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using Transliterator.Core;
using Transliterator.Core.Services;
using Transliterator.Services;
using Transliterator.Views;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace Transliterator.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        // TODO: Rewrite to bool
        [ObservableProperty]
        private string _appState;

        [ObservableProperty]
        private string _toggleAppStateShortcut;

        [ObservableProperty]
        private string _applicationTitle = String.Empty;

        [ObservableProperty]
        private ObservableCollection<object> _navigationItems = new();

        [ObservableProperty]
        private ObservableCollection<object> _navigationFooter = new();

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new();

        // TODO: Refactor into TransliteratorTypeController or something like this
        private BufferedTransliteratorService bufferedTransliterator;

        public MainWindowViewModel()
        {
            // TODO: Connect to backend
            AppState = "On";
            ToggleAppStateShortcut = "Alt + T";
            bufferedTransliterator = new BufferedTransliteratorService();
            bufferedTransliterator.SetTableModel("Resources/TranslitTables/tableLAT-UKR.json");
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
    }
}