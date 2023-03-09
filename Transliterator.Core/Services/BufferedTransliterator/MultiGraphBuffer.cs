using Transliterator.Core.Models;

namespace Transliterator.Core.Services.BufferedTransliterator;

public class MultiGraphBuffer : List<string>
{
    public bool MultiGraphBrokenEventIsBeingHandled = false;

    public MultiGraphBuffer()
    {
    }

    // TODO: Refactor. Predicate requires returning bool and that's not what is necessary
    public event Action<string>? MultiGraphBrokenEvent;

    public virtual void Add(string item, TransliterationTable tableModel)
    {
        // sometimes combo can be broken by a character contributing towards bigger combo.
        // e. g, "s" (combo init for "sh") can be broken by c and then followed by h for "sch" ("щ")

        if (tableModel.EndsWithBrokenMultiGraph(GetAsString() + item) && !tableModel.IsPartOfMultiGraph(GetAsString() + item))
        {
            BrokeMultiGraph();
        }

        base.Add(item);
    }

    public void BrokeMultiGraph()
    {
        MultiGraphBrokenEventIsBeingHandled = true;
        MultiGraphBrokenEvent?.Invoke(GetAsString());
        MultiGraphBrokenEventIsBeingHandled = false;
    }

    public string GetAsString()
    {
        return string.Join("", this);
    }
}