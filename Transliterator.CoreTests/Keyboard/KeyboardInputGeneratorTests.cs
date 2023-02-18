using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transliterator.CoreTests.Keyboard;

namespace Transliterator.Core.Keyboard.Tests
{
    [TestClass()]
    public class KeyboardInputGeneratorTests
    {
        public EventLoopForm testWindow;
        private int delayBetweenEachKeypress = 10;

        [TestInitialize]
        public void Initialize()
        {
            // Runs before each test
            testWindow = new();
            testWindow.AttachKeyboardHook();

            new Thread(() =>
            {
                testWindow.Show();
            }).Start();
        }

        [TestMethod()]
        public void InjectText()
        {
            // arrange
            string testString = "abcd";

            // act

            foreach (char c in testString)
            {
                KeyboardInputGenerator.TextEntry(c.ToString());
                Thread.Sleep(delayBetweenEachKeypress);
            }

            // assert
            string expected = "abcd";
            Assert.AreEqual(expected, testWindow.keyboardHookMemory);
        }

        [TestMethod()]
        public void InjectEmojis()
        {
            // arrange
            string testString = "😀🤣😇";

            // act

            foreach (char c in testString)
            {
                KeyboardInputGenerator.TextEntry(c.ToString());
                Thread.Sleep(delayBetweenEachKeypress);
            }

            // assert
            string expected = "😀🤣😇";
            Assert.AreEqual(expected, testWindow.keyboardHookMemory);
        }

        [TestMethod()]
        public void InjectOsuEmojis()
        {
            // arrange
            string testString = "😃😛😥😊😎😭👎😏😈👍😑✋😀😠👼😬😡😆😖😍😮😯";

            // act

            foreach (char c in testString)
            {
                KeyboardInputGenerator.TextEntry(c.ToString());
                Thread.Sleep(delayBetweenEachKeypress);
            }

            // assert
            string expected = "😃😛😥😊😎😭👎😏😈👍😑✋😀😠👼😬😡😆😖😍😮😯";
            Assert.AreEqual(expected, testWindow.keyboardHookMemory);
        }

        [TestMethod()]
        public void InjectUnicode()
        {
            // arrange
            string testString = "♂☢–⛤";

            // act

            foreach (char c in testString)
            {
                KeyboardInputGenerator.TextEntry(c.ToString());
                Thread.Sleep(delayBetweenEachKeypress);
            }

            // assert
            string expected = "♂☢–⛤";
            Assert.AreEqual(expected, testWindow.keyboardHookMemory);
        }
    }
}