using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
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
        private static IntPtr hookId = IntPtr.Zero;

        public static bool IsHookSetup { get; private set; }

        public static void SetupSystemHook()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    hookId = NativeMethods.SetWindowsHookEx(HookTypes.WH_KEYBOARD_LL, HookCallback, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
                }
            }

            IsHookSetup = true;
        }

        public static void ShutdownSystemHook()
        {
            NativeMethods.UnhookWindowsHookEx(hookId);
            IsHookSetup = false;
        } 

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var isKeyDown = wParam == WmKeyDown || wParam == WmSysKeyDown;

                var keyboardLowLevelHookStruct = (KeyboardLowLevelHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardLowLevelHookStruct));

                string character = KeyCodeToUnicode(keyboardLowLevelHookStruct.VirtualKeyCode);
                var kea = KeyEventArgs.KeyDown(keyboardLowLevelHookStruct.VirtualKeyCode, character);

                if (isKeyDown)
                {
                    Debug.WriteLine(keyboardLowLevelHookStruct.VirtualKeyCode + " pressed" + " (" + character + ")");
                    KeyPressed?.Invoke(null, kea);
                }

                if (kea.Handled)
                {
                    return 1;
                }           
            }

            return NativeMethods.CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        private static string KeyCodeToUnicode(VirtualKeyCode virtualKeyCode)
        {
            byte[] keyboardState = new byte[255];
            bool keyboardStateStatus = NativeMethods.GetKeyboardState(keyboardState);

            if (!keyboardStateStatus)
            {
                return "";
            }

            uint scanCode = NativeMethods.MapVirtualKey((uint)virtualKeyCode, 0);
            IntPtr inputLocaleIdentifier = NativeMethods.GetKeyboardLayout(0);

            StringBuilder result = new StringBuilder();
            _ = NativeMethods.ToUnicodeEx((uint)virtualKeyCode, scanCode, keyboardState, result, 5, 0, inputLocaleIdentifier);

            return result.ToString();
        }
    }
}