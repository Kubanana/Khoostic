
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

using BiggyTools.Debugging;

using Khoostic.Player;

namespace Khoostic.UI
{
    public class KhoosticWindow : Window
    {
        private StackPanel _songsPanel;
        private readonly string[] _supportedExtentions = { ".mp3", ".wav", ".flac", ".m4a" };

        public KhoosticWindow()
        {
            Title = "Khoostic";
            Width = 800;
            Height = 600;

            _songsPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(10)
            };

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = _songsPanel
            };

            var root = new Grid();
            root.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            root.Children.Add(scrollViewer);

            Content = root;

            KhoosticPlayer.InitPlayer();
            DiscordRPController.Init();
            
            LoadSongs();
        }

        private void LoadSongs()
        {
            string? homeDir = Environment.GetEnvironmentVariable("HOME");

            if (string.IsNullOrEmpty(homeDir))
            {
                Logger.LogError("Could not find users home folder");
                return;
            }

            string musicDir = Path.Combine(homeDir, "Music");

            if (!Directory.Exists(musicDir))
            {
                Logger.Log("Music folder not found...");
                return;
            }

            var songs = Directory.GetFiles(musicDir, "*.*", SearchOption.AllDirectories)
                                    .Where(f => _supportedExtentions.Contains(Path.GetExtension(f).ToLower()))
                                    .ToList();

            foreach (var song in songs)
            {
                var button = new Button
                {
                    Content = Path.GetFileName(song),
                    Tag = song,
                    Margin = new Thickness(0, 5, 0, 5),
                    Padding = new Thickness(10),
                    BorderThickness = new Thickness(1),
                    Background = Brushes.LightGray
                };

                button.Click += PlaySong;
                _songsPanel.Children.Add(button);
            }
        }

        private void PlaySong(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string filePath)
            {
                KhoosticPlayer.Play(filePath);
            }
        }
    }
}