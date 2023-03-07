using Newtonsoft.Json;
using System;
using System.IO;
using Transliterator.Models;

namespace Transliterator.Services;

public partial class SettingsService
{
    private const string configurationFilePath = "Settings.json";

    private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
    {
        DefaultValueHandling = DefaultValueHandling.Include,
        Formatting = Formatting.Indented,
        ObjectCreationHandling = ObjectCreationHandling.Replace
    };

    private static SettingsService _instance;

    public SettingsService()
    {
        Load();
    }

    public event EventHandler? SettingsLoadedEvent;

    public event EventHandler? SettingsResetEvent;

    public event EventHandler? SettingsSavedEvent;

    public bool IsAltShiftGlobalShortcutEnabled { get; set; } = true;
    public bool IsAutoStartEnabled { get; set; }
    public bool IsBufferInputEnabled { get; set; }
    public bool IsMinimizedStartEnabled { get; set; }
    public bool IsStateOverlayEnabled { get; set; } = true;
    public bool IsToggleSoundOn { get; set; } = true;
    public bool IsTransliteratorEnabledAtStartup { get; set; }

    // TODO: Add theming settings

    public string LastSelectedTransliterationTable { get; set; } = "";
    public string PathToCustomToggleOffSound { get; set; } = "";
    public string PathToCustomToggleOnSound { get; set; } = "";

    public HotKey ToggleHotKey { get; set; }

    public static SettingsService GetInstance()
    {
        if (_instance == null)
        {
            _instance = new SettingsService();
        }
        return _instance;
    }

    public void Load()
    {
        if (File.Exists(configurationFilePath))
        {
            var settings = File.ReadAllText(configurationFilePath);
            JsonConvert.PopulateObject(settings, this, JsonSerializerSettings);
        }
        else
        {
            Save();
        }

        SynchronizeJSONAndWindowsStartupSettings();

        SettingsLoadedEvent?.Invoke(this, EventArgs.Empty);
    }

    // TODO: Write the implementation
    public void Reset()
    {
        SettingsResetEvent?.Invoke(this, EventArgs.Empty);
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
        bool hasAutoStartEntry = AutostartMethods.HasAutostartEntry();

        if (IsAutoStartEnabled && !hasAutoStartEntry)
            AutostartMethods.WriteAutostartEntry();
        if (!IsAutoStartEnabled && hasAutoStartEntry)
            AutostartMethods.DeleteAutostartEntry();
    }
}