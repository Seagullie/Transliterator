using Transliterator.Core.Models;

namespace Transliterator.Core.Services
{
    public interface ITransliteratorService
    {
        public const string StandardTransliterationTablesPath = "Resources/TranslitTables";

        public bool TransliterationEnabled { get; set; }

        public TransliterationTable? TransliterationTable { get; set; }

        public string Transliterate(string text);
    }
}
