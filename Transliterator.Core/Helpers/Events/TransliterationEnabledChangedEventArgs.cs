namespace Transliterator.Core.Helpers.Events;

public class TransliterationEnabledChangedEventArgs : EventArgs
{
    public bool NewState { get; }

    public TransliterationEnabledChangedEventArgs(bool newState)
    {
        NewState = newState;
    }
}