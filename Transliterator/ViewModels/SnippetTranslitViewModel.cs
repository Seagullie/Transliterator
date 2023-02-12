using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using System.Windows;
using Transliterator.Views;

namespace Transliterator.ViewModels
{
    public partial class SnippetTranslitViewModel : ObservableObject
    {
        // TODO: Uncomment after migrating more things from old project
        //private readonly SettingsService settingsService;
        //private readonly KeyLoggerService keyLogger;
        //private TransliteratorService transliterator;

        [ObservableProperty]
        private string _userInput;

        [ObservableProperty]
        private string _transliterationResults;

        public SnippetTranslitViewModel()
        {
            // TODO: Uncomment after migrating more things from old project

            //settingsService = SettingsService.GetInstance();
            //keyLogger = KeyLoggerService.GetInstance();
            //transliterator = keyLogger.transliterator;
        }

        [RelayCommand]
        private void TransliterateSnippet()
        {
            string textToTransliterate = UserInput;
            // TODO: Uncomment after migrating more things from old project
            //string transliterationResults = transliterator.Transliterate(textToTransliterate);
            //TransliterationResults = transliterationResults;
        }
    }
}