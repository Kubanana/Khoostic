using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;

using Khoostic.Player;

namespace Khoostic.UI
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public KhoosticPlayer KhoosticPlayer = new KhoosticPlayer();

        public ObservableCollection<Song> Songs { get; set;} = new();

        public string CurrentTitle =>
            KhoosticPlayer.CurrentTitle!;

        public string CurrentArtist =>
            KhoosticPlayer.CurrentArtist!;
            
        public Bitmap CurrentArt =>
            KhoosticPlayer.CurrentArt!;

        public double Volume
        {
            get => KhoosticPlayer.Volume;
            set
            {
                KhoosticPlayer.Volume = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            KhoosticPlayer.InitPlayer();

            foreach (var song in KhoosticPlayer.Songs!)
            {
                Songs.Add(song);
            }
        }

        private void Song_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Song song)
            {
                KhoosticPlayer.PlaySong(song);
            }
        }

        private void Play_Click(object? sender, RoutedEventArgs e)
        {
            KhoosticPlayer.TogglePause();
        }

        private void Stop_Click(object? sender, RoutedEventArgs e)
        {
            KhoosticPlayer.Stop();
        }
    }
}