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
        private int indicesLength = 0;

        public ShaderProgram shaderProgram { get; }
        public List<int> TextureIds { get; private set; }

        public Model(ShaderProgram shaderProgram, float[] vertices, params int[] structure)
        {
            this.shaderProgram = shaderProgram;
            
            GenerateVAO();

            GenerateVBO(vertices);

            LinkVertexAttributes(structure);
            
            GL.BindVertexArray(0);
        }

        public Model(ShaderProgram shaderProgram, float[] vertices, uint[] indices, params int[] structure)
        {
            this.shaderProgram = shaderProgram;

            GenerateVAO();

            GenerateVBO(vertices);

            LinkVertexAttributes(structure);
            
            GenerateEBO(indices);

            indicesLength = indices.Length;
            
            GL.BindVertexArray(0);
        }

        public void SetTextureIds(params int[] textureIds)
        {
            if (textureIds.Length == 0)
                throw new ArgumentException("No texture ids passed!");
            TextureIds = new List<int>(textureIds);
        }

        public void Use()
        {
            GL.BindVertexArray(VAO);

            GL.UseProgram(shaderProgram.Id);

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
            GL.DrawElements(PrimitiveType.Triangles, indicesLength, DrawElementsType.UnsignedInt,0);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        private void GenerateVAO()
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
        }

        private void GenerateVBO(float[] vertices)
        {
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);
        }

        private void GenerateEBO(uint[] indices)
        {
            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);
        }

        private void LinkVertexAttributes(int[] structure)
        {   
            var structureLength = structure.Length;
            if (structureLength == 0)
                throw new ArgumentException("No vertex attributes passed!");

            var stride = structure.Sum();
            var offset = 0;
            
            for (var index = 0; index < structureLength; index++)
            {
                GL.EnableVertexAttribArray(index);
                GL.VertexAttribPointer(index, structure[index], VertexAttribPointerType.Float, false,
                    stride * sizeof(float), offset * sizeof(float));
                offset += structure[index];
            }
        }


        private void ReleaseUnmanagedResources()
        {
            if (VAO != 0)
                GL.DeleteVertexArray(VAO);
            if (VBO != 0)
                GL.DeleteBuffer(VBO);
            if (EBO != 0)
                GL.DeleteBuffer(EBO);
        }
        
        ~Model() => ReleaseUnmanagedResources();
    }
}