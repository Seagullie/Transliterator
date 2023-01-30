using System.Windows.Input;

using Key = System.Windows.Input.Key;

namespace Transliterator.Helpers
{
    public class KeyStateChecker
    {
        private Key[] modifiers = { Key.LeftCtrl, Key.LeftAlt, Key.LWin, Key.RightCtrl, Key.RightAlt, Key.RWin };

        public bool IsCAPSLOCKon()
        {
            var isCapsLockToggled = Keyboard.IsKeyToggled(Key.CapsLock);
            return isCapsLockToggled;
        }

        public bool IsShiftPressedDown()
        {
            return Keyboard.IsKeyDown(Key.LeftShift);
        }

        public bool IsUpperCase()
        {
            return IsShiftPressedDown() || IsCAPSLOCKon();
        }

        public bool IsLowerCase()
        {
            return !IsShiftPressedDown() && !IsCAPSLOCKon();
        }

        public bool IsKeyDown(Key key)
        {
            return Keyboard.IsKeyDown(key);
        }

        public bool IsModifierPressedDown()
        {
            foreach (Key modifier in modifiers)
            {
                if (Keyboard.IsKeyDown(modifier)) return true;
            }

            return false;
        }
    }
}