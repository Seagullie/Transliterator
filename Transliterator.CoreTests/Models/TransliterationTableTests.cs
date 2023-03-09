using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transliterator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Transliterator.Core.Models.Tests
{
    [TestClass()]
    public class TransliterationTableTests
    {
        public TransliterationTable transliterationTable;

        public TransliterationTableTests()
        {
            transliterationTable = new(ReadReplacementMapFromJson("Resources/TranslitTables/tableLAT-UKR"));
        }

        // TODO: Move to Utilities or something
        public Dictionary<string, string> ReadReplacementMapFromJson(string fileName)
        {
            string TableAsString = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{fileName}.json"));
            dynamic deserializedTableObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(TableAsString);
            Dictionary<string, string> TableAsDictionary = deserializedTableObj;

            return TableAsDictionary;
        }

        [TestMethod()]
        public void IsInAlphabetTest()
        {
            // arrange
            string character = "H";

            // act
            bool isInAlphabet = transliterationTable.Alphabet.Contains(character);

            // assert

            Assert.IsTrue(isInAlphabet);
        }

        [TestMethod()]
        public void EndsWithBrokenMultiGraphTest()
        {
            // arrange
            string text = "sCk";

            // act
            bool isBrokenCombo = transliterationTable.IsBrokenMultiGraph(text);

            // assert
            Assert.IsTrue(isBrokenCombo);
        }

        [TestMethod()]
        public void EndsWithMultiGraphInitTest()
        {
            // arrange
            string text = "cJ";

            // act
            bool endsWithComboInit = transliterationTable.EndsWithMultiGraphInit(text);

            // assert
            Assert.IsTrue(endsWithComboInit);
        }

        [TestMethod()]
        public void IsAddingUpToMultiGraphTest()
        {
            // arrange
            string prefix = "SC";
            string text = "H";

            // act
            bool isAddingUpToCombo = transliterationTable.IsAddingUpToMultiGraph(prefix, text);

            // assert
            Assert.IsTrue(isAddingUpToCombo);
        }

        [TestMethod()]
        public void IsMultiGraphTest()
        {
            // arrange
            string combo = "sCh";
            string notCombo = "hCs";

            // act
            bool isCombo = transliterationTable.IsMultiGraph(combo);
            bool isNotCombo = !transliterationTable.IsMultiGraph(notCombo);

            // assert

            Assert.IsTrue(isCombo);
            Assert.IsTrue(isNotCombo);
        }

        [TestMethod()]
        public void IsMultiGraphFinisherTest()
        {
            // arrange
            string character = "H";

            // act
            bool isComboFinisher = transliterationTable.IsMultiGraphFinisher(character);

            // assert
            Assert.IsTrue(isComboFinisher);
        }

        [TestMethod()]
        public void IsPartOfMultiGraphTest()
        {
            // arrange
            string comboPartSingleChar = "C";
            string comboPartSeveralChars1 = "SC";
            string comboPartSeveralChars2 = "CH";

            // act
            bool isSingleCharPartOfCombination = transliterationTable.IsPartOfMultiGraph(comboPartSingleChar);
            bool isSeveralChars1PartOfCombination = transliterationTable.IsPartOfMultiGraph(comboPartSeveralChars1);
            bool isSeveralChars2PartOfCombination = transliterationTable.IsPartOfMultiGraph(comboPartSeveralChars2);

            // assert
            Assert.IsTrue(isSingleCharPartOfCombination);
            Assert.IsTrue(isSeveralChars1PartOfCombination);
            Assert.IsTrue(isSeveralChars2PartOfCombination);
        }

        [TestMethod()]
        public void IsStartOfMultiGraphTest()
        {
            // arrange
            string MultiGraphStartSingleChar = "C";
            string MultiGraphStartSeveralChars1 = "SC";

            // act
            bool isSingleCharPartOfMultiGraph = transliterationTable.IsPartOfMultiGraph(MultiGraphStartSingleChar);
            bool isSeveralChars1PartOfMultiGraph = transliterationTable.IsPartOfMultiGraph(MultiGraphStartSeveralChars1);

            // assert
            Assert.IsTrue(isSingleCharPartOfMultiGraph);
            Assert.IsTrue(isSeveralChars1PartOfMultiGraph);
        }

        [TestMethod()]
        public void IsIsolatedGraphemeTest()
        {
            // arrange
            string oneToOneReplacementChars = "kolo";
            string notOneToOneReplacementChars = "zhcj";

            // act & assert
            foreach (char ch in oneToOneReplacementChars)
            {
                Assert.IsTrue(transliterationTable.IsIsolatedGrapheme(ch.ToString()), $"{ch} is not one-to-one replacement char");
            }
            foreach (char ch in notOneToOneReplacementChars)
            {
                Assert.IsFalse(transliterationTable.IsIsolatedGrapheme(ch.ToString()), $"{ch} is one-to-one replacement char");
            }
        }
    }
}