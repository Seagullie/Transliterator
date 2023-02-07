using System.Threading;
using System.Windows;
using Transliterator.Core.Enums;
using Transliterator.Core.Keyboard;

namespace Transliterator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            KeyboardHook.SetupSystemHook();
            Thread.Sleep(1000);
            KeyboardInputGenerator.KeyPresses(VirtualKeyCode.KeyA, VirtualKeyCode.KeyB, VirtualKeyCode.KeyC);
        }
    }
}
