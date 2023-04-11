using Transliterator.Core.Models;

namespace Transliterator.Core.Services;

public interface ITransliteratorService
{
    public bool TransliterationEnabled { get; set; }

    public TransliterationTable? TransliterationTable { get; set; }
}