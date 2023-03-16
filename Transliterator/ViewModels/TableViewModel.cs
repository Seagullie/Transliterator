using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Transliterator.Core.Models;

namespace Transliterator.ViewModels;

public partial class TableViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<TransliterationTable>? transliterationTables;

    [ObservableProperty]
    private TransliterationTable? selectedTransliterationTable;

    public TableViewModel()
    {
    }
}