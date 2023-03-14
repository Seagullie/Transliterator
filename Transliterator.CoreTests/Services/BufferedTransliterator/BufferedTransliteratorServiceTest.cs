using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transliterator.Core.Models;
using Transliterator.Core.Services;
using Transliterator.CoreTests.Fakes;

namespace Transliterator.CoreTests.Services.BufferedTransliterator;

[TestClass]
public class BufferedTransliteratorServiceTest
{
    private FakeKeyboardHook fakeKeyboardHook;
    private FakeKeyboardInputGenerator fakeKeyboardInputGenerator;
    private BufferedTransliteratorService bufferedTransliteratorService;
    private TransliterationTable transliterationTable;

    public BufferedTransliteratorServiceTest()
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
    public void TestLongWord()
    {
        // Arrange
        string testString = "Odynadcjatytomnyj";

        // Act
        fakeKeyboardHook.TextEntry(testString);
        fakeKeyboardHook.TextEntry(" "); // Break a combo

        // Assert
        string expected = "Одинадцятитомний";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    [TestMethod]
    public void TestMultiGraphBreakByIsolatedGrapheme()
    {
        // Arrange
        string brokenMG = "scm";
        char isolatedGrapheme = brokenMG[^1];

        // Act
        fakeKeyboardHook.TextEntry(brokenMG);
        // Make sure last char in test string is isolated grapheme
        Assert.IsTrue(transliterationTable.IsIsolatedGrapheme(isolatedGrapheme), $"{isolatedGrapheme} is not an isolated grapheme");

        // Assert
        string expected = "сцм";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    // MG = MultiGraph
    [TestMethod]
    public void TestMGBreakByMGInit()
    {
        // Arrange
        string testString = "Cjatka";

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "Цятка";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    // TODO: Rename
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

    [TestMethod]
    public void TestIsolatedGraphemesWord()
    {
        // Arrange
        string testString = "kolo";

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Make sure each char in test string does not belong to a combo
        foreach (char chr in testString)
            Assert.IsFalse(transliterationTable.IsPartOfMultiGraph(chr.ToString()), $"{chr} belongs to a combo");

        // Assert
        string expected = "коло";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    [TestMethod]
    public void TestMultiGraph()
    {
        // Arrange
        string testString = "schuka";

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "щука";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

    [TestMethod]
    public void TestUppercaseMultiGraph()
    {
        // Arrange
        string testString = "Schuka";

        // Act
        fakeKeyboardHook.TextEntry(testString);

        // Assert
        string expected = "Щука";
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }

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

    [TestMethod]
    public void SkipNoCaseCharIfShiftIsDown()
    {
        // Arrange
        string testString = "'";

        // Act
        FakeKeyboardHook.IsShiftDown = true;
        fakeKeyboardHook.TextEntry(testString);
        FakeKeyboardHook.IsShiftDown = false;

        // Assert
        string expected = string.Empty;
        Assert.AreEqual(expected, fakeKeyboardInputGenerator.Result);
    }
}