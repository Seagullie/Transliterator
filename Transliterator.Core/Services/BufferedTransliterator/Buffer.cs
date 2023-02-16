using Transliterator.Core.Models;

namespace Transliterator.Core.Services.BufferedTransliterator;

public class Buffer : List<string>
{
    // TODO: Refactor. Predicate requires returning bool and that's not what is necessary
    public event Predicate<string> ComboBrokenEvent;

    public Buffer()
    {
    }

    public void Add(string item, TransliterationTable tableModel)
    {
        // sometimes combo can be broken by a character contributing towards bigger combo. e. g, "s" (combo init for "sh") can be broken by c and then followed by h for "sch" ("щ")

        if (tableModel.EndsWithBrokenCombo(GetAsString() + item) && !tableModel.IsPartOfCombination(GetAsString() + item))
        {
            {
                ComboBrokenEvent?.Invoke(GetAsString());
            }
        }

        base.Add(item);
    }

    public string GetAsString()
    {
        return string.Join("", this);
    }
}