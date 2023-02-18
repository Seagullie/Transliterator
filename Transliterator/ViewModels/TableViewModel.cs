using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace Transliterator.ViewModels;

public partial class TableViewModel : ObservableObject
{
    // TODO: Uncomment after migrating other things from old project
    //private readonly TransliteratorService transliteratorService;

    [ObservableProperty]
    private string selectedTableName;

    [ObservableProperty]
    private List<string> translitTables;

    public TableViewModel()
    {
        // TODO: Uncomment after migrating other things from old project

        //transliteratorService = TransliteratorService.GetInstance();

        //TranslitTables = transliteratorService.TranslitTables;
        //SelectedTableName = transliteratorService.transliterationTableModel.replacementMapFilename;

        //transliteratorService.TransliterationTableChangedEvent += () => SelectedTableName = transliteratorService.transliterationTableModel.replacementMapFilename;

        // TODO: Subscribe to TransliterationTablesListChangedEvent
    }

    [RelayCommand]
    private void ToggleAppState()
    {
    }
}