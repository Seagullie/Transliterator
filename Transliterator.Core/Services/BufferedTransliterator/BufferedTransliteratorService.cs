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

    protected readonly LoggerService _loggerService;
    protected readonly IKeyboardHook _keyboardHook;
    protected readonly IKeyboardInputGenerator _keyboardInputGenerator;

    public BufferedTransliteratorService()
    {
        _loggerService = LoggerService.GetInstance();
        _keyboardHook = Singleton<KeyboardHook>.Instance;
        _keyboardInputGenerator = Singleton<KeyboardInputGenerator>.Instance;
        _keyboardHook.KeyDown += HandleKeyPressed;

        buffer.MultiGraphBrokenEvent += Transliterate;
    }

    // Constructor for testing purposes
    internal BufferedTransliteratorService(IKeyboardHook keyboardHook, IKeyboardInputGenerator keyboardInputGenerator)
    {
        _loggerService = LoggerService.GetInstance();
        _keyboardHook = keyboardHook;
        _keyboardInputGenerator = keyboardInputGenerator;

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

    // TODO: Write tests for erasing scenarios
    /// <summary>
    /// Keep buffer in sync with keyboard input by erasing last character on backspace
    /// </summary>
    private bool HandleBackspace(KeyboardHookEventArgs e)
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

    /// <summary>
    /// Irrelevant = everything that is not needed for transliteration.<br/>
    /// Things that are needed for transliteration: <br/>
    /// table keys, backspace
    /// </summary>
    private bool SkipIrrelevant(KeyboardHookEventArgs e)
    {
        bool isModifierOrShortcut = e.IsModifier || e.IsShortcut;
        // TODO: Annotate
        bool isNoCaseCharAndShiftIsDown = KeyboardHook.IsShiftDown && TransliterationTable.IsGraphemeWithoutCase(e.Character);

        bool isIrrelevant = !TransliterationTable.Alphabet.Contains(e.Character.ToLower()) || isModifierOrShortcut || isNoCaseCharAndShiftIsDown;

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

    protected virtual void Transliterate(string text)
    {
        if (TransliterationTable == null)
            throw new TableNotSetException("TransliterationTable is not initialized");

        var outputText = TransliterationTable.Transliterate(text);

        buffer.Clear();

        _keyboardInputGenerator.TextEntry(outputText);
    }

    protected void HandleKeyPressed(object? sender, KeyboardHookEventArgs e)
    {
        if (!TransliterationEnabled || TransliterationTable == null || HandleBackspace(e) || SkipIrrelevant(e))
            return;

        buffer.Add(e.Character, TransliterationTable);

        // Must be called after buffer.Add()
        SuppressKeypress(e);

        var bufferAsString = buffer.GetAsString();
        if (!TransliterationTable.IsStartOfMultiGraph(bufferAsString))
            Transliterate(bufferAsString);
    }

    /// <summary>
    /// Prevent the KeyboardEvent from reaching other applications
    /// </summary>
    protected virtual void SuppressKeypress(KeyboardHookEventArgs e)
    {
        e.Handled = true;
    }
}