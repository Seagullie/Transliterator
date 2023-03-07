using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Models;

namespace Transliterator.Core.Services.Tests
{
    [TestClass()]
    public class BufferedTransliteratorServiceTests
    {
        private static BufferedTransliteratorServiceTestClass _transliteratorService;

        public BufferedTransliteratorServiceTests()
        {
            _transliteratorService = BufferedTransliteratorServiceTestClass.GetInstance();

            // TODO: Write tests for other tables
            _transliteratorService.TransliterationTable = new TransliterationTable(_transliteratorService.ReadReplacementMapFromJson("Resources/TranslitTables/tableLAT-UKR"));
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

        [ClassCleanup()]
        public static void ClassTeardown()
        {
            _transliteratorService.DisposeOfKeyDownEventHandler();
        }

        [TestMethod]
        public void TestLongWord()
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
        public void TestMultiGraphBreakByIsolatedGrapheme()
        {
            // arrange
            string combo = "jak";
            char nonComboChar = combo[^1];

            // act
            KeyboardInputGenerator.TextEntry(combo);
            // make sure last char in test string does not belong to a combo
            Assert.IsFalse(_transliteratorService.transliterationTable.IsPartOfMultiGraph(nonComboChar.ToString()), $"{nonComboChar} belongs to a combo");

            // assert
            string expected = "як";
            Assert.AreEqual(expected, _transliteratorService.transliterationResults);
        }

        // MG = MultiGraph
        [TestMethod]
        public void TestMGBreakByMGInit()
        {
            // arrange
            string testString = "Cjatka";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "Цятка";
            Assert.AreEqual(expected, _transliteratorService.transliterationResults);
        }

        // TODO: Rename
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

        [TestMethod]
        public void TestIsolatedGraphemesWord()
        {
            // arrange
            string testString = "kolo";

            // act
            // make sure each char in test string does not belong to a combo
            foreach (char chr in testString) Assert.IsFalse(_transliteratorService.transliterationTable.IsPartOfMultiGraph(chr.ToString()), $"{chr} belongs to a combo");

            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "коло";
            Assert.AreEqual(expected, _transliteratorService.transliterationResults);
        }

        [TestMethod()]
        public void TestMultiGraph()
        {
            // arrange
            string testString = "schuka";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "щука";
            Assert.AreEqual(expected, _transliteratorService.transliterationResults);
        }

        [TestMethod()]
        public void TestUppercaseMultiGraph()
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
        public void TestMultiGraphBreakByPunctuation()
        {
            // arrange
            string testString = "sc!";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "сц!";
            Assert.AreEqual(expected, _transliteratorService.transliterationResults);
        }

        [TestMethod]
        public void TestSeveralWords()
        {
            // arrange
            string testString = "Odynadcjatytomnyj slovnyk ukrajins'koji movy.";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "Одинадцятитомний словник української мови.";
            Assert.AreEqual(expected, _transliteratorService.transliterationResults);
        }
    }
}