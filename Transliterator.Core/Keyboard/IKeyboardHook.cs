namespace Transliterator.Core.Keyboard;

public interface IKeyboardHook
{
    event EventHandler<KeyboardHookEventArgs>? KeyDown;
}
