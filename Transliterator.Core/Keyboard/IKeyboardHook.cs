namespace Transliterator.Core.Keyboard;

public interface IKeyboardHook
{
    bool SkipUnicodeKeys { get; set; }

    event EventHandler<KeyboardHookEventArgs>? KeyDown;
}