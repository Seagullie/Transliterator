using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Transliterator.Core.Keyboard.Tests
{
    [TestClass()]
    public class KeyboardInputGeneratorTests
    {
        private KeyboardHook _hook;
        private string _output = "";

        public KeyboardInputGeneratorTests()
        {
            _hook = new KeyboardHook();
            _hook.SkipUnicodeKeys = false;
            _hook.KeyDown += (sender, args) => { _output += args.Character; };
        }

        ~KeyboardInputGeneratorTests()
        {
            _hook.Dispose();
        }

        [TestInitialize()]
        public void Cleanup()
        {
            _output = "";
        }

        [TestMethod()]
        public void InjectText()
        {
            // Arrange
            string testString = "abcd";

            // Act
            KeyboardInputGenerator.TextEntry(testString);

            // Assert
            Thread.Sleep(100);  // wait for the event handler to run
            Assert.AreEqual(testString, _output);
        }

        [TestMethod()]
        public void InjectEmojis()
        {
            // Arrange
            string testString = "😀🤣😇";

            // Act
            KeyboardInputGenerator.TextEntry(testString);

            // Assert
            Thread.Sleep(100);  // wait for the event handler to run
            Assert.AreEqual(testString, _output);
        }

        [TestMethod()]
        public void InjectOsuEmojis()
        {
            // Arrange
            string testString = "😃😛😥😊😎😭👎😏😈👍😑✋😀😠👼😬😡😆😖😍😮😯";

            // Act
            KeyboardInputGenerator.TextEntry(testString);

            // Assert
            Thread.Sleep(100);  // wait for the event handler to run
            Assert.AreEqual(testString, _output);
        }

        [TestMethod()]
        public void InjectUnicode()
        {
            // Arrange
            string testString = "♂☢–⛤";

            // Act
            KeyboardInputGenerator.TextEntry(testString);

            // Assert
            Thread.Sleep(100);  // wait for the event handler to run
            Assert.AreEqual(testString, _output);
        }
    }
}