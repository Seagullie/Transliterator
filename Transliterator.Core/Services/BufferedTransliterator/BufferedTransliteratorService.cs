using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transliterator.Core.Helpers.Events;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Models;
using Transliterator.Core.Services.BufferedTransliterator;
using Transliterator.Services;

namespace Transliterator.Core.Services
{
    public class BufferedTransliteratorService : BaseTransliterator
    {
        public TransliterationTable TransliterationTable 
        {
            get => transliterationTable;
            set
            {
                if (value != transliterationTable)
                {
                    if (value != null)
                    {
                        tableKeyAnalyzerService = new(value.Combos.ToArray());

                    }
                    transliterationTable = value;
                }
            }
        }

        private TableKeyAnalyzerService tableKeyAnalyzerService;
        private BufferedTransliterator.Buffer buffer = new();
        // at any given time, buffer can be in these 5 states:
        // 1. empty
        // 2. contains a single character that is not a combo
        // 3. contains a single character that is a combo init
        // 4. contains several characters that are part of a combo
        // 5. contains a full combo

        public BufferedTransliteratorService()
        {
            KeyboardHook.SetupSystemHook();

            KeyboardHook.KeyPressed += HandleBackspace;

            KeyboardHook.KeyPressed += SkipIrrelevant;
            KeyboardHook.KeyPressed += KeyPressedHandler;

            buffer.ComboBrokenEvent += (bufferContent) =>
            {
                var transliteratedBuffer = Transliterate(bufferContent);
                KeyboardInputGenerator.TextEntry(AdaptCase(transliteratedBuffer));

                return true;
            };
        }

        public new void SetTableModel(string relativePathToJsonFile)
        {
            SetTableModel(relativePathToJsonFile);
            tableKeyAnalyzerService = new TableKeyAnalyzerService(TransliterationTable.Combos.ToArray());
        }

        private void KeyPressedHandler(object? sender, KeyEventArgs e)
        {
            if (e.ShouldPassHint)
            {
                return;
            }

            // suppress keypress
            e.Handled = true;

            // rendered character is a result of applying any modifers to base keystroke. E.g, "1" (base keystroke) + "shift" (modifier) = "!" (rendered character)
            string renderedCharacter = e.Character;
            buffer.Add(renderedCharacter, tableKeyAnalyzerService);

            // check if should wait for complete combo
            bool defer = tableKeyAnalyzerService.IsStartOfCombination(buffer.GetAsString());

            if (!defer)
            {
                var transliteratedBuffer = Transliterate(buffer.GetAsString());
                KeyboardInputGenerator.TextEntry(AdaptCase(transliteratedBuffer, e));
            }
        }

        public new string Transliterate(string text)
        {
            buffer.Clear();
            return base.Transliterate(text);
        }

        // TODO: Rename
        private string AdaptCase(string transliterated, KeyEventArgs e = null)
        {  // TODO: Change case logic
            if (e != null && (e.IsShift || e.IsCapsLockActive))
            {
                return transliterated.ToUpper();
            }

            return transliterated;
        }

        // keep buffer in sync with keyboard input by erasing last character on backspace
        public void HandleBackspace(object? sender, KeyEventArgs e)
        {
            if (e.Character != "\b")
            {
                return;
            }

            // signal other handlers that this event shouldn't be suppressed or handled
            e.ShouldPassHint = true;

            if (buffer.Count() == 0)
            {
                return;
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
        }

        // TODO: Rename
        // Irrelevant = everything that is not needed for transliteration
        // things that are needed for transliteration:
        // table keys, backspace
        public void SkipIrrelevant(object? sender, KeyEventArgs e)
        {
            string renderedCharacter = e.Character.ToLower();

            // shift is used for capitalization and should not be ignored
            bool isShortcut = e.IsModifier && !e.IsShift;
            bool isIrrelevant = !TransliterationTable.Keys.Contains(renderedCharacter) || isShortcut;

            if (isIrrelevant)
            {
                // signal other handlers that this event shouldn't be suppressed or handled
                e.ShouldPassHint = true;

                // transliterate whatever is left in buffer
                // but only if key is not a modifier. Otherwise combos get broken by simply pressing shift, for example
                if (e.IsModifier) return;

                var transliteratedBuffer = Transliterate(buffer.GetAsString());
                KeyboardInputGenerator.TextEntry(AdaptCase(transliteratedBuffer, e));
            }
        }
    }
}