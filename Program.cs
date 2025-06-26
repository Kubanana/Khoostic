using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Themes.Fluent;

using Khoostic.UI;

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
        public override void Initialize()
        {
            base.Initialize();

            Styles.Add(new FluentTheme());
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new KhoosticWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}