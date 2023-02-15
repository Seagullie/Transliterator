using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transliterator.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transliterator.Core.Services.Tests
{
    [TestClass()]
    public class BufferedTransliteratorServiceTests
    {
        // TODO: Implement after figuring out how to test KeyboardHook
        [TestMethod()]
        public void TestComboBreakByOtherKey()
        {
            string testString = "Одинадцятитомний";
        }

        // TODO: Implement after figuring out how to test KeyboardHook
        [TestMethod()]
        public void TestComboBreakByComboInit()
        {
            string testString = "Цятка";
        }

        // TODO: Implement after figuring out how to test KeyboardHook
        [TestMethod()]
        public void TestComboBreakByPunctuation()
        {
            string testString = "sc!";

            string expected = "сц!";
        }
    }
}