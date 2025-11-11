using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;

using Khoostic.Player;

namespace Khoostic.UI
{
    public class MainWindow : Window
    {
        public KhoosticPlayer KhoosticPlayer = new KhoosticPlayer();

        public MainWindow()
        {
            Title = "Khoostic";
            Width = 800;
            Height = 600;

            KhoosticPlayer.InitPlayer();

            Background = Brushes.Black;

            var grid = new Grid();
            grid.ColumnDefinitions = new ColumnDefinitions("200, *");

            var songListPanel = new StackPanel();
            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = songListPanel
            };

            var mainPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var mainPanelText = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text = $"Now playing...",
                Foreground = new SolidColorBrush(Colors.White)
            };

            var mainPanelCoverArt = new Image
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 500,
                Height = 500,
                Stretch = Stretch.UniformToFill,
                Margin = new Thickness(4)
            };

            if (KhoosticPlayer.LoadedSongs == null) return;
            foreach (var song in KhoosticPlayer.LoadedSongs)
            {
                var songButton = CreateSongButton(KhoosticPlayer.GetSongName(song));
                songButton.Click += (_, _) => KhoosticPlayer.PlaySong(song);
                songButton.Click += (_, _) => mainPanelText.Text = $"Now playing: {KhoosticPlayer.CurrentSongName}";
                songButton.Click += (_, _) => mainPanelCoverArt.Source = LoadCoverArt(song);

                songListPanel.Children.Add(songButton);
            }

            mainPanel.Children.Add(mainPanelCoverArt);
            mainPanel.Children.Add(mainPanelText);

            var bottomPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Background = new SolidColorBrush(Colors.Gray),
                Height = 60,
                VerticalAlignment = VerticalAlignment.Bottom,
                Spacing = 10
            };

            var volumeSlider = new Slider
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Minimum = 0,
                Maximum = 100,
                Value = 50,
                Width = 100
            };

            bottomPanel.Children.Add(volumeSlider);

            grid.Children.Add(scrollViewer);
            grid.Children.Add(mainPanel);
            Grid.SetColumn(mainPanel, 1);
            Grid.SetRow(mainPanel, 0);

            grid.Children.Add(bottomPanel);
            Grid.SetColumn(bottomPanel, 1);
            Grid.SetRow(bottomPanel, 1);

            Content = grid;
        }

        private Button CreateSongButton(string content)
        {
            var button = new Button();

            button.Content = content;
            button.Background = Brushes.Gray;
            button.Foreground = Brushes.White;
            button.Width = 200;
            button.Height = 50;

            return button;
        }

        private void OnButtonClick(object? sender, RoutedEventArgs e)
        {
            Console.WriteLine("Pressed Button");
        }

        private Bitmap? LoadCoverArt(string filePath)
        {
            var bytes = KhoosticPlayer.GetCoverArt(filePath);
            if (bytes == null) return null;

            var ms = new MemoryStream(bytes);
            return new Bitmap(ms);
        }
    }
}