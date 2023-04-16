using System.Diagnostics;
using System.Runtime.InteropServices;
using Transliterator.Core.Enums;
using Transliterator.Core.Native;
using Transliterator.Core.Services;
using Transliterator.Core.Structs;

namespace Transliterator.Core.Keyboard;

public sealed class KeyboardHook : IKeyboardHook, IDisposable
{
    private const int WmKeyDown = 256;
    private const int WmSysKeyDown = 260;

    private readonly ILoggerService? _loggerService;

    /// <summary>
    /// Handle to the hook, need this to unhook and call the next hook
    /// </summary>
    private IntPtr _hookId = IntPtr.Zero;

    private NativeMethods.LowLevelKeyboardProc _proc;

    public KeyboardHook()
    {
        _proc = HookCallback;
        SetHook(_proc);
    }

    public KeyboardHook(ILoggerService loggerService) : this()
    {
        _loggerService = loggerService;
    }

    ~KeyboardHook()
    {
        Dispose(false);
    }

    public event EventHandler<KeyboardHookEventArgs>? KeyDown;

    /// <summary>
    /// Ignores input from KeyboardInputGenerator
    /// </summary>
    public bool SkipUnicodeKeys { get; set; } = true;

    public static bool GetKeyState(VirtualKeyCode key)
    {
        return (NativeMethods.GetAsyncKeyState(key) & 0x8000) != 0;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Create the KeyboardHookEventArgs from the parameters which where in the event
    /// </summary>
    /// <param name="wParam">IntPtr</param>
    /// <param name="lParam">IntPtr</param>
    /// <returns>KeyboardHookEventArgs</returns>
    private static KeyboardHookEventArgs CreateKeyboardEventArgs(IntPtr wParam, IntPtr lParam)
    {
        var isKeyDown = wParam == WmKeyDown || wParam == WmSysKeyDown;

        var keyboardLowLevelHookStruct = (KeyboardLowLevelHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardLowLevelHookStruct));

        char character;

        // TODO: Annotate
        if (keyboardLowLevelHookStruct.VirtualKeyCode == VirtualKeyCode.Packet)
        {
            uint scanCode = keyboardLowLevelHookStruct.ScanCode;
            character = Convert.ToChar(scanCode);
        }
        else
        {
            character = keyboardLowLevelHookStruct.VirtualKeyCode.ToUnicode();
        }

        bool isModifier = keyboardLowLevelHookStruct.VirtualKeyCode.IsModifier();

        var keyEventArgs = new KeyboardHookEventArgs
        {
            Character = character,      // _capsLock || _leftShift || _rightShift ? char.ToUpper(character) : char.ToLower(character),
            Flags = keyboardLowLevelHookStruct.Flags,
            IsCapsLockActive = GetKeyState(VirtualKeyCode.Capital),
            IsKeyDown = isKeyDown,
            IsLeftAlt = GetKeyState(VirtualKeyCode.LeftMenu),
            IsLeftControl = GetKeyState(VirtualKeyCode.LeftControl),
            IsLeftShift = GetKeyState(VirtualKeyCode.LeftShift),
            IsLeftWindows = GetKeyState(VirtualKeyCode.LeftWin),
            IsModifier = isModifier,
            IsNumLockActive = GetKeyState(VirtualKeyCode.NumLock),
            IsRightAlt = GetKeyState(VirtualKeyCode.RightMenu),
            IsRightControl = GetKeyState(VirtualKeyCode.RightControl),
            IsRightShift = GetKeyState(VirtualKeyCode.RightShift),
            IsRightWindows = GetKeyState(VirtualKeyCode.RightWin),
            IsScrollLockActive = GetKeyState(VirtualKeyCode.Scroll),
            Key = keyboardLowLevelHookStruct.VirtualKeyCode,
            TimeStamp = keyboardLowLevelHookStruct.TimeStamp
        };

        return keyEventArgs;
    }

    private void Dispose(bool disposing)
    {
        if (_hookId != IntPtr.Zero)
        {
            NativeMethods.UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            var eventArgs = CreateKeyboardEventArgs(wParam, lParam);

            // Skip the unicode input
            if (SkipUnicodeKeys && eventArgs.Key == VirtualKeyCode.Packet)
            {
                var unicodeLog = $"[KeyboardHook]: {eventArgs.Key} ignored (injected) ({eventArgs.Character}) ({(eventArgs.IsKeyDown ? "down" : "up")})";
                Debug.WriteLine(unicodeLog);
                _loggerService?.LogMessage(this, unicodeLog);

                return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
            }

            var log = $"[KeyboardHook]: {eventArgs.Key} ({eventArgs.Character}) pressed {(eventArgs.IsKeyDown ? "down" : "up")} (Alt {(eventArgs.IsAlt ? "down" : "up")})";
            Debug.WriteLine(log);
            _loggerService?.LogMessage(this, log);

            if (eventArgs.IsKeyDown)
            {
                //var log = $"[KeyboardHook]: {eventArgs.Key} pressed down ({eventArgs.Character})";
                //Debug.WriteLine(log);
                //_loggerService?.LogMessage(this, log);

                KeyDown?.Invoke(this, eventArgs);

                if (eventArgs.Handled)
                {
                    return 1;
                }
            }
        }

        return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private IntPtr SetHook(NativeMethods.LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return NativeMethods.SetWindowsHookEx(HookTypes.WH_KEYBOARD_LL, proc, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
        }
    }
}