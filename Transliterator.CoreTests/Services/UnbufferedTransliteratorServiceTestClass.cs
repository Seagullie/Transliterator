using Transliterator.Core.Keyboard;

namespace Transliterator.Core.Services.Tests
{
    internal class UnbufferedTransliteratorServiceTestClass : UnbufferedTransliteratorService
    {
        public string IO = ""; // user input + transliterator output

        private static UnbufferedTransliteratorServiceTestClass _instance = null;

        public static new UnbufferedTransliteratorServiceTestClass GetInstance()
        {
            _instance ??= new UnbufferedTransliteratorServiceTestClass();
            return _instance;
        }

        public UnbufferedTransliteratorServiceTestClass() : base()
        {
            // state may depend on settings or whatever, so better set it to true here
            TransliterationEnabled = true;
            AllowUnicode();
        }

        // repalces base method with logging function for input param
        public override string EnterTransliterationResults(string text)
        {
            IO += text;
            return text;
        }

        // mock erase
        public override void Erase(int times)
        {
            if (times < 1)
            {
                return;
            }

            // TODO: Rewrite

            for (int i = 0; i < times; i++)
            {
                if (IO.Length > 0)
                {
                    IO = IO.Remove(IO.Length - 1);
                }
            }
        }

        public Dictionary<string, string> ReadReplacementMapFromJson(string fileName)
        {
            string TableAsString = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{fileName}.json"));
            dynamic deserializedTableObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(TableAsString);
            Dictionary<string, string> TableAsDictionary = deserializedTableObj;

            return TableAsDictionary;
        }

        // decorated AddToBuffer
        //protected override void AddToBuffer(string renderedCharacter)
        //{
        //    base.AddToBuffer(renderedCharacter);
        //    transliterationResults += renderedCharacter;
        //}

        //protected override void SuppressKeypress(KeyboardHookEventArgs e)
        //{
        //    base.SuppressKeypress(e);
        //    // if character was skipped to the system as part of a combo, log it for full input picture
        //    if (!e.Handled)
        //    {
        //        IO += e.Character;
        //    }
        //}

        protected override void HandleKeyPressed(object? sender, KeyboardHookEventArgs e)
        {
            base.HandleKeyPressed(sender, e);
            // if character was skipped to the system as part of a MultiGraph, log it for full input picture
            if (!e.Handled)
            {
                e.Handled = true;
                IO += e.Character;
            }
        }

        public void ClearBuffer()
        {
            buffer.Clear();
        }
    }
}