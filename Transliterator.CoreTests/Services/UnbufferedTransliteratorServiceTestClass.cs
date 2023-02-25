using Transliterator.Core.Keyboard;

namespace Transliterator.Core.Services.Tests
{
    internal class UnbufferedTransliteratorServiceTestClass : UnbufferedTransliteratorService
    {
        public string transliterationResults = "";

        private static UnbufferedTransliteratorServiceTestClass _instance = null;

        public static new UnbufferedTransliteratorServiceTestClass GetInstance()
        {
            _instance ??= new UnbufferedTransliteratorServiceTestClass();
            return _instance;
        }

        public UnbufferedTransliteratorServiceTestClass() : base()
        {
            // state may depend on settings or whatever, so better set it to true here
            State = true;
            AllowUnicode();
        }

        // repalces base method with logging function for input param
        public override string EnterTransliterationResults(string text)
        {
            transliterationResults += text;
            return text;
        }

        // decorates base SkipIrrelevant
        public override bool SkipIrrelevant(object? sender, KeyboardHookEventArgs e)
        {
            bool skipped = base.SkipIrrelevant(sender, e);
            if (skipped)
            {
                transliterationResults += e.Character;
            }

            return skipped;
        }

        // mock erase
        public override void Erase(int times)
        {
            if (transliterationResults.Length == 1)
            {
                transliterationResults = string.Empty;
                return;
            }

            transliterationResults.Remove((transliterationResults.Length - 1) - times);
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

        // decorated SuppressKeypress
        //protected override void SuppressKeypress(KeyboardHookEventArgs e)
        //{
        //    base.SuppressKeypress(e);
        //    // if character was skipped to the system as part of a combo, log it for full input picture
        //    if (!e.Handled)
        //    {
        //        transliterationResults += e.Character;
        //    }
        //}

        protected override void HandleKeyPressed(object? sender, KeyboardHookEventArgs e)
        {
            base.HandleKeyPressed(sender, e);
            // if character was skipped to the system as part of a combo, log it for full input picture
            if (!e.Handled)
            {
                e.Handled = true;
                transliterationResults += e.Character;
            }
        }
    }
}