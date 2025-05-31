using LibVLCSharp.Shared;

namespace Khoostic.Player
{
    public static class KhoosticPlayer
    {
        private static LibVLC? _libVLC;
        private static MediaPlayer? _mediaPlayer;

        public static void InitPlayer()
        {
            Core.Initialize();
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);
        }

        public static void Play(string path)
        {
            var media = new Media(_libVLC, path, FromType.FromPath);
            _mediaPlayer?.Play(media);
        }

        public static void Pause() => _mediaPlayer?.Pause();
        public static void Stop() => _mediaPlayer?.Stop();
        public static void Seek(float position) => _mediaPlayer.Position = position;
    }
}