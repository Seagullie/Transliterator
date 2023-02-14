using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using System.Windows;
using Transliterator.Services;
using Transliterator.Views;

namespace Transliterator.ViewModels
{
    public partial class SnippetTranslitViewModel : ObservableObject
    {
        private BaseTransliterator baseTransliterator;

        [ObservableProperty]
        private string _userInput;

        [ObservableProperty]
        private string _transliterationResults;

        public SnippetTranslitViewModel()
        {
            baseTransliterator = new();
            // TODO: remove hardcoding
            baseTransliterator.SetTableModel("Resources/TranslitTables/tableLAT-UKR.json");
        }

        [RelayCommand]
        private void TransliterateSnippet()
        {
            string textToTransliterate = UserInput;
            string transliterationResults = baseTransliterator.Transliterate(textToTransliterate);
            TransliterationResults = transliterationResults;
        }
    }
}