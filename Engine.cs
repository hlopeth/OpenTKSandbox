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

        public Engine(GameWindow window)
        {
            _window = window;
            _window.Load += WindowOnLoad;
            _window.Unload += WindowOnUnload;
            _window.Resize +=WindowOnResize;
            _window.UpdateFrame += WindowOnUpdateFrame;
            _window.RenderFrame += WindowOnRenderFrame;
            _window.KeyDown += WindowOnKeyDown;
        }

        private ShaderProgram _shaderProgram;
        private SceneObject _cube;
        
        private void WindowOnLoad(object sender, EventArgs e)
        {
            //GL.Enable(EnableCap.DepthTest);
            try
            {
                _shaderProgram = new ShaderProgram("object.vs", "object.fs");
                _cube = new SceneObject(_shaderProgram.Id, new[] { 1f, 2f, 3f }, 3);
            }
            catch (ShaderProgramException ex)
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
            // Logic updates.
        }

        private void WindowOnRenderFrame(object sender, FrameEventArgs e)
        {
            GL.ClearColor(Color.Gray);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            _window.SwapBuffers();
        }

        private void WindowOnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    _window.Close();
                    break;
                default:
                    Console.WriteLine($"no {e.Key} key handler");
                    break;
            }   
        }
    }
}