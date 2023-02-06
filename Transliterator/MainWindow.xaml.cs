using System.Windows;
using Transliterator.Core;

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
        }
    }
}
