using System;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKSandbox
{
    public class ModelException : ApplicationException
    {
        public ModelException() { }
        public ModelException(string message) : base(message) { }
    }

    struct Vertex
    {
        OpenTK.Vector4 Position { get; set; }
        OpenTK.Graphics.Color4 Color { get; set; }

        public Vertex(OpenTK.Vector4 position, OpenTK.Graphics.Color4 color)
        {
            this.Position = position;
            this.Color = color;
        }

        float X() => Position.X;
        float Y() => Position.Y;
        float Z() => Position.Z;
        float W() => Position.W;
        float R() => Color.R;
        float G() => Color.G;
        float B() => Color.B;
        float A() => Color.A;

        public const int size = (sizeof(float) * 4) * 2;
    }
    enum ModelState { NoInitialized, Initialized, Deleted}

    class ModelV3 : IModel
    {
        Vertex[] Vertices { get; set; }
        uint[] Indices { get; set; }
        ShaderProgram ShaderProgram { get; }
        int VAO;
        int VBO;
        int IBO;
        ModelState ModelState = ModelState.NoInitialized;

        public ModelV3(ShaderProgram shaderProgram, Vertex[] vertices, uint[] indices)
        {            
            this.Vertices = vertices;
            this.Indices = indices;
            this.ShaderProgram = shaderProgram;

            VAO = GL.GenVertexArray();

            VBO = GL.GenBuffer();
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertex.size * Vertices.Length, Vertices, BufferUsageHint.StaticDraw);
            //GL.NamedBufferStorage(VBO, Vertex.size * vertices.Length, vertices, BufferStorageFlags.MapWriteBit); the same

            IBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * Indices.Length, Indices, BufferUsageHint.StaticDraw);
            //GL.NamedBufferStorage(IBO, sizeof(uint) * indices.Length, indices, BufferStorageFlags.MapWriteBit); the same

            //position
            GL.VertexArrayAttribBinding(VAO, 0, 0);
            GL.EnableVertexArrayAttrib(VAO, 0);
            GL.VertexArrayAttribFormat(VAO, 0, 4, VertexAttribType.Float, false, 0);

            //color
            GL.VertexArrayAttribBinding(VAO, 1, 0);
            GL.EnableVertexArrayAttrib(VAO, 1);
            GL.VertexArrayAttribFormat(VAO, 1, 4, VertexAttribType.Float, false, 4 * sizeof(float));

            GL.VertexArrayVertexBuffer(VAO, 0, VBO, IntPtr.Zero, Vertex.size);
            ModelState = ModelState.Initialized;
        }

        public void Update(Vertex[] vertices, uint[] indices)
        {

        }

        public void Update()//not tested
        {
            if (ModelState != ModelState.Initialized)
                throw new ModelException("trying to update non initialized model");
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertex.size * Vertices.Length, Vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * Indices.Length, Indices, BufferUsageHint.StaticDraw);

            GL.VertexArrayVertexBuffer(VAO, 0, VBO, IntPtr.Zero, Vertex.size);
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(VAO);
            GL.DeleteBuffers(2, new[] { VBO, IBO });
            ModelState = ModelState.Deleted;
        }

        public void Draw()
        {
            if (ModelState != ModelState.Initialized)
                throw new ModelException("trying to draw non initialized model");
            GL.BindVertexArray(VAO);
            GL.UseProgram(ShaderProgram.Id);
            GL.DrawElements(BeginMode.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        ~ModelV3()
        {
            if(ModelState != ModelState.Deleted)
                Dispose();
        }
    }
}
