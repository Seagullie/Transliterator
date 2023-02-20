using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Models;

namespace Transliterator.Core.Services.Tests
{
    // TODO: Refactor into mocks once I learn how to use them properly
    internal class BufferedTransliteratorServiceTestClass : BufferedTransliteratorService
    {
        public string transliterationResults = "";

        private static BufferedTransliteratorServiceTestClass _instance = null;

        public static new BufferedTransliteratorServiceTestClass GetInstance()
        {
            _instance ??= new BufferedTransliteratorServiceTestClass();
            return _instance;
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
    }

    [TestClass()]
    public class BufferedTransliteratorServiceTests
    {
        private BufferedTransliteratorServiceTestClass _transliteratorService;
        private int delayBetweenEachKeypress = 100;

        // TODO: Move to Utilities or something
        public Dictionary<string, string> ReadReplacementMapFromJson(string fileName)
        {
            string TableAsString = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{fileName}.json"));
            dynamic deserializedTableObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(TableAsString);
            Dictionary<string, string> TableAsDictionary = deserializedTableObj;

            return TableAsDictionary;
        }

        public BufferedTransliteratorServiceTests()
        {
            _transliteratorService = BufferedTransliteratorServiceTestClass.GetInstance();
            // state may depend on settings or whatever, so better set it to true here
            _transliteratorService.State = true;
            _transliteratorService.AllowUnicode();

            _transliteratorService.TransliterationTable = new TransliterationTable(ReadReplacementMapFromJson("Resources/TranslitTables/tableLAT-UKR"));
        }

        [TestInitialize()]
        public void Initialize()
        {
            // Runs before each test
        }

        [TestCleanup()]
        public void Teardown()
        {
            _transliteratorService.transliterationResults = "";
        }

        [TestMethod]
        public void TestComboBreakByOtherKey()
        {
            // arrange
            string testString = "Odynadcjatytomnyj.";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "Одинадцятитомний.";
            Assert.AreEqual(expected, _transliteratorService.transliterationResults);
        }

        [TestMethod]
        public void TestComboBreakByComboInit()
        {
            // arrange
            string testString = "Cjatka";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "Цятка";
            Assert.AreEqual(expected, _transliteratorService.transliterationResults);
        }

        [TestMethod]
        public void TestSimple()
        {
            // arrange
            string testString = "sonce";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "сонце";
            Assert.AreEqual(expected, _transliteratorService.transliterationResults);
        }

        [TestMethod()]
        public void TestUppercaseCombo()
        {
            // arrange
            string testString = "Schuka";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "Щука";
            Assert.AreEqual(expected, _transliteratorService.transliterationResults);
        }

        [TestMethod()]
        public void TestComboBreakByPunctuation()
        {
            // arrange
            string testString = "sc!";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "сц!";
            Assert.AreEqual(expected, _transliteratorService.transliterationResults);
        }
    }
}