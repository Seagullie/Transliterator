using System.Diagnostics;
using System.Runtime.InteropServices;
using Transliterator.Core.Enums;
using Transliterator.Core.Native;
using Transliterator.Core.Structs;

namespace Transliterator.Core
{
    public static class KeyboardHook
    {
        private const int WH_KEYBOARD_LL = 13;

        private const int WmKeyDown = 256;
        private const int WmSysKeyUp = 261;
        private const int WmSysKeyDown = 260;

        /// <summary>
		/// Defines the callback type for the hook
		/// </summary>
        private static NativeMethods.LowLevelKeyboardProc LowLevelProc = HookCallback;

        /// <summary>
		/// Handle to the hook, need this to unhook and call the next hook
		/// </summary>
        private static IntPtr HookID = IntPtr.Zero;

        public static bool IsHookSetup { get; private set; }

        private static List<VirtualKeyCode> keys { get; set; } = new();

        public static void SetupSystemHook()
        {
            HookID = SetHook(LowLevelProc);
            IsHookSetup = true;
        }

        public static void ShutdownSystemHook()
        {
            NativeMethods.UnhookWindowsHookEx(HookID);
            IsHookSetup = false;
        }

        private static IntPtr SetHook(NativeMethods.LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    return NativeMethods.SetWindowsHookEx(WH_KEYBOARD_LL, proc, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var isKeyDown = wParam == WmKeyDown || wParam == WmSysKeyDown;

                var keyboardLowLevelHookStruct = (KeyboardLowLevelHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardLowLevelHookStruct));
                //bool isModifier = keyboardLowLevelHookStruct.VirtualKeyCode.IsModifier();

                if (isKeyDown)
                    Debug.WriteLine(keyboardLowLevelHookStruct.VirtualKeyCode + " pressed");
            }

            return NativeMethods.CallNextHookEx(HookID, nCode, wParam, lParam);
        }
    }
}