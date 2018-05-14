using System;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKSandbox
{
    struct Vertex2
    {
        OpenTK.Vector4 position { get; set; }
        OpenTK.Graphics.Color4 color { get; set; }

        public Vertex2(OpenTK.Vector4 position, OpenTK.Graphics.Color4 color)
        {
            this.position = position;
            this.color = color;
        }

        float X() => position.X;
        float Y() => position.Y;
        float Z() => position.Z;
        float W() => position.W;
        float R() => color.R;
        float G() => color.G;
        float B() => color.B;
        float A() => color.A;

        public const int size = (sizeof(float) * 4) * 2;
    }


    class ModelV3 : IModel
    {
        int VAO;
        int VBO;
        int IBO;
        public Vertex2[] vertices;
        public uint[] indices;
        ShaderProgram shaderProgram;

        public ModelV3(ShaderProgram shaderProgram, Vertex2[] vertices, uint[] indices)
        {
            this.vertices = vertices;
            this.indices = indices;
            this.shaderProgram = shaderProgram;

            VAO = GL.GenVertexArray();

            VBO = GL.GenBuffer();
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            //GL.NamedBufferStorage(VBO, Vertex.size * vertices.Length, vertices, BufferStorageFlags.MapWriteBit);

            IBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.NamedBufferStorage(IBO, sizeof(uint) * indices.Length, indices, BufferStorageFlags.MapWriteBit);

            //position
            GL.VertexArrayAttribBinding(VAO, 0, 0);
            GL.EnableVertexArrayAttrib(VAO, 0);
            GL.VertexArrayAttribFormat(VAO, 0, 4, VertexAttribType.Float, false, 0);

            //color
            GL.VertexArrayAttribBinding(VAO, 1, 0);
            GL.EnableVertexArrayAttrib(VAO, 1);
            GL.VertexArrayAttribFormat(VAO, 1, 4, VertexAttribType.Float, false, 4 * sizeof(float));

            //GL.VertexArrayVertexBuffer(VAO, 0, VBO, IntPtr.Zero, Vertex.size);
        }

        public ModelV3(ShaderProgram shaderProgram, Vertex[] vertices, uint[] indices)
        {
            
        }

        public void Dispose()
        {
        }

        public void Draw()
        {
            GL.BindVertexArray(VAO);
            GL.UseProgram(shaderProgram.Id);
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
