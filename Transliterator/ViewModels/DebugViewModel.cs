using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using Transliterator.Core.Keyboard;
using Transliterator.Helpers;
using Transliterator.Services;
using Transliterator.Views;

namespace Transliterator.ViewModels
{
    public partial class DebugViewModel : ObservableObject
    {
        public bool logsEnabled = true;

        // TODO: Uncomment after migrating more things from old project
        //private Main liveTransliterator;
        //private TransliteratorService transliteratorService;
        //private KeyLoggerService keyLoggerService;
        private LoggerService loggerService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AppStateDesc))]
        private bool appState;

        public string AppStateDesc { get => AppState ? "On" : "Off"; }

        [ObservableProperty]
        private List<string> translitTables;

        [ObservableProperty]
        private string selectedTranslitTable;

        // TODO: Refactor
        partial void OnSelectedTranslitTableChanged(string value)
        {
            // TODO: Uncomment after migrating more things from old project
            //transliteratorService.SetTableModel(value);
        }

        // TODO: Uncomment after migrating more things from old project
        public DebugViewModel()
        {
            // TODO: Dependency injection
            loggerService = LoggerService.GetInstance();
            //liveTransliterator = Main.GetInstance();
            //keyLoggerService = KeyLoggerService.GetInstance();
            //transliteratorService = TransliteratorService.GetInstance();

            // ---

            //TranslitTables = transliteratorService.TranslitTables;
            TranslitTables = new List<string> { "lorem.json", "ipsum.json", "dolor.json" };

            //keyLoggerService.StateChangedEvent += (s, e) => AppState = keyLoggerService.State;

            // TODO: Convert it into two-way binding by configuring "Path" property of XAML binding and targeting property
            // of TransliteratorService.
            // Is it ok to make service property observable?
            // https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/binding-declarations-overview?view=netdesktop-6.0
            //transliteratorService.TransliterationTableChangedEvent += () => SelectedTranslitTable = transliteratorService.transliterationTableModel.replacementMapFilename;

            //SelectedTranslitTable = transliteratorService.transliterationTableModel.replacementMapFilename;

            // TODO: Subscribe to TranslitTablesChanged event
        }

        [RelayCommand]
        private void ToggleLogs()
        {
            logsEnabled = !logsEnabled;
        }

        // TODO: Uncomment after migrating more things from old project
        [RelayCommand]
        private void ToggleTranslit()
        {
            //liveTransliterator.keyLogger.State = !liveTransliterator.keyLogger.State;
            //string stateDesc = liveTransliterator.keyLogger.State ? "enabled" : "disabled";

            //loggerService.LogMessage(this, $"Translit {stateDesc}");
        }

        [RelayCommand]
        private void OpenSettingsWindow()
        {
            SettingsWindow settingsWindow = new();
            settingsWindow.Show();
        }

        // TODO: Uncomment after migrating more things from old project
        [RelayCommand]
        private void ShowTranslitTable()
        {
            //string serializedTable = Newtonsoft.Json.JsonConvert.SerializeObject(liveTransliterator.transliteratorService.transliterationTableModel.replacementTable, Newtonsoft.Json.Formatting.Indented);
            //loggerService.LogMessage(this, "\n" + serializedTable);
        }

        // TODO: Uncomment after migrating more things from old project
        [RelayCommand]
        private void GetKeyLoggerMemory()
        {
            //string memory = liveTransliterator.keyLogger.GetMemoryAsString();
            //loggerService.LogMessage(this, $"Key Logger memory: [{memory}]");
        }

        [RelayCommand]
        private void AllowInjectedKeys()
        {
            KeyboardHook.SkipInjected = false;
            loggerService.LogMessage(this, "Injected Keys will now be handled by Keyboard Hook");
        }

        // TODO: Uncomment after migrating more things from old project
        [RelayCommand]
        private void SlowDownKBEInjections()
        {
            //liveTransliterator.slowDownKBEInjections = 2000;
        }

        [RelayCommand]
        private void GetLayout()
        {
            string layout = Utilities.GetCurrentKbLayout();
            loggerService.LogMessage(this, layout);
        }

        // TODO: Uncomment after migrating more things from old project
        [RelayCommand]
        private void CheckCaseButtons()
        {
            //bool isCAPSLOCKon = liveTransliterator.keyLogger.keyStateChecker.IsCAPSLOCKon();
            //bool isShiftPressedDown = liveTransliterator.keyLogger.keyStateChecker.IsShiftPressedDown();

            //loggerService.LogMessage(this, $"CAPS LOCK on: {isCAPSLOCKon}. SHIFT pressed down: {isShiftPressedDown}");
        }

        // TODO: Uncomment after migrating more things from old project
        // TODO: Move to own service
        [RelayCommand]
        private void ChangeTheme()
        {
            //if (ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark)
            //{
            //    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            //}
            //else
            //{
            //    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
            //}
        }
    }
}