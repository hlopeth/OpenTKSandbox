using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKSandbox
{
    public class Model : IDisposable, IModel
    {
        public int VAO { get; private set; }
        public int VBO { get; private set; }
        public int EBO { get; private set; }

        public IVertexData VertexData { get; }
        public uint[] Indices { get; }

        public ShaderProgram ShaderProgram { get; }
        public List<int> TextureIds { get; private set; }

        public Model(ShaderProgram shaderProgram, IVertexData vertexData)
        {
            this.ShaderProgram = shaderProgram;
            
            VertexData = vertexData;
            GenerateVAO();
            GenerateVBO();
            LinkVertexAttributes(VertexData.Structure);
            GL.BindVertexArray(0);
        }

        public Model(ShaderProgram shaderProgram, Vertex[] vertices, uint[] indices)
        {
            ShaderProgram = shaderProgram;
            VertexData = new VertexData(vertices);
            GenerateVAO();
            GenerateVBO();
            LinkVertexAttributes(VertexData.Structure);
            Indices = indices;
            GenerateEBO();
            GL.BindVertexArray(0);
        }

        private void GenerateVAO()
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
        }

        private void GenerateVBO()
        {
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexData.Size, VertexData.GetBufferData(), BufferUsageHint.StaticDraw);   
        }

        private void GenerateEBO()
        {
            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);
        }
        
        private void LinkVertexAttributes(int[] structure)
        {   
            var stride = structure.Sum();
            var offset = 0;
            
            for (var index = 0; index < structure.Length; index++)
            {
                GL.EnableVertexAttribArray(index);
                GL.VertexAttribPointer(index, structure[index], VertexAttribPointerType.Float, false,
                    stride * sizeof(float), offset * sizeof(float));
                offset += structure[index];
            }
        }

        public void SetTextureIds(params int[] textureIds)
        {
            if (textureIds.Length == 0)
                throw new ArgumentException("No texture ids passed!");
            TextureIds = new List<int>(textureIds);
        }

        private void Use()
        {
            GL.BindVertexArray(VAO);

            GL.UseProgram(ShaderProgram.Id);

            if (TextureIds == null) return;
            foreach (var textureId in TextureIds)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + textureId);
                GL.BindTexture(TextureTarget.Texture2D, textureId);
            }
        }

        public void Draw()
        {
            Use();
            
            if (Indices == null)
                GL.DrawArrays(PrimitiveType.Triangles, 0, VertexData.Count);
            else
                GL.DrawElements(BeginMode.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        bool disposed = false;
        public void ReleaseUnmanagedResources()
        {
            if (!disposed)
            {
                if (VAO != 0)
                    GL.DeleteVertexArray(VAO);
                if (VBO != 0)
                    GL.DeleteBuffer(VBO);
                if (EBO != 0)
                    GL.DeleteBuffer(EBO);
                disposed = true;
            }
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

    }
}