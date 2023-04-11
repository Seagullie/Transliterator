using Transliterator.Core.Enums;

namespace Transliterator.Core.Keyboard;

public interface IKeyboardInputGenerator
{
    uint KeyCombinationPress(params VirtualKeyCode[] keycodes);

    uint KeyDown(params VirtualKeyCode[] keycodes);

    uint KeyPresses(params VirtualKeyCode[] keycodes);

    uint KeyUp(params VirtualKeyCode[] keycodes);

    uint TextEntry(char character);

    uint TextEntry(string text);
}