using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transliterator.Core.Keyboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transliterator.Core.Models;
using Transliterator.Services;
using Transliterator.Helpers;
using System.Diagnostics;
using Transliterator.CoreTests.Keyboard;

// try creating a test window and setting a hook with public properties there

namespace Transliterator.Core.Keyboard.Tests
{
    [TestClass()]
    public class KeyboardHookTests
    {
        public EventLoopForm testWindow;

        public KeyboardHookTests()
        {
        }

        [TestMethod()]
        public void ListenToGeneratedKeys()
        {
            EventLoopForm testWindow = new();

            new Thread(() =>
            {
                testWindow.Show();
            }).Start();

            // arrange
            string testString = "abcd";

            // act
            // the input is handled, but not parsed properly
            // it is recognized as "packet" and that's it
            KeyboardInputGenerator.TextEntry("a");
            Thread.Sleep(50);

            KeyboardInputGenerator.TextEntry("b");
            Thread.Sleep(50);

            KeyboardInputGenerator.TextEntry("c");
            Thread.Sleep(50);

            KeyboardInputGenerator.TextEntry("d");
            Thread.Sleep(50);

            Thread.Sleep(5000);

            // assert
            string expected = "abcd";
            Assert.AreEqual(expected, testWindow.keyboardHookMemory);
        }
    }
}