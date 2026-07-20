using System.Collections.ObjectModel;
using System.ComponentModel;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;

using CommunityToolkit.Mvvm.Input;

using Khoostic.Player;

namespace Khoostic.UI
{
    public partial class MainViewModel : ViewModelBase, INotifyPropertyChanged
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

        public MainViewModel()
        {
            KhoosticPlayer.InitPlayer();

            foreach (var song in KhoosticPlayer.Songs!)
            {
                Songs.Add(song);
            }
        }

        [RelayCommand]
        private void SongClick(Song song)
        {
            KhoosticPlayer.PlaySong(song);

            OnPropertyChanged(nameof(CurrentTitle));
            OnPropertyChanged(nameof(CurrentArtist));
            OnPropertyChanged(nameof(CurrentArt));
        }

        [RelayCommand]
        private void PlayClick()
        {
            KhoosticPlayer.TogglePause();
        }

        [RelayCommand]
        private void StopClick()
        {
            KhoosticPlayer.Stop();
        }
    }
}