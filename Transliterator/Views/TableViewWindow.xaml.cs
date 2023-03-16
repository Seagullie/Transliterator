using System.Windows.Input;
using Transliterator.ViewModels;

namespace Transliterator.Views
{
    /// <summary>
    /// Interaction logic for TableViewWindow.xaml
    /// </summary>
    public partial class TableViewWindow
    {
        public TableViewModel ViewModel { get; set; }

        public TableViewWindow()
        {
            InitializeComponent();
            ViewModel = new();
            DataContext = ViewModel;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Make window draggable
            if (e.ChangedButton == MouseButton.Left)
                try
                {
                    DragMove();
                }
                catch { }
        }
    }
}