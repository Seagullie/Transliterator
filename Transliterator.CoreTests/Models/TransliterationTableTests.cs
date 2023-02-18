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
        public void isInAlphabetTest()
        {
            // arrange
            string character = "H";

            // act
            bool isInAlphabet = transliterationTable.isInAlphabet(character);

            // assert

            Assert.IsTrue(isInAlphabet);
        }

        [TestMethod()]
        public void EndsWithBrokenComboTest()
        {
            // arrange
            string text = "sCk";

            // act
            bool isBrokenCombo = transliterationTable.EndsWithBrokenCombo(text);

            // assert
            Assert.IsTrue(isBrokenCombo);
        }

        [TestMethod()]
        public void EndsWithComboInitTest()
        {
            // arrange
            string text = "cJ";

            // act
            bool endsWithComboInit = transliterationTable.EndsWithComboInit(text);

            // assert
            Assert.IsTrue(endsWithComboInit);
        }

        [TestMethod()]
        public void IsAddingUpToComboTest()
        {
            // arrange
            string prefix = "SC";
            string text = "H";

            // act
            bool isAddingUpToCombo = transliterationTable.IsAddingUpToCombo(prefix, text);

            // assert
            Assert.IsTrue(isAddingUpToCombo);
        }

        [TestMethod()]
        public void IsComboTest()
        {
            // arrange
            string combo = "sCh";
            string notCombo = "hCs";

            // act
            bool isCombo = transliterationTable.IsCombo(combo);
            bool isNotCombo = !transliterationTable.IsCombo(notCombo);

            // assert

            Assert.IsTrue(isCombo);
            Assert.IsTrue(isNotCombo);
        }

        [TestMethod()]
        public void IsComboFinisherTest()
        {
            // arrange
            string character = "H";

            // act
            bool isComboFinisher = transliterationTable.IsComboFinisher(character);

            // assert
            Assert.IsTrue(isComboFinisher);
        }

        [TestMethod()]
        public void IsPartOfCombinationTest()
        {
            // arrange
            string comboPartSingleChar = "C";
            string comboPartSeveralChars1 = "SC";
            string comboPartSeveralChars2 = "CH";

            // act
            bool isSingleCharPartOfCombination = transliterationTable.IsPartOfCombination(comboPartSingleChar);
            bool isSeveralChars1PartOfCombination = transliterationTable.IsPartOfCombination(comboPartSeveralChars1);
            bool isSeveralChars2PartOfCombination = transliterationTable.IsPartOfCombination(comboPartSeveralChars2);

            // assert
            Assert.IsTrue(isSingleCharPartOfCombination);
            Assert.IsTrue(isSeveralChars1PartOfCombination);
            Assert.IsTrue(isSeveralChars2PartOfCombination);
        }

        [TestMethod()]
        public void IsStartOfCombinationTest()
        {
            // arrange
            string comboStartSingleChar = "C";
            string comboStartSeveralChars1 = "SC";

            // act
            bool isSingleCharPartOfCombination = transliterationTable.IsPartOfCombination(comboStartSingleChar);
            bool isSeveralChars1PartOfCombination = transliterationTable.IsPartOfCombination(comboStartSeveralChars1);

            // assert
            Assert.IsTrue(isSingleCharPartOfCombination);
            Assert.IsTrue(isSeveralChars1PartOfCombination);
        }
    }
}