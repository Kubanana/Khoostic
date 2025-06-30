using LibVLCSharp.Shared;

namespace Khoostic.Player
{
    public class KhoosticPlayer
    {
        public static string[] AllowedExtentions = { ".flac", ".mp3", ".wav", ".m4a" };

        public string? CurrentSong;

        public List<string> LoadedSongs;

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

            LoadSongs();
        }

        public static void PlaySong(string song)
        {

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
                Console.WriteLine(song);
            }
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