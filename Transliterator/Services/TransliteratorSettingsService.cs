using Cogwheel;
using System;
using Transliterator.Core.Enums;
using Transliterator.Core.Models;

namespace Transliterator.Services;

public partial class SettingsService : SettingsBase
{
    private const string configurationFilePath = "Settings.json";

    public bool IsAutoStartEnabled { get; set; }
    public bool IsAltShiftGlobalShortcutEnabled { get; set; } = true;
    public bool IsBufferInputEnabled { get; set; }
    public bool IsMinimizedStartEnabled { get; set; }
    public bool IsStateOverlayEnabled { get; set; } = true;
    public bool IsToggleSoundOn { get; set; } = true;
    public bool IsTransliteratorEnabledAtStartup { get; set; } = true;

    // TODO: Add theming settings

    public string LastSelectedTransliterationTable { get; set; } = "";
    public string PathToCustomToggleOffSound { get; set; } = "";
    public string PathToCustomToggleOnSound { get; set; } = "";

    public HotKey ToggleHotKey { get; set; } = new(VirtualKeyCode.KeyT, ModifierKeys.Alt);

    // Events

    public event EventHandler? SettingsReset;

    public event EventHandler? SettingsLoaded;

    public event EventHandler? SettingsSaved;

    public SettingsService() : base(configurationFilePath)
    {
        Load();
    }

    public override void Reset()
    {
        base.Reset();
        SettingsReset?.Invoke(this, EventArgs.Empty);
    }

    public override void Save()
    {
        base.Save();

        SynchronizeJSONAndWindowsStartupSettings();

        SettingsSaved?.Invoke(this, EventArgs.Empty);
    }

    public override bool Load()
    {
        var wasLoaded = base.Load();

        SynchronizeJSONAndWindowsStartupSettings();

        SettingsLoaded?.Invoke(this, EventArgs.Empty);

        return wasLoaded;
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