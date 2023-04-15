using System.Diagnostics;
using System.Windows.Interop;
using Transliterator.Core.Models;
using Transliterator.Core.Native;

namespace Transliterator.Core.Services;

public class HotKeyService : IHotKeyService, IDisposable
{
    private readonly ILoggerService _loggerService;
    private readonly Dictionary<HotKey, Action> _hotKeys = new();

    public bool IsHotkeyHandlingEnabled { get; set; } = true;

    public HotKeyService(ILoggerService loggerService)
    {
        _loggerService = loggerService;
        ComponentDispatcher.ThreadPreprocessMessage += HotKeyHandler;
    }

    public void Dispose()
    {
        ComponentDispatcher.ThreadPreprocessMessage -= HotKeyHandler;

        foreach (var key in _hotKeys.Keys)
            UnregisterHotKey(key);
    }

    public bool RegisterHotKey(HotKey hotKey, Action action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        if (hotKey.Key == Enums.VirtualKeyCode.None || hotKey.Modifiers == Enums.ModifierKeys.None)
            throw new ArgumentNullException(nameof(hotKey));

        if (!NativeMethods.RegisterHotKey(IntPtr.Zero, hotKey.GetHashCode(), (uint)hotKey.Modifiers, (uint)hotKey.Key))
            return false;

        _hotKeys.Add(hotKey, action);

        return true;
    }

    public bool UnregisterHotKey(HotKey hotKey)
    {
        if (!_hotKeys.ContainsKey(hotKey))
            return false;

        if (!NativeMethods.UnregisterHotKey(IntPtr.Zero, hotKey.GetHashCode()))
            return false;

        _hotKeys.Remove(hotKey);

        return true;
    }

    private void HotKeyHandler(ref MSG msg, ref bool handled)
    {
        foreach (var hotKey in _hotKeys)
        {
            if (msg.message != 0x0312 || msg.wParam.ToInt32() != hotKey.Key.GetHashCode())
                continue;

            if (IsHotkeyHandlingEnabled)
            {
                hotKey.Value();

                var log = $"[HotKeyService]: Class name: {hotKey.Value.Method.DeclaringType.Name}, Method name: {hotKey.Value.Method.Name}";
                Debug.WriteLine(log);
                _loggerService.LogMessage(this, log);

                handled = true;
                break;
            }
        }
    }
}
