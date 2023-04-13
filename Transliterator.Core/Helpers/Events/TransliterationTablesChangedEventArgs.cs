using System.Collections.ObjectModel;
using Transliterator.Core.Models;

namespace Transliterator.Core.Helpers.Events;

public class TransliterationTablesChangedEventArgs : EventArgs
{
    public ObservableCollection<TransliterationTable>? NewCollection { get; }

    public TransliterationTablesChangedEventArgs(ObservableCollection<TransliterationTable>? newCollection)
    {
        NewCollection = newCollection;
    }
}