using Transliterator.Core.Keyboard;

namespace Transliterator.Core.Services.Tests
{
    // TODO: Refactor into mocks once I learn how to use them properly
    internal class BufferedTransliteratorServiceTestClass : BufferedTransliteratorService
    {
        public string transliterationResults = "";

        private static BufferedTransliteratorServiceTestClass _instance = null;

        // warning: this hook does not override base class hook
        private KeyboardHook _keyboardHook;

        public static new BufferedTransliteratorServiceTestClass GetInstance()
        {
            _instance ??= new BufferedTransliteratorServiceTestClass();
            return _instance;
        }

        public BufferedTransliteratorServiceTestClass() : base()
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

        public Dictionary<string, string> ReadReplacementMapFromJson(string fileName)
        {
            string TableAsString = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{fileName}.json"));
            dynamic deserializedTableObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(TableAsString);
            Dictionary<string, string> TableAsDictionary = deserializedTableObj;

            return TableAsDictionary;
        }
    }
}