using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Transliterator.Core.Enums;

namespace Transliterator.Core.Native;

internal static partial class NativeMethods
{
    private const string User32 = "user32.dll";

    public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SetWindowsHookEx(HookTypes hookType, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    /// <summary>
    ///     Wrapper to simplify sending of inputs
    /// </summary>
    /// <param name="inputs">Input array</param>
    /// <returns>inputs send</returns>
    public static uint SendInput(Structs.Input[] inputs)
    {
        return SendInput((uint)inputs.Length, inputs, Structs.Input.Size);
    }

    /// <summary>
    ///     Synthesizes keystrokes, mouse motions, and button clicks.
    ///     The function returns the number of events that it successfully inserted into the keyboard or mouse input stream.
    ///     If the function returns zero, the input was already blocked by another thread.
    ///     To get extended error information, call GetLastError.
    /// </summary>
    [DllImport(User32, SetLastError = true)]
    private static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray)][In] Structs.Input[] inputs, int cbSize);

    [DllImport(User32, CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern IntPtr GetKeyboardLayout(uint dwLayout);

    [DllImport(User32)]
    public static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

    [DllImport(User32)]
    public static extern bool GetKeyboardState(byte[] lpKeyState);

    /// <summary>
    ///     Retrieve the state of a key
    /// </summary>
    /// <param name="keyCode"></param>
    /// <returns></returns>
    [DllImport(User32, ExactSpelling = true)]
    [ResourceExposure(ResourceScope.None)]
    public static extern ushort GetKeyState(VirtualKeyCode keyCode);

    [DllImport(User32)]
    public static extern uint MapVirtualKey(uint uCode, uint uMapType);
}