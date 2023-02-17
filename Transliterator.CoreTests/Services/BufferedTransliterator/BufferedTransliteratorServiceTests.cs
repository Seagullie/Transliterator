using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void TestUppercaseCombo()
        {
            string testString = "Щука";
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