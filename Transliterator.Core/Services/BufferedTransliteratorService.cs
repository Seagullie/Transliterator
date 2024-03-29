﻿using System.Diagnostics;
using Transliterator.Core.Enums;
using Transliterator.Core.Helpers.Exceptions;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Models;

namespace Transliterator.Core.Services;

public class BufferedTransliteratorService : ITransliteratorService
{
    protected readonly IKeyboardHook _keyboardHook;
    protected readonly IKeyboardInputGenerator _keyboardInputGenerator;
    protected readonly ILoggerService? _loggerService;

    protected MultiGraphBuffer buffer = new();

    public BufferedTransliteratorService(IKeyboardHook keyboardHook, IKeyboardInputGenerator keyboardInputGenerator)
    {
        _keyboardHook = keyboardHook;
        _keyboardInputGenerator = keyboardInputGenerator;

        _keyboardHook.KeyDown += HandleKeyDown;

        buffer.MultiGraphBrokenEvent += Transliterate;
    }

    public BufferedTransliteratorService(IKeyboardHook keyboardHook, IKeyboardInputGenerator keyboardInputGenerator, ILoggerService loggerService) : this(keyboardHook, keyboardInputGenerator)
    {
        _loggerService = loggerService;
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
            buffer.RemoveAt(buffer.Count - 1);

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
        bool isNoCaseCharAndShiftIsDown = e.IsShift && TransliterationTable.IsGraphemeWithoutCase(e.Character.ToString());

        bool isIrrelevant = !TransliterationTable.IsInAlphabet(e.Character) || isModifierOrShortcut || isNoCaseCharAndShiftIsDown;

        // Transliterate whatever is left in buffer, but only if key is not shift.
        // Otherwise combos get broken by simply pressing shift, for example
        if (isIrrelevant)
        {
            var log = $"[Transliterator]: This key was skipped as irrelevant: {e.Character}";
            Debug.WriteLine(log);
            _loggerService?.LogMessage(this, log);

            if (buffer.Count > 0 && !e.IsModifier)
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

    protected void HandleKeyDown(object? sender, KeyboardHookEventArgs e)
    {
        if (!TransliterationEnabled || TransliterationTable == null || HandleBackspace(e) || SkipIrrelevant(e))
            return;

        buffer.Add(e.Character.ToString(), TransliterationTable);

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