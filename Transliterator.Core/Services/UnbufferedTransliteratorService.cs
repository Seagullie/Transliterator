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
    // (except for Isolated Graphemes. Those can be transliterated right away so there is no need to pass and instantly erase a character)
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

            //unless that's Isolated Grapheme
            if (transliterationTable.IsIsolatedGrapheme(e.Character))
            {
                e.Handled = true;
            }

            //// or current (one that's in buffer) MultiGraph finisher
            //else if (transliterationTable.IsAddingUpToMultiGraph(buffer.GetAsString(), e.Character))
            //{
            //    e.Handled = true;
            //}
        }

        // should never defer as it's unbuffered version
        //protected override bool ShouldDeferTransliteration()
        //{
        //    return false;
        //}

        public override string Transliterate(string text)
        {
            // Isolated Graphemes are still buffered
            //if (!transliterationTable.IsIsolatedGrapheme(text))
            //{
            //    int nOfCharsToErase = text.Length;
            //    // MultiGraph finishers are also suppressed, so no need to erase them
            //    if (transliterationTable.IsMultiGraph(text))
            //    {
            //        nOfCharsToErase -= 1;
            //    }
            //    // -1 if
            //}

            int nOfCharsToErase = text.Length;
            Erase(nOfCharsToErase);

            string transilteratedText = base.Transliterate(text);
            return transilteratedText;
        }
    }
}