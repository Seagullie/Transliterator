using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
            KeyboardHook.KeyPressed += KeyPressedHandler;

            InitializeComponent();
        }

        private void KeyPressedHandler(object? sender, Core.Keyboard.KeyEventArgs e)
        {
            keyboardHookMemory += e.Character;
            e.Handled = true;
        }
    }
}