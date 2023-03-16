using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Transliterator.Core.Models;
using Transliterator.Core.Services;

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

    //private void OnSelectedTransliterationTableChanged(TransliterationTable? value)
    //{
    //}
}