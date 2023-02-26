using System.Reflection;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Models;
using Transliterator.Services;

namespace Transliterator.Core.Services;

public class BufferedTransliteratorService : BaseTransliterator
{
    // At any given time, buffer can be in these 5 states:
    // 1. empty
    // 2. contains a single character that is an isolated grapheme
    // 3. contains a single character that is a beginning of MultiGraph
    // 4. contains several characters that are part of a MultiGraph
    // 5. contains a full MultiGraph
    protected BufferedTransliterator.Buffer buffer = new();

    private static BufferedTransliteratorService _instance;

    private readonly KeyboardHook _keyboardHook;

    // TODO: Sync with settings
    private bool _state = true;

    private LoggerService loggerService;

    public BufferedTransliteratorService()
    {
        loggerService = LoggerService.GetInstance();

        _keyboardHook = new();
        _keyboardHook.KeyDown += HandleKeyPressed;

        buffer.MultiGraphBrokenEvent += (IncompleteMultiGraph) =>
        {
            Transliterate(IncompleteMultiGraph);
            return true;
        };
    }

    public event EventHandler? StateChangedEvent;

    public bool State { get => _state; set => SetState(value); }

    // Do we really need this simple check? It's not like setting same translit table is expensive or causes any issues
    public TransliterationTable TransliterationTable
    {
        get => transliterationTable;
        set
        {
            if (value != transliterationTable)
            {
                transliterationTable = value;
            }
        }
    }

    public static BufferedTransliteratorService GetInstance()
    {
        _instance ??= new BufferedTransliteratorService();
        return _instance;
    }

    // TODO: Come up with better way to share the event object
    private KeyboardHookEventArgs? currentlyHandledKeyboardHookEvent = null;

    protected virtual void HandleKeyPressed(object? sender, KeyboardHookEventArgs e)
    {
        currentlyHandledKeyboardHookEvent = e;

        if (!State || HandleBackspace(sender, e) || SkipIrrelevant(sender, e)) return;

        // rendered character is a result of applying any modifers to base keystroke. E.g, "1" (base keystroke) + "shift" (modifier) = "!" (rendered character)
        string renderedCharacter = e.Character;
        AddToBuffer(renderedCharacter);

        // has to be called after .AddToBuffer()
        SuppressKeypress(e);

        if (!ShouldDeferTransliteration())
        {
            Transliterate(buffer.GetAsString());
        }

        currentlyHandledKeyboardHookEvent = null;
    }

    // keep buffer in sync with keyboard input by erasing last character on backspace
    public bool HandleBackspace(object? sender, KeyboardHookEventArgs e)
    {
        if (e.Character != "\b")
        {
            return false;
        }

        if (buffer.Count == 0)
        {
            return true;
        }
        // ctrl + backspace erases entire word and needs additional handling. Here word = any sequence of characters such as abcdefg123, but not punctuation or other special symbols
        if (e.IsLeftControl || e.IsRightControl)
        {
            // erase untill apostrophe is met
            while (buffer.Count() > 0 && buffer.Last() != "'")
            {
                buffer.RemoveAt(buffer.Count() - 1);
            }
        }
        else
        {
            buffer.RemoveAt(buffer.Count() - 1);
        }

        return true;
    }

    // TODO: Rename
    // Irrelevant = everything that is not needed for transliteration
    // things that are needed for transliteration:
    // table keys, backspace

    // "virtual" for testing purposes
    public virtual bool SkipIrrelevant(object? sender, KeyboardHookEventArgs e)
    {
        string renderedCharacter = e.Character;

        // shift is used for capitalization and should not be ignored
        bool isShortcut = e.IsModifier && !e.IsShift;
        bool isIrrelevant = !TransliterationTable.IsInAlphabet(renderedCharacter) || isShortcut;

        if (isIrrelevant)
        {
            loggerService.LogMessage(this, $"[Transliterator]: This key was skipped as irrelevant: {renderedCharacter}");

            // transliterate whatever is left in buffer
            // but only if key is not a modifier. Otherwise combos get broken by simply pressing shift, for example
            if (e.IsModifier) return true;

            // TODO: Refactor
            if (buffer.Count != 0)
            {
                // this should trigger broken MG event and transliterate whatever is left in buffer
                AddToBuffer(renderedCharacter);
                // remove irrelevant character from buffer afterwards
                buffer.Clear();
            }
            return true;
        }

        return false;
    }

    protected virtual void AddToBuffer(string renderedCharacter)
    {
        buffer.Add(renderedCharacter, TransliterationTable);
    }

    // prevent the kbevent from reaching other applications
    protected virtual void SuppressKeypress(KeyboardHookEventArgs e)
    {
        e.Handled = true;
    }

    // check if should wait for complete MultiGraph
    protected virtual bool ShouldDeferTransliteration()
    {
        bool defer = TransliterationTable.IsStartOfMultiGraph(buffer.GetAsString());
        return defer;
    }

    // "virtual" for testing purposes
    public virtual string EnterTransliterationResults(string text)
    {
        KeyboardInputGenerator.TextEntry(text);
        return text;
    }

    public new virtual string Transliterate(string text)
    {
        var transliteratedText = base.Transliterate(text);
        buffer.Clear();

        EnterTransliterationResults(transliteratedText);
        return transliteratedText;
    }

    // we could also copy case from previously entered character
    public override string GetCaseForNonalphabeticString(string replacement)
    {
        if (currentlyHandledKeyboardHookEvent != null && currentlyHandledKeyboardHookEvent.IsCapsLockActive)
        {
            return replacement.ToUpper();
        }

        return replacement;
    }

    // this method is for testing purposes only
    protected void AllowUnicode()
    {
        _keyboardHook.SkipUnicodeKeys = false;
    }

    private void SetState(bool value)
    {
        _state = value;
        if (!_state) buffer.Clear();
        StateChangedEvent?.Invoke(this, EventArgs.Empty);
    }
}