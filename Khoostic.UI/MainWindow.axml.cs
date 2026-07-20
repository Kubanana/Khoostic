using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;

using Khoostic.Player;

namespace Khoostic.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}