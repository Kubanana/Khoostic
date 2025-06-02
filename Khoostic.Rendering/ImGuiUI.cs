using System.Collections.Concurrent;
using System.Globalization;
using System.Numerics;
using BiggyTools.Debugging;
using ImGuiNET;

using Khoostic.Player;
using Khoostic.Rendering;

using Newtonsoft.Json.Linq;
using static ImGuiNET.ImGui;

namespace Rendering.UI
{
    public static class ImGuiUI
    {
        public static Vector4 BackgroundClearColor = new Vector4(0f, 0f, 0f, 1f);

        private static string _musicDir;
        private static List<string> _musicFiles;
        private static string _songSearchFilter = string.Empty;

        private static float _leftPanelWidth = 250f;
        private static float _bottomBarHeight = 100f;

        private static ConcurrentQueue<Action> _uiThreadActions = new ConcurrentQueue<Action>();
        private static int _uiThreadId;

        public static void OnStart()
        {
            _musicDir = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Music");
            _uiThreadId = Thread.CurrentThread.ManagedThreadId;


            KhoosticPlayer.InitPlayer();

            if (KhoosticPlayer.MediaPlayer != null)
            {
                KhoosticPlayer.MediaPlayer.EndReached += (sender, e) =>
                {
                    _uiThreadActions.Enqueue(() =>
                    {
                        KhoosticPlayer.PlayNextSong();
                    });
                };
            }

            LoadAllMusicFiles(_musicDir);
            DiscordRPController.Init();

            Logger.Log(_musicDir);
        }

        public static void Render()
        {
            while (_uiThreadActions.TryDequeue(out var action))
            {
                action.Invoke();
            }

            if (File.Exists("colors.json"))
            {
                var colors = LoadJsonColors("colors.json");
                ApplyTheme(colors);
            }

            Vector2 viewportSize = GetMainViewport().Size;
            Vector2 windowPos = GetMainViewport().Pos;

            SetNextWindowPos(windowPos);
            SetNextWindowSize(viewportSize);

            ImGuiWindowFlags windowFlags =
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoBackground;

            Begin("##MainWindow", windowFlags);

            float leftPanelWidth = 200f;
            float rightPanelWidth = GetContentRegionAvail().X;
            float panelHeight = GetContentRegionAvail().Y;

            if (BeginChild("##SongListPanel", new Vector2(leftPanelWidth, panelHeight)))
            {
                Text("Music Library");
                Separator();
                Spacing();

                InputTextWithHint("##Search", "Search songs...", ref _songSearchFilter, 256);
                Spacing();

                if (BeginChild("##SongList", new Vector2(-1, -1), ImGuiChildFlags.None))
                {
                    InitializeSongs();
                    EndChild();
                }

                EndChild();
            }

            SameLine();

            float middlePanelWidth = viewportSize.X - _leftPanelWidth;
            float middlePanelHeight = viewportSize.Y - _bottomBarHeight;

            SetCursorPosX(_leftPanelWidth);

            if (BeginChild("##MiddlePanel", new Vector2(middlePanelWidth, middlePanelHeight), ImGuiChildFlags.Borders))
            {
                if (!string.IsNullOrEmpty(KhoosticPlayer.CurrentSong))
                {
                    var songFile = TagLib.File.Create(KhoosticPlayer.CurrentSong);
                    var songTitle = songFile.Tag.Title ?? Path.GetFileNameWithoutExtension(KhoosticPlayer.CurrentSong);
                    var artist = songFile.Tag.FirstPerformer ?? "Unknown Artist";

                    float contentWidth = GetContentRegionAvail().X;
                    float contentHeight = GetContentRegionAvail().Y;

                    float imageSize = 512f;
                    float textSpacing = 10f;

                    float estimatedTextHeight = CalcTextSize(songTitle).Y + CalcTextSize(artist).Y + textSpacing;
                    float totalContentHeight = imageSize + textSpacing + estimatedTextHeight;

                    float startY = (contentHeight - totalContentHeight) * 0.5f;
                    if (startY < 0) startY = 0;

                    SetCursorPosY(startY);

                    if (songFile.Tag.Pictures.Length > 0)
                    {
                        var picData = songFile.Tag.Pictures[0].Data.Data;
                        var textureId = TextureCache.GetOrCreateTexture(picData);

                        SetCursorPosX((contentWidth - imageSize) * 0.5f);
                        Image((IntPtr)textureId, new Vector2(imageSize, imageSize));
                    }

                    else
                    {
                        SetCursorPosX((contentWidth - imageSize) * 0.5f);
                        Dummy(new Vector2(imageSize, imageSize));
                        SetCursorPosX((contentWidth - CalcTextSize("No Album Art").X) * 0.5f);
                        Text("No Album Art");
                    }

                    Dummy(new Vector2(0, textSpacing));

                    var nowPlaying = $"Now Playing: {songTitle}";
                    var nowPlayingSize = CalcTextSize(nowPlaying);
                    SetCursorPosX((contentWidth - nowPlayingSize.X) * 0.5f);
                    Text(nowPlaying);

                    var artistSize = CalcTextSize(artist);
                    SetCursorPosX((contentWidth - artistSize.X) * 0.5f);
                    Text(artist);
                }

                else
                {
                    string message = "No song playing...";

                    float messageWidth = CalcTextSize(message).X;
                    float messageHeight = CalcTextSize(message).Y;
                    SetCursorPos(new Vector2((GetContentRegionAvail().X - messageWidth) * 0.5f, (GetContentRegionAvail().Y - messageHeight) * 0.5f));
                    Text(message);
                }

                EndChild();
            }

            SetCursorPosY(viewportSize.Y - _bottomBarHeight);
            if (BeginChild("##ControlPanel", new Vector2(rightPanelWidth, _bottomBarHeight), ImGuiChildFlags.Borders))
            {

                float regionWidth = GetContentRegionAvail().X;
                float largeButtonWidth = 80f;
                float iconButtonSize = 32f;
                float playbackSliderWidth = regionWidth * 0.4f;
                float volumeSliderWidth = 120f;
                float spacing = 10f;

                float totalControlsPanelWidth = largeButtonWidth + CalcTextSize("00:00 / 00:00").X + playbackSliderWidth + volumeSliderWidth + (iconButtonSize * 4) + (spacing * 10);

                SetCursorPosX((regionWidth - totalControlsPanelWidth) * 0.5f);

                if (Button("<<", new Vector2(iconButtonSize, 40)))
                {
                    KhoosticPlayer.PlayPreviousSong();
                }

                SameLine(0, spacing);

                if (Button(KhoosticPlayer.MediaPlayer.IsPlaying ? "Pause" : "Play", new Vector2(largeButtonWidth, 40)))
                {
                    KhoosticPlayer.TogglePause();
                }

                SameLine(0, spacing);

                if (Button(">>", new Vector2(iconButtonSize, 40)))
                {
                    KhoosticPlayer.PlayNextSong();
                }

                SameLine(0, spacing * 2);

                float playbackPosition = KhoosticPlayer.MediaPlayer.Time / 1000f;
                float totalSongLength = KhoosticPlayer.MediaPlayer.Length / 1000f;
                string timeLabel = $"{KhoosticPlayer.FormatTime(playbackPosition)} / {KhoosticPlayer.FormatTime(totalSongLength)}";
                Text(timeLabel);
                SameLine(0, spacing);

                SetNextItemWidth(playbackSliderWidth);
                if (SliderFloat("##PlaybackSlider", ref playbackPosition, 0.0f, totalSongLength, ""))
                {
                    KhoosticPlayer.MediaPlayer.Time = (long)(playbackPosition * 1000);
                }

                SameLine(0, spacing * 2);

                int volume = KhoosticPlayer.MediaPlayer.Volume;
                SetNextItemWidth(volumeSliderWidth);
                if (SliderInt("##Volume", ref volume, 0, 125))
                {
                    KhoosticPlayer.MediaPlayer.Volume = (int)volume;
                }

                SameLine(0, spacing);

                bool isShuffle = KhoosticPlayer.IsShuffleEnabled;
                if (isShuffle) PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.5f, 0.8f, 1.0f));
                if (Button("Shuffle", new Vector2(iconButtonSize, 40)))
                {
                    KhoosticPlayer.ToggleShuffle();
                }

                if (isShuffle) PopStyleColor();
                SameLine(0, spacing);

                string repeatText = "";
                Vector4 repeatColor = new Vector4(0.3f, 0.3f, 0.3f, 1.0f);
                switch (KhoosticPlayer.GetRepeatMode)
                {
                    case KhoosticPlayer.RepeatMode.None:
                        repeatText = "Repeat Off";
                        break;
                    case KhoosticPlayer.RepeatMode.One:
                        repeatText = "Repeat 1"; repeatColor = new Vector4(0.2f, 0.5f, 0.8f, 1.0f);
                        break;
                    case KhoosticPlayer.RepeatMode.All:
                        repeatText = "Repeat All"; repeatColor = new Vector4(0.2f, 0.5f, 0.8f, 1.0f);
                        break;
                }

                PushStyleColor(ImGuiCol.Button, repeatColor);
                if (Button(repeatText, new Vector2(iconButtonSize * 1.5f, 40)))
                {
                    KhoosticPlayer.CycleRepeatMode();
                }

                PopStyleColor();

                EndChild();
            }

            End();
        }

        private static void InitializeSongs()
        {
            var filteredSongs = string.IsNullOrEmpty(_songSearchFilter)
                ? _musicFiles
                : _musicFiles.Where(s => Path.GetFileNameWithoutExtension(s).ToLower().Contains(_songSearchFilter.ToLower())).ToList();

            KhoosticPlayer.SetPlayList(filteredSongs);

            for (int i = 0; i < filteredSongs.Count; i++)
            {
                string song = filteredSongs[i];
                bool isCurrentSong = song == KhoosticPlayer.CurrentSong;

                if (isCurrentSong)
                {
                    PushStyleColor(ImGuiCol.Text, new Vector4(0.2f, 0.7f, 0.9f, 1.0f));
                    PushStyleColor(ImGuiCol.Button, new Vector4(0.1f, 0.3f, 0.4f, 1.0f));
                    PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.2f, 0.4f, 0.5f, 1.0f));
                    PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.15f, 0.35f, 0.45f, 1.0f));
                }

                if (Button(Path.GetFileNameWithoutExtension(song)))
                {
                    KhoosticPlayer.PlayByIndex(i);

                    var songFile = TagLib.File.Create(song);
                    var songTitle = songFile.Tag.Title ?? Path.GetFileNameWithoutExtension(KhoosticPlayer.CurrentSong);
                    var artist = songFile.Tag.FirstPerformer ?? "Unknown Artist";

                    DiscordRPController.UpdateSongPresence(songTitle, artist);
                }

                if (isCurrentSong)
                {
                    PopStyleColor(4);
                }
            }
        }

        public static Dictionary<string, Vector4> LoadJsonColors(string path)
        {
            var colorDict = new Dictionary<string, Vector4>();

            try
            {
                var json = JObject.Parse(File.ReadAllText(path));

                if (json != null && json.ContainsKey("colors"))
                {
                    var colors = (JObject?)json["colors"];

                    if (colors != null)
                    {
                        foreach (var kvp in colors)
                        {
                            string key = kvp.Key;
                            string hex = kvp.Value?.ToString() ?? string.Empty;
                            var colorVec = HexToVec4(hex);
                            colorDict[key] = colorVec;
                        }

                        return colorDict;
                    }

                    else
                    {
                        Logger.LogWarning($"Colors Property in '{path}' is not a valid JSON object");
                    }
                }

                else
                {
                    Logger.LogWarning($"Colors property not found in '{path}' or JSON in null/empty");
                }
            }

            catch (Exception ex)
            {
                Logger.LogError($"An unexpected error occured while loading colors from '{path}': {ex.Message}");
            }

            return colorDict;
        }

        private static void LoadAllMusicFiles(string musicFolderPath)
        {
            if (Directory.Exists(musicFolderPath))
            {
                _musicFiles = Directory.GetFiles(musicFolderPath, "*.mp3", SearchOption.AllDirectories)
                                    .Concat(Directory.GetFiles(musicFolderPath, "*.flac", SearchOption.AllDirectories))
                                    .Concat(Directory.GetFiles(musicFolderPath, "*.wav", SearchOption.AllDirectories))
                                    .ToList();
            }

            else
            {
                Logger.Log($"Music folder not found: {musicFolderPath}");
            }
        }

        private static Vector4 HexToVec4(string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

            return new Vector4(r / 255f, g / 255f, b / 255f, 1.0f);
        }

        public static void ApplyTheme(Dictionary<string, Vector4> colors)
        {
            var style = GetStyle();

            style.Colors[(int)ImGuiCol.WindowBg] = colors["color0"];
            style.Colors[(int)ImGuiCol.Text] = colors["color7"];
            style.Colors[(int)ImGuiCol.Button] = colors["color2"];
            style.Colors[(int)ImGuiCol.ButtonHovered] = colors["color3"];
            style.Colors[(int)ImGuiCol.ButtonActive] = colors["color4"];
            style.Colors[(int)ImGuiCol.Header] = colors["color5"];
            style.Colors[(int)ImGuiCol.HeaderHovered] = colors["color6"];
            style.Colors[(int)ImGuiCol.HeaderActive] = colors["color1"];

            BackgroundClearColor = colors["color0"];
        }
    }
}