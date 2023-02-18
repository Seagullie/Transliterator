using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Transliterator.Services;

namespace Transliterator.ViewModels;

public partial class SnippetTranslitViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _shouldTransliterateOnTheFly;

    [ObservableProperty]
    private string _transliterationResults;

    [ObservableProperty]
    private string _userInput;

    private BaseTransliterator baseTransliterator;

    public SnippetTranslitViewModel()
    {
        baseTransliterator = new();
        // TODO: remove hardcoding
        baseTransliterator.SetTableModel("Resources/TranslitTables/tableLAT-UKR.json");
    }

    // For some reason, this handler is called only when "Transliterate" button is clicked
    // TODO: Fix bug
    partial void OnUserInputChanged(string value)
    {
        if (ShouldTransliterateOnTheFly)
        {
            string textToTransliterate = value;
            string transliterationResults = baseTransliterator.Transliterate(textToTransliterate);
            TransliterationResults = transliterationResults;
        }
    }

    [RelayCommand]
    private void TransliterateSnippet()
    {
        string textToTransliterate = UserInput;
        string transliterationResults = baseTransliterator.Transliterate(textToTransliterate);
        TransliterationResults = transliterationResults;
    }
}