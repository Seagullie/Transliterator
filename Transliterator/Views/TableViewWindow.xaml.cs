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
                DragMove();
        }
    }
}