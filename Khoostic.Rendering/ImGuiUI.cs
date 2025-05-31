using System.Globalization;
using System.Numerics;
using BiggyTools.Debugging;
using ImGuiNET;

using Khoostic.Player;
using Khoostic.Rendering;

using LibVLCSharp.Shared;

using Newtonsoft.Json.Linq;
using static ImGuiNET.ImGui;

namespace Rendering.UI
{
    public static class ImGuiUI
    {
        public static Vector4 BackgroundClearColor = new Vector4(0f, 0f, 0f, 1f);
        public static ImFontPtr DefaultFont;

        private static string _musicDir;
        private static string[] _musicFiles;

        private static float _leftPanelWidth = 250f;
        private static float _bottomBarHeight = 80f;

        public static void OnStart()
        {
            _musicDir = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Music");
            _musicFiles = Directory.GetFiles(_musicDir, "*.*", SearchOption.AllDirectories)
            .Where(f => f.EndsWith(".mp3") || f.EndsWith(".ogg") || f.EndsWith(".flac") || f.EndsWith(".wav"))
            .ToArray();

            KhoosticPlayer.InitPlayer();

            Logger.Log(_musicDir);
        }

        public static void Render()
        {
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

                InitializeSongs();

                EndChild();
            }

            SameLine();

            SetCursorPosX(0);
            SetCursorPosX(_leftPanelWidth);

            float middlePanelWidth = viewportSize.X - _leftPanelWidth;
            float middlePanelHeight = viewportSize.Y - _bottomBarHeight;

            if (BeginChild("##MiddlePanel", new Vector2(middlePanelWidth, middlePanelHeight), ImGuiChildFlags.Borders))
            {
                if (!string.IsNullOrEmpty(KhoosticPlayer.CurrentSong))
                {
                    var songFile = TagLib.File.Create(KhoosticPlayer.CurrentSong);
                    var songTitle = songFile.Tag.Title ?? Path.GetFileNameWithoutExtension(KhoosticPlayer.CurrentSong);
                    var artist = songFile.Tag.FirstPerformer ?? "Unknown Artist";

                    var region = GetContentRegionAvail();
                    float imageSize = 256f;
                    float spacing = 10f;

                    float totalHeight = imageSize + spacing * 2 + CalcTextSize(songTitle).Y + CalcTextSize(artist).Y;

                    float startY = (region.Y - totalHeight) * 0.5f;
                    SetCursorPosY(startY);

                    float centerX = GetContentRegionAvail().X * 0.5f;

                    if (songFile.Tag.Pictures.Length > 0)
                    {
                        var picData = songFile.Tag.Pictures[0].Data.Data;
                        var textureId = TextureCache.GetOrCreateTexture(picData);

                        SetCursorPosX(centerX - imageSize * 0.5f);
                        Image((IntPtr)textureId, new Vector2(imageSize, imageSize));

                        Dummy(new Vector2(0, spacing));
                    }

                    var nowPlaying = $"Now Playing: {songTitle}";
                    var nowPlayingSize = CalcTextSize(nowPlaying);
                    SetCursorPosX(centerX - nowPlayingSize.X * 0.5f);
                    Text(nowPlaying);

                    var artistSize = CalcTextSize(artist);
                    SetCursorPosX(centerX - artistSize.X * 0.5f);
                    Text(artist);
                }

                EndChild();
            }

            SetCursorPosY(viewportSize.Y - _bottomBarHeight);
            if (BeginChild("##ControlPanel", new Vector2(rightPanelWidth, _bottomBarHeight), ImGuiChildFlags.Borders))
            {

                float regionWidth = GetContentRegionAvail().X;
                float buttonWidth = 60f;
                float sliderWidth = 600f;
                float spacing = 2f;

                SetCursorPosX((regionWidth - buttonWidth) * 0.5f);
                if (Button(KhoosticPlayer.MediaPlayer.IsPlaying ? "Pause" : "Play", new Vector2(buttonWidth, 40)))
                {
                    KhoosticPlayer.TogglePause();
                }

                Dummy(new Vector2(0, spacing));

                float playbackPosition = KhoosticPlayer.MediaPlayer.Time / 1000f;
                float totalSongLength = KhoosticPlayer.MediaPlayer.Length / 1000f;

                float textSpacing = 10f;
                string timeLabel = $"{KhoosticPlayer.FormatTime(playbackPosition)} / {KhoosticPlayer.FormatTime(totalSongLength)}";
                Vector2 timeSize = CalcTextSize(timeLabel);

                float totalWidth = timeSize.X + textSpacing + sliderWidth;
                float startX = (regionWidth - totalWidth) * 0.5f;

                SetCursorPosX(startX);
                Text(timeLabel);

                SameLine();
                SetCursorPosX(startX + timeSize.X + textSpacing);
                SetNextItemWidth(sliderWidth);
        
                if (SliderFloat("##PlaybackSlider", ref playbackPosition, 0.0f, totalSongLength, ""))
                {
                    KhoosticPlayer.MediaPlayer.Time = (long)(playbackPosition * 1000);
                }

                EndChild();
            }

            End();
        }

        private static void InitializeSongs()
        {
            foreach (var song in _musicFiles)
            {
                if (Button(Path.GetFileName(Path.GetFileNameWithoutExtension(song))))
                {
                    KhoosticPlayer.Play(song);
                    KhoosticPlayer.CurrentSong = song;
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