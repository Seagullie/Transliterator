using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transliterator.Core.Enums;
using Transliterator.Core.Keyboard;

namespace Transliterator.Core.Services
{
    // this version of translit still uses buffer, but only as storage container. User input, however, is not buffered
    // (except for one-to-one replacements)
    public class UnbufferedTransliteratorService : BufferedTransliteratorService
    {
        private static UnbufferedTransliteratorService _instance;

        public static new UnbufferedTransliteratorService GetInstance()
        {
            _instance ??= new UnbufferedTransliteratorService();
            return _instance;
        }

        // send backspace characters to erase user input
        public virtual void Erase(int times)
        {
            VirtualKeyCode[] backspaceKeyArray = Enumerable.Repeat(VirtualKeyCode.Back, times).ToArray();
            KeyboardInputGenerator.KeyPresses(backspaceKeyArray);
        }

        // should not prevent the kbevent from reaching other applications as this version of transliterator does not buffer user input
        protected override void SuppressKeypress(KeyboardHookEventArgs e)
        {
            // do nothing. That's intended

            // unless that's one-to-one replacement character
            if (transliterationTable.IsOneToOneReplacementChar(e.Character))
            {
                e.Handled = true;
            }
            // or current (one that's in buffer) combination finisher
            else if (transliterationTable.IsAddingUpToCombo(buffer.GetAsString(), e.Character))
            {
                e.Handled = true;
            }
        }

        // should never defer as it's unbuffered version
        //protected override bool ShouldDeferTransliteration()
        //{
        //    return false;
        //}

        public override string Transliterate(string text)
        {
            if (text == string.Empty) return "";

            // one-to-one replacement is still buffered
            if (!transliterationTable.IsOneToOneReplacementChar(text))
            {
                int nOfCharsToErase = text.Length;
                // combo finishers are also suppressed, so no need to erase them
                if (transliterationTable.IsCombo(text))
                {
                    nOfCharsToErase -= 1;
                }
                // -1 if

                Erase(nOfCharsToErase);
            }
            string transilteratedText = base.Transliterate(text);
            return transilteratedText;
        }
    }
}