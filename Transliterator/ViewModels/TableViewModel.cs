using CommunityToolkit.Mvvm.ComponentModel;
using System;
using Transliterator.Core.Helpers.Events;
using Transliterator.Core.Models;
using Transliterator.Core.Services;

namespace Transliterator.ViewModels;

public partial class TableViewModel : ObservableObject, IDisposable
{
    private readonly ITransliteratorServiceContext _transliteratorServiceContext;

    public TransliterationTable? SelectedTransliterationTable
    {
        get => _transliteratorServiceContext.TransliterationTable;
    }

    public TableViewModel(ITransliteratorServiceContext transliteratorServiceContext)
    {
        _transliteratorServiceContext = transliteratorServiceContext;
        _transliteratorServiceContext.TransliterationTableChanged += OnTransliterationTableChanged;
    }

    private void OnTransliterationTableChanged(object? sender, TransliterationTableChangedEventArgs e)
    {
        OnPropertyChanged(nameof(SelectedTransliterationTable));
    }

    public void Dispose()
    {
        _transliteratorServiceContext.TransliterationTableChanged -= OnTransliterationTableChanged;
    }
}