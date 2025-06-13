using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;

namespace Program
{
    public class Program
    {

        public static void Main(string[] args)
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
        }
    }

    public class App : Application
    {
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }

    public class MainWindow : Window
    {
        public MainWindow()
        {
            Title = "Khoostic";
            Width = 800;
            Height = 600;

            var root = new DockPanel();

            var leftPanel = new StackPanel
            {
                Width = 250,
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5)
            };

            DockPanel.SetDock(leftPanel, Dock.Left);

            var title = new TextBlock
            {
                Text = "Music Library",
                FontWeight = Avalonia.Media.FontWeight.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var searchBox = new TextBox
            {
                Watermark = "Search songs...",
                Margin = new Thickness(0, 0, 0, 10)
            };

            var songList = new ListBox();

            leftPanel.Children.Add(title);
            leftPanel.Children.Add(searchBox);
            leftPanel.Children.Add(new ScrollViewer { Content = songList });

            var bottomPanel = new StackPanel
            {
                Height = 100,
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Spacing = 10,
                Margin = new Thickness(5)
            };

            DockPanel.SetDock(bottomPanel, Dock.Bottom);

            bottomPanel.Children.Add(new Button { Content = "<<", Width = 40 });
            bottomPanel.Children.Add(new Button { Content = "Play", Width = 80 });
            bottomPanel.Children.Add(new Button { Content = ">>", Width = 40 });
            bottomPanel.Children.Add(new Slider { Width = 200, Minimum = 0, Maximum = 100 });
            bottomPanel.Children.Add(new Slider { Width = 100, Minimum = 0, MaxHeight = 125 });
            bottomPanel.Children.Add(new Button { Content = "Shuffle" });
            bottomPanel.Children.Add(new Button { Content = "Repeat Off" });

            var centerPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Spacing = 10
            };

            centerPanel.Children.Add(new TextBlock { Text = "Album Art" });
            centerPanel.Children.Add(new TextBlock { Text = "Now playing: Song" });
            centerPanel.Children.Add(new TextBlock { Text = "Artists" });

            var leftPanelBorder = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Child = leftPanel,
                Margin = new Thickness(5)
            };

            DockPanel.SetDock(leftPanelBorder, Dock.Left);

            var bottomPanelBorder = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Child = bottomPanel,
                Margin = new Thickness(5)
            };

            DockPanel.SetDock(bottomPanelBorder, Dock.Bottom);

            var centerPanelBorder = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Child = centerPanel,
                Margin = new Thickness(5)
            };

            root.Children.Add(leftPanelBorder);
            root.Children.Add(bottomPanelBorder);
            root.Children.Add(centerPanelBorder);

            Content = root;
        }
    }
}