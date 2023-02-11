namespace Transliterator.Models
{
    public class AppConfig
    {
        public bool PlaySoundOnTranslitToggle { get; set; }

        public string ToggleTranslitShortcut { get; set; }

        public bool DisplayCombos { get; set; }

        public bool StartMinimized { get; set; }

        public bool TurnOnTranslitAtStart { get; set; }

        public bool LaunchProgramOnSystemStartup { get; set; }

        public string ProgramFolder { get; set; }

        public string LastTranslitTable { get; set; }

        public string PathToCustomToggleOnSound { get; set; }

        public string PathToCustomToggleOffSound { get; set; }

        public string SelectedTheme { get; set; }

        public bool EnableStateOverlayWindow { get; set; }

        public bool SuppressAltShift { get; set; }

        public string ProgramName { get; set; }
    }
}