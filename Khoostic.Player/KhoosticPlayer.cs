using Avalonia.Media.Imaging;

using LibVLCSharp.Shared;

namespace Khoostic.Player
{
    public class Song
    {
        public string? Name { get; set; }
        public string? Artist { get; set; }
        public string? FilePath { get; set; }
        public Bitmap? CoverArt { get; set; }

        public Song(string name, string artist)
        {
            Name = name;
            Artist = artist;
        }
    }

    public class KhoosticPlayer
    {
        public static string[] AllowedExtentions = { ".flac", ".mp3", ".wav", ".m4a", ".opus" };
        
        public static string? CurrentTitle;
        public static string? CurrentArtist;
        public static Bitmap? CurrentArt;

        public static List<Song>? Songs = new List<Song>();
        public static double Volume;

        private LibVLC? _libVLC;
        public MediaPlayer? MediaPlayer;

        private bool _shuffle = false;

        public KhoosticPlayer()
        {

        }

        public void InitPlayer()
        {
            Core.Initialize();
            _libVLC = new LibVLC();
            MediaPlayer = new MediaPlayer(_libVLC);
            DiscordRPController.Init();

            LoadSongs();
        }

        public void PlaySong(Song song)
        {
            var media = new Media(_libVLC!, song.FilePath!, FromType.FromPath);
            MediaPlayer?.Play(media);

            CurrentTitle = song.Name;
            CurrentArtist = song.Artist;
            CurrentArt = song.CoverArt;

            DiscordRPController.UpdateSongPresence(song.Name!, song.Artist!);
        }

        public void PlayRandomSong()
        {
            Random random = new Random();

            int max = Songs!.Count;
            int randomIndex = random.Next(max);

            string song = Songs[randomIndex].FilePath!;
        }

        public string GetSongTitle(string songPath)
        {
            var file = TagLib.File.Create(songPath);

            if (file.Tag.Title != null)
            {
                return file.Tag.Title;
            }

            return Path.GetFileNameWithoutExtension(songPath);
        }

        public string GetArtist(string songPath)
        {
            var file = TagLib.File.Create(songPath);

            if (file.Tag.FirstAlbumArtist != null)
            {
                return file.Tag.FirstPerformer;
            }

            return "Unknown Artist";
        }

        public byte[]? GetCoverArt(string songPath)
        {
            var file = TagLib.File.Create(songPath);

            var pictures = file.Tag.Pictures;

            if (pictures.Length > 0)
            {
                return pictures[0].Data.Data;
            }

            return null;
        }

        public bool IsShuffleEnabled => _shuffle;

        public void TogglePause() => MediaPlayer?.Pause();
        public void Stop() => MediaPlayer?.Stop();

        public string FormatTime(float seconds)
        {
            int mins = (int)seconds / 60;
            int secs = (int)seconds % 60;

            return $"{mins:D2}:{secs:D2}";
        }

        private void LoadSongs()
        {
            string musicFolderPath = GetMusicFolderPath();

            var songs = Directory.EnumerateFiles(musicFolderPath, "*.*", SearchOption.AllDirectories)
                .Where(song => AllowedExtentions.Contains(Path.GetExtension(song)))
                .ToList();

            foreach (var song in songs)
            {
                Song loadedSong = new Song("bob", "bob");

                loadedSong.Name = GetSongTitle(song);
                loadedSong.Artist = GetArtist(song);
                loadedSong.FilePath = song;
                loadedSong.CoverArt = LoadCoverArt(song);

                Songs!.Add(loadedSong);
            }
        }

        private Bitmap? LoadCoverArt(string filePath)
        {
            var bytes = GetCoverArt(filePath);
            if (bytes == null) return null;

            var ms = new MemoryStream(bytes);
            return new Bitmap(ms);
        }

        private string GetMusicFolderPath()
        {
            string? homePath;

            if (OperatingSystem.IsWindows())
            {
                homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }

            else
            {
                homePath = Environment.GetEnvironmentVariable("HOME");
            }

            string musicFolder = Path.Combine(homePath!, "Music");

            return musicFolder;
        }
    }
}
