using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transliterator.Core.Models;
using Transliterator.Core.Services;

namespace Transliterator.CoreTests.Models;

[TestClass]
public class TransliterationTableTests
{
    private TransliterationTable transliterationTable;

    public TransliterationTableTests()
    {
        ClassInitialize();
    }

    // TODO: Fix ClassInitialize attribute
    //[ClassInitialize]
    public void ClassInitialize()
    {
        string relativePathToJsonFile = Path.Combine(ITransliteratorService.StandardTransliterationTablesPath, "tableLAT-UKR" + ".json");
        Dictionary<string, string> replacementMap = FileService.Read<Dictionary<string, string>>(AppDomain.CurrentDomain.BaseDirectory, relativePathToJsonFile);
        transliterationTable = new TransliterationTable(replacementMap);
    }

    [TestMethod]
    public void IsInAlphabetTest()
    {
        // Arrange
        string character = "h";

        // Act
        bool isInAlphabet = transliterationTable.Alphabet.Contains(character);

        // Assert
        Assert.IsTrue(isInAlphabet);
    }

    [TestMethod]
    public void IsNotInAlphabetTest()
    {
        // Arrange
        string character = "";

        // Act
        bool isInAlphabet = transliterationTable.Alphabet.Contains(character);

        // Assert
        Assert.IsFalse(isInAlphabet);
    }

    [TestMethod]
    public void EndsWithBrokenMultiGraphTest()
    {
        // Arrange
        string text = "sCk";

        // Act
        bool isBrokenCombo = transliterationTable.IsBrokenMultiGraph(text);

        // Assert
        Assert.IsTrue(isBrokenCombo);
    }

    [TestMethod]
    public void IsAddingUpToMultiGraphTest()
    {
        // Arrange
        string prefix = "SC";
        string text = "H";

        // Act
        bool isAddingUpToCombo = transliterationTable.IsAddingUpToMultiGraph(prefix, text);

        // Assert
        Assert.IsTrue(isAddingUpToCombo);
    }

    [TestMethod]
    public void IsMultiGraphTest()
    {
        // Arrange
        string combo = "sCh";
        string notCombo = "hCs";

        // Act
        bool isCombo = transliterationTable.IsMultiGraph(combo);
        bool isNotCombo = !transliterationTable.IsMultiGraph(notCombo);

        // Assert

        Assert.IsTrue(isCombo);
        Assert.IsTrue(isNotCombo);
    }

    [TestMethod]
    public void IsMultiGraphFinisherTest()
    {
        // Arrange
        string character = "H";

        // Act
        bool isComboFinisher = transliterationTable.IsMultiGraphFinisher(character);

        // Assert
        Assert.IsTrue(isComboFinisher);
    }

    [TestMethod]
    public void IsPartOfMultiGraphTest()
    {
        // Arrange
        string comboPartSingleChar = "C";
        string comboPartSeveralChars1 = "SC";
        string comboPartSeveralChars2 = "CH";

        // Act
        bool isSingleCharPartOfCombination = transliterationTable.IsPartOfMultiGraph(comboPartSingleChar);
        bool isSeveralChars1PartOfCombination = transliterationTable.IsPartOfMultiGraph(comboPartSeveralChars1);
        bool isSeveralChars2PartOfCombination = transliterationTable.IsPartOfMultiGraph(comboPartSeveralChars2);

        // Assert
        Assert.IsTrue(isSingleCharPartOfCombination);
        Assert.IsTrue(isSeveralChars1PartOfCombination);
        Assert.IsTrue(isSeveralChars2PartOfCombination);
    }

    [TestMethod]
    public void IsStartOfMultiGraphTest()
    {
        // Arrange
        string MultiGraphStartSingleChar = "C";
        string MultiGraphStartSeveralChars1 = "SC";

        // Act
        bool isSingleCharPartOfMultiGraph = transliterationTable.IsPartOfMultiGraph(MultiGraphStartSingleChar);
        bool isSeveralChars1PartOfMultiGraph = transliterationTable.IsPartOfMultiGraph(MultiGraphStartSeveralChars1);

        // Assert
        Assert.IsTrue(isSingleCharPartOfMultiGraph);
        Assert.IsTrue(isSeveralChars1PartOfMultiGraph);
    }

    [TestMethod]
    public void IsIsolatedGraphemeTest()
    {
        // Arrange
        string oneToOneReplacementChars = "kolo";
        string notOneToOneReplacementChars = "zhcj";

        // Act & assert
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