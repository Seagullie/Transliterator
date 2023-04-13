using Transliterator.Core.Models;

namespace Transliterator.Core.Helpers.Events;

public class TransliterationTableChangedEventArgs : EventArgs
{
    public TransliterationTable? NewTable { get; }

    public TransliterationTableChangedEventArgs(TransliterationTable? newTable)
    {
        NewTable = newTable;
    }
}