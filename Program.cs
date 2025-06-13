using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

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

            var button = new Button
            {
                Content = "Play",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            button.Click += (_, _) => button.Content = "Playing";

            Content = button;
        }
    }
}