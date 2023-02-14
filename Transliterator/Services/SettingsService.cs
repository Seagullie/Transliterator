using Newtonsoft.Json;
using System;
using System.IO;
using Transliterator.Models;

namespace Transliterator.Services
{
    public partial class SettingsService
    {
        private const string configurationFilePath = "Settings.json";

        private static SettingsService _instance;

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            DefaultValueHandling = DefaultValueHandling.Include,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        public bool IsToggleSoundOn { get; set; } = true;

        public bool IsMinimizedStartEnabled { get; set; }

        public bool IsAutoStartEnabled { get; set; }

        public bool IsStateOverlayEnabled { get; set; } = true;

        public bool IsTranslitEnabledAtStartup { get; set; }

        public bool IsAltShiftGlobalShortcutEnabled { get; set; } = true;

        public bool IsBufferInputEnabled { get; set; }

        // TODO: Add theming settings

        public string LastSelectedTranslitTable { get; set; }

        public string PathToCustomToggleOnSound { get; set; }

        public string PathToCustomToggleOffSound { get; set; }

        // HotKeys
        public HotKey ToggleHotKey { get; set; }

        // Events
        public event EventHandler SettingsResetEvent;

        public event EventHandler SettingsLoadedEvent;

        public event EventHandler SettingsSavedEvent;

        private SettingsService()
        {
        }

        public static SettingsService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SettingsService();
            }
            return _instance;
        }

        // TODO: Write the implementation
        public void Reset()
        {
            SettingsResetEvent?.Invoke(this, EventArgs.Empty);
        }

        public void Load()
        {
            if (File.Exists(configurationFilePath))
            {
                var settings = File.ReadAllText(configurationFilePath);
                JsonConvert.PopulateObject(settings, this, JsonSerializerSettings);
            }

            SynchronizeJSONAndWindowsStartupSettings();

            SettingsLoadedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void Save()
        {
            string settings = JsonConvert.SerializeObject(this, JsonSerializerSettings);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + configurationFilePath, settings);

            SynchronizeJSONAndWindowsStartupSettings();

            SettingsSavedEvent?.Invoke(this, EventArgs.Empty);
        }

        private void SynchronizeJSONAndWindowsStartupSettings()
        {
            bool isHasStartProgEntry = AutostartMethods.HasAutostartEntry();

            if (IsAutoStartEnabled && !isHasStartProgEntry)
                AutostartMethods.WriteAutostartEntry();
            if (!IsAutoStartEnabled && isHasStartProgEntry)
                AutostartMethods.DeleteAutostartEntry();
        }
    }
}