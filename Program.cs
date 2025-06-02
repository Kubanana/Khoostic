using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

using Rendering.UI;

namespace Program
{
    public class Program
    {
        static NativeWindowSettings nativeWindowSettings = new NativeWindowSettings
        {
            ClientSize = new Vector2i(800, 600),
            Title = "Khoostic"
        };

        public static void Main(string[] args)
        {
            KhoosticWindow window = new KhoosticWindow(GameWindowSettings.Default, nativeWindowSettings);
            window.MinimumSize = new Vector2i(600, 350);
            window.Run();
        }
    }
}