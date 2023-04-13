using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Transliterator.Core.Models;
using Transliterator.Core.Services;

namespace Transliterator.ViewModels;

public partial class SnippetTransliteratorViewModel : ObservableObject
{
    private ITransliteratorServiceContext _transliteratorServiceContext;

    [ObservableProperty]
    private bool _shouldTransliterateOnTheFly;

    [ObservableProperty]
    private string _transliterationResults;

    [ObservableProperty]
    private string _userInput;

    [ObservableProperty]
    private bool _isTextBoxFocused;

    public SnippetTransliteratorViewModel(ITransliteratorServiceContext transliteratorServiceContext)
    {
        _transliteratorServiceContext = transliteratorServiceContext;
    }

    partial void OnUserInputChanged(string value)
    {
        if (ShouldTransliterateOnTheFly)
            TransliterationResults = _transliteratorServiceContext.TransliterationTable?.Transliterate(value);
    }

    [RelayCommand]
    private void TransliterateSnippet()
    {
        if (!string.IsNullOrEmpty(UserInput))
            TransliterationResults = _transliteratorServiceContext.TransliterationTable?.Transliterate(UserInput);
    }

    partial void OnIsTextBoxFocusedChanged(bool value)
    {
        if (value && _transliteratorServiceContext.TransliterationEnabled)
            _transliteratorServiceContext.TransliterationEnabled = false;
    }
}