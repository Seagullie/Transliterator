using Transliterator.Core.Enums;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Models;

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
        // exceptions apply
        // general overview:
        // we're working at grapheme level here
        // there are two types of graphemes: isolated and multigraph
        // isolated graphemes are transliterated right away
        // multigraph graphemes are skipped, if they start or contribute to a multigraph.
        // if they finish a multigraph, then they are suppressed and entire multigraph is transliterated
        protected override void SuppressKeypress(KeyboardHookEventArgs e)
        {
            string renderedCharacter = e.Character;
            string bufferAsString = buffer.GetAsString();
            TransliterationTable table = TransliterationTable;

            // unless that's Isolated Grapheme
            // or current (one that's in buffer) MultiGraph finisher
            if (TransliterationTable.IsIsolatedGrapheme(renderedCharacter) || table.IsMultiGraph(bufferAsString))
            {
                e.Handled = true;
            }

            // or MultiGraph grapheme outside MultiGraph
            // those can be transliterated instantly. Unless they initiate a MultiGraph
            else if (!table.IsStartOfMultiGraph(bufferAsString) && table.IsMultiGraphGrapheme(renderedCharacter))
            {
                e.Handled = true;
            }
        }

        public override string Transliterate(string text)
        {
            TransliterationTable table = TransliterationTable;

            // TODO: Refactor erase block into its own function
            // -- Erase Block --

            // Main use case for erase is when there is a need to delete graphemes leading up to MultiGraph

            int nOfCharsToErase = text.Length;
            if (table.IsMultiGraph(text)) nOfCharsToErase -= 1;

            // TODO: Annotate
            else if (table.IsGrapheme(text) && !buffer.MultiGraphBrokenEventIsBeingHandled) nOfCharsToErase = 0;
            Erase(nOfCharsToErase);

            // -- Erase Block --

            string transilteratedText = base.Transliterate(text);
            return transilteratedText;
        }
    }
}