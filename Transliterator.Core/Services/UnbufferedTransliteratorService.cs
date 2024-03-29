﻿using Transliterator.Core.Enums;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Models;

namespace Transliterator.Core.Services;

// this version of translit still uses buffer, but only as storage container. User input, however, is not buffered
// (except for Isolated Graphemes. Those can be transliterated right away so there is no need to pass and instantly erase a character)
public class UnbufferedTransliteratorService : BufferedTransliteratorService
{
    public UnbufferedTransliteratorService(IKeyboardHook keyboardHook, IKeyboardInputGenerator keyboardInputGenerator)
        : base(keyboardHook, keyboardInputGenerator)
    {
    }

    public UnbufferedTransliteratorService(IKeyboardHook keyboardHook, IKeyboardInputGenerator keyboardInputGenerator, ILoggerService loggerService)
        : base(keyboardHook, keyboardInputGenerator, loggerService)
    {
    }

    /// <summary>
    /// Send backspace characters to erase user input
    /// </summary>
    private void Erase(int times)
    {
        VirtualKeyCode[] backspaceKeyArray = Enumerable.Repeat(VirtualKeyCode.Back, times).ToArray();
        _keyboardInputGenerator.KeyPresses(backspaceKeyArray);
    }

    // should not prevent the kbevent from reaching other applications as this version of transliterator does not buffer user input
    // exceptions apply
    // general overview:
    // we're working at grapheme level here
    // there are two types of graphemes: isolated and multigraph
    // isolated graphemes are transliterated right away
    // multigraph graphemes are skipped, if they start or contribute to a multigraph.
    // if they finish a multigraph, then they are suppressed and entire multigraph is transliterated
    protected override void SuppressKeypress(KeyboardHookEventArgs e)
    {
        string bufferAsString = buffer.GetAsString();

        // unless that's Isolated Grapheme
        // or current (one that's in buffer) MultiGraph finisher
        if (TransliterationTable.IsIsolatedGrapheme(e.Character) || TransliterationTable.IsMultiGraph(bufferAsString))
        {
            e.Handled = true;
        }

        // or MultiGraph grapheme outside MultiGraph
        // those can be transliterated instantly. Unless they initiate a MultiGraph
        else if (!TransliterationTable.IsStartOfMultiGraph(bufferAsString) && TransliterationTable.IsMultiGraphGrapheme(e.Character))
        {
            e.Handled = true;
        }
    }

    // TODO: Annotate
    protected override void Transliterate(string text)
    {
        if (buffer.MultiGraphBrokenEventIsBeingHandled)
        {
            Erase(text.Length);
        }
        else if (text.Length > 1)
        {
            Erase(text.Length - 1);
        }

        base.Transliterate(text);
    }
}