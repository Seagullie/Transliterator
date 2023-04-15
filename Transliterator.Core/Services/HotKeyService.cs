using System.Windows.Interop;
using Transliterator.Core.Models;
using Transliterator.Core.Native;

namespace Transliterator.Core.Services;

public class HotKeyService : IHotKeyService, IDisposable
{
    private readonly Dictionary<HotKey, Action> _hotKeys = new();

    public bool IsHotkeyHandlingEnabled { get; set; } = true;

    public void Dispose()
    {
        foreach (var key in _hotKeys.Keys)
            UnregisterHotKey(key);
    }

    /// <summary>
    /// Registers a hotkey with the specified modifiers and key.
    /// </summary>
    /// <param name="modifierKeys">The modifiers that must be pressed along with the key.</param>
    /// <param name="key">The key that must be pressed.</param>
    /// <param name="callback">The method to be called when the hotkey is pressed.</param>
    /// <returns>True if the hotkey registration was successful; otherwise, false.</returns>
    public bool RegisterHotKey(HotKey hotKey, Action action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));
        if (hotKey.Key == Enums.VirtualKeyCode.None || hotKey.Modifiers == Enums.ModifierKeys.None)
            throw new ArgumentNullException(nameof(hotKey));

        if (!NativeMethods.RegisterHotKey(IntPtr.Zero, hotKey.Id, (uint)hotKey.Modifiers, (uint)hotKey.Key))
            return false;

        _hotKeys.Add(hotKey, action);

        ComponentDispatcher.ThreadPreprocessMessage += (ref MSG msg, ref bool handled) =>
        {
            if (msg.message != 0x0312 || msg.wParam.ToInt32() != hotKey.Id)
                return;

            if (IsHotkeyHandlingEnabled)
                action();

            handled = true;
        };

        return true;
    }

    /// <summary>
    /// Unregisters the hotkey with the specified modifiers and key.
    /// </summary>
    /// <param name="modifierKeys">The modifiers that were pressed along with the key.</param>
    /// <param name="key">The key that was pressed.</param>
    /// <returns>True if the hotkey unregistration was successful; otherwise, false.</returns>
    public bool UnregisterHotKey(HotKey hotKey)
    {
        if (!_hotKeys.ContainsKey(hotKey))
            return false;

        if (!NativeMethods.UnregisterHotKey(IntPtr.Zero, hotKey.Id))
            return false;

        _hotKeys.Remove(hotKey);

        ComponentDispatcher.ThreadPreprocessMessage -= (ref MSG msg, ref bool handled) =>
        {
            if (msg.message != 0x0312 || msg.wParam.ToInt32() != hotKey.Id)
                return;

            handled = true;
        };

        return true;
    }
}