using System.Runtime.InteropServices;

namespace Transliterator.Core.Native;

internal static partial class NativeMethods
{
    private const string Kernel32 = "kernel32.dll";

    [DllImport(Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr GetModuleHandle(string lpModuleName);
}