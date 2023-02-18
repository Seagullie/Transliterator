using System.Diagnostics;
using System.Runtime.InteropServices;
using Transliterator.Core.Enums;
using Transliterator.Core.Native;
using Transliterator.Core.Structs;

namespace Transliterator.Core.Keyboard;

public sealed class KeyboardHook : IDisposable
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

    public event EventHandler<KeyboardHookEventArgs>? KeyPressed;

    private NativeMethods.LowLevelKeyboardProc _proc;

    public bool SkipInjected { get; set; } = true;

    public KeyboardHook()
    {
        SyncLockState();
        _proc = HookCallback;
        SetHook(_proc);
    }
    private IntPtr SetHook(NativeMethods.LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return NativeMethods.SetWindowsHookEx(HookTypes.WH_KEYBOARD_LL, proc, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    public void ShutdownSystemHook()
    {
        NativeMethods.UnhookWindowsHookEx(_hookId);
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

        string character;

        // TODO: Annotate
        if (keyboardLowLevelHookStruct.VirtualKeyCode == VirtualKeyCode.Packet)
        {
            uint scanCode = keyboardLowLevelHookStruct.ScanCode;
            // can't convert to string right away, Convert.ToString simply gives me the scanCode back. So I have to chain it like this
            character = Convert.ToChar(scanCode).ToString();
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
                {
                    _capsLock = !_capsLock;
                }
                break;

            case VirtualKeyCode.NumLock:
                if (isKeyDown)
                {
                    _numLock = !_numLock;
                }
                break;

            case VirtualKeyCode.Scroll:
                if (isKeyDown)
                {
                    _scrollLock = !_scrollLock;
                }
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
            TimeStamp = keyboardLowLevelHookStruct.TimeStamp,
            Key = keyboardLowLevelHookStruct.VirtualKeyCode,
            Flags = keyboardLowLevelHookStruct.Flags,
            IsModifier = isModifier,
            IsKeyDown = isKeyDown,
            IsLeftShift = _leftShift,
            IsRightShift = _rightShift,
            IsLeftAlt = _leftAlt,
            IsRightAlt = _rightAlt,
            IsLeftControl = _leftCtrl,
            IsRightControl = _rightCtrl,
            IsLeftWindows = _leftWin,
            IsRightWindows = _rightWin,
            IsScrollLockActive = _scrollLock,
            IsNumLockActive = _numLock,
            IsCapsLockActive = _capsLock,
            Character = character
        };

        return keyEventArgs;
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            var eventArgs = CreateKeyboardEventArgs(wParam, lParam);

            // Skip the injected key
            if (SkipInjected && eventArgs.Flags == ExtendedKeyFlags.Injected)
            {
                Debug.WriteLine(eventArgs.Key + " ignored (injected)" + " (" + eventArgs.Character + ")");
                return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
            }

            if (eventArgs.IsKeyDown)
            {
                Debug.WriteLine(eventArgs.Key + " pressed" + " (" + eventArgs.Character + ")");
                KeyPressed?.Invoke(this, eventArgs);

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