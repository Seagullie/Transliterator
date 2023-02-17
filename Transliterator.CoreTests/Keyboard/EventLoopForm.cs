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
        public string keyboardHookMemory = "";

        public EventLoopForm()
        {
            Width = 0;
            Height = 0;
            Visible = false;

            KeyboardHook.SkipInjected = false;
            KeyboardHook.SetupSystemHook();
            KeyboardHook.KeyPressed += KeyPressedHandler;

            InitializeComponent();
        }

        private void EventLoopForm_Load(object sender, EventArgs e)
        {
        }

        private void KeyPressedHandler(object? sender, KeyboardHookEventArgs e)
        {
            keyboardHookMemory += e.Character;
            e.Handled = true;
        }
    }
}