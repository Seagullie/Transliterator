using System.Text.RegularExpressions;
using Transliterator.Core.Helpers.Exceptions;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Models;
using Transliterator.Helpers;

namespace Transliterator.Core.Services;

public class BufferedTransliteratorService : ITransliteratorService
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

    private LoggerService loggerService;

    public BufferedTransliteratorService()
    {
        loggerService = LoggerService.GetInstance();

        //_keyboardHook = Singleton<KeyboardHook>.Instance;
        _keyboardHook = new KeyboardHook();
        _keyboardHook.KeyDown += HandleKeyPressed;

        buffer.MultiGraphBrokenEvent += (IncompleteMultiGraph) =>
        {
            Transliterate(IncompleteMultiGraph);
            return true;
        };
    }

    public BufferedTransliteratorService(TransliterationTable transliterationTable) : this()
    {
        TransliterationTable = transliterationTable;
    }

    public event EventHandler? StateChangedEvent;


    private bool transliterationEnabled = true;
    public bool TransliterationEnabled { get => transliterationEnabled; set => SetTransliterationState(value); }

    // Do we really need this simple check? It's not like setting same translit table is expensive or causes any issues
    private TransliterationTable? transliterationTable;
    public TransliterationTable? TransliterationTable
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

        if (!TransliterationEnabled || HandleBackspace(sender, e) || SkipIrrelevant(sender, e)) return;

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

        bool isIrrelevant = !TransliterationTable.IsInAlphabet(renderedCharacter) || e.IsModifier || e.IsShortcut;

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

    public virtual string Transliterate(string text)
    {
        if (transliterationTable == null)
        {
            throw new TableNotSetException(".transliterationTableModel is not initialized");
        }

        // table keys and inputlText should have same case
        string inputText = text.ToLower();
        string transliteratedText = text;

        // loop over all possible user inputs and replace them with corresponding transliterations.
        // Replacement map is sorted, thus combinations will be transliterated first
        // This is because individual characters that make up a combination may also be present as separate replacements in the transliterationTableModel.replacementTable. By transliterating combinations first, the separate characters that make up the combination will not be replaced individually and will instead be treated as a single unit.
        // © ChatGPT

        foreach (string key in transliterationTable.Keys)
        {
            // skip keys not present in the text
            if (!inputText.Contains(key))
            {
                continue;
            }

            transliteratedText = ReplaceKeepCase(key, transliterationTable.ReplacementMap[key], transliteratedText);
            // remove already transliterated keys from inputText. This is needed to prevent some bugs
            inputText = inputText.Replace(key, "");
        }

        buffer.Clear();

        EnterTransliterationResults(transliteratedText);
        return transliteratedText;
    }

    // we could also copy case from previously entered character
    public string GetCaseForNonalphabeticString(string replacement)
    {
        if (currentlyHandledKeyboardHookEvent != null && currentlyHandledKeyboardHookEvent.IsCapsLockActive)
        {
            return replacement.ToUpper();
        }

        return replacement;
    }

    private void SetTransliterationState(bool value)
    {
        transliterationEnabled = value;
        if (!transliterationEnabled) buffer.Clear();
        StateChangedEvent?.Invoke(this, EventArgs.Empty);
    }

    // this method is for testing purposes only
    protected void AllowUnicode()
    {
        _keyboardHook.SkipUnicodeKeys = false;
    }

    // this method is for testing purposes only
    public void DisposeOfKeyDownEventHandler()
    {
        _keyboardHook.KeyDown -= HandleKeyPressed;
    }

    public string ReplaceKeepCase(string word, string replacement, string text)
    {
        Func<Match, string> onMatch = match =>
        {
            string matchString = match.Value;

            // nonalphabetic characters don't have uppercase
            // TODO: Optimize this part by making a dictionary for such characters when replacement map is installed
            if (!Utilities.HasUpperCase(matchString))
            {
                return GetCaseForNonalphabeticString(replacement);
            }

            if (Utilities.IsLowerCase(matchString)) return replacement.ToLower();
            if (char.IsUpper(matchString[0])) return replacement.ToUpper();
            // ^TODO: handle case when the replacement consists of several letters

            // if last character is uppercase, replacement should be uppercase as well:
            if (char.IsUpper(matchString[matchString.Length - 1])) return replacement.ToUpper();
            // not sure if C# has a method for converting to titlecase
            if (Utilities.IsUpperCase(matchString)) return replacement.ToUpper();

            return replacement;
        };

        return Regex.Replace(text, Regex.Escape(word), new MatchEvaluator(onMatch), RegexOptions.IgnoreCase);
    }
}