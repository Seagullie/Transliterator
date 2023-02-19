using Transliterator.Core.Keyboard;
using Transliterator.Core.Models;
using Transliterator.Services;

namespace Transliterator.Core.Services;

public class BufferedTransliteratorService : BaseTransliterator
{
    private static BufferedTransliteratorService _instance;

    // TODO: Sync with settings
    private bool _state = true;

    // At any given time, buffer can be in these 5 states:
    // 1. empty
    // 2. contains a single character that is not a combo
    // 3. contains a single character that is a combo init
    // 4. contains several characters that are part of a combo
    // 5. contains a full combo
    private BufferedTransliterator.Buffer buffer = new();

    private LoggerService loggerService;

    private readonly KeyboardHook _keyboardHook;



    public BufferedTransliteratorService()
    {
        loggerService = LoggerService.GetInstance();

        _keyboardHook = new();
        _keyboardHook.KeyDown += KeyPressedHandler;

        buffer.ComboBrokenEvent += (bufferContent) =>
        {
            var transliteratedBuffer = Transliterate(bufferContent);
            EnterTransliterationResults(transliteratedBuffer);

            return true;
        };
    }

    public event EventHandler StateChangedEvent;

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

    public virtual string EnterTransliterationResults(string text)
    {
        KeyboardInputGenerator.TextEntry(text);
        return text;
    }

    // keep buffer in sync with keyboard input by erasing last character on backspace
    public bool HandleBackspace(object? sender, KeyboardHookEventArgs e)
    {
        if (e.Character != "\b")
        {
            return false;
        }

        if (buffer.Count() == 0)
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
    public bool SkipIrrelevant(object? sender, KeyboardHookEventArgs e)
    {
        string renderedCharacter = e.Character;

        // shift is used for capitalization and should not be ignored
        bool isShortcut = e.IsModifier && !e.IsShift;
        bool isIrrelevant = !TransliterationTable.isInAlphabet(renderedCharacter) || isShortcut;

        if (isIrrelevant)
        {
            loggerService.LogMessage(this, $"This key was skipped as irrelevant: {renderedCharacter}");

            // transliterate whatever is left in buffer
            // but only if key is not a modifier. Otherwise combos get broken by simply pressing shift, for example
            if (e.IsModifier) return true;

            var transliteratedBuffer = Transliterate(buffer.GetAsString());
            EnterTransliterationResults(transliteratedBuffer);

            return true;
        }

        return false;
    }

    public new string Transliterate(string text)
    {
        buffer.Clear();
        return base.Transliterate(text);
    }

    // TODO: Rename
    private string AdaptCase(string transliterated, KeyboardHookEventArgs e = null)
    {
        // TODO: Change case logic
        if (e != null && (e.IsShift || e.IsCapsLockActive))
        {
            return transliterated.ToUpper();
        }

        return transliterated;
    }

    private void KeyPressedHandler(object? sender, KeyboardHookEventArgs e)
    {
        if (!State || HandleBackspace(sender, e) || SkipIrrelevant(sender, e)) return;

        // suppress keypress
        e.Handled = true;

        // rendered character is a result of applying any modifers to base keystroke. E.g, "1" (base keystroke) + "shift" (modifier) = "!" (rendered character)
        string renderedCharacter = e.Character;
        loggerService.LogMessage(this, $"This key was pressed {renderedCharacter}");
        buffer.Add(renderedCharacter, TransliterationTable);

        // check if should wait for complete combo
        bool defer = TransliterationTable.IsStartOfCombination(buffer.GetAsString());

        if (!defer)
        {
            var transliteratedBuffer = Transliterate(buffer.GetAsString());
            EnterTransliterationResults(transliteratedBuffer);
        }
    }

    private void SetState(bool value)
    {
        _state = value;
        if (!_state) buffer.Clear();
        StateChangedEvent?.Invoke(this, EventArgs.Empty);
    }
}