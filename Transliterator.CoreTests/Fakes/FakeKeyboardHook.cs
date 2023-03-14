using Transliterator.Core.Keyboard;

namespace Transliterator.CoreTests.Fakes
{
    internal class FakeKeyboardHook : IKeyboardHook
    {
        public event EventHandler<KeyboardHookEventArgs>? KeyDown;

        public static bool IsShiftDown;

        public void TextEntry(string text)
        {
            foreach (var character in text)
            {
                var e = new KeyboardHookEventArgs()
                {
                    Character = character
                };

                KeyDown?.Invoke(this, e);
            }
        }
    }
}