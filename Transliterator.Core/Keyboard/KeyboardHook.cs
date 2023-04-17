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

    /// <summary>
    /// Handle to the hook, need this to unhook and call the next hook
    /// </summary>
    private IntPtr _hookId = IntPtr.Zero;

    private NativeMethods.LowLevelKeyboardProc _proc;

    private readonly ILoggerService? _loggerService;

    private bool _leftAlt;
    private bool _leftCtrl;
    private bool _leftShift;
    private bool _leftWin;
    private bool _rightAlt;
    private bool _rightCtrl;
    private bool _rightShift;
    private bool _rightWin;
    private bool _numLock;
    private bool _scrollLock;
    private bool _capsLock;

    public KeyboardHook()
    {
        _proc = HookCallback;
        SetHook(_proc);
    }

    public KeyboardHook(ILoggerService loggerService) : this()
    {
        _loggerService = loggerService;
    }

    public event EventHandler<KeyboardHookEventArgs>? KeyDown;

    /// <summary>
    /// Ignores input from KeyboardInputGenerator
    /// </summary>
    public bool SkipUnicodeKeys { get; set; } = true;

    public static bool GetAsyncKeyState(VirtualKeyCode key)
    {
        return (NativeMethods.GetAsyncKeyState(key) & 0x8000) != 0;
    }

    public void Dispose()
    {
        if (_hookId != IntPtr.Zero)
        {
            NativeMethods.UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }
    }

    /// <summary>
    /// Create the KeyboardHookEventArgs from the parameters which where in the event
    /// </summary>
    /// <param name="wParam">IntPtr</param>
    /// <param name="lParam">IntPtr</param>
    /// <returns>KeyboardHookEventArgs</returns>
    private KeyboardHookEventArgs CreateKeyboardEventArgs(IntPtr wParam, IntPtr lParam)
    {
        bool isKeyDown = (wParam == WmKeyDown || wParam == WmSysKeyDown);

        var keyboardLowLevelHookStruct = (KeyboardLowLevelHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardLowLevelHookStruct));

        // Sometimes when other programs intercept their own hotkeys, HookCallback is not called for the raised key.
        // So, if at least one modifier is pressed, we regularly check that they do not "stick"
        if (_leftAlt || _leftCtrl || _leftShift || _leftWin
            || _rightAlt || _rightCtrl || _rightShift || _rightWin)
        {
            UpdateAllModifiers();
        }

        bool isModifier = keyboardLowLevelHookStruct.VirtualKeyCode.IsModifier();

        if (isModifier)
            UpdateModifier(keyboardLowLevelHookStruct.VirtualKeyCode, isKeyDown);

        char character;

        if (keyboardLowLevelHookStruct.VirtualKeyCode == VirtualKeyCode.Packet)
            character = Convert.ToChar(keyboardLowLevelHookStruct.ScanCode);
        else
            character = keyboardLowLevelHookStruct.VirtualKeyCode.ToUnicode();

        var keyEventArgs = new KeyboardHookEventArgs
        {
            Character = character,
            Flags = keyboardLowLevelHookStruct.Flags,
            IsCapsLockActive = _capsLock,
            IsKeyDown = isKeyDown,
            IsLeftAlt = _leftAlt,
            IsLeftControl = _leftCtrl,
            IsLeftShift = _leftShift,
            IsLeftWindows = _leftWin,
            IsModifier = isModifier,
            IsNumLockActive = _numLock,
            IsRightAlt = _rightAlt,
            IsRightControl = _rightCtrl,
            IsRightShift = _rightShift,
            IsRightWindows = _rightWin,
            IsScrollLockActive = _scrollLock,
            Key = keyboardLowLevelHookStruct.VirtualKeyCode,
            TimeStamp = keyboardLowLevelHookStruct.TimeStamp
        };

        return keyEventArgs;
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

    private void UpdateAllModifiers()
    {
        _leftAlt = GetAsyncKeyState(VirtualKeyCode.LeftMenu);
        _leftCtrl = GetAsyncKeyState(VirtualKeyCode.LeftControl);
        _leftShift = GetAsyncKeyState(VirtualKeyCode.LeftShift);
        _leftWin = GetAsyncKeyState(VirtualKeyCode.LeftWin);
        _rightAlt = GetAsyncKeyState(VirtualKeyCode.RightMenu);
        _rightCtrl = GetAsyncKeyState(VirtualKeyCode.RightControl);
        _rightShift = GetAsyncKeyState(VirtualKeyCode.RightShift);
        _rightWin = GetAsyncKeyState(VirtualKeyCode.RightWin);
    }

    private void UpdateModifier(VirtualKeyCode keyCode, bool isKeyDown)
    {
        // Check the key to find if there any modifiers, store these in the global values.
        switch (keyCode)
        {
            case VirtualKeyCode.Capital:
                if (isKeyDown)
                    _capsLock = !_capsLock;
                break;

            case VirtualKeyCode.NumLock:
                if (isKeyDown)
                    _numLock = !_numLock;
                break;

            case VirtualKeyCode.Scroll:
                if (isKeyDown)
                    _scrollLock = !_scrollLock;
                break;

            case VirtualKeyCode.LeftShift:
                _leftShift = isKeyDown;
                break;

            case VirtualKeyCode.RightShift:
                _rightShift = isKeyDown;
                break;

            case VirtualKeyCode.LeftControl:
                _leftCtrl = isKeyDown;
                break;

            case VirtualKeyCode.RightControl:
                _rightCtrl = isKeyDown;
                break;

            case VirtualKeyCode.LeftMenu:
                _leftAlt = isKeyDown;
                break;

            case VirtualKeyCode.RightMenu:
                _rightAlt = isKeyDown;
                break;

            case VirtualKeyCode.LeftWin:
                _leftWin = isKeyDown;
                break;

            case VirtualKeyCode.RightWin:
                _rightWin = isKeyDown;
                break;
        }
    }
}