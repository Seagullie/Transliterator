using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Transliterator.Core.Models;
using Transliterator.Core.Services;
using Wpf.Ui.Common.Interfaces;

namespace Transliterator.ViewModels;

public partial class SnippetTransliteratorViewModel : ObservableObject, INavigationAware
{
    private ITransliteratorService _transliteratorService;

    [ObservableProperty]
    private bool _shouldTransliterateOnTheFly;

    [ObservableProperty]
    private string _transliterationResults;

    [ObservableProperty]
    private string _userInput;

    [ObservableProperty]
    private TransliterationTable transliterationTable;

    public SnippetTransliteratorViewModel(TransliteratorServiceStrategy transliteratorServiceStrategy)
    {
        _transliteratorService = transliteratorServiceStrategy.CurrentService;
        transliteratorServiceStrategy.TransliteratorServiceChanged += OnTransliteratorServiceChanged;
    }

    private void OnTransliteratorServiceChanged(object? sender, TransliteratorServiceChangedEventArgs e)
    {
        _transliteratorService = e.NewService;
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

    public void OnNavigatedTo()
    {
        _transliteratorService.TransliterationEnabled = false;
    }

    public void OnNavigatedFrom()
    {
        _transliteratorService.TransliterationEnabled = true;
    }
}