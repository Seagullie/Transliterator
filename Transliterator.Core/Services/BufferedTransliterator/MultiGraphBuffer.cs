using Transliterator.Core.Models;

namespace Transliterator.Core.Services.BufferedTransliterator;

/// <summary>
/// At any given time, buffer can be in these 5 states:<br/>
/// 1. empty <br/>
/// 2. contains a single character that is an isolated grapheme <br/>
/// 3. contains a single character that is a beginning of MultiGraph <br/>
/// 4. contains several characters that are part of a MultiGraph <br/>
/// 5. contains a full MultiGraph <br/>
/// </summary>
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

        if (tableModel.IsBrokenMultiGraph(GetAsString() + item) && !tableModel.IsPartOfMultiGraph(GetAsString() + item))
        {
            InvokeBrokenMultiGraphEvent();
        }

        base.Add(item);
    }

    public void InvokeBrokenMultiGraphEvent()
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