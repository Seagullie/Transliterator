using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using Transliterator.Core.Models;
using Transliterator.Core.Services;
using Wpf.Ui.Common.Interfaces;

namespace Transliterator.ViewModels;

public partial class SnippetTransliteratorViewModel : ObservableObject, ITransliteratorServiceObserver, IDisposable, INavigationAware
{
    private readonly TransliteratorServiceFactory _transliteratorServiceFactory;
    private ITransliteratorService _transliteratorService;

    [ObservableProperty]
    private bool _shouldTransliterateOnTheFly;

    [ObservableProperty]
    private string _transliterationResults;

    [ObservableProperty]
    private string _userInput;

    [ObservableProperty]
    private TransliterationTable transliterationTable;

    public SnippetTransliteratorViewModel(TransliteratorServiceFactory transliteratorServiceFactory)
    {
        _transliteratorServiceFactory = transliteratorServiceFactory;
        _transliteratorServiceFactory.AddObserver(this);
        _transliteratorService = _transliteratorServiceFactory.CreateTransliteratorService();
    }

    public void OnTransliteratorServiceChanged(ITransliteratorService newTransliteratorService)
    {
        _transliteratorService = newTransliteratorService;
    }

    partial void OnUserInputChanged(string value)
    {
        if (ShouldTransliterateOnTheFly)
        {
            string textToTransliterate = value;
            string transliterationResults = _transliteratorService.TransliterationTable?.Transliterate(textToTransliterate);
            TransliterationResults = transliterationResults;
        }
    }

    [RelayCommand]
    private void TransliterateSnippet()
    {
        string textToTransliterate = UserInput;
        if (!string.IsNullOrEmpty(textToTransliterate))
        {
            string transliterationResults = _transliteratorService.TransliterationTable?.Transliterate(textToTransliterate);
            TransliterationResults = transliterationResults;
        }
    }

    public void Dispose()
    {
        _transliteratorServiceFactory.RemoveObserver(this);
    }

    public void OnNavigatedTo()
    {
        _transliteratorService.TransliterationEnabled = false;
    }

    public void OnNavigatedFrom()
    {
        _transliteratorService.TransliterationEnabled = true;
    }
}