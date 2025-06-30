using DiscordRPC;

using LibVLCSharp.Shared;

namespace Khoostic.Player
{
    public class KhoosticPlayer
    {
        public static string[] AllowedExtentions = { ".flac", ".mp3", ".wav", ".m4a" };

        public string? CurrentSongName;

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
            DiscordRPController.Init();

            LoadSongs();
        }

        public void PlaySong(string song)
        {
            var media = new Media(_libVLC, song, FromType.FromPath);
            MediaPlayer?.Play(media);

            CurrentSongName = Path.GetFileNameWithoutExtension(song);

            DiscordRPController.UpdateSongPresence(Path.GetFileNameWithoutExtension(song), "Unknown Artist");
        }

        public void PlayRandomSong()
        {
            Random random = new Random();

            int max = LoadedSongs.Count;
            int randomIndex = random.Next(max);

            string song = LoadedSongs[randomIndex];
            PlaySong(song);
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

            LoadedSongs = songs;
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