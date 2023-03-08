using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transliterator.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Enums;
using Transliterator.Core.Models;

namespace Transliterator.Core.Services.Tests
{
    // TODO: Inherit from Buffered Translit tests
    [TestClass()]
    public class UnbufferedTransliteratorServiceTests
    {
        private UnbufferedTransliteratorServiceTestClass _transliteratorService;

        public UnbufferedTransliteratorServiceTests()
        {
            _transliteratorService = UnbufferedTransliteratorServiceTestClass.GetInstance();

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
            _transliteratorService.IO = "";
            _transliteratorService.ClearBuffer();
        }

        [TestMethod]
        public void TestIsolatedGraphemeWord_()
        {
            // arrange
            string testString = "kolo";

            // act
            // make sure each char in test string is an isolated grapheme
            foreach (char chr in testString) Assert.IsTrue(_transliteratorService.TransliterationTable.IsIsolatedGrapheme(chr.ToString()), $"{chr} belongs to a combo");

            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "коло";
            Assert.AreEqual(expected, _transliteratorService.IO);
        }

        [TestMethod]
        public void TestSimple_()
        {
            // arrange
            string testString = "sonce";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "сонце";
            Assert.AreEqual(expected, _transliteratorService.IO);
        }

        // Todo: test all MG finishers
        [TestMethod]
        public void TestMultiGraphFinishers()
        {
            // arrange
            string testString = "h";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "г";
            Assert.AreEqual(expected, _transliteratorService.IO);
        }

        [TestMethod()]
        public void TestDiGraph()
        {
            // arrange
            string testString = "ch";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "ч";
            Assert.AreEqual(expected, _transliteratorService.IO);
        }

        // TODO: Test all DiGraphs by iterating over .DiGraphs
        [TestMethod()]
        public void TestDiGraphs()
        {
            // arrange
            string testString = "chzhsh";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "чжш";
            Assert.AreEqual(expected, _transliteratorService.IO);
        }

        // TODO: Test all TriGraphs by iterating over .TriGraphs
        [TestMethod()]
        public void TestTriGraphs()
        {
            // arrange
            string testString = "sch";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "щ";
            Assert.AreEqual(expected, _transliteratorService.IO);
        }

        // TODO: Test all MultiGraphs by iterating over .MultiGraphs
        [TestMethod()]
        public void TestMultiGraphs()
        {
            // arrange
            string testString = "chzhshsch";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "чжшщ";
            Assert.AreEqual(expected, _transliteratorService.IO);
        }

        [TestMethod]
        public void TestBufferStateOnIncompleteMultiGraph_()
        {
            // arrange
            string testString = "sc"; // sch

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "sc";
            Assert.AreEqual(expected, _transliteratorService.IO);
        }

        [TestMethod()]
        public void TestBrokenMultiGraph()
        {
            // arrange
            string testString = "ce"; // broken sch or ch

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "це";
            Assert.AreEqual(expected, _transliteratorService.IO);
        }

        // TODO: Rename
        [TestMethod()]
        public void TestUnnamed()
        {
            // arrange
            string testString = " k";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = " к";
            Assert.AreEqual(expected, _transliteratorService.IO);
        }

        // Note: This is copypaste from BufferedTransliteratorServiceTests
        [TestMethod()]
        public void TestMultiGraphBreakByPunctuation()
        {
            // arrange
            string testString = "sc!";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "сц!";
            Assert.AreEqual(expected, _transliteratorService.IO);
        }

        [TestMethod()]
        public void TestMultiGraphStartBreakByPunctuation()
        {
            // arrange
            string testString = "j!";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "й!";
            Assert.AreEqual(expected, _transliteratorService.IO);
        }

        // MG = MultiGraph
        // Note: This is copypaste from BufferedTransliteratorServiceTests
        [TestMethod]
        public void TestMGBreakByMGInit()
        {
            // arrange
            string testString = "cja";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "ця";
            Assert.AreEqual(expected, _transliteratorService.IO);
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
            Assert.AreEqual(expected, _transliteratorService.IO);
        }

        [TestMethod]
        public void TestTwoWords()
        {
            // arrange
            string testString = "Odynadcjatytomnyj slovnyk.";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "Одинадцятитомний словник.";
            Assert.AreEqual(expected, _transliteratorService.IO);
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
            Assert.AreEqual(expected, _transliteratorService.IO);
        }

        // TODO: Simulate Caps Lock in Test Class
        [TestMethod]
        public void TestNoCaseChars()
        {
            // arrange
            string testString = "SIL' ";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "СІЛЬ ";
            Assert.AreEqual(expected, _transliteratorService.IO);
        }
    }
}