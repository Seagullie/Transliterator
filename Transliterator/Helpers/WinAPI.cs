using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static Transliterator.Helpers.WindowsVirtualKey;
using System.Windows.Forms;

namespace Transliterator.Helpers
{
    public class WinAPI
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardInput
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseInput
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HardwareInput
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)] public MouseInput mi;
            [FieldOffset(0)] public KeyboardInput keyboardInput;
            [FieldOffset(0)] public HardwareInput hi;
        }

        public struct INPUT
        {
            public int type;
            public InputUnion union;
        }

        [Flags]
        public enum InputType
        {
            Mouse = 0,
            Keyboard = 1,
            Hardware = 2
        }

        [Flags]
        public enum KeyEventF
        {
            KeyDown = 0x0000,
            ExtendedKey = 0x0001,
            KeyUp = 0x0002,
            Unicode = 0x0004,
            Scancode = 0x0008
        }

        [Flags]
        public enum MouseEventF
        {
            Absolute = 0x8000,
            HWheel = 0x01000,
            Move = 0x0001,
            MoveNoCoalesce = 0x2000,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            VirtualDesk = 0x4000,
            Wheel = 0x0800,
            XDown = 0x0080,
            XUp = 0x0100
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        public static INPUT CreateKeyInput(KeyCode keyCode, int state = 0) // 0x0002 for up, 0 for down
        {
            INPUT keyPress = new INPUT();
            keyPress.type = 1; //Keyboard
            keyPress.union.keyboardInput.wVk = (ushort)keyCode;
            keyPress.union.keyboardInput.dwFlags = (uint)state;
            keyPress.union.keyboardInput.wScan = 0; //use VirtualKey

            return keyPress;
        }

        public static INPUT CreateUnicodeInput(short character, int state = 0x0004) // unicode down –  0x0004

        // for key up use this: 0x0004 | 0x0002
        {
            INPUT keyPress = new INPUT();
            keyPress.type = 1; //Keyboard
            keyPress.union.keyboardInput.wVk = 0; //Use unicode
            keyPress.union.keyboardInput.dwFlags = (uint)state; //Unicode Key Down
            keyPress.union.keyboardInput.wScan = (ushort)character;

            return keyPress;
        }

        public static void WriteStringThroughSendInput(string inputStr)
        {
            List<INPUT> keyList = new List<INPUT>();
            foreach (short c in inputStr)
            {
                switch (c)
                {
                    case 8: // Translate \b to VK_BACKSPACE
                        {
                            INPUT keyDown = CreateKeyInput(KeyCode.BACKSPACE);
                            keyList.Add(keyDown);

                            INPUT keyUp = CreateKeyInput(KeyCode.BACKSPACE, 0x0002);
                            keyList.Add(keyUp);
                        }
                        break;

                    case 9: // Translate \t to VK_TAB
                        {
                            INPUT keyDown = CreateKeyInput(KeyCode.VK_TAB);
                            keyList.Add(keyDown);

                            INPUT keyUp = CreateKeyInput(KeyCode.VK_TAB, 0x0002);
                            keyList.Add(keyUp);
                        }
                        break;

                    case 10: // Translate \n to VK_RETURN
                        {
                            INPUT keyDown = CreateKeyInput(KeyCode.VK_RETURN);
                            keyList.Add(keyDown);

                            INPUT keyUp = CreateKeyInput(KeyCode.VK_RETURN, 0x0002);
                            keyList.Add(keyUp);
                        }
                        break;

                    default:
                        {
                            bool isAscii = c <= sbyte.MaxValue;
                            if (isAscii) // warning danger. kz table doesn't work so I'm temporarily diabling this branch
                                         // as of now it is reEnabled
                            {
                                char cAsChar = (char)c;
                                Keys cAsWinFormKey = ConvertCharToVirtualKey(cAsChar);

                                INPUT keyDown = CreateKeyInput((KeyCode)cAsWinFormKey);
                                keyList.Add(keyDown);

                                INPUT keyUp = CreateKeyInput((KeyCode)cAsWinFormKey, 0x0002);
                                keyList.Add(keyUp);
                            }
                            else
                            {
                                INPUT keyDown = CreateUnicodeInput(c, 0x0004);
                                keyList.Add(keyDown);

                                INPUT keyUp = CreateUnicodeInput(c, 0x0004 | 0x0002);
                                keyList.Add(keyUp);
                            }
                        }
                        break;
                }
            }
            SendInput((uint)keyList.Count, keyList.ToArray(), Marshal.SizeOf(typeof(INPUT)));
        }

        public static Keys ConvertCharToVirtualKey(char ch)
        {
            short vkey = VkKeyScan(ch);
            Keys retval = (Keys)(vkey & 0xff);
            int modifiers = vkey >> 8;

            if ((modifiers & 1) != 0) retval |= Keys.Shift;
            if ((modifiers & 2) != 0) retval |= Keys.Control;
            if ((modifiers & 4) != 0) retval |= Keys.Alt;

            return retval;
        }

        [DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);
    }
}