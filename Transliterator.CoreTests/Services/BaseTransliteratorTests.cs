using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transliterator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transliterator.Core.Models;
using Transliterator.Core.Helpers.Exceptions;

// https://learn.microsoft.com/en-us/visualstudio/test/unit-test-basics?view=vs-2022

namespace Transliterator.Services.Tests
{
    [TestClass()]
    public class BaseTransliteratorTests

    {
        private string replacementTablePath;
        private BaseTransliterator baseTransliterator;

        public BaseTransliteratorTests()
        {
            replacementTablePath = "Resources/TranslitTables/tableLAT-UKR.json";
            baseTransliterator = new();
            baseTransliterator.SetTableModel(replacementTablePath);
        }

        public BaseTransliterator InstantiateTranslit(string replacementTablePath)
        {
            BaseTransliterator baseTransliterator = new();
            baseTransliterator.SetTableModel(replacementTablePath);

            return baseTransliterator;
        }

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            // Executes once for the test class. (Optional)
        }

        [TestInitialize]
        public void Initialize()
        {
            // Runs before each test. (Optional)
        }

        [TestMethod()]
        public void Transliterate()
        {
            // arrange
            string testString = "test raz dva";

            // act
            string transliterationResult = baseTransliterator.Transliterate(testString);

            // assert
            string expected = "тест раз два";
            Assert.AreEqual(expected, transliterationResult);
        }

        [TestMethod()]
        public void TransliterateUPPERCASE()
        {
            // arrange
            string testString = "TEST RAZ DVA";

            // act
            string transliterationResult = baseTransliterator.Transliterate(testString);

            // assert
            string expected = "ТЕСТ РАЗ ДВА";
            Assert.AreEqual(expected, transliterationResult);
        }

        [TestMethod()]
        public void TransliterateTitleCase()
        {
            // arrange
            string testString = "tEst Raz dvA";

            // act
            string transliterationResult = baseTransliterator.Transliterate(testString);

            // assert
            string expected = "тЕст Раз двА";
            Assert.AreEqual(expected, transliterationResult);
        }

        [TestMethod()]
        public void IgnoreCharactersNotInTable()
        {
            // arrange
            string testString = "test кирилиця ![dva]!";

            // act
            string transliterationResult = baseTransliterator.Transliterate(testString);

            // assert
            string expected = "тест кирилиця ![два]!";
            Assert.AreEqual(expected, transliterationResult);
        }

        [TestMethod()]
        public void ThrowIfTableNotSet()
        {
            TransliterationTable table = baseTransliterator.transliterationTable;
            baseTransliterator.transliterationTable = null;

            Assert.ThrowsException<TableNotSetException>(() => baseTransliterator.Transliterate("random"));

            baseTransliterator.transliterationTable = table;
        }

        [TestMethod()]
        public void TransliterateCombos()
        {
            // arrange
            string testString = "sch";

            // act
            string transliterationResult = baseTransliterator.Transliterate(testString);

            // assert
            string expected = "щ";
            Assert.AreEqual(expected, transliterationResult);
        }

        [TestMethod()]
        public void TransliterateWordsWithApostrophe()
        {
            // arrange
            string testString = "pir''ja, suzir''ja, matir''ju, bur''jan";

            // act
            string transliterationResult = baseTransliterator.Transliterate(testString);

            // assert
            string expected = "пір'я, сузір'я, матір'ю, бур'ян";
            Assert.AreEqual(expected, transliterationResult);
        }

        [TestMethod()]
        public void TransliterateWordsWithApostropheAndSoftSign()
        {
            // arrange
            string testString = "pir''ja, kysil'";

            // act
            string transliterationResult = baseTransliterator.Transliterate(testString);

            // assert
            string expected = "пір'я, кисіль";
            Assert.AreEqual(expected, transliterationResult);
        }
    }
}