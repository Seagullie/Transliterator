using System.Collections.ObjectModel;
using Transliterator.Core.Helpers.Events;
using Transliterator.Core.Models;

namespace Transliterator.Core.Services;

public interface ITransliteratorServiceContext
{
    bool TransliterationEnabled { get; set; }

    TransliterationTable? TransliterationTable { get; set; }

    ObservableCollection<TransliterationTable>? TransliterationTables { get; set; }

    bool UseUnbufferedTransliteratorService { get; set; }

    event EventHandler<TransliterationEnabledChangedEventArgs>? TransliterationEnabledChanged;

    event EventHandler<TransliterationTableChangedEventArgs>? TransliterationTableChanged;

    event EventHandler<TransliterationTablesChangedEventArgs>? TransliterationTablesChanged;
}