using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transliterator.Core.Enums;

namespace Transliterator.Core.Keyboard.Tests
{
    [TestClass()]
    public class KeyboardHookTests
    {
        private KeyboardHook _hook;

        [TestInitialize()]
        public void Setup()
        {
            _hook = new KeyboardHook();
        }

        [TestCleanup()]
        public void Teardown()
        {
            _hook.Dispose();
        }

        [TestMethod()]
        public void TestKeyDownEvent()
        {
            // Arrange
            bool eventFired = false;
            _hook.KeyDown += (sender, args) => { eventFired = true; };

            // Act
            KeyboardInputGenerator.KeyDown(VirtualKeyCode.KeyA);

            // Assert
            Thread.Sleep(100);  // wait for the event handler to run
            Assert.IsTrue(eventFired, "KeyDown event was not fired");
        }

        [TestMethod()]
        public void TestUnicodeKeyDownEvent()
        {
            // Arrange
            bool eventFired = false;
            _hook.KeyDown += (sender, args) => { eventFired = true; };

            // Act
            KeyboardInputGenerator.TextEntry("A");

            // Assert
            Thread.Sleep(100);  // wait for the event handler to run
            Assert.IsFalse(eventFired, "Unicode key should not trigger KeyDown event");
        }

        [TestMethod()]
        public void ListenToGeneratedKeys()
        {
            // Arrange
            string testString = "abcd";
            string outputString = "";

            _hook.SkipUnicodeKeys = false;
            _hook.KeyDown += (sender, args) => { outputString += args.Character; };

            // Act
            KeyboardInputGenerator.TextEntry(testString);


            // Assert
            Thread.Sleep(100);  // wait for the event handler to run
            Assert.AreEqual(testString, outputString);
        }

        [TestMethod()]
        public void ListenToGeneratedDifferentCaseKeys()
        {
            // Arrange
            string testString = "aBcD";
            string outputString = "";

            _hook.SkipUnicodeKeys = false;
            _hook.KeyDown += (sender, args) => { outputString += args.Character; };

            // Act
            KeyboardInputGenerator.TextEntry(testString);

            // Assert
            Thread.Sleep(100);  // wait for the event handler to run
            Assert.AreEqual(testString, outputString);
        }

        [TestMethod()]
        public void ListenToGeneratedCyrillicKeys()
        {
            // Arrange
            string testString = "абвгд";
            string outputString = "";

            _hook.SkipUnicodeKeys = false;
            _hook.KeyDown += (sender, args) => { outputString += args.Character; };

            // Act
            KeyboardInputGenerator.TextEntry(testString);

            // Assert
            Thread.Sleep(100);  // wait for the event handler to run
            Assert.AreEqual(testString, outputString);
        }

        [TestMethod()]
        public void ListenToGeneratedPunctuation()
        {
            // Arrange
            string testString = ";!_#";
            string outputString = "";

            _hook.SkipUnicodeKeys = false;
            _hook.KeyDown += (sender, args) => { outputString += args.Character; };

            // Act
            KeyboardInputGenerator.TextEntry(testString);

            // Assert
            Thread.Sleep(100);  // wait for the event handler to run
            Assert.AreEqual(testString, outputString);
        }
    }
}