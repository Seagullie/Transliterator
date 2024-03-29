﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transliterator.Core.Enums;

namespace Transliterator.Core.Keyboard.Tests;

[TestClass]
public class KeyboardHookTests
{
    private KeyboardHook _hook;
    private KeyboardInputGenerator _inputGenerator;

    public KeyboardHookTests()
    {
        ClassInitialize();
    }

    // TODO: Fix ClassInitialize attribute
    //[ClassInitialize]
    public void ClassInitialize()
    {
        _inputGenerator = new KeyboardInputGenerator();
    }

    [TestInitialize]
    public void Setup()
    {
        _hook = new KeyboardHook();
    }

    [TestCleanup]
    public void Teardown()
    {
        _hook.Dispose();
    }

    [TestMethod]
    public void TestKeyDownEvent()
    {
        // Arrange
        bool eventFired = false;
        _hook.KeyDown += (sender, args) => { eventFired = true; };

        // Act
        _inputGenerator.KeyDown(VirtualKeyCode.KeyA);

        // Assert
        Assert.IsTrue(eventFired, "KeyDown event was not fired");
    }

    [TestMethod]
    public void TestUnicodeKeyDownEvent()
    {
        // Arrange
        bool eventFired = false;
        _hook.KeyDown += (sender, args) => { eventFired = true; };

        // Act
        _inputGenerator.TextEntry("A");

        // Assert
        Assert.IsFalse(eventFired, "Unicode key should not trigger KeyDown event");
    }

    [TestMethod]
    public void ListenToGeneratedKeys()
    {
        // Arrange
        string testString = "abcd";
        string outputString = "";

        _hook.SkipUnicodeKeys = false;
        _hook.KeyDown += (sender, args) => { outputString += args.Character; };

        // Act
        _inputGenerator.TextEntry(testString);

        // Assert
        Assert.AreEqual(testString, outputString);
    }

    [TestMethod]
    public void ListenToGeneratedDifferentCaseKeys()
    {
        // Arrange
        string testString = "aBcD";
        string outputString = "";

        _hook.SkipUnicodeKeys = false;
        _hook.KeyDown += (sender, args) => { outputString += args.Character; };

        // Act
        _inputGenerator.TextEntry(testString);

        // Assert
        Assert.AreEqual(testString, outputString);
    }

    [TestMethod]
    public void ListenToGeneratedCyrillicKeys()
    {
        // Arrange
        string testString = "абвгд";
        string outputString = "";

        _hook.SkipUnicodeKeys = false;
        _hook.KeyDown += (sender, args) => { outputString += args.Character; };

        // Act
        _inputGenerator.TextEntry(testString);

        // Assert
        Assert.AreEqual(testString, outputString);
    }

    [TestMethod]
    public void ListenToGeneratedPunctuation()
    {
        // Arrange
        string testString = ";!_#";
        string outputString = "";

        _hook.SkipUnicodeKeys = false;
        _hook.KeyDown += (sender, args) => { outputString += args.Character; };

        // Act
        _inputGenerator.TextEntry(testString);

        // Assert
        Assert.AreEqual(testString, outputString);
    }
}