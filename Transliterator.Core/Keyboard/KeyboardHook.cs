using System.Diagnostics;
using System.Runtime.InteropServices;
using Transliterator.Core.Enums;
using Transliterator.Core.Native;
using Transliterator.Core.Structs;

namespace Transliterator.Core.Keyboard
{
    public static class KeyboardHook
    {
        private const int WmKeyDown = 256;
        private const int WmSysKeyUp = 261;
        private const int WmSysKeyDown = 260;

        public static event EventHandler<KeyEventArgs>? KeyPressed;

        /// <summary>
		/// Handle to the hook, need this to unhook and call the next hook
		/// </summary>
        private static IntPtr HookID = IntPtr.Zero;

        public static bool IsHookSetup { get; private set; }

        public static void SetupSystemHook()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    HookID = NativeMethods.SetWindowsHookEx(HookTypes.WH_KEYBOARD_LL, HookCallback, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
                }
            }

            IsHookSetup = true;
        }

        public static void ShutdownSystemHook()
        {
            NativeMethods.UnhookWindowsHookEx(HookID);
            IsHookSetup = false;
        } 

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var isKeyDown = wParam == WmKeyDown || wParam == WmSysKeyDown;

                var keyboardLowLevelHookStruct = (KeyboardLowLevelHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardLowLevelHookStruct));

                var kea = KeyEventArgs.KeyDown(keyboardLowLevelHookStruct.VirtualKeyCode);

                if (isKeyDown)
                {
                    Debug.WriteLine(keyboardLowLevelHookStruct.VirtualKeyCode + " pressed");
                    KeyPressed?.Invoke(null, kea);
                }

                if (kea.Handled)
                {
                    return 1;
                }           
            }

            return NativeMethods.CallNextHookEx(HookID, nCode, wParam, lParam);
        }
    }
}