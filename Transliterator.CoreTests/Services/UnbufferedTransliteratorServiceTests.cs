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
    // TODO: Fix test inheritance
    [TestClass()]
    public class UnbufferedTransliteratorServiceTests : BufferedTransliteratorServiceTests
    {
        private UnbufferedTransliteratorServiceTestClass _transliteratorService;

        public UnbufferedTransliteratorServiceTests()
        {
            _transliteratorService = UnbufferedTransliteratorServiceTestClass.GetInstance();

            // TODO: Write tests for other tables
            _transliteratorService.TransliterationTable = new TransliterationTable(_transliteratorService.ReadReplacementMapFromJson("Resources/TranslitTables/tableLAT-UKR"));
        }

        [TestInitialize()]
        public new void Initialize()
        {
            // Runs before each test
        }

        [TestCleanup()]
        public new void Teardown()
        {
            _transliteratorService.transliterationResults = "";
        }

        [TestMethod]
        public new void TestNoComboCharsWord_()
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

        [TestMethod]
        public void TestSimple_()
        {
            // arrange
            string testString = "sonce";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "сонце";
            Assert.AreEqual(expected, _transliteratorService.transliterationResults);
        }

        // Todo: test all combofinishers
        [TestMethod]
        public void TestComboFinishers()
        {
            // arrange
            string testString = "h";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "г";
            Assert.AreEqual(expected, _transliteratorService.transliterationResults);
        }

        [TestMethod()]
        public void TestSimpleCombo()
        {
            // arrange
            string testString = "ch";

            // act
            KeyboardInputGenerator.TextEntry(testString);

            // assert
            string expected = "ч";
            Assert.AreEqual(expected, _transliteratorService.transliterationResults);
        }
    }
}