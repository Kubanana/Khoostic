using LibVLCSharp.Shared;

namespace Khoostic.Player
{
    public static class KhoosticPlayer
    {
        public static string? CurrentSong;

        private static LibVLC? _libVLC;
        public static MediaPlayer? MediaPlayer;

        public static void InitPlayer()
        {
            Core.Initialize();
            _libVLC = new LibVLC();
            MediaPlayer = new MediaPlayer(_libVLC);
        }

        public static void Play(string path)
        {
            var media = new Media(_libVLC, path, FromType.FromPath);
            MediaPlayer?.Play(media);
        }

        public static void TogglePause() => MediaPlayer?.Pause();
        public static void Stop() => MediaPlayer?.Stop();

        public static string FormatTime(float seconds)
        {
            int mins = (int)seconds / 60;
            int secs = (int)seconds % 60;

            return $"{mins:D2}:{secs:D2}";
        }
    }
}