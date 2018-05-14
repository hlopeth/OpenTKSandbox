using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenTKSandbox
{
    public class Engine
    {
        private readonly GameWindow _window;
        public bool windowSouldClose;
        IScene scene;

        public Engine(GameWindow window)
        {
            _window = window;
            _window.Load += WindowOnLoad;
            _window.Unload += WindowOnUnload;
            _window.Resize +=WindowOnResize;
            _window.UpdateFrame += WindowOnUpdateFrame;
            _window.RenderFrame += WindowOnRenderFrame;
            _window.KeyDown += WindowOnKeyDown;
            scene = new Scene();
        }

        private ShaderProgram _shaderProgram;
        private IModel _cube;
        
        
        private void WindowOnLoad(object sender, EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
            try
            {
                _shaderProgram = new ShaderProgram("object.vs", "object.fs");
                Vertex[] vert =
                {
                    new Vertex(new OpenTK.Vector4(-0.5f,-0.5f, 0.5f, 1f), OpenTK.Graphics.Color4.Red),
                    new Vertex(new OpenTK.Vector4( 0.5f,-0.5f, 0.5f, 1f), OpenTK.Graphics.Color4.Green),
                    new Vertex(new OpenTK.Vector4( 0.5f, 0.5f, 0.5f, 1f), OpenTK.Graphics.Color4.Blue),
                    new Vertex(new OpenTK.Vector4(-0.5f, 0.5f, 0.5f, 1f), OpenTK.Graphics.Color4.Pink)
                };
                uint[] ind = new uint[] { 0, 1, 2, 0, 2, 3};
                _cube = new ModelV3(_shaderProgram, vert, ind);
                scene.Models.Add(_cube);
            }
            catch (ShaderProgramException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ModelException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void WindowOnUnload(object sender, EventArgs e)
        {
            // Free resources.
            _shaderProgram.Dispose();
            _cube.Dispose();
        }

        private void WindowOnResize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, _window.Width, _window.Height);
        }

        private void WindowOnUpdateFrame(object sender, FrameEventArgs e)
        {
            if (windowSouldClose)
                _window.Close();
            // Logic updates.
        }

        private void WindowOnRenderFrame(object sender, FrameEventArgs e)
        {
            _window.Title = $"Sandbox FPS: {1f / e.Time:0}";
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            scene.Draw();
            _window.SwapBuffers();
        }

        private void WindowOnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    windowSouldClose = true;
                    break;
                default:
                    Console.WriteLine($"no {e.Key} key handler");
                    break;
            }   
        }
    }
}