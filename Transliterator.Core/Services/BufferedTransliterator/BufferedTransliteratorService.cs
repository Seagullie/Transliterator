using Transliterator.Core.Enums;
using Transliterator.Core.Helpers;
using Transliterator.Core.Helpers.Exceptions;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Models;
using Transliterator.Core.Services.BufferedTransliterator;

namespace Transliterator.Core.Services;

public class BufferedTransliteratorService : ITransliteratorService
{
    // At any given time, buffer can be in these 5 states:
    // 1. empty
    // 2. contains a single character that is an isolated grapheme
    // 3. contains a single character that is a beginning of MultiGraph
    // 4. contains several characters that are part of a MultiGraph
    // 5. contains a full MultiGraph
    protected MultiGraphBuffer buffer = new();

    private readonly LoggerService _loggerService;
    private readonly KeyboardHook _keyboardHook;

    public BufferedTransliteratorService()
    {
        _loggerService = LoggerService.GetInstance();
        _keyboardHook = Singleton<KeyboardHook>.Instance;
        _keyboardHook.KeyDown += HandleKeyPressed;

        buffer.MultiGraphBrokenEvent += Transliterate;
    }

    public event EventHandler? StateChangedEvent;

    private bool transliterationEnabled = true;

    public bool TransliterationEnabled
    {
        get => transliterationEnabled;
        set
        {
            if (value != transliterationEnabled)
            {
                transliterationEnabled = value;
                buffer.Clear();
                StateChangedEvent?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public TransliterationTable? TransliterationTable { get; set; }

    // this method is for testing purposes only
    public void DisposeOfKeyDownEventHandler()
    {
        _keyboardHook.KeyDown -= HandleKeyPressed;
    }

    // "virtual" for testing purposes
    public virtual string EnterTransliterationResults(string text)
    {
        KeyboardInputGenerator.TextEntry(text);
        return text;
    }

    /// <summary>
    /// Keep buffer in sync with keyboard input by erasing last character on backspace
    /// </summary>
    public bool HandleBackspace(KeyboardHookEventArgs e)
    {
        if (e.Key != VirtualKeyCode.Back)
            return false;

        if (buffer.Count == 0)
            return true;

        // TODO: Check if everything is okay
        // ctrl + backspace erases entire word and needs additional handling.
        // Here word = any sequence of characters such as abcdefg123, but not punctuation or other special symbols
        if (e.IsControl)
            buffer.Clear();
        else
            buffer.RemoveAt(buffer.Count() - 1);

        return true;
    }

    // TODO: Rename
    // Irrelevant = everything that is not needed for transliteration
    // things that are needed for transliteration:
    // table keys, backspace
    // "virtual" for testing purposes
    public virtual bool SkipIrrelevant(KeyboardHookEventArgs e)
    {
        bool isIrrelevant = !TransliterationTable.Alphabet.Contains(e.Character.ToLower()) || e.IsModifier || e.IsShortcut;

        if (isIrrelevant)
        {
            _loggerService.LogMessage(this, $"[Transliterator]: This key was skipped as irrelevant: {e.Character}");

            // Transliterate whatever is left in buffer, but only if key is not a modifier.
            // Otherwise combos get broken by simply pressing shift, for example
            if (e.IsModifier)
                return true;

            if (buffer.Count > 0)
            {
                buffer.BrokeMultiGraph();
                buffer.Clear();
            }
            return true;
        }

        return false;
    }

    public virtual void Transliterate(string text)
    {
        if (TransliterationTable == null)
            throw new TableNotSetException("TransliterationTable is not initialized");

        // When MultiGraphBrokenEvent invoked, buffered characters may not form an MultiGraph
        if (text.Length > 1 && !TransliterationTable.Keys.Contains(text.ToLower()))
        {
            foreach (var c in text )
            {
                string cAsString = c.ToString();

                var outputCharacter = TransliterationTable[cAsString.ToLower()];

                if (char.IsUpper(c))
                    outputCharacter = outputCharacter.ToUpper();

                EnterTransliterationResults(outputCharacter);
            }

            buffer.Clear();
            return;
        }

        // Table keys and input text should have same case
        var outputText = TransliterationTable[text.ToLower()];
        if (text.HasUppercase())
            outputText = outputText.ToUpper();

        buffer.Clear();

        EnterTransliterationResults(outputText);
    }

    // this method is for testing purposes only
    protected void AllowUnicode()
    {
        _keyboardHook.SkipUnicodeKeys = false;
    }

    protected virtual void HandleKeyPressed(object? sender, KeyboardHookEventArgs e)
    {
        if (!TransliterationEnabled || HandleBackspace(e) || SkipIrrelevant(e))
            return;

        buffer.Add(e.Character, TransliterationTable);

        // Must be called after buffer.Add()
        SuppressKeypress(e);

        var bufferAsString = buffer.GetAsString();
        if (!TransliterationTable.IsStartOfMultiGraph(bufferAsString))
            Transliterate(bufferAsString);
    }

    // prevent the KeyboardEvent from reaching other applications
    protected virtual void SuppressKeypress(KeyboardHookEventArgs e)
    {
        e.Handled = true;
    }
}