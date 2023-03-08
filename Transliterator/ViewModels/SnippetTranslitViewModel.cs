using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System;
using Transliterator.Core.Helpers;
using Transliterator.Core.Models;
using Transliterator.Core.Services;

namespace Transliterator.ViewModels;

public partial class SnippetTranslitViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _shouldTransliterateOnTheFly;

    [ObservableProperty]
    private string _transliterationResults;

    [ObservableProperty]
    private string _userInput;

    private ITransliteratorService _transliteratorService;

    public SnippetTranslitViewModel()
    {
        _transliteratorService = Singleton<BufferedTransliteratorService>.Instance;
        _transliteratorService.TransliterationEnabled = false;
        // TODO: remove hardcoding
        Dictionary<string, string> replacementMap = FileService.Read<Dictionary<string, string>>(AppDomain.CurrentDomain.BaseDirectory, "Resources/TranslitTables/tableLAT-UKR.json");
        _transliteratorService.TransliterationTable = new TransliterationTable(replacementMap);
    }

    // For some reason, this handler is called only when "Transliterate" button is clicked
    // TODO: Fix bug
    partial void OnUserInputChanged(string value)
    {
        if (ShouldTransliterateOnTheFly)
        {
            string textToTransliterate = value;
            string transliterationResults = _transliteratorService.Transliterate(textToTransliterate);
            TransliterationResults = transliterationResults;
        }
    }

    [RelayCommand]
    private void TransliterateSnippet()
    {
        string textToTransliterate = UserInput;
        string transliterationResults = _transliteratorService.Transliterate(textToTransliterate);
        TransliterationResults = transliterationResults;
    }
}