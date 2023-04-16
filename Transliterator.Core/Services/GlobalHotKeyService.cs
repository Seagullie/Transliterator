using System.Diagnostics;
using System.Windows.Interop;
using Transliterator.Core.Models;
using Transliterator.Core.Native;

namespace Transliterator.Core.Services;

public class GlobalHotKeyService : IGlobalHotKeyService, IDisposable
{
    private const int WmHotkey = 0x0312;

    private readonly ILoggerService _loggerService;
    private readonly Dictionary<HotKey, Action> _hotKeys = new();

    public bool IsHotkeyHandlingEnabled { get; set; } = true;

    public GlobalHotKeyService(ILoggerService loggerService)
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

        var log = $"[GlobalHotKeyService] Hotkey registered: <{hotKey}> for {action.Method.DeclaringType.FullName}.{action.Method.Name}() method";
        Debug.WriteLine(log);
        _loggerService.LogMessage(this, log);

        return true;
    }

    public bool UnregisterHotKey(HotKey hotKey)
    {
        if (!_hotKeys.ContainsKey(hotKey))
            return false;

        if (!NativeMethods.UnregisterHotKey(IntPtr.Zero, hotKey.GetHashCode()))
            return false;

        var log = $"[GlobalHotKeyService] Hotkey unregistered: <{hotKey}> for {_hotKeys[hotKey].Method.DeclaringType.FullName}.{_hotKeys[hotKey].Method.Name}() method";
        Debug.WriteLine(log);
        _loggerService.LogMessage(this, log);

        _hotKeys.Remove(hotKey);

        return true;
    }

    private void HotKeyHandler(ref MSG msg, ref bool handled)
    {
        foreach (var hotKey in _hotKeys)
        {
            if (msg.message != WmHotkey || msg.wParam.ToInt32() != hotKey.Key.GetHashCode())
                continue;

            if (IsHotkeyHandlingEnabled)
            {
                hotKey.Value();

                handled = true;

                var log = $"[GlobalHotKeyService] Hotkey pressed: <{hotKey.Key}> in {hotKey.Value.Method.DeclaringType.FullName}.{hotKey.Value.Method.Name}() method";
                Debug.WriteLine(log);
                _loggerService.LogMessage(this, log);
               
                break;
            }
        }
    }
}