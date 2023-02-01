using System.Runtime.InteropServices;
using Transliterator.Core.Enums;

namespace Transliterator.Core;

/// <summary>
///     A global keyboard hook, using System.Reactive
/// </summary>
public sealed class KeyboardHook
{
    public KeyboardHook()
    {
        
    }

    /// <summary>
    ///     Defines the callback type for the hook
    /// </summary>
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    /// <summary>
    ///     Sets the windows hook, do the desired event, one of hInstance or threadId must be non-null
    /// </summary>
    /// <param name="hookType">The type of the event you want to hook</param>
    /// <param name="lowLevelKeyboardProc">The callback.</param>
    /// <param name="hInstance">The handle you want to attach the event to, can be null</param>
    /// <param name="threadId">The thread you want to attach the event to, can be null</param>
    /// <returns>ID to be able to unhook it again</returns>
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(HookTypes hookType, LowLevelKeyboardProc lowLevelKeyboardProc, IntPtr hInstance, uint threadId);

    /// <summary>
    ///     Used to remove a hook which was set with SetWindowsHookEx
    /// </summary>
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    /// <summary>
    ///     Used to call the next hook in the list, if there was any
    /// </summary>
    /// <param name="hhk">The hook id</param>
    /// <param name="nCode">The hook code</param>
    /// <param name="wParam">The wParam.</param>
    /// <param name="lParam">The lParam.</param>
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
}