using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transliterator.CoreTests.Keyboard;

namespace Transliterator.Core.Keyboard.Tests
{
    [TestClass()]
    public class KeyboardHookTests
    {
        public EventLoopForm testWindow;
        private const int delayBetweenEachKeypress = 10;

        public KeyboardHookTests()
        {
        }

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
        public void ListenToGeneratedKeys()
        {
            // arrange
            string testString = "abcd";

            // act

            foreach (char i in testString)
            {
                KeyboardInputGenerator.TextEntry(i.ToString());
                Thread.Sleep(delayBetweenEachKeypress);
            }

            // assert
            string expected = "abcd";
            Assert.AreEqual(expected, testWindow.keyboardHookMemory);
        }

        [TestMethod()]
        public void ListenToGeneratedDifferentCaseKeys()
        {
            // arrange
            string testString = "aBcD";

            // act

            foreach (char i in testString)
            {
                KeyboardInputGenerator.TextEntry(i.ToString());
                Thread.Sleep(delayBetweenEachKeypress);
            }

            // assert
            string expected = "aBcD";
            Assert.AreEqual(expected, testWindow.keyboardHookMemory);
        }

        [TestMethod()]
        public void ListenToGeneratedCyrillicKeys()
        {
            // arrange
            string testString = "абвгд";

            // act

            foreach (char i in testString)
            {
                KeyboardInputGenerator.TextEntry(i.ToString());
                Thread.Sleep(delayBetweenEachKeypress);
            }

            // assert
            string expected = "абвгд";
            Assert.AreEqual(expected, testWindow.keyboardHookMemory);
        }

        [TestMethod()]
        public void ListenToGeneratedPunctuation()
        {
            // arrange
            string testString = ";!_#";

            // act

            foreach (char i in testString)
            {
                KeyboardInputGenerator.TextEntry(i.ToString());
                Thread.Sleep(delayBetweenEachKeypress);
            }

            // assert
            string expected = ";!_#";
            Assert.AreEqual(expected, testWindow.keyboardHookMemory);
        }

        // TODO: Fix SkipInjected test method
        //[TestMethod()]
        //public void SkipInjected()
        //{
        //    // arrange
        //    string testString = "абвгд";
        //    KeyboardHook.SkipInjected = true;

        //    // act

        //    foreach (char i in testString)
        //    {
        //        KeyboardInputGenerator.TextEntry(i.ToString());
        //        Thread.Sleep(delayBetweenEachKeypress);
        //    }

        //    KeyboardHook.SkipInjected = false;

        //    // assert
        //    string expected = "";
        //    Assert.AreEqual(expected, testWindow.keyboardHookMemory);
        //}
    }
}