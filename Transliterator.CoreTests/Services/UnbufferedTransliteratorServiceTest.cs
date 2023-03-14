using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transliterator.Core.Models;
using Transliterator.Core.Services;
using Transliterator.CoreTests.Fakes;

namespace Transliterator.CoreTests.Services;

[TestClass]
public class UnbufferedTransliteratorServiceTest
{
    private FakeKeyboardHook fakeKeyboardHook;
    private FakeKeyboardInputGenerator fakeKeyboardInputGenerator;
    private BufferedTransliteratorService bufferedTransliteratorService;
    private TransliterationTable transliterationTable;

    public UnbufferedTransliteratorServiceTest()
    {
        ClassInitialize();
    }

    // TODO: Fix ClassInitialize attribute
    //[ClassInitialize]
    public void ClassInitialize()
    {
        fakeKeyboardHook = new FakeKeyboardHook();
        fakeKeyboardInputGenerator = new FakeKeyboardInputGenerator();

        string relativePathToJsonFile = Path.Combine(ITransliteratorService.StandardTransliterationTablesPath, "tableLAT-UKR" + ".json");
        Dictionary<string, string> replacementMap = FileService.Read<Dictionary<string, string>>(AppDomain.CurrentDomain.BaseDirectory, relativePathToJsonFile);
        transliterationTable = new TransliterationTable(replacementMap);
    }

    [TestInitialize]
    public void TestInitialize()
    {
        bufferedTransliteratorService = new BufferedTransliteratorService(fakeKeyboardHook, fakeKeyboardInputGenerator);
        bufferedTransliteratorService.TransliterationTable = transliterationTable;
    }

    [TestCleanup]
    public void TestCleanup()
    {
        fakeKeyboardInputGenerator.ClearBuffer();
    }

    [TestMethod]
    public void TestIsolatedGraphemeWord()
    {
        // Arrange
        string testString = "kolo";

        // Make sure each char in test string is an isolated grapheme
        foreach (char chr in testString) 
            Assert.IsTrue(transliterationTable.IsIsolatedGrapheme(chr), $"{chr} belongs to a combo");

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "коло";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    [TestMethod]
    public void TestSimple()
    {
        // Arrange
        string testString = "sonce";

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "сонце";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    // Todo: test all MG finishers
    [TestMethod]
    public void TestMultiGraphFinishers()
    {
        // Arrange
        string testString = "h";

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "г";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    [TestMethod]
    public void TestDiGraph()
    {
        // Arrange
        string testString = "ch";

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "ч";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    // TODO: Test all DiGraphs by iterating over .DiGraphs
    [TestMethod]
    public void TestDiGraphs()
    {
        // Arrange
        string testString = "chzhsh";

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "чжш";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    // TODO: Test all TriGraphs by iterating over .TriGraphs
    [TestMethod]
    public void TestTriGraphs()
    {
        // Arrange
        string testString = "sch";

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "щ";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    // TODO: Test all MultiGraphs by iterating over .MultiGraphs
    [TestMethod]
    public void TestMultiGraphs()
    {
        // Arrange
        string testString = "chzhshsch";

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "чжшщ";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    [Ignore]
    [TestMethod]
    public void TestBufferStateOnIncompleteMultiGraph()
    {
        // Arrange
        string testString = "sc"; // sch

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "sc";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    [TestMethod]
    public void TestBrokenMultiGraph()
    {
        // Arrange
        string testString = "ce"; // broken sch or ch

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "це";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    // TODO: Rename
    [TestMethod]
    public void TestUnnamed()
    {
        // Arrange
        string testString = " k";

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "к";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    // Note: This is copypaste from BufferedTransliteratorServiceTests
    [TestMethod]
    public void TestMultiGraphBreakByPunctuation()
    {
        // Arrange
        string testString = "sc!";

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "сц";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    [TestMethod]
    public void TestMultiGraphStartBreakByPunctuation()
    {
        // Arrange
        string testString = "j!";

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "й";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    // MG = MultiGraph
    // Note: This is copypaste from BufferedTransliteratorServiceTests
    [TestMethod]
    public void TestMGBreakByMGInit()
    {
        // Arrange
        string testString = "cja";

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "ця";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    [TestMethod]
    public void TestLongWord()
    {
        // Arrange
        string testString = "Odynadcjatytomnyj.";

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "Одинадцятитомний";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    [TestMethod]
    public void TestTwoWords()
    {
        // Arrange
        string testString = "Odynadcjatytomnyj slovnyk.";

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "Одинадцятитомнийсловник";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    [TestMethod]
    public void TestSeveralWords()
    {
        // Arrange
        string testString = "Odynadcjatytomnyj slovnyk ukrajins'koji movy.";

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "Одинадцятитомнийсловникукраїнськоїмови";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    // TODO: Simulate Caps Lock in Test Class
    [Ignore]
    [TestMethod]
    public void TestNoCaseChars()
    {
        // Arrange
        string testString = "SIL' ";

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "СІЛЬ ";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }
}
