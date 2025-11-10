using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

using Khoostic.Player;

namespace Khoostic.UI
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            Title = "Khoostic";
            Width = 800;
            Height = 600;

            KhoosticPlayer khoosticPlayer = new KhoosticPlayer();
            khoosticPlayer.InitPlayer();

            Background = Brushes.Black;

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var PlayButton = CreateButton("Play");
            PlayButton.Click += OnButtonClick;

            stackPanel.Children.Add(PlayButton);

            if (khoosticPlayer.LoadedSongs == null) return;
            foreach (var song in khoosticPlayer.LoadedSongs)
            {
                var songButton = CreateButton(song);

                stackPanel.Children.Add(songButton);
            }

            Content = stackPanel;
        }

        private Button CreateButton(string content)
        {
            var button = new Button();

            button.Content = content;
            button.Background = Brushes.White;
            button.Width = 200;
            button.Height = 100;

            return button;
        }

        private void OnButtonClick(object? sender, RoutedEventArgs e)
        {
            Console.WriteLine("Pressed Button");
        }
    }
}