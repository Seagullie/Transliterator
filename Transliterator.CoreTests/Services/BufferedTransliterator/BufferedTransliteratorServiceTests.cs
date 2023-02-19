using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Models;
using Transliterator.CoreTests.Keyboard;

namespace Transliterator.Core.Services.Tests
{
    [TestClass()]
    public class BufferedTransliteratorServiceTests
    {
        public EventLoopForm testWindow;
        private int delayBetweenEachKeypress = 100;

        // TODO: Move to Utilities or something
        public Dictionary<string, string> ReadReplacementMapFromJson(string fileName)
        {
            string TableAsString = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{fileName}.json"));
            dynamic deserializedTableObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(TableAsString);
            Dictionary<string, string> TableAsDictionary = deserializedTableObj;

            return TableAsDictionary;
        }

        [TestInitialize]
        public void Initialize()
        {
            // Runs before each test
            testWindow = new();
            testWindow.AttachBufferedTransliteratorService();
            // catch everything skipped by translit handler for full input picture
            // for some reason, input characters are captured as well, even though they are supposed to be e.Handled = true by translit handler and not passed to the next handler
            testWindow.AttachKeyboardHook();
            testWindow.bufferedTransliteratorService.TransliterationTable = new TransliterationTable(ReadReplacementMapFromJson("Resources/TranslitTables/tableLAT-UKR"));

            new Thread(() =>
            {
                testWindow.Show();
            }).Start();
        }

        [TestCleanup]
        public void Cleanup()
        {
            //KeyboardHook.ShutdownSystemHook();
        }

        [TestMethod()]
        public void TestComboBreakByOtherKey()
        {
            // arrange
            string testString = "Odynadcjatytomnyj.";

            // act
            foreach (char c in testString)
            {
                KeyboardInputGenerator.TextEntry(c.ToString());
                Thread.Sleep(delayBetweenEachKeypress);
            }

            // assert
            string expected = "Одинадцятитомний.";
            Assert.AreEqual(expected, testWindow.keyboardHookMemory);
        }

        [TestMethod()]
        public void TestComboBreakByComboInit()
        {
            // arrange
            string testString = "Cjatka";

            // act
            foreach (char c in testString)
            {
                KeyboardInputGenerator.TextEntry(c.ToString());
                Thread.Sleep(delayBetweenEachKeypress);
            }

            // assert
            string expected = "Цятка";
            Assert.AreEqual(expected, testWindow.keyboardHookMemory);
        }

        [TestMethod()]
        public void TestUppercaseCombo()
        {
            // arrange
            string testString = "Schuka";

            // act
            foreach (char c in testString)
            {
                KeyboardInputGenerator.TextEntry(c.ToString());
                Thread.Sleep(delayBetweenEachKeypress);
            }

            // assert
            string expected = "Щука";
            Assert.AreEqual(expected, testWindow.keyboardHookMemory);
        }

        [TestMethod()]
        public void TestComboBreakByPunctuation()
        {
            // arrange
            string testString = "sc!";

            // act
            foreach (char c in testString)
            {
                KeyboardInputGenerator.TextEntry(c.ToString());
                Thread.Sleep(delayBetweenEachKeypress);
            }

            // assert
            string expected = "сц!";
            Assert.AreEqual(expected, testWindow.keyboardHookMemory);
        }
    }
}