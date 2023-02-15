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

// try creating a test window and setting a hook with public properties there

namespace Transliterator.Core.Keyboard.Tests
{
    [TestClass()]
    public class KeyboardHookTests
    {
        private string keyboardHookMemory = String.Empty;

        private void KeyPressedHandler(object? sender, KeyEventArgs e)
        {
            keyboardHookMemory += e.Character;
            e.Handled = true;
        }

        public KeyboardHookTests()
        {
        }

        [TestMethod()]
        public void ListenToGeneratedKeys()
        {
            // arrange
            string testString = "abcd";

            // act
            KeyboardInputGenerator.TextEntry("a");
            Thread.Sleep(200);

            KeyboardInputGenerator.TextEntry("b");
            Thread.Sleep(200);

            KeyboardInputGenerator.TextEntry("c");
            Thread.Sleep(200);

            KeyboardInputGenerator.TextEntry("d");
            Thread.Sleep(200);

            // assert
            string expected = "abcd";
            Assert.AreEqual(expected, keyboardHookMemory);
        }
    }
}