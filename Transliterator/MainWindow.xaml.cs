using System.Threading;
using System.Windows;
using Transliterator.Core.Enums;
using Transliterator.Core.Keyboard;

namespace Transliterator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindowViewModel ViewModel { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new();
            DataContext = ViewModel;

            KeyboardHook.SetupSystemHook();
            Thread.Sleep(1000);
            KeyboardInputGenerator.KeyPresses(VirtualKeyCode.KeyA, VirtualKeyCode.KeyB, VirtualKeyCode.KeyC);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Make window draggable
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // TODO: Uncomment after migrating other things referenced by the methods

            //ViewModel.SaveSettings();
            //ViewModel.DisposeOfNotifyIcon();
        }
    }
}