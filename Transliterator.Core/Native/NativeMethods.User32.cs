using System.Runtime.InteropServices;
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
}