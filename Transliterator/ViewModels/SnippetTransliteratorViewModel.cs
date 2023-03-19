using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System;
using Transliterator.Core.Models;
using Transliterator.Core.Services;

namespace Transliterator.ViewModels;

public partial class SnippetTransliteratorViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _shouldTransliterateOnTheFly;

    [ObservableProperty]
    private string _transliterationResults;

    [ObservableProperty]
    private string _userInput;

    [ObservableProperty]
    private TransliterationTable transliterationTable;

    public SnippetTransliteratorViewModel()
    {
    }

    partial void OnUserInputChanged(string value)
    {
        if (ShouldTransliterateOnTheFly)
        {
            string textToTransliterate = value;
            string transliterationResults = transliterationTable.Transliterate(textToTransliterate);
            TransliterationResults = transliterationResults;
        }
    }

    [RelayCommand]
    private void TransliterateSnippet()
    {
        string textToTransliterate = UserInput;
        if (!string.IsNullOrEmpty(textToTransliterate))
        {
            string transliterationResults = transliterationTable.Transliterate(textToTransliterate);
            TransliterationResults = transliterationResults;
        }
    }
}