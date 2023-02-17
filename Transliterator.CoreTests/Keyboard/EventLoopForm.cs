using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Transliterator.Core.Keyboard;

namespace Transliterator.CoreTests.Keyboard
{
    public partial class EventLoopForm : Form
    {
        public string keyboardHookMemory = "garbage";

        public EventLoopForm()
        {
            KeyboardHook.SkipInjected = false;
            KeyboardHook.SetupSystemHook();
            KeyboardHook.KeyPressed += KeyPressedHandler;

            InitializeComponent();
        }

        private void EventLoopForm_Load(object sender, EventArgs e)
        {
            KeyboardInputGenerator.TextEntry("a");
            Thread.Sleep(50);

            KeyboardInputGenerator.TextEntry("b");
            Thread.Sleep(50);

            KeyboardInputGenerator.TextEntry("c");
            Thread.Sleep(50);

            KeyboardInputGenerator.TextEntry("d");
            Thread.Sleep(50);
        }

        private void KeyPressedHandler(object? sender, Core.Keyboard.KeyEventArgs e)
        {
            keyboardHookMemory += e.Character;
            e.Handled = true;
        }
    }
}