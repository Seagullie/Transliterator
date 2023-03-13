using Newtonsoft.Json;
using Transliterator.Core.Enums;
using Transliterator.Core.Models;
using Transliterator.Core.Services;

namespace Transliterator.Services;

public partial class TransliteratorSettingsService : SettingsService
{
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
}