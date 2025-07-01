using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

namespace Khoostic.UI
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            Title = "Khoostic";
            Width = 800;
            Height = 600;

            Background = Brushes.Black;

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var button = new Button
            {
                Content = "Play",
                Background = Brushes.White,
                Width = 200,
                Height = 100
            };

            button.Click += OnButtonClick;

            stackPanel.Children.Add(button);

            Content = stackPanel;
        }

        private void OnButtonClick(object? sender, RoutedEventArgs e)
        {
            Console.WriteLine("Pressed Button");
        }
    }
}