using System.Drawing;
using BiggyTools.Debugging;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Rendering.UI
{
    public class KhoosticWindow : GameWindow
    {
        private ImGuiController _imGuiController;

        public KhoosticWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            _imGuiController = new ImGuiController(ClientSize.X, ClientSize.Y);

            GL.LoadBindings(new GLFWBindingsContext());
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
    }
}