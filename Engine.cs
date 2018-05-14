using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenTKSandbox
{
    public class Engine : GameWindow
    {
        private IScene _mainScene;

        public Engine(int width, int height) : base(width, height)
        {
            GL.Enable(EnableCap.DepthTest);   
        }

        protected override void OnLoad(EventArgs e)
        {
            _mainScene = new Scene();

            var vertexData = new VertexData(new[]
            {
                new Vertex(new Vector3(-0.5f, 0.5f, 0.0f)),
                new Vertex(new Vector3(-0.5f, -0.5f, 0.0f)),
                new Vertex(new Vector3(0.5f, -0.5f, 0.0f))
            });

            var shaderProgram = new ShaderProgram("object.vs", "object.fs");
            
            var model = new Model(shaderProgram, vertexData);
            
            _mainScene.Models.Add(model);
        }

        protected override void OnUnload(EventArgs e)
        {
            Scene scene = (Scene)_mainScene;
            foreach(var curModel in scene.Models)
            {
                var model = (Model)curModel;
                model.ShaderProgram.Dispose();
                model.Dispose();
            }
            //var model = (Model)_mainScene.Models[0];
            //model.ReleaseUnmanagedResources();
            //model.ReleaseUnmanagedResources();
            //model.ShaderProgram.Dispose();
            //_mainScene.Models[0].Dispose();
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Title = $"FPS: {1f / e.Time:0}";
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _mainScene.Draw();
            
            SwapBuffers();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;
                case Key.Number1:
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                    break;
                case Key.Number2:
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                    break;
                default:
                    Console.WriteLine($"no {e.Key} key handler");
                    break;
            }
        }
    }
}