using Transliterator.Core.Keyboard;
using Transliterator.Core.Models;

namespace Transliterator.Core
{
    public class TransliteratorService
    {
        public TransliterationTable? TransliterationTable { get; set; }

        public bool State { get; set; } = true;

        public TransliteratorService()
        {
            KeyboardHook.SetupSystemHook();
            KeyboardHook.KeyPressed += KeyPressedHandler;
        }

        private void KeyPressedHandler(object? sender, KeyEventArgs e)
        {
            if (TransliterationTable == null)
                return;

            if (TransliterationTable.Alphabet.Contains(e.Character))
            {
                var transliteratedCharacter = TransliterationTable.ReplacementMap[e.Character];

                e.Handled = true;

                if (e.IsShift || e.IsCapsLockActive)
                {
                    KeyboardInputGenerator.TextEntry(e.Character.ToUpper());
                }
                else
                {
                    KeyboardInputGenerator.TextEntry(e.Character);
                }
            }
        }
    }
}