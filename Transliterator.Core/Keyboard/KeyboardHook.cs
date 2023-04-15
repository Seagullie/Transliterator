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

    // Flags for the current state
    private static bool _leftAlt;

    private static bool _leftCtrl;
    private static bool _leftShift;
    private static bool _leftWin;
    private static bool _rightAlt;
    private static bool _rightCtrl;
    private static bool _rightShift;
    private static bool _rightWin;

    // Flags for the lock keys, initialize the locking keys state one time, these will be updated later
    private static bool _capsLock;

    private static bool _numLock;
    private static bool _scrollLock;

    /// <summary>
    /// Handle to the hook, need this to unhook and call the next hook
    /// </summary>
    private IntPtr _hookId = IntPtr.Zero;

    public event EventHandler<KeyboardHookEventArgs>? KeyDown;

    private NativeMethods.LowLevelKeyboardProc _proc;

    private readonly ILoggerService? _loggerService;

    /// <summary>
    /// Ignores input from KeyboardInputGenerator
    /// </summary>
    public bool SkipUnicodeKeys { get; set; } = true;

    public static bool IsCapsLockActive { get => _capsLock; }
    public static bool IsShiftDown { get => _leftShift || _rightShift; }

    public KeyboardHook()
    {
        SyncLockState();
        _proc = HookCallback;
        SetHook(_proc);
    }

    public KeyboardHook(ILoggerService loggerService) : this()
    {
        _loggerService = loggerService;
    }

    private IntPtr SetHook(NativeMethods.LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return NativeMethods.SetWindowsHookEx(HookTypes.WH_KEYBOARD_LL, proc, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    ~KeyboardHook()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_hookId != IntPtr.Zero)
        {
            NativeMethods.UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }
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
        bool isModifier = keyboardLowLevelHookStruct.VirtualKeyCode.IsModifier();

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

        // Check the key to find if there any modifiers, store these in the global values.
        switch (keyboardLowLevelHookStruct.VirtualKeyCode)
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

        var keyEventArgs = new KeyboardHookEventArgs
        {
            Character = _capsLock || _leftShift || _rightShift ? char.ToUpper(character) : char.ToLower(character),
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
                //var log = $"[KeyboardHook]: {eventArgs.Key} ignored (injected) ({eventArgs.Character}) ({(eventArgs.IsKeyDown ? "down" : "up")})";
                //Debug.WriteLine(log);
                //_loggerService?.LogMessage(this, log);

                return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
            }

            var log = $"[KeyboardHook]: {eventArgs.Key} pressed {(eventArgs.IsKeyDown ? "down" : "up")} (Alt {(eventArgs.IsAlt ? "down" : "up")})";
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

    private static void SyncLockState()
    {
        _capsLock = NativeMethods.GetKeyState(VirtualKeyCode.Capital) > 0;
        _numLock = NativeMethods.GetKeyState(VirtualKeyCode.NumLock) > 0;
        _scrollLock = NativeMethods.GetKeyState(VirtualKeyCode.Scroll) > 0;
    }
}