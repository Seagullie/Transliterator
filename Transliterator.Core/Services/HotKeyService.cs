using Transliterator.Core.Helpers;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Models;
using Transliterator.Helpers;

namespace Transliterator.Core.Services;

public class HotKeyService : IHotKeyService
{
    private readonly KeyboardHook _keyboardHook;
    private readonly Dictionary<HotKey, List<Action>> _hotKeys = new();

    public bool IsHotkeyHandlingEnabled { get; set; } = true;

    public HotKeyService()
    {
        _keyboardHook = Singleton<KeyboardHook>.Instance;
        _keyboardHook.KeyDown += HandleKeyPressed;
    }

    public void RegisterHotKey(HotKey hotKey, Action action)
    {
        if (!_hotKeys.ContainsKey(hotKey))
        {
            _hotKeys.Add(hotKey, new List<Action>());
        }

        _hotKeys[hotKey].Add(action);
    }

    public void UnregisterHotKey(HotKey hotKey)
    {
        if (_hotKeys.TryGetValue(hotKey, out var actions))
        {
            actions.Clear();
            _hotKeys.Remove(hotKey);
        }
    }

    private void HandleKeyPressed(object? sender, KeyboardHookEventArgs e)
    {
        if (IsHotkeyHandlingEnabled)
        {
            var hotKey = new HotKey(e.Key, e.GetModifierKeys());

            var b = _hotKeys.ContainsKey(hotKey);
            if (_hotKeys.TryGetValue(hotKey, out var actions))
            {
                // TODO: Remove temp fix
                // double check whether alt is down by using other key state reading method
                if (hotKey.Modifiers == Enums.ModifierKeys.Alt && !Utilities.IsAltDown()) return;

                foreach (var action in actions)
                {
                    action();
                }

                e.Handled = true;
            }
        }
    }
}