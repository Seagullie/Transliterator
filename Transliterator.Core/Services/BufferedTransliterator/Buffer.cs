using Transliterator.Core.Models;

namespace Transliterator.Core.Services.BufferedTransliterator;

// can be confused with System.Buffer
public class Buffer : List<string>
{
    public bool MultiGraphBrokenEventIsBeingHandled = false;

    public Buffer()
    {
    }

    // TODO: Refactor. Predicate requires returning bool and that's not what is necessary
    public event Predicate<string>? MultiGraphBrokenEvent;

    public virtual void Add(string item, TransliterationTable tableModel)
    {
        // sometimes combo can be broken by a character contributing towards bigger combo. e. g, "s" (combo init for "sh") can be broken by c and then followed by h for "sch" ("щ")

        if (tableModel.EndsWithBrokenMultiGraph(GetAsString() + item) && !tableModel.IsPartOfMultiGraph(GetAsString() + item))
        {
            MultiGraphBrokenEventIsBeingHandled = true;
            MultiGraphBrokenEvent?.Invoke(GetAsString());
            MultiGraphBrokenEventIsBeingHandled = false;
        }

        base.Add(item);
    }

    public string GetAsString()
    {
        return string.Join("", this);
    }
}