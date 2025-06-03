using BiggyTools.Debugging;

using LibVLCSharp.Shared;

namespace Khoostic.Player
{
    public static class KhoosticPlayer
    {
        public static string? CurrentSong;

        private static LibVLC? _libVLC;
        public static MediaPlayer? MediaPlayer;

        private static List<string> _currentSongQueue = new List<string>();
        private static int _currentSongIndex = -1;
        private static bool _isShuffleEnabled = false;
        private static RepeatMode _repeatMode = RepeatMode.None;

        public enum RepeatMode
        {
            None,
            One,
            All
        }

        public static void InitPlayer()
        {
            Core.Initialize();
            _libVLC = new LibVLC();
            MediaPlayer = new MediaPlayer(_libVLC);
        }

        public static void SetPlayList(List<string> songs, string initialSongPath = null)
        {
            _currentSongQueue = new List<string>(songs);

            if (!string.IsNullOrEmpty(initialSongPath))
            {
                _currentSongIndex = _currentSongQueue.IndexOf(initialSongPath);
            }

            else if (_currentSongQueue.Any())
            {
                _currentSongIndex = 0;
            }

            else
            {
                _currentSongIndex = -1;
            }
        }

        public static void Play(string path)
        {
            var media = new Media(_libVLC, path, FromType.FromPath);
            MediaPlayer?.Play(media);

            CurrentSong = path;

            var songFile = TagLib.File.Create(path);
            var songTitle = songFile.Tag.Title ?? Path.GetFileNameWithoutExtension(path);
            var artist = songFile.Tag.FirstPerformer ?? "Unknown Artist";

            DiscordRPController.UpdateSongPresence(songTitle, artist);
        }

        public static void PlayByIndex(int index)
        {
            if (index >= 0 && index < _currentSongQueue.Count)
            {
                _currentSongIndex = index;
                Play(_currentSongQueue[index]);
            }

            else
            {
                Stop();
            }
        }

        public static void PlayNextSong()
        {
            if (!_currentSongQueue.Any()) return;

            if (_repeatMode == RepeatMode.One)
            {
                PlayByIndex(_currentSongIndex);
                return;
            }

            int nextIndex = _currentSongIndex;

            if (_isShuffleEnabled)
            {
                Random rand = new Random();

                do
                {
                    nextIndex = rand.Next(0, _currentSongQueue.Count);
                } while (nextIndex == _currentSongIndex && _currentSongQueue.Count > 1);
            }

            else
            {
                nextIndex++;
            }

            if (nextIndex >= _currentSongQueue.Count)
            {
                if (_repeatMode == RepeatMode.All)
                {
                    nextIndex = 0;
                }

                else
                {
                    Stop();
                    _currentSongIndex = -1;
                    CurrentSong = null;

                    return;
                }
            }

            PlayByIndex(nextIndex);
        }

        public static void PlayPreviousSong()
        {
            if (!_currentSongQueue.Any()) return;

            int prevIndex = _currentSongIndex - 1;

            if (prevIndex < 0)
            {
                if (_repeatMode == RepeatMode.All)
                {
                    prevIndex = _currentSongQueue.Count - 1;
                }

                else
                {
                    Stop();
                    _currentSongIndex = -1;
                    CurrentSong = null;

                    return;
                }
            }

            PlayByIndex(prevIndex);
        }

        public static void ToggleShuffle()
        {
            _isShuffleEnabled = !_isShuffleEnabled;
        }

        public static void CycleRepeatMode()
        {
            _repeatMode = (RepeatMode)(((int)_repeatMode + 1) % Enum.GetValues(typeof(RepeatMode)).Length);
        }

        public static bool IsShuffleEnabled => _isShuffleEnabled;
        public static RepeatMode GetRepeatMode => _repeatMode;
        public static string GetCurrentPlaySongPath() => CurrentSong;
        public static List<string> GetCurrentSongQueue() => _currentSongQueue;

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