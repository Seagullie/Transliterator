using System.Text;
using Transliterator.Core.Enums;
using Transliterator.Core.Keyboard;

namespace Transliterator.CoreTests.Fakes
{
    internal class FakeKeyboardInputGenerator : IKeyboardInputGenerator
    {
        private StringBuilder result = new();

        public string Result { get => result.ToString(); }

        public uint KeyCombinationPress(params VirtualKeyCode[] keycodes)
        {
            throw new NotImplementedException();
        }

        public uint KeyDown(params VirtualKeyCode[] keycodes)
        {
            throw new NotImplementedException();
        }

        public uint KeyPresses(params VirtualKeyCode[] keycodes)
        {
            throw new NotImplementedException();
        }

        public uint KeyUp(params VirtualKeyCode[] keycodes)
        {
            throw new NotImplementedException();
        }

        public uint TextEntry(char character)
        {
            result.Append(character);
            return 0;
        }

        public uint TextEntry(string text)
        {
            result.Append(text);
            return 0;
        }

        public void ClearBuffer()
        {
            result.Clear();
        }
    }
}
