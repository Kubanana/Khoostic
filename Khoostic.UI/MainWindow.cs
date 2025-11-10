using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

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

            mainPanel.Children.Add(mainPanelText);


            if (KhoosticPlayer.LoadedSongs == null) return;
            foreach (var song in KhoosticPlayer.LoadedSongs)
            {
                var songButton = CreateSongButton(KhoosticPlayer.GetSongName(song));
                songButton.Click += (_, _) => KhoosticPlayer.PlaySong(song);
                songButton.Click += (_, _) => mainPanelText.Text = $"Now playing: {KhoosticPlayer.CurrentSongName}";

                songListPanel.Children.Add(songButton);
            }

            grid.Children.Add(scrollViewer);
            grid.Children.Add(mainPanel);
            Grid.SetColumn(mainPanel, 1);

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
    }
}