using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Transliterator.Core.Keyboard.Tests;

[TestClass]
public class KeyboardInputGeneratorTests
{
    private readonly IKeyboardInputGenerator _generator;
    private KeyboardHook _hook;
    private string _output = "";

    // TODO: Add ClassInitialize attribute
    public KeyboardInputGeneratorTests()
    {
        _generator = new KeyboardInputGenerator();
        _hook = new KeyboardHook();
        _hook.SkipUnicodeKeys = false;
        _hook.KeyDown += (sender, args) => { _output += args.Character; };
    }

    // TODO: Add ClassCleanup attribute
    ~KeyboardInputGeneratorTests()
    {
        _hook.Dispose();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _output = "";
    }

    [TestMethod]
    public void InjectText()
    {
        // Arrange
        string testString = "abcd";

        // Act
        _generator.TextEntry(testString);

        // Assert
        Assert.AreEqual(testString, _output);
    }

    [TestMethod]
    public void InjectEmojis()
    {
        // Arrange
        string testString = "😀🤣😇";

        // Act
        _generator.TextEntry(testString);

        // Assert
        Assert.AreEqual(testString, _output);
    }

    [TestMethod]
    public void InjectOsuEmojis()
    {
        // Arrange
        string testString = "😃😛😥😊😎😭👎😏😈👍😑✋😀😠👼😬😡😆😖😍😮😯";

        // Act
        _generator.TextEntry(testString);

        // Assert
        Assert.AreEqual(testString, _output);
    }

    [TestMethod]
    public void InjectUnicode()
    {
        // Arrange
        string testString = "♂☢–⛤";

        // Act
        _generator.TextEntry(testString);

        // Assert
        Assert.AreEqual(testString, _output);
    }
}