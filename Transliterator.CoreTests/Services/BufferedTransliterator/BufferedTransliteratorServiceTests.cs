using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Models;

namespace Transliterator.Core.Services.Tests
{
    [TestClass()]
    public class BufferedTransliteratorServiceTests
    {
        private BufferedTransliteratorServiceTestClass _transliteratorService;

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

        [TestMethod]
        public void TestNoComboChars()
        {
            // arrange
            string testString = "kolo";

            // act
            // make sure each char in test string does not belong to a combo
            foreach (char chr in testString) Assert.IsFalse(_transliteratorService.transliterationTable.IsPartOfCombination(chr.ToString()), $"{chr} belongs to a combo");

            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "коло";
            Assert.AreEqual(expected, _transliteratorService.transliterationResults);
        }

        [TestMethod()]
        public void TestCombo()
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