using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using Transliterator.Services;

namespace Transliterator.ViewModels
{
    public partial class TableViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<string> translitTables;

        [ObservableProperty]
        private string selectedTableName;

        // TODO: Uncomment after migrating other things from old project
        //public TransliteratorService transliteratorService;

        public TableViewModel()
        {
            // TODO: Uncomment after migrating other things from old project

            //transliteratorService = TransliteratorService.GetInstance();

            //TranslitTables = transliteratorService.TranslitTables;
            //SelectedTableName = transliteratorService.transliterationTableModel.replacementMapFilename;

            //transliteratorService.TransliterationTableChangedEvent += () => SelectedTableName = transliteratorService.transliterationTableModel.replacementMapFilename;

            // TODO: Subscribe to TransliterationTablesListChangedEvent
        }

        [RelayCommand]
        private void ToggleAppState()
        {
        }
    }
}