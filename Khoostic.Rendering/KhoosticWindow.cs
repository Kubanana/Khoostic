using System.Runtime.CompilerServices;

using BiggyTools.Debugging;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using SixLabors.ImageSharp.PixelFormats;

namespace Rendering.UI
{
    public class KhoosticWindow : GameWindow
    {
        private ImGuiController _imGuiController;

        public KhoosticWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            GL.LoadBindings(new GLFWBindingsContext());
            SetWindowIcon("Assets/Logo.png");

            _imGuiController = new ImGuiController(ClientSize.X, ClientSize.Y);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            ImGui.CreateContext();
            ImGui.SetCurrentContext(ImGui.GetCurrentContext());

            ImGuiUI.OnStart();

            GL.ClearColor(0f, 0f, 0f, 1f);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _imGuiController.Update(this, (float)args.Time);

            ImGuiUI.Render();

            _imGuiController.Render();

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            _imGuiController.WindowResized(ClientSize.X, ClientSize.Y);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _imGuiController.Dispose();
        }

        public override void Close()
        {
            base.Close();

            Logger.Log("UIWindow::Closed Window");
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);

            if (e.Unicode != 0)
            {
                _imGuiController.AddInputCharacter((char)e.Unicode);
            }
        }

        private void SetWindowIcon(string imagePath)
        {
            using (SixLabors.ImageSharp.Image<Rgba32> image = (SixLabors.ImageSharp.Image<Rgba32>)SixLabors.ImageSharp.Image.Load(imagePath))
            {
                byte[] pixels = new byte[image.Width * image.Height * Unsafe.SizeOf<Rgba32>()];

                var windowIcon = new WindowIcon(new[]
                {
                    new OpenTK.Windowing.Common.Input.Image(image.Width, image.Height, pixels)
                });

                Icon = windowIcon;
            }
        }
    }
}