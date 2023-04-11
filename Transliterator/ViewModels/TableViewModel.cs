using CommunityToolkit.Mvvm.ComponentModel;
using Transliterator.Core.Models;
using Transliterator.Core.Services;

namespace Transliterator.ViewModels;

public partial class TableViewModel : ObservableObject
{
    private readonly ITransliteratorService _transliteratorService;

    public TransliterationTable? SelectedTransliterationTable
    {
        get => _transliteratorService.TransliterationTable;
    }

    public TableViewModel(TransliteratorServiceContext transliteratorServiceContext)
    {
        _transliteratorService = transliteratorServiceContext;
        transliteratorServiceContext.TransliterationTableChanged += OnTransliterationTableChanged;
    }

    private void OnTransliterationTableChanged(object? sender, System.EventArgs e)
    {
        OnPropertyChanged(nameof(SelectedTransliterationTable));
    }
}