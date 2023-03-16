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

    private TransliterationTable transliterationTable;

    public SnippetTranslitViewModel()
    {
        // TODO: remove hardcoding
        Dictionary<string, string> replacementMap = FileService.Read<Dictionary<string, string>>(AppDomain.CurrentDomain.BaseDirectory, "Resources/TranslitTables/tableLAT-UKR.json");
        transliterationTable = new TransliterationTable(replacementMap);
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