using Transliterator.Core.Enums;
using Transliterator.Core.Helpers;
using Transliterator.Core.Helpers.Exceptions;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Models;
using Transliterator.Core.Services.BufferedTransliterator;

namespace Transliterator.Core.Services;

public class BufferedTransliteratorService : ITransliteratorService
{
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

    // TODO: Write tests for erasing scenarios
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
        if (e.IsControl)
            buffer.Clear();
        else
            buffer.RemoveAt(buffer.Count() - 1);

        return true;
    }

    // TODO: Rename
    // "virtual" for testing purposes

    /// <summary>
    /// Irrelevant = everything that is not needed for transliteration.<br/>
    /// Things that are needed for transliteration: <br/>
    /// table keys
    /// </summary>
    public virtual bool SkipIrrelevant(KeyboardHookEventArgs e)
    {
        bool isIrrelevant = !TransliterationTable.IsInAlphabet(e.Character) || e.IsModifier || e.IsShortcut;

        if (isIrrelevant)
        {
            _loggerService.LogMessage(this, $"[Transliterator]: This key was skipped as irrelevant: {e.Character}");

            // Transliterate whatever is left in buffer, but only if key is not a modifier.
            // Otherwise combos get broken by simply pressing shift, for example
            if (e.IsModifier)
                return true;

            if (buffer.Count > 0)
            {
                buffer.InvokeBrokenMultiGraphEvent();
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

        // When MultiGraphBrokenEvent іs invoked, buffered characters may not form an MultiGraph
        // and thus have to be transliterated one by one
        if (text.Length > 1 && !TransliterationTable.Keys.Contains(text.ToLower()))
        {
            foreach (char c in text)
            {
                string cAsString = c.ToString();
                string outputCharacter = TransliterationTable.ReplacementMap[cAsString.ToLower()];

                if (char.IsUpper(c))
                    outputCharacter = outputCharacter.ToUpper();

                EnterTransliterationResults(outputCharacter);
            }

            buffer.Clear();
            return;
        }

        // Table keys and input text should both be in lowercase
        var outputText = TransliterationTable.ReplacementMap[text.ToLower()];
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