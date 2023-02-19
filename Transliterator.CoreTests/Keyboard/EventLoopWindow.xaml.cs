using System.Windows;
using Transliterator.Core.Keyboard;

namespace Transliterator.CoreTests.Keyboard
{
    /// <summary>
    /// Interaction logic for EventLoopWindow.xaml
    /// </summary>
    public partial class EventLoopWindow : Window
    {
        public string keyboardHookMemory = string.Empty;

        public EventLoopWindow()
        {
            // TODO: Fix EventLoopWindow
            //KeyboardHook.KeyPressed += KeyPressedHandler;

            InitializeComponent();
        }

        private void KeyPressedHandler(object? sender, KeyboardHookEventArgs e)
        {
            keyboardHookMemory += e.Character;
            e.Handled = true;
        }
    }
}